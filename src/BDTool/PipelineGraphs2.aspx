﻿<%@ Page Language="VB" AutoEventWireup="true" CodeFile="PipelineGraphs2.aspx.vb" Inherits="PipelineGraphs2" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="PipelinePies.ascx" TagName="PipelinePies" TagPrefix="uc5" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>This Week's Pipeline Data</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <uc7:MyModal ID="myModalControl" runat="server" />
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg"/><br />
            
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
             <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <div style="float:left;width:25%">
                        <asp:DropDownList ID="cboGraphType" runat="server" Visible="true" AutoPostBack="false">
                            <asp:ListItem Text="By Anchor/Secondary" Value="Anchor" />
                            <asp:ListItem Text="By Hunted/Farmed" Value="Hunted" />
                            <asp:ListItem Text="By New/Extension" Value="New" />
                        </asp:DropDownList>
                        <asp:DropDownList ID="cboRevenueType" runat="server" Visible="true" AutoPostBack="false">
                            <asp:ListItem Text="Show Estimated Revenue" Value="Estimated" Selected="True" />
                            <asp:ListItem Text="Show Weighted Revenue" Value="Weighted" />
                        </asp:DropDownList>
                        <asp:Button runat="server" ID="btnRefresh" Text="Create Graphs" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>        
            <div style="clear:both">   
            <asp:UpdatePanel ID="pnlGraphUpdate" runat="server">
                <ContentTemplate>                   
                    <span style="float:left;width:40%"><uc5:PipelinePies runat="server" ID="grph1" Visible="false"/></span>
                    <span style="float:left;width:40%"><uc5:PipelinePies runat="server" ID="grph2" Visible="false"/></span>
                </ContentTemplate>
            </asp:UpdatePanel>                        
            </div>            
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Centric Consulting. All Rights Reserved. </p><br />
        </div>
 <asp:ObjectDataSource ID="ODS_PriorDate" runat="server" TypeName="cPipelineData" SelectMethod="GetDates" >
     <SelectParameters>
         <asp:Parameter DefaultValue="Before" Name="strPrior" Type="String" />
     </SelectParameters>
</asp:ObjectDataSource>    
<asp:ObjectDataSource ID="ODS_AfterDate" runat="server" TypeName="cPipelineData" SelectMethod="GetDates" >
     <SelectParameters>
         <asp:Parameter DefaultValue="After" Name="strPrior" Type="String" />
     </SelectParameters>
</asp:ObjectDataSource>    
    </form>
</body>
</html>
