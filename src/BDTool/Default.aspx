<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="_Default" %>
<%@ Register Src="Navigation.ascx" TagName="Navigation" TagPrefix="uc3" %>
<%@ Register Src="myModalLoading.ascx" TagName="MyModal" TagPrefix="uc7" %>
<%@ Register Src="BUControl.ascx" TagName="BU" TagPrefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN" "http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Landing Page</title>
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
            Welcome to Centric's Business Development Site
            <uc1:BU ID="BU1" runat="server" />
                <ul>
                    <li>10/28/2011</li>
                        <ul>
                        <li><a href="InactivateOpportunities.aspx">Inactivate Opportunities</a>:  Fixed bug that prevented a user from inactivating multiple opportunities at once.</li>
                        </ul>
                    <li>10/7/2011</li>
                        <ul>
                        <li><a href="InactivateOpportunities.aspx">Inactivate Opportunities</a>:  Fixed bug that prevented page from loading.</li>
                </ul>       
                <li>10/4/2011</li>
                    <ul>
                        <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Added ability to filter out extensions from the manage pipeline list.</li>
                        <li><a href="Pipeline2Excel.aspx">Export Pipeline to Excel</a>:  Added ability to filter out extensions from the manage pipeline list.</li>
                    </ul>
                <li>4/12/2011</li>
                    <ul>
                        <li><a href="Pipeline2Excel.aspx">Export Pipeline to Excel</a>:  Added a RFP Required, RFP Lead, and RFP Assessment Location fields on the export</li>
                        <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Added ability to add RFP Required, RFP Lead, and RFP Assessment Location to opportunities (free form text)</li>
                    </ul>
                <li>3/29/2011</li>
                    <ul>
                        <li><a href="Pipeline2Excel.aspx">Export Pipeline to Excel</a>:  Added a roles column on the export</li>
                        <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Added ability to add roles to opportunities (free form text)</li>
                    </ul>
                <li>8/15/2010</li>
                    <ul>
                        <li><a href="ForecastGraphs.aspx">BU Financial - Booked</a>:  Add business tax calculation</li>
                        <li><a href="BURevenueVarianceReport.aspx">BU Financial - Variance</a>:  Add business tax calculation</li>
                    </ul>
                <li>8/11/2010</li>
                    <ul>
                        <li><a href="ForecastGraphs.aspx">BU Financial - Booked</a>:  You can now choose to see only forecast or only booked information</li>
                        <li><a href="BURevenueVarianceReport.aspx">BU Financial - Variance</a>:  Remove all "Booked" information to show only variance between forecast and actuals</li>
                    </ul>
                <li>7/30/2010</li>
                <ul>
                    <li><a href="AnnualGlobalFactors.aspx">BU Annual Factors</a>:  A new page to store annual factors, such as average FTE salary for better forecasting</li>
                    <li><a href="MonthlyGlobalActuals.aspx">BU Monthly Overhead Forecast/Actuals</a>:  Added the ability to track actual profit and transfer notes</li>
                    <li><a href="ForecastGraphs.aspx">BU Financial - Booked</a>:  Added row to display actual profit, when entered</li>
                    <li><a href="BURevenueVarianceReport.aspx">BU Financial - Variance</a>:  Added row to display actual profit, when entered.  Also now calculates profit variance.</li>
                </ul>
                <li>3/8/2010</li>
                <ul>
                    <li><a href="benchreport1.aspx">Beta Bench Report</a>:  A new bench report that shows assignments people are booked and forecasted on as well  as the date</li>
                </ul>
                <li>1/2/2010</li>
                <ul>
                    <li><a href="BUFinancialVariance.aspx">BU Financial - Variance</a>:  Renamed "BU Financial - Revenue Variance" to "BU Financial - Variance" </li>
                    <li><a href="BUFinancialVariance.aspx">BU Financial - Variance</a>:  Added Revenue Variance, Cost Variance, and Profit Variance to the page</li>
                </ul>
                <li>12/31/2009</li>
                <ul>
                    <li><a href="ForecastGraphs.aspx">BU Financial - Booked</a>:  Renamed BU Financial - Booked to better match its functionality. Changed the line graph of revenue/profit to a bar graph.</li>
                    <li><a href="BUFinancialVariance.aspx">BU Financial - Revenue Variance</a>:  Created a new report to show forecasted vs. actual revenue for a selected time period.</li>
                </ul>
                
                <li>10/20/2009</li>
                <ul>
                    <li><a href="AccountGroupForecastLayout.aspx">Client Summary Forecast</a>:  This page now weights the revenue's and costs based on the opportunity win percentage.  The total's on this page will match the shadow forecast on the BU Summary page.</li>
                </ul>
                <li>9/28/2009</li>
                <ul>
                    <li><a href="ForecastGraphs.aspx">BU Summary Forecast</a>:  Inactive resources created an editing issue.  Resolved this by no longer allowing resource names to be edited/re-assigned.  In effect, the same functionality is had via deletion of the old record and insertion of a new record</li>
                </ul>
				<li>9/28/2009</li>
                <ul>
                    <li><a href="Pipeline2Excel.aspx">Export Pipeline Report</a>:  Made it easier to print by shadowing cells vice rows.  You're welcome Gwen.  New color scheme.  Green=Won.  Gray=Lost.  Red=Need New Update.  Yellow=Update made in past 7 days</li>
					<li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  New color scheme.  Green=Won.  Gray=Lost.  Red=Need New Update.  Yellow=Update made in past 7 days</li>
					<li><a href="NewBenchReport.aspx">Bench Report</a>:  Opportunity Name now links to a new page where you can modify that single assignment.  I recommend right clicking and opening in a new window if you have to make a lot of updates using the Bench Report as a reference.</li>
					<li><a href="ForecastGraphs.aspx">BU Summary Forecast</a>:  Added new filter criteria of "Active" which is any won opportunity with assignments that end in the future.</li>
					<li>Contact Tech Support:  Am now trapping site level errors and enabling the user to email the information for troubleshooting.  Added a couple of DB tables for this as well.</li>

                </ul>
				<li>9/25/2009</li>
                <ul>
                    <li><a href="ForecastGraphs.aspx">BU Summary Forecast</a>:  Now allows for showing booked (opportunities that are won) and shadow (opportunities between 70% and won) information.  FYI, the Client Summary will show 100% of revenue instead of adjusting it based on Shadow/Booked.  This will be recitified later</li>
                </ul>
				<li>9/9/2009</li>
                <ul>
                    <li><a href="ManageResources.aspx">Edit Resource Information</a>:  Fixed default sort to sort by last name, then first name.</li>
                    <li><a href="ForecastGraphs.aspx">BU Summary Forecast</a>:  You can now export the data to excel.</li>
                </ul>
				<li>9/8/2009</li>
                <ul>
                    <li><a href="Pipeline2Excel.aspx">Export Pipeline Report</a>:  Fixed Days Open Calculation.  Was previously calculating based on close date vs. today's date.  This is now fixed</li>
                    <li><a href="Pipeline2Excel.aspx">Export Pipeline Report</a>:  Expanded the functionality to include exporting other data, similar to the Manage Opportunities Page</li>
                    <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Added a tooltip for the win percentage criteria.  Just hover over the word "Win %"</li>
                </ul>
				<li>8/31/2009</li>
                <ul>
                    <li><b>Help Files</b>:  Added screencast help files to key pages</li>
                </ul>
                <li>8/25/2009</li>
                <ul>
                    <li><b>Multi-Business Unit Support</b>:  The site now supports multiple business units</li>
                    <li><a href="NewBenchReport.aspx">Bench Report</a>:  Added Now allows user to export the bench report to excel</li>
                </ul>    
                <li>8/18/2009</li>
                <ul>
                    <li><a href="Pipeline2Excel.aspx">Export Pipeline Report</a>:  Changed Report to remove alternating band highlighting</li>
                    <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Added filter criteria to include wins/losses in past 7 days.  Changed the listbox to add the gray visual cue for win/loss</li>
                </ul>    
                <li>8/17/2009</li>
                <ul>
                    <li><a href="NewBenchReport.aspx">Bench Report</a>:  Changed Report.  Now offers filtering by Employee/Contractor.  Also, expands to show assignments</li>
                    <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Changed highlighting to 5 days instead of 6.  Makes for better printing the day before the BD meeting.  Also changed the listbox to match the yellow/red visual cue for udpates/needing close dates</li>
                </ul>    
                <li>8/12/2009</li>
                <ul>
                    <li><a href="ManageHolidays.aspx">Manage Holidays</a>:  Added this new page to manage holidays</li>
                    <li>Fixed a forecasting issue.  Booked used to be defined as: If actuals>0 then use actuals in booked calculation otherwise use forecasted.  Changed it so that if actuals is null, use forecasted.  
                    If actuals are 0, then use 0.  This is due to the issue seen with MCCA.  I needed to zero out actuals and have it count as 0.</li>
                    <li><a href="ForecastingUpdates.aspx">Forecast Updates</a>:  Now allow user to exclude assignments from BU Revenue calculations</li>
                    <li><a href="ManageResources.aspx">Manage Resources</a>: Fixed an issue where user couldn't add a resource if a field was null</li>
                </ul>    
                <li>8/11/2009</li>
                <ul>
                    <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Moved opportunity update entry fields from the footer to the header to facilitate ease of entry</li>
                    <li><a href="ManageOpportunities.aspx">Manage Opportunities</a>:  Defaulted the new entries to the latest entry found for Win %, Close Date, and Estimated Revenue</li>
                    <li><a href="Pipeline2Excel">Export Pipeline to Excel</a>:  Added Gray highlighting, for the row, for opportunities marked won or lost in the 6 days.</li>
                    <li><a href="Pipeline2Excel">Export Pipeline to Excel</a>:  Added back any wins or losses in the past 6 days to the exported list.</li>
                    <li>Site:  Retooled Ajax to be compatible with Safari 3.x and Google Chrome 2.x.  The site now supports IE, FF, Chrome, Safari, and any v3.x webkit based browsers</li>
                </ul>
                <li>8/4/2009</li>
                <ul>
                    <li><a href="Pipeline2Excel">Export Pipeline to Excel</a>:  Added Red Cell highlighting for close days that occur in the next 7 days.</li>
                    <li><a href="Pipeline2Excel">Export Pipeline to Excel</a>:  Added Yellow highlighting, for the row, for changes in the past 6 days.</li>
                    <li>Added <a href="bugreports.aspx">bug reporting</a></li>
                </ul>
                <li>7/30/2009</li>
                <ul>
                    <li>The Site is Live at www.centricbd.com</li>
                </ul>
            </ul>
        </div>
        <div id="footer" >
            <p id="copyright">&copy; 2009 Joseph Ours. All Rights Reserved. </p><br />
        </div>
    </form>
</body>
</html>
