using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using System.Drawing;
using System.Data.SqlClient;
using System.Text;
using Core;
using System.Web.UI.DataVisualization.Charting;
using System.Collections.Generic;
using System.Globalization;

namespace Status
{
    public partial class _Default : System.Web.UI.Page
    {
        DateTime SetLinksAndGetThreshold()
        {
            LinkButton1.Enabled = true;
            LinkButton2.Enabled = true;
            LinkButton3.Enabled = true;

            if (thresholdOption == ThresholdOption.ThreeHours)
            {
                LinkButton1.Enabled = false;
                return DateTime.Now.AddHours(-2);
            }
            else if (thresholdOption == ThresholdOption.EightHours)
            {
                LinkButton2.Enabled = false;
                return DateTime.Now.AddHours(-8);
            }
            else
            {
                LinkButton3.Enabled = false;
                if (DateTime.Now.Hour > 7)
                    return DateTime.Today.AddHours(9);
                else
                    return DateTime.Today.AddDays(-1).AddHours(9);
            }
        }

        DateTime threshold;
        

        #region Log read and post

        enum ThresholdOption
        {
            ThreeHours,
            EightHours,
            Day
        }

        static ThresholdOption thresholdOption = ThresholdOption.ThreeHours;

        static SqlConnection connection = null;
        static SqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = "localhost";
                    builder.InitialCatalog = "SlyBot";
                    builder.IntegratedSecurity = false;
                    builder.UserID = "Misha2Kota";
                    builder.Password = "2KotaSignature";

                    connection = new SqlConnection(builder.ConnectionString);
                    connection.Open();
                }

                return connection;
            }
        }

        SqlDataReader GetLogReader()
        {
            StringBuilder commandBuilder = new StringBuilder();
            commandBuilder.AppendLine(@"select [Date], [Level], [Logger], [Message] from log");
            commandBuilder.AppendLine("where  ([Date] > '" + threshold.ToString("s") + @"')  and level <> 'DEBUG'");
            commandBuilder.AppendLine("order by [Date]");

            SqlCommand command = new SqlCommand(commandBuilder.ToString(), Connection);

            return command.ExecuteReader();
        }

        void ReadAndPostLog()
        {
            var reader = GetLogReader();
            while (reader.Read())
            {
                object[] data = new object[reader.FieldCount];
                int res = reader.GetValues(data);

                TableRow row = new TableRow();

                foreach (object dataItem in data)
                    row.Cells.Add(new TableCell() { Text = dataItem.ToString(), BorderWidth = 1 });

                if (data[2].Equals("Performance"))
                    row.BackColor = Color.LightBlue;
                else if (data[1].Equals("WARN"))
                    row.BackColor = Color.Yellow;
                else if (data[1].Equals("ERROR"))
                    row.BackColor = Color.Red;
                else if (data[1].Equals("FATAL"))
                    row.BackColor = Color.DarkRed;
                else
                    row.BackColor = Color.White;

                logTable.Rows.Add(row);
            }
            reader.Close();
        }

        #endregion

        class StatusTableProcessor : StatusCheckResponseProcessor
        {
            Table table;
            Color okColor;
            
            public StatusTableProcessor(Table table, Color okColor) 
            {
                this.table = table;
                this.okColor = okColor;
            }

            bool hasErrors = false;
            public bool HasErrors { get { return hasErrors; } }

            public void Response(string message, bool isOk)
            {
                TableRow row = new TableRow();

                if (isOk)
                {
                    row.Cells.Add(new TableCell() { Text = message });
                    row.BackColor = okColor;
                }
                else
                {
                    row.Cells.Add(new TableCell() { Text = "<b>" + message + "</b>", BorderWidth = 1 });
                    row.BackColor = Color.Red;
                    hasErrors = true;
                }

                table.Rows.Add(row);
            }
        }

        class InfoTableProcessor : StatusCheckResponseProcessor
        {
            Table table;

            public InfoTableProcessor(Table table)
            {
                this.table = table;
            }

            public void Response(string message, bool isOk)
            {
                TableRow row = new TableRow(); // { Height = 30 };
                if (isOk)
                    row.BackColor = Color.Yellow;

                if (message.Contains(':'))
                {
                    var parts = message.Split(':');

                    row.Cells.Add(new TableCell() { Text = parts[0], BorderWidth = 1});
                    row.Cells.Add(new TableCell() { Text = "<b>" + parts[1] + "</b>", BorderWidth = 1, Width = new Unit(80)});
                }
                else
                {
                    row.Height = 30;
                    row.Cells.Add(new TableCell() { Text = message, ColumnSpan = 2, BorderWidth = 1 });
                }

                table.Rows.Add(row);
            }
        }

        void CheckStatus()
        {
            var statusTableProc = new StatusTableProcessor(statusTable, Color.LightGreen);
            var infoTableProc = new InfoTableProcessor(infoTable);
            
            StatusCheck statusCheck = new StatusCheck();
            statusCheck.MakeStatusCheck(statusTableProc);
            statusCheck.MakeStatusInfo(infoTableProc);
        }


        private void UpdateChartSeries(Series chartSeries, IEnumerable<CandleStickDto> dataEnum)
        {
            var data = dataEnum.ToList();

            chartSeries.Points.Clear();

            for (int dataNum = 0; dataNum < data.Count; ++dataNum)
            {
                var item = data[dataNum];

                chartSeries.Points.AddXY(item.Time, item.High);
                chartSeries.Points[dataNum].YValues[1] = item.Low;
                chartSeries.Points[dataNum].YValues[2] = item.Open;
                chartSeries.Points[dataNum].YValues[3] = item.Close;
            }
        }


        void PlotCandles()
        {
            UpdateChartSeries(lastCandlesChart.Series[0], Util.GetBuyCandleSticks(threshold));
            UpdateChartSeries(lastCandlesChart.Series[1], Util.GetSellCandleSticks(threshold));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            DateTime start = DateTime.Now;

            threshold = SetLinksAndGetThreshold();

            PlotCandles();

            CheckStatus();

            ReadAndPostLog();

            timeReportLabel.Text = (DateTime.Now - start).TotalSeconds.ToString("F") + " seconds";
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            thresholdOption = ThresholdOption.ThreeHours;
            Response.Redirect(Request.RawUrl);
        }

        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            thresholdOption = ThresholdOption.EightHours;
            Response.Redirect(Request.RawUrl);
        }

        protected void LinkButton3_Click(object sender, EventArgs e)
        {
            thresholdOption = ThresholdOption.Day;
            Response.Redirect(Request.RawUrl);
        }

    }
}
