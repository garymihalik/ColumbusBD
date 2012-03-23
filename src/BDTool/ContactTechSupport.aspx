<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ContactTechSupport.aspx.vb" Inherits="ContactTechSupport" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Contact Tech Support</title>
    
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" />
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br />
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
            <asp:Label ID="results" runat="server" Visible="false" />
            We apologize for the inconvenience.  If you would like us to troubleshoot this error with this information, please click the email button below.  
            <asp:Button ID="EmailReport" runat="server" Text="Email This Information To Tech Support"/><br />
            <asp:Literal ID="CookiePlaceHolder1" runat="server"></asp:Literal>
            <asp:Literal ID="SessionPlaceHolder1" runat="server"></asp:Literal>
            <asp:Literal ID="ServerErrorPlaceholder1" runat="server"></asp:Literal>
        </div>
        <div id="footer" >
             <p id="copyright">&copy; 2009 Centric Consulting All Rights Reserved. </p><br />        </div>
    </form>
</body>
</html>
