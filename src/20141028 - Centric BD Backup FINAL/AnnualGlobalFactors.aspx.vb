
Partial Class AnnualGlobalFactors
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
    Protected Sub SetUpInitialEditValue(ByVal sender As Object, ByVal e As System.EventArgs)
        If sender.id = "cboReportMonth" Then
            sender.Items.Insert(0, New ListItem("-Select-", -1))
            Dim gvRow As GridViewRow = DirectCast(sender.namingcontainer, GridViewRow)
            If Not gvRow.DataItem Is Nothing Then
                'ReportMonth is fieldname of dataset
                sender.SelectedValue = DirectCast(gvRow.DataItem, System.Data.DataRowView)("ReportMonth").ToString
            End If
        End If
    End Sub
    Protected Sub SetUpCurrentMonth(ByVal sender As Object, ByVal e As System.EventArgs)
        cCommonDateCode.SetupInitialDate(sender, e)
    End Sub

    Protected Sub ShowFooterCode(ByVal sender As Object, ByVal e As System.EventArgs)
        'Hide row if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Then
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
            Me.ODS_Item.InsertMethod = "InsertAnnualReport"
            Dim selectedRow As GridViewRow = sender.FooterRow

            Dim ReportYearValue As String = (CType(selectedRow.FindControl("txtReportYear"), TextBox).Text)
            Dim NonSalaryEmpFactorValue As String = (CType(selectedRow.FindControl("txtNonSalaryEmpFactor"), TextBox).Text)
            Dim AvgFTECompValue As String = (CType(selectedRow.FindControl("txtAvgFTEComp"), TextBox).Text)
            Dim LastUpdateByValue As String = cCookie.ReadSingleCookieValue("UserSettings", "betterName")
            Dim LastUpdateDateValue As String = String.Format(Date.Now)

            If String.IsNullOrEmpty(ReportYearValue) Then ReportYearValue = 2000
            If String.IsNullOrEmpty(NonSalaryEmpFactorValue) Then NonSalaryEmpFactorValue = 0.18
            If String.IsNullOrEmpty(AvgFTECompValue) Then AvgFTECompValue = 90000

            ODS_Item.InsertParameters.Add("ReportYear", TypeCode.Double, ReportYearValue)
            ODS_Item.InsertParameters.Add("NonSalaryEmpFactor", TypeCode.Double, NonSalaryEmpFactorValue)
            ODS_Item.InsertParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
            ODS_Item.InsertParameters.Add("LastUpdateBy", TypeCode.String, LastUpdateByValue)
            ODS_Item.InsertParameters.Add("LastUpdateDate", TypeCode.String, LastUpdateDateValue)
            ODS_Item.InsertParameters.Add("AvgFTEComp", TypeCode.Double, AvgFTECompValue)


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
        ODS_Item.UpdateMethod = "UpdateAnnualReport"
        Dim selectedRow As GridViewRow = grdItems.Rows(e.RowIndex)

        Dim GlobalAnnualIDValue As String = (CType(selectedRow.FindControl("lblGlobalAnnualID"), Label).Text)
        Dim ReportYearValue As String = (CType(selectedRow.FindControl("txtReportYear"), TextBox).Text)
        Dim NonSalaryEmpFactorValue As String = (CType(selectedRow.FindControl("txtNonSalaryEmpFactor"), TextBox).Text)
        Dim AvgFTECompValue As String = (CType(selectedRow.FindControl("txtAvgFTEComp"), TextBox).Text)
        Dim LastUpdateByValue As String = cCookie.ReadSingleCookieValue("UserSettings", "betterName")
        Dim LastUpdateDateValue As String = String.Format(Date.Now)

        If String.IsNullOrEmpty(GlobalAnnualIDValue) Then GlobalAnnualIDValue = 0
        If String.IsNullOrEmpty(ReportYearValue) Then ReportYearValue = 2000
        If String.IsNullOrEmpty(NonSalaryEmpFactorValue) Then NonSalaryEmpFactorValue = 0.18
        If String.IsNullOrEmpty(AvgFTECompValue) Then AvgFTECompValue = 90000
        ODS_Item.UpdateParameters.Clear()

        ODS_Item.UpdateParameters.Add("GlobalAnnualID", TypeCode.Double, GlobalAnnualIDValue)
        ODS_Item.UpdateParameters.Add("ReportYear", TypeCode.Double, ReportYearValue)
        ODS_Item.UpdateParameters.Add("NonSalaryEmpFactor", TypeCode.Double, NonSalaryEmpFactorValue)
        ODS_Item.UpdateParameters.Add("BusinessUnit", TypeCode.Double, CStr(BU1.BU_ID))
        ODS_Item.UpdateParameters.Add("LastUpdateBy", TypeCode.String, LastUpdateByValue)
        ODS_Item.UpdateParameters.Add("LastUpdateDate", TypeCode.String, LastUpdateDateValue)
        ODS_Item.UpdateParameters.Add("AvgFTEComp", TypeCode.Double, AvgFTECompValue)


    End Sub

End Class