using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Core
{
    public class Settings
    {
        public static readonly double shift = 10; //-15;

        #region Curve Params

        public static readonly double arg_mult = 0.08;
        public static readonly double arg_add = -1.7;
        public static readonly double mult = 4.0;
        public static readonly double add = 30;
        public static readonly double Balance_mult = 0.5;
        public static readonly double Balance_add = 0;

        public static readonly double TrailingOrderBonus = 12.0;
        public static readonly double MaxLooseToMarket = 10.0;
        public static readonly double Actual2TargetPriceDeviation = 12.0;

        public static readonly IFormatProvider culture = new CultureInfo("en-US");

        #endregion

        public static readonly int maxPos = 170;
        public static readonly double minFreeMoney = 15000;
        public static readonly double rebalanceFreeMoneyThreshold = 30000;

        public static bool exitPortfolio = false;

        public const string Si_instrument = "SiH0";
        public const string Eu_instrument = "EuH0";
        public const string ED_instrument = "EDH0";

        public static readonly string quikDir = @"D:\!\QUIK_BCS\";
        public static readonly string skypeAgentDir = @"D:\!\SkypeAgent\";

        public static TimeSpan morningStart = new TimeSpan(10, 25, 0), initiationFinish = new TimeSpan(10, 27, 0),
                        dayClearingStart = new TimeSpan(14, 0, 0),
                        dayClearingEnd = new TimeSpan(14, 3, 0),
                        eveClearingStart = new TimeSpan(18, 45, 0),
                        eveClearingEnd = new TimeSpan(19, 0, 0);

        public static TimeSpan tradingStart = new TimeSpan(10, 30, 20),
                        tradingEndBeforeDayClearing = new TimeSpan(13, 59, 40),
                        tradingStartAfterDayClearing = new TimeSpan(14, 03, 20),
                        tradingEndBeforeEveClearing = new TimeSpan(18, 44, 40),
                        tradingStartAfterEveClearing = new TimeSpan(19, 00, 20),
                        tradingEnd = new TimeSpan(23, 49, 30);
    }
}
