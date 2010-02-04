using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SqlClient;

namespace Core
{
    public enum StatusResponseContext
    {
        Ok,
        Error,
        Info      
    }

    public interface StatusCheckResponseProcessor
    {
        void Response(string message, bool isOk);
    }

    public class StatusCheck
    {
        int eurRur = Portfolio.RealEurRur;
        int eurUsd = Portfolio.RealEurUsd;
        int usdRur = Portfolio.RealUsdRur;

        public void MakeStatusCheck(StatusCheckResponseProcessor processor)
        {
            if (!Util.IsOperationTime())
            {
                processor.Response("Current time is not operational.", true);
                return;
            }

            if (Process.GetProcessesByName("UI").Length > 0)
                processor.Response("Slybot process OK.", true);
            else
            {
                processor.Response("Slybot process not found.", false);
                return;
            }


            int lastQuikSeconds = (int)(DateTime.Now - DataExtractor.GetLastQuikSuccess()).TotalSeconds;
            if (lastQuikSeconds < 10)
                processor.Response("Quik connection OK.", true);
            else
            {
                processor.Response("Quik not connected for " + lastQuikSeconds + " seconds.", false);
                return;
            }

            int eurBalance = eurRur + eurUsd;
            if (eurBalance == 0)
                processor.Response(string.Format("{0} and {1} balance OK.", Settings.Eu_instrument, Settings.ED_instrument), true);
            else
                processor.Response(string.Format("{0}({1}) and {2}({3}) not balanced.", Settings.Eu_instrument, eurRur, Settings.ED_instrument, eurUsd), false);

            double eurUsdPrice = (DataExtractor.GetBid(Settings.ED_instrument, 0) + DataExtractor.GetAsk(Settings.ED_instrument, 0)) / 2;
            double dollarPos = eurRur * eurUsdPrice + usdRur;
            if (Math.Abs(dollarPos) < 0.5)
                processor.Response(Settings.Si_instrument + " exposure OK.", true);
            else
                processor.Response(Settings.Si_instrument + " exposure: " + dollarPos.ToString("F", Settings.culture) + ".", false);
        }
        
        public void MakeStatusInfo(StatusCheckResponseProcessor processor)
        {
            Market market = Market.QueryMarket();
            processor.Response("arb(thr)", false);
            processor.Response(string.Format("Buy:{0}({1})", (int)market.PointsBuyEur, (int)Portfolio.GetBuyThreshold(eurRur)), false);
            processor.Response(string.Format("Sell:{0}({1})", (int)market.PointsSellEur, (int)Portfolio.GetSellThreshold(eurRur)), false);

            processor.Response("Portfolio", false);
            processor.Response(Settings.Eu_instrument + ":" + eurRur, false);
            processor.Response(Settings.ED_instrument + ":" + eurUsd, false);
            processor.Response(Settings.Si_instrument + ":" + usdRur, false);

            processor.Response("Equity", false);
            processor.Response("current" + ":" + DataExtractor.GetEquity(false), false);
            processor.Response("free" + ":" + DataExtractor.GetFreeMoneyAmount(), false);

            QuikDataContext dataContext = new QuikDataContext();
            var activeOrders = dataContext.QuikOrders.Where(order => order.State.Equals("Активна")).
                   Select(order => order.SecCode + ":" + order.Operation.Substring(0, 3) + " " + order.Quantity).ToList();

            if (activeOrders.Count > 0)
            {
                processor.Response("Orders", false);
                foreach (var order in activeOrders)
                    processor.Response(order, false);
            }
            else
                processor.Response("No orders", false);

        }
    }
}
