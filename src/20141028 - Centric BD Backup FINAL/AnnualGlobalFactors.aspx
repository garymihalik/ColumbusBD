<%@ Page Language="VB" AutoEventWireup="true" CodeFile="AnnualGlobalFactors.aspx.vb" Inherits="AnnualGlobalFactors" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Global Annual Information</title>
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
                        DataSourceID="ODS_Item" ShowFooter="True" DataKeyNames="GlobalAnnualID" 
                        Width="90%" EmptyDataText="No Entries Yet" Visible="True" OnDataBound="ShowFooterCode">
                    <Columns>
                        <asp:TemplateField Visible="False" HeaderText="ID"><EditItemTemplate>	<asp:Label ID="lblGlobalAnnualID" runat="server" Text='<%# Bind("GlobalAnnualID") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblGlobalAnnualID" runat="server" Text='<%# Bind("GlobalAnnualID") %>' /></ItemTemplate>	<FooterTemplate><asp:Label ID="lblGlobalAnnualID" runat="server" Text='<%# Bind("GlobalAnnualID") %>' /></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Year" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtReportYear" runat="server" Text='<%# Bind("ReportYear") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblReportYear" runat="server" Text='<%# Bind("ReportYear") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtReportYear" runat="server" Text='<%# Bind("ReportYear") %>' /></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Non Salary Employee Factor" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtNonSalaryEmpFactor" runat="server" Text='<%# Bind("NonSalaryEmpFactor") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblNonSalaryEmpFactor" runat="server" Text='<%# Bind("NonSalaryEmpFactor","{0:p2}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtNonSalaryEmpFactor" runat="server" Text='<%# Bind("NonSalaryEmpFactor") %>' /></FooterTemplate></asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Last Updated By"><ItemTemplate><asp:Label ID="lblLastUpdateBy" runat="server" Text='<%# Bind("LastUpdateBy") %>' /></ItemTemplate>	</asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Last Update Date" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><ItemTemplate><asp:Label ID="lblLastUpdateDate" runat="server" Text='<%# Bind("LastUpdateDate","{0:d}") %>' /></ItemTemplate>	</asp:TemplateField>
<asp:TemplateField Visible="true" HeaderText="Average FTE Salary" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right"><EditItemTemplate>	<asp:TextBox ID="txtAvgFTEComp" runat="server" Text='<%# Bind("AvgFTEComp") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblAvgFTEComp" runat="server" Text='<%# Bind("AvgFTEComp","{0:c}") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtAvgFTEComp" runat="server" Text='<%# Bind("AvgFTEComp") %>' /></FooterTemplate></asp:TemplateField>

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
                                <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Account Group" CommandName="DELETE" CommandArgument='<%# Bind("GlobalAnnualID") %>'/>
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
    <asp:ObjectDataSource ID="ODS_Item" runat="server"  SelectMethod="GetAnnualItemsByBu" TypeName="cGlobalAnnual" DeleteMethod="DeleteAnnualReport">
        <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="ODS_ReportDates" runat="server" SelectMethod="GetReportMonths" TypeName="cForecast"/>
    </form>
</body>
</html>
