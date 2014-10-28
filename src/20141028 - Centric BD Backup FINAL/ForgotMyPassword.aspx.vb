Imports cCommon
Partial Class ForgotMyPassword
    Inherits System.Web.UI.Page
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Me.Theme = cLoginUtilities.GetUserTheme
        If Not Page.IsPostBack Then
            If ConfigurationManager.AppSettings("TrackUserEveryPage").ToUpper = "ON" Then
                InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        cCookie.DeleteCookie("UserSettings")
    End Sub

    Protected Sub PasswordRecovery1_UserLookupError(ByVal sender As Object, ByVal e As System.EventArgs) Handles PasswordRecovery1.UserLookupError
        Console.WriteLine(e.ToString)
    End Sub

    Protected Sub PasswordRecovery1_VerifyingAnswer(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) Handles PasswordRecovery1.VerifyingAnswer
        Console.WriteLine(e.ToString)
    End Sub
    Protected Sub PasswordRecovery1_VerifyingUser(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.LoginCancelEventArgs) Handles PasswordRecovery1.VerifyingUser
        Console.WriteLine(e.ToString)
    End Sub
End Class
