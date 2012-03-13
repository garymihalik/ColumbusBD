Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.ComponentModel
Imports System.IO
<DataObjectAttribute()> _
Public Class cErrorManagement
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, True)> _
 Public Shared Function GetListOfErrors(ByVal strModuleName As String, ByVal sortColumns As String) As DataSet
        Dim connection As New OleDbConnection(conString)
        Dim strSQL As String
        If String.IsNullOrEmpty(strModuleName) Then
            strSQL = "Select * FROM tblErrors"
        Else
            strSQL = "Select * FROM tblErrors Where ModuleArea='" & strModuleName & "'"
        End If


        If String.IsNullOrEmpty(sortColumns) Then
            strSQL &= " ORDER BY ID"
        Else
            If sortColumns.Trim() = "" Then
                strSQL &= " ORDER BY ID"
            Else
                strSQL &= " ORDER BY " & sortColumns
            End If
        End If


        Dim da As OleDbDataAdapter = New OleDbDataAdapter(strSQL, connection)
        Dim ds As DataSet = New DataSet()
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            da.Fill(ds, "ErrorList")

        Catch e As SqlException
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
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertErrorMessage( _
ByVal ERRORMESSAGE As String, _
ByVal MODULEAREA As String) As Integer

        Dim returnID As Integer
        Dim cmdText As String
        cmdText = "Insert Into tblErrors " & _
  "([MODULEAREA], [ErrorText], [ErrorDateTime]) " & _
  "Values (@MODULEAREA, @ERRORMESSAGE, @ErrorDateTime) "
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@MODULEAREA", MODULEAREA)
        cmd.Parameters.AddWithValue("@ERRORMESSAGE", ERRORMESSAGE)
        cmd.Parameters.AddWithValue("@ErrorDateTime", Format(Now, "general date"))
        


        If cmd.Parameters("@ERRORMESSAGE").Value Is Nothing Then cmd.Parameters("@ERRORMESSAGE").Value = "No Error Text Available"

        cmd.Connection = connection
        If connection.State <> ConnectionState.Open Then
            connection.Open()
        End If
        Try
            If cmd.ExecuteNonQuery() <> 0 Then
                cmd.CommandText = "Select @@Identity"
                returnID = cmd.ExecuteScalar()
                Return returnID 'ID of newly created row
            Else
                Return -1 '0 rows updated.
            End If
        Catch e As SqlException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function
End Class
