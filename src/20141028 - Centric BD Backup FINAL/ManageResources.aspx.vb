Imports cCommon
Partial Class ManageResources
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
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdItems.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub

    Protected Sub grdItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdItems.RowDataBound
        'Code for confirmation of deleting an item
        Dim lnk As ImageButton
        lnk = e.Row.FindControl("lnkDeleteClient")
        If lnk IsNot Nothing Then
            'Console.WriteLine(lnk.CommandArgument)
            cCommon.CreateConfirmBox2(lnk, "Are you sure you want to delete this client?")
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
        Me.grdItems.PageSize = Integer.Parse(dropDown.SelectedValue)
        grdItems.DataBind()
    End Sub
    Protected Sub GoToPage(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtGoToPage As TextBox = DirectCast(sender, TextBox)

        Dim pageNumber As Integer
        If Integer.TryParse(txtGoToPage.Text.Trim(), pageNumber) AndAlso pageNumber > 0 AndAlso pageNumber <= Me.grdItems.PageCount Then
            Me.grdItems.PageIndex = pageNumber - 1
        Else
            Me.grdItems.PageIndex = 0
        End If
    End Sub

    Protected Sub grdItems_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdItems.RowCommand
        'current e.CommandNames are insert, cancel, update, edit
        If e.CommandName.ToUpper = "INSERT" Then
            Me.ODS_Item.InsertParameters.Clear()
            Dim LastNameValue As String = (CType(sender.FooterRow.FindControl("txtLastName"), TextBox).Text)
            Dim FirstNameValue As String = (CType(sender.FooterRow.FindControl("txtFirstName"), TextBox).Text)
            Dim OfficeValue As String = (CType(sender.FooterRow.FindControl("txtOffice"), TextBox).Text)
            Dim CellValue As String = (CType(sender.FooterRow.FindControl("txtCell"), TextBox).Text)
            Dim HomeValue As String = (CType(sender.FooterRow.FindControl("txtHome"), TextBox).Text)
            Dim EmailValue As String = (CType(sender.FooterRow.FindControl("txtEmail"), TextBox).Text)
            Dim EmployeeValue As String = (CType(sender.FooterRow.FindControl("ckEmployee"), CheckBox).Checked)
            Dim InactiveValue As String = (CType(sender.FooterRow.FindControl("ckInactive"), CheckBox).Checked)

            If String.IsNullOrEmpty(LastNameValue) Then LastNameValue = ""
            If String.IsNullOrEmpty(FirstNameValue) Then FirstNameValue = ""
            If String.IsNullOrEmpty(OfficeValue) Then OfficeValue = ""
            If String.IsNullOrEmpty(CellValue) Then CellValue = ""
            If String.IsNullOrEmpty(HomeValue) Then HomeValue = ""
            If String.IsNullOrEmpty(EmailValue) Then EmailValue = ""
            If String.IsNullOrEmpty(EmployeeValue) Then EmployeeValue = False
            If String.IsNullOrEmpty(InactiveValue) Then InactiveValue = False


            ODS_Item.InsertMethod = "InsertResource"
            ODS_Item.InsertParameters.Add("LastName", TypeCode.String, LastNameValue)
            ODS_Item.InsertParameters.Add("FirstName", TypeCode.String, FirstNameValue)
            ODS_Item.InsertParameters.Add("Office", TypeCode.String, OfficeValue)
            ODS_Item.InsertParameters.Add("Cell", TypeCode.String, CellValue)
            ODS_Item.InsertParameters.Add("Home", TypeCode.String, HomeValue)
            ODS_Item.InsertParameters.Add("Email", TypeCode.String, EmailValue)
            ODS_Item.InsertParameters.Add("Employee", TypeCode.Boolean, EmployeeValue)
            ODS_Item.InsertParameters.Add("Inactive", TypeCode.Boolean, InactiveValue)
            ODS_Item.InsertParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
            ODS_Item.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
            ODS_Item.Insert()
        End If
    End Sub
    Protected Sub grdItems_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdItems.RowUpdating
        'test


        'This is the save after the edit
        Me.ODS_Item.UpdateParameters.Clear()
        Dim selectedRow As GridViewRow = grdItems.Rows(e.RowIndex)
        Dim PK_PersonIDValue As String = (CType(selectedRow.FindControl("lblPK_PersonID"), Label).Text)
        Dim LastNameValue As String = (CType(selectedRow.FindControl("txtLastName"), TextBox).Text)
        Dim FirstNameValue As String = (CType(selectedRow.FindControl("txtFirstName"), TextBox).Text)
        Dim OfficeValue As String = (CType(selectedRow.FindControl("txtOffice"), TextBox).Text)
        Dim CellValue As String = (CType(selectedRow.FindControl("txtCell"), TextBox).Text)
        Dim HomeValue As String = (CType(selectedRow.FindControl("txtHome"), TextBox).Text)
        Dim EmailValue As String = (CType(selectedRow.FindControl("txtEmail"), TextBox).Text)
        Dim EmployeeValue As String = (CType(selectedRow.FindControl("ckEmployee"), CheckBox).Checked)
        Dim InactiveValue As String = (CType(selectedRow.FindControl("ckInactive"), CheckBox).Checked)

        If String.IsNullOrEmpty(LastNameValue) Then LastNameValue = ""
        If String.IsNullOrEmpty(FirstNameValue) Then FirstNameValue = ""
        If String.IsNullOrEmpty(OfficeValue) Then OfficeValue = ""
        If String.IsNullOrEmpty(CellValue) Then CellValue = ""
        If String.IsNullOrEmpty(HomeValue) Then HomeValue = ""
        If String.IsNullOrEmpty(EmailValue) Then EmailValue = ""
        If String.IsNullOrEmpty(EmployeeValue) Then EmployeeValue = False
        If String.IsNullOrEmpty(InactiveValue) Then InactiveValue = False


        If String.IsNullOrEmpty(PK_PersonIDValue) Then
            'If ClientID is null, then let's add them as new!
            Me.ODS_Item.InsertMethod = "InsertResource"
            ODS_Item.InsertParameters.Add("LastName", TypeCode.String, LastNameValue)
            ODS_Item.InsertParameters.Add("FirstName", TypeCode.String, FirstNameValue)
            ODS_Item.InsertParameters.Add("Office", TypeCode.String, OfficeValue)
            ODS_Item.InsertParameters.Add("Cell", TypeCode.String, CellValue)
            ODS_Item.InsertParameters.Add("Home", TypeCode.String, HomeValue)
            ODS_Item.InsertParameters.Add("Email", TypeCode.String, EmailValue)
            ODS_Item.InsertParameters.Add("Employee", TypeCode.Boolean, EmployeeValue)
            ODS_Item.InsertParameters.Add("Inactive", TypeCode.Boolean, InactiveValue)
            ODS_Item.InsertParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
        Else
            Me.ODS_Item.UpdateMethod = "UpdateResource"
            ODS_Item.UpdateParameters.Add("PK_PersonID", TypeCode.Double, PK_PersonIDValue)
            ODS_Item.UpdateParameters.Add("LastName", TypeCode.String, LastNameValue)
            ODS_Item.UpdateParameters.Add("FirstName", TypeCode.String, FirstNameValue)
            ODS_Item.UpdateParameters.Add("Office", TypeCode.String, OfficeValue)
            ODS_Item.UpdateParameters.Add("Cell", TypeCode.String, CellValue)
            ODS_Item.UpdateParameters.Add("Home", TypeCode.String, HomeValue)
            ODS_Item.UpdateParameters.Add("Email", TypeCode.String, EmailValue)
            ODS_Item.UpdateParameters.Add("Employee", TypeCode.Boolean, EmployeeValue)
            ODS_Item.UpdateParameters.Add("Inactive", TypeCode.Boolean, InactiveValue)
            ODS_Item.UpdateParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
        End If
    End Sub
End Class