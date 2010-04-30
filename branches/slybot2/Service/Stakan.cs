using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Diagnostics;
using log4net;

namespace Service
{
    [DebuggerDisplay("Order({Price}, {Quantity}, {MyQuantity})")]
    public struct StakanOrder
    {
        public double Price;
        public int Quantity;
        public int MyQuantity;
        public DateTime Time;
    }


    public class Stakan
    {
        static readonly ILog StakansLogger = LogManager.GetLogger("Stakans");

        static Dictionary<string, Stakan> Items = new Dictionary<string, Stakan>();

        public static SQLiteConnection _connection = null;

        private static IEnumerable<object[]> QueryCmdText(string cmdText)
        {
            var command = new SQLiteCommand(cmdText, _connection);
            var reader = command.ExecuteReader();

            while (true)
            {
                var objs = new object[reader.FieldCount];

                reader.GetValues(objs);

                yield return objs;

                if (!reader.Read())
                    break;
            }

            reader.Close();
        }


        public static void UpdateStakan(string instrument)
        {
            var sw = new Stopwatch(); sw.Start();

            Stakan oldStakan = null;
            Items.TryGetValue(instrument, out oldStakan);
            
            var newStakan = new Stakan(instrument);
            Items[instrument] = newStakan;

            if (oldStakan == null || oldStakan.ToString() != newStakan.ToString())
                StakansLogger.Debug(newStakan.ToString());

            sw.Stop();

            //if (sw.ElapsedMilliseconds > 300)
            //    Util.PerformanceLogger.ErrorFormat("Stakan of {0} update took {1} milliseconds.", instrument, sw.ElapsedMilliseconds);
        }

        //static readonly int readDepth = 10;
        static readonly int printDepth = 5;

        private Stakan(string instrument)
        {
            Instrument = instrument;

            string queryBid = string.Format(
                @"select Price, Bid, MyBid from stakan_{0} where bid > 0 order by price desc", Instrument);

            string queryAsk = string.Format(
                @"select Price, Ask, MyAsk from stakan_{0} where ask > 0 order by price asc", Instrument);

            Bids = new List<StakanOrder>();
            foreach (var item in QueryCmdText(queryBid))
            {
                try
                {
                    double price = (double)item[0];
                    int contracts = (int)(long)item[1];
                    int myContracts = (int)(long)item[2];

                    Bids.Add(new StakanOrder()
                    {
                        Price = price,
                        Quantity = contracts - myContracts,
                        MyQuantity = myContracts,
                        Time = DateTime.Now
                    });
                }
                catch
                {}
            }

            Asks = new List<StakanOrder>();
            foreach (var item in QueryCmdText(queryAsk))
            {
                try
                {
                    double price = (double)item[0];
                    int contracts = (int)(long)item[1];
                    int myContracts = (int)(long)item[2];

                    Asks.Add(new StakanOrder()
                    {
                        Price = price,
                        Quantity = contracts - myContracts,
                        MyQuantity = myContracts,
                        Time = DateTime.Now
                    });
                }
                catch
                { }
            }
        }

        public string Instrument { get; set; }
        public List<StakanOrder> Bids { get; set; }
        public List<StakanOrder> Asks { get; set; }
        
        string stringView = null;
        public override string ToString()
        {
            if (stringView == null)
            {
                StringBuilder stakanStrBuilder = new StringBuilder();

                stakanStrBuilder.Append(Instrument);
                stakanStrBuilder.Append(": ");
                
                bool first = true;
                int bidStartIndex = Math.Min(printDepth, Bids.Count) - 1;
                for (int i = bidStartIndex; i >= 0; --i)
                {
                    var item = Bids[i];

                    if (!first)
                        stakanStrBuilder.Append(" ");

                    stakanStrBuilder.AppendFormat("({0},{1}", item.Price, item.Quantity + item.MyQuantity);
                    if (item.MyQuantity > 0)
                        stakanStrBuilder.AppendFormat(",{0})", item.MyQuantity);
                    else
                        stakanStrBuilder.AppendFormat(")");

                    first = false;
                }

                stakanStrBuilder.Append("  # ");

                int askStopIndex = Math.Min(printDepth, Asks.Count);
                for(int i = 0; i < askStopIndex; ++i)
                {
                    var item = Asks[i];

                    stakanStrBuilder.Append(" ");

                    stakanStrBuilder.AppendFormat("({0},{1}", item.Price, item.Quantity + item.MyQuantity);
                    if (item.MyQuantity > 0)
                        stakanStrBuilder.AppendFormat(",{0})", item.MyQuantity);
                    else
                        stakanStrBuilder.AppendFormat(")");
                }

                stringView = stakanStrBuilder.ToString();
            }

            return stringView;
        }
    }
}
