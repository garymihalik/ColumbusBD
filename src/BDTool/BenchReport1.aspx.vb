Imports cCommon
Imports System.Data
Partial Class BenchReport1
    Inherits System.Web.UI.Page
    'No changes from original at this point!!!!!!
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If User.Identity.IsAuthenticated Then
            If Not cCookie.bCookieExist("UserSettings") Then
                Response.Redirect("login.aspx")
            End If
        Else
            Response.Redirect("login.aspx")
        End If
        If Not Page.IsPostBack Then
            txtEndDate.Text = DateAdd(DateInterval.Month, 2, DateTime.Now).ToShortDateString
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

    Protected Sub grdItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdItems.RowDataBound
        'Code for sorting
        Dim gridView As GridView = DirectCast(sender, GridView)

        If gridView.SortExpression.Length > 0 Then
            Dim cellIndex As Integer = -1
            For Each field As DataControlField In gridView.Columns
                If field.SortExpression = gridView.SortExpression Then
                    cellIndex = gridView.Columns.IndexOf(field)
                    Exit For
                End If
            Next

            If cellIndex > -1 Then
                If e.Row.RowType = DataControlRowType.Header Then
                    '  this is a header row,
                    '  set the sort style
                    e.Row.Cells(cellIndex).ForeColor = Drawing.Color.Black
                    e.Row.Cells(cellIndex).CssClass = IIf(gridView.SortDirection = SortDirection.Ascending, "sortascheaderstyle", "sortdescheaderstyle")
                ElseIf e.Row.RowType = DataControlRowType.DataRow Then
                    '  this is an alternating row
                    e.Row.Cells(cellIndex).CssClass = IIf(e.Row.RowIndex Mod 2 = 0, "sortalternatingrowstyle", "sortrowstyle")
                End If
            End If
        End If

        'Additional code for nested grid is below
        Dim row As GridViewRow = e.Row
        Dim strSort As String = String.Empty

        ' Make sure we aren't in header/footer rows
        If row.DataItem Is Nothing Then
            Exit Sub
        End If
        'Find BookedRepeater control
        Dim rptrBooked As New Repeater
        rptrBooked = DirectCast(row.FindControl("rptrBookedDetails"), Repeater)

        rptrBooked.DataSource = cAssignments.GetPersonBenchDetailsByWinPercentage(DirectCast(e.Row.DataItem, DataRowView)("PK_PersonID").ToString(), strSort, txtEndDate.Text, True, 1)
        rptrBooked.DataBind()

        Dim rptrProposed As New Repeater
        rptrProposed = DirectCast(row.FindControl("rptrProposedDetails"), Repeater)
        Dim dblTemp As Double
        If IsNumeric(Me.txtWinPercentage.Text) Then
            dblTemp = Me.txtWinPercentage.Text
            If dblTemp <= 100 Then dblTemp /= 100
        Else
            dblTemp = 1

        End If
        rptrProposed.DataSource = cAssignments.GetPersonBenchDetailsByWinPercentage(DirectCast(e.Row.DataItem, DataRowView)("PK_PersonID").ToString(), strSort, txtEndDate.Text, True, dblTemp)
        rptrProposed.DataBind()
    End Sub

    Protected Sub btnUpdateChart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateReport.Click
        Me.grdItems.DataSourceID = "ODS_Resources"
        grdItems.DataBind()
        'Me.grdItemsForExport.DataSourceID = "ODS_Resources"
        'grdItemsForExport.DataBind()
    End Sub

    Protected Sub ODS_Resources_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles ODS_Resources.Selecting
        If Not IsDate(Me.txtEndDate.Text) Then
            Me.txtEndDate.Text = DateTime.Now.ToShortDateString
            e.InputParameters("dtEndDate") = DateTime.Now.ToShortDateString
        Else
            e.InputParameters("dtEndDate") = Me.txtEndDate.Text
        End If
        e.InputParameters("strIncluded") = Me.cboInclusion.SelectedValue
    End Sub
End Class