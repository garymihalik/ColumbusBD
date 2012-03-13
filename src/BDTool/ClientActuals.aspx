<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ClientActuals.aspx.vb" Inherits="ClientActuals" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Enter Client Actuals</title>
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
            <div style="text-align:right;float:right"><asp:HyperLink runat="server" ID="help" NavigateUrl="~/Help/ClientActuals/ClientActuals.html" Target="_blank">Help</asp:HyperLink></div>
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
                <ContentTemplate>
                <uc1:BU ID="BU1" runat="server"/>
                <ew:MultiTextListBox runat="server" ID="lstClients" DataSourceid="ODS_Client" DataValueField="ClientID" DataTextFields="Client" DataTextFormatString="{0:g}" SelectionMode="Single" Rows="15" AutoPostBack="true"/><br />
                <asp:GridView ID="grdItems" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="ODS_Item" ShowFooter="True" DataKeyNames="ActualsID" 
                            Width="90%" EmptyDataText="No Entries Yet" Visible="false" OnDataBound="ShowFooterCode">
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
<asp:TemplateField Visible="false" HeaderText="Actual ID"><EditItemTemplate>	<asp:TextBox ID="txtActualsID" runat="server" Text='<%# Bind("ActualsID") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblActualsID" runat="server" Text='<%# Bind("ActualsID") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtActualsID" runat="server" Text='<%# Bind("ActualsID") %>' /></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="false" HeaderText="Client Name"><EditItemTemplate>	<asp:TextBox ID="txtClient" runat="server" Text='<%# Bind("Client") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblClient" runat="server" Text='<%# Bind("Client") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtClient" runat="server" Text='<%# Bind("Client") %>' /></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Revenue" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtActualRevenue" runat="server" Text='<%# Bind("ActualRevenue") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblActualRevenue" runat="server" Text='<%# Bind("ActualRevenue","{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtActualRevenue" runat="server" Text='<%# Bind("ActualRevenue") %>' CssClass="DateTextBox"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Cost" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtActualCost" runat="server" Text='<%# Bind("ActualCost") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblActualCost" runat="server" Text='<%# Bind("ActualCost","{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtActualCost" runat="server" Text='<%# Bind("ActualCost") %>' CssClass="DateTextBox"/></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="false" HeaderText="Client ID"><EditItemTemplate>	<asp:TextBox ID="txtClientID" runat="server" Text='<%# Bind("ClientID") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblClientID" runat="server" Text='<%# Bind("ClientID") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtClientID" runat="server" Text='<%# Bind("ClientID") %>' /></FooterTemplate></asp:TemplateField>
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
                                    <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Account Group" CommandName="DELETE" CommandArgument='<%# Bind("ActualsID") %>'/>
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
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Item" runat="server"  SelectMethod="GetClientActuals" TypeName="cActuals" DeleteMethod="DeleteSingleClientActual">
        <SelectParameters>
                <asp:ControlParameter ControlID="lstClients" DefaultValue="-1" Name="ClientID" PropertyName="SelectedValue" Type="Double" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_Client" runat="server" SelectMethod="GetAllClientsByBU" TypeName="cAccountClientIndustry">
        <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_ReportDates" runat="server" SelectMethod="GetReportMonths" TypeName="cForecast"/>
    </form>
</body>
</html>
