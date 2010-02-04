using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace Core
{
    public class Portfolio
    {
        static ILog logger = LogWrapper.GetLogger("Portfolio");

        static readonly double unacceptableThreshold = 10000;

        public static bool IsPosIncreaseProhibited(int pos)
        {
            if (Settings.exitPortfolio)
                return true;

            if (Math.Abs(pos) > Settings.maxPos)
                return true;

            double freeMoney = DataExtractor.GetFreeMoneyAmount();

            return freeMoney < Settings.minFreeMoney;
        }

        public static double GetBuyThreshold()
        { return GetBuyThreshold(EurRur); }
        public static double GetBuyThreshold(int pos) // pos means eurRur
        {
            if (EurRur >= 0 && IsPosIncreaseProhibited(pos))
                return unacceptableThreshold;

            return (25.0 / Settings.maxPos) * pos + 25.0;
        }

        public static double GetSellThreshold()
        { return GetSellThreshold(EurRur); }
        public static double GetSellThreshold(int pos)
        {
            if (EurRur <= 0 && IsPosIncreaseProhibited(pos))
                return unacceptableThreshold;

            return  (-25.0 / Settings.maxPos) * pos + 25.0;
        }

        static int usdRur;
        static int eurRur;
        static int eurUsd;
        static DateTime lastSynchronized = DateTime.Now - new TimeSpan(1, 0, 0);

        public static int UsdRur { get { return usdRur; } }
        public static int EurRur { get { return eurRur; } }
        public static int EurUsd { get { return eurUsd; } }

        public static int Postition(string secCode)
        {
            switch (secCode)
            {
                case Settings.Si_instrument:
                    return usdRur;

                case Settings.Eu_instrument:
                    return eurRur;

                case Settings.ED_instrument:
                    return eurUsd;

                default:
                    throw new Exception("Unknown security code: " + secCode);
            }
        }


        static bool equityReported = true;

        public static void SynchronizePosition()
        {
            var lastSyncSeconds = (DateTime.Now - lastSynchronized).TotalSeconds;

            bool suspectForZeroPortfolio = (eurRur == 0 || eurUsd == 0 || usdRur == 0) && lastSyncSeconds > 3;

            if (lastSyncSeconds < 30 && !suspectForZeroPortfolio)
                return;

            int times = 0;
            while (times < 5)
            {
                eurRur = RealEurRur;
                eurUsd = RealEurUsd;
                usdRur = RealUsdRur;

                if (eurRur != -999 && eurUsd != -999 && usdRur != -999)
                    break;

                Thread.Sleep(1000);
                times++;
            }

            if (eurRur == 0 && eurUsd == 0 && usdRur == 0)
            {
                if (!equityReported)
                {
                    logger.Info("Zero portfolio detected. Current equity: " + DataExtractor.GetEquity(true) + ".");
                    equityReported = true;
                }
            }
            else
            {
                equityReported = false;
            }

            ResetSyncFlag();
        }

        static void ResetSyncFlag() { lastSynchronized = DateTime.Now; }

        public static void ProcessExecutedOrder(Order order)
        {
            int posChange = (int)order.Quantity * ((order.Operation == 'B' || order.Operation == 'b') ? 1 : -1);

            switch (order.SecCode)
            {
                case Settings.Si_instrument:
                    usdRur += posChange;
                    break;

                case Settings.Eu_instrument:
                    eurRur += posChange;
                    break;

                case Settings.ED_instrument:
                    eurUsd += posChange;
                    break;

                default:
                    throw new Exception("Unknown security code: " + order.SecCode);
            }

            ResetSyncFlag();
        }


        public static int RealUsdRur
        {
            get 
            {
                var dataItem = DataExtractor.QueryData(string.Format(
                    "select Position from Portfolio where Instrument = '{0}'",
                    Settings.Si_instrument)).FirstOrDefault();

                if (dataItem == null)
                    return 0;

                return (int)dataItem[0];
            }
        }

        public static int RealEurRur
        {
            get
            {
                var dataItem = DataExtractor.QueryData(string.Format(
                    "select Position from Portfolio where Instrument = '{0}'",
                    Settings.Eu_instrument)).FirstOrDefault();

                if (dataItem == null)
                    return 0;

                return (int)dataItem[0];
            }
        }

        public static int RealEurUsd
        {
            get
            {
                var dataItem = DataExtractor.QueryData(string.Format(
                    "select Position from Portfolio where Instrument = '{0}'",
                    Settings.ED_instrument)).FirstOrDefault();

                if (dataItem == null)
                    return 0;

                return (int)dataItem[0];
            }
        }

        public static string GetReport()
        {
            int usdRur = UsdRur;
            int eurRur = EurRur;
            int eurUsd = EurUsd;

            if (eurRur != -999 && eurUsd != -999 && usdRur != -999)
            {
                StringBuilder builder = new StringBuilder("Pos:");
                builder.Append(eurRur);

                double eurUsdPrice = (DataExtractor.GetBid(Settings.ED_instrument, 0) + DataExtractor.GetAsk(Settings.ED_instrument, 0)) / 2;
                double dollarPos = eurRur * eurUsdPrice + usdRur;
                builder.Append("    $:" + dollarPos.ToString("F", Settings.culture));

                if (eurRur + eurUsd != 0)
                    builder.Append(string.Format("  (€/$: {0})", eurRur + eurUsd));

                return builder.ToString();
            }
            else
                return "No VALID DATA!";
        }
    }
}
