<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ForecastingUpdates.aspx.vb" Inherits="ForecastingUpdates" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forecast Updates</title>
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
            <div style="float:right;text-align:right">
            <asp:HyperLink ID="lnkAI" runat="server" NavigateUrl="~/ForecastingUpdatesAdvanced.aspx" skinID="NavLink" Text="Manage Client Forecast - Advanced"></asp:HyperLink><br />
            <asp:HyperLink runat="server" ID="HyperLink1" NavigateUrl="~/Help/WonAndForecast/WonAndForecast.html" Target="_blank">Help with Basic Forecasting</asp:HyperLink><br />
            <asp:HyperLink runat="server" ID="help" NavigateUrl="~/Help/AdvancedForecasting/AdvancedForecasting.html" Target="_blank">Help with Advanced Forecasting</asp:HyperLink></div>
            <asp:UpdatePanel runat="server" ID="pnlAccounts" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <ew:MultiTextDropDownList runat="server" ID="cboAccounts" DataSourceid="ODS_AccountGroup" DataValueField="AccountGroupID" DataTextFields="AccountGroup" DataTextFormatString="{0:g}" AutoPostBack="true"/><br />
                    <asp:DropDownList ID="RadioButtonList1" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="Active (Won with ongoing assignments)" Value="Active" Selected="True"/>
                        <asp:ListItem Text="Won in Last 12 Months" Value="Won12months"/>
                        <asp:ListItem Text="Won in Last 6 Months" Value="Won6months" />
                        <asp:ListItem Text="Won in Last 3 Months" Value="Won3months" />
                        <asp:ListItem Text="Won with 0 Resources Assigned" Value="Won0Resources" />
                        <asp:ListItem Text="All Won" Value="AllWon" />
                        <asp:ListItem Text=">70% in Last 12 Months" Value="70_12months"/>
                        <asp:ListItem Text=">70% in Last 6 Months" Value="70_6months" />
                        <asp:ListItem Text=">70% in Last 3 Months" Value="70_3months" />
                        <asp:ListItem Text=">70% with 0 Resources Assigned" Value="70_0Resources" />
                        <asp:ListItem Text=">10% in Last 12 Months" Value="10_12Resources" />
                        <asp:ListItem Text=">10% in Last 6 Months" Value="10_6months" />
                        <asp:ListItem Text=">10% in Last 3 Months" Value="10_3months" />
                        <asp:ListItem Text=">10% with 0 Resources Assigned" Value="10_0Resources" />
                        <asp:ListItem Text="All Entries with Assignments" Value="AllAssignments" />
                        <asp:ListItem Text="All Entries" Value="All" />
                    </asp:DropDownList>
                </ContentTemplate>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="pnlWonOpportunities" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <ew:MultiTextListBox runat="server" ID="lstOpportunities" DataSourceid="ODS_AccountGroupWins" DataValueField="PK_OpportunityID" DataTextFields="Client, OpportunityName, UpdateDate" DataTextFormatString="{0:g}~{1:g}~{2:d}" SelectionMode="Single" Rows="15" AutoPostBack="true" Font-Names="Courier New">
                    </ew:MultiTextListBox><br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="BU1" EventName="BUSelectionChanged" />
                </Triggers>
            </asp:UpdatePanel>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <asp:GridView ID="grdAssignments" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="ODS_Assignments" ShowFooter="True" DataKeyNames="AssignmentID" 
                            Width="90%" EmptyDataText="No Assignments Yet" Visible="false">
                        <Columns>
                            <asp:BoundField DataField="AssignmentID" HeaderText="AssignmentID" Visible="False"/>
                            <asp:BoundField DataField="FK_OpportunityID" HeaderText="FK_OpportunityID" Visible="False"/>
                            <asp:TemplateField HeaderText="Resource ID" SortExpression="FK_PersonID" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblPersonID" runat="server" Text='<%# Eval("FK_PersonID") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblPersonID" runat="server" Text='<%# Eval("FK_PersonID") %>' />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" FooterStyle-HorizontalAlign="Left" HeaderText="Resource Name" SortExpression="PersonName">
                                <ItemTemplate>
                                    <asp:Label ID="lblPersonName" runat="server" Text='<%# eval("PersonName","{0:g}") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblPersonName" runat="server" Text='<%# eval("PersonName","{0:g}") %>' />
                                    <ew:MultiTextDropDownList runat="server" ID="cboPeople" DataSourceid="ODS_People" DataValueField="PK_PersonID" DataTextFields="FirstName,LastName" DataTextFormatString="{0:g} {1:g}" AutoPostBack="false" OnDataBound="SetUpInitialEditValue" Visible="false"/>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <ew:MultiTextDropDownList runat="server" ID="cboPeople" DataSourceid="ODS_People" DataValueField="PK_PersonID" DataTextFields="FirstName,LastName" DataTextFormatString="{0:g} {1:g}" AutoPostBack="false"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="Start Date" SortExpression="UpdateDate">
                                <ItemTemplate>
                                    <asp:Label ID="lblStartDate" runat="server" Text='<%# Eval("StartDate","{0:d}") %>' CssClass="DateTextBox"  />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Textbox ID="txtStartDate" runat="server" Text='<%# Bind("StartDate","{0:d}")%>' CssClass="DateTextBox" Width="95%" />
                                    <cc1:CalendarExtender ID="ccUpdateDate" runat="server" TargetControlID="txtStartDate" />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Textbox ID="txtStartDate" runat="server" Text='<%# Bind("StartDate","{0:d}")%>' CssClass="DateTextBox" Width="95%" />
                                    <cc1:CalendarExtender ID="ccUpdateDate" runat="server" TargetControlID="txtStartDate" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="End Date" SortExpression="OpportunityCloseDate">
                                <ItemTemplate>
                                    <asp:Label ID="lblEndDate" runat="server" Text='<%# Eval("EndDate","{0:d}") %>' CssClass="DateTextBox"/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Textbox ID="txtEndDate" runat="server" Text='<%# Bind("EndDate","{0:d}") %>' CssClass="DateTextBox" Width="95%"/>
                                    <cc1:CalendarExtender ID="ccCloseDate" runat="server" TargetControlID="txtEndDate" />
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Textbox ID="txtEndDate" runat="server" Text='<%# Bind("EndDate","{0:d}") %>' CssClass="DateTextBox" Width="95%"/>
                                    <cc1:CalendarExtender ID="ccCloseDate" runat="server" TargetControlID="txtEndDate" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Bill Rate" SortExpression="Bill Rate" FooterStyle-HorizontalAlign="Right" >
                                <ItemTemplate>
                                    <asp:Label ID="lblBillRate" runat="server" Text='<%# Bind("BillRate","{0:c}") %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Textbox ID="txtBillRate" runat="server" Text='<%# Bind("BillRate") %>' CssClass="DateTextBox" Width="95%"/>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Textbox ID="txtBillRate" runat="server" Text='<%# Bind("BillRate") %>' CssClass="DateTextBox" Width="95%"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="1099 Costs" SortExpression="Costs" FooterStyle-HorizontalAlign="Right" >
                                <ItemTemplate>
                                    <asp:Label ID="lblCosts" runat="server" Text='<%# Bind("Costs","{0:c}") %>' CssClass="DateTextBox"/>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Textbox ID="txtCosts" runat="server" Text='<%# Bind("Costs") %>' CssClass="DateTextBox" Width="95%"/>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Textbox ID="txtCosts" runat="server" Text='<%# Bind("Costs") %>' CssClass="DateTextBox" Width="95%"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Utilization" SortExpression="PeriodUtilizationRate" FooterStyle-HorizontalAlign="Right" >
                                <ItemTemplate>
                                    <asp:Label ID="lblPeriodUtilizationRate" runat="server" Text='<%# Bind("PeriodUtilizationRate","{0:p}") %>' CssClass="DateTextBox" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Textbox ID="txtPeriodUtilizationRate" runat="server" Text='<%# Bind("PeriodUtilizationRate") %>' CssClass="DateTextBox" Width="70%"/>%
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:Textbox ID="txtPeriodUtilizationRate" runat="server" Text='<%# Bind("PeriodUtilizationRate") %>' CssClass="DateTextBox" Width="70%"/>%
                                </FooterTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField Visible="true" ItemStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center" HeaderText="Exclude From BU Revenue?">
                                <EditItemTemplate>	
                                    <asp:Checkbox ID="ckExcludeFromBURevenueCalculation" runat="server" Checked='<%# Bind("ExcludeFromBURevenueCalculation") %>' />
                                </EditItemTemplate>	
                                <ItemTemplate>
                                    <asp:Checkbox ID="ckExcludeFromBURevenueCalculation" runat="server" Checked='<%# IIf(Eval("ExcludeFromBURevenueCalculation") Is DBNull.Value, "False", Eval("ExcludeFromBURevenueCalculation")) %>' Enabled="false"/>
                                </ItemTemplate>	
                                <FooterTemplate>
                                    <asp:Checkbox ID="ckExcludeFromBURevenueCalculation" runat="server" Checked='<%# Bind("ExcludeFromBURevenueCalculation") %>'/>
                                </FooterTemplate>
                            </asp:TemplateField>
                            
                            <asp:TemplateField ItemStyle-HorizontalAlign="Right" HeaderText="Resource Revenue" SortExpression="CalculatedRevenue" FooterStyle-HorizontalAlign="Right" >
                                <ItemTemplate>
                                    <asp:Label ID="lblCalculatedRevenue" runat="server" Text='<%# Eval("CalculatedRevenue","{0:c}") %>' CssClass="DateTextBox" />
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:CommandField ButtonType="Image" EditImageUrl="~/CommonImages/edit.gif" DeleteImageUrl="~/CommonImages/delete.gif" UpdateImageUrl="~/CommonImages/save.gif" CancelImageUrl="~/CommonImages/undo.gif" InsertImageUrl="~/CommonImages/save.gif" ShowDeleteButton="false" ShowEditButton="True" ShowCancelButton="true" NewImageUrl="~/CommonImages/Insert.GIF" ItemStyle-Width="5%" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lnkDeleteAssignment" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Account Group" CommandName="DELETE" CommandArgument='<%# Bind("AssignmentID") %>'/>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:ImageButton ID="lnkInsertUpdate" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Update" CommandName="INSERT"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                       </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="lstOpportunities" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="BU1" EventName="BUSelectionChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Centric Consulting. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_AccountGroup" runat="server" SelectMethod="GetAllAccountGroupsByBU" TypeName="cAccountClientIndustry" >
         <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource> 
    <asp:ObjectDataSource ID="ODS_AccountGroupWins" runat="server" SelectMethod="GetWonOpportunitiesByAccountGroup" TypeName="cOpportunity">
        <SelectParameters>
            <asp:ControlParameter ControlID="cboAccounts" DefaultValue="-1" Name="AccountGroupID" PropertyName="SelectedValue" Type="Double" />
            <asp:ControlParameter ControlID="RadioButtonList1" DefaultValue="" Name="Filter" PropertyName="SelectedValue" Type="String" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_Assignments" runat="server" SelectMethod="GetAssignments" UpdateMethod="UpdateSingleAssignment"
            TypeName="cAssignments" InsertMethod="InsertNewAssignment" DeleteMethod="DeleteSingleAssignment">
        <DeleteParameters>
            <asp:Parameter Name="AssignmentID" Type="Int32" />
        </DeleteParameters>
        <SelectParameters>
            <asp:ControlParameter ControlID="lstOpportunities" DefaultValue="-1" Name="OpportunityID" PropertyName="SelectedValue" Type="Double" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="AssignmentID" Type="Double" />
            <asp:Parameter Name="FK_OpportunityID" Type="Double" />
            <asp:Parameter Name="FK_PersonID" Type="Double" />
            <asp:Parameter Name="StartDate" Type="DateTime" />
            <asp:Parameter Name="EndDate" Type="DateTime" />
            <asp:Parameter Name="BillRate" Type="Double" />
            <asp:Parameter Name="Costs" Type="Double" />
            <asp:Parameter Name="PeriodUtilizationRate" Type="Double" />
        </UpdateParameters>
        <InsertParameters>
            <asp:Parameter Name="FK_OpportunityID" Type="Double" />
            <asp:Parameter Name="FK_PersonID" Type="Double" />
            <asp:Parameter Name="StartDate" Type="DateTime" />
            <asp:Parameter Name="EndDate" Type="DateTime" />
            <asp:Parameter Name="BillRate" Type="Double" />
            <asp:Parameter Name="Costs" Type="Double" />
            <asp:Parameter Name="PeriodUtilizationRate" Type="Double" />
            <asp:Parameter Name="NewItemID" Type="Double" />
        </InsertParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_People" runat="server" SelectMethod="GetPeopleByBU" TypeName="cAssignments">
        <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
</html>
