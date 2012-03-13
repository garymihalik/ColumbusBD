<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ClientResourceRevenueReport.aspx.vb" Inherits="ClientResourceRevenueReport" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Client Monthly Revenue Report</title>
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
            <asp:UpdatePanel runat="server" ID="pnlAccounts" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <ew:MultiTextDropDownList runat="server" ID="cboAccounts" DataSourceid="ODS_AccountGroup" DataValueField="AccountGroupID" DataTextFields="AccountGroup" DataTextFormatString="{0:g}" /><br />
                    <asp:Label runat="server" ID="lblStart" Text="Report Month Run Date" />
                    <ew:MultiTextDropDownList ID="cboCurrentMonth" runat="server" DataSourceID="ODS_Months" 
                        DataTextField="MontlyReportDates, Year" DataValueField="ReportOrder"
                        DataTextFormatString="{0:g} {1:g}" Rows="12" OnDataBound="SetupInitialDate"/><br />
                    <asp:Button ID="btnGetReport" Text="Get Report" runat="server" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <asp:GridView ID="grdAssignments" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="ODS_Assignments" ShowFooter="True" DataKeyNames="EntryID" 
                            Width="90%" EmptyDataText="No Assignments Found for Report Month" Visible="true">
                            <FooterStyle Font-Size="20px" />
                        <Columns>
                            <asp:BoundField DataField="FirstName" HeaderText="First Name"/>
                            <asp:BoundField DataField="LastName" HeaderText="Last Name"/>
                            <asp:BoundField DataField="HoursInMonth" HeaderText="Hours Used" ItemStyle-CssClass="DateTextBox" />
                            <asp:BoundField DataField="PeriodUtilization" HeaderText="% Utilization For Month" DataFormatString="{0:p2}" ItemStyle-CssClass="DateTextBox" />
                            <asp:BoundField DataField="BillRate" HeaderText="Bill Rate" DataFormatString="{0:c2}" ItemStyle-CssClass="DateTextBox" />
                            <asp:BoundField DataField="CompCosts" HeaderText="1099 Rate" DataFormatString="{0:c2}" ItemStyle-CssClass="DateTextBox" />
                            <asp:BoundField DataField="Revenue" HeaderText="Monthly Revenue" DataFormatString="{0:c2}" ItemStyle-CssClass="DateTextBox" />
                            <asp:BoundField DataField="Costs" HeaderText="Monthly 1099 Costs" DataFormatString="{0:c2}" ItemStyle-CssClass="DateTextBox" />
                        </Columns>
                    </asp:GridView>                        
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnGetReport" EventName="Click" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
        <asp:ObjectDataSource ID="ODS_Months" runat="server" TypeName="cForecast" SelectMethod="GetReportMonths" />
    <asp:ObjectDataSource ID="ODS_AccountGroup" runat="server" SelectMethod="GetAllAccountGroupsByBU" TypeName="cAccountClientIndustry" >
         <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource> 
    <asp:ObjectDataSource ID="ODS_Assignments" runat="server" SelectMethod="GetClientAssignmentsForMonth" TypeName="cAssignments">
        <SelectParameters>
            <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="BUID" DefaultValue="-1" />
            <asp:ControlParameter ControlID="cboCurrentMonth" PropertyName="SelectedValue" Name="ReportMonth" DefaultValue="-1" />
            <asp:ControlParameter ControlID="cboAccounts" PropertyName="SelectedValue" Name="AccountGroupID" DefaultValue="-1" />
        </SelectParameters>
     </asp:ObjectDataSource>
    </form>
</body>
</html>
