Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.ComponentModel
Imports System.IO
Public Class cCommon


    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    'Public Shared conString As String = ConfigurationManager.AppSettings("DBConnection")

    Public Shared Function GetUserTheme() As String
        Dim strTheme As String = cCookie.ReadSingleCookieValue("UserSettings", "Theme")
        If String.IsNullOrEmpty(strTheme) Or strTheme.IndexOf("Error") >= 0 Then
            GetUserTheme = "Centric4"
        Else
            GetUserTheme = strTheme
        End If
    End Function
    Public Shared Sub CreateConfirmBox(ByRef btn As WebControls.Button, ByVal strMessage As String)
        btn.Attributes.Add("onclick", "return confirm('" & strMessage & "');")
    End Sub
    Public Shared Sub CreateConfirmBox2(ByRef btn As WebControls.ImageButton, ByVal strMessage As String)
        btn.Attributes.Add("onclick", "return confirm('" & strMessage & "');")
    End Sub
    Public Shared Function ReturnNumber(ByVal strInput As String) As Double
        If String.IsNullOrEmpty(strInput) Then
            Return 0
        Else
            Return CDbl(strInput)
        End If
    End Function
    Public Shared Function InsertPageVisit( _
            ByVal UserEmail As String, _
            ByVal IPAddress As String, _
            ByVal CurrentPage As String, ByVal CurrentViewState As String, ByVal OtherMessage As String) As Boolean
        'Can do this but would expect a performance hit
        ', ByVal userRequestObject As System.Web.UI.Page
        'userRequestObject.request.UserAddress


        Dim cmdText As String = "INSERT INTO tblLogging (UserEmail,IPAddress,Logdate,CurrentPage,CurrentViewState) Values " & _
                    "  (@UserEmail,@IPAddress,@Logdate,@CurrentPage,@CurrentViewState) "

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@UserEmail", UserEmail)
        cmd.Parameters.AddWithValue("@IPAddress", IPAddress)
        cmd.Parameters.AddWithValue("@Logdate", Format(Now(), "General Date"))
        cmd.Parameters.AddWithValue("@CurrentPage", CurrentPage)
        cmd.Parameters.AddWithValue("@CurrentViewState", CurrentViewState)
        cmd.Parameters.AddWithValue("@OtherMessage", OtherMessage)
        cmd.Connection = connection
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                Return True 'Rows updated
            Else
                Return False '0 rows updated.
            End If
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function
End Class
