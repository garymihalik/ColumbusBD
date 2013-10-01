Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports System.IO
<DataObjectAttribute()> _
Public Class cManageHolidays
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
#Region "Get All Items in Group"
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllHolidays() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Holiday, " & _
            "HolidayDescription " & _
          " From tblHolidays " & _
          " Order By Holiday Asc"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblHolidays")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblHolidays") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("Holiday") = CDate(#1/1/1900#)
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
#End Region
#Region "Update Single Item in Group"
    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
Public Shared Function UpdateSingleHoliday(ByVal Holiday As Date, _
        ByVal HolidayDescription As String) As Boolean
        'Original_xxxx is passed in, but ignored because the bulk edit datagrid on forces a pass in on an update
        'This is annoying and took a while to debug, but this is the only way I can get the code to pass.
        Dim cmdText As String

        cmdText = "UPDATE tblHolidays " & _
                    "  SET " & _
                    "[Holiday]=@Holiday," & _
                    "[HolidayDescription]=@HolidayDescription " & _
                    " WHERE [Holiday]=@Holiday"
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Holiday", Holiday)
        cmd.Parameters.AddWithValue("@HolidayDescription", HolidayDescription)


        '-1 values from linked tables are nulls.  Therefore, convert them to nulls
        If cmd.Parameters("@HolidayDescription").Value = "" Then cmd.Parameters("@HolidayDescription").Value = System.DBNull.Value

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
#End Region
#Region "Insert Single Item"
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertSingleHoliday( _
ByVal Holiday As Date, _
ByVal HolidayDescription As String) As Date

        Dim cmdText As String
        'This would be code for SQL Server
        'cmdText = "Insert Into tblHolidays " & _
        '"([Holiday],[IndustryID],[AccountGroupID],[Anchor]) " & _
        '"Values (@Holiday,@IndustryID,@AccountGroupID,@Anchor) SET @HolidayID = SCOPE_IDENTITY()"

        'But for MS Access I have to do it this way
        cmdText = "Insert Into tblHolidays " & _
        "([Holiday],[HolidayDescription]) " & _
        "Values (@Holiday,@HolidayDescription)"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Holiday", Holiday)
        cmd.Parameters.AddWithValue("@HolidayDescription", HolidayDescription)


        ''This would be code for SQL Server This is how I get the ID back
        'Dim HolidayID As New OleDbParameter("@HolidayID", OleDbType.Double)
        'HolidayID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(HolidayID)

        '************************************************
        'Ensure empty strings are stored as nulls
        If cmd.Parameters("@HolidayDescription").Value Is Nothing Then cmd.Parameters("@HolidayDescription").Value = System.DBNull.Value
        '************************************************


        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                Return Holiday 'Rows updated
            Else
                Return Holiday '0 rows updated.
            End If
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return Holiday
    End Function
#End Region
    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
Public Shared Function DeleteSingleHoliday(ByVal Holiday As Date) As Boolean

        If String.IsNullOrEmpty(Holiday) Then _
          Throw New ArgumentException("Holiday cannot be null or an empty string.")

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Delete from tblHolidays where [Holiday]=@Holiday", connection)
        cmd.Connection = connection
        cmd.Parameters.AddWithValue("@Holiday", Holiday)
        Try
            connection.Open()
            cmd.ExecuteNonQuery()
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function
End Class
