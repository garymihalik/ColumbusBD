Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports System.IO
'ActualsID is key value for CRUD
'tblActuals is tablename/queryname 

<DataObjectAttribute()> _
Public Class cActuals
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, True)> _
Public Shared Function GetClientActuals(ByVal ClientID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
        "ActualsID, " & _
        "Client, " & _
        "ReportMonth, " & _
        "ActualRevenue, " & _
        "ActualCost, " & _
        "ClientID,LastUpdateBy,LastUpdateDate " & _
        " FROM tblActuals where ClientID=" & ClientID & " order by reportmonth"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblActuals")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblActuals") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("ActualsID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertSingleClientActual( _
        ByVal Client As String, _
        ByVal ReportMonth As Double, _
        ByVal ActualRevenue As String, _
        ByVal ActualCost As String, _
        ByVal ClientID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String

        Dim p As ProfileCommon = DirectCast(HttpContext.Current.Profile, ProfileCommon)
        Dim u As System.Web.Security.RolePrincipal = DirectCast(HttpContext.Current.User, System.Web.Security.RolePrincipal)

        'For MS Access
        cmdText = "Insert Into tblActuals (" & _
        "[Client], " & _
        "[ReportMonth], " & _
        "[ActualRevenue], " & _
        "[ActualCost], " & _
        "[LastUpdateBy], " & _
        "[LastUpdateDate], " & _
        "[ClientID]) " & _
        " values   (" & _
        "@Client, " & _
        "@ReportMonth, " & _
        "@ActualRevenue, " & _
        "@ActualCost, " & _
        "@LastUpdateBy, " & _
        "@LastUpdateDate, " & _
        "@ClientID) "

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Client", Client)
        cmd.Parameters.AddWithValue("@ReportMonth", ReportMonth)
        cmd.Parameters.AddWithValue("@ActualRevenue", ActualRevenue)
        cmd.Parameters.AddWithValue("@ActualCost", ActualCost)
        cmd.Parameters.AddWithValue("@LastUpdateBy", p.FriendlyName)
        cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now.ToString)
        cmd.Parameters.AddWithValue("@ClientID", ClientID)

        If cmd.Parameters("@ActualRevenue").Value Is Nothing Then cmd.Parameters("@ActualRevenue").Value = System.DBNull.Value
        If cmd.Parameters("@ActualCost").Value Is Nothing Then cmd.Parameters("@ActualCost").Value = System.DBNull.Value
        If cmd.Parameters("@LastUpdateBy").Value Is Nothing Then cmd.Parameters("@LastUpdateBy").Value = System.DBNull.Value


        'This would be code for SQL Server  - This is how I get the ID back
        'Dim IndustryID As New OleDbParameter("@IndustryID", OleDbType.Double)
        'IndustryID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(IndustryID)

        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                'This would be code for SQL Server
                'returnID = CInt(IndustryID.Value)

                'This is for MS Access
                cmd.CommandText = "Select @@Identity"
                returnID = cmd.ExecuteScalar()
                Return returnID 'Rows updated
            Else
                Return -1 '0 rows updated.
            End If
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Update, False)> _
Public Shared Function UpdateSingleClientActual( _
ByVal ActualsID As Double, _
ByVal Client As String, _
ByVal ReportMonth As Double, _
ByVal ActualRevenue As String, _
ByVal ActualCost As String, _
ByVal ClientID As Double) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim p As ProfileCommon = DirectCast(HttpContext.Current.Profile, ProfileCommon)
        Dim u As System.Web.Security.RolePrincipal = DirectCast(HttpContext.Current.User, System.Web.Security.RolePrincipal)

        Dim cmdText As String
        cmdText = "Update tblActuals Set " & _
        "[Client]=@Client," & _
        "[ReportMonth]=@ReportMonth," & _
        "[ActualRevenue]=@ActualRevenue," & _
        "[ActualCost]=@ActualCost," & _
        "[ClientID]=@ClientID, " & _
        "[LastUpdateBy]=@LastUpdateBy, " & _
        "[LastUpdateDate]=@LastUpdateDate " & _
        "Where [ActualsID]=@ActualsID"



        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)
        cmd.Parameters.AddWithValue("@Client", Client)
        cmd.Parameters.AddWithValue("@ReportMonth", ReportMonth)
        cmd.Parameters.AddWithValue("@ActualRevenue", ActualRevenue)
        cmd.Parameters.AddWithValue("@ActualCost", ActualCost)
        cmd.Parameters.AddWithValue("@ClientID", ClientID)
        cmd.Parameters.AddWithValue("@LastUpdateBy", p.FriendlyName)
        cmd.Parameters.AddWithValue("@LastUpdateDate", DateTime.Now.ToString)
        cmd.Parameters.AddWithValue("@ActualsID", ActualsID)

        If cmd.Parameters("@ActualRevenue").Value Is Nothing Then cmd.Parameters("@ActualRevenue").Value = System.DBNull.Value
        If cmd.Parameters("@ActualCost").Value Is Nothing Then cmd.Parameters("@ActualCost").Value = System.DBNull.Value
        If cmd.Parameters("@LastUpdateBy").Value Is Nothing Then cmd.Parameters("@LastUpdateBy").Value = System.DBNull.Value

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
            If connection.State <> ConnectionState.Closed Then
                connection.Close()
            End If
        End Try
        Return True
    End Function


    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
Public Shared Function DeleteSingleClientActual(ByVal ActualsID As Double) As Boolean
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        Dim sqlCommand As String = "Delete from tblActuals Where [ActualsID]=" & ActualsID
        Dim cmd As OleDbCommand = New OleDbCommand(sqlCommand, connection)

        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            'cmd.ExecuteScalar()
            If cmd.ExecuteNonQuery() <> 0 Then
                Return True 'Rows updated
            Else
                Return False '0 rows updated.
            End If
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            If connection.State <> ConnectionState.Closed Then
                connection.Close()
            End If
        End Try
        Return True
    End Function


End Class
