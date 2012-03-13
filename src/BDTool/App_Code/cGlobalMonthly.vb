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
'tblGlobalMonthReporting is tablename/queryname 

<DataObjectAttribute()> _
Public Class cGlobalMonthly
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, True)> _
Public Shared Function GetMonthlyItems() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
       "ReportMonth, " & _
        "Expenses, " & _
        "TransfersInOut, " & _
        "MiscCOGS, " & _
        "ForecastedFTECount, " & _
        "GlobalMonthlyID, ActualProfit, TransferNotes, LastUpdateBy, LastUpdateDate " & _
        " FROM tblGlobalMonthReporting order by reportmonth"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblGlobalMonthReporting")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblGlobalMonthReporting") IsNot Nothing Then
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
    <DataObjectMethodAttribute(DataObjectMethodType.Select, True)> _
Public Shared Function GetMonthlyItemsByBu(ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
       "ReportMonth, " & _
        "Expenses, " & _
        "TransfersInOut, " & _
        "MiscCOGS, " & _
        "ForecastedFTECount, " & _
        "GlobalMonthlyID, ActualProfit, TransferNotes, LastUpdateBy, LastUpdateDate " & _
        " FROM tblGlobalMonthReporting where BusinessUnit=" & dblBU_ID & " order by reportmonth"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblGlobalMonthReporting")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblGlobalMonthReporting") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("GlobalMonthlyID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertMonthlyReport( _
        ByVal ReportMonth As Double, _
ByVal Expenses As Double, _
ByVal TransfersInOut As Double, _
ByVal MiscCOGS As Double, _
ByVal BusinessUnit As Double, _
ByVal ForecastedFTECount As Double, _
ByVal ActualProfit As Double, _
ByVal TransferNotes As String, _
ByVal LastUpdateBy As String, _
ByVal LastUpdateDate As Date) As Double

        Dim returnID As Double
        Dim cmdText As String

        'For MS Access
        cmdText = "Insert Into tblGlobalMonthReporting (" & _
"[ReportMonth], " & _
"[Expenses], " & _
"[TransfersInOut], " & _
"[MiscCOGS], " & _
"[BusinessUnit], " & _
"[ForecastedFTECount],  " & _
"[ActualProfit],  " & _
"[TransferNotes],  " & _
"[LastUpdateBy],  " & _
"[LastUpdateDate]) " & _
"values (@ReportMonth, " & _
"@Expenses, " & _
"@TransfersInOut, " & _
"@MiscCOGS, " & _
"@BusinessUnit, " & _
"@ForecastedFTECount, " & _
"@ActualProfit, " & _
"@TransferNotes, " & _
"@LastUpdateBy, " & _
"@LastUpdateDate) "

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@ReportMonth", ReportMonth)
        cmd.Parameters.AddWithValue("@Expenses", Expenses)
        cmd.Parameters.AddWithValue("@TransfersInOut", TransfersInOut)
        cmd.Parameters.AddWithValue("@MiscCOGS", MiscCOGS)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@ForecastedFTECount", ForecastedFTECount)
        cmd.Parameters.AddWithValue("@ActualProfit", ActualProfit)
        cmd.Parameters.AddWithValue("@TransferNotes", TransferNotes)
        cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy)
        cmd.Parameters.AddWithValue("@LastUpdateDate", CStr(DateTime.Now))

        'This would be code for SQL Server  - This is how I get the ID back
        'Dim IndustryID As New OleDbParameter("@IndustryID", OleDbType.Double)
        'IndustryID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(IndustryID)
        If cmd.Parameters("@ActualProfit").Value Is Nothing Then cmd.Parameters("@ActualProfit").Value = System.DBNull.Value
        If cmd.Parameters("@TransferNotes").Value Is Nothing Then cmd.Parameters("@TransferNotes").Value = System.DBNull.Value
        If cmd.Parameters("@LastUpdateBy").Value Is Nothing Then cmd.Parameters("@LastUpdateBy").Value = System.DBNull.Value
        If cmd.Parameters("@LastUpdateDate").Value Is Nothing Then cmd.Parameters("@LastUpdateDate").Value = DateTime.Now.ToString

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
Public Shared Function UpdateMonthlyReport( _
ByVal ReportMonth As Double, _
ByVal Expenses As Double, _
ByVal TransfersInOut As Double, _
ByVal MiscCOGS As Double, _
ByVal BusinessUnit As Double, _
ByVal ForecastedFTECount As Double, _
ByVal ActualProfit As Double, _
ByVal TransferNotes As String, _
ByVal LastUpdateBy As String, _
ByVal LastUpdateDate As Date, _
ByVal GlobalMonthlyID As Double) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim cmdText As String
        cmdText = "Update tblGlobalMonthReporting Set " & _
        " [ReportMonth]=@ReportMonth," & _
        " [Expenses]=@Expenses," & _
        " [TransfersInOut]=@TransfersInOut," & _
        " [MiscCOGS]=@MiscCOGS," & _
        " [BusinessUnit]=@BusinessUnit," & _
        " [ForecastedFTECount]=@ForecastedFTECount,  " & _
        "[ActualProfit]=@ActualProfit, " & _
        "[TransferNotes]=@TransferNotes, " & _
        "[LastUpdateBy]=@LastUpdateBy, " & _
        "[LastUpdateDate]=@LastUpdateDate " & _
        "Where [GlobalMonthlyID]=@GlobalMonthlyID"



        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)
        cmd.Parameters.AddWithValue("@ReportMonth", ReportMonth)
        cmd.Parameters.AddWithValue("@Expenses", Expenses)
        cmd.Parameters.AddWithValue("@TransfersInOut", TransfersInOut)
        cmd.Parameters.AddWithValue("@MiscCOGS", MiscCOGS)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@ForecastedFTECount", ForecastedFTECount)
        cmd.Parameters.AddWithValue("@ActualProfit", ActualProfit)
        cmd.Parameters.AddWithValue("@TransferNotes", TransferNotes)
        cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy)
        cmd.Parameters.AddWithValue("@LastUpdateDate", CStr(DateTime.Now))
        cmd.Parameters.AddWithValue("@GlobalMonthlyID", GlobalMonthlyID)
        If cmd.Parameters("@TransferNotes").Value = "" Or cmd.Parameters("@TransferNotes").Value Is Nothing Then cmd.Parameters("@TransferNotes").Value = System.DBNull.Value


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
Public Shared Function DeleteMonthlyReport(ByVal GlobalMonthlyID As Double) As Boolean
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        Dim sqlCommand As String = "Delete from tblGlobalMonthReporting Where [GlobalMonthlyID]=" & GlobalMonthlyID

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
