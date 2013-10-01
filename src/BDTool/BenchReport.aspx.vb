Imports cCommon
Imports System.Data
Partial Class BenchReport
    Inherits System.Web.UI.Page
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

        'Find Child GridView control
        Dim gv As New GridView()
        gv = DirectCast(row.FindControl("GridView2"), GridView)

        'Check if any additional conditions (Paging, Sorting, Editing, etc) to be applied on child GridView
        If gv.UniqueID = gvUniqueID Then
            gv.PageIndex = gvNewPageIndex
            gv.EditIndex = gvEditIndex
            'Check if Sorting used
            If gvSortExpr <> String.Empty Then
                GetSortDirection()
                strSort = " ORDER BY " & String.Format("{0} {1}", gvSortExpr, gvSortDir)
            End If
            'Expand the Child grid
            ClientScript.RegisterStartupScript([GetType](), "Expand", "<SCRIPT LANGUAGE='javascript'>expandcollapse('div" & DirectCast(e.Row.DataItem, DataRowView)("PK_PersonID").ToString() & "','one');</script>")
        End If

        'Prepare the query for Child GridView by passing the Customer ID of the parent row

        gv.DataSource = cAssignments.GetPersonBenchDetails(DirectCast(e.Row.DataItem, DataRowView)("PK_PersonID").ToString(), strSort, txtEndDate.Text, True)
        gv.DataBind()
    End Sub

#Region "Variables"
    Private gvUniqueID As String = [String].Empty
    Private gvNewPageIndex As Integer = 0
    Private gvEditIndex As Integer = -1
    Private gvSortExpr As String = [String].Empty
    Private Property gvSortDir() As String

        Get
            Return If(TryCast(ViewState("SortDirection"), String), "ASC")
        End Get

        Set(ByVal value As String)
            ViewState("SortDirection") = value
        End Set
    End Property

#End Region

    'This procedure returns the Sort Direction
    Private Function GetSortDirection() As String
        Select Case gvSortDir
            Case "ASC"
                gvSortDir = "DESC"
                Exit Select

            Case "DESC"
                gvSortDir = "ASC"
                Exit Select
        End Select
        Return gvSortDir
    End Function
#Region "GridView2 Event Handlers"
    Protected Sub GridView2_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID
        gvNewPageIndex = e.NewPageIndex
        'grdItems.DataBind()
    End Sub

    Protected Sub GridView2_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        'Check if this is our Blank Row being databound, if so make the row invisible
        If e.Row.RowType = DataControlRowType.DataRow Then
            If DirectCast(e.Row.DataItem, DataRowView)("PK_PersonID").ToString() = [String].Empty Then
                e.Row.Visible = False
            End If
            'Now create link to page for adjusting
            Dim lnk As HyperLink
            lnk = e.Row.FindControl("lnkID")
            If lnk IsNot Nothing Then
                Dim strString As String
                Dim intItemID As Integer = e.Row.DataItem.Item("AssignmentID")
                strString = cTamperProof.CreateTamperProofURL("", "", "id=" & intItemID)
                lnk.Text = e.Row.DataItem.Item("OpportunityName")
                lnk.NavigateUrl = "~/IndividualAssignmentUpdate.aspx" & strString
            End If
        End If
    End Sub

    Protected Sub GridView2_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        Dim gvTemp As GridView = DirectCast(sender, GridView)
        gvUniqueID = gvTemp.UniqueID
        gvSortExpr = e.SortExpression
        grdItems.DataBind()
    End Sub
#End Region

    Protected Sub btnUpdateChart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateReport.Click
        Me.grdItems.DataSourceID = "ODS_Resources"
        grdItems.DataBind()
        Me.grdItemsForExport.DataSourceID = "ODS_Resources"
        grdItemsForExport.DataBind()
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