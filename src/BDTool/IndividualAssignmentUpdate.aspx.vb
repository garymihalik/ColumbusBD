Imports cCommon
Partial Class IndividualAssignmentUpdate
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Verify user is authenticated
        If User.Identity.IsAuthenticated Then
            If Not cCookie.bCookieExist("UserSettings") Then
                Response.Redirect("login.aspx")
            End If
        Else
            Response.Redirect("login.aspx")
        End If
        Dim bTamperedWith As Boolean = "True"
        If Not Page.IsPostBack Then
            bTamperedWith = cTamperProof.TamperedURL(String.Format("id={0}", Request.QueryString("id")), Request.QueryString("Digest"))
            If bTamperedWith Then
                Response.Redirect("login.aspx")
            End If
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
        'Hide grid's footer row, because I won't be inserting data from this page
        sender.Visible = "false"
    End Sub
    Protected Sub ddlUsers_DataBound(ByVal sender As Object, ByVal e As EventArgs) Handles lstOpportunities.DataBound
        ' loop through all items in the users list
        'http://forums.eworldui.net/ShowPost.aspx?PostID=5180
        lstOpportunities.Items.Insert(0, New ListItem("Client~Opportunity Name~Date Won", -1))
        'lstOpportunities.Items(0).Enabled = False
        Dim tempString As String
        For i As Integer = 0 To lstOpportunities.Items.Count - 1
            tempString = ""
            ' split the text value of the item by the tilde that seperates them
            Dim s As String() = lstOpportunities.Items(i).ToString().Split("~"c)
            ' create a variable to get the amount of charachters in the first value of the split
            'Dim count As Integer = s(0).Trim().Length
            ' set the amount of spaces that we need based on the amount that we want
            ' overall and subtract the amount of the current string
            Dim spaces As Integer
            ' set the text of the dropdownlist item with the spaces added
            ' by calling the method to add the spaces
            For x = 0 To UBound(s)
                Select Case x
                    Case 0
                        spaces = 10 - s(x).Trim.Length
                    Case 1
                        spaces = 75 - s(x).Trim.Length
                    Case 2
                        If IsDate(s(x)) Then
                            s(x) = String.Format("{0:d}", CDate(s(x)))
                        Else
                            s(x) = String.Format("{0:d}", (s(x)))
                        End If
                        spaces = 8 - s(x).Trim.Length
                End Select
                tempString &= s(x).ToString().Trim() + BuildDropDownListSpaces(spaces) & ":"
            Next
            lstOpportunities.Items(i).Text = Left(tempString, tempString.Length - 1) '(s(0).ToString().Trim() + BuildDropDownListSpaces(spaces) & ":") + BuildDropDownListSpaces(2) + s(1).ToString().Trim()
        Next
    End Sub
    Protected Function BuildDropDownListSpaces(ByVal numberOfSpaces As Integer) As String
        Dim spaces As String = ""
        For i As Integer = 0 To numberOfSpaces - 1
            spaces += "&nbsp;"
        Next
        Return Server.HtmlDecode(spaces)
    End Function
    Protected Sub SetUpInitialEditValue(ByVal sender As Object, ByVal e As System.EventArgs)
        If sender.id = "cboPeople" Then
            sender.Items.Insert(0, New ListItem("-Select-", -1))
            Dim gvRow As GridViewRow = DirectCast(sender.namingcontainer, GridViewRow)
            If Not gvRow.DataItem Is Nothing Then
                sender.SelectedValue = DirectCast(gvRow.DataItem, System.Data.DataRowView)("FK_PersonID").ToString
            End If
        End If
    End Sub
    Protected Sub ShowFooterRowOnly(ByVal sender As Object, ByVal e As System.EventArgs) Handles grdAssignments.DataBound
        'Hide if there is the fake row of -1
        If sender.datakeys(0).value = "-1" Then
            sender.Rows(0).Visible = "false"
        End If
    End Sub
    Protected Sub ODS_Assignments_Inserting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceMethodEventArgs) Handles ODS_Assignments.Inserting
        Dim StartDate As String = (CType(grdAssignments.FooterRow.FindControl("txtStartDate"), TextBox).Text)
        Dim EndDate As String = (CType(grdAssignments.FooterRow.FindControl("txtEndDate"), TextBox).Text)
        Dim BillRate As String = (CType(grdAssignments.FooterRow.FindControl("txtBillRate"), TextBox).Text)
        Dim Costs As String = (CType(grdAssignments.FooterRow.FindControl("txtCosts"), TextBox).Text)
        Dim PeriodUtilizationRate As String = (CType(grdAssignments.FooterRow.FindControl("txtPeriodUtilizationRate"), TextBox).Text)
        Dim ExcludeFromBURevenueCalculation As String = (CType(grdAssignments.FooterRow.FindControl("ckExcludeFromBURevenueCalculation"), CheckBox).Checked)

        If String.IsNullOrEmpty(StartDate) Then StartDate = String.Format("{0:d}", Date.Now)
        If String.IsNullOrEmpty(EndDate) Then EndDate = String.Format("{0:d}", Date.Now)
        If String.IsNullOrEmpty(BillRate) Then BillRate = 0
        If String.IsNullOrEmpty(Costs) Then Costs = 0
        If String.IsNullOrEmpty(PeriodUtilizationRate) Then PeriodUtilizationRate = 0

        e.InputParameters("FK_OpportunityID") = Me.lstOpportunities.SelectedValue
        e.InputParameters("FK_PersonID") = CType(grdAssignments.FooterRow.FindControl("cboPeople"), eWorld.UI.MultiTextDropDownList).SelectedValue
        e.InputParameters("StartDate") = StartDate
        e.InputParameters("EndDate") = EndDate
        e.InputParameters("BillRate") = BillRate
        e.InputParameters("Costs") = Costs
        e.InputParameters("ExcludeFromBURevenueCalculation") = ExcludeFromBURevenueCalculation
        e.InputParameters("PeriodUtilizationRate") = PeriodUtilizationRate
    End Sub

    Protected Sub grdAssignments_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdAssignments.RowCommand
        If e.CommandName = "INSERT" Then
            ODS_Assignments.Insert()
        End If
    End Sub
    Protected Sub grdAssignments_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdAssignments.RowDataBound
        Dim lnk As ImageButton
        lnk = e.Row.FindControl("lnkDeleteAssignment")
        If lnk IsNot Nothing Then
            cCommon.CreateConfirmBox2(lnk, "Are you sure you want to delete this assignment? Existing hours and calculated revenue will also be deleted!")
        End If
    End Sub

    Protected Sub grdAssignments_RowUpdating(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewUpdateEventArgs) Handles grdAssignments.RowUpdating
        Dim selectedRow As GridViewRow = grdAssignments.Rows(e.RowIndex)
        Dim StartDate As String = (CType(selectedRow.FindControl("txtStartDate"), TextBox).Text)
        Dim EndDate As String = (CType(selectedRow.FindControl("txtEndDate"), TextBox).Text)
        Dim BillRate As String = (CType(selectedRow.FindControl("txtBillRate"), TextBox).Text)
        Dim Costs As String = (CType(selectedRow.FindControl("txtCosts"), TextBox).Text)
        Dim PeriodUtilizationRate As String = (CType(selectedRow.FindControl("txtPeriodUtilizationRate"), TextBox).Text)
        Dim ExcludeFromBURevenueCalculation As String = (CType(selectedRow.FindControl("ckExcludeFromBURevenueCalculation"), CheckBox).Checked)
        'Dim AssignmentID As String=
        Dim PersonID As String = CType(selectedRow.FindControl("cboPeople"), eWorld.UI.MultiTextDropDownList).SelectedValue
        Dim OppsID As String = (CType(selectedRow.FindControl("lblFK_OpportunityID"), Label).Text)

        If String.IsNullOrEmpty(StartDate) Then StartDate = String.Format("{0:d}", Date.Now)
        If String.IsNullOrEmpty(EndDate) Then EndDate = String.Format("{0:d}", Date.Now)
        If String.IsNullOrEmpty(BillRate) Then BillRate = 0
        If String.IsNullOrEmpty(Costs) Then Costs = 0
        If String.IsNullOrEmpty(PeriodUtilizationRate) Then PeriodUtilizationRate = 0
        ODS_Assignments.UpdateParameters.Clear()

        ODS_Assignments.UpdateParameters.Add("FK_OpportunityID", TypeCode.Double, OppsID)
        ODS_Assignments.UpdateParameters.Add("FK_PersonID", TypeCode.Double, PersonID)
        ODS_Assignments.UpdateParameters.Add("StartDate", TypeCode.DateTime, StartDate)
        ODS_Assignments.UpdateParameters.Add("EndDate", TypeCode.DateTime, EndDate)
        ODS_Assignments.UpdateParameters.Add("BillRate", TypeCode.Double, BillRate)
        ODS_Assignments.UpdateParameters.Add("Costs", TypeCode.Double, Costs)
        ODS_Assignments.UpdateParameters.Add("ExcludeFromBURevenueCalculation", TypeCode.Boolean, ExcludeFromBURevenueCalculation)
        ODS_Assignments.UpdateParameters.Add("PeriodUtilizationRate", TypeCode.Double, PeriodUtilizationRate)

    End Sub

    Protected Sub cboAccounts_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboAccounts.SelectedIndexChanged
        Me.lstOpportunities.ClearSelection()
    End Sub

    Protected Sub BU1_BUSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles BU1.BUSelectionChanged
        cboAccounts.DataBind()
        'Me.lstOpportunities.DataBind()
        Me.grdAssignments.Visible = False
    End Sub

    Protected Sub ODS_Assignments_Selected(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceStatusEventArgs) Handles ODS_Assignments.Selected
        If e.ReturnValue IsNot Nothing Then
            If e.ReturnValue.Tables.Count > 0 Then
                Me.ODS_AccountGroupWins.SelectParameters.Clear()
                If e.ReturnValue.Tables(0).Rows.Count > 0 Then
                    If Not e.ReturnValue.Tables(0).Rows(0).Item("FK_OpportunityID") Is System.DBNull.Value Then
                        Me.ODS_AccountGroupWins.SelectParameters.Add("PK_OpportunityID", TypeCode.Double, e.ReturnValue.Tables(0).Rows(0).Item("FK_OpportunityID"))
                    Else
                        Me.ODS_AccountGroupWins.SelectParameters.Add("PK_OpportunityID", TypeCode.Double, "-1")
                    End If
                End If
            End If
        End If


    End Sub
  
End Class