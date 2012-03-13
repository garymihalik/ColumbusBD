<%@ Page Language="VB" AutoEventWireup="true" CodeFile="UnBookedWinsReport.aspx.vb" Inherits="UnBookedWinsReport" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>UnBooked Wins Report</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <uc7:MyModal ID="myModalControl" runat="server" />
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br/>
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
            <asp:Label runat="server" ID="Label1" Text="Generate report of wins that have not been forecasted with resources" /><br />
            <div style="float:left;width:25%">
                        <asp:Label runat="server" ID="lblStart" Text="Report Start Date" />
                        <ew:MultiTextDropDownList ID="cboPrior" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="StartDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="10" /><br />
                        <asp:Label runat="server" ID="lblEnd" Text="Report End Date" />                            
                        <ew:MultiTextDropDownList ID="cboAfter" runat="server" DataSourceID="ODS_Months" 
                            DataTextField="MontlyReportDates, Year" DataValueField="EndDate"
                            DataTextFormatString="{0:g} {1:g}" Rows="10" /><br />
            </div>           <br />
            <asp:gridview  ID="grdItems" runat="server" AutoGenerateColumns="False" SaveButtonID="btnSaveBulkEdit"
            DataSourceID="ODS_Items" DataKeyNames="PK_OpportunityID" Width="80%" EmptyDataText="No Updates Yet" AllowSorting="true" Visible="false">
                <Columns>
                    <asp:BoundField DataField="PK_OpportunityID" HeaderText="PK_OpportunityID" Visible="False"/>
                    <asp:BoundField DataField="Client" HeaderText="Last Name" Visible="True" SortExpression="Client"/>
                    <asp:BoundField DataField="OpportunityName" HeaderText="First Name" Visible="True" SortExpression="OpportunityName"/>
                    <asp:BoundField DataField="DateEntered" HeaderText="Actual Roll Off" Visible="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="DateEntered"/>
                    <asp:BoundField DataField="OpportunityCloseDate" HeaderText="Booked Roll Off" Visible="True" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="OpportunityCloseDate"/>
                    <asp:BoundField DataField="EstimatedRevenue" HeaderText="Est. Revenue" Visible="True" SortExpression="EstimatedRevenue" DataFormatString="{0:c0}" ItemStyle-HorizontalAlign="Right"/>
                    <asp:BoundField DataField="OpportunityOwner" HeaderText="Opportunity Owners" Visible="True" SortExpression="OpportunityOwner"/>
               </Columns>
            </asp:gridview ><br />
            <div style="clear:both;">
                <asp:Button ID="btnSaveBulkEdit" runat="server" Text="Update Report" Visible="true"/>
            </div>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Items" runat="server" SelectMethod="GetBookedWithoutAssignments" TypeName="cForecast">
        <SelectParameters>
            <asp:ControlParameter ControlID="cboPrior" PropertyName="SelectedValue" DefaultValue="#1/1/1900#" Name="dtStart"/>
            <asp:ControlParameter ControlID="cboAfter" PropertyName="SelectedValue" DefaultValue="#1/1/1900#" Name="dtEnd"/>
        </SelectParameters>
    </asp:ObjectDataSource>
     <asp:ObjectDataSource ID="ODS_Months" runat="server" TypeName="cForecast" SelectMethod="GetReportMonths" >
</asp:ObjectDataSource>
    </form>
</body>
</html>
