<%@ Page Language="VB" AutoEventWireup="true" CodeFile="ManageHolidays.aspx.vb" Inherits="ManageHolidays" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Assembly="eWorld.UI, Version=2.0.6.2393, Culture=neutral, PublicKeyToken=24d65337282035f2" Namespace="eWorld.UI" TagPrefix="ew" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Holidays</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"><Scripts><asp:ScriptReference Path="~/Scripts/Safari3AjaxHack.js" /></Scripts></asp:ScriptManager>
        <div id="header" >
            <span style="text-align:right;float:right"><asp:LoginName ID="LoginName1" runat="server" FormatString="Welcome, {0}" />&nbsp;<asp:LoginStatus ID="LoginStatus1" runat="server" LogoutPageUrl="login.aspx" LogoutAction="RedirectToLoginPage" /></span>
            <asp:Image ID="imgLogo" runat="server" skinid="mainlogo" ImageUrl="~/commonimages/logo1.jpg" /><br />
        </div>
        <div id="leftcol" >
            <uc3:Navigation ID="Navigation1" runat="server" Location="Default"/>
        </div>
        <div id="content" >
            <asp:UpdatePanel runat="server" ID="UpdatePanel1" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <asp:GridView ID="grdItems" runat="server" AutoGenerateColumns="False" 
                            DataSourceID="ODS_Item" ShowFooter="True" DataKeyNames="Holiday" 
                            Width="90%" EmptyDataText="No Entries Yet" Visible="true" OnDataBound="ShowFooterCode">
                        <Columns>
                            <asp:TemplateField Visible="true" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right" HeaderText="Date">
                                <EditItemTemplate>
                                    <asp:textbox ID="txtHoliday" runat="server" Text='<%# Bind("Holiday","{0:d}") %>' CssClass="DateTextBox"/>    
                                    <cc1:CalendarExtender runat="server" ID="cc1" TargetControlID="txtHoliday" />
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblHoliday" runat="server" Text='<%# Eval("Holiday","{0:d}") %>'  CssClass="DateTextBox"/>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:textbox ID="txtHoliday" runat="server" Text='<%# Bind("Holiday","{0:d}") %>' CssClass="DateTextBox"/>
                                    <cc1:CalendarExtender runat="server" ID="cc1" TargetControlID="txtHoliday" />
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField Visible="true" HeaderText="Holiday Description">
                                <EditItemTemplate>
                                    <asp:textbox ID="txtHolidayDescription" runat="server" Text='<%# Bind("HolidayDescription") %>' />    
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblHolidayDescription" runat="server" Text='<%# Eval("HolidayDescription") %>' />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:textbox ID="txtHolidayDescription" runat="server" Text='<%# Bind("HolidayDescription") %>' />
                                </FooterTemplate>
                            </asp:TemplateField>
                             <asp:CommandField ButtonType="Image" EditImageUrl="~/CommonImages/edit.gif" 
                                DeleteImageUrl="~/CommonImages/delete.gif" 
                                UpdateImageUrl="~/CommonImages/save.gif" 
                                CancelImageUrl="~/CommonImages/undo.gif" 
                                InsertImageUrl="~/CommonImages/save.gif" ShowDeleteButton="false" 
                                ShowEditButton="True" ShowCancelButton="true" 
                                NewImageUrl="~/CommonImages/Insert.GIF" ItemStyle-Width="5%" >
                            <ItemStyle Width="5%" />
                            </asp:CommandField>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:ImageButton ID="lnkDelete" runat="server" ImageUrl="~/CommonImages/delete.GIF" AlternateText="Delete Holiday Entry" CommandName="DELETE" CommandArgument='<%# Bind("Holiday") %>'/>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:ImageButton ID="lnkInsertUpdate" runat="server" ImageUrl="~/CommonImages/Insert.GIF" AlternateText="Insert New Update" CommandName="INSERT"/>
                                </FooterTemplate>
                            </asp:TemplateField>
                       </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    <asp:ObjectDataSource ID="ODS_Item" runat="server"  SelectMethod="GetAllHolidays" TypeName="cManageHolidays" DeleteMethod="DeleteSingleHoliday">
    </asp:ObjectDataSource>
    </form>
</body>
</html>
