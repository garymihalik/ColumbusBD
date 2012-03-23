<%@ Page Language="VB" AutoEventWireup="true" CodeFile="MonthlyGlobalActuals.aspx.vb" Inherits="MonthlyGlobalActuals" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Global Actuals Information</title>
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
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
                <ContentTemplate>
                <uc1:BU ID="BU1" runat="server"/>
                <asp:GridView ID="grdItems" runat="server" AutoGenerateColumns="False" 
                        DataSourceID="ODS_Item" ShowFooter="True" DataKeyNames="GlobalMonthlyID" 
                        Width="90%" EmptyDataText="No Entries Yet" Visible="True" OnDataBound="ShowFooterCode">
                    <Columns>
                        <asp:TemplateField Visible="true" HeaderText="Report Month">
                            <EditItemTemplate>
                                <ew:MultiTextDropDownList runat="server" ID="cboReportMonth" DataSourceid="ODS_ReportDates" DataValueField="ReportOrder" DataTextFields="MontlyReportDates,Year" DataTextFormatString="{0:g} {1:g}" OnDataBound="SetUpInitialEditValue" Enabled="true"/>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <ew:MultiTextDropDownList runat="server" ID="cboReportMonth" DataSourceid="ODS_ReportDates" DataValueField="ReportOrder" DataTextFields="MontlyReportDates,Year" DataTextFormatString="{0:g} {1:g}" OnDataBound="SetUpInitialEditValue" Enabled="false"/>
                            </ItemTemplate>
                            <FooterTemplate>
                                <ew:MultiTextDropDownList runat="server" ID="cboReportMonth" DataSourceid="ODS_ReportDates" DataValueField="ReportOrder" DataTextFields="MontlyReportDates,Year" DataTextFormatString="{0:g} {1:g}" OnDataBound="SetUpCurrentMonth" Enabled="true"/>
                            </FooterTemplate>
                        </asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Expenses" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" ><EditItemTemplate>	<asp:TextBox ID="txtExpenses" runat="server" Text='<%# Bind("Expenses") %>' CssClass="DateTextBox" width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblExpenses" runat="server" Text='<%# Bind("Expenses" ,"{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtExpenses" runat="server" Text='<%# Bind("Expenses") %>' width="95%"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Transfers In/Out" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtTransfersInOut" runat="server" Text='<%# Bind("TransfersInOut") %>' CssClass="DateTextBox" width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblTransfersInOut" runat="server" Text='<%# Bind("TransfersInOut" ,"{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtTransfersInOut" runat="server" Text='<%# Bind("TransfersInOut") %>' width="95%"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Misc COGS" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtMiscCOGS" runat="server" Text='<%# Bind("MiscCOGS") %>' CssClass="DateTextBox" width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblMiscCOGS" runat="server" Text='<%# Bind("MiscCOGS" ,"{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtMiscCOGS" runat="server" Text='<%# Bind("MiscCOGS") %>' width="95%"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="FTE Count" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtForecastedFTECount" runat="server" Text='<%# Bind("ForecastedFTECount") %>' CssClass="DateTextBox" width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblForecastedFTECount" runat="server" Text='<%# Bind("ForecastedFTECount") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtForecastedFTECount" runat="server" Text='<%# Bind("ForecastedFTECount") %>' width="95%"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="false" HeaderText="GlobalMonthlyID" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtGlobalMonthlyID" runat="server" Text='<%# Bind("GlobalMonthlyID") %>' CssClass="DateTextBox" width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblGlobalMonthlyID" runat="server" Text='<%# Bind("GlobalMonthlyID") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtGlobalMonthlyID" runat="server" Text='<%# Bind("GlobalMonthlyID") %>' width="95%"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Actual Profit" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="lblActualProfit" runat="server" Text='<%# Bind("ActualProfit") %>' CssClass="DateTextBox" width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblActualProfit" runat="server" Text='<%# Bind("ActualProfit","{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="lblActualProfit" runat="server" Text='<%# Bind("ActualProfit") %>' width="95%"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Transfer Notes"><EditItemTemplate>	<asp:TextBox ID="txtTransferNotes" runat="server" Text='<%# Bind("TransferNotes") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblTransferNotes" runat="server" Text='<%# Bind("TransferNotes") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtTransferNotes" runat="server" Text='<%# Bind("TransferNotes") %>' /></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="LastUpdatedBy">	<ItemTemplate><asp:Label ID="lblLastUpdateBy" runat="server" Text='<%# Bind("LastUpdateBy") %>' /></ItemTemplate>	</asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="LastUpdateDate" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><ItemTemplate><asp:Label ID="lblLastUpdateDate" runat="server" Text='<%# Bind("LastUpdateDate","{0:d}") %>' CssClass="DateTextBox"/></ItemTemplate>	</asp:TemplateField>
                        
                        <asp:CommandField ButtonType="Image" EditImageUrl="~/CommonImages/edit.gif" 
                            DeleteImageUrl="~/CommonImages/delete.gif" 
                            UpdateImageUrl="~/CommonImages/save.gif" 
                            CancelImageUrl="~/CommonImages/undo.gif" 
                            InsertImageUrl="~/CommonImages/save.gif" ShowDeleteButton="false" 
                            ShowEditButton="True" ShowCancelButton="true" 
                            NewImageUrl="~/CommonImages/Insert.GIF" ItemStyle-Width="5%" >
                        <ItemStyle Width="5%" />
                        </asp:CommandField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Account Group" CommandName="DELETE" CommandArgument='<%# Bind("GlobalMonthlyID") %>'/>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:ImageButton ID="lnkInsertUpdate" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Update" CommandName="INSERT"/>
                            </FooterTemplate>
                        </asp:TemplateField>
                   </Columns>
                </asp:GridView>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Centric Consulting. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Item" runat="server"  SelectMethod="GetMonthlyItemsByBu" TypeName="cGlobalMonthly" DeleteMethod="DeleteMonthlyReport">
        <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_ReportDates" runat="server" SelectMethod="GetReportMonths" TypeName="cForecast"/>
    </form>
</body>
</html>
