Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports System.IO

Public Class cDataSetManipulation
    

    ''' <summary>
    ''' The data should be fully summarized by SQL Server already -- i.e., you should GROUP BY your pivot column plus any other columns you wish to return, and you should aggregate your pivot value accordingly.  The DataReader should also be sorted so that all rows which will be pivoted into one are sorted together.  
    ''' AGGREGATE AND SORT YOUR VALUES FIRST, before calling this routine or you will end up with double entries!!!!!
    ''' </summary>
    ''' <param name="dataValues">this is any open DataReader object, ready to be transformed and pivoted into a DataTable.  As mentioned, it should be fully grouped, aggregated, sorted and ready to go</param>
    ''' <param name="keyColumn">This is the column in the DataReader which serves to identify each row.  In the previous example, this would be CustomerID.  Your DataReader's recordset should be grouped and sorted by this column as well</param>
    ''' <param name="pivotNameColumn">This is the column in the DataReader that contains the values you'd like to transform from rows into columns.</param>
    ''' <param name="pivotValueColumn">This is the column that in the DataReader that contains the values to pivot into the appropriate columns.</param>
    ''' <returns>DataTable</returns>
    ''' <remarks>Code converted from C# to VB.NET from http://weblogs.sqlteam.com/jeffs/articles/5091.aspx</remarks>
    Public Shared Function PivotDataReader(ByVal dataValues As IDataReader, ByVal keyColumn As String, ByVal pivotNameColumn As String, ByVal pivotValueColumn As String) As DataTable
        Dim tmp As New DataTable()
        Dim r As DataRow
        Dim LastKey As String = "//dummy//"
        Dim i As Integer, pValIndex As Integer, pNameIndex As Integer
        Dim s As String
        Dim FirstRow As Boolean = True
        ' Add non-pivot columns to the data table: 
        pValIndex = dataValues.GetOrdinal(pivotValueColumn) 'This is 0based count of where the column is in ds.  Data for cells
        pNameIndex = dataValues.GetOrdinal(pivotNameColumn) 'This is 0based count of where the column is in ds.  Column headers
        For i = 0 To dataValues.FieldCount - 1
            If i <> pValIndex AndAlso i <> pNameIndex Then
                'Add the columns if they are not the pivotcolumnname or pivotnamecoumn
                tmp.Columns.Add(dataValues.GetName(i), dataValues.GetFieldType(i))
            End If
        Next
        r = tmp.NewRow()
        ' now, fill up the table with the data: 
        While dataValues.Read()
            ' see if we need to start a new row 
            If dataValues(keyColumn).ToString() <> LastKey Then
                ' if this isn't the very first row, we need to add the last one to the table 
                If Not FirstRow Then
                    tmp.Rows.Add(r)
                End If
                r = tmp.NewRow()
                FirstRow = False
                For i = 0 To dataValues.FieldCount - 3
                    r(i) = dataValues(tmp.Columns(i).ColumnName)
                Next
                ' Add all non-pivot column values to the new row: 
                LastKey = dataValues(keyColumn).ToString()
            End If
            ' assign the pivot values to the proper column; add new columns if needed: 
            s = dataValues(pNameIndex).ToString()
            'Temp
            If Not String.IsNullOrEmpty(s) Then
                If Not tmp.Columns.Contains(s) Then
                    Dim c As DataColumn = tmp.Columns.Add(s, dataValues.GetFieldType(pValIndex))
                    ' set the index so that it is sorted properly: 
                    Dim newOrdinal As Integer = c.Ordinal
                    For i = newOrdinal - 1 To dataValues.FieldCount - 2 Step -1
                        If c.ColumnName.CompareTo(tmp.Columns(i).ColumnName) < 0 Then
                            newOrdinal = i
                        End If
                    Next
                    c.SetOrdinal(newOrdinal)
                End If
                r(s) = dataValues(pValIndex)
            End If
        End While
        ' add that final row to the datatable: 
        'If Not FirstRow Then
        'tmp.Rows.Add(r)
        'End If
        tmp.Rows.Add(r)
        ' Close the DataReader 
        dataValues.Close()
        ' and that's it! 
        Return tmp
    End Function
    Public Shared Function AddColumn(ByVal ds As DataSet, ByVal dcName As String, ByVal dcType As String, ByVal strTableName As String) As DataSet
        Dim tempCol As New DataColumn
        tempCol.ColumnName = dcName
        tempCol.DataType = System.Type.GetType(dcType)

        If ds IsNot Nothing Then
            ds.Tables(strTableName).Columns.Add(tempCol)
        End If
        Return ds
    End Function
    Shared Sub WriteDataSetToFile(ByVal strFileName As String, ByVal dt As Data.DataTable)
        Dim x, y As Integer
        Dim tempString As String = ""
        For x = 0 To dt.Rows.Count - 1
            If x = 0 Then
                For y = 0 To dt.Columns.Count - 1
                    tempString &= dt.Columns(y).ColumnName & ","
                Next y
                My.Computer.FileSystem.WriteAllText(Path.Combine(HttpContext.Current.Server.MapPath("~/LogFiles/"), strFileName), tempString & vbNewLine, True)
                tempString = ""
            End If
            For y = 0 To dt.Columns.Count - 1
                tempString &= dt.Rows(x).Item(dt.Columns(y).ColumnName).ToString & ","
            Next y
            My.Computer.FileSystem.WriteAllText(Path.Combine(HttpContext.Current.Server.MapPath("~/LogFiles/"), strFileName), tempString & vbNewLine, True)
            tempString = ""
        Next x
    End Sub
    Public Shared Function PivotDataTable(ByVal dataValues As DataTable, ByVal keyColumn As String, ByVal pivotNameColumn As String, ByVal pivotValueColumn As String, Optional ByVal bSortColumnHeaders As Boolean = False) As DataTable
        Dim tmp As New DataTable()
        Dim r As DataRow
        Dim LastKey As String = "//dummy//"
        Dim i As Integer, pValIndex As Integer, pNameIndex As Integer
        Dim s As String
        Dim FirstRow As Boolean = True
        ' Add non-pivot columns to the data table: 
        pValIndex = dataValues.Columns(pivotValueColumn).Ordinal 'cell data
        pNameIndex = dataValues.Columns(pivotNameColumn).Ordinal 'columnheader
        For i = 0 To dataValues.Columns.Count - 1
            If i <> pValIndex AndAlso i <> pNameIndex Then
                'Add columns that are not column headers or cell data
                tmp.Columns.Add(dataValues.Columns(i).ColumnName, dataValues.Columns(i).DataType)
            End If
        Next
        r = tmp.NewRow()
        ' now, fill up the table with the data: 
        For x = 0 To dataValues.Rows.Count - 1
            ' see if we need to start a new row 
            If dataValues.Rows(x).Item(keyColumn).ToString() <> LastKey Then
                ' if this isn't the very first row, we need to add the last one to the table 
                If Not FirstRow Then
                    tmp.Rows.Add(r)
                End If
                r = tmp.NewRow()
                FirstRow = False
                'This loops goes through all tmp table columns and adds teh data.  The -3 is because
                'we have special use of 3 columns (passed in variables)
                For i = 0 To dataValues.Columns.Count - 3
                    r(i) = dataValues.Rows(x).Item(tmp.Columns(i).ColumnName)
                Next
                ' Add all non-pivot column values to the new row: 
                LastKey = dataValues.Rows(x).Item(keyColumn).ToString()
            End If
            ' assign the pivot values to the proper column; add new columns if needed: 
            s = dataValues.Rows(x).Item(pNameIndex).ToString()
            If Not tmp.Columns.Contains(s) Then
                Dim c As DataColumn = tmp.Columns.Add(s, dataValues.Columns(pValIndex).DataType)
                'This code sorts the column headers based on column header names!
                If bSortColumnHeaders Then
                    Dim newOrdinal As Integer = c.Ordinal
                    For i = newOrdinal - 1 To dataValues.Columns.Count - 2 Step -1
                        If c.ColumnName.CompareTo(tmp.Columns(i).ColumnName) < 0 Then
                            newOrdinal = i
                        End If
                    Next
                    c.SetOrdinal(newOrdinal)
                End If
            End If
            r(s) = dataValues.Rows(x).Item(pValIndex)
        Next x
        ' add that final row to the datatable: 
        'If Not FirstRow Then
        'tmp.Rows.Add(r)
        'End If
        tmp.Rows.Add(r)

        ' and that's it! 
        Return tmp
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function ExecuteStatement(ByVal strSQL As String) As Boolean

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(strSQL, connection)

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
        Catch e As SqlException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetSingleSQLValue(ByVal strSQL As String) As String
        Dim temp As String
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(strSQL, connection)

        cmd.Connection = connection
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            temp = Convert.ToString(cmd.ExecuteScalar)
        Catch e As SqlException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return temp
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetSingleDataset(ByVal strSQL As String) As DataSet
        Dim connection As New OleDbConnection(conString)
        Dim da As OleDbDataAdapter = New OleDbDataAdapter(strSQL, connection)
        Dim ds As DataSet = New DataSet()

        Try
            da.Fill(ds, "cDataSet")
        Catch e As SqlException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("cDataSet") IsNot Nothing Then
            Return ds
        Else
            Return Nothing
        End If
    End Function
    Public Shared Sub WriteEntryToLogFile(ByVal strFileName As String, ByVal strEntry As String)
        My.Computer.FileSystem.WriteAllText(Path.Combine(HttpContext.Current.Server.MapPath("~/LogFiles/"), strFileName), strEntry & vbNewLine, True)
    End Sub
End Class
