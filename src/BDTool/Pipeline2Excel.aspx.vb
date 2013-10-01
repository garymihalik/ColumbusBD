Imports cCommon
Partial Class DatagridDefault
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
    Protected Sub ShowFooterCode(ByVal sender As Object, ByVal e As System.EventArgs)
        'Hide grid if there isn't an parent control value selected
        'If String.IsNullOrEmpty(CStr(lstOpportunities.SelectedValue)) Then
        ' sender.visible = "false"
        ' Else
        ' sender.visible = "true"
        ' End If
        'Hide row if there is the fake row of -1
        'If sender.datakeys(0).value = "-1" Then
        ' sender.Rows(0).Visible = "false"
        ' End If

    End Sub
    Protected Sub grdItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdItems.RowDataBound


        'Code for Highlighting Column Header adn providing arrows for sorting
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
                    'See HighlightDataRow instead
                    '  this is an alternating row
                    'e.Row.Cells(cellIndex).CssClass = IIf(e.Row.RowIndex Mod 2 = 0, "sortalternatingrowstyle", "sortrowstyle")
                End If
            End If
        End If

        'Highlight cells
        'First Close Date should be RED if less than today+7days
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim tempValue As String = CType(e.Row.FindControl("lblOpportunityCloseDate"), Label).Text
            If IsDate(tempValue) Then
                If DateDiff(DateInterval.Day, DateTime.Now, CDate(tempValue)) <= 5 Then
                    CType(e.Row.FindControl("lblOpportunityCloseDate"), Label).ForeColor = Drawing.Color.Red
                End If
            End If
        End If
        'Highlight row if update date is within past 7 days
        HighlightDataRow(e.Row)

    End Sub
    Private Sub HighlightDataRow(ByRef myDR As GridViewRow)
        If myDR.RowType = DataControlRowType.DataRow Then
            Dim tempUpdateDate As String = CType(myDR.FindControl("lblUpdateDate"), Label).Text
            Dim tempWinPercentage As String = CType(myDR.FindControl("lblWinPercentage"), Label).Text
            If IsDate(tempUpdateDate) Then
                If DateDiff(DateInterval.Day, CDate(tempUpdateDate), DateTime.Now) <= 5 Then
                    'Have to do this in this manner to get it to show in the export.
                    myDR.Cells(2).Attributes("style") = "background-color:yellow"
                    'CType(myDR.FindControl("lblOpportunityName"), Label).BackColor = Drawing.Color.Yellow
                Else
                    'myDR.CssClass = IIf(myDR.RowIndex Mod 2 = 0, "sortalternatingrowstyle", "sortrowstyle")
                End If
            End If
            If Not String.IsNullOrEmpty(tempWinPercentage) Then
                tempWinPercentage = Replace(tempWinPercentage, " %", "")
                If tempWinPercentage = 0 Then
                    myDR.Attributes("style") = "background:#dddddd"
                ElseIf tempWinPercentage = 100 Then
                    myDR.Attributes("style") = "background-color:lightgreen"
                Else
                    'myDR.CssClass = IIf(myDR.RowIndex Mod 2 = 0, "sortalternatingrowstyle", "sortrowstyle")
                End If
            End If
        End If
    End Sub

    Protected Sub btnUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        grdItems.Visible = True
        grdItems.DataBind()
    End Sub

    Protected Sub BU1_BUSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BU1.BUSelectionChanged
        btnUpdate.Visible = False
    End Sub
End Class