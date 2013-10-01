<%@ Page Language="VB" AutoEventWireup="true" CodeFile="BusinessUnitAdmin.aspx.vb" Inherits="BusinessUnitAdmin" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="RealWorld.Grids" Namespace="RealWorld.Grids" TagPrefix="cc2" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Business Unit Admin</title>
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
        <cc2:BulkEditGridView ID="grdBusinessUnitItems" runat="server" AutoGenerateColumns="False" ShowFooter="true" Width="100%" Visible="True" 
        DataSourceID="ODS_BusinessUnit" DataKeyNames="ID" SaveButtonID="btnSaveBulkEdit" AllowPaging="true" AllowSorting="true">
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
                <asp:TemplateField HeaderText="ID" Visible="false">
                    <FooterTemplate>
                        <asp:Label ID="lblID" Text="" runat="server"></asp:Label>
                    </FooterTemplate>
                    <EditItemTemplate>
                        <asp:Label ID="lblID" Text='<%# Bind("ID") %>' runat="server"></asp:Label>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblID" Text='<%# Bind("ID") %>' runat="server"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="BusinessUnit Name" SortExpression="BusinessUnit">
                    <FooterTemplate>
                        <asp:TextBox ID="txtBusinessUnit" Text='<%# Bind("BusinessUnit") %>' runat="server" Width="95%"></asp:TextBox>
                    </FooterTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtBusinessUnit" Text='<%# Bind("BusinessUnit") %>' runat="server" Width="95%"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblBusinessUnit" Text='<%# Bind("BusinessUnit") %>' runat="server" Width="95%"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Avg Base FTE Salary" >
                    <FooterTemplate>
                        <asp:TextBox ID="txtAvgFTEComp" Text='<%# Bind("AvgFTEComp") %>' runat="server" Width="95%"></asp:TextBox>
                    </FooterTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtAvgFTEComp" Text='<%# Bind("AvgFTEComp") %>' runat="server" Width="95%"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="txtAvgFTEComp" Text='<%# Bind("AvgFTEComp") %>' runat="server" Width="95%"/>
                    </ItemTemplate>
                </asp:TemplateField>
		        <asp:TemplateField HeaderText="NonSalaryEmpFactor" >
                    <FooterTemplate>
                        <asp:TextBox ID="txtNonSalaryEmpFactor" Text='<%# Bind("NonSalaryEmpFactor") %>' runat="server" Width="95%"></asp:TextBox>
                    </FooterTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtNonSalaryEmpFactor" Text='<%# Bind("NonSalaryEmpFactor") %>' runat="server" Width="95%"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:label ID="txtNonSalaryEmpFactor" Text='<%# Bind("NonSalaryEmpFactor") %>' runat="server" Width="95%"/>
                    </ItemTemplate>
                </asp:TemplateField>
		        <asp:TemplateField HeaderText="ReportOrder?" SortExpression="ReportOrder">
                    <FooterTemplate>
                        <asp:textbox ID="txtReportOrder" Text='<%# Bind("ReportOrder") %>' runat="server" Width="95%" />
                    </FooterTemplate>
                    <EditItemTemplate>
                        <asp:textbox ID="txtReportOrder" Text='<%# Bind("ReportOrder") %>' runat="server" Width="95%" />
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:label ID="lblReportOrder" Text='<%# Bind("ReportOrder") %>' runat="server" Width="95%" />
                    </ItemTemplate>
                </asp:TemplateField>
		        <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton ID="lnkDeleteBUData" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Account Group" CommandName="DELETE" CommandArgument='<%# Bind("ID") %>'/>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:ImageButton ID="lnkAddBusinessUnit" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Account Group" CommandName="INSERT"/>
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
    <asp:ObjectDataSource ID="ODS_BusinessUnit" runat="server" SelectMethod="GetBUData" TypeName="cAdmin" InsertMethod="InsertBUData"  UpdateMethod="UpdateBUData" DeleteMethod="DeleteBUData">
        <DeleteParameters>
            <asp:Parameter DefaultValue="-1" Name="ID" Type="Int32" />
        </DeleteParameters>
    </asp:ObjectDataSource>
</form>
    
</body>
</html>
