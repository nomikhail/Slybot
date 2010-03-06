using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;

namespace TestFunctionality
{
    public class TestStakanData
    {
        public void Test()
        {
            int resultsCount = 1;

            string[] instruments = new string[] { Settings.ED_instrument, Settings.Eu_instrument, Settings.Si_instrument };

            foreach (string instr in instruments)
            {
                Console.WriteLine("Bids for " + instr);
                foreach (var item in DataExtractor.GetBids(instr, resultsCount))
                    Console.WriteLine(item.ToString());
                Console.WriteLine();
                Console.WriteLine("Asks for " + instr);
                foreach (var item in DataExtractor.GetAsks(instr, resultsCount))
                    Console.WriteLine(item.ToString());
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
