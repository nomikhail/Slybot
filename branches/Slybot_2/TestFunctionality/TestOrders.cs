using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using System.Threading;

namespace TestFunctionality
{
    class TestOrders
    {
        public void Test()
        {
            double avgCodeObtainTime = 0;
            double avgActivateTime = 0;
            double avgKillTime = 0;

            Console.WriteLine("Connecting ...");
            while (!QuikManager.Connected) Thread.Sleep(1000);
            Console.WriteLine("Connected.");


            int iterations = 10;

            for (int i = 0; i < iterations; ++i)
            {
                Console.WriteLine("Order #" + (i + 1));
                Order order = new Order(Settings.Si_instrument, 'S', DataExtractor.GetAsk(Settings.Si_instrument, 0) + 350);

                Console.WriteLine("Pushing ...");

                DateTime start = DateTime.Now;

                order.Push();

                while (order.StateOld == Order.OrderStateOld.NoCode)
                {
                    Thread.Sleep(20);
                }

                double nocodeTime = (DateTime.Now - start).TotalSeconds;
                Console.WriteLine("Code obtained during " + nocodeTime.ToString("F") + " seconds.");
                avgCodeObtainTime += nocodeTime / iterations;


                while (order.StateOld != Order.OrderStateOld.Active)
                {
                    Thread.Sleep(20);
                }

                double actTime = (DateTime.Now - start).TotalSeconds;
                Console.WriteLine("Activated during " + actTime.ToString("F") + " seconds.");
                avgActivateTime += actTime / iterations;

                Console.WriteLine("Waiting to the order to kill itself ...");
                start = DateTime.Now;
                while (order.StateOld != Order.OrderStateOld.Killed)
                {
                    Thread.Sleep(20);
                }

                double killTime = (DateTime.Now - start).TotalSeconds;
                Console.WriteLine("Killed after " + killTime.ToString("F") + " seconds.");
                avgKillTime += killTime / iterations;

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Average code obtain time: " + avgCodeObtainTime.ToString("F"));
            Console.WriteLine("Average activation time: " + avgActivateTime.ToString("F"));
            Console.WriteLine("Average kill time: " + avgKillTime.ToString("F"));

            Console.ReadLine();

            QuikManager.Terminate();
        }
    }
}
