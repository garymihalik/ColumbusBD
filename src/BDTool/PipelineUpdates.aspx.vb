
Partial Class _Default
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
    End Sub
    Protected Sub SetUpInitialEditValue(ByVal sender As Object, ByVal e As System.EventArgs)
        If FormView1.CurrentMode <> FormViewMode.Insert Then
            Dim dr As Data.DataRow = CType(CType(FormView1.DataItem, Data.DataRowView).Row, Data.DataRow)
            If sender.id = "cboSource" Then
                If Not dr.Item(dr.Table.Columns("Source").Ordinal) Is DBNull.Value Then
                    sender.selectedvalue = CStr(dr.Item(dr.Table.Columns("Source").Ordinal))
                End If
            ElseIf sender.id = "cboFit" Then
                If Not dr.Item(dr.Table.Columns("Fit").Ordinal) Is DBNull.Value Then
                    sender.selectedvalue = CStr(dr.Item(dr.Table.Columns("Fit").Ordinal))
                End If
            End If
        End If
    End Sub
    Protected Sub grdOpportunityUpdate_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        If e.CommandName = "INSERT" Then
            Dim dg As GridView = Me.FormView1.FindControl("grdOpportunityUpdate")
            Dim dtUpdateDate As Date = (CType(dg.FooterRow.FindControl("txtUpdateDate"), TextBox)).Text
            Dim dtCloseDate As Date = (CType(dg.FooterRow.FindControl("txtOpportunityCloseDate"), TextBox)).Text
            Dim WinPercentage As Double = (CType(dg.FooterRow.FindControl("txtWinPercentage"), TextBox)).Text
            Dim NextSteps As String = (CType(dg.FooterRow.FindControl("txtNextSteps"), TextBox)).Text
            Dim EstimatedRevenue As String = (CType(dg.FooterRow.FindControl("txtEstimatedRevenue"), TextBox)).Text
            Dim UpdateNotes As String = (CType(dg.FooterRow.FindControl("txtUpdateNotes"), TextBox)).Text
            Dim UpdatePerson As String = (CType(dg.FooterRow.FindControl("txtUpdatePerson"), TextBox)).Text
            Dim FK_OpporutnityID As Double = Me.lstOpportunities.SelectedValue

            If Not IsNumeric(EstimatedRevenue) Then EstimatedRevenue = 0
            If WinPercentage > 1 Then WinPercentage /= 100


            ODS_OpportunityUpdate.InsertParameters.Clear()
            ODS_OpportunityUpdate.InsertParameters.Add("UpdateDate", TypeCode.DateTime, CStr(dtUpdateDate))
            ODS_OpportunityUpdate.InsertParameters.Add("OpportunityCloseDate", TypeCode.DateTime, CStr(dtCloseDate))
            ODS_OpportunityUpdate.InsertParameters.Add("WinPercentage", TypeCode.Double, CStr(WinPercentage))
            ODS_OpportunityUpdate.InsertParameters.Add("NextSteps", TypeCode.String, CStr(NextSteps))
            ODS_OpportunityUpdate.InsertParameters.Add("EstimatedRevenue", TypeCode.String, CStr(EstimatedRevenue))
            ODS_OpportunityUpdate.InsertParameters.Add("UpdateNotes", TypeCode.String, CStr(UpdateNotes))
            ODS_OpportunityUpdate.InsertParameters.Add("UpdatePerson", TypeCode.String, CStr(UpdatePerson))
            ODS_OpportunityUpdate.InsertParameters.Add("FK_OpportunityID", TypeCode.Double, CStr(FK_OpporutnityID))
            ODS_OpportunityUpdate.InsertParameters.Add("NewItemID", TypeCode.Double, CStr("-1"))
            ODS_OpportunityUpdate.Insert()
        End If
    End Sub


    Protected Sub FormView1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles FormView1.DataBound
        Dim dr As Data.DataRow = CType(CType(FormView1.DataItem, Data.DataRowView).Row, Data.DataRow)
        If Not dr.Item(dr.Table.Columns("Fit").Ordinal) Is DBNull.Value Then
            If CDate(dr.Item(dr.Table.Columns("OpportunityCloseDate").Ordinal)) <= CDate(Format(Now, "short date")) Then
                CType(FormView1.FindControl("lblWarning"), Label).Visible = True
            Else
                CType(FormView1.FindControl("lblWarning"), Label).Visible = False
            End If
        End If
    End Sub
End Class