Imports cCommon
Partial Class ClientResourceRevenueReport
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

    Protected Sub BU1_BUSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BU1.BUSelectionChanged
        cboAccounts.DataBind()
        Me.grdAssignments.Visible = False
    End Sub

    Protected Sub SetupInitialDate(ByVal sender As Object, ByVal e As System.EventArgs)
        cCommonDateCode.SetupInitialDate(sender, e)
    End Sub

    Protected Sub btnGetReport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnGetReport.Click
        Me.grdAssignments.Visible = True
    End Sub
    Public dblTotalRevenue As Double = 0
    Public dblTotalCost As Double = 0

    Protected Sub TotalRows(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdAssignments.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            dblTotalRevenue += Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "Revenue"))
            dblTotalCost += Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "Costs"))
        ElseIf e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(0).Text = "Totals "
            e.Row.Cells(6).Text = String.Format("{0:c2}", dblTotalRevenue)
            e.Row.Cells(7).Text = String.Format("{0:c2}", dblTotalCost)
            e.Row.Cells(6).HorizontalAlign = HorizontalAlign.Right
            e.Row.Cells(7).HorizontalAlign = HorizontalAlign.Right
        End If
    End Sub
End Class