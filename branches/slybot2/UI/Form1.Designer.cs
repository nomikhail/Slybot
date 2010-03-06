namespace UI
{
    partial class mainUiForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sellButton = new System.Windows.Forms.Button();
            this.timeLabel = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buyButton = new System.Windows.Forms.Button();
            this.sellUsdButton = new System.Windows.Forms.Button();
            this.buyUsdButton = new System.Windows.Forms.Button();
            this.sellEurUsdButton = new System.Windows.Forms.Button();
            this.buyEurUsdbutton = new System.Windows.Forms.Button();
            this.positionLabel = new System.Windows.Forms.Label();
            this.slyboxEnableCheckBox = new System.Windows.Forms.CheckBox();
            this.trailingBuyLabel = new System.Windows.Forms.Label();
            this.trailingSellLabel = new System.Windows.Forms.Label();
            this.logGridView = new System.Windows.Forms.DataGridView();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Source = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeBorder = new System.Windows.Forms.DateTimePicker();
            this.upbdateGridButton = new System.Windows.Forms.Button();
            this.fromNowButton = new System.Windows.Forms.Button();
            this.showDebugCheckBox = new System.Windows.Forms.CheckBox();
            this.daysAgoChooser = new System.Windows.Forms.NumericUpDown();
            this.equityLabel = new System.Windows.Forms.Label();
            this.arbitrageChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.logGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.daysAgoChooser)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.arbitrageChart)).BeginInit();
            this.panel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sellButton
            // 
            this.sellButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sellButton.BackColor = System.Drawing.SystemColors.Control;
            this.sellButton.Location = new System.Drawing.Point(234, 244);
            this.sellButton.Name = "sellButton";
            this.sellButton.Size = new System.Drawing.Size(54, 23);
            this.sellButton.TabIndex = 0;
            this.sellButton.TabStop = false;
            this.sellButton.Text = "Sell";
            this.sellButton.UseVisualStyleBackColor = false;
            this.sellButton.Click += new System.EventHandler(this.sellButton_Click);
            this.sellButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TradeButtonDown);
            this.sellButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TradeButtonUp);
            // 
            // timeLabel
            // 
            this.timeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timeLabel.AutoSize = true;
            this.timeLabel.BackColor = System.Drawing.SystemColors.Control;
            this.timeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.timeLabel.Location = new System.Drawing.Point(384, 249);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(19, 13);
            this.timeLabel.TabIndex = 1;
            this.timeLabel.Text = "11";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // buyButton
            // 
            this.buyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buyButton.BackColor = System.Drawing.SystemColors.Control;
            this.buyButton.Location = new System.Drawing.Point(59, 244);
            this.buyButton.Name = "buyButton";
            this.buyButton.Size = new System.Drawing.Size(54, 23);
            this.buyButton.TabIndex = 2;
            this.buyButton.TabStop = false;
            this.buyButton.Text = "Buy";
            this.buyButton.UseVisualStyleBackColor = false;
            this.buyButton.Click += new System.EventHandler(this.buyButton_Click);
            this.buyButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TradeButtonDown);
            this.buyButton.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TradeButtonUp);
            // 
            // sellUsdButton
            // 
            this.sellUsdButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sellUsdButton.Location = new System.Drawing.Point(139, 244);
            this.sellUsdButton.Name = "sellUsdButton";
            this.sellUsdButton.Size = new System.Drawing.Size(27, 23);
            this.sellUsdButton.TabIndex = 3;
            this.sellUsdButton.TabStop = false;
            this.sellUsdButton.Text = "$-";
            this.sellUsdButton.UseVisualStyleBackColor = true;
            this.sellUsdButton.Click += new System.EventHandler(this.sellUsdButton_Click);
            // 
            // buyUsdButton
            // 
            this.buyUsdButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buyUsdButton.Location = new System.Drawing.Point(314, 244);
            this.buyUsdButton.Name = "buyUsdButton";
            this.buyUsdButton.Size = new System.Drawing.Size(27, 23);
            this.buyUsdButton.TabIndex = 4;
            this.buyUsdButton.TabStop = false;
            this.buyUsdButton.Text = "$+";
            this.buyUsdButton.UseVisualStyleBackColor = true;
            this.buyUsdButton.Click += new System.EventHandler(this.buyUsdButton_Click);
            // 
            // sellEurUsdButton
            // 
            this.sellEurUsdButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sellEurUsdButton.Location = new System.Drawing.Point(168, 244);
            this.sellEurUsdButton.Name = "sellEurUsdButton";
            this.sellEurUsdButton.Size = new System.Drawing.Size(39, 23);
            this.sellEurUsdButton.TabIndex = 5;
            this.sellEurUsdButton.TabStop = false;
            this.sellEurUsdButton.Text = "€/$-";
            this.sellEurUsdButton.UseVisualStyleBackColor = true;
            this.sellEurUsdButton.Click += new System.EventHandler(this.sellEurUsdButton_Click);
            // 
            // buyEurUsdbutton
            // 
            this.buyEurUsdbutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buyEurUsdbutton.Location = new System.Drawing.Point(343, 244);
            this.buyEurUsdbutton.Name = "buyEurUsdbutton";
            this.buyEurUsdbutton.Size = new System.Drawing.Size(39, 23);
            this.buyEurUsdbutton.TabIndex = 6;
            this.buyEurUsdbutton.TabStop = false;
            this.buyEurUsdbutton.Text = "€/$+";
            this.buyEurUsdbutton.UseVisualStyleBackColor = true;
            this.buyEurUsdbutton.Click += new System.EventHandler(this.buyEurUsdbutton_Click);
            // 
            // positionLabel
            // 
            this.positionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.positionLabel.AutoSize = true;
            this.positionLabel.Location = new System.Drawing.Point(32, 272);
            this.positionLabel.Name = "positionLabel";
            this.positionLabel.Size = new System.Drawing.Size(138, 13);
            this.positionLabel.TabIndex = 7;
            this.positionLabel.Text = "Pos:      $:         Buy:     Sell:";
            // 
            // slyboxEnableCheckBox
            // 
            this.slyboxEnableCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.slyboxEnableCheckBox.AutoSize = true;
            this.slyboxEnableCheckBox.Location = new System.Drawing.Point(17, 249);
            this.slyboxEnableCheckBox.Name = "slyboxEnableCheckBox";
            this.slyboxEnableCheckBox.Size = new System.Drawing.Size(15, 14);
            this.slyboxEnableCheckBox.TabIndex = 8;
            this.slyboxEnableCheckBox.UseVisualStyleBackColor = true;
            this.slyboxEnableCheckBox.CheckedChanged += new System.EventHandler(this.slyboxEnableCheckBox_CheckedChanged);
            // 
            // trailingBuyLabel
            // 
            this.trailingBuyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trailingBuyLabel.AutoSize = true;
            this.trailingBuyLabel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.trailingBuyLabel.Enabled = false;
            this.trailingBuyLabel.Location = new System.Drawing.Point(112, 249);
            this.trailingBuyLabel.Name = "trailingBuyLabel";
            this.trailingBuyLabel.Size = new System.Drawing.Size(28, 13);
            this.trailingBuyLabel.TabIndex = 10;
            this.trailingBuyLabel.Text = "(-15)";
            // 
            // trailingSellLabel
            // 
            this.trailingSellLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.trailingSellLabel.AutoSize = true;
            this.trailingSellLabel.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.trailingSellLabel.Enabled = false;
            this.trailingSellLabel.Location = new System.Drawing.Point(287, 249);
            this.trailingSellLabel.Name = "trailingSellLabel";
            this.trailingSellLabel.Size = new System.Drawing.Size(28, 13);
            this.trailingSellLabel.TabIndex = 11;
            this.trailingSellLabel.Text = "(-15)";
            // 
            // logGridView
            // 
            this.logGridView.AllowUserToAddRows = false;
            this.logGridView.AllowUserToDeleteRows = false;
            this.logGridView.AllowUserToResizeRows = false;
            this.logGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.logGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.Level,
            this.Source,
            this.Message});
            this.logGridView.Location = new System.Drawing.Point(541, 7);
            this.logGridView.Name = "logGridView";
            this.logGridView.ReadOnly = true;
            this.logGridView.RowHeadersWidth = 30;
            this.logGridView.Size = new System.Drawing.Size(658, 283);
            this.logGridView.TabIndex = 12;
            // 
            // Date
            // 
            this.Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Date.HeaderText = "Date";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            this.Date.Width = 116;
            // 
            // Level
            // 
            this.Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Level.HeaderText = "Level";
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.Width = 52;
            // 
            // Source
            // 
            this.Source.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Source.HeaderText = "Source";
            this.Source.Name = "Source";
            this.Source.ReadOnly = true;
            this.Source.Width = 85;
            // 
            // Message
            // 
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            // 
            // timeBorder
            // 
            this.timeBorder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.timeBorder.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeBorder.Location = new System.Drawing.Point(403, 176);
            this.timeBorder.Name = "timeBorder";
            this.timeBorder.ShowUpDown = true;
            this.timeBorder.Size = new System.Drawing.Size(67, 20);
            this.timeBorder.TabIndex = 13;
            this.timeBorder.ValueChanged += new System.EventHandler(this.timeBorder_ValueChanged);
            // 
            // upbdateGridButton
            // 
            this.upbdateGridButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.upbdateGridButton.Location = new System.Drawing.Point(18, 174);
            this.upbdateGridButton.Name = "upbdateGridButton";
            this.upbdateGridButton.Size = new System.Drawing.Size(70, 23);
            this.upbdateGridButton.TabIndex = 14;
            this.upbdateGridButton.Text = "Update";
            this.upbdateGridButton.UseVisualStyleBackColor = true;
            this.upbdateGridButton.Click += new System.EventHandler(this.upbdateGridButton_Click);
            // 
            // fromNowButton
            // 
            this.fromNowButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fromNowButton.Location = new System.Drawing.Point(98, 174);
            this.fromNowButton.Name = "fromNowButton";
            this.fromNowButton.Size = new System.Drawing.Size(65, 23);
            this.fromNowButton.TabIndex = 15;
            this.fromNowButton.Text = "From Now";
            this.fromNowButton.UseVisualStyleBackColor = true;
            this.fromNowButton.Click += new System.EventHandler(this.fromNowButton_Click);
            // 
            // showDebugCheckBox
            // 
            this.showDebugCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showDebugCheckBox.AutoSize = true;
            this.showDebugCheckBox.Location = new System.Drawing.Point(282, 178);
            this.showDebugCheckBox.Name = "showDebugCheckBox";
            this.showDebugCheckBox.Size = new System.Drawing.Size(56, 17);
            this.showDebugCheckBox.TabIndex = 16;
            this.showDebugCheckBox.Text = "debug";
            this.showDebugCheckBox.UseVisualStyleBackColor = true;
            this.showDebugCheckBox.CheckedChanged += new System.EventHandler(this.showDebugCheckBox_CheckedChanged);
            // 
            // daysAgoChooser
            // 
            this.daysAgoChooser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.daysAgoChooser.Location = new System.Drawing.Point(361, 176);
            this.daysAgoChooser.Name = "daysAgoChooser";
            this.daysAgoChooser.Size = new System.Drawing.Size(32, 20);
            this.daysAgoChooser.TabIndex = 17;
            this.daysAgoChooser.ValueChanged += new System.EventHandler(this.daysAgoChooser_ValueChanged);
            // 
            // equityLabel
            // 
            this.equityLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.equityLabel.AutoSize = true;
            this.equityLabel.Location = new System.Drawing.Point(56, 211);
            this.equityLabel.Name = "equityLabel";
            this.equityLabel.Size = new System.Drawing.Size(38, 13);
            this.equityLabel.TabIndex = 18;
            this.equityLabel.Text = "equity:";
            // 
            // arbitrageChart
            // 
            this.arbitrageChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.arbitrageChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.LeftRight;
            chartArea1.AxisX.Interval = 30;
            chartArea1.AxisX.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Minutes;
            chartArea1.AxisX.LabelStyle.Format = "d MMM H:mm";
            chartArea1.AxisX.LabelStyle.Interval = 30;
            chartArea1.AxisX.LabelStyle.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Minutes;
            chartArea1.AxisY.Maximum = 150;
            chartArea1.AxisY.Minimum = -100;
            chartArea1.AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea1.AxisY2.Maximum = 150;
            chartArea1.AxisY2.Minimum = -100;
            chartArea1.BackColor = System.Drawing.Color.Cornsilk;
            chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 95F;
            chartArea1.Position.Width = 95F;
            this.arbitrageChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.arbitrageChart.Legends.Add(legend1);
            this.arbitrageChart.Location = new System.Drawing.Point(0, 0);
            this.arbitrageChart.Name = "arbitrageChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series1.CustomProperties = "PointWidth=0.6";
            series1.IsXValueIndexed = true;
            series1.Legend = "Legend1";
            series1.Name = "Buy Points";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series1.YValuesPerPoint = 4;
            series2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series2.CustomProperties = "PriceDownColor=White";
            series2.IsXValueIndexed = true;
            series2.Legend = "Legend1";
            series2.Name = "Sell Points";
            series2.XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series2.YValuesPerPoint = 4;
            this.arbitrageChart.Series.Add(series1);
            this.arbitrageChart.Series.Add(series2);
            this.arbitrageChart.Size = new System.Drawing.Size(1368, 441);
            this.arbitrageChart.TabIndex = 19;
            this.arbitrageChart.Text = "chart1";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.arbitrageChart);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1207, 441);
            this.panel1.TabIndex = 20;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.upbdateGridButton);
            this.splitContainer1.Panel2.Controls.Add(this.equityLabel);
            this.splitContainer1.Panel2.Controls.Add(this.sellButton);
            this.splitContainer1.Panel2.Controls.Add(this.daysAgoChooser);
            this.splitContainer1.Panel2.Controls.Add(this.timeLabel);
            this.splitContainer1.Panel2.Controls.Add(this.showDebugCheckBox);
            this.splitContainer1.Panel2.Controls.Add(this.buyButton);
            this.splitContainer1.Panel2.Controls.Add(this.fromNowButton);
            this.splitContainer1.Panel2.Controls.Add(this.sellUsdButton);
            this.splitContainer1.Panel2.Controls.Add(this.buyUsdButton);
            this.splitContainer1.Panel2.Controls.Add(this.timeBorder);
            this.splitContainer1.Panel2.Controls.Add(this.sellEurUsdButton);
            this.splitContainer1.Panel2.Controls.Add(this.logGridView);
            this.splitContainer1.Panel2.Controls.Add(this.buyEurUsdbutton);
            this.splitContainer1.Panel2.Controls.Add(this.trailingSellLabel);
            this.splitContainer1.Panel2.Controls.Add(this.positionLabel);
            this.splitContainer1.Panel2.Controls.Add(this.trailingBuyLabel);
            this.splitContainer1.Panel2.Controls.Add(this.slyboxEnableCheckBox);
            this.splitContainer1.Size = new System.Drawing.Size(1207, 745);
            this.splitContainer1.SplitterDistance = 444;
            this.splitContainer1.TabIndex = 21;
            // 
            // mainUiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1207, 745);
            this.Controls.Add(this.splitContainer1);
            this.MaximumSize = new System.Drawing.Size(1920, 1200);
            this.MinimumSize = new System.Drawing.Size(420, 250);
            this.Name = "mainUiForm";
            this.Text = "SlyBot";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.logGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.daysAgoChooser)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.arbitrageChart)).EndInit();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button sellButton;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Button buyButton;
        private System.Windows.Forms.Button sellUsdButton;
        private System.Windows.Forms.Button buyUsdButton;
        private System.Windows.Forms.Button sellEurUsdButton;
        private System.Windows.Forms.Button buyEurUsdbutton;
        private System.Windows.Forms.Label positionLabel;
        private System.Windows.Forms.CheckBox slyboxEnableCheckBox;
        private System.Windows.Forms.Label trailingBuyLabel;
        private System.Windows.Forms.Label trailingSellLabel;
        private System.Windows.Forms.DataGridView logGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Source;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DateTimePicker timeBorder;
        private System.Windows.Forms.Button upbdateGridButton;
        private System.Windows.Forms.Button fromNowButton;
        private System.Windows.Forms.CheckBox showDebugCheckBox;
        private System.Windows.Forms.NumericUpDown daysAgoChooser;
        private System.Windows.Forms.Label equityLabel;
        private System.Windows.Forms.DataVisualization.Charting.Chart arbitrageChart;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}

