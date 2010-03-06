using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core;
using System.IO;

namespace AnalyzeData
{
    class Instrument
    {
        public Instrument(string name, double origProfit)
        {
            Name = name;
            originalProfit = origProfit;
        }
        public string Name { get; set; }

        double originalProfit;

        List<double> buys = new List<double>();
        List<double> sells = new List<double>();

        public void AddOperation(string operation, string price)
        {
            if (operation == "B")
            {
                buys.Add(double.Parse(price));
            }
            else
            {
                sells.Add(double.Parse(price));
            }
        }

        public double GetProfit()
        {
            double totalProfit = originalProfit;
            for (int i = 0; i < Math.Min(buys.Count, sells.Count); ++i)
            {
                double profit = sells[i] - buys[i];

                if (Name == Settings.ED_instrument)
                    totalProfit += profit * 1000 * 34.7;
                else
                    totalProfit += profit;

                totalProfit -= 2;
            }

            return totalProfit;
        }


        public double GetAfterTaxProfit()
        {
            double profit = GetProfit();

            if (profit > 0)
                profit *= 0.87; // Income Tax

            return profit;
        }


        public void ReportPosition()
        {
            int pos = buys.Count - sells.Count;
            Console.WriteLine(Name + " position: " + pos + "; profit: " + (int)GetProfit());
        }

        public void ReportPosMatch()
        {
            int pos = buys.Count - sells.Count;
            int realPos = Portfolio.Postition(Name);

            if (pos != realPos)
                Console.WriteLine(Name + " position (" + pos + ") does not match real one (" + realPos + ") !!!");
            else
                Console.WriteLine(Name + " pos match ok.");
        }
    }

    class AnalyzeOrders
    {
        static Regex regex = new Regex(@"(\d\d.\d\d.\d\d\d\d \d\d:\d\d:\d\d); ACTION=NEW_ORDER; CLASSCODE=SPBFUT; SECCODE=(\w+); OPERATION=(\w); PRICE=(\d*.\d*); QUANTITY=1; TRANS_ID=\d+; ACCOUNT=SPBFUT0028L; CLIENT_CODE=SPBFUT0028L; TYPE=L;  OrderCode=\d+; Result=Executed;", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        static Instrument[] instrs = new Instrument[] {
            new Instrument(Settings.Eu_instrument, -42886.0),
            new Instrument(Settings.Si_instrument, -33468.0),
            new Instrument(Settings.ED_instrument,  79657.0) 
        };

        static void Analyze()
        {
            File.Copy(@"D:\!\orders.txt", @"D:\!\temp\orders.txt", true);
            File.Copy(@"D:\!\actions.txt", @"D:\!\temp\actions.txt", true);

            var reader = new StreamReader(File.Open(@"D:\!\temp\actions.txt", FileMode.Open, FileAccess.Read), Encoding.Default);
            Console.WriteLine(reader.ReadToEnd());
            reader.Close();

            reader = new StreamReader(File.Open(@"D:\!\temp\orders.txt", FileMode.Open, FileAccess.Read), Encoding.Default);
            string allFile = reader.ReadToEnd();
            reader.Close();

            MatchCollection collection = regex.Matches(allFile);

            List<double> buys = new List<double>();
            List<double> sells = new List<double>();

            foreach (Match match in collection)
            {
                var instr = instrs.Single(inst => inst.Name == match.Groups[2].Captures[0].Value);
                instr.AddOperation(match.Groups[3].Captures[0].Value, match.Groups[4].Captures[0].Value);

                //if(++i % 3 == 0)
                //Console.WriteLine(match.Groups[1].Captures[0].Value + ": " + (int)instrs.Sum(inst => inst.GetProfit()));
            }

            Portfolio.SynchronizePosition();

            foreach (var inst in instrs) inst.ReportPosition();
            Console.WriteLine();

            Console.WriteLine("Total profit: " + (int)instrs.Sum(inst => inst.GetProfit()));
            Console.WriteLine("Aftertax profit: " + (int)instrs.Sum(inst => inst.GetAfterTaxProfit()));
            Console.WriteLine();

            foreach (var inst in instrs) inst.ReportPosMatch();

            Console.ReadLine();
        }
    }
}
