<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ForecastGraphs.aspx.vb" Inherits="ForecastGraphs" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>BU Financial Report - Booked</title>
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
        <div style="width:2500px" >
             <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                        <asp:Label runat="server" ID="lblStart" Text="Report Start Date" />
                        <ew:MultiTextDropDownList ID="cboPrior" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="StartDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="12" OnDataBound="SetupInitialDate"/><br />
                        <asp:Label runat="server" ID="lblEnd" Text="Report End Date" />                            
                        <ew:MultiTextDropDownList ID="cboAfter" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="EndDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="12" OnDataBound="SetupInitialDate"/><br />
                    <asp:RadioButtonList ID="rblShadowOrBooked" runat="server">
                        <asp:ListItem Selected="False" Text="Include Shadow Opportunities (Includes weighted revenue of opportunities that are >=70% with resources assigned) " Value="Shadow" />
                        <asp:ListItem Selected="True" Text="Only Show Booked Opportunities (Only includes revenue of opportunities that are won with resources assigned)" Value="Booked" />
                    </asp:RadioButtonList>
                    <hr />
                    <asp:RadioButtonList ID="rblType" runat="server">
                        <asp:ListItem Selected="True" Text="Show forecasted values only" Value="Forecast" />
                        <asp:ListItem Selected="False" Enabled="true" Text="Show booked meaning if there are actuals use the actuals, otherwise use the forecasted amount" Value="Booked" />
                    </asp:RadioButtonList>
                    <hr />
                  <asp:CheckBox ID="ckExport" runat="server" Checked="false" Text="Export Booked Data to Excel?" />                            
                  </ContentTemplate>
            </asp:UpdatePanel> 
            <br />
            <asp:Button ID="btnUpdateChart" runat="server" Text="Refresh Data" /><br />       
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <asp:chart id="MSNChart1" runat="server" ImageType="Png"  Visible="true">
                            <Titles>
                                <asp:Title Name="default" />
                            </Titles>
                            <chartareas>
                                <asp:chartarea Name="ChartArea1" >
                                </asp:chartarea>
                            </chartareas>
                    </asp:chart>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div style="clear:both">   
            <asp:UpdatePanel ID="pnlGraphUpdate" runat="server">
                <ContentTemplate>                   
                    

                    <asp:Panel runat="server" ID="pnlgridview">
                        
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
