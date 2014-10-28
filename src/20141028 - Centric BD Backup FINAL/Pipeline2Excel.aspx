<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Pipeline2Excel.aspx.vb" Inherits="DatagridDefault" EnableEventValidation="false" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="ExportToExcelImage" Namespace="Jours.Web.Controls" TagPrefix="JO" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Export Pipeline</title>
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
            <asp:UpdatePanel ID="pnlUpdate" runat="server" ChildrenAsTriggers="true" UpdateMode="Always">
                <ContentTemplate> 
                    <JO:ExportToExcelImage ID="ExportToExcel1" runat="server" ApplyStyleInExcel="True" Charset="utf-8"
                        ContentEncoding="windows-1250" EnableHyperLinks="False" ExportFileName="FileName.xls"
                        IncludeTimeStamp="True" PageSize="All" AlternateText="Export To Excel" ImageUrl="~/commonimages/export_2.gif" 
                        Width="2%" Height="2%" GridViewID="grdItems" Enabled="true"/>               
                    <uc1:BU ID="BU1" runat="server"/>
                    <asp:RadioButtonList ID="RadioButtonList1" runat="server" AutoPostBack="true">
                        <asp:ListItem Text="Working Pipeline" Value="Working Pipeline" Selected="True"/>
                        <asp:ListItem Text="Needing Updates" Value="Needing Updates" />
                        <asp:ListItem Text="Unqualified" Value="Un-qualified" />
                        <asp:ListItem Text="Include Win/Loss in Past 7 Days" Value="WinLoss" />
                        <asp:ListItem Text="All" Value="All" />
                    </asp:RadioButtonList>
                    <asp:CheckBox ID="ckExcludeExtensions" runat="server" Checked="false" AutoPostBack="true" Text="Filter out extensions"/>  
                    <asp:Button runat="server" ID="btnUpdate" Text="Generate Report" />
             
                    <asp:GridView ID="grdItems" runat="server" AutoGenerateColumns="False" AllowSorting="true"
                            ShowFooter="True" DataKeyNames="PK_OpportunityID" DataSourceID="ODS_Item"
                            Width="90%" EmptyDataText="No Data Or Report Not Run" Visible="true" OnDataBound="ShowFooterCode" skinid="Export">
                        <Columns>
                            <asp:TemplateField Visible="true" HeaderText="Owner" SortExpression="OpportunityOwner">
                                <EditItemTemplate>
                                    <asp:Label ID="lblOpportunityOwner" runat="server" Text='<%# Bind("OpportunityOwner") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblOpportunityOwner" runat="server" Text='<%# Bind("OpportunityOwner") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblOpportunityOwner" runat="server" Text='<%# Bind("OpportunityOwner") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Client" SortExpression="Client">
                                <EditItemTemplate>
                                    <asp:Label ID="lblClient" runat="server" Text='<%# Bind("Client") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblClient" runat="server" Text='<%# Bind("Client") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblClient" runat="server" Text='<%# Bind("Client") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Opportunity Name" SortExpression="OpportunityName">
                                <EditItemTemplate>
                                    <asp:Label ID="lblOpportunityName" runat="server" Text='<%# Bind("OpportunityName") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblOpportunityName" runat="server" Text='<%# Bind("OpportunityName") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblOpportunityName" runat="server" Text='<%# Bind("OpportunityName") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Fit" SortExpression="Fit">
                                <EditItemTemplate>
                                    <asp:Label ID="lblFit" runat="server" Text='<%# Bind("Fit") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblFit" runat="server" Text='<%# Bind("Fit") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblFit" runat="server" Text='<%# Bind("Fit") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Anchor?" SortExpression="Anchor">
                                <EditItemTemplate>
                                    <asp:Label ID="lblAnchor" runat="server" Text='<%# Bind("Anchor") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblAnchor" runat="server" Text='<%# Bind("Anchor") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblAnchor" runat="server" Text='<%# Bind("Anchor") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Source?" SortExpression="Source">
                                <EditItemTemplate>
                                    <asp:Label ID="lblSource" runat="server" Text='<%# Bind("Source") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblSource" runat="server" Text='<%# Bind("Source") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblSource" runat="server" Text='<%# Bind("Source") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Date Entered" SortExpression="[Date Entered]"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblDateEntered" runat="server" Text='<%# Bind("[Date Entered]") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDateEntered" runat="server" Text='<%# Bind("[Date Entered]","{0:d}") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblDateEntered" runat="server" Text='<%# Bind("[Date Entered]") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Close Date" SortExpression="OpportunityCloseDate"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblOpportunityCloseDate" runat="server" Text='<%# Bind("OpportunityCloseDate") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblOpportunityCloseDate" runat="server" Text='<%# Bind("OpportunityCloseDate","{0:d}") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblOpportunityCloseDate" runat="server" Text='<%# Bind("OpportunityCloseDate") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Days Open" SortExpression="DaysOpen"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblDaysOpen" runat="server" Text='<%# Bind("DaysOpen") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblDaysOpen" runat="server" Text='<%# Bind("DaysOpen") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblDaysOpen" runat="server" Text='<%# Bind("DaysOpen") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Win %" SortExpression="WinPercentage"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblWinPercentage" runat="server" Text='<%# Bind("WinPercentage") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblWinPercentage" runat="server" Text='<%# Bind("WinPercentage","{0:p0}") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblWinPercentage" runat="server" Text='<%# Bind("WinPercentage") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Est. Revenue" SortExpression="EstimatedRevenue"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue","{0:c0}") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblEstimatedRevenue" runat="server" Text='<%# Bind("EstimatedRevenue") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Weight Rev" SortExpression="WeightedRevenue"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblWeightedRevenue" runat="server" Text='<%# Bind("WeightedRevenue") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblWeightedRevenue" runat="server" Text='<%# Bind("WeightedRevenue","{0:c0}") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblWeightedRevenue" runat="server" Text='<%# Bind("WeightedRevenue") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Next Steps" SortExpression="NextSteps">
                                <EditItemTemplate>
                                    <asp:Label ID="lblNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblNextSteps" runat="server" Text='<%# Bind("NextSteps") %>' /></FooterTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField Visible="true" HeaderText="Roles Needed" SortExpression="RolesNeeded">
                                <EditItemTemplate>
                                    <asp:Label ID="lblRolesNeeded" runat="server" Text='<%# Bind("RolesNeeded") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRolesNeeded" runat="server" Text='<%# Bind("RolesNeeded") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblRolesNeeded" runat="server" Text='<%# Bind("RolesNeeded") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Roles Needed" SortExpression="RFPRequired">
                                <EditItemTemplate>
                                    <asp:Label ID="lblRFPRequired" runat="server" Text='<%# Bind("RFPRequired") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRFPRequired" runat="server" Text='<%# Bind("RFPRequired") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblRFPRequired" runat="server" Text='<%# Bind("RFPRequired") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="RFP Lead" SortExpression="RFPLead">
                                <EditItemTemplate>
                                    <asp:Label ID="lblRFPLead" runat="server" Text='<%# Bind("RFPLead") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRFPLead" runat="server" Text='<%# Bind("RFPLead") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblRFPLead" runat="server" Text='<%# Bind("RFPLead") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Risk Assessment Location" SortExpression="RFPRiskAssessment">
                                <EditItemTemplate>
                                    <asp:Label ID="lblRFPRiskAssessment" runat="server" Text='<%# Bind("RFPRiskAssessment") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblRFPRiskAssessment" runat="server" Text='<%# Bind("RFPRiskAssessment") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblRFPRiskAssessment" runat="server" Text='<%# Bind("RFPRiskAssessment") %>' /></FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Last Update Date" SortExpression="UpdateDate"
                                ItemStyle-HorizontalAlign="Right">
                                <EditItemTemplate>
                                    <asp:Label ID="lblUpdateDate" runat="server" Text='<%# Bind("UpdateDate","{0:d}") %>' /></EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblUpdateDate" runat="server" Text='<%# Bind("UpdateDate","{0:d}") %>' /></ItemTemplate>
                                <FooterTemplate>
                                    <asp:Label ID="lblUpdateDate" runat="server" Text='<%# Bind("UpdateDate","{0:d}") %>' /></FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:PostBackTrigger ControlID="ExportToExcel1" />
                </Triggers>
           </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Item" runat="server"  SelectMethod="GetWorkingPipeLineForExcelByBU" TypeName="cPipelineData">
        <SelectParameters>
            <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
            <asp:ControlParameter ControlID="RadioButtonList1" DefaultValue="Working Pipeline" Name="strFilter" PropertyName="SelectedValue" Type="String" />
            <asp:ControlParameter ControlID="ckExcludeExtensions" DefaultValue="Unchecked" Name="bExcludeExclusions" PropertyName="Checked" Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
</html>
