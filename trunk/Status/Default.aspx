<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Status._Default" %>

<%@ Register assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>SlyBot status</title>
	</head>
<body style="background-color: #CCFFFF">
    <form id="form1" runat="server">
	    <div style="width: 900px; margin-left: auto; margin-right: auto; margin-top: 60px;
	        margin-bottom: 20px;">
	        
	          <div style="height:100%">
 		        <div style="float:right; margin-bottom:10px;">
		            <asp:Table ID="statusTable" runat="server" BackColor="White" BorderWidth="1px" CellPadding="3"
		                CellSpacing="0" Height="16px" Width="250px"/>
   		        </div>

                <div style="margin-left: 10px;" >
		                <asp:LinkButton ID="LinkButton1" runat="server" Style="margin-right: 30px" OnClick="LinkButton1_Click">2 hours</asp:LinkButton>
		                <asp:LinkButton ID="LinkButton2" runat="server" Style="margin-right: 30px" OnClick="LinkButton2_Click">8 hours</asp:LinkButton>
                        <asp:LinkButton ID="LinkButton3" runat="server" Style="margin-right: 30px" OnClick="LinkButton3_Click">Day</asp:LinkButton>
		        </div>		        
	        
	          </div>
		        
		        <div style="width: 100%; margin-bottom:10px; font-weight: 700;">

		            <asp:Chart ID="lastCandlesChart" runat="server" Width="900" Height="252px" 
                        BorderlineColor="Black" BorderlineDashStyle="Dash">
                            <Series>
                                <asp:Series Name="Series1" ChartType="Candlestick" YValuesPerPoint="4" 
                                    CustomProperties="PointWidth=0.6">
                                </asp:Series>
                                <asp:Series ChartArea="ChartArea1" ChartType="Candlestick" Name="Series2" 
                                    YValuesPerPoint="4" CustomProperties="PointWidth=0.6">
                                </asp:Series>
                            </Series>
                            <ChartAreas>
                                <asp:ChartArea Name="ChartArea1" BackColor="192, 255, 255" 
                                    BackGradientStyle="TopBottom" BackSecondaryColor="255, 255, 192">
                                    <AxisY Maximum="150" Minimum="-100">
                                    </AxisY>
                                    <AxisX>
                                        <LabelStyle Format="H:mm" Interval="30" IntervalType="Minutes" />
                                    </AxisX>
                                    <Position Height="94" Width="96" Y="1" />
                                </asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>
		        </div>
		        
		        <div style="width: 100%;">
		        
	                <asp:Table ID="infoTable" runat="server" BackColor="White" BorderWidth="1px" CellPadding="0"
	                        CellSpacing="0" Width="150px" BorderColor="Black" 
                                style="margin-left: 0px; margin-top: 0px; float:right; text-align:center" />

		            <div style="width:740px;">
	                    <asp:Table ID="logTable" runat="server" BackColor="White" Style="width: 100%; margin-bottom: 5px; margin-top: 0px;"
	                        BorderWidth="1px" CellPadding="3" CellSpacing="0" Width="100%">
	                        <asp:TableHeaderRow>
	                            <asp:TableHeaderCell Width="160">Time</asp:TableHeaderCell>
	                            <asp:TableHeaderCell Width="50">Level</asp:TableHeaderCell>
	                            <asp:TableHeaderCell Width="100">Source</asp:TableHeaderCell>
	                            <asp:TableHeaderCell>Message</asp:TableHeaderCell>
	                        </asp:TableHeaderRow>
	                    </asp:Table>
	                    
   	                    <asp:Label runat="server" ID="timeReportLabel" Style="font-size: smaller; float:right">1.43 seconds</asp:Label>
                    </div>                  
                               
		        </div>

	    </div>
    </form>
</body>
</html>
