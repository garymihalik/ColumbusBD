Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports System.IO
<DataObjectAttribute()> _
Public Class cUsers
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetPeople() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "PK_PersonID, " & _
            "LastName, " & _
            "FirstName, " & _
            "Office, " & _
            "Cell, " & _
            "Home, " & _
            "Email, " & _
            "inactive, " & _
            "Employee " & _
            " From tblResources Order By FirstName,LastName "

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblResources")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblResources") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_PersonID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetPeopleByBU(ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "PK_PersonID, " & _
            "LastName, " & _
            "FirstName, " & _
            "Office, " & _
            "Cell, " & _
            "Home, " & _
            "Email, " & _
            "inactive, " & _
            "Employee " & _
            " From tblResources where BusinessUnit=" & dblBU_ID & " Order By LastName,FirstName "

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblResources")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblResources") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_PersonID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function UpdateResource(ByVal PK_PersonID As Double, _
ByVal LastName As String, _
ByVal FirstName As String, _
ByVal Office As String, _
ByVal Cell As String, _
ByVal Home As String, _
ByVal Email As String, _
ByVal Employee As Boolean, _
ByVal BusinessUnit As Double, _
ByVal Inactive As Boolean) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "Update tblResources Set " & _
            "[LastName]=@LastName," & _
            "[FirstName]=@FirstName," & _
            "[Office]=@Office," & _
            "[Cell]=@Cell," & _
            "[Home]=@Home," & _
            "[Employee]=@Employee," & _
            "[Inactive]=@Inactive," & _
            "[Email]=@Email," & _
            "[BusinessUnit]=@BusinessUnit" & _
            " Where [PK_PersonID]=" & PK_PersonID



        Dim cmd As OleDbCommand = New OleDbCommand(sqlCommand, connection)
        'cmd.Parameters.AddWithValue("@PK_PersonID", PK_PersonID)
        cmd.Parameters.AddWithValue("@LastName", LastName)
        cmd.Parameters.AddWithValue("@FirstName", FirstName)
        cmd.Parameters.AddWithValue("@Office", Office)
        cmd.Parameters.AddWithValue("@Cell", Cell)
        cmd.Parameters.AddWithValue("@Home", Home)
        cmd.Parameters.AddWithValue("@Employee", Employee)
        cmd.Parameters.AddWithValue("@Inactive", Inactive)
        cmd.Parameters.AddWithValue("@Email", Email)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)

        If cmd.Parameters("@LastName").Value = "" Then cmd.Parameters("@LastName").Value = System.DBNull.Value
        If cmd.Parameters("@FirstName").Value = "" Then cmd.Parameters("@FirstName").Value = System.DBNull.Value
        If cmd.Parameters("@Office").Value = "" Then cmd.Parameters("@Office").Value = System.DBNull.Value
        If cmd.Parameters("@Cell").Value = "" Then cmd.Parameters("@Cell").Value = System.DBNull.Value
        If cmd.Parameters("@Home").Value = "" Then cmd.Parameters("@Home").Value = System.DBNull.Value
        If cmd.Parameters("@Email").Value = "" Then cmd.Parameters("@Email").Value = System.DBNull.Value



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
  Public Shared Function DeleteResource(ByVal PK_PersonID As Double) As Boolean

        If String.IsNullOrEmpty(PK_PersonID) Then _
          Throw New ArgumentException("PK_PersonID cannot be null or an empty string.")

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Delete from tblResources where [PK_PersonID]=@PK_PersonID", connection)
        cmd.Connection = connection
        cmd.Parameters.AddWithValue("@PK_PersonID", PK_PersonID)
        Try
            connection.Open()
            ExecuteStatement("Delete from tblReportData where PersonID=" & PK_PersonID)
            ExecuteStatement("Delete from tblAssignments where FK_PersonID=" & PK_PersonID)
            cmd.ExecuteNonQuery()
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertResource( _
ByVal LastName As String, _
ByVal FirstName As String, _
ByVal Office As String, _
ByVal Cell As String, _
ByVal Home As String, _
ByVal Email As String, _
ByVal Employee As Boolean, _
ByVal Inactive As Boolean, _
ByVal BusinessUnit As Double, _
ByVal NewItemID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String
        'This would be code for SQL Server
        'cmdText = "Insert Into tblClients " & _
        '"([Client],[IndustryID],[AccountGroupID],[Anchor]) " & _
        '"Values (@Client,@IndustryID,@AccountGroupID,@Anchor) SET @ClientID = SCOPE_IDENTITY()"

        'But for MS Access I have to do it this way
        cmdText = "Insert Into tblResources  (" & _
                    "[LastName], " & _
                    "[FirstName], " & _
                    "[Office], " & _
                    "[Cell], " & _
                    "[Home], " & _
                    "[Email], " & _
                    "[Employee], " & _
                    "[Inactive],[BusinessUnit]) " & _
                    " values   (" & _
                    "@LastName, " & _
                    "@FirstName, " & _
                    "@Office, " & _
                    "@Cell, " & _
                    "@Home, " & _
                    "@Email, " & _
                    "@Employee, " & _
                    "@Inactive, @BusinessUnit)"
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@LastName", LastName)
        cmd.Parameters.AddWithValue("@FirstName", FirstName)
        cmd.Parameters.AddWithValue("@Office", Office)
        cmd.Parameters.AddWithValue("@Cell", Cell)
        cmd.Parameters.AddWithValue("@Home", Home)
        cmd.Parameters.AddWithValue("@Email", Email)
        cmd.Parameters.AddWithValue("@Employee", Employee)
        cmd.Parameters.AddWithValue("@Inactive", Inactive)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)

        If cmd.Parameters("@LastName").Value = "" Then cmd.Parameters("@LastName").Value = System.DBNull.Value
        If cmd.Parameters("@FirstName").Value = "" Then cmd.Parameters("@FirstName").Value = System.DBNull.Value
        If cmd.Parameters("@Office").Value = "" Then cmd.Parameters("@Office").Value = System.DBNull.Value
        If cmd.Parameters("@Cell").Value = "" Then cmd.Parameters("@Cell").Value = System.DBNull.Value
        If cmd.Parameters("@Home").Value = "" Then cmd.Parameters("@Home").Value = System.DBNull.Value
        If cmd.Parameters("@Email").Value = "" Then cmd.Parameters("@Email").Value = System.DBNull.Value


        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                'This is for MS Access
                cmd.CommandText = "Select @@Identity"
                returnID = cmd.ExecuteScalar()

                'This would be code for SQL Server
                'returnID = CInt(ClientID.Value)
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
End Class
