<%@ Page Language="VB" AutoEventWireup="true" CodeFile="PipelineVsBooked.aspx.vb" Inherits="PipelineVsBooked" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Pipeline Vs. Booked Revenue Report - STILL UNDER DEVELOPMENT - Data is not reported accurately</title>
    <style type ="text/css"  >
      table
      {
        table-layout:fixed;
      }
      td
      {
         white-space: -moz-pre-wrap  /* Mozilla*/
         word-wrap: break-word;       /* IE 5.5+ */
      }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <uc7:MyModal ID="myModalControl" runat="server" />
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br />
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
             <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <asp:Label runat="server" ID="Label1" Text="Generate report of Booked Versus Pipeline Revenue" /><br />
                    <uc1:BU ID="BU1" runat="server"/>
                    <div style="float:left;width:25%">
                        <asp:Label runat="server" ID="lblStart" Text="Report Start Date" />
                        <ew:MultiTextDropDownList ID="cboPrior" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="StartDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="10" OnDataBound="SetupInitialDate"/><br />
                        <asp:Label runat="server" ID="lblEnd" Text="Report End Date" />                            
                        <ew:MultiTextDropDownList ID="cboAfter" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="EndDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="10" OnDataBound="SetupInitialDate"/><br />
                        <asp:RadioButtonList ID="rblTypes" runat="server" AutoPostBack="true" Visible="true">
                            <asp:ListItem Text="Month By Month" Value="Block" />
                            <asp:ListItem Text="Trend (Running Totals)" Value="Trend" Selected="True" />
                        </asp:RadioButtonList>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>        
            <div style="clear:both">   
            <asp:UpdatePanel ID="pnlGraphUpdate" runat="server">
                <ContentTemplate>                   
                    <asp:Button ID="btnUpdateChart" runat="server" Text="Refresh Data" />
                    <asp:chart id="MSNChart1" runat="server" ImageType="Png"  Visible="true">
                        <Titles>
                            <asp:Title Name="default" />
                        </Titles>
                        <chartareas>
                            <asp:chartarea Name="ChartArea1" >
                            </asp:chartarea>
                        </chartareas>
                    </asp:chart>
                    <asp:Panel runat="server" ID="pnlgridview">
                        <asp:HyperLink ID="lnkUnBookedWinsReport" runat="server" Text="Make This Data Better!" NavigateUrl="~/UnBookedWinsReport.aspx" />
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>                        
            </div>            
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
 <asp:ObjectDataSource ID="ODS_Months" runat="server" TypeName="cForecast" SelectMethod="GetReportMonths" >
</asp:ObjectDataSource>    
 
    </form>
</body>
</html>
