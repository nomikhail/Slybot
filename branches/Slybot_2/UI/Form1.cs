using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Core;
using System.Threading;
using System.Diagnostics;
using log4net.Config;
using log4net;
using System.Reflection;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;

namespace UI
{
    public partial class mainUiForm : Form
    {
        List<Button> buttons = new List<Button>();
        string[] syms = new string[] {"/","--","\\"," |"};
        int iteration = 0;

        double lastBuyPoints = 0;
        double lastSellPoints = 0;

        SlyBot slyBot = null;

        private void UpdateState()
        {
            ++iteration;

            Market market = Market.QueryMarket();

            if (!Util.IsOperationTime())
            {
                timeLabel.BackColor = SystemColors.Control;
                timeLabel.Text = "Not operational time.";
                return;
            }
            else if (!QuikManager.Connected)
            {
                timeLabel.BackColor = Color.Yellow;
                timeLabel.Text = "Connection lost!";
                return;
            }
            else if (slyBot.ActiveOrders)
            {
                timeLabel.BackColor = Color.Yellow;
                timeLabel.Text = "Pending orders ...";
            }


            double thresholdToBuy = Portfolio.GetBuyThreshold();
            double thresholdToSell = Portfolio.GetSellThreshold();


            positionLabel.Text = Portfolio.GetReport() +
                string.Format("              B:{0}  S:{1}          Exc:{2} Kill:{3} Act:{4}",
                (int)thresholdToBuy, (int)thresholdToSell,
                Order.TotalExecuted, Order.TotalKilled, Order.TotalActive);

            int timePassed = (int)(DateTime.Now.TimeOfDay - market.Time.TimeOfDay).TotalSeconds;

            if (timePassed < 10)
            {
                timeLabel.BackColor = SystemColors.Control;
                timeLabel.Text = syms[iteration % 4];
            }
            else
            {
                timeLabel.BackColor = Color.Yellow;
                timeLabel.Text = timePassed.ToString();
            }

            buyButton.Text = "B: " + (int)market.PointsBuyEur;
            lastBuyPoints = market.PointsBuyEur;
            if (market.PointsBuyEur > thresholdToBuy)
                buyButton.BackColor = Color.Yellow;
            else
                buyButton.BackColor = SystemColors.Control;

            sellButton.Text = "S: " + (int)market.PointsSellEur;
            lastSellPoints = market.PointsSellEur;
            if (market.PointsSellEur > thresholdToSell)
                sellButton.BackColor = Color.Yellow;
            else
                sellButton.BackColor = SystemColors.Control;

            if (!market.IsFull())
            {
                buyButton.BackColor = Color.Red;
                sellButton.BackColor = Color.Red;

                timeLabel.BackColor = Color.Yellow;
                timeLabel.Text = "No valid data!!!";
            }

            ReportTrailingOrder(slyBot.buyOrder, slyBot.putBuy, trailingBuyLabel, market.EurBid);
            ReportTrailingOrder(slyBot.sellOrder, slyBot.putSell, trailingSellLabel, market.EurAsk);
        }

        SqlConnection logConnection = null;
        DateTime lastUpdate = DateTime.Today;
        private void UpdateLogGrid(bool force)
        {
            if (!force && (DateTime.Now - lastUpdate).TotalSeconds < 600)
                return;

            lastUpdate = DateTime.Now;

            DateTime start = DateTime.Now;

            DateTime border = timeBorder.Value;
            border = border.AddDays(-(double)(daysAgoChooser.Value));

            StringBuilder commandBuilder = new StringBuilder();
            commandBuilder.AppendLine(@"select [Date], [Level], [Logger], [Message] from log");
            commandBuilder.Append("where  ([Date] > '" + border.ToString("s") + @"')");
            if(!showDebugCheckBox.Checked)
                commandBuilder.Append(" and level <> 'DEBUG'");
            commandBuilder.AppendLine();
            commandBuilder.AppendLine("order by [Id]");

            SqlCommand command = new SqlCommand(commandBuilder.ToString(), logConnection);

            var reader = command.ExecuteReader();

            logGridView.Rows.Clear();

            while (reader.Read())
            {
                object[] data = new object[reader.FieldCount];
                int res = reader.GetValues(data);

                logGridView.Rows.Add(data);

                var cellStyle = logGridView.Rows[logGridView.RowCount - 1].DefaultCellStyle;
                if (data[1].Equals("WARN"))
                    cellStyle.BackColor = Color.Yellow;

                if (data[1].Equals("ERROR"))
                    cellStyle.BackColor = Color.Red;

                if (data[1].Equals("FATAL"))
                    cellStyle.BackColor = Color.DarkRed;
            }
            reader.Close();

            logGridView.Rows.Add(logGridView.RowCount + " rows, " + (int)((DateTime.Now - start).TotalMilliseconds) + " ms");

            logGridView.FirstDisplayedScrollingRowIndex = logGridView.RowCount  - 1;
        }

        private void ReportTrailingOrder(Order order, int distance, Label label, double bestPrice)
        {
            if (distance < 5000)
                label.Text = distance.ToString();
            else
                label.Text = "--";

            label.Enabled = order != null && order.StateOld == Order.OrderStateOld.Active;

            label.BackColor = (order != null && order.Price == bestPrice) ?
                Color.Yellow : SystemColors.Control;
        }

        #region Trade Buttons

        private void buyButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Market market = Market.QueryMarket();

            if (!market.IsFull())
            {
                MessageBox.Show("No VALID market data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Enabled = true;
                return;
            }

            if (market.PointsBuyEur < lastBuyPoints - 5)
            {
                MessageBox.Show("The good price has gone!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Enabled = true;
                return;
            }

            slyBot.ToggleBuy = true;

            Enabled = true;
        }

        private void sellUsdButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Market market = Market.QueryMarket();

            if (!market.IsFull())
            {
                MessageBox.Show("No VALID market data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Order usdOrder = new Order(Settings.Si_instrument, 'S', market.UsdBid);
            usdOrder.Push();

            Enabled = true;
        }

        private void sellEurUsdButton_Click(object sender, EventArgs e)
        {
            Enabled = false;

            Market market = Market.QueryMarket();

            if (!market.IsFull())
            {
                MessageBox.Show("No VALID market data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Order eurUsdOrder = new Order(Settings.ED_instrument, 'S', market.EurUsdBid);
            eurUsdOrder.Push();

            Enabled = true;
        }


        private void sellButton_Click(object sender, EventArgs e)
        {
            Enabled = false;
            Market market = Market.QueryMarket();

            if (!market.IsFull())
            {
                MessageBox.Show("No VALID market data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (market.PointsSellEur < lastSellPoints - 5)
            {
                MessageBox.Show("The good price has gone!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            slyBot.ToggleSell = true;

            Enabled = true;
        }
        
        private void buyUsdButton_Click(object sender, EventArgs e)
        {
            Enabled = false;

            Market market = Market.QueryMarket();

            if (!market.IsFull())
            {
                MessageBox.Show("No VALID market data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Order usdOrder = new Order(Settings.Si_instrument, 'B', market.UsdAsk);
            usdOrder.Push();

            Enabled = true;
        }

        private void buyEurUsdbutton_Click(object sender, EventArgs e)
        {
            Enabled = false;

            Market market = Market.QueryMarket();

            if (!market.IsFull())
            {
                MessageBox.Show("No VALID market data!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Order eurUsdOrder = new Order(Settings.ED_instrument, 'B', market.EurUsdAsk);
            eurUsdOrder.Push();

            Enabled = true;
        }

        #endregion

        ILog logger = LogWrapper.GetLogger("UI");

        #region UI events

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateState();

            UpdateLogGrid(false);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            logger.Info("Closing Slybot.");

            if(slyBot != null)
                slyBot.Terminate();

            QuikManager.Terminate();
            DataExtractor.Terminate();
        }

        private void KillConcurents()
        {
            Process self = Process.GetCurrentProcess();

            var suspects = Process.GetProcessesByName("UI");

            foreach (var proc in suspects)
                if (proc.Id != self.Id)
                {
                    logger.WarnFormat("Killing concurrent process ({0}, {1})", proc.Id, proc.ProcessName);
                    proc.Kill();
                    Thread.Sleep(500);
                }
        }


        private void HolidayShutdown()
        {
            logger.Info("Initiating system shutdown due to holiday morning.");

            string myDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Process process = new Process();
            process.StartInfo.FileName = "shutdown";
            process.StartInfo.Arguments = "-s -t 5";
            process.StartInfo.WorkingDirectory = myDir;

            process.Start();

            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = Settings.enUsCulture;

            logger.Info("SlyBot launched.");

            KillConcurents();

            if (Util.IsHoliday() && DateTime.Now.Hour == 10 && DateTime.Now.Minute == 25)
            {
                HolidayShutdown();
                return;
            }

            QuikManager.StartProcessing();
            DataExtractor.StartProcessing();

            slyBot = new SlyBot();

            buttons.Add(sellButton);
            buttons.Add(buyButton);
            buttons.Add(buyUsdButton);
            buttons.Add(sellUsdButton);
            buttons.Add(buyEurUsdbutton);
            buttons.Add(sellEurUsdButton);

            string[] args = Environment.GetCommandLineArgs();
            if(args.Length > 1 && args[1].Equals("-active", StringComparison.InvariantCultureIgnoreCase))
                slyboxEnableCheckBox.Checked = true;
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
        
        private void UpdateChart()
        {
            DateTime border = timeBorder.Value;
            border = border.AddDays(-(double)(daysAgoChooser.Value));

            arbitrageChart.BeginInit();

            UpdateChartSeries(arbitrageChart.Series[0], Util.GetBuyCandleSticks(border));
            UpdateChartSeries(arbitrageChart.Series[1], Util.GetSellCandleSticks(border));

            arbitrageChart.Width = Math.Max(arbitrageChart.Series[0].Points.Count, arbitrageChart.Series[1].Points.Count) * 8 + 200;
            arbitrageChart.ChartAreas[0].Position.Width = 100 - (10 * 1000 / arbitrageChart.Width);

            arbitrageChart.EndInit();

            panel1.ScrollControlIntoView(panel1.Controls[0]);
        }

        public mainUiForm()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "localhost";
            builder.InitialCatalog = "SlyBot";
            builder.IntegratedSecurity = true;

            logConnection = new SqlConnection(builder.ConnectionString);
            logConnection.Open();

            InitializeComponent();
        }

        private void TradeButtonDown(object sender, MouseEventArgs e)
        {
            timer.Enabled = false;
        }

        private void TradeButtonUp(object sender, MouseEventArgs e)
        {
            timer.Enabled = true;
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void slyboxEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            slyBot.Active = slyboxEnableCheckBox.Checked;

            foreach (var button in buttons)
                button.Enabled = !slyboxEnableCheckBox.Checked;
        }

        private void pinButton_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void upbdateGridButton_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateLogGrid(true);

                UpdateChart();
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }

        private void timeBorder_ValueChanged(object sender, EventArgs e)
        {
            UpdateLogGrid(true);
        }

        private void fromNowButton_Click(object sender, EventArgs e)
        {
            daysAgoChooser.Value = 0;
            timeBorder.Value = DateTime.Now;
        }

        private void showDebugCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateLogGrid(true);
        }

        private void daysAgoChooser_ValueChanged(object sender, EventArgs e)
        {
            //UpdateLogGrid(true);
        }

        #endregion

    }
}

#region Legacy code - report state

//private string fileName = null;
//private void ReportState(Market market, int eurRurPos, double buyThreshold, double sellThreshold)
//{
//    if (DateTime.Now.Hour < 10)
//        return;

//    StringBuilder reportBuilder = new StringBuilder();
//    reportBuilder.Append(DateTime.Now.ToString());
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)market.EurBid);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)market.EurAsk);
//    reportBuilder.Append(", ");
//    reportBuilder.Append(market.EurUsdBid);
//    reportBuilder.Append(", ");
//    reportBuilder.Append(market.EurUsdAsk);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)market.UsdBid);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)market.UsdAsk);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)market.PointsBuyEur);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)market.PointsSellEur);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)eurRurPos);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)buyThreshold);
//    reportBuilder.Append(", ");
//    reportBuilder.Append((int)sellThreshold);

//    StreamWriter writer = null;
//    while (writer == null)
//    {
//        try
//        {
//            writer = new StreamWriter(File.Open(fileName, FileMode.Append, FileAccess.Write));
//        }
//        catch (Exception)
//        {
//            Thread.Sleep(10);
//        }
//    }

//    writer.WriteLine(reportBuilder.ToString());
//    writer.Close();

//}

#endregion
