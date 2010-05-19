using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading;
using log4net;
using System.Data.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Globalization;

namespace Core
{
    public class DataExtractor
    {
        static SqlConnection con = null;
        static object conLock = new object();

        static ILog Logger = LogWrapper.GetLogger("DataExtractor");

        static DataExtractor()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "localhost";
            builder.InitialCatalog = "QUIK";
            builder.IntegratedSecurity = false;
            builder.UserID = "Misha2Kota";
            builder.Password = "2KotaSignature";

            con = new SqlConnection(builder.ConnectionString);

            con.Open();
        }

        public static void StartProcessing()
        {
            processingThread = new Thread(new ThreadStart(WorkingProc)) { CurrentCulture = Settings.enUsCulture};
            processingThread.Start();
        }

        public static IEnumerable<object[]> GetData(string table)
        {
            lock (conLock)
            {
                SqlCommand command = new SqlCommand("select * from " + table, con);
                var reader = command.ExecuteReader();
                object[] data = new object[reader.FieldCount];

                while (reader.Read())
                {
                    int res = reader.GetValues(data);
                    yield return data;
                }
                reader.Close();
            }
        }

        public static List<object[]> QueryData(string query)
        {
            return QueryData(query, false);
        }

        public static List<object[]> QueryData(string query, bool reportLockWait)
        {
            List<object[]> result = null;

            Stopwatch sw = null;
            if(reportLockWait)
            {
                sw = new Stopwatch();
                sw.Start();
            }

            lock (conLock)
            {
                if (reportLockWait)
                {
                    sw.Stop();
                    int millisecondsElaplsed = (int)sw.ElapsedMilliseconds;
                    if (millisecondsElaplsed > 100)
                        Util.PerformanceLogger.ErrorFormat("Connection lock wait elapsed {0} milliseconds.",
                            millisecondsElaplsed);
                }

                result = new List<object[]>();

                SqlCommand command = new SqlCommand(query, con);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    object[] data = new object[reader.FieldCount];
                    int res = reader.GetValues(data);
                    result.Add(data);
                }
                reader.Close();
            }

            return result;
        }

        public static object QueryOneValue(string query)
        {
            lock (conLock)
            {
                SqlCommand command = new SqlCommand(query, con);
                object result = command.ExecuteScalar();

                if (result == null)
                    throw new Exception("No value found for the query \"" + query + "\".");

                return result;
            }
        }
        
        public static double QueryNumericValue(string query)
        {
            object result = QueryOneValue(query);
            return (double)(decimal)result;
        }


        #region Equity

        static readonly string equityQuery =
@"  select sum(varmargin) as equity from portfolio
        union all
    select currentBalance as equity from futlimits
    where openbalance > 150000
        union all
    select currentPos as equity from micexLimits";


        static readonly string instantEquityQuery =
@"  select sum(varmargin) as equity from portfolio
        union all
    select currentBalance as equity from futlimits
    where openbalance > 150000";

        public static double GetEquity(bool instant)
        {
            string query = (instant) ? instantEquityQuery : equityQuery;

            var data = QueryData(query);

            double result = 0;
            foreach(var item in data)
            {
                if(item[0] != DBNull.Value)
                    result += (double)(decimal)item[0];
            }

            return Math.Round(result);
        }

        public static double GetFreeMoneyAmount()
        {
            try
            {
                return Math.Round(QueryNumericValue(
                        @"select currentbalance from futlimits where openbalance > 10000"));
            }
            catch (Exception ex)
            {
                Util.SystemLogger.Error("Free amount query fail.", ex);
                return 0;
            }
        }

        #endregion


        static readonly string lastQuikCheckSuccessQuery =
            @"select top 1 date from [log]
                where logger = 'QuikManager' and [message] = 'Quik connection check OK.'
                order by date desc";

        public static DateTime GetLastQuikSuccess()
        {
            lock (conLock)
            {
                DateTime result = DateTime.Today;
                try
                {
                    con.ChangeDatabase("SlyBot");
                    result = (DateTime)QueryOneValue(lastQuikCheckSuccessQuery);
                }
                finally
                {
                    con.ChangeDatabase("QUIK");
                }

                return result;
            }            
        }


        static List<double> ExtractStakanData(string query, int resultsCount)
        {
            List<object[]> data = QueryData(query);

            List<double> results = new List<double>();

            foreach (object[] dataItem in data)
            {
                int ordersCount = (int)(decimal)dataItem[0];
                double price = (double)(decimal)dataItem[1];

                for (int i = 0; i < ordersCount; ++i)
                {
                    results.Add(price);
                    if (results.Count >= resultsCount)
                        return results;
                }
            }

            return results;
        }

        public static List<double> GetBids(string instrument, int resultsCount)
        {
            string queryTemplate =
                @"select top {0} bid, price from stakan_{1}
                         where bid > 0
                         order by price desc";

            string query = string.Format(queryTemplate, resultsCount, instrument);

            return ExtractStakanData(query, resultsCount);
        }

        public static List<double> GetAsks(string instrument, int resultsCount)
        {
            string queryTemplate =
                @"select top {0} ask, price from stakan_{1}
                         where ask > 0
                         order by price asc";

            string query = string.Format(queryTemplate, resultsCount, instrument);

            return ExtractStakanData(query, resultsCount);
        }




        public static double GetBid(string instrument, int ordersSkip)
        {
            List<double> res = GetBids(instrument, 1 + ordersSkip);
            if (res.Count > ordersSkip)
                return res[ordersSkip];

            return double.MinValue;
        }


        public static double GetAsk(string instrument, int ordersSkip)
        {
            List<double> res = GetAsks(instrument, 1 + ordersSkip);
            if (res.Count > ordersSkip)
                return res[ordersSkip];

            return double.MinValue;
        }

        public static string GetStakanState(string instrument, int depth)
        {
            string queryBid = string.Format(
                @"select top {0} Price, Bid, MyBid from stakan_{1}
                         where bid > 0
                         order by price desc",
            depth, instrument);

            string queryAsk = string.Format(
                @"select top {0} Price, Ask, MyAsk from stakan_{1}
                         where ask > 0
                         order by price asc",
            depth, instrument);

            StringBuilder stakanStrBuilder = new StringBuilder();

            var bidResults = QueryData(queryBid);
            bool first = true;
            
            for(int i = bidResults.Count - 1; i >= 0; --i)
            {
                var item = bidResults[i];

                if (!first)
                    stakanStrBuilder.Append(" ");

                double price = (double)(decimal)item[0];
                int contracts = (int)(decimal)item[1];
                int myContracts = (int)(decimal)item[2];
                stakanStrBuilder.AppendFormat("({0},{1}", price, contracts);
                if (myContracts > 0)
                    stakanStrBuilder.AppendFormat(",{0})", myContracts);
                else
                    stakanStrBuilder.AppendFormat(")");

                first = false;
            }

            stakanStrBuilder.Append("  # ");

            foreach (var item in QueryData(queryAsk))
            {
                stakanStrBuilder.Append(" ");

                double price = (double)(decimal)item[0];
                int contracts = (int)(decimal)item[1];
                int myContracts = (int)(decimal)item[2];
                stakanStrBuilder.AppendFormat("({0},{1}", price, contracts);
                if (myContracts > 0)
                    stakanStrBuilder.AppendFormat(",{0})", myContracts);
                else
                    stakanStrBuilder.AppendFormat(")");
            }

            return stakanStrBuilder.ToString();
        }


        public static List<string> GetUpdatedData()
        {
            try
            {
                QuikDataContext quikData = new QuikDataContext();

                int countChanged = quikData.States.Count(state => state.isUpdated);

                if (countChanged == 0)
                    return null;

                lock (conLock)
                {
                    List<string> result = new List<string>();

                    var updatedData = quikData.States.Where(state => state.isUpdated).ToList();
                    updatedData.ForEach(elem => elem.isUpdated = false);
                    quikData.SubmitChanges();
                    quikData.Refresh(RefreshMode.OverwriteCurrentValues, quikData.States);


                    return updatedData.Select(elem => elem.entity).ToList();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("deadlock victim. Rerun the transaction"))
                {
                    Logger.Debug(ex);
                    Logger.Warn("Transaction was aborted.");
                    return null;
                }

                throw;
            }
        }

        static bool firstLaunch = true;
        static HashSet<int> transIdsFoundInQuik = new HashSet<int>();
        static void ProcessNewTransactions()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            int totalTransactionsFound = 0;

            QuikDataContext quikData = new QuikDataContext();
            SlyBotDataContext slyBotData = new SlyBotDataContext();

            var transIds = (from trans in quikData.Transactions
                           where trans.ClassCode != "FUTEVN"
                           select trans.Number).ToList();

            var notFoundIds = transIds.Except(transIdsFoundInQuik).ToList();

            transIdsFoundInQuik.UnionWith(notFoundIds);

            var newTransactionIds = notFoundIds.Where(transNum => !slyBotData.LoggedTransactions.Any(logged => logged.Number == transNum)).ToList();

            var newTransactions = newTransactionIds.Select(id => quikData.Transactions.Single(trans => trans.Number == id));

            foreach (var transaction in newTransactions)
            {
                string operation;
                if(transaction.Operation.Equals("Купля", StringComparison.InvariantCultureIgnoreCase))
                    operation = "B";
                else if(transaction.Operation.Equals("Продажа", StringComparison.InvariantCultureIgnoreCase))
                    operation = "S";
                else
                    throw new Exception(string.Format("Operation {0} is unknown for transaction {1}.",
                        transaction.Operation, transaction.Number));


                LoggedTransaction transToLog = new LoggedTransaction()
                {
                    Number = transaction.Number,
                    Time = DateTime.Now,
                    OrderCode = transaction.OrderCode,
                    SecCode = transaction.SecCode,
                    Operation = operation,
                    Price = transaction.Price,
                    Quantity = transaction.Quantity,
                    TimeQuik = transaction.Time,
                    Volume = transaction.Volume
                };

                // TODO: validate quik time against own time, warn if big mismatch

                slyBotData.LoggedTransactions.InsertOnSubmit(transToLog);

                Order.ProcessNewTransaction(transaction);

                Logger.DebugFormat("Found new transaction (id={0}; order={1}).", transaction.Number, transaction.OrderCode);
                ++totalTransactionsFound;
            }

            slyBotData.SubmitChanges();

            stopwatch.Stop();

            if(!firstLaunch)
            {
                if(stopwatch.ElapsedMilliseconds > 200)
                    Util.PerformanceLogger.ErrorFormat("Processing transactions: {0} milliseconds, {1} found.",
                        stopwatch.ElapsedMilliseconds, totalTransactionsFound);
            }

            firstLaunch = false;
        }

        static Thread processingThread = null;
        static void WorkingProc()
        {
            try
            {
                Util.SystemLogger.Debug("DataExtractor processing thread started.");

                ProcessNewTransactions();

                Stakan.UpdateStakan(Settings.ED_instrument);
                Stakan.UpdateStakan(Settings.Eu_instrument);
                Stakan.UpdateStakan(Settings.Si_instrument);

                while (active)
                {
                    var updatedData = GetUpdatedData();

                    if (updatedData != null)
                    {
                        if (updatedData.Contains("ed9"))
                            Stakan.UpdateStakan(Settings.ED_instrument);

                        if (updatedData.Contains("eu9"))
                            Stakan.UpdateStakan(Settings.Eu_instrument);

                        if (updatedData.Contains("si9"))
                            Stakan.UpdateStakan(Settings.Si_instrument);

                        if (updatedData.Contains("transactionChanged"))
                            ProcessNewTransactions();

                        if (updatedData.Contains("orderChanged"))
                            //Order.ProcessTableUpdate();

                        continue;
                    }

                    Thread.Sleep(1);
                }

                Util.SystemLogger.Debug("DataExtractor processing thread finished.");
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }

        private static bool active = true;
        public static void Terminate()
        {
            active = false;
        }
    }
}
