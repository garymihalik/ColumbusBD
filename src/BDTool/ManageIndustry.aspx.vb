
Partial Class ManageIndustry
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
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdIndustryItems.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub

    Protected Sub grdIndustryItems_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdIndustryItems.RowUpdating
        'test
        Dim Industry As String
        Dim IndustryID As String

        'This is the save after the edit
        Me.ODS_Industry.UpdateParameters.Clear()
        Dim selectedRow As GridViewRow = grdIndustryItems.Rows(e.RowIndex)
        Industry = CType(selectedRow.FindControl("txtIndustry"), TextBox).Text
        IndustryID = CType(selectedRow.FindControl("lblIndustryID"), Label).Text
        If String.IsNullOrEmpty(IndustryID) Then
            'If IndustryID is null, then let's add them as new!
            Me.ODS_Industry.InsertMethod = "InsertSingleIndustry"
            ODS_Industry.InsertParameters.Add("Industry", TypeCode.String, Industry)
            ODS_Industry.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
        Else
            Me.ODS_Industry.UpdateMethod = "UpdateSingleIndustry"
            ODS_Industry.UpdateParameters.Add("Industry", TypeCode.String, Industry)
            ODS_Industry.UpdateParameters.Add("IndustryID", TypeCode.Int32, IndustryID)
            ODS_Industry.UpdateParameters.Add("ORIGINAL_IndustryID", TypeCode.Int32, IndustryID)
        End If
    End Sub
    Protected Sub grdIndustryItems_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdIndustryItems.RowCommand
        'current e.CommandNames are insert, cancel, update, edit
        Dim Industry As String
        If e.CommandName.ToUpper = "INSERT" Then
            Me.ODS_Industry.InsertParameters.Clear()
            Industry = CType(sender.FooterRow.FindControl("txtIndustry"), TextBox).Text

            ODS_Industry.InsertMethod = "InsertSingleIndustry"
            ODS_Industry.InsertParameters.Add("Industry", TypeCode.String, Industry)
            ODS_Industry.InsertParameters.Add("NewItemID", TypeCode.Int32, CStr(-1))
            ODS_Industry.Insert()
        End If
    End Sub
    Protected Sub grdIndustryItems_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdIndustryItems.RowDataBound
        Dim lnk As ImageButton
        lnk = e.Row.FindControl("lnkDeleteIndustry")
        If lnk IsNot Nothing Then
            'Console.WriteLine(lnk.CommandArgument)
            cCommon.CreateConfirmBox2(lnk, "Are you sure you want to delete this Industry? ")
        End If
    End Sub

End Class