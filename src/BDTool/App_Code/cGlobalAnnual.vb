Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports System.IO
'GlobalAnnualID is key value for CRUD
'tblGlobalAnnualReporting is tablename/queryname 

<DataObjectAttribute()> _
Public Class cGlobalAnnual
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, True)> _
Public Shared Function GetAnnualItems() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
       "GlobalAnnualID, " & _
        "ReportYear, " & _
        "NonSalaryEmpFactor, " & _
        "BusinessUnit, " & _
        "LastUpdateBy, " & _
        "LastUpdateDate, " & _
        "AvgFTEComp " & _
        " FROM tblGlobalAnnualReporting order by reportyear"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblGlobalAnnualReporting")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblGlobalAnnualReporting") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("GlobalAnnualID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, True)> _
Public Shared Function GetAnnualItemsByBu(ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
        "GlobalAnnualID, " & _
        "ReportYear, " & _
        "NonSalaryEmpFactor, " & _
        "BusinessUnit, " & _
        "LastUpdateBy, " & _
        "LastUpdateDate, " & _
        "AvgFTEComp " & _
        " FROM tblGlobalAnnualReporting where BusinessUnit=" & dblBU_ID & " order by reportyear"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblGlobalAnnualReporting")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblGlobalAnnualReporting") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("GlobalAnnualID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertAnnualReport( _
        ByVal ReportYear As Double, _
ByVal NonSalaryEmpFactor As Double, _
ByVal BusinessUnit As Double, _
ByVal LastUpdateBy As String, _
ByVal LastUpdateDate As Date, _
ByVal AvgFTEComp As Double) As Double

        Dim returnID As Double
        Dim cmdText As String

        'For MS Access
        cmdText = "Insert Into tblGlobalAnnualReporting (" & _
"[ReportYear], " & _
"[NonSalaryEmpFactor], " & _
"[BusinessUnit], " & _
"[LastUpdateBy], " & _
"[LastUpdateDate], " & _
"[AvgFTEComp]) " & _
"values (" & _
"@ReportYear, " & _
"@NonSalaryEmpFactor, " & _
"@BusinessUnit, " & _
"@LastUpdateBy, " & _
"@LastUpdateDate, " & _
"@AvgFTEComp) "


        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@ReportYear", ReportYear)
        cmd.Parameters.AddWithValue("@NonSalaryEmpFactor", NonSalaryEmpFactor)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy)
        cmd.Parameters.AddWithValue("@LastUpdateDate", LastUpdateDate)
        cmd.Parameters.AddWithValue("@AvgFTEComp", AvgFTEComp)


        'This would be code for SQL Server  - This is how I get the ID back
        'Dim IndustryID As New OleDbParameter("@IndustryID", OleDbType.Double)
        'IndustryID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(IndustryID)
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
Public Shared Function UpdateAnnualReport( _
ByVal ReportYear As Double, _
ByVal NonSalaryEmpFactor As Double, _
ByVal BusinessUnit As Double, _
ByVal LastUpdateBy As String, _
ByVal LastUpdateDate As Date, _
ByVal AvgFTEComp As Double, _
ByVal GlobalAnnualID As Double) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim cmdText As String
        cmdText = "Update tblGlobalAnnualReporting Set " & _
       "[ReportYear]=@ReportYear," & _
"[NonSalaryEmpFactor]=@NonSalaryEmpFactor," & _
"[BusinessUnit]=@BusinessUnit," & _
"[LastUpdateBy]=@LastUpdateBy," & _
"[LastUpdateDate]=@LastUpdateDate," & _
"[AvgFTEComp]=@AvgFTEComp " & _
        "Where [GlobalAnnualID]=@GlobalAnnualID"


        If NonSalaryEmpFactor >= 1 Then
            NonSalaryEmpFactor /= 100
        End If
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)
        cmd.Parameters.AddWithValue("@ReportYear", ReportYear)
        cmd.Parameters.AddWithValue("@NonSalaryEmpFactor", NonSalaryEmpFactor)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy)
        cmd.Parameters.AddWithValue("@LastUpdateDate", CStr(DateTime.Now))
        cmd.Parameters.AddWithValue("@AvgFTEComp", AvgFTEComp)
        cmd.Parameters.AddWithValue("@GlobalAnnualID", GlobalAnnualID)


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
Public Shared Function DeleteAnnualReport(ByVal GlobalAnnualID As Double) As Boolean
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        Dim sqlCommand As String = "Delete from tblGlobalAnnualReporting Where [GlobalAnnualID]=" & GlobalAnnualID

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
