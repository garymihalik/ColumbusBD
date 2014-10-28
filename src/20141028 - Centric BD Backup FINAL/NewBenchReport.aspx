<%@ Page Language="VB" AutoEventWireup="true" CodeFile="NewBenchReport.aspx.vb" Inherits="NewBenchReport" %>
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
        <script language="javascript" type="text/javascript">
    function expandcollapse(obj,row)
    {
        var div = document.getElementById(obj);
        var img = document.getElementById('img' + obj);
        
        if (div.style.display == "none")
        {
            div.style.display = "block";
            if (row == 'alt')
            {
                img.src = "commonimages/redminus.png";
            }
            else
            {
                img.src = "commonimages/redminus.png";
            }
            img.alt = "Close to view other Resources";
        }
        else
        {
            div.style.display = "none";
            if (row == 'alt')
            {
                img.src = "commonimages/redplus.png";
            }
            else
            {
                img.src = "commonimages/redplus.png";
            }
            img.alt = "Expand to show Assignments";
        }
    } 
    </script>
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
                        Width="2%" Height="2%" GridViewID="grdItemsForExport" Enabled="true"/>               
                    <uc1:BU ID="BU1" runat="server"/>  
                    <asp:DropDownList runat="server" ID="cboInclusion">
                        <asp:ListItem Value="Employee" Text="Only Employees" Selected="True" />
                        <asp:ListItem Value="1099" Text="Only Contractors" />
                        <asp:ListItem Value="All" Text="All Resources" />
                    </asp:DropDownList>
                    <asp:TextBox ID="txtEndDate" runat="server" Text="" CssClass="DateTextBox" />
                    <cc1:CalendarExtender runat="server" ID="cctextEnd" TargetControlID="txtEndDate" />
                    <asp:Button runat="server" ID="btnUpdateReport" Text="Update Report" />
                   <asp:gridview  ID="grdItems" runat="server" AutoGenerateColumns="False" 
                   SaveButtonID="btnSaveBulkEdit"
                    DataSourceID="" DataKeyNames="PK_PersonID" 
                    Width="80%" Visible="True" EmptyDataText="No Updates Yet" AllowSorting="true" >
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <a href="javascript:expandcollapse('div<%# Eval("PK_PersonID") %>', 'one');">
                                        <img id="imgdiv<%# Eval("PK_PersonID") %>" alt="Click to show/hide Assignments for Resources <%# Eval("PK_PersonID") %>"  width="15px" border="0" src="commonimages/redplus.png"/>
                                    </a>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="PK_PersonID" HeaderText="Person ID" Visible="False"/>
                            <asp:BoundField DataField="LastName" HeaderText="Last Name" Visible="True" SortExpression="LastName"/>
                            <asp:BoundField DataField="FirstName" HeaderText="First Name" Visible="True" SortExpression="FirstName"/>
                            <asp:BoundField DataField="ActualRollOff" HeaderText="Booked Roll Off" Visible="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="ActualRollOff"/>
                            <asp:BoundField DataField="BookedRollOff" HeaderText="Shadow Roll Off" Visible="True" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="BookedRollOff"/>
                            <asp:CheckBoxField DataField="Employee" HeaderText="Employee?" Visible="True" SortExpression="Employee"/>
                            <asp:TemplateField>
			                    <ItemTemplate>
			                        <tr>
                                        <td colspan="100%">
                                            <div id="div<%# Eval("PK_PersonID") %>" style="display:none;position:relative;left:15px;OVERFLOW: auto;WIDTH:97%" >
                                                <asp:GridView ID="GridView2" AllowPaging="false" AllowSorting="false" BackColor="White" Width="100%" Font-Size="X-Small"
                                                    AutoGenerateColumns="false" Font-Names="Verdana" runat="server" ShowFooter="False" OnRowdataBOund="GridView2_RowDataBound">
                                                    <Columns>
                                                        <asp:BoundField DataField="PK_PersonID" HeaderText="Person ID" Visible="false" SortExpression="PK_PersonID"/>
                                                        <asp:TemplateField SortExpression="OpportunityName" ItemStyle-Width="33%" HeaderText="Opportunity Name">
                                                            <ItemTemplate>
                                                                <asp:HyperLink ID="lnkID" runat="server" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="OpportunityName" HeaderText="Opportunity Name" Visible="true" SortExpression="OpportunityName" ItemStyle-Width="33%"/>
                                                        <asp:BoundField DataField="myStart" HeaderText="Start" Visible="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="myStart"/>
                                                        <asp:BoundField DataField="myEnd" HeaderText="End" Visible="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="myEnd"/>
                                                        <asp:BoundField DataField="RollOffType" HeaderText="Scheduled Type" Visible="true" SortExpression="RollOffType"/>
                                                        <asp:BoundField DataField="PeriodUtilizationRate" DataFormatString="{0:p2}" HeaderText="Utilization %" Visible="true" SortExpression="PeriodUtilizationRate"/>
                                                    </Columns>
                                               </asp:GridView>
                                            </div>
                                         </td>
                                    </tr>
			                    </ItemTemplate>			       
			                </asp:TemplateField>			    

                       </Columns>
                    </asp:gridview >
                    <!--This grid is for exporting only.-->
                    <asp:gridview  ID="grdItemsForExport" runat="server" AutoGenerateColumns="False" 
                   SaveButtonID="btnSaveBulkEdit" DataSourceID="" DataKeyNames="PK_PersonID" 
                    Width="80%" Visible="false" EmptyDataText="No Updates Yet" AllowSorting="true" >
                        <Columns>
                            <asp:BoundField DataField="LastName" HeaderText="Last Name" Visible="True" SortExpression="LastName"/>
                            <asp:BoundField DataField="FirstName" HeaderText="First Name" Visible="True" SortExpression="FirstName"/>
                            <asp:BoundField DataField="ActualRollOff" HeaderText="Actual/Booked Roll Off" Visible="true" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="ActualRollOff"/>
                            <asp:BoundField DataField="BookedRollOff" HeaderText="Forecasted Roll Off" Visible="True" DataFormatString="{0:d}" ItemStyle-HorizontalAlign="Right" SortExpression="BookedRollOff"/>
                            <asp:CheckBoxField DataField="Employee" HeaderText="Employee?" Visible="True" SortExpression="Employee"/>
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
