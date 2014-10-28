Imports System.Data
Imports cCommonChart
Imports System.Drawing

Partial Class PipelineGraphs2
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
                cCommon.InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If

    End Sub

    Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnRefresh.Click
        grph1.Visible = True : grph2.Visible = True
        grph1.strItemChartType = Me.cboGraphType.SelectedValue
        grph1.strYType = Me.cboRevenueType.SelectedValue
        grph1.BusinessUnit = BU1.BU_ID
        grph1.CreateChart()

        grph2.strItemChartType = Me.cboGraphType.SelectedValue
        grph2.strYType = "Count"
        grph2.BusinessUnit = BU1.BU_ID
        grph2.CreateChart()
    End Sub
End Class