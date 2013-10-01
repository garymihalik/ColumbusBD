Imports System
Imports System.Web
Imports System.Diagnostics
Imports System.Data
Imports System.Data.OleDb
Imports System.Net.Mail
Imports System.IO
Namespace ExceptionHandler
    'From http://www.eggheadcafe.com/articles/20030816.asp
    Public Class LogException
        Public Sub New()
            'ctor
        End Sub
        Public Sub HandleException(ByVal ex As Exception)
            Dim ctx As HttpContext = HttpContext.Current
            Dim strData As String = ""
            Dim evtId As Integer = 0
            Dim logIt As Boolean = Convert.ToBoolean(ConfigurationManager.AppSettings("logErrors"))
            If logIt Then
                Dim dbConnString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))

                Dim referer As String = [String].Empty
                If ctx.Request.ServerVariables("HTTP_REFERER") IsNot Nothing Then
                    referer = ctx.Request.ServerVariables("HTTP_REFERER").ToString()
                End If
                Dim sForm As String = IIf((ctx.Request.Form IsNot Nothing), ctx.Request.Form.ToString(), [String].Empty)

                Dim sQuery As String = IIf((ctx.Request.QueryString IsNot Nothing), ctx.Request.QueryString.ToString(), [String].Empty)
                'strData = "" + Chr(10) + "SOURCE: " + ex.Source + "" + Chr(10) + "MESSAGE: " + ex.Message + "" + Chr(10) + "FORM: " + sForm + "" + Chr(10) + "QUERYSTRING: " + sQuery + "" + Chr(10) + "TARGETSITE: " + ex.TargetSite + "" + Chr(10) + "STACKTRACE: " + ex.StackTrace + "" + Chr(10) + "REFERER: " + referer
                strData = "" & Chr(10) & "SOURCE: " & ex.Source.ToString & "" & Chr(10) & "MESSAGE: " & ex.Message.ToString & "" & Chr(10) & "FORM: " & sForm.ToString & "" & Chr(10) & "QUERYSTRING: " & sQuery.ToString & "" & Chr(10) & "TARGETSITE: " & ex.TargetSite.ToString & "" & Chr(10) & "STACKTRACE: " & ex.StackTrace.ToString & "" & Chr(10) & "REFERER: " & referer.ToString



                If dbConnString.Length > 0 Then
                    Dim cmd As New oledbCommand()
                    cmd.CommandText = "INSERT INTO LOGITEMS " & _
                        "(Source, LogDateTime,Message,Form,QueryString,TargetSite,StackTrace,Referer) " & _
                        " Values ( @Source,@LogDateTime,@Message,@Form,@QueryString,@TargetSite,@StackTrace,@Referer)"
                    Dim cn As New OleDbConnection(dbConnString)
                    cmd.Connection = cn
                    cn.Open()
                    Try
                        cmd.Parameters.Add(New OleDbParameter("@Source", ex.Source))
                        cmd.Parameters.Add(New OleDbParameter("@LogDateTime", Now.ToString))
                        cmd.Parameters.Add(New OleDbParameter("@Message", ex.Message))
                        cmd.Parameters.Add(New OleDbParameter("@Form", sForm))
                        cmd.Parameters.Add(New OleDbParameter("@QueryString", sQuery))
                        cmd.Parameters.Add(New OleDbParameter("@TargetSite", ex.TargetSite.ToString()))
                        cmd.Parameters.Add(New OleDbParameter("@StackTrace", ex.StackTrace.ToString()))
                        cmd.Parameters.Add(New OleDbParameter("@Referer", referer))
                        
                        If cmd.ExecuteNonQuery() <> 0 Then
                            cmd.CommandText = "Select @@Identity"
                            evtId = cmd.ExecuteScalar()
                        End If
                        cmd.Dispose()
                        cn.Close()
                    Catch exc As Exception
                        EventLog.WriteEntry(ex.Source, "Database Error From Exception Log!", EventLogEntryType.Error, 65535)
                    Finally
                        cmd.Dispose()
                        cn.Close()
                    End Try
                    Try
                        'No need to write to the administrative tools-->event view at this time
                        'EventLog.WriteEntry(ex.Source, Left(strData, 32000), EventLogEntryType.[Error], evtId)
                    Catch exl As Exception

                        Throw
                    End Try
                End If
            End If

            Dim strEmails As String = ConfigurationManager.AppSettings("emailAddresses").ToString()
            If strEmails.Length > 0 Then
                Dim emails As String() = strEmails.Split(Convert.ToChar("|"))
                Dim fromEmail As String = ConfigurationManager.AppSettings("fromEmail").ToString()
                Dim emailTo As String = ""

                For i As Integer = 0 To emails.Length - 1
                    emailTo &= emails(i) & ","
                Next


                Dim msg As New MailMessage(fromEmail, emailTo)

                msg.Subject = "Web application error!"
                Dim detailURL As String = ConfigurationManager.AppSettings("detailURL").ToString()
                msg.Body = strData + detailURL + "?EvtId=" + evtId.ToString()

                Dim client As New SmtpClient()
                client.Host = ConfigurationManager.AppSettings("smtpServer").ToString()
                Try
                    'disable until we have email host
                    If CBool(ConfigurationManager.AppSettings("SendExceptionEmails").ToString()) Then
                        client.Send(msg)
                    End If
                Catch excm As Exception
                    Throw
                End Try
            Else

                Return
            End If
        End Sub
    End Class
    Public Class GetExceptions
        Public Shared connection As New OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection")))
        Public Shared Function GetListOfErrors(ByVal strModuleName As String, ByVal sortColumns As String) As DataSet
            Dim sqlCommand As String
            If String.IsNullOrEmpty(strModuleName) Then
                sqlCommand = "Select * FROM LogItems"
            Else
                sqlCommand = "Select * FROM LogItems Where Referer='" & strModuleName & "'"
            End If

            If String.IsNullOrEmpty(sortColumns) Then
                sqlCommand &= " ORDER BY EventId Desc"
            Else
                If sortColumns.Trim() = "" Then
                    sqlCommand &= " ORDER BY EventId Desc"
                Else
                    sqlCommand &= " ORDER BY " & sortColumns
                End If
            End If


            Dim da As OleDbDataAdapter = New OleDbDataAdapter(sqlCommand, connection)
            Dim ds As DataSet = New DataSet()
            Try
                If connection.State <> ConnectionState.Open Then
                    connection.Open()
                End If
                da.Fill(ds, "ErrorList")

            Catch e As OleDbException
                Throw New ArgumentException(e.ToString)
            Finally
                connection.Close()
            End Try

            If ds.Tables("ErrorList") IsNot Nothing Then
                Return ds
            Else
                Return Nothing
            End If
        End Function
        Public Shared Function GetLastError() As DataSet
            Dim sqlCommand As String
            sqlCommand = "Select * FROM LogItems WHere EventId=(Select Max(EventId) from LogItems)"


            Dim da As OleDbDataAdapter = New OleDbDataAdapter(sqlCommand, connection)
            Dim ds As DataSet = New DataSet()
            Try
                If connection.State <> ConnectionState.Open Then
                    connection.Open()
                End If
                da.Fill(ds, "ErrorList")

            Catch e As OleDbException
                Throw New ArgumentException(e.ToString)
            Finally
                connection.Close()
            End Try

            If ds.Tables("ErrorList") IsNot Nothing Then
                Return ds
            Else
                Return Nothing
            End If
        End Function

    End Class
End Namespace
