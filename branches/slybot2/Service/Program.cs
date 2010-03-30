using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Threading;
using log4net;
using System.Diagnostics;

namespace Service
{
    class Program
    {
        static void Main()
        {
            var builder = new SQLiteConnectionStringBuilder {DataSource = @"D:\!\Database\quik_sqlite.db3"};

            var connection = new SQLiteConnection(builder.ConnectionString);
            connection.Open();


            var sw = new Stopwatch(); sw.Start();
            int total = 0;
            while (!Console.KeyAvailable)
            {
                //Thread.Sleep(10);
                total++;
                QueryPapers(connection);
                if (total % 200 == 0)
                {
                    Console.WriteLine();
                    Console.WriteLine(((double) sw.ElapsedMilliseconds/total) + " msec/iteration.");
                }
            }

            LogManager.Shutdown();
        }

        class PaperItem : IEquatable<PaperItem>
        {
            public string Instrument { private get; set; }

            public double Bid { private get; set; }
            public double Ask { private get; set; }
            public double Price { private get; set; }

            public DateTime Time { private get; set; }

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

        static List<PaperItem> _lastItems = new List<PaperItem>();
        static bool QueryPapers(SQLiteConnection connection)
        {
            QueryCmdText(connection, "select * from stakan_eum0 order by price");
            QueryCmdText(connection, "select * from stakan_edm0 order by price");
            QueryCmdText(connection, "select * from stakan_sim0 order by price");

            QueryCmdText(connection, "select * from futlimits");
            QueryCmdText(connection, "select * from micexlimits");
            QueryCmdText(connection, "select count(*) from orders");
            QueryCmdText(connection, "select * from portfolio");
            QueryCmdText(connection, "select count(*) from trades");

            //var command = new SQLiteCommand("select * from papers order by time desc", connection);

            //var reader = command.ExecuteReader();

            //var items = new List<PaperItem>();
            //while (reader.Read())
            //{
            //    var objs = new object[reader.FieldCount];
            //    reader.GetValues(objs);

            //    items.Add(new PaperItem
            //    {
            //      Instrument = (string)objs[0],
            //      Bid = (double)objs[1],
            //      Ask = (double)objs[2],
            //      Price = (double)objs[3],
            //      Time = (DateTime)objs[4]
            //    });
            //}

            //reader.Close();

            //if (!items.SequenceEqual(_lastItems))
            //{
            //    _lastItems = items;

            //    Console.Clear();
            //    foreach (var item in items)
            //        Console.WriteLine(item);

            //    return true;
            //}
            return false;
        }

        private static void QueryCmdText(SQLiteConnection connection, string cmdText)
        {
            var command1 = new SQLiteCommand(cmdText, connection);
            QueryCommand(command1);
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
