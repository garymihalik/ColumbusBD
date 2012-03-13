<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ManageWebSiteUsers.aspx.vb" Inherits="ManageWebSiteUsers" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Web Site Users</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <uc7:MyModal ID="myModalControl" runat="server" />
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br/>
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
            <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" AutoGeneratePassword="true" FinishDestinationPageUrl="~/Default.aspx" ContinueDestinationPageUrl="~/managewebsiteusers.aspx" LoginCreatedUser="false">
                <MailDefinition  
                   BodyFileName="~/NewUserEmail.txt"  
                   From="welcome@centricbd.com" 
                   Subject="Welcome to the Centric Business Development site!"/> 
                <WizardSteps>
                <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                </asp:CreateUserWizardStep>
                <asp:WizardStep ID="wzAssignRoles" runat="server">
                    <asp:CheckBoxList runat="server" id="ckRoleList" DataSourceID="ODS_Item" DataTextField="RoleName" DataValueField="RoleID">
                    </asp:CheckBoxList>
                </asp:WizardStep>
                <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server" >
                </asp:CompleteWizardStep>
                </WizardSteps>
            </asp:CreateUserWizard>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
        <asp:ObjectDataSource ID="ODS_Item" runat="server"  SelectMethod="GetListOfASPMEMBERSHIPROLES" TypeName="cJoeUserTroubleshooting">
        </asp:ObjectDataSource>
    </form>
</body>
</html>
