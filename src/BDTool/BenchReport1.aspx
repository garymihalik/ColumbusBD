<%@ Page Language="VB" AutoEventWireup="true" CodeFile="BenchReport1.aspx.vb" Inherits="BenchReport1" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc2" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>
<%@ Register Assembly="ExportToExcelImage" Namespace="Jours.Web.Controls" TagPrefix="JO" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Bench Report</title>
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
            <div style="text-align:right;float:right"><asp:HyperLink runat="server" ID="help" NavigateUrl="~/Help/BenchReport/BenchReport.html" Target="_blank">Help</asp:HyperLink></div>
        <p>Shadow Rolloff - This is the last date a person is forecasted on a possible opportunity (Win percentage is 70%-100%)</p>
        <p>Booked Rolloff - This is the last date a person is booked on a won opportunity (Win percentage 100%)</p>
        <hr align="left" />
        <p>Show Roll-Offs up through</p>
           <asp:UpdatePanel runat="server" ID="pnlBenchReport" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <JO:ExportToExcelImage ID="ExportToExcel1" runat="server" ApplyStyleInExcel="True" Charset="utf-8"
                        ContentEncoding="windows-1250" EnableHyperLinks="False" ExportFileName="FileName.xls"
                        IncludeTimeStamp="True" PageSize="All" AlternateText="Export To Excel" ImageUrl="~/commonimages/export_2.gif" 
                        Width="2%" Height="2%" GridViewID="grdItems" Enabled="true"/>               
                    <uc1:BU ID="BU1" runat="server"/>
                    <asp:DropDownList runat="server" ID="cboInclusion">
                        <asp:ListItem Value="Employee" Text="Only Employees" Selected="True" />
                        <asp:ListItem Value="1099" Text="Only Contractors" />
                        <asp:ListItem Value="All" Text="All Resources" />
                    </asp:DropDownList><br />
                    <asp:Label ID="lblWinPercentage" runat="server" Text="Show Proposed Starting At - " />
                    <asp:TextBox ID="txtWinPercentage" runat="server" Text="70" CssClass="DateTextBox" />%<br />
                    <asp:Label ID="lblThroughDate" runat="server" Text="Run Report Through - " />
                    <asp:TextBox ID="txtEndDate" runat="server" Text="" CssClass="DateTextBox" />
                    <cc1:CalendarExtender runat="server" ID="cctextEnd" TargetControlID="txtEndDate" /><br />
                    <asp:Button runat="server" ID="btnUpdateReport" Text="Update Report" />
                   <asp:gridview  ID="grdItems" runat="server" AutoGenerateColumns="False" 
                   SaveButtonID="btnSaveBulkEdit"
                    DataSourceID="" DataKeyNames="PK_PersonID" 
                    Width="99%" Visible="True" EmptyDataText="No Resources Matching Criteria Found" AllowSorting="true" >
                        <Columns>
                            <asp:BoundField DataField="PK_PersonID" HeaderText="Person ID" Visible="False"/>
                            <asp:BoundField DataField="LastName" HeaderText="Last Name" Visible="True" SortExpression="LastName"/>
                            <asp:BoundField DataField="FirstName" HeaderText="First Name" Visible="True" SortExpression="FirstName"/>
                            <asp:BoundField DataField="ActualRollOff" HeaderText="Booked Roll Off" Visible="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="ActualRollOff"/>
                            <asp:BoundField DataField="BookedRollOff" HeaderText="Shadow Roll Off (>=70%)" Visible="True" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="BookedRollOff"/>
                            <asp:CheckBoxField DataField="Employee" HeaderText="Employee?" Visible="True" SortExpression="Employee"/>
                            <asp:TemplateField HeaderText="Booked">
			                    <ItemTemplate>
                                        <asp:Repeater ID="rptrBookedDetails" runat="server">
                                            <ItemTemplate>
                                                &#176;&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "Client")%>:<%#DataBinder.Eval(Container.DataItem, "OpportunityName")%>&nbsp;<%#DataBinder.Eval(Container.DataItem, "myStart", "{0:d}")%>&nbsp;through&nbsp;<%#DataBinder.Eval(Container.DataItem, "myEnd", "{0:d}")%>&nbsp;at&nbsp;<%#DataBinder.Eval(Container.DataItem, "PeriodUtilizationRate", "{0:p0}")%>&nbsp;Utilization<br />
                                            </ItemTemplate>
                                        </asp:Repeater>
			                    </ItemTemplate>			       
			                </asp:TemplateField>			    
                            <asp:TemplateField HeaderText="Shadow/Proposed">
			                    <ItemTemplate>
                                        <asp:Repeater ID="rptrProposedDetails" runat="server">
                                            <ItemTemplate>
                                                &#176;&nbsp;&nbsp;<%#DataBinder.Eval(Container.DataItem, "Client")%>:<%#DataBinder.Eval(Container.DataItem, "OpportunityName")%>(Win%-<%#DataBinder.Eval(Container.DataItem, "WinPercentage", "{0:p0}")%>)&nbsp;<%#DataBinder.Eval(Container.DataItem, "myStart", "{0:d}")%>&nbsp;through&nbsp;<%#DataBinder.Eval(Container.DataItem, "myEnd", "{0:d}")%>&nbsp;at&nbsp;<%#DataBinder.Eval(Container.DataItem, "PeriodUtilizationRate", "{0:p0}")%>&nbsp;Utilization<br />
                                            </ItemTemplate>
                                        </asp:Repeater>
			                    </ItemTemplate>			       
			                </asp:TemplateField>
                       </Columns>
                    </asp:gridview >
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="ExportToExcel1" />
                </Triggers>
           </asp:UpdatePanel>
            <asp:HyperLink ID="lnkManageResources" runat="server" NavigateUrl="~/ManageResources.aspx" Text="Update Resource Information"/>
            <asp:Button ID="btnSaveBulkEdit" runat="server" Text="Save Changes" Visible="false"/>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Resources" runat="server" SelectMethod="GetBenchReportByBU" TypeName="cAssignments">
        <SelectParameters>
            <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
</html>
