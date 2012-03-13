Imports cCommon
Partial Class Login
    Inherits System.Web.UI.Page
    Public bLogDetails As Boolean = ConfigurationManager.AppSettings("LogDetails")
    Protected Sub Login1_LoggedIn(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoggedIn
        Dim myListOfRoles As String = cLoginUtilities.AuthenticateLoginID(Login1.UserName)
        If cCookie.ReadSingleCookieValue("UserSettings", "UserName") = Login1.UserName Then
            If CBool(InStr(cCookie.ReadSingleCookieValue("UserSettings", "UserName"), "Error")) Then
                cCookie.AddValue("UserSettings", "UserName", Login1.UserName)
            End If
            If CBool(InStr(cCookie.ReadSingleCookieValue("UserSettings", "BU"), "Error")) Then
                cCookie.AddValue("UserSettings", "BU", myListOfRoles)
            End If
        Else
            cCookie.DeleteCookie("UserSettings")
            cCookie.AddValue("UserSettings", "UserName", Login1.UserName)
            cCookie.AddValue("UserSettings", "BU", myListOfRoles)
        End If


        If Not Page.IsPostBack Then
            If ConfigurationManager.AppSettings("TrackUserEveryPage").ToUpper = "ON" Then
                InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If
    End Sub

    Protected Sub Login1_LoginError(ByVal sender As Object, ByVal e As System.EventArgs) Handles Login1.LoginError
        'There was a problem logging in the user

        'See if this user exists in the database
        Dim userInfo As MembershipUser = Membership.GetUser(User.Identity.Name)

        If userInfo Is Nothing Then
            'The user entered an invalid username...
            LoginErrorDetails.Text = "There is no user in the database with the username " & User.Identity.Name
            'If bLogDetails Then cJoesUtilities.WriteLogData("userInfo is nothing")
        Else
            'See if the user is locked out or not approved
            If Not userInfo.IsApproved Then
                LoginErrorDetails.Text = "Your account has not yet been approved by the site's administrators. Please try again later..."
                'If bLogDetails Then cJoesUtilities.WriteLogData("userInfo.IsApproved=False")
            ElseIf userInfo.IsLockedOut Then
                LoginErrorDetails.Text = "Your account has been locked out because of a maximum number of incorrect login attempts. You will NOT be able to login until you contact a site administrator and have your account unlocked."
                'If bLogDetails Then cJoesUtilities.WriteLogData("userInfo.IsLockedOut=True")
            Else
                'The password was incorrect (don't show anything, the Login control already describes the problem)
                LoginErrorDetails.Text = "password was incorrect"
                'If bLogDetails Then cJoesUtilities.WriteLogData("userInfo has some other issue.")
                'If bLogDetails Then cJoesUtilities.WriteLogData(userInfo.ToString)
                'If bLogDetails Then cJoesUtilities.WriteLogData(e.ToString)
                'If bLogDetails Then cJoesUtilities.WriteLogData(sender.ToString)
            End If
        End If
        If ConfigurationManager.AppSettings("TrackUserLogin").ToUpper = "ON" Then
            'cJoesUtilities.InsertPageVisitDetail(User.Identity.Name, Me.Page, Me.ViewState, Me.Login1.Password & " - Failed Login")
        End If
        If bLogDetails Then
            If userInfo IsNot Nothing Then
                'cJoesUtilities.WriteLogData(userInfo.Comment)
                'cJoesUtilities.WriteLogData(userInfo.CreationDate)
                'cJoesUtilities.WriteLogData(userInfo.Email)
                'cJoesUtilities.WriteLogData(userInfo.IsApproved)
                'cJoesUtilities.WriteLogData(userInfo.IsLockedOut)
                'cJoesUtilities.WriteLogData(userInfo.IsOnline)
                'cJoesUtilities.WriteLogData(userInfo.LastActivityDate)
                'cJoesUtilities.WriteLogData(userInfo.LastLockoutDate)
                'cJoesUtilities.WriteLogData(userInfo.LastLoginDate)
                'cJoesUtilities.WriteLogData(userInfo.LastPasswordChangedDate)
                'cJoesUtilities.WriteLogData(userInfo.PasswordQuestion)
                'cJoesUtilities.WriteLogData(userInfo.ProviderName)
                'cJoesUtilities.WriteLogData(userInfo.ProviderUserKey.ToString)
                'cJoesUtilities.WriteLogData(userInfo.UserName)
                'cJoesUtilities.WriteLogData(Membership.ValidateUser(User.Identity.Name, Login1.Password))
            Else
                'cJoesUtilities.WriteLogData("userinfo is empty")
            End If
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If cCookie.bCookieExist("UserSettings") Then
            Me.Login1.UserName = cCookie.ReadSingleCookieValue("UserSettings", "UserName")
        End If
    End Sub

    Protected Sub btnClearCookies_Click(ByVal sender As Object, ByVal e As System.EventArgs) 'Handles btnClearCookies.Click
        cCookie.DeleteCookie("UserSettings")
    End Sub

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Me.Theme = cLoginUtilities.GetUserTheme
        Me.Theme = "Centric4"
    End Sub
End Class
