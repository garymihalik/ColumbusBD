
Partial Class BusinessUnitAdmin
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
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdBusinessUnitItems.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub

    Protected Sub grdBusinessUnitItems_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdBusinessUnitItems.RowUpdating
        'test
        Dim BusinessUnit As String
        Dim ReportOrder As String
        Dim ID As String
        Dim AvgFTEComp As String
        Dim NonSalaryEmpFactor As String


        'This is the save after the edit
        Me.ODS_BusinessUnit.UpdateParameters.Clear()
        Dim selectedRow As GridViewRow = grdBusinessUnitItems.Rows(e.RowIndex)
        ID = CType(selectedRow.FindControl("lblID"), Label).Text
        BusinessUnit = CType(selectedRow.FindControl("txtBusinessUnit"), TextBox).Text
        ReportOrder = CType(selectedRow.FindControl("txtReportOrder"), TextBox).Text
        AvgFTEComp = CType(selectedRow.FindControl("txtAvgFTEComp"), TextBox).Text
        NonSalaryEmpFactor = CType(selectedRow.FindControl("txtNonSalaryEmpFactor"), TextBox).Text


        If String.IsNullOrEmpty(ID) Then
            'If ID is null, then let's add them as new!
            Me.ODS_BusinessUnit.InsertMethod = "InsertBUData"
            ODS_BusinessUnit.InsertParameters.Add("AvgFTEComp", TypeCode.Double, AvgFTEComp)
            ODS_BusinessUnit.InsertParameters.Add("NonSalaryEmpFactor", TypeCode.Double, AvgFTEComp)
            ODS_BusinessUnit.InsertParameters.Add("BusinessUnit", TypeCode.String, BusinessUnit)
            ODS_BusinessUnit.InsertParameters.Add("ReportOrder", TypeCode.Double, ReportOrder)
            ODS_BusinessUnit.InsertParameters.Add("NewItemID", TypeCode.Double, CStr(-1))
        Else
            Me.ODS_BusinessUnit.UpdateMethod = "UpdateBUData"
            ODS_BusinessUnit.UpdateParameters.Add("AvgFTEComp", TypeCode.Double, AvgFTEComp)
            ODS_BusinessUnit.UpdateParameters.Add("NonSalaryEmpFactor", TypeCode.Double, NonSalaryEmpFactor)
            ODS_BusinessUnit.UpdateParameters.Add("BusinessUnit", TypeCode.String, BusinessUnit)
            ODS_BusinessUnit.UpdateParameters.Add("ReportOrder", TypeCode.Double, ReportOrder)
            ODS_BusinessUnit.UpdateParameters.Add("ID", TypeCode.Double, ID)
            ODS_BusinessUnit.UpdateParameters.Add("ORIGINAL_ID", TypeCode.Double, ID)
        End If
    End Sub
    Protected Sub grdBusinessUnitItems_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdBusinessUnitItems.RowCommand
        'current e.CommandNames are insert, cancel, update, edit
        Dim AvgFTEComp As String
        Dim BusinessUnit As String
        Dim NonSalaryEmpFactor As String
        Dim ReportOrder As String

        If e.CommandName.ToUpper = "INSERT" Then
            Me.ODS_BusinessUnit.InsertParameters.Clear()
            AvgFTEComp = CType(sender.FooterRow.FindControl("txtAvgFTEComp"), TextBox).Text
            BusinessUnit = CType(sender.FooterRow.FindControl("txtBusinessUnit"), TextBox).Text
            NonSalaryEmpFactor = CType(sender.FooterRow.FindControl("txtNonSalaryEmpFactor"), TextBox).Text
            ReportOrder = CType(sender.FooterRow.FindControl("txtReportOrder"), TextBox).Text


            ODS_BusinessUnit.InsertMethod = "InsertBUData"
            ODS_BusinessUnit.InsertParameters.Add("AvgFTEComp", TypeCode.Double, AvgFTEComp)
            ODS_BusinessUnit.InsertParameters.Add("NonSalaryEmpFactor", TypeCode.Double, NonSalaryEmpFactor)
            ODS_BusinessUnit.InsertParameters.Add("BusinessUnit", TypeCode.String, BusinessUnit)
            ODS_BusinessUnit.InsertParameters.Add("ReportOrder", TypeCode.Double, ReportOrder)
            ODS_BusinessUnit.InsertParameters.Add("NewItemID", TypeCode.Double, CStr(-1))
            ODS_BusinessUnit.Insert()
        End If
    End Sub
    Protected Sub grdBusinessUnitItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdBusinessUnitItems.RowDataBound
        'Code for confirmation of deleting an item
        Dim lnk As ImageButton
        lnk = e.Row.FindControl("lnkDeleteBUData")
        If lnk IsNot Nothing Then
            'Console.WriteLine(lnk.CommandArgument)
            cCommon.CreateConfirmBox2(lnk, "Are you sure you want to delete this BusinessUnit?")
        End If

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

    Protected Sub ChangeNumberDisplayed(ByVal sender As Object, ByVal e As EventArgs)
        Dim dropDown As DropDownList = DirectCast(sender, DropDownList)
        Me.grdBusinessUnitItems.PageSize = Integer.Parse(dropDown.SelectedValue)
        grdBusinessUnitItems.DataBind()
    End Sub
    Protected Sub GoToPage(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtGoToPage As TextBox = DirectCast(sender, TextBox)

        Dim pageNumber As Integer
        If Integer.TryParse(txtGoToPage.Text.Trim(), pageNumber) AndAlso pageNumber > 0 AndAlso pageNumber <= Me.grdBusinessUnitItems.PageCount Then
            Me.grdBusinessUnitItems.PageIndex = pageNumber - 1
        Else
            Me.grdBusinessUnitItems.PageIndex = 0
        End If
    End Sub
End Class