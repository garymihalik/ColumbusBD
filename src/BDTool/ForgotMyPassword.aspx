<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ForgotMyPassword.aspx.vb" Inherits="ForgotMyPassword" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Password Retrieval Page</title>
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
            <asp:PasswordRecovery ID="PasswordRecovery1" runat="server">
            </asp:PasswordRecovery>
        </div>
        <div id="footer">
            <p id="copyright">&copy; 2009 Centric Consulting. All Rights Reserved. </p><br />
        </div>
    </form>
</body>
</html>
