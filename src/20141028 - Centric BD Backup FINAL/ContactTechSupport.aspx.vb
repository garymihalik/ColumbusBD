Imports System.Net.Mail
Imports cCommon
Partial Class ContactTechSupport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If User.Identity.IsAuthenticated Then
            If Not cCookie.bCookieExist("UserSettings") Then
                Response.Redirect("login.aspx")
            End If
        Else
            Response.Redirect("login.aspx")
        End If
        LoadUserData()
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Me.Theme = cLoginUtilities.GetUserTheme
        If Not Page.IsPostBack Then
            If ConfigurationManager.AppSettings("TrackUserEveryPage").ToUpper = "ON" Then
                InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If
    End Sub
    Protected Sub LoadUserData()
        Dim output As New System.Text.StringBuilder()
        Dim strArray() As String
        Dim aCookie As HttpCookie
        For i As Integer = 0 To Request.Cookies.Count - 1
            aCookie = Request.Cookies(i)
            output.Append("Cookie " & i & " <br />")
            output.Append("Cookie name = " + Server.HtmlEncode(aCookie.Name) + "<br />")
            output.Append("Variable = Value <br />")
            strArray = Split(aCookie.Value, "&")
            If UBound(strArray) = 0 Then
                output.Append("No Variable Name =" & strArray(0) & "<br/>")
            End If
            For x = 0 To UBound(strArray) - 1
                output.Append(strArray(x) & "<br/>")
            Next
            output.Append("<hr/><br/>")
            'output.Append("Cookie value = " + Server.HtmlEncode(aCookie.Value) + "<br /><br />")
        Next
        Me.CookiePlaceHolder1.Text = output.ToString()

        Dim output2 As New System.Text.StringBuilder()
        output2.Append("Session Variable = Value<br/>")
        For g As Integer = 0 To Session.Keys.Count - 1
            output2.Append(Session.Keys(g) & " = " & Session.Item(g).ToString & "<br/>")
        Next
        output2.Append("<hr/><br/>")
        Me.SessionPlaceHolder1.Text = output2.ToString

        Dim output3 As New StringBuilder
        Dim ds As System.Data.DataSet = ExceptionHandler.GetExceptions.GetLastError
        output3.Append("<table>")
        If ds IsNot Nothing Then
            output3.Append("<tr><td>Event ID</td><td>")
            output3.Append(ds.Tables(0).Rows(0).Item("EventID") & "</td></tr>")

            output3.Append("<tr><td>Referer</td><td>")
            output3.Append(ds.Tables(0).Rows(0).Item("Referer") & "</td></tr>")

            output3.Append("<tr><td>QueryString</td><td>")
            output3.Append(ds.Tables(0).Rows(0).Item("QueryString") & "</td></tr>")

            output3.Append("<tr><td>Date/Time</td><td>")
            output3.Append(ds.Tables(0).Rows(0).Item("LogDateTime") & "</td></tr>")

            output3.Append("<tr><td>Stack Trace</td><td>")
            output3.Append(ds.Tables(0).Rows(0).Item("StackTrace") & "</td></tr>")

            output3.Append("<tr><td>Message</td><td>")
            output3.Append(ds.Tables(0).Rows(0).Item("Message") & "</td></tr>")
        End If
        output3.Append("</table>")
        ServerErrorPlaceholder1.Text = output3.ToString
    End Sub

    Protected Sub EmailReport_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles EmailReport.Click
        'Insert Email Code
        Dim emlTo As New MailAddress("joseph.ours@centricconsulting.com")

        Dim msg As New MailMessage
        msg.Subject = "Error Report"
        msg.IsBodyHtml = True
        msg.BodyEncoding = System.Text.Encoding.ASCII
        msg.Body = Me.ServerErrorPlaceholder1.Text & "<br/> " & Me.CookiePlaceHolder1.Text & "<br/> " & Me.SessionPlaceHolder1.Text & "<br/> sent by " & User.Identity.Name
        msg.From = New MailAddress("error@centricbd.com")
        msg.To.Add(emlTo)

        Dim sc As SmtpClient
        Try
            sc = New SmtpClient()
            sc.Send(msg)
        Catch ex As Exception
            cErrorManagement.InsertErrorMessage(ex.ToString, "ContactTechSupport.EmailReport")
        End Try
        Me.results.Text = "Tech Support has been notified of the issue."
        Me.EmailReport.Enabled = False
    End Sub
End Class