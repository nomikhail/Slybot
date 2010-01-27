using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace AnalyzeData
{
    class DataAnalyzer
    {
        Regex parser = new Regex(@"\d\d.\d\d.\d\d\d\d \d\d:\d\d:\d\d, \d+, \d+, [\d.]+, [\d.]+, \d+, \d+, (-?\d+), (-?\d+), -?\d+, -?\d+, -?\d+", RegexOptions.Compiled);

        List<Regex> badLineParsers = new List<Regex>() 
        { 
            new Regex(@"\d\d.\d\d.\d\d\d\d \d\d:\d\d:\d\d, -2147483648,", RegexOptions.Compiled),
            new Regex(@"\d\d.\d\d.\d\d\d\d \d\d:\d\d:\d\d, \d+, \d+, -1.79769313486232E\+308,", RegexOptions.Compiled)
        };

        List<int> buyPointsList = new List<int>();
        List<int> sellPointsList = new List<int>();



        public void ParseFile(string filename)
        {
            Console.Write("Parsing " + filename + " ...   ");

            var reader = new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read), Encoding.Default);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (string.IsNullOrEmpty(line))
                    continue;

                ParseLine(line);
            }

            reader.Close();

            Console.WriteLine("Done.");
        }

        void ParseLine(string line)
        {
            Match match = parser.Match(line);

            if (!match.Success)
            {
                if (line.Contains("-2147483648"))
                    return;

                foreach (var badParser in badLineParsers)
                    if (badParser.IsMatch(line))
                        return;

                Console.WriteLine("Could not parse:");
                Console.WriteLine(line);
                Console.ReadLine();
                return;
            }

            int buyPoints = int.Parse(match.Groups[1].Captures[0].Value);
            int sellPoints = int.Parse(match.Groups[2].Captures[0].Value);

            buyPointsList.Add(buyPoints);
            sellPointsList.Add(sellPoints);
        }

        public void PrintReport()
        {
            buyPointsList.Sort();
            buyPointsList.Reverse();

            sellPointsList.Sort();
            sellPointsList.Reverse();

            Console.WriteLine("Total buy points: " + buyPointsList.Count);
            Console.WriteLine("Total sell points: " + sellPointsList.Count);

            double percentageStep = 0.2;
            for (int i = 1; i < 20; ++i)
            {
                int threshold = (int)(buyPointsList.Count * i * percentageStep) / 100;
                int buyPoints = buyPointsList[threshold];
                int sellPoints = sellPointsList[threshold];

                int balance = (buyPoints - sellPoints) / 2;

                int profit = buyPoints + sellPoints;

                Console.WriteLine((i * percentageStep).ToString("F") + "%:  " + balance + "      (" + buyPoints + ", " + sellPoints + ") = " + profit);
            }

            Console.ReadLine();
        }
    }
}
