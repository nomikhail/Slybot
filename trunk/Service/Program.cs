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
        private static SQLiteConnection _connection = null;

        static void Main()
        {
            var builder = new SQLiteConnectionStringBuilder {DataSource = @"R:\quik_sqlite.db3"};

            _connection = new SQLiteConnection(builder.ConnectionString);
            _connection.Open();

            Stakan._connection = _connection;


            var sw = new Stopwatch(); sw.Start();
            int total = 0;
            while (!Console.KeyAvailable)
            {
                Thread.Sleep(100);
                total++;


                Stakan.UpdateStakan("FSMICXM0");
                Stakan.UpdateStakan("RiM0");
                //QueryCmdText("select * from Stakan_FSMICXM0 order by Price");
                //QueryCmdText("select * from stakan_rim0 order by Price");
                
                //if (total % 200 == 0)
                //{
                //    Console.WriteLine();
                //    Console.WriteLine(((double)sw.ElapsedMilliseconds / total) + " msec/iteration.");
                //}
            }

            LogManager.Shutdown();
        }
    }
}
