<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ExceptionManagement.aspx.vb" Inherits="ExceptionManagement" EnableEventValidation="false"%>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error Management</title>
<!--[if IE]> <style type="text/css">#content1{clear:right;float:left; }</style> <![endif]-->
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo.gif" />Exception Management<br />
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="ErrorList"/>
        </div>
        <div id="content1" style="width:100%" >
            <asp:ObjectDataSource ID="ODS_ErrorList" runat="server" TypeName="ExceptionHandler.GetExceptions" SelectMethod="GetListOfErrors" >
                <SelectParameters>
                    <asp:Parameter defaultValue="" Type="String" Name="strModuleName" />
                    <asp:Parameter defaultValue="" Type="String" Name="sortColumns" />
                </SelectParameters>
            </asp:ObjectDataSource>
            <asp:GridView runat="server" id="grdErrorList" AutoGenerateColumns="false" EmptyDataText="No Records Found"
                AllowPaging="true" PageSize="10" AllowSorting="True" DataSourceID="ODS_ErrorList" 
                DataKeyNames="EventID" EnableViewState="True" SelectedRowStyle-CssClass="selectedrow">
                <PagerStyle HorizontalAlign="Right" />
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
                    <asp:BoundField DataField="EventID" SortExpression="Event ID" Visible="True" HtmlEncode="false"/>
                    <asp:BoundField DataField="Referer" HeaderText="Referer" SortExpression="Referer" HtmlEncode="false"/>
                    <asp:BoundField DataField="LogDateTime" HeaderText="Created On" SortExpression="LogDateTime" DataFormatString="{0:g}" HtmlEncode="false" />
                    <asp:BoundField DataField="Source" HeaderText="Source" SortExpression="Source" HtmlEncode="false"/>
                    <asp:BoundField DataField="Message" HeaderText="Message" SortExpression="Message" HtmlEncode="false"/>
                    <asp:BoundField DataField="Form" HeaderText="Form" SortExpression="Form" HtmlEncode="false"/>
                    <asp:BoundField DataField="QueryString" HeaderText="QueryString" SortExpression="QueryString" HtmlEncode="false"/>
                    <asp:BoundField DataField="TargetSite" HeaderText="TargetSite" SortExpression="TargetSite" HtmlEncode="false"/>
                    <asp:BoundField DataField="StackTrace" HeaderText="StackTrace" SortExpression="StackTrace" HtmlEncode="false"/>
                </Columns>
            </asp:GridView>
        </div>
        <div id="footer">
             <p id="copyright">&copy; 2009 Joseph Ours All Rights Reserved. </p><br />
        </div>
    </form>
</body>
</html>
