Imports ExtExtenders
Imports Samples

Partial Class Navigation
    Inherits System.Web.UI.UserControl
#Region "Class Variables"
    Private strLocation As String = ""
    Private intCurrentLevelID As Integer = -1
#End Region
#Region "Properties"
    ''' <summary>
    ''' This provides consistent navigation across the application
    ''' </summary>
    ''' <value>Receives the user location and stores it in local variable</value>
    ''' <returns>Nothing</returns>
    ''' <remarks>Provides control to limit what menu items are available.</remarks>
    Public Property Location() As String
        Get
            Dim _out As String
            If String.IsNullOrEmpty(strLocation) Then
                _out = "Empty"
            Else
                _out = strLocation
            End If
            Return _out
        End Get
        Set(ByVal value As String)
            strLocation = value
        End Set
    End Property
    Public Property CurrentLevelID() As String
        Get
            Dim _out As Integer
            If String.IsNullOrEmpty(intCurrentLevelID) Then
                _out = cCookie.ReadSingleCookieValue("UserSettings", "CurrentLevelID")
                intCurrentLevelID = _out
            Else
                _out = intCurrentLevelID
            End If
            Return _out
        End Get
        Set(ByVal value As String)
            intCurrentLevelID = value
        End Set
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'This is Admin's Section
        If Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Administrator") Then
            'Mark links visible
            lnkAddUsersToSite.Visible = "true"
        Else
            'Mark links not visible as needed
            lnkAddUsersToSite.Visible = "false"
        End If
        If Not (Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Forecast Owner") Or Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "Account Owner") Or Roles.IsUserInRole(HttpContext.Current.User.Identity.Name, "National")) Then
            lnkBenchReport.Visible = False
            lnkForecastEntry.Visible = False
            lnkBUSummary.Visible = False
            lnkClientSummary.Visible = False
            lnkPvBReport.Visible = False
            lnkActual.Visible = False
            lnkBUOverhead.Visible = False
            lnkAcctGroup.Visible = False
            lnkIndustry.Visible = False
            lnkClient.Visible = False
            lnkInactivateOpp.Visible = False
            lnkResources.Visible = False
            lnkHoliday.Visible = False
            lnkBURevenueVarianceReport.Visible = False
            lnkClientResourceRevenueReport.Visible = False
            lnkAnnualGlobalFactors.Visible = False
        End If


        If Not Page.IsPostBack Then

        End If
    End Sub
End Class
