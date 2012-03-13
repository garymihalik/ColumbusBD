<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Navigation.ascx.vb" Inherits="Navigation" %>
<%@ Register Assembly="ExtExtenders" Namespace="ExtExtenders" TagPrefix="cc1" %>
    <asp:UpdatePanel ID="pnl1" runat="server" UpdateMode="Always" ChildrenAsTriggers="true" >
        <ContentTemplate>
                <div class="NavHeaderClass">
                    <asp:Label ID="Label2" runat="server" Text="Pipeline" skinid="NavHeader"></asp:Label>
                </div>
                <asp:HyperLink ID="lnkManageOpportunities" runat="server" NavigateUrl="~/ManageOpportunities.aspx" skinID="NavLink">Manage Opportunities </asp:HyperLink><br />
                <asp:HyperLink ID="lnkPipelineTrend" runat="server" NavigateUrl="~/PipelineGraphs.aspx" skinID="NavLink">PipeLine Trends </asp:HyperLink><br />
                <asp:HyperLink ID="lnkPipelinePie" runat="server" NavigateUrl="~/PipelineGraphs2.aspx" skinID="NavLink">This Week PipeLine Graph</asp:HyperLink><br />
                <asp:HyperLink ID="lnkExport2Excel" runat="server" NavigateUrl="~/Pipeline2Excel.aspx" skinID="NavLink">Excel Export Current Pipeline</asp:HyperLink><br />
                <asp:HyperLink ID="lnkBenchReport" runat="server" NavigateUrl="~/BenchReport.aspx" skinID="NavLink">Summary Bench Report</asp:HyperLink><br />
                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/BenchReport1.aspx" skinID="NavLink">Shadow Bench Report</asp:HyperLink><br />
                &nbsp;
                <div class="NavHeaderClass">
                    <asp:Label ID="lblFunctions" runat="server" Text="Forecasting" skinid="NavHeader"></asp:Label>
                </div>
                <asp:HyperLink ID="lnkForecastEntry" runat="server" NavigateUrl="~/ForecastingUpdates.aspx" skinID="NavLink">Enter Forecast Data</asp:HyperLink><br />
                <asp:HyperLink ID="lnkBUSummary" runat="server" NavigateUrl="~/ForecastGraphs.aspx" skinID="NavLink">BU Financial - Booked</asp:HyperLink><br />
                <asp:HyperLink ID="lnkBURevenueVarianceReport" runat="server" NavigateUrl="~/BUFinancialVariance.aspx" skinID="NavLink">BU Financial - Variance</asp:HyperLink><br />
                <asp:HyperLink ID="lnkClientSummary" runat="server" NavigateUrl="~/AccountGroupForecastLayout.aspx" skinID="NavLink">Client Summary Forecast</asp:HyperLink><br />
                <asp:HyperLink ID="lnkClientResourceRevenueReport" runat="server" NavigateUrl="~/ClientResourceRevenueReport.aspx" skinID="NavLink">Client Monthly Revenue Report</asp:HyperLink><br />
                <asp:HyperLink ID="lnkPvBReport" runat="server" NavigateUrl="~/PipelineVsBooked.aspx" skinID="NavLink">Pipeline Vs. Booked Report</asp:HyperLink><br />
                &nbsp;
                <div class="NavHeaderClass">
                    <asp:Label ID="Label3" runat="server" Text="Actuals" skinid="NavHeader"></asp:Label>
                </div>
                <asp:HyperLink ID="lnkActual" runat="server" NavigateUrl="~/ClientActuals.aspx" skinID="NavLink">Client Actuals</asp:HyperLink><br />
                <asp:HyperLink ID="lnkBUOverhead" runat="server" NavigateUrl="~/MonthlyGlobalActuals.aspx" skinID="NavLink">BU Monthly Overhead Data</asp:HyperLink><br />
                <asp:HyperLink ID="lnkAnnualGlobalFactors" runat="server" NavigateUrl="~/AnnualGlobalFactors.aspx" skinID="NavLink">BU Annual Factors</asp:HyperLink><br />
                &nbsp;
                <div class="NavHeaderClass">
                    <asp:Label ID="Label1" runat="server" Text="Admin" skinid="NavHeader"></asp:Label>
                </div>
                <asp:HyperLink ID="lnkAcctGroup" runat="server" NavigateUrl="~/ManageAccountGroup.aspx" skinID="NavLink">Manage Account Group List</asp:HyperLink>
                <asp:HyperLink ID="lnkIndustry" runat="server" NavigateUrl="~/ManageIndustry.aspx" skinID="NavLink">Manage Industry List</asp:HyperLink>
                <asp:HyperLink ID="lnkClient" runat="server" NavigateUrl="~/ManageClients.aspx" skinID="NavLink">Manage Client List</asp:HyperLink>
                <asp:HyperLink ID="lnkInactivateOpp" runat="server" NavigateUrl="~/InactivateOpportunities.aspx" skinID="NavLink">Inactivate Opportunities</asp:HyperLink><br />
                <asp:HyperLink ID="lnkResources" runat="server" NavigateUrl="~/ManageResources.aspx" skinID="NavLink">Edit Resource Info</asp:HyperLink><br />
                <asp:HyperLink ID="lnkHoliday" runat="server" NavigateUrl="~/ManageHolidays.aspx" skinID="NavLink">Manage Holiday List</asp:HyperLink><br />
                <br />
                <asp:HyperLink ID="HyperLink13" runat="server" NavigateUrl="~/Profile.aspx" skinID="NavLink">Profile</asp:HyperLink><br />
                <asp:HyperLink ID="lnkAddUsersToSite" runat="server" NavigateUrl="~/ManageWebSiteUsers.aspx" skinID="NavLink">Add Users to Website</asp:HyperLink><br />
        </ContentTemplate>
  </asp:UpdatePanel>    