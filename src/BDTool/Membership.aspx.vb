'
'Copyright © 2005, Peter Kellner
'All rights reserved.
'http://peterkellner.net
'
'Redistribution and use in source and binary forms, with or without
'modification, are permitted provided that the following conditions
'are met:
'
'- Redistributions of source code must retain the above copyright
'notice, this list of conditions and the following disclaimer.
'
'- Neither Peter Kellner, nor the names of its
'contributors may be used to endorse or promote products
'derived from this software without specific prior written 
'permission. 
'
'THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
'"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
'LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
'FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
'COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
'INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
'BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
'LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
'CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
'LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
'ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
'POSSIBILITY OF SUCH DAMAGE.
'


Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

Public Partial Class _Default
	Inherits System.Web.UI.Page
	Protected Sub Page_Load(sender As Object, e As EventArgs)
        If User.Identity.IsAuthenticated Then
            If Not cCookie.bCookieExist("UserSettings") Then
                Response.Redirect("login.aspx")
            End If
        Else
            Response.Redirect("login.aspx")
        End If
		' Grab first username and load roles below
		If Not IsPostBack Then
			FindFirstUserName()
		End If
	End Sub

	''' <summary>
	''' Used to retrieve the first user that would normally be processed
	''' by the Membership List
	''' </summary>
	Private Sub FindFirstUserName()
		Dim muc As MembershipUserCollection = Membership.GetAllUsers()
		For Each mu As MembershipUser In muc
			' Just grab the first name then break out of loop
			Dim userName As String = mu.UserName
			ObjectDataSourceRoleObject.SelectParameters("UserName").DefaultValue = userName
			Exit For
		Next
	End Sub


	Protected Sub GridViewMembershipUser_SelectedIndexChanged(sender As Object, e As EventArgs)

		LabelInsertMessage.Text = ""

		Dim gv As GridView = DirectCast(sender, GridView)

		' cover case where there is no current user
		If Membership.GetUser() IsNot Nothing Then
			ObjectDataSourceRoleObject.SelectParameters("UserName").DefaultValue = Membership.GetUser().UserName
			ObjectDataSourceRoleObject.SelectParameters("ShowOnlyAssignedRolls").DefaultValue = "true"
		End If

		GridViewRole.DataBind()
	End Sub
	Protected Sub ButtonCreateNewRole_Click(sender As Object, e As EventArgs)
		If TextBoxCreateNewRole.Text.Length > 0 Then
			ObjectDataSourceRoleObject.InsertParameters("RoleName").DefaultValue = TextBoxCreateNewRole.Text
			

			ObjectDataSourceRoleObject.Insert()
			GridViewRole.DataBind()
			TextBoxCreateNewRole.Text = ""
		End If
	End Sub

	Protected Sub ToggleInRole_Click(sender As Object, e As EventArgs)
		' Grab text from button and parse, not so elegant, but gets the job done
		Dim bt As Button = DirectCast(sender, Button)
		Dim buttonText As String = bt.Text

		Dim seps As Char() = New Char(0) {}
		seps(0) = " "
		Dim buttonTextArray As String() = buttonText.Split(seps)
		Dim roleName As String = buttonTextArray(4)
        If Ubound(buttonTextArray)=5 Then
            roleName=buttonTextArray(4) & " " & buttonTextArray(5)
        End IF
		Dim userName As String = buttonTextArray(1)
		Dim whatToDo As String = buttonTextArray(0)
		Dim userNameArray As String() = New String(0) {}
		userNameArray(0) = userName
		' Need to do this because RemoveUserFromRole requires string array.
		If whatToDo.StartsWith("Un") Then
			' need to remove assignment of this role to this user
			Roles.RemoveUsersFromRole(userNameArray, roleName)
		Else
			Roles.AddUserToRole(userName, roleName)
		End If
		GridViewRole.DataBind()
	End Sub

	Protected Sub ButtonNewUser_Click(sender As Object, e As EventArgs)
		'if (TextBoxUserName.Text.Length > 0 && TextBoxPassword.Text.Length > 0)
		'{
		ObjectDataSourceMembershipUser.InsertParameters("UserName").DefaultValue = TextBoxUserName.Text
		

		ObjectDataSourceMembershipUser.InsertParameters("password").DefaultValue = TextBoxPassword.Text
		ObjectDataSourceMembershipUser.InsertParameters("passwordQuestion").DefaultValue = TextBoxPasswordQuestion.Text
		ObjectDataSourceMembershipUser.InsertParameters("passwordAnswer").DefaultValue = TextBoxPasswordAnswer.Text
		ObjectDataSourceMembershipUser.InsertParameters("email").DefaultValue = TextBoxEmail.Text
		ObjectDataSourceMembershipUser.InsertParameters("isApproved").DefaultValue = If(CheckboxApproval.Checked = True, "true", "false")

		ObjectDataSourceMembershipUser.Insert()
		GridViewMemberUser.DataBind()
		TextBoxUserName.Text = ""
		TextBoxPassword.Text = ""
		TextBoxEmail.Text = ""
		TextBoxPasswordAnswer.Text = ""
		TextBoxPasswordQuestion.Text = ""
		CheckboxApproval.Checked = False
		'}
	End Sub

	Protected Sub GridViewMembership_RowDeleted(sender As Object, e As GridViewDeletedEventArgs)
		FindFirstUserName()
		' Current user is deleted so need to select a new user as current
		GridViewRole.DataBind()
		' update roll lists to reflect new counts
	End Sub


	Protected Function ShowNumberUsersInRole(numUsersInRole As Integer) As String
		Dim result As String
		result = "Number of Users In Role: " & numUsersInRole.ToString()
		Return result
	End Function

	Protected Function ShowInRoleStatus(userName As String, roleName As String) As String
		Dim result As String

		If userName Is Nothing Or roleName Is Nothing Then
			Return "No UserName Specified"
		End If

		If Roles.IsUserInRole(userName, roleName) = True Then
			result = "Unassign " & userName & " From Role " & roleName
		Else
			result = "Assign " & userName & " To Role " & roleName
		End If

		Return result
	End Function


	Protected Sub DetailsView1_ItemInserted(sender As Object, e As DetailsViewInsertedEventArgs)
		GridViewMemberUser.DataBind()
	End Sub
	Protected Sub DetailsView1_PageIndexChanging(sender As Object, e As DetailsViewPageEventArgs)

	End Sub
	Protected Sub ObjectDataSourceMembershipUser_Inserted(sender As Object, e As ObjectDataSourceStatusEventArgs)
		If e.Exception IsNot Nothing Then
			LabelInsertMessage.Text = e.Exception.InnerException.Message & " Insert Failed"
			LabelInsertMessage.ForeColor = System.Drawing.Color.Red

			e.ExceptionHandled = True
		Else
			LabelInsertMessage.Text = "Member " + TextBoxUserName.Text & " Inserted Successfully."
			LabelInsertMessage.ForeColor = System.Drawing.Color.Green
		End If
	End Sub

End Class