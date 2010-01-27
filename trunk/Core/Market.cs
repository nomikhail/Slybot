using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Core
{
    public class Market
    {
        public static Market GetBlankMarket()
        {
            return new Market();
        }

        public static Market QueryMarket()
        {
            return QueryFromStakan(0);
        }

        private static readonly Regex regex        = new Regex(@"(\S+): .+ \(([\d\.]+),\d+\)  #  \(([\d\.]+),\d+\) .+$");
        private static readonly Regex regexNodData = new Regex(@"(\S+):\s+#\s+$");

        public void ProcessLogMessage(Log logMsg)
        {
            string instrument;
            double bid; double ask;

            var match = regex.Match(logMsg.Message);
            if (match.Success)
            {
                instrument = match.Groups[1].Captures[0].Value;
                bid = double.Parse(match.Groups[2].Captures[0].Value);
                ask = double.Parse(match.Groups[3].Captures[0].Value);
            }
            else
            {
                var noDataMatch = regexNodData.Match(logMsg.Message);
                if (noDataMatch.Success)
                {
                    instrument = noDataMatch.Groups[1].Captures[0].Value;
                    bid = ask = double.MinValue;
                }
                else
                    return;
                //    throw new Exception("Non-stakan log message: " + logMsg.Message);
            }


            if (instrument == Settings.Si_instrument)
            {
                UsdBid = bid;
                UsdAsk = ask;
            }
            else if (instrument == Settings.Eu_instrument)
            {
                EurBid = bid;
                EurAsk = ask;
            }
            else if (instrument == Settings.ED_instrument)
            {
                EurUsdBid = bid;
                EurUsdAsk = ask;
            }
            else
                throw new Exception("Unknown instrument: " + instrument);
        }

        public static Market QueryFromStakan(int liquidityInsurance)
        {
            Market market = new Market();

            market.UsdBid = DataExtractor.GetBid(Settings.Si_instrument, liquidityInsurance);
            market.UsdAsk = DataExtractor.GetAsk(Settings.Si_instrument, liquidityInsurance);

            market.EurBid = DataExtractor.GetBid(Settings.Eu_instrument, 0);
            market.EurAsk = DataExtractor.GetAsk(Settings.Eu_instrument, 0);

            market.EurUsdBid = DataExtractor.GetBid(Settings.ED_instrument, liquidityInsurance);
            market.EurUsdAsk = DataExtractor.GetAsk(Settings.ED_instrument, liquidityInsurance);

            market.Time = DateTime.Now;

            return market;
        }

        private Market()
        {
            UsdBid = double.MinValue;
            UsdAsk = double.MinValue;
            EurBid = double.MinValue;
            EurAsk = double.MinValue;
            EurUsdBid = double.MinValue;
            EurUsdAsk = double.MinValue;
        }

        public double UsdBid { get; set; }
        public double UsdAsk { get; set; }

        public double EurBid { get; set; }
        public double EurAsk { get; set; }

        public double EurUsdBid { get; set; }
        public double EurUsdAsk { get; set; }

        public DateTime Time { get; set; }

        public bool IsFull()
        {
            if (UsdBid == double.MinValue || UsdBid == 0)
                return false;
            if (UsdAsk == double.MinValue || UsdAsk == 0)
                return false;
            if (EurBid == double.MinValue || EurBid == 0)
                return false;
            if (EurAsk == double.MinValue || EurAsk == 0)
                return false;
            if (EurUsdBid == double.MinValue || EurUsdBid == 0)
                return false;
            if (EurUsdAsk == double.MinValue || EurUsdAsk == 0)
                return false;

            return true;
        }


        public double BidBuyEur(double profitPoints)
        {
            return UsdBid * EurUsdBid - profitPoints;
        }
        public double PointsBuyEur
        {
            get
            {
                return UsdBid * EurUsdBid - EurAsk; 
            }
        }

        public double AskSellEur(double profitPoints)
        {
            return profitPoints + UsdAsk * EurUsdAsk;
        }
        public double PointsSellEur
        {
            get
            {
                return EurBid - UsdAsk * EurUsdAsk;
            }
        }
    }
}
