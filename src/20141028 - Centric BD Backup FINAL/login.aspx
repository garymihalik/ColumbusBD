<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Login" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Centric Login Page</title>
</head>
<body>
    <form id="form1" runat="server">
       <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <div id="header" >
            <span style="text-align:right;float:right"></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br/>
        </div>
        <div id="leftcol" >
            
        </div>
        <div id="content" >
            Content
            <asp:Login ID="Login1" runat="server" DestinationPageUrl="~/default.aspx" 
                RememberMeSet="false" DisplayRememberMe="false" 
                PasswordRecoveryText="Forgotten Password?" 
                PasswordRecoveryUrl="~/ForgotMyPassword.aspx">
            </asp:Login>
            <asp:Label runat="server" ID="LoginErrorDetails" />
            <hr />
            </div>
        <div id="footer">
             <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    </form>

</body>
</html>
