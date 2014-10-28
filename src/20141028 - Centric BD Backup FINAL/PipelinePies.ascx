<%@ Control Language="VB" AutoEventWireup="false" CodeFile="PipelinePies.ascx.vb" Inherits="PipelinePies" %>
<asp:Panel runat="server" ID="pnlgridview" Width="90%" >

</asp:Panel>
<asp:chart id="Chart1" runat="server" ImageType="Png"  Visible="true" Width="412px" Height="296px">
    <Titles>
        <asp:Title Name="default" />
    </Titles>
    <Series>
        <asp:series Name="Series 1" ChartType="Pie" ></asp:series>
    </Series>
    <chartareas>
        <asp:chartarea Name="ChartArea1" >
        </asp:chartarea>
    </chartareas>
</asp:chart>

