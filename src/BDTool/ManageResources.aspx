<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ManageResources.aspx.vb" Inherits="ManageResources" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc2" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Manage Resources</title>
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
                    <cc2:BulkEditGridView ID="grdItems" runat="server" AutoGenerateColumns="False" ShowFooter="true"  Visible="True" 
                    DataSourceID="ODS_Item" DataKeyNames="PK_PersonID" SaveButtonID="btnSaveBulkEdit" AllowPaging="true" AllowSorting="true">
                        <PagerStyle HorizontalAlign="Right"/>
                        <PagerTemplate>
                            <asp:Label ID="Label1" runat="server" Text="Show rows:" />
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ChangeNumberDisplayed">
                                <asp:ListItem Value="5" />
                                <asp:ListItem Value="10" Selected="True"/>
                                <asp:ListItem Value="15" />
                                <asp:ListItem Value="20" />
                                <asp:ListItem Value="50" />
                                <asp:ListItem Value="100" />
                                <asp:ListItem Value="250" /><asp:ListItem Value="500" /><asp:ListItem Value="1000" />
                            </asp:DropDownList>
                            &nbsp;
                            Page 
                            <asp:TextBox ID="txtGoToPage" runat="server" AutoPostBack="true" OnTextChanged="GoToPage" Width="20px" Font-Size="X-Small" />
                            of
                            <asp:Label ID="lblTotalNumberOfPages" runat="server" Font-Size="X-Small" />
                            &nbsp;
                            <asp:ImageButton ID="Button1" runat="server" CommandName="Page" ToolTip="Previous Page" CommandArgument="Prev" BackColor="Transparent" ImageUrl="~/commonimages/leftarrow.gif"/>
                            <asp:ImageButton ID="Button2" runat="server" CommandName="Page" ToolTip="Next Page" CommandArgument="Next" BackColor="Transparent" ImageUrl="~/commonimages/rightarrow.gif"/>
                        </PagerTemplate>
                        <Columns>
                            <asp:TemplateField Visible="False" HeaderText="PersonID"><EditItemTemplate>	<asp:Label ID="lblPK_PersonID" runat="server" Text='<%# Bind("PK_PersonID") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblPK_PersonID" runat="server" Text='<%# Bind("PK_PersonID") %>' /></ItemTemplate>	<FooterTemplate><asp:Label ID="lblPK_PersonID" runat="server" Text='<%# Bind("PK_PersonID") %>' /></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Last Name" SortExpression="LastName"><EditItemTemplate>	<asp:TextBox ID="txtLastName" runat="server" Text='<%# Bind("LastName") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblLastName" runat="server" Text='<%# Bind("LastName") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtLastName" runat="server" Text='<%# Bind("LastName") %>' /></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="First Name" SortExpression="FirstName"><EditItemTemplate>	<asp:TextBox ID="txtFirstName" runat="server" Text='<%# Bind("FirstName") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("FirstName") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtFirstName" runat="server" Text='<%# Bind("FirstName") %>' /></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="BU Office" HeaderStyle-Width="75%" SortExpression="Office" ><EditItemTemplate>	<asp:TextBox ID="txtOffice" runat="server" Text='<%# Bind("Office") %>' Width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblOffice" runat="server" Text='<%# Bind("Office") %>' Width="95%"/></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtOffice" runat="server" Text='<%# Bind("Office") %>' Width="95%"/></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Cell Phone" HeaderStyle-Width="75%" SortExpression="Cell" ><EditItemTemplate>	<asp:TextBox ID="txtCell" runat="server" Text='<%# Bind("Cell") %>' Width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblCell" runat="server" Text='<%# Bind("Cell") %>' Width="95%"/></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtCell" runat="server" Text='<%# Bind("Cell") %>' Width="95%"/></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Home Phone" HeaderStyle-Width="75%" SortExpression="Home" ><EditItemTemplate>	<asp:TextBox ID="txtHome" runat="server" Text='<%# Bind("Home") %>' Width="95%"/></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblHome" runat="server" Text='<%# Bind("Home") %>' Width="95%"/></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtHome" runat="server" Text='<%# Bind("Home") %>' Width="95%"/></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Email Address" SortExpression="Email" ><EditItemTemplate>	<asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>' /></ItemTemplate>	<FooterTemplate><asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>' /></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Employee?" SortExpression="Employee" ><EditItemTemplate>	<asp:CheckBox ID="ckEmployee" runat="server" Checked='<%# IIf(Eval("Employee") Is DBNull.Value, "False", Eval("Employee")) %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblEmployee" runat="server" Text='<%# Bind("Employee") %>' /></ItemTemplate>	<FooterTemplate><asp:CheckBox ID="ckEmployee" runat="server" Checked='<%# Bind("Employee") %>' /></FooterTemplate></asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Inactive?" SortExpression="Inactive" ><EditItemTemplate>	<asp:CheckBox ID="ckInactive" runat="server" Checked='<%# IIf(Eval("InActive") Is DBNull.Value, "False", Eval("InActive")) %>' /></EditItemTemplate>	<ItemTemplate><asp:Label ID="lblInactive" runat="server" Text='<%# Bind("Inactive") %>' /></ItemTemplate>	<FooterTemplate><asp:CheckBox ID="ckInactive" runat="server" Checked='<%# Bind("Inactive") %>' /></FooterTemplate></asp:TemplateField>
		                    <asp:TemplateField><ItemTemplate><asp:ImageButton ID="lnkDeleteUser" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Resource" CommandName="DELETE" CommandArgument='<%# Bind("PK_PersonID") %>'/></ItemTemplate><FooterTemplate><asp:ImageButton ID="lnkAddUser" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Resource" CommandName="INSERT"/></FooterTemplate></asp:TemplateField>
                        </Columns>
                    </cc2:BulkEditGridView>
                    <asp:Button ID="btnSaveBulkEdit" runat="server" Text="Save Changes" />
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Item" runat="server" SelectMethod="GetPeopleByBU" TypeName="cUsers" DeleteMethod="DeleteResource" UpdateMethod="UpdateResource" InsertMethod="InsertResource">
        <SelectParameters>
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
</form>
    
</body>
</html>
