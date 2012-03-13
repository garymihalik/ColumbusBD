<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BUControl.ascx.vb" Inherits="BUControl" %>
<asp:UpdatePanel ID="pnlCBO" runat="server">
    <ContentTemplate>
        <asp:DropDownList ID="cboBU" runat="server" DataSourceID="ODS_BU" DataValueField="ID" DataTextField="BusinessUnit" AutoPostBack="true">
        </asp:DropDownList>
        <asp:Label runat="server" ID="lblLastSelectedBUID" Text="-2" visible="false"/>
        <asp:Label runat="server" ID="lblLastSelectedBUText" Text="Nothing" visible="false"/>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:ObjectDataSource ID="ODS_BU" runat="server" SelectMethod="GetBUList" TypeName="cAdmin">
</asp:ObjectDataSource>
