
Partial Class ManageClients
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
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdClientItems.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Or sender.datakeys(0).value = -1 Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub

    Protected Sub grdClientItems_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdClientItems.RowUpdating
        'test
        Dim Client As String
        Dim Anchor As String
        Dim ClientID As String
        Dim AccountGroup As String
        Dim Industry As String


        'This is the save after the edit
        Me.ODS_Client.UpdateParameters.Clear()
        Dim selectedRow As GridViewRow = grdClientItems.Rows(e.RowIndex)
        ClientID = CType(selectedRow.FindControl("lblClientID"), Label).Text
        Client = CType(selectedRow.FindControl("txtClient"), TextBox).Text
        Anchor = CType(selectedRow.FindControl("ckAnchor"), CheckBox).Checked
        AccountGroup = CType(selectedRow.FindControl("cboAccountGroup"), eWorld.UI.MultiTextDropDownList).SelectedValue
        Industry = CType(selectedRow.FindControl("cboIndustry"), eWorld.UI.MultiTextDropDownList).SelectedValue


        If String.IsNullOrEmpty(ClientID) Then
            'If ClientID is null, then let's add them as new!
            Me.ODS_Client.InsertMethod = "InsertSingleClient"
            ODS_Client.InsertParameters.Add("AccountGroup", TypeCode.String, AccountGroup)
            ODS_Client.InsertParameters.Add("Client", TypeCode.String, Client)
            ODS_Client.InsertParameters.Add("Industry", TypeCode.String, AccountGroup)
            ODS_Client.InsertParameters.Add("Anchor", TypeCode.Boolean, Anchor)
            ODS_Client.InsertParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
            ODS_Client.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
        Else
            Me.ODS_Client.UpdateMethod = "UpdateSingleClient"
            ODS_Client.UpdateParameters.Add("AccountGroupID", TypeCode.String, AccountGroup)
            ODS_Client.UpdateParameters.Add("IndustryID", TypeCode.String, Industry)
            ODS_Client.UpdateParameters.Add("Client", TypeCode.String, Client)
            ODS_Client.UpdateParameters.Add("Anchor", TypeCode.Boolean, Anchor)
            ODS_Client.UpdateParameters.Add("ClientID", TypeCode.Int32, ClientID)
            ODS_Client.UpdateParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
            ODS_Client.UpdateParameters.Add("ORIGINAL_ClientID", TypeCode.Int32, ClientID)
        End If
    End Sub
    Protected Sub grdClientItems_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdClientItems.RowCommand
        'current e.CommandNames are insert, cancel, update, edit
        Dim AccountGroup As String
        Dim Client As String
        Dim Industry As String
        Dim Anchor As String

        If e.CommandName.ToUpper = "INSERT" Then
            Me.ODS_Client.InsertParameters.Clear()
            AccountGroup = CType(sender.FooterRow.FindControl("cboAccountGroup"), eWorld.UI.MultiTextDropDownList).SelectedValue
            Client = CType(sender.FooterRow.FindControl("txtClient"), TextBox).Text
            Industry = CType(sender.FooterRow.FindControl("cboIndustry"), eWorld.UI.MultiTextDropDownList).SelectedValue
            Anchor = CType(sender.FooterRow.FindControl("ckAnchor"), CheckBox).Checked


            ODS_Client.InsertMethod = "InsertSingleClient"
            ODS_Client.InsertParameters.Add("AccountGroupID", TypeCode.String, AccountGroup)
            ODS_Client.InsertParameters.Add("Client", TypeCode.String, Client)
            ODS_Client.InsertParameters.Add("IndustryID", TypeCode.String, Industry)
            ODS_Client.InsertParameters.Add("Anchor", TypeCode.Boolean, Anchor)
            ODS_Client.InsertParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
            ODS_Client.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
            ODS_Client.Insert()
        End If
    End Sub
    Protected Sub grdClientItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdClientItems.RowDataBound
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
    Protected Sub SetUpInitialEditValue(ByVal sender As Object, ByVal e As System.EventArgs)
        If sender.id = "cboAccountGroup" Then
            sender.Items.Insert(0, New ListItem("-Select-", -1))
            Dim gvRow As GridViewRow = DirectCast(sender.namingcontainer, GridViewRow)
            If Not gvRow.DataItem Is Nothing Then
                sender.SelectedValue = DirectCast(gvRow.DataItem, System.Data.DataRowView)("AccountGroupID").ToString
            End If
        ElseIf sender.id = "cboIndustry" Then
            sender.Items.Insert(0, New ListItem("-Select-", -1))
            Dim gvRow As GridViewRow = DirectCast(sender.namingcontainer, GridViewRow)
            If Not gvRow.DataItem Is Nothing Then
                sender.SelectedValue = DirectCast(gvRow.DataItem, System.Data.DataRowView)("IndustryID").ToString
            End If
        End If
    End Sub
    Protected Sub ChangeNumberDisplayed(ByVal sender As Object, ByVal e As EventArgs)
        Dim dropDown As DropDownList = DirectCast(sender, DropDownList)
        Me.grdClientItems.PageSize = Integer.Parse(dropDown.SelectedValue)
        grdClientItems.DataBind()
    End Sub
    Protected Sub GoToPage(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtGoToPage As TextBox = DirectCast(sender, TextBox)

        Dim pageNumber As Integer
        If Integer.TryParse(txtGoToPage.Text.Trim(), pageNumber) AndAlso pageNumber > 0 AndAlso pageNumber <= Me.grdClientItems.PageCount Then
            Me.grdClientItems.PageIndex = pageNumber - 1
        Else
            Me.grdClientItems.PageIndex = 0
        End If
    End Sub
End Class