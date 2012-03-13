Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports System.IO
<DataObjectAttribute()> _
Public Class cAdmin
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetBUData() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT * from tblBusinessUnit order by ReportOrder"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tbldata")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblData") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("ID") = "-1"
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
   Public Shared Function GetBUList(ByVal strBU As String) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT * from tblBusinessUnit where BusinessUnit in ('" & Replace(strBU, ",", "','") & "') order by ReportOrder"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tbldata")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        
        Return ds

    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
Public Shared Function UpdateBUData(ByVal AvgFTEComp As Double, _
        ByVal NonSalaryEmpFactor As Double, ByVal BusinessUnit As String, ByVal ReportOrder As Double, ByVal ID As Double, ByVal Original_id As Double) As Boolean
        'Original_xxxx is passed in, but ignored because the bulk edit datagrid on forces a pass in on an update
        'This is annoying and took a while to debug, but this is the only way I can get the code to pass.
        Dim cmdText As String

        cmdText = "UPDATE tblBusinessUnit " & _
                    "  SET " & _
                    "[AvgFTEComp]=@AvgFTEComp," & _
                    "[NonSalaryEmpFactor]=@NonSalaryEmpFactor, " & _
                    "[BusinessUnit]=@BusinessUnit, " & _
                    "[ReportOrder]=@ReportOrder " & _
                    "where [ID]=@ID"
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@AvgFTEComp", AvgFTEComp)
        cmd.Parameters.AddWithValue("@NonSalaryEmpFactor", NonSalaryEmpFactor)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@ReportOrder", ReportOrder)
        cmd.Parameters.AddWithValue("@ID", ID)


        '-1 values from linked tables are nulls.  Therefore, convert them to nulls
        If cmd.Parameters("@AvgFTEComp").Value Is Nothing Then cmd.Parameters("@AvgFTEComp").Value = 0
        If cmd.Parameters("@NonSalaryEmpFactor").Value Is Nothing Then cmd.Parameters("@NonSalaryEmpFactor").Value = 0
        If cmd.Parameters("@BusinessUnit").Value Is Nothing Then cmd.Parameters("@BusinessUnit").Value = System.DBNull.Value
        If cmd.Parameters("@ReportOrder").Value Is Nothing Then cmd.Parameters("@ReportOrder").Value = 0

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
    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
Public Shared Function InsertBUData(ByVal AvgFTEComp As Double, _
    ByVal NonSalaryEmpFactor As Double, _
    ByVal BusinessUnit As String, _
    ByVal ReportOrder As Double, _
    ByVal NewItemID As Double) As Boolean
        'Original_xxxx is passed in, but ignored because the bulk edit datagrid on forces a pass in on an update
        'This is annoying and took a while to debug, but this is the only way I can get the code to pass.
        Dim cmdText As String

        cmdText = "INSERT into tblBusinessUnit " & _
                    "  ([AvgFTEComp],[NonSalaryEmpFactor],[BusinessUnit],[ReportOrder] )" & _
                    " values (@AvgFTEComp,@NonSalaryEmpFactor,@BusinessUnit,@ReportOrder) "

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@AvgFTEComp", AvgFTEComp)
        cmd.Parameters.AddWithValue("@NonSalaryEmpFactor", NonSalaryEmpFactor)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@ReportOrder", ReportOrder)



        '-1 values from linked tables are nulls.  Therefore, convert them to nulls
        If cmd.Parameters("@AvgFTEComp").Value Is Nothing Then cmd.Parameters("@AvgFTEComp").Value = 0
        If cmd.Parameters("@NonSalaryEmpFactor").Value Is Nothing Then cmd.Parameters("@NonSalaryEmpFactor").Value = 0
        If cmd.Parameters("@BusinessUnit").Value Is Nothing Then cmd.Parameters("@BusinessUnit").Value = System.DBNull.Value
        If cmd.Parameters("@ReportOrder").Value Is Nothing Then cmd.Parameters("@ReportOrder").Value = 0

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

    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
Public Shared Function DeleteBUData(ByVal ID As Double) As Boolean

        If String.IsNullOrEmpty(ID) Then _
          Throw New ArgumentException("ID cannot be null or an empty string.")

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Delete from tblBusinessUnit where [ID]=@ID", connection)
        cmd.Connection = connection
        cmd.Parameters.AddWithValue("@ID", ID)
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
