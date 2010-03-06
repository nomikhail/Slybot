using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using log4net;


namespace Core
{
    public class SlyBot
    {
        static ILog ActiveLogger = LogWrapper.GetLogger("SlyBot.Active");
        static ILog TrailingLogger = LogWrapper.GetLogger("SlyBot.Trailing");

        static readonly double CbUsdRur = 32;
        static readonly bool enableTrailing = true;
        static readonly bool fightEnemyTrailing = false;

        object globalSyncObject = new object();

        public SlyBot()
        {
            Active = false;

            ToggleBuy = false;
            ToggleSell = false;

            activeQuotesTradingThread = new Thread(new ThreadStart(WorkingProcedureActiveTrade));
            activeQuotesTradingThread.Start();

            buyWorkingThread = new Thread(new ThreadStart(WorkingProcedureTrailingBuyOrder));
            buyWorkingThread.Start();

            sellWorkingThread = new Thread(new ThreadStart(WorkingProcedureTrailingSellOrder));
            sellWorkingThread.Start();
        }

        Thread buyWorkingThread = null;
        Thread sellWorkingThread = null;
        Thread activeQuotesTradingThread = null;

        public bool Active { get; set; }


        #region Trailing Buy

        public void ProcessTrailingBuy(Order buyOrder, Market market)
        {
            // buy other stuff
            double estimatedPointsProfit = market.UsdBid * market.EurUsdBid - buyOrder.Price;
            double thresholdDiff = estimatedPointsProfit - Portfolio.GetBuyThreshold();
            string diffStr = (thresholdDiff > 0) ? "(thr + " + (int)thresholdDiff + ")" : "(thr - " + (int)(-thresholdDiff) + ")";

            string logStr = "Trailing buy with " + (int)estimatedPointsProfit + diffStr + " points.";
            TrailingLogger.Info(logStr);

            List<Order> orders = new List<Order>();
            orders.Add(new Order(Settings.ED_instrument, 'S', market.EurUsdBid));
            orders.Add(new Order(Settings.Si_instrument, 'S', market.UsdBid));

            double eurUsdPrice = (market.EurUsdBid + market.EurUsdAsk) / 2;
            double dollarPos = (Portfolio.EurRur + (buyOrder.processedExecuted ? -1 : 0)) * eurUsdPrice + Portfolio.UsdRur;
            if (dollarPos > 0.33)
            {
                orders.Add(new Order(Settings.Si_instrument, 'S', market.UsdBid));
            }

            ServeOrders(orders, TrailingLogger);

            TrailingLogger.Info("Done.");
        }

        public Order buyOrder = null;
        public int putBuy = 0;
        
        public void WorkingProcedureTrailingBuyOrder()
        {
            try
            {

                Util.SystemLogger.Debug("Trailing buy thread started.");

                while (true)
                {
                    Thread.Sleep(50);

                    if (buyOrder != null && (buyOrder.StateOld == Order.OrderStateOld.Killed || buyOrder.StateOld == Order.OrderStateOld.Error))
                        buyOrder = null;

                    if (terminate && buyOrder == null)
                        break;

                    if (!QuikManager.Connected)
                        continue;

                    Market market = Market.QueryFromStakan(0);

                    if (buyOrder != null && buyOrder.StateOld == Order.OrderStateOld.Executed)
                    {
                        ProcessTrailingBuy(buyOrder, market);
                        buyOrder = null;
                    }

                    // analyze market condition
                    bool shouldPutOrder = false;
                    double targetBuyPrice = 0;

                    Market liquidMarket = Market.QueryFromStakan(0); //2
                    if (liquidMarket.IsFull() && !terminate)
                    {
                        var thresholdToBuy = Portfolio.GetBuyThreshold();
                        thresholdToBuy += Settings.TrailingOrderBonus;
                        targetBuyPrice = (int)liquidMarket.BidBuyEur(thresholdToBuy);
                        shouldPutOrder = targetBuyPrice - market.EurBid + Settings.MaxLooseToMarket > 0;

                        if (buyOrder != null)
                            putBuy = (int)(market.EurAsk - buyOrder.Price);
                        else
                            putBuy = (int)(market.EurAsk - targetBuyPrice);
                    }

                    if (!Active && buyOrder == null)
                        continue;

                    // operate order based on market condition
                    if (buyOrder == null)
                    {
                        if (enableTrailing && Util.IsTradingTime() && shouldPutOrder)
                        {
                            lock (globalSyncObject)
                            {
                                if (targetBuyPrice > market.EurAsk - Settings.TrailingOrderBonus)
                                    targetBuyPrice = market.EurAsk - Settings.TrailingOrderBonus;

                                buyOrder = new Order(Settings.Eu_instrument, 'B', targetBuyPrice);
                                buyOrder.AutoKill = false;
                                buyOrder.Push();
                            }
                        }
                    }
                    else
                    {
                        bool movedAway = Math.Abs(targetBuyPrice - buyOrder.Price) > Settings.Actual2TargetPriceDeviation;
                        bool isBeingTrailed = fightEnemyTrailing && (market.EurBid - buyOrder.Price == 1);

                        if (!Active || !Util.IsTradingTime() || terminate || movedAway || isBeingTrailed)
                        {
                            while (buyOrder.StateOld == Order.OrderStateOld.NoCode || buyOrder.StateOld == Order.OrderStateOld.NotFoundInOrders)
                                Thread.Sleep(50);

                            if (buyOrder.StateOld == Order.OrderStateOld.Error)
                            {
                                buyOrder = null;
                                continue;
                            }

                            buyOrder.Kill();

                            Order.OrderStateOld state = buyOrder.StateOld;

                            int i = 0;
                            while (i < 50 && state != Order.OrderStateOld.Executed && state != Order.OrderStateOld.Killed)
                            {
                                Thread.Sleep(100);
                                state = buyOrder.StateOld;
                                i++;
                            }
                        }
                    }
                }

                Util.SystemLogger.Debug("Trailing buy thread finished.");
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }

        #endregion


        #region Trailing Sell

        public void ProcessTrailingSell(Order sellOrder, Market market)
        {
            // sell other stuff
            double estimatedPointsProfit = - market.UsdAsk * market.EurUsdAsk + sellOrder.Price;
            double thresholdDiff = estimatedPointsProfit - Portfolio.GetSellThreshold();
            string diffStr = (thresholdDiff > 0) ? "(thr + " + (int)thresholdDiff + ")" : "(thr - " + (int)(-thresholdDiff) + ")";

            string logStr = "Trailing sell with " + (int)estimatedPointsProfit + diffStr + " points.";
            TrailingLogger.Info(logStr);

            List<Order> orders = new List<Order>();
            orders.Add(new Order(Settings.ED_instrument, 'B', market.EurUsdAsk));
            orders.Add(new Order(Settings.Si_instrument, 'B', market.UsdAsk));

            double eurUsdPrice = (market.EurUsdBid + market.EurUsdAsk) / 2;
            double dollarPos = (Portfolio.EurRur + (sellOrder.processedExecuted ? 1 : 0)) * eurUsdPrice + Portfolio.UsdRur;
            if (dollarPos < -0.33)
            {
                orders.Add(new Order(Settings.Si_instrument, 'B', market.UsdAsk));
            }


            ServeOrders(orders, TrailingLogger);

            TrailingLogger.Info("Done");
        }


        public Order sellOrder = null;
        public int putSell = 0;
        public void WorkingProcedureTrailingSellOrder()
        {
            try
            {
                Util.SystemLogger.Debug("Trailing sell thread started.");

                while (true)
                {
                    Thread.Sleep(50);

                    if (sellOrder != null && (sellOrder.StateOld == Order.OrderStateOld.Killed || sellOrder.StateOld == Order.OrderStateOld.Error))
                        sellOrder = null;

                    if (terminate && sellOrder == null)
                        break;

                    if (!QuikManager.Connected)
                        continue;

                    Market market = Market.QueryFromStakan(0);

                    if (sellOrder != null && sellOrder.StateOld == Order.OrderStateOld.Executed)
                    {
                        ProcessTrailingSell(sellOrder, market);
                        sellOrder = null;
                    }

                    // analyze market condition
                    bool shouldPutOrder = false;
                    double targetSellPrice = 0;

                    Market liquidMarket = Market.QueryFromStakan(0); //2
                    if (liquidMarket.IsFull() && !terminate)
                    {
                        double thresholdToSell = Portfolio.GetSellThreshold();
                        thresholdToSell += Settings.TrailingOrderBonus;
                        targetSellPrice = (int)liquidMarket.AskSellEur(thresholdToSell);
                        shouldPutOrder = -targetSellPrice + market.EurAsk + Settings.MaxLooseToMarket > 0;

                        if (sellOrder != null)
                            putSell = (int)(sellOrder.Price - market.EurBid);
                        else
                            putSell = (int)(targetSellPrice - market.EurBid);
                    }

                    if (!Active && sellOrder == null)
                        continue;

                    // operate order based on market condition
                    if (sellOrder == null)
                    {
                        if (enableTrailing && Util.IsTradingTime() && shouldPutOrder)
                        {
                            lock (globalSyncObject)
                            {
                                if (targetSellPrice < market.EurBid + Settings.TrailingOrderBonus)
                                    targetSellPrice = market.EurBid + Settings.TrailingOrderBonus;

                                sellOrder = new Order(Settings.Eu_instrument, 'S', targetSellPrice);
                                sellOrder.AutoKill = false;
                                sellOrder.Push();
                            }
                        }
                    }
                    else
                    {
                        bool movedAway = Math.Abs(targetSellPrice - sellOrder.Price) > Settings.Actual2TargetPriceDeviation;
                        bool isBeingTrailed = fightEnemyTrailing && (sellOrder.Price - market.EurAsk == 1);

                        if (!Active || !Util.IsTradingTime() || terminate || movedAway || isBeingTrailed)
                        {
                            while (sellOrder.StateOld == Order.OrderStateOld.NoCode || sellOrder.StateOld == Order.OrderStateOld.NotFoundInOrders)
                                Thread.Sleep(50);

                            if (sellOrder.StateOld == Order.OrderStateOld.Error)
                            {
                                sellOrder = null;
                                continue;
                            }

                            sellOrder.Kill();

                            Order.OrderStateOld state = sellOrder.StateOld;

                            int i = 0;
                            while (i < 50 && state != Order.OrderStateOld.Executed && state != Order.OrderStateOld.Killed)
                            {
                                Thread.Sleep(100);
                                state = sellOrder.StateOld;

                                i++;
                            }
                        }
                    }
                }

                Util.SystemLogger.Debug("Trailing sell thread finished.");
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }

        #endregion


        #region Active Trade From Quotes

        void KillTrailingOrders()
        {
            if (buyOrder != null && buyOrder.StateOld == Order.OrderStateOld.Active)
                buyOrder.Kill();

            if (sellOrder != null && sellOrder.StateOld == Order.OrderStateOld.Active)
                sellOrder.Kill();
        }

        public bool ToggleBuy { get; set; }
        public bool ToggleSell { get; set; }

        void Buy(Market market)
        {
            ActiveLogger.Info((ToggleBuy?"Manual buy":"Buy") + " with " + (int)market.PointsBuyEur + " points.");

            KillTrailingOrders();

            ToggleBuy = false;

            List<Order> orders = new List<Order>();

            orders.Add(new Order(Settings.Eu_instrument, 'B', market.EurAsk));
            orders.Add(new Order(Settings.ED_instrument, 'S', market.EurUsdBid));
            orders.Add(new Order(Settings.Si_instrument, 'S', market.UsdBid));

            double eurUsdPrice = (market.EurUsdBid + market.EurUsdAsk) / 2;
            double dollarPos = Portfolio.EurRur * eurUsdPrice + Portfolio.UsdRur;
            if (dollarPos > 0.33)
            {
                orders.Add(new Order(Settings.Si_instrument, 'S', market.UsdBid));
            }

            ServeOrders(orders, ActiveLogger);

            ActiveLogger.Info("Done.");
        }

        void Sell(Market market)
        {
            ActiveLogger.Info((ToggleSell?"Manual sell":"Sell") + " with " + (int)market.PointsSellEur + " points.");

            KillTrailingOrders();

            ToggleSell = false;

            List<Order> orders = new List<Order>();

            orders.Add(new Order(Settings.Eu_instrument, 'S', market.EurBid));
            orders.Add(new Order(Settings.ED_instrument, 'B', market.EurUsdAsk));
            orders.Add(new Order(Settings.Si_instrument, 'B', market.UsdAsk));

            double eurUsdPrice = (market.EurUsdBid + market.EurUsdAsk) / 2;
            double dollarPos = Portfolio.EurRur * eurUsdPrice + Portfolio.UsdRur;
            if (dollarPos < -0.33)
            {
                orders.Add(new Order(Settings.Si_instrument, 'B', market.UsdAsk));
            }

            ServeOrders(orders, ActiveLogger);

            ActiveLogger.Info("Done.");
        }


        public void WorkingProcedureActiveTrade()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(50);

                    if (terminate)
                        break;

                    lock (globalSyncObject)
                    {
                        while (Active && Util.IsTradingTime() && QuikManager.Connected)
                        {
                            Market market = Market.QueryFromStakan(0);
                            if (!market.IsFull())
                                break;

                            if (market.IsFull() && market.PointsBuyEur > Portfolio.GetBuyThreshold())
                            {
                                Buy(market);
                                continue;
                            }

                            if (market.IsFull() && market.PointsSellEur > Portfolio.GetSellThreshold())
                            {
                                Sell(market);
                                continue;
                            }

                            break;
                        }

                        Market newMarket = Market.QueryFromStakan(0);
                        if (!newMarket.IsFull())
                            continue;

                        if (ToggleBuy)
                        {
                            Buy(newMarket);
                            continue;
                        }

                        if (ToggleSell)
                        {
                            Sell(newMarket);
                            continue;
                        }

                    } // end lock
                }
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }


        void ReportSlippage(Order oldOrder, Order newOrder, ILog logger)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Slippage of  ");
            builder.Append(oldOrder.SecCode);
            builder.Append(" for ");

            double points = newOrder.Price - oldOrder.Price;
            if (oldOrder.Operation == 'S' || oldOrder.Operation == 's')
                points = -points;

            if (oldOrder.SecCode.Equals(Settings.ED_instrument))
                points *= 1000 * CbUsdRur;

            builder.Append((int)points);
            builder.Append(" pts.");

            logger.Warn(builder.ToString());
        }

        #endregion
        
        
        public bool ActiveOrders
        {
            get { return activeOrders > 0; }
        }

        int activeOrders = 0;
        void ServeOrders(List<Order> orders, ILog logger)
        {
            activeOrders++;

            foreach (Order order in orders)
                order.Push();

            while (orders.Count > 0)
            {
                bool actionMade = false;
                foreach (Order order in orders)
                {
                    if (order.StateOld == Order.OrderStateOld.Executed)
                    {
                        orders.Remove(order);
                        actionMade = true;
                        break;
                    }

                    if (order.StateOld == Order.OrderStateOld.Killed)
                    {
                        Order newOrder = new Order(order);
                        ReportSlippage(order, newOrder, logger);
                        newOrder.Push();
                        orders.Remove(order);
                        orders.Add(newOrder);
                        actionMade = true;
                        break;
                    }

                    if (order.StateOld == Order.OrderStateOld.Error)
                    {
                        orders.Clear();
                        actionMade = true;

                        logger.Error(order.Error);

                        break;
                    }
                }

                if (!actionMade)
                    Thread.Sleep(50);
            }

            activeOrders--;
        }

        List<KeyValuePair<DateTime, string>> actions = new List<KeyValuePair<DateTime, string>>();

        bool terminate = false;
        public void Terminate()
        {
            terminate = true;
        }
    }
}
