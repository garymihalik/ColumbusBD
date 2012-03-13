Imports cCommon
Partial Class InactivateOpportunities
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
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdOpportunityUpdate.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Or sender.datakeys(0).value = -1 Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub

    Protected Sub grdOpportunityUpdate_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdOpportunityUpdate.RowUpdating
        Dim Inactive As String
        Dim selectedRow As GridViewRow = grdOpportunityUpdate.Rows(e.RowIndex)
        Me.ODS_Opportunity.UpdateParameters.Clear()
        Me.ODS_Opportunity.UpdateMethod = "MarkOpportunityActive"
        Inactive = CType(selectedRow.FindControl("ckInactive"), CheckBox).Checked
        ODS_Opportunity.UpdateParameters.Add("Inactive", TypeCode.Boolean, Inactive)

    End Sub
    Protected Sub ChangeNumberDisplayed(ByVal sender As Object, ByVal e As EventArgs)
        Dim dropDown As DropDownList = DirectCast(sender, DropDownList)
        Me.grdOpportunityUpdate.PageSize = Integer.Parse(dropDown.SelectedValue)
        grdOpportunityUpdate.DataBind()
    End Sub
    Protected Sub GoToPage(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtGoToPage As TextBox = DirectCast(sender, TextBox)

        Dim pageNumber As Integer
        If Integer.TryParse(txtGoToPage.Text.Trim(), pageNumber) AndAlso pageNumber > 0 AndAlso pageNumber <= Me.grdOpportunityUpdate.PageCount Then
            Me.grdOpportunityUpdate.PageIndex = pageNumber - 1
        Else
            Me.grdOpportunityUpdate.PageIndex = 0
        End If
    End Sub
    Protected Sub grdItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdOpportunityUpdate.RowDataBound
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
        'Code for pager
        If e.Row.RowType = DataControlRowType.Pager Then
            Dim lblTotalNumberOfPages As Label = DirectCast(e.Row.FindControl("lblTotalNumberOfPages"), Label)
            lblTotalNumberOfPages.Text = gridView.PageCount.ToString()

            Dim txtGoToPage As TextBox = DirectCast(e.Row.FindControl("txtGoToPage"), TextBox)
            txtGoToPage.Text = (gridView.PageIndex + 1).ToString()

            Dim ddlPageSize As DropDownList = DirectCast(e.Row.FindControl("ddlPageSize"), DropDownList)
            ddlPageSize.SelectedValue = gridView.PageSize.ToString()
        End If
    End Sub
End Class