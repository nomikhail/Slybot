using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.SqlClient;

namespace AnalyzeData
{
    class AnalyzeSlippage
    {
        static Regex regex = new Regex(@"Slippage of  (\S\S\S\S) for (\d*) pts.");

        static string query = @"select * from log
                                where message like '%slippage%'
                                       and date > '2009-08-14 00:33:35'";

        public static void Analyze()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "localhost";
            builder.InitialCatalog = "SlyBot";
            builder.IntegratedSecurity = false;
            builder.UserID = "Misha2Kota";
            builder.Password = "2KotaSignature";

            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);
            var reader = command.ExecuteReader();

            List<int> slipItems = new List<int>();

            var biggestIds = new Dictionary<int, List<int>>();

            while (reader.Read())
            {
                string message = reader["Message"].ToString();
                var match = regex.Match(message);

                string instr = match.Groups[1].Captures[0].Value;
                int slipvalue = int.Parse(match.Groups[2].Captures[0].Value);

                slipItems.Add(slipvalue);

                if (!biggestIds.ContainsKey(slipvalue))
                    biggestIds[slipvalue] = new List<int>();

                biggestIds[slipvalue].Add((int)reader["Id"]);
            }
            reader.Close();


            for (int i = 500; i >= 0; i -= 20)
            {
                Console.Write(" > " + i + ": ");
                
                var targetItems = slipItems.Where(item => item > i);

                Console.WriteLine(targetItems.Count() + " items, " + targetItems.Sum() + " total");
            }

            Console.WriteLine();
            Console.WriteLine("Ids with biggest slippage:");
            var keys = biggestIds.Keys.ToList();
            keys.Sort();
            keys.Reverse();
            for (int i = 0; i < Math.Min(keys.Count, 10); ++i)
                foreach (var id in biggestIds[keys[i]])
                    Console.WriteLine(id);

            Console.WriteLine();
            Console.WriteLine("Total slippage: " + (slipItems.Sum()));
            Console.ReadLine();
        }
    }
}
