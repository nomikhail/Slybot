using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Core
{
    public class Settings
    {

        public static readonly double TrailingOrderBonus = 12.0;
        public static readonly double MaxLooseToMarket = 10.0;
        public static readonly double Actual2TargetPriceDeviation = 12.0;

        public static readonly int maxPos = 200;
        public static readonly double minFreeMoney = 30000;

        public static readonly bool exitPortfolio = false;
        public static readonly double counterExitPortfolioBonus = 10;

        public const string Si_instrument = "SiM0";
        public const string Eu_instrument = "EuM0";
        public const string ED_instrument = "EDM0";

        public static readonly string quikDir = @"D:\!\QUIK_BCS\";
        public static readonly string skypeAgentDir = @"D:\!\SkypeAgent\";

        public static TimeSpan morningStart = new TimeSpan(10, 00, 0), initiationFinish = new TimeSpan(10, 27, 0),
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

        public static readonly CultureInfo enUsCulture = new CultureInfo("en-US");
    }
}
