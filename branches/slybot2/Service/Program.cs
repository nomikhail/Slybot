using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Threading;
using log4net;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Service
{
    class Program
    {
        static readonly ILog logger = LogManager.GetLogger("TestLogger");


        static void Main(string[] args)
        {
            var builder = new SQLiteConnectionStringBuilder();
            builder.DataSource = @"D:\!\Database\quik_sqlite.db3";
            
            SQLiteConnection connection = new SQLiteConnection(builder.ConnectionString);
            connection.Open();


            Stopwatch sw = new Stopwatch(); sw.Start();
            int total = 0;
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(50);
                total++;
                if (QueryPapers(connection))
                {
                    Console.WriteLine();
                    Console.WriteLine(((double)sw.ElapsedMilliseconds / total) + " msec/iteration.");                    
                }
            }

            LogManager.Shutdown();
        }

        class PaperItem : IEquatable<PaperItem>
        {
            public string Instrument { get; set; }

            public double Bid { get; set; }
            public double Ask { get; set; }
            public double Price { get; set; }

            public DateTime Time { get; set; }

            public bool Equals(PaperItem other)
            {
                return other != null &&
                    Instrument == other.Instrument &&
                    Bid == other.Bid &&
                    Ask == other.Ask &&
                    Price == other.Price &&
                    Time == other.Time;
            }

            public override string ToString()
            {
                return string.Format("{0}: {1}({2},{3}); {4}", Time.ToLongTimeString(), Instrument, Bid, Ask, Price);
            }
        }

        static List<PaperItem> lastItems = new List<PaperItem>();
        static bool QueryPapers(SQLiteConnection connection)
        {
            var command1 = new SQLiteCommand("select * from stakan_edm0 order by price", connection);
            var command2 = new SQLiteCommand("select * from stakan_eum0 order by price", connection);
            var command3 = new SQLiteCommand("select * from stakan_sim0 order by price", connection);

            QueryCommand(command1);
            QueryCommand(command2);
            QueryCommand(command3);


            var command = new SQLiteCommand("select * from papers order by time desc", connection);

            var reader = command.ExecuteReader();

            List<PaperItem> items = new List<PaperItem>();
            while (reader.Read())
            {
                var objs = new object[reader.FieldCount];
                reader.GetValues(objs);

                items.Add(new PaperItem()
                {
                    Instrument = (string)objs[0],
                    Bid = (double)objs[1],
                    Ask = (double)objs[2],
                    Price = (double)objs[3],
                    Time = (DateTime)objs[4]
                });
            }

            reader.Close();

            if (!items.SequenceEqual(lastItems))
            {
                lastItems = items;

                Console.Clear();
                foreach (var item in items)
                    Console.WriteLine(item);

                return true;
            }
            return false;
        }


        static void QueryCommand(SQLiteCommand command)
        {
            var reader = command.ExecuteReader();

            while (true)
            {
                var objs = new object[reader.FieldCount];

                reader.GetValues(objs);

                if(!reader.Read())
                    break;
            }

            reader.Close();
        }
    }
}
