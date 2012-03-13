
Partial Class ManageOpportunities
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

    Protected Sub UpdateListBoxITems()
        'Highlight red if in the label list (see ods.selected below.  Highlight if updatedate<=7days old)
        For Each myItem In Me.lstOpportunities.Items
            Dim lstOfReds() As String
            lstOfReds = Split(lblKeepRed.Text, ",")

            Dim lstOfYellows() As String
            lstOfYellows = Split(lblHighlightYellow.Text, ",")

            Dim lstOfLost() As String
            lstOfLost = Split(lblLost.Text, ",")

            Dim lstOfWon() As String
            lstOfWon = Split(lblWon.Text, ",")

            If lstOfReds.Contains(myItem.Value) And lstOfYellows.Contains(myItem.Value) Then
                myItem.Attributes("style") = "background:yellow;color:red"
            ElseIf lstOfReds.Contains(myItem.Value) Then
                myItem.Attributes("style") = "color:red"
            ElseIf lstOfYellows.Contains(myItem.Value) Then
                myItem.Attributes("style") = "background:yellow"
            End If
            'Override if won/lost
            If lstOfLost.Contains(myItem.Value) Then
                myItem.Attributes("style") = "background:lightgray"
            End If
            If lstOfWon.Contains(myItem.Value) Then
                myItem.Attributes("style") = "background:lightgreen"
            End If
        Next
    End Sub
    Protected Sub ShowFooterCode(ByVal sender As Object, ByVal e As System.EventArgs)
        'Hide grid if there isn't an opportunity selected
        If String.IsNullOrEmpty(CStr(lstOpportunities.SelectedValue)) Then
            sender.visible = "false"
        Else
            sender.visible = "true"
        End If
        'Hide row if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Then
            sender.Rows(0).Visible = "false"
        Else
            'Add user as default update person
            Dim myText As TextBox = CType(sender.headerrow.FindControl("txtUpdatePerson"), TextBox)
            myText.Text = cCookie.ReadSingleCookieValue("UserSettings", "betterName")

            'Set Default Values
            CType(sender.headerrow.FindControl("txtOpportunityCloseDate"), TextBox).Text = CType(sender.Rows(0).FindControl("lblOpportunityCloseDate"), Label).Text
            CType(sender.headerrow.FindControl("txtWinPercentage"), TextBox).Text = Trim(Replace(CType(sender.Rows(0).FindControl("lblWinPercentage"), Label).Text, "%", ""))
            CType(sender.headerrow.FindControl("txtEstimatedRevenue"), TextBox).Text = Trim(Replace(Replace(CType(sender.Rows(0).FindControl("lblEstimatedRevenue"), Label).Text, "$", ""), ",", ""))
        End If
    End Sub
    Protected Sub SetUpInitialEditValue(ByVal sender As Object, ByVal e As System.EventArgs)
        If FormView1.CurrentMode <> FormViewMode.Insert And FormView1.DataItem IsNot Nothing Then
            Dim dr As Data.DataRow = CType(CType(FormView1.DataItem, Data.DataRowView).Row, Data.DataRow)
            If sender.id = "cboSource" Then
                If Not dr.Item(dr.Table.Columns("Source").Ordinal) Is DBNull.Value Then
                    sender.selectedvalue = CStr(dr.Item(dr.Table.Columns("Source").Ordinal))
                End If
            ElseIf sender.id = "cboFit" Then
                If Not dr.Item(dr.Table.Columns("Fit").Ordinal) Is DBNull.Value Then
                    sender.selectedvalue = CStr(dr.Item(dr.Table.Columns("Fit").Ordinal))
                End If
            ElseIf sender.id = "cboClients" Then
                If Not dr.Item(dr.Table.Columns("ClientID").Ordinal) Is DBNull.Value Then
                    sender.selectedvalue = CStr(dr.Item(dr.Table.Columns("ClientID").Ordinal))
                End If
            End If
        End If
    End Sub
    Protected Sub grdOpportunityUpdate_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs)
        If e.CommandName = "INSERT" Then
            Dim dg As GridView = Me.FormView1.FindControl("grdOpportunityUpdate")
            Dim dtUpdateDate As Date = (CType(dg.HeaderRow.FindControl("txtUpdateDate"), TextBox)).Text
            Dim dtCloseDate As Date = (CType(dg.HeaderRow.FindControl("txtOpportunityCloseDate"), TextBox)).Text
            Dim WinPercentage As String = (CType(dg.HeaderRow.FindControl("txtWinPercentage"), TextBox)).Text
            Dim NextSteps As String = (CType(dg.HeaderRow.FindControl("txtNextSteps"), TextBox)).Text
            Dim EstimatedRevenue As String = (CType(dg.HeaderRow.FindControl("txtEstimatedRevenue"), TextBox)).Text
            Dim UpdateNotes As String = (CType(dg.HeaderRow.FindControl("txtUpdateNotes"), TextBox)).Text
            Dim UpdatePerson As String = (CType(dg.HeaderRow.FindControl("txtUpdatePerson"), TextBox)).Text
            Dim FK_OpporutnityID As Double = Me.lstOpportunities.SelectedValue

            If Not IsNumeric(EstimatedRevenue) Then EstimatedRevenue = 0
            If IsNumeric(WinPercentage) Then
                If WinPercentage > 1 Then WinPercentage /= 100
            End If

            ODS_OpportunityUpdate.InsertParameters.Clear()
            ODS_OpportunityUpdate.InsertParameters.Add("UpdateDate", TypeCode.DateTime, CStr(dtUpdateDate))
            ODS_OpportunityUpdate.InsertParameters.Add("OpportunityCloseDate", TypeCode.DateTime, CStr(dtCloseDate))
            ODS_OpportunityUpdate.InsertParameters.Add("WinPercentage", TypeCode.String, Trim(Replace(CStr(WinPercentage), "%", "")))
            ODS_OpportunityUpdate.InsertParameters.Add("NextSteps", TypeCode.String, CStr(NextSteps))
            ODS_OpportunityUpdate.InsertParameters.Add("EstimatedRevenue", TypeCode.String, Trim(CStr(Replace(Replace(EstimatedRevenue, "$", ""), ",", ""))))
            ODS_OpportunityUpdate.InsertParameters.Add("UpdateNotes", TypeCode.String, CStr(UpdateNotes))
            ODS_OpportunityUpdate.InsertParameters.Add("UpdatePerson", TypeCode.String, CStr(UpdatePerson))
            ODS_OpportunityUpdate.InsertParameters.Add("FK_OpportunityID", TypeCode.Double, CStr(FK_OpporutnityID))
            ODS_OpportunityUpdate.InsertParameters.Add("NewItemID", TypeCode.Double, CStr("-1"))
            ODS_OpportunityUpdate.Insert()
        End If
    End Sub

    Protected Sub FormView1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles FormView1.DataBound

        If FormView1.CurrentMode = FormViewMode.ReadOnly Then
            Dim dr As Data.DataRow = CType(CType(FormView1.DataItem, Data.DataRowView).Row, Data.DataRow)
            If Not dr.Item(dr.Table.Columns("OpportunityCloseDate").Ordinal) Is DBNull.Value Then
                If CDate(dr.Item(dr.Table.Columns("OpportunityCloseDate").Ordinal)) <= CDate(String.Format("{0:d}", Date.Now)) Then
                    CType(FormView1.FindControl("lblWarning"), Label).Visible = True
                Else
                    CType(FormView1.FindControl("lblWarning"), Label).Visible = False
                End If
            End If
        ElseIf FormView1.CurrentMode = FormViewMode.Insert Then
            CType(FormView1.FindControl("cboClients"), eWorld.UI.MultiTextDropDownList).Items.Insert(0, New ListItem("-Select-", -1))
            CType(FormView1.FindControl("txtUpdatePerson"), TextBox).Text = cCookie.ReadSingleCookieValue("UserSettings", "betterName")
        End If
    End Sub

    Protected Sub FormView1_ModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles FormView1.ModeChanged
        If FormView1.CurrentMode <> FormViewMode.ReadOnly Then
            Me.lstOpportunities.Visible = False
            Me.RadioButtonList1.Visible = False
            Me.BU1.Visible = False
        Else
            Me.lstOpportunities.Visible = True
            Me.RadioButtonList1.Visible = True
            Me.BU1.Visible = True
        End If
        UpdateListBoxITems()
    End Sub

    Protected Sub ODS_SingleOpportunity_Inserted(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles ODS_SingleOpportunity.Inserted
        lstOpportunities.DataBind()
    End Sub

    Protected Sub ODS_SingleOpportunity_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles ODS_SingleOpportunity.Inserting
        e.InputParameters("OpportunityOwner") = CType(FormView1.FindControl("Owner"), TextBox).Text
        e.InputParameters("OpportunityName") = CType(FormView1.FindControl("txtName"), TextBox).Text
        e.InputParameters("Skills") = "None"
        e.InputParameters("Anchor") = "False"
        e.InputParameters("ClientID") = CType(FormView1.FindControl("cboClients"), eWorld.UI.MultiTextDropDownList).SelectedValue
        e.InputParameters("Fit") = CType(FormView1.FindControl("cboFit"), DropDownList).SelectedValue
        e.InputParameters("Source") = CType(FormView1.FindControl("cboSource"), DropDownList).SelectedValue
        e.InputParameters("Extension") = CStr(CType(FormView1.FindControl("ckExtension"), CheckBox).Checked)
        e.InputParameters("DateEntered") = Now()
        e.InputParameters("OpportunityCloseDate") = CStr(CType(FormView1.FindControl("txtCloseDate"), TextBox).Text)
        e.InputParameters("EstimatedRevenue") = Trim(Replace(Replace(CStr(CType(FormView1.FindControl("txtEstimatedRevenue"), TextBox).Text), ",", ""), "$", ""))
        e.InputParameters("NextSteps") = CType(FormView1.FindControl("txtNextSteps"), TextBox).Text
        'txtEstimatedRevenue
        Dim tempWin As String = CStr(CType(FormView1.FindControl("txtWinPercentage"), TextBox).Text)
        If String.IsNullOrEmpty(tempWin) Or Not IsNumeric(tempWin) Then
            'Set this as null/ not qualified.  The CRUD code replaces -1 with a null value
            e.InputParameters("WinPercentage") = "-1"
        Else
            e.InputParameters("WinPercentage") = tempWin
        End If
        e.InputParameters("UpdatePerson") = CStr(CType(FormView1.FindControl("txtUpdatePerson"), TextBox).Text)
        e.InputParameters("DateEntered") = Now
        e.InputParameters("RolesNeeded") = CStr(CType(FormView1.FindControl("txtRolesNeeded"), TextBox).Text)
        e.InputParameters("RFPRequired") = CStr(CType(FormView1.FindControl("ckRFPRequired"), CheckBox).Checked)
        e.InputParameters("RFPLead") = CStr(CType(FormView1.FindControl("txtRFPLead"), TextBox).Text)
        e.InputParameters("RFPRiskAssessment") = CStr(CType(FormView1.FindControl("txtRFPRiskAssessment"), TextBox).Text)
    End Sub

    Protected Sub ODS_SingleOpportunity_Updated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles ODS_SingleOpportunity.Updated
        'Refreshes list!
        lstOpportunities.DataBind()
    End Sub


    Protected Sub ODS_Opportunities_Updating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles ODS_SingleOpportunity.Updating
        e.InputParameters("PK_OpportunityID") = CType(FormView1.FindControl("txtPK_OpportunityID"), TextBox).Text
        e.InputParameters("Original_PK_OpportunityID") = CType(FormView1.FindControl("txtPK_OpportunityID"), TextBox).Text
        e.InputParameters("OpportunityOwner") = CType(FormView1.FindControl("Owner"), TextBox).Text
        e.InputParameters("OpportunityName") = CType(FormView1.FindControl("txtName"), TextBox).Text
        e.InputParameters("Skills") = "None"
        e.InputParameters("Anchor") = "False"
        e.InputParameters("ClientID") = CType(FormView1.FindControl("cboClients"), eWorld.UI.MultiTextDropDownList).SelectedValue
        e.InputParameters("Fit") = CType(FormView1.FindControl("cboFit"), DropDownList).SelectedValue
        e.InputParameters("Source") = CType(FormView1.FindControl("cboSource"), DropDownList).SelectedValue
        e.InputParameters("Extension") = CStr(CType(FormView1.FindControl("ckExtension"), CheckBox).Checked)
        e.InputParameters("LastUpdateBy") = cCookie.ReadSingleCookieValue("UserSettings", "betterName")
        e.InputParameters("RolesNeeded") = CStr(CType(FormView1.FindControl("txtRolesNeeded"), TextBox).Text)
        e.InputParameters("RFPRequired") = CStr(CType(FormView1.FindControl("ckRFPRequired"), CheckBox).Checked)
        e.InputParameters("RFPLead") = CStr(CType(FormView1.FindControl("txtRFPLead"), TextBox).Text)
        e.InputParameters("RFPRiskAssessment") = CStr(CType(FormView1.FindControl("txtRFPRiskAssessment"), TextBox).Text)
    End Sub

    Protected Sub lstOpportunities_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstOpportunities.DataBound, lstOpportunities.SelectedIndexChanged, ODS_OpportunityUpdate.Inserted
        UpdateListBoxITems()
    End Sub

    Protected Sub ODS_Opportunities_Selected(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles ODS_Opportunities.Selected
        'Store each ID that has an updatedate of less than 7 days in a label.  I'll refer to this label to determine whether or not
        'to highlight an entry
        If e.ReturnValue IsNot Nothing Then
            For x = 0 To e.ReturnValue.Tables(0).Rows.Count - 1
                If IsDate(e.ReturnValue.Tables(0).Rows(x).Item("UpdateDate")) Then
                    With e.ReturnValue.Tables(0).Rows(x)
                        If DateDiff(DateInterval.Day, .Item("UpdateDate"), DateTime.Now) <= 5 Then
                            Me.lblHighlightYellow.Text &= "," & .Item("PK_OpportunityID")
                        End If
                    End With
                End If

                'Store each ID that has a close date of <7 days in a label.  I'll refer to this label to determine whether or not to make the text red
                If IsDate(e.ReturnValue.Tables(0).Rows(x).Item("OpportunityCloseDate")) Then
                    With e.ReturnValue.Tables(0).Rows(x)
                        If DateDiff(DateInterval.Day, Now, .Item("OpportunityCloseDate")) <= 5 Then
                            Me.lblKeepRed.Text &= "," & .Item("PK_OpportunityID")
                        End If
                    End With
                End If

                'Store each ID that is won or loss, and mark it gray.
                If IsNumeric(e.ReturnValue.Tables(0).Rows(x).item("WinPercentage")) Then
                    With e.ReturnValue.Tables(0).Rows(x)
                        If .item("WinPercentage") = 0 Then
                            lblLost.Text &= "," & .Item("PK_OpportunityID")
                        ElseIf .item("WinPercentage") = 1 Then
                            lblWon.Text &= "," & .Item("PK_OpportunityID")
                        End If
                    End With
                End If
            Next

        End If
    End Sub

    Protected Sub BU1_BUSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BU1.BUSelectionChanged
        lstOpportunities.DataBind()
    End Sub
    Protected Sub grdOpportunityUpdate_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs)
        Dim tempLabel As New Label
        If e.Row.RowType = DataControlRowType.Header Then
            tempLabel = CType(e.Row.FindControl("lblWinPercentage"), Label)
            tempLabel.ToolTip = "Blank=Not Yet Qualified" & vbNewLine & "0%=Lost" & vbNewLine
            tempLabel.ToolTip &= "10%=Opportunity known, discussed with client" & vbNewLine & "30%=Detailed discussion with client" & vbNewLine
            tempLabel.ToolTip &= "50%=Proposal" & vbNewLine & "70%=Short List/Asked to Present Resources" & vbNewLine
            tempLabel.ToolTip &= "90%=Verbal Commitment" & vbNewLine & "100%=Won"
        End If

    End Sub
End Class