Imports System.Data
Imports cCommonChart
Imports System.Drawing
Imports cAccountGroupForecast
Imports cCommon

Partial Class AccountGroupForecastLayout
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If User.Identity.IsAuthenticated Then
            If Not cCookie.bCookieExist("UserSettings") Then
                Response.Redirect("login.aspx")
            End If
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Me.Theme = cCommon.GetUserTheme
        If Not Page.IsPostBack Then
            If ConfigurationManager.AppSettings("TrackUserEveryPage").ToUpper = "ON" Then
                InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If
    End Sub

    Protected Sub btnUpdateChart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateChart.Click
        
        pnlgridview.Controls.Add(GetAccountGroupForecast(cboPrior.SelectedValue, cboAfter.SelectedValue, cboAccountGroup.SelectedValue))

    End Sub
    Protected Sub SetupInitialDate(ByVal sender As Object, ByVal e As System.EventArgs)
        cCommonDateCode.SetupInitialDate(sender, e)
    End Sub
End Class