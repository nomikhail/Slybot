using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using Core;
using System.Diagnostics;
using System.Data.SqlClient;
using GoogleChartSharp;


namespace AnalyzeData
{
    class Program
    {

        static void Main(string[] args)
        {
            var data = Enumerable.Range(1, 50).Select(i => (i * i) % 1000);

            // Create a line chart that is 250 pixels wide and 150 pixels high
            LineChart lineChart = new LineChart(800, 250);

            // Set the title text, title color and title font size in pixels
            lineChart.SetTitle("Single Dataset Per Line", "0000FF", 14);

            // Set the chart to use our collection of datasets
            lineChart.SetData(data.ToArray());

            // If we create an axis with only this parameter it will
            // have a range of 0-100 and be evenly spaced across the chart
            lineChart.AddAxis(new ChartAxis(ChartAxisType.Left));
            lineChart.AddAxis(new ChartAxis(ChartAxisType.Bottom));

            System.Diagnostics.Process.Start(lineChart.GetUrl());
        }

        //static readonly int numrows = 10000;
        //static int iteration = 0;
        //static void WithSql(int cmdsperq)
        //{
        //    //Thread.Sleep(500);

        //    var sw = new Stopwatch(); sw.Start();

        //    var con = new SqlConnection(new SlyBotDataContext().Connection.ConnectionString);
        //    con.Open();

        //    var bld = new StringBuilder();
        //    for (int i = numrows * iteration; i < numrows * (iteration + 1); ++i)
        //    {
        //        bld.AppendLine(string.Format("insert into loggedtransactions values ({0}, '2009-08-18 14:26:27.780', 1, 'asdf', 'b', 111.333, 7, '11:15:59', 123.456)", i));

        //        if ((i + 1) % cmdsperq == 0)
        //        {
        //            var cmd = new SqlCommand(bld.ToString(), con);
        //            cmd.ExecuteNonQuery();
        //            bld.Length = 0;
        //        }
        //    }

        //    Console.WriteLine("With sql, {0} per query: {1}", cmdsperq, sw.ElapsedMilliseconds);

        //    ++iteration;
        //}

        //static void WithLinq()
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();

        //    SlyBotDataContext data = new SlyBotDataContext();
        //    var item = data.LoggedTransactions.First();

        //    //List<LoggedTransaction
        //    for (int i = numrows * iteration; i < numrows * (iteration + 1); ++i)
        //    {
        //        var newItem = new LoggedTransaction()
        //        {
        //            Number = i,
        //            Operation = item.Operation,
        //            OrderCode = item.OrderCode,
        //            Price = item.Price,
        //            Quantity = item.Quantity,
        //            SecCode = item.SecCode,
        //            Time = item.Time,
        //            TimeQuik = item.TimeQuik,
        //            Volume = item.Volume
        //        };

        //        data.LoggedTransactions.InsertOnSubmit(newItem);
        //    }

        //    data.SubmitChanges();

        //    sw.Stop();
        //    Console.WriteLine("With linq: " + sw.ElapsedMilliseconds);

        //    ++iteration;
        //}
    }
}
