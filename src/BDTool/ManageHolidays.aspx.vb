Imports cCommon
Partial Class ManageHolidays
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
        If sender.datakeys(0).value = CDate(#1/1/1900#) Then
            sender.Rows(0).Visible = "false"
        End If

    End Sub

    Protected Sub ODS_Assignments_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles ODS_Item.Inserting
        'Depending on the need, can do insert via ODS or grdRowCommand
        'Dim selectedRow As GridViewRow = sender.FooterRow
        'Dim FieldName1Value As String = (CType(selectedRow.FindControl("ControlName"), TextBox).Text)
        'If String.IsNullOrEmpty(FieldName1Value) Then FieldName1Value = 0
        'e.InputParameters("FieldName1") = FieldName1Value

    End Sub

    Protected Sub grdItems_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdItems.RowCommand
        If e.CommandName = "INSERT" Then
            Me.ODS_Item.InsertParameters.Clear()
            Me.ODS_Item.InsertMethod = "InsertSingleHoliday"
            Dim selectedRow As GridViewRow = sender.FooterRow

            Dim Holiday As String = (CType(selectedRow.FindControl("txtHoliday"), TextBox).Text)
            If String.IsNullOrEmpty(Holiday) Then Holiday = #1/1/1900#

            Dim HolidayDescription As String = (CType(selectedRow.FindControl("txtHolidayDescription"), TextBox).Text)
            If String.IsNullOrEmpty(HolidayDescription) Then HolidayDescription = ""

            ODS_Item.InsertParameters.Add("Holiday", TypeCode.DateTime, Holiday)
            ODS_Item.InsertParameters.Add("HolidayDescription", TypeCode.String, HolidayDescription)

            ODS_Item.Insert()
        End If
    End Sub
    Protected Sub grdItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdItems.RowDataBound
        'If you have a delete button, this code adds confirmation. Ensure delete name is lnkDelete
        Dim lnk As ImageButton
        lnk = e.Row.FindControl("lnkDelete")
        If lnk IsNot Nothing Then
            cCommon.CreateConfirmBox2(lnk, "Are you sure you want to delete this? Existing related data will also be deleted!")
        End If

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
                    '  this is an alternating row
                    e.Row.Cells(cellIndex).CssClass = IIf(e.Row.RowIndex Mod 2 = 0, "sortalternatingrowstyle", "sortrowstyle")
                End If
            End If
        End If

        'This is code for using the pager.  This code assumes you've added the commented asp code to the aspx page
        'Code for pager
        '<PagerStyle HorizontalAlign="Right" />
        '    <PagerTemplate>
        '        <asp:Label ID="Label1" runat="server" Text="Show rows:" />
        '        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ChangeNumberDisplayed">
        '            <asp:ListItem Value="5" />
        '            <asp:ListItem Value="10" Selected="True"/>
        '            <asp:ListItem Value="15" />
        '            <asp:ListItem Value="20" />
        '            <asp:ListItem Value="50" />
        '            <asp:ListItem Value="100" />
        '            <asp:ListItem Value="250" /><asp:ListItem Value="500" /><asp:ListItem Value="1000" />
        '        </asp:DropDownList>
        '        &nbsp;
        'Page
        '        <asp:TextBox ID="txtGoToPage" runat="server" AutoPostBack="true" OnTextChanged="GoToPage" Width="20px" Font-Size="X-Small" />
        '        of
        '        <asp:Label ID="lblTotalNumberOfPages" runat="server" Font-Size="X-Small" />
        '        &nbsp;
        '        <asp:ImageButton ID="Button1" runat="server" CommandName="Page" ToolTip="Previous Page" CommandArgument="Prev" BackColor="Transparent" ImageUrl="~/commonimages/leftarrow.gif"/>
        '        <asp:ImageButton ID="Button2" runat="server" CommandName="Page" ToolTip="Next Page" CommandArgument="Next" BackColor="Transparent" ImageUrl="~/commonimages/rightarrow.gif"/>
        '    </PagerTemplate>
        If e.Row.RowType = DataControlRowType.Pager Then
            Dim lblTotalNumberOfPages As Label = DirectCast(e.Row.FindControl("lblTotalNumberOfPages"), Label)
            lblTotalNumberOfPages.Text = gridView.PageCount.ToString()

            Dim txtGoToPage As TextBox = DirectCast(e.Row.FindControl("txtGoToPage"), TextBox)
            txtGoToPage.Text = (gridView.PageIndex + 1).ToString()

            Dim ddlPageSize As DropDownList = DirectCast(e.Row.FindControl("ddlPageSize"), DropDownList)
            ddlPageSize.SelectedValue = gridView.PageSize.ToString()
        End If

    End Sub

    Protected Sub grdItems_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdItems.RowUpdating
        'This is where do we do updating!
        ODS_Item.UpdateMethod = "UpdateSingleHoliday"
        Dim selectedRow As GridViewRow = grdItems.Rows(e.RowIndex)

        Dim Holiday As String = (CType(selectedRow.FindControl("txtHoliday"), TextBox).Text)
        Dim HolidayDescription As String = (CType(selectedRow.FindControl("txtHolidayDescription"), TextBox).Text)

        If String.IsNullOrEmpty(Holiday) Then Holiday = #1/1/1900#
        If String.IsNullOrEmpty(HolidayDescription) Then HolidayDescription = ""

        ODS_Item.UpdateParameters.Clear()
        ODS_Item.UpdateParameters.Add("Holiday", TypeCode.DateTime, Holiday)
        ODS_Item.UpdateParameters.Add("HolidayDescription", TypeCode.String, HolidayDescription)
    End Sub

End Class