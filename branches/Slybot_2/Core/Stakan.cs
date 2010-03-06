using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using log4net;

namespace Core
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
        static ILog StakansLogger = LogWrapper.GetLogger("Stakans");

        static Dictionary<string, Stakan> Items = new Dictionary<string, Stakan>();

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

            if (sw.ElapsedMilliseconds > 300)
                Util.PerformanceLogger.ErrorFormat("Stakan of {0} update took {1} milliseconds.", instrument, sw.ElapsedMilliseconds);
        }

        static readonly int readDepth = 10;
        static readonly int printDepth = 5;

        private Stakan(string instrument)
        {
            Instrument = instrument;

            string queryBid = string.Format(
                @"select top {0} Price, Bid, MyBid from stakan_{1}
                         where bid > 0
                         order by price desc", readDepth, Instrument);

            string queryAsk = string.Format(
                @"select top {0} Price, Ask, MyAsk from stakan_{1}
                         where ask > 0
                         order by price asc",
            readDepth, Instrument);

            Bids = new List<StakanOrder>();
            foreach (var item in DataExtractor.QueryData(queryBid, true))
            {
                double price = (double)(decimal)item[0];
                int contracts = (int)(decimal)item[1];
                int myContracts = (int)(decimal)item[2];

                Bids.Add(new StakanOrder(){
                    Price = price,
                    Quantity = contracts - myContracts,
                    MyQuantity = myContracts,
                    Time = DateTime.Now
                });
            }

            Asks = new List<StakanOrder>();
            foreach (var item in DataExtractor.QueryData(queryAsk))
            {
                double price = (double)(decimal)item[0];
                int contracts = (int)(decimal)item[1];
                int myContracts = (int)(decimal)item[2];

                Asks.Add(new StakanOrder()
                {
                    Price = price,
                    Quantity = contracts - myContracts,
                    MyQuantity = myContracts,
                    Time = DateTime.Now
                });
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
