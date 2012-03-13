<%@ Page Language="VB" AutoEventWireup="true" CodeFile="InactivateOpportunities.aspx.vb" Inherits="InactivateOpportunities" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc2" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Inactivate Opportunities</title>
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
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <uc1:BU ID="BU1" runat="server"/>
                    <cc2:BulkEditGridView  ID="grdOpportunityUpdate" runat="server" AutoGenerateColumns="False" SaveButtonID="btnSaveBulkEdit"
                    DataSourceID="ODS_Opportunity" ShowFooter="False" DataKeyNames="PK_OpportunityID" 
                    Width="95%" Visible="True" EmptyDataText="No Updates Yet" Caption="Opportunities" AllowPaging="true">
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
                            <asp:BoundField DataField="PK_OpportunityID" HeaderText="PK_OpportunityID ID" Visible="False"/>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Opportunity Name" SortExpression="OpportunityName">
                                <EditItemTemplate>
                                    <asp:Label ID="lblClient" runat="server" Text='<%# Eval("Client") %>' />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Opportunity Name" SortExpression="OpportunityName">
                                <EditItemTemplate>
                                    <asp:Label ID="lblOpportunityName" runat="server" Text='<%# Eval("OpportunityName") %>' />
                                </EditItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderText="Inactive?">
                                <EditItemTemplate>
                                    <asp:Checkbox ID="ckInactive" runat="server" Checked='<%# IIf(Eval("Inactive") Is DBNull.Value, "False", Eval("InActive")) %>'/>
                                </EditItemTemplate>
                            </asp:TemplateField>
                       </Columns>
                    </cc2:BulkEditGridView >
                    <asp:Button ID="btnSaveBulkEdit" runat="server" Text="Save Changes" />
                
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Opportunity" runat="server" SelectMethod="GetOpportunitiesByBU" TypeName="cOpportunity" UpdateMethod="MarkOpportunityActive" >
        <SelectParameters>
             <asp:Parameter DefaultValue="" Name="strFilter" Type="String" />
             <asp:Parameter DefaultValue="False" Name="bExcludeExclusions" Type="Boolean" />
             <asp:ControlParameter ControlID="BU1" PropertyName="BU_ID" Name="dblBU_ID" DefaultValue="-1" />
        </SelectParameters>
    </asp:ObjectDataSource>
    </form>
</body>
</html>
