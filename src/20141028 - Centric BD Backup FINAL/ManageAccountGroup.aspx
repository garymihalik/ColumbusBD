<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ManageAccountGroup.aspx.vb" Inherits="ManageAccountGroup" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc2" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Account Groups List</title>
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
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <cc2:BulkEditGridView ID="grdAccountGroupItems" runat="server" AutoGenerateColumns="False" ShowFooter="true" Width="100%" Visible="True" 
                    DataSourceID="ODS_AccountGroup" DataKeyNames="ACCOUNTGROUPID" SaveButtonID="btnSaveBulkEdit">
                        <Columns>
                            <asp:TemplateField HeaderText="ACCOUNTGROUPID" Visible="false">
                                <FooterTemplate>
                                    <asp:Label ID="lblACCOUNTGROUPID" Text="" runat="server"></asp:Label>
                                </FooterTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lblACCOUNTGROUPID" Text='<%# Bind("ACCOUNTGROUPID") %>' runat="server"></asp:Label>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblACCOUNTGROUPID" Text='<%# Bind("ACCOUNTGROUPID") %>' runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Account Group Name">
                                <FooterTemplate>
                                    <asp:TextBox ID="txtACCOUNTGROUP" Text='<%# Bind("ACCOUNTGROUP") %>' runat="server" Width="95%"></asp:TextBox>
                                </FooterTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtACCOUNTGROUP" Text='<%# Bind("ACCOUNTGROUP") %>' runat="server" Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblACCOUNTGROUP" Text='<%# Bind("ACCOUNTGROUP") %>' runat="server" Width="95%"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Account Owner">
                                <FooterTemplate>
                                    <asp:TextBox ID="txtAccountOwner" Text='<%# Bind("AccountOwner") %>' runat="server" Width="95%" ></asp:TextBox>
                                </FooterTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtAccountOwner" Text='<%# Bind("AccountOwner") %>' runat="server" Width="95%" ></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblAccountOwner" Text='<%# Bind("AccountOwner") %>' runat="server" Width="95%" ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
		                    <asp:TemplateField HeaderText="Forecast Owner">
                                <FooterTemplate>
                                    <asp:TextBox ID="txtForecastOwner" Text='<%# Bind("ForecastOwner") %>' runat="server" Width="95%"  ></asp:TextBox>
                                </FooterTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtForecastOwner" Text='<%# Bind("ForecastOwner") %>' runat="server" Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblForecastOwner" Text='<%# Bind("ForecastOwner") %>' runat="server" Width="95%"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
		                    <asp:TemplateField HeaderText="Report On?">
                                <FooterTemplate>
                                    <asp:CheckBox ID="ckReportOn" Checked='<%# IIf(Eval("ReportOn") Is DBNull.Value, "False", Eval("ReportOn")) %>' runat="server" Width="95%" ></asp:CheckBox>
                                </FooterTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox ID="ckReportOn" Checked='<%# IIf(Eval("ReportOn") Is DBNull.Value, "False", Eval("ReportOn")) %>' runat="server" Width="95%" ></asp:CheckBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckReportOn" Checked='<%# Bind("ReportOn") %>' runat="server" Width="95%" ></asp:CheckBox>
                                </ItemTemplate>
                            </asp:TemplateField>
		                    <asp:TemplateField HeaderText="Reporting Order">
                                <FooterTemplate>
                                    <asp:TextBox ID="txtReportOrder" Text='<%# Bind("ReportOrder") %>' runat="server" Width="95%"></asp:TextBox>
                                </FooterTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtReportOrder" Text='<%# Bind("ReportOrder") %>' runat="server" Width="95%"></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblReportOrder" Text='<%# Bind("ReportOrder") %>' runat="server" Width="95%"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
		                    <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lnkDeleteAccountGroup" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Account Group" CommandName="DELETE" CommandArgument='<%# Bind("ACCOUNTGROUPID") %>'/>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:ImageButton ID="lnkAddAccountGroup" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Account Group" CommandName="INSERT"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </cc2:BulkEditGridView>
                    <asp:Button ID="btnSaveBulkEdit" runat="server" Text="Save Changes" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
<asp:ObjectDataSource ID="ODS_AccountGroup" runat="server" 
SelectMethod="GetAllAccountGroupsByBU" TypeName="cAccountClientIndustry"
InsertMethod="InsertSingleAccountGroup" 
UpdateMethod="UpdateSingleAccountGroup" DeleteMethod="DeleteSingleAccountGroup">
    <SelectParameters>
        <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
    </SelectParameters>
    <UpdateParameters>
        <asp:Parameter Name="ORIGINAL_ACCOUNTGROUPID" Type="Int32" />
        <asp:Parameter Name="AccountGroup" Type="String" />
        <asp:Parameter Name="AccountOwner" Type="string" />
        <asp:Parameter Name="ForecastOwner" Type="string" />
        <asp:Parameter Name="ReportOn" Type="Boolean" />
	<asp:Parameter Name="ReportOrder" Type="Int32" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="AccountGroup" Type="String" />
        <asp:Parameter Name="AccountOwner" Type="string" />
        <asp:Parameter Name="ForecastOwner" Type="string" />
        <asp:Parameter Name="ReportOn" Type="Boolean" />
	<asp:Parameter Name="ReportOrder" Type="Int32" />
    </InsertParameters>
    <DeleteParameters>
        <asp:Parameter DefaultValue="-1" Name="ACCOUNTGROUPID" Type="Int32" />
    </DeleteParameters>
</asp:ObjectDataSource>
    </form>
</body>
</html>
