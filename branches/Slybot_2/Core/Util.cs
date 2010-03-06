using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Windows.Forms;
using System.Diagnostics;

namespace Core
{
    public class CandleStickDto
    {
        public CandleStickDto(DateTime time, List<double> values)
        {
            Time = time;
            Open = values.First();
            Close = values.Last();
            High = values.Max();
            Low = values.Min();
        }

        public DateTime Time { get; set; }

        public double Open { get; set; }
        public double Close { get; set; }

        public double High { get; set; }
        public double Low { get; set; }
    }

    public class Util
    {
        public static IEnumerable<CandleStickDto> GetBuyCandleSticks(DateTime startTime)
        {
            return GetCandleSticks(startTime, market => market.PointsBuyEur);
        }

        public static IEnumerable<CandleStickDto> GetSellCandleSticks(DateTime startTime)
        {
            return GetCandleSticks(startTime, market => market.PointsSellEur);
        }

        private static IEnumerable<CandleStickDto> GetCandleSticks(DateTime startTime, Func<Market, double> getValFunc)
        {
            var dataContext = new SlyBotDataContext();

            var stakanItems = from msg in dataContext.Logs
                              where msg.Date > startTime && msg.Logger == "Stakans"
                              orderby msg.Id
                              select msg;

            var market = Market.GetBlankMarket();

            List<KeyValuePair<DateTime, double>> buyValues = new List<KeyValuePair<DateTime, double>>();


            double previousBuyValue = double.MinValue;
            foreach (var stakatItem in stakanItems.Where(item => item.Date.TimeOfDay > Settings.tradingStart))
            {
                market.ProcessLogMessage(stakatItem);

                if (market.IsFull())
                {
                    double buyPoints = getValFunc(market);
                    if (previousBuyValue != buyPoints)
                    {
                        buyValues.Add(new KeyValuePair<DateTime, double>(stakatItem.Date, buyPoints));
                        previousBuyValue = buyPoints;
                    }
                }
            }

            if (buyValues.Count == 0)
                yield break;

            var calcPoint = RoundTime(buyValues[0].Key);
            List<double> currentInterval = new List<double>();
            foreach (var buyVal in buyValues)
            {
                if (RoundTime(buyVal.Key) != calcPoint)
                {
                    if (currentInterval.Count > 0)
                    {
                        yield return new CandleStickDto(calcPoint, currentInterval);
                        currentInterval.Clear();
                    }
                    calcPoint = RoundTime(buyVal.Key);
                }

                currentInterval.Add(buyVal.Value);
            }

            if (currentInterval.Count > 0)
            {
                yield return new CandleStickDto(calcPoint, currentInterval);
                currentInterval.Clear();
            }
        }


        private static DateTime RoundTime(DateTime orig)
        {
            return orig.Date.AddMinutes(Math.Round(orig.TimeOfDay.TotalMinutes));
        }


        public static ILog SystemLogger = LogWrapper.GetLogger("System");

        public static ILog PerformanceLogger = LogWrapper.GetLogger("Performance");

        public static ILog TemporaryLogger = LogWrapper.GetLogger("Temporary");
        
        static Util()
        {
        }

        public static bool IsHoliday()
        {
            DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
            return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday;
        }

        public static bool IsOperationTime()
        {
            if (IsHoliday())
                return false;

            TimeSpan nowTime = DateTime.Now.TimeOfDay;
            return (nowTime > Settings.morningStart && nowTime < Settings.dayClearingStart) ||
                 (nowTime > Settings.dayClearingEnd && nowTime < Settings.eveClearingStart) ||
                                                        nowTime > Settings.eveClearingEnd;
        }

        public static bool IsInitiationTime()
        {
            if (IsHoliday())
                return false;

            TimeSpan nowTime = DateTime.Now.TimeOfDay;
            return (nowTime > Settings.morningStart && nowTime < Settings.initiationFinish);

        }

        public static bool IsTradingTime()
        {
            if (IsHoliday())
                return false;

            TimeSpan nowTime = DateTime.Now.TimeOfDay;
            return                 (nowTime > Settings.tradingStart && nowTime < Settings.tradingEndBeforeDayClearing) ||
                   (nowTime > Settings.tradingStartAfterDayClearing && nowTime < Settings.tradingEndBeforeEveClearing) ||
                   (nowTime > Settings.tradingStartAfterEveClearing && nowTime < Settings.tradingEnd);
        }


        static volatile bool firstErrorReported = false;
        static object syncObject = new object();
        public static void InitiateSkypeSms(string smsMessage)
        {
            lock (syncObject)
            {
                if (firstErrorReported)
                    return;

                firstErrorReported = true;
            }

            var dataContext = new SlyBotDataContext();
            SkypeMessage msg = new SkypeMessage() { Date = DateTime.Now, IsSent = false, Message = smsMessage };
            dataContext.SkypeMessages.InsertOnSubmit(msg);
            dataContext.SubmitChanges();

            Process process = new Process();
            process.StartInfo.FileName = Settings.skypeAgentDir + "SkypeAgent.exe";
            process.StartInfo.WorkingDirectory = Settings.skypeAgentDir;

            process.Start();
        }

        public static void ProcessFatalException(Exception ex)
        {
            SystemLogger.Fatal("Fatal error: " + ex.Message, ex);

            InitiateSkypeSms(ex.ToString().Substring(0, 65));            

            SystemLogger.Warn("Terminating application.");
            Application.Exit();
        }
    }
}
