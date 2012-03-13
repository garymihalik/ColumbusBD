Imports System
Imports System.Web
Imports System.Diagnostics
Imports System.Data
Imports System.Data.SqlClient
Imports System.Net.Mail
Imports System.Data.OleDb
Imports System.IO
Public Class cJoeUserTroubleshooting
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    Public Shared Function GetListOfASPMEMBERSHIP() As DataSet
        Dim sqlCommand As String
        sqlCommand = "Select * FROM aspnet_membership"
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            da.Fill(ds, "Users")

        Catch e As SqlException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("Users") IsNot Nothing Then
            Return ds
        Else
            Return Nothing
        End If
    End Function
    Public Shared Function GetListOfASPMEMBERSHIPROLES() As DataSet
        Dim sqlCommand As String
        sqlCommand = "Select * FROM aspnet_Roles where rolename not in ('Administrators','Friends')"
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            da.Fill(ds, "Roles")

        Catch e As SqlException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("Roles") IsNot Nothing Then
            Return ds
        Else
            Return Nothing
        End If
    End Function

End Class

