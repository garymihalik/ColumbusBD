<%@ Page Language="VB" AutoEventWireup="true" CodeFile="AccountGroupForecastLayout.aspx.vb" Inherits="AccountGroupForecastLayout" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Forecasting Summary</title>
    <style type ="text/css"  >
      table
      {
        table-layout:auto;
        border-color:Black;
        border-style:solid;
        /*white-space: -moz-pre-wrap;  Mozilla*/
        /*word-wrap: break-word;       IE 5.5+ */
        white-space:nowrap;
        word-wrap:normal;
        border-collapse:collapse;
      }
      td
      {
        border-color:Black;
        border-style: solid; 
        border-width: 1px;
        white-space:nowrap;
        word-wrap:normal;
        padding:2px;
        text-align:right;
      }
      td.left
      {
          border-left-width:3px;   
      }
      td.right
      {
          border-right-width:3px;   
      }
      td.mid
      {
      }
      th
      {
        border-color:Black;
        border-style: solid; 
        border-left-width: 3px;
        border-right-width: 3px;
        /*border-collapse:thin;*/
        background-color: #ddd; 
      }
      tr
      {
      }
      tr.myAlt
      {
        background-color:#FAFFD1;
      }
      tr.myNorm
      {
        background-color:#ffffff;
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
        <div  >
            <div style="text-align:right;float:right"><asp:HyperLink runat="server" ID="help" NavigateUrl="~/Help/ClientSummary/ClientSummary.html" Target="_blank">Help</asp:HyperLink></div>
             <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <div style="float:left;width:25%">
                        <asp:Label runat="server" ID="lblStart" Text="Report Start Date" />
                        <ew:MultiTextDropDownList ID="cboPrior" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="StartDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="12" OnDataBound="SetupInitialDate"/><br />
                        <asp:Label runat="server" ID="lblEnd" Text="Report End Date" />                            
                        <ew:MultiTextDropDownList ID="cboAfter" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="EndDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="12" OnDataBound="SetupInitialDate"/><br />
                        <asp:Label runat="server" ID="lblAccountGroup" Text="Account Group" />                            
                        <ew:MultiTextDropDownList ID="cboAccountGroup" runat="server" DataSourceID="ODS_AccountGroup" 
                            DataTextField="AccountGroup" DataValueField="AccountGroupID"
                            DataTextFormatString="{0:g}" Rows="10" /><br />    
                    <asp:Button ID="btnUpdateChart" runat="server" Text="Refresh Data" /><br />
                    Scroll to bottom of page to see results<br /></div>
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
     <asp:ObjectDataSource ID="ODS_Months" runat="server" TypeName="cForecast" SelectMethod="GetReportMonths"/>    
     <asp:ObjectDataSource ID="ODS_AccountGroup" runat="server" SelectMethod="GetAllAccountGroupsByBu" TypeName="cAccountClientIndustry">
        <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
     </asp:ObjectDataSource>
    </form>
</body>
</html>
