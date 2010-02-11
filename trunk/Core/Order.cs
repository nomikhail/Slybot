using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace Core
{
    public class Order
    {
        public int QuikCorrelationId { get; set; }
        public string SecCode { get; set; }
        public char Operation { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int LifeTime { get; set; }
        public bool AutoKill { get; set; }

        private int? code = null;

        List<double> executedPrices = new List<double>();
        IEnumerable<double> ExecutedPrices
        {
            get { return executedPrices; }
        }

        string error = null;
        public string Error { get { return error; } }


        private Order()
        {
            QuikCorrelationId = GetNewCorrelationId();

            LifeTime = 500;
            AutoKill = true;

            lifeThread = new Thread(new ThreadStart(LifeCycle));
            lifeThread.Start();
        }
        
        public Order(string instrument, char operation, double price) : this(instrument, operation, price, 1)
        { }

        public Order(string instrument, char operation, double price, int quantity)
            : this()
        {
            SecCode = instrument;
            Operation = operation;
            Price = price;
            Quantity = quantity;
        }


        public Order(Order previousOrder) : this()
        {
            SecCode = previousOrder.SecCode;
            Operation = previousOrder.Operation;

            if (Operation == 'B' || Operation == 'b')
                Price = DataExtractor.GetAsk(SecCode, 0);
            else
                Price = DataExtractor.GetBid(SecCode, 0);

            Quantity = previousOrder.Quantity;
        }
        
        static int globalCorellationId = new Random().Next(50000);

        static int GetNewCorrelationId()
        {
            globalCorellationId++;
            return globalCorellationId;
        }

        #region Lifecycle

        public bool processedExecuted = false;
        public bool ProcessedExecuted { get { return processedExecuted; } }

        private AutoResetEvent startMonitoring = new AutoResetEvent(false);

        static readonly int quikAnswerWaitingTime = 60;
        public void LifeCycle()
        {
            try
            {
                startMonitoring.WaitOne();

                DateTime time = DateTime.Now;

                while (StateOld == OrderStateOld.NoCode || StateOld == OrderStateOld.NotFoundInOrders)
                {
                    Thread.Sleep(50);

                    if ((DateTime.Now - time).TotalSeconds > quikAnswerWaitingTime)
                    {
                        error = "No answer from quik was found after " + quikAnswerWaitingTime + " seconds of waiting.";
                        break;
                    }
                }

                if (AutoKill && StateOld == OrderStateOld.Active)
                {
                    Thread.Sleep(LifeTime);

                    if (StateOld == OrderStateOld.Active)
                        Kill();
                }

                OrderStateOld state = StateOld;

                while (state != OrderStateOld.Executed && state != OrderStateOld.Killed && state != OrderStateOld.Error)
                {
                    Thread.Sleep(50);
                    state = StateOld;
                }

                if (state == OrderStateOld.Executed)
                {
                    processedExecuted = true;
                    Portfolio.ProcessExecutedOrder(this);
                }
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }
        Thread lifeThread = null;

        #endregion

        public static void TransactionsReply(int transactionResult,
            int transactionExtendedErrorCode, int transactionReplyCode,
            uint transactionID, double orderNumber, string transactionReplyMessage)
        {
            Order order = null;
            OrdersWithoutCode.TryGetValue((int)transactionID, out order);

            if (order != null)
            {
                if (transactionResult == 0 && transactionReplyCode == 3)
                {
                    order.code = (int)orderNumber;
                    OrdersWithoutCode.Remove((int)transactionID);
                    ActiveOrders[order.code.Value] = order;
                }
                else
                {
                    order.error = transactionReplyMessage;
                }
                order.startMonitoring.Set();
            }

            //transactionId > 0 && order == null
            //    throw new Exception(string.Format("Transaction not found for reply. (TransId={0}; Result={1}; Message={2})",
            //        transactionID, transactionResult, transactionReplyMessage));


            //if (transactionReplyMessage.Contains("Снятое количество") ||
            //    transactionReplyMessage.Contains("Вы не можете снять данную заявку") ||
            //    transactionReplyMessage.Contains("Не найдена заявка для удаления")) //||
            //    return;

            //transactionReplyMessage.Contains("Указанная транзакция по указанному классу не найдена") ||
            //transactionReplyMessage.Contains("Указанный класс не найден"))

            //@"Recieve <- (0; 0; 2; 19479; 0; ""No gate for SPBFUT589000\"")";

            // reasonoable transaction id and following message:
            // "Communication gate is down"
        }

        

        #region Push, Kill

        public string GetQuikString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ACTION=NEW_ORDER; ");
            builder.Append("CLASSCODE=SPBFUT; ");
            builder.Append(string.Format("SECCODE={0}; ", SecCode));
            builder.Append(string.Format("OPERATION={0}; ", Operation));
            builder.Append(string.Format("PRICE={0}; ", Price.ToString(Settings.enUsCulture)));
            builder.Append(string.Format("QUANTITY={0}; ", Quantity));
            builder.Append(string.Format("TRANS_ID={0}; ", QuikCorrelationId));

          //builder.Append("ACCOUNT=SPBFUT0028L; ");
          //builder.Append("CLIENT_CODE=SPBFUT0028L; ");
            builder.Append("ACCOUNT=SPBFUT00ML9; ");
            builder.Append("CLIENT_CODE=SPBFUT00ML9; ");
            

            builder.Append("TYPE=L; ");
            //builder.Append("EXECUTION_CONDITION=FILL_OR_KILL; ");            

            return builder.ToString();
        }

        // key is TransactionId
        static Dictionary<int, Order> OrdersWithoutCode = new Dictionary<int, Order>();
        // key is code
        static Dictionary<int, Order> ActiveOrders = new Dictionary<int, Order>();

        public void Push()
        {
            if (!QuikManager.PushOrder(GetQuikString()))
            {
                error = "Could not push.";
            }

            startMonitoring.Set();
            OrdersWithoutCode.Add(QuikCorrelationId, this);

            State = OrderState.Sent;
        }

        public void Kill()
        {
            var state = StateOld;

            if (state == OrderStateOld.NotFoundInOrders || state == OrderStateOld.NoCode || state == OrderStateOld.Error)
                throw new Exception("Attempt to kill not returned order");

            string killString = string.Format(
                "CLASSCODE=SPBFUT; SECCODE={0}; TRANS_ID={1}; ACTION=KILL_ORDER; ORDER_KEY={2}; ",
                SecCode, GetNewCorrelationId(), code);

            QuikManager.PushOrder(killString);
            State = OrderState.Killing;
        }

        # endregion

        #region State
        
        // passive state 
        public enum OrderState
        {
            Created,
            Sent,
            Active,
            Moving,
            Killing,
            Finished,
            Error
        }

        void LogStateChange(OrderState oldState, OrderState newState)
        {
            var strbld = new StringBuilder("Order ");
            if (code.HasValue)
                strbld.AppendFormat("(code={0})", code.Value);
            else
                strbld.AppendFormat("(transId={0})", QuikCorrelationId);
            strbld.Append(" state changed: '");
            strbld.Append(oldState.ToString());
            strbld.Append("' -> '");
            strbld.Append(newState.ToString());
            strbld.Append("'.");

            Util.TemporaryLogger.Debug(strbld.ToString());
        }

        OrderState state = OrderState.Created;
        public OrderState State
        {
            get { return state; }
            set
            {
                //LogStateChange(state, value);
                state = value;
            }
        }

        public enum OrderStateOld
        {
            NoCode,
            NotFoundInOrders,
            Active,
            Executed,
            Killed,
            Error
        }

        public OrderStateOld StateOld
        {
            get
            {
                if (error != null)
                    return OrderStateOld.Error;

                if (code == null)
                    return OrderStateOld.NoCode;

                var data = DataExtractor.QueryData("select [State] from Orders where [Code] = " + code);

                if(data.Count == 0)
                    return OrderStateOld.NotFoundInOrders;

                string state = (string)data[0][0];

                if (state.Equals("Активна"))
                    return OrderStateOld.Active;

                if (state.Equals("Исполнена"))
                    return OrderStateOld.Executed;

                if (state.Equals("Снята"))
                    return OrderStateOld.Killed;

                throw new Exception("Unknown order state: " + state);
            }
        }

        #endregion

        #region Counters

        public static int TotalExecuted
        {
            get 
            {
                return (int)DataExtractor.QueryData(
                    "select count(*) from orders where state = 'Исполнена'")[0][0];
            }
        }

        public static int TotalKilled
        {
            get
            {
                return (int)DataExtractor.QueryData(
                    "select count(*) from orders where state = 'Снята'")[0][0];
            }
        }

        public static int TotalActive
        {
            get
            {
                return (int)DataExtractor.QueryData(
                    "select count(*) from orders where state = 'Активна'")[0][0];
            }
        }

        #endregion


        internal static void ProcessNewTransaction(Transaction transaction)
        {
            Order order = null;

            if (!ActiveOrders.TryGetValue(transaction.OrderCode, out order))
            {
                Util.SystemLogger.ErrorFormat("Order with code {0} not found for transaction with number {1}.",
                    transaction.OrderCode, transaction.Number);
                return;
            }

            double price = (double)transaction.Price;
            for (int i = 0; i < transaction.Quantity; ++i)
                order.executedPrices.Add(price);
        }
    }
}
