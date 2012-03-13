
Partial Class ManageAccountGroup
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
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdAccountGroupItems.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Or sender.datakeys(0).value = -1 Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub

    Protected Sub grdAccountGroupItems_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdAccountGroupItems.RowUpdating
        'test
        Dim AccountGroup As String
        Dim ForecastOwner As String
        Dim AccountOwner As String
        Dim ReportOn As String
        Dim ReportOrder As String
        Dim AccountGroupID As String

        'This is the save after the edit
        Me.ODS_AccountGroup.UpdateParameters.Clear()
        Dim selectedRow As GridViewRow = grdAccountGroupItems.Rows(e.RowIndex)
        AccountGroup = CType(selectedRow.FindControl("txtAccountGroup"), TextBox).Text
        ForecastOwner = CType(selectedRow.FindControl("txtForecastOwner"), TextBox).Text
        AccountOwner = CType(selectedRow.FindControl("txtAccountOwner"), TextBox).Text
        ReportOn = CType(selectedRow.FindControl("ckReportOn"), CheckBox).Checked
        ReportOrder = CType(selectedRow.FindControl("txtReportOrder"), TextBox).Text
        AccountGroupID = CType(selectedRow.FindControl("lblAccountGroupID"), Label).Text
        If String.IsNullOrEmpty(AccountGroupID) Then
            'If AccountGroupID is null, then let's add them as new!
            Me.ODS_AccountGroup.InsertMethod = "InsertSingleAccountGroup"
            ODS_AccountGroup.InsertParameters.Add("AccountGroup", TypeCode.String, AccountGroup)
            ODS_AccountGroup.InsertParameters.Add("ForecastOwner", TypeCode.String, ForecastOwner)
            ODS_AccountGroup.InsertParameters.Add("AccountOwner", TypeCode.String, AccountOwner)
            ODS_AccountGroup.InsertParameters.Add("ReportOn", TypeCode.Boolean, ReportOn)
            ODS_AccountGroup.InsertParameters.Add("BusinessUnit", TypeCode.Int32, CStr(BU1.BU_ID))
            ODS_AccountGroup.InsertParameters.Add("ReportOrder", TypeCode.Double, ReportOrder)
            ODS_AccountGroup.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
        Else
            Me.ODS_AccountGroup.UpdateMethod = "UpdateSingleAccountGroup"
            ODS_AccountGroup.UpdateParameters.Add("AccountGroup", TypeCode.String, AccountGroup)
            ODS_AccountGroup.UpdateParameters.Add("AccountOwner", TypeCode.String, AccountOwner)
            ODS_AccountGroup.UpdateParameters.Add("ForecastOwner", TypeCode.String, ForecastOwner)
            ODS_AccountGroup.UpdateParameters.Add("ReportOn", TypeCode.Boolean, ReportOn)
            ODS_AccountGroup.UpdateParameters.Add("BusinessUnit", TypeCode.Int32, CStr(BU1.BU_ID))
            ODS_AccountGroup.UpdateParameters.Add("ReportOrder", TypeCode.Int32, ReportOrder)
            ODS_AccountGroup.UpdateParameters.Add("ACCOUNTGROUPID", TypeCode.Int32, AccountGroupID)
            ODS_AccountGroup.UpdateParameters.Add("ORIGINAL_ACCOUNTGROUPID", TypeCode.Int32, AccountGroupID)
        End If
    End Sub
    Protected Sub grdAccountGroupItems_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdAccountGroupItems.RowCommand
        'current e.CommandNames are insert, cancel, update, edit
        Dim AccountGroup As String
        Dim ForecastOwner As String
        Dim AccountOwner As String
        Dim ReportOn As String
        Dim ReportOrder As String



        If e.CommandName.ToUpper = "INSERT" Then
            Me.ODS_AccountGroup.InsertParameters.Clear()
            AccountGroup = CType(sender.FooterRow.FindControl("txtAccountGroup"), TextBox).Text
            ForecastOwner = CType(sender.FooterRow.FindControl("txtForecastOwner"), TextBox).Text
            AccountOwner = CType(sender.FooterRow.FindControl("txtAccountOwner"), TextBox).Text
            ReportOn = CType(sender.FooterRow.FindControl("ckReportOn"), CheckBox).Checked
            ReportOrder = CType(sender.FooterRow.FindControl("txtReportOrder"), TextBox).Text

            
            ODS_AccountGroup.InsertMethod = "InsertSingleAccountGroup"
            ODS_AccountGroup.InsertParameters.Add("AccountGroup", TypeCode.String, AccountGroup)
            ODS_AccountGroup.InsertParameters.Add("ForecastOwner", TypeCode.String, ForecastOwner)
            ODS_AccountGroup.InsertParameters.Add("AccountOwner", TypeCode.String, AccountOwner)
            ODS_AccountGroup.InsertParameters.Add("ReportOn", TypeCode.Boolean, ReportOn)
            ODS_AccountGroup.InsertParameters.Add("BusinessUnit", TypeCode.Int32, CStr(BU1.BU_ID))
            ODS_AccountGroup.InsertParameters.Add("ReportOrder", TypeCode.Double, ReportOrder)
            ODS_AccountGroup.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
            ODS_AccountGroup.Insert()
        End If
    End Sub
    Protected Sub grdAccountGroupItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdAccountGroupItems.RowDataBound
        Dim lnk As ImageButton
        lnk = e.Row.FindControl("lnkDeleteAccountGroup")
        If lnk IsNot Nothing Then
            'Console.WriteLine(lnk.CommandArgument)
            cCommon.CreateConfirmBox2(lnk, "Are you sure you want to delete this Account Group? ")
        End If
    End Sub

End Class