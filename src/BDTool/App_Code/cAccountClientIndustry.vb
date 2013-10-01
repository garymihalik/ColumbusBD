Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports System.IO
<DataObjectAttribute()> _
Public Class cAccountClientIndustry
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
#Region "Get All Items in Group, Regardless of BU"
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllAccountGroups() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "AccountGroup, " & _
            "AccountOwner, " & _
            "ForecastOwner, " & _
            "ReportOn, " & _
            "ReportOrder, " & _
            "AccountGroupID " & _
          " From tblAccountGroup " & _
          " Order By ReportOrder"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblAccountGroup")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblAccountGroup") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("AccountGroupID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllIndustry() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Industry, " & _
            "IndustryID " & _
          " From tblIndustry "

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblIndustry")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblIndustry") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("IndustryID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllClients() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Client, " & _
            "IndustryID, " & _
            "ClientID, " & _
            "AccountGroupID, " & _
            "Anchor " & _
          " From tblClients Where Inactive=false Order By Client "

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblClients")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblClients") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("ClientID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
#End Region
#Region "Get All Items in Group by BU"
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllAccountGroupsByBU(ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "AccountGroup, " & _
            "AccountOwner, " & _
            "ForecastOwner, " & _
            "ReportOn, " & _
            "ReportOrder, " & _
            "AccountGroupID " & _
          " From tblAccountGroup Where BusinessUnit=" & dblBU_ID & _
          " Order By ReportOrder"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblAccountGroup")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblAccountGroup") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("AccountGroupID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllIndustryByBU(ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Industry, " & _
            "IndustryID " & _
          " From tblIndustry Where BusinessUnit=" & dblBU_ID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblIndustry")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblIndustry") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("IndustryID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetAllClientsByBU(ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Client, " & _
            "IndustryID, " & _
            "ClientID, " & _
            "AccountGroupID, " & _
            "Anchor " & _
          " From tblClients Where Inactive=false and BusinessUnit=" & dblBU_ID & " Order By Client "

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblClients")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblClients") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("ClientID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
#End Region
#Region "Get Single Item in a Group, Regardless of BU (But is only a single thing, like a single client; which already has a BU"
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetSingleAccountGroup(ByVal dblID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "AccountGroup, " & _
            "AccountOwner, " & _
            "ForecastOwner, " & _
            "ReportOn, " & _
            "ReportOrder, " & _
            "AccountGroupID " & _
            " From tblAccountGroup " & _
            " Where AccountGroupID=" & dblID


        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblAccountGroup")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblAccountGroup") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("AccountGroupID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetSingleIndustry(ByVal dblID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Industry, " & _
            "IndustryID " & _
          " From tblIndustry " & _
         " Where IndustryID=" & dblID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblIndustry")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblIndustry") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("IndustryID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetSingleClient(ByVal dblID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "Client, " & _
            "IndustryID, " & _
            "ClientID, " & _
            "AccountGroupID, " & _
            "Anchor " & _
            " From tblClients " & _
            " Where ClientID=" & dblID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblClients")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblClients") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("ClientID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
#End Region
#Region "Update Single Item in Group"
    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
    Public Shared Function UpdateSingleAccountGroup(ByVal ORIGINAL_ACCOUNTGROUPID As Double, _
        ByVal AccountGroup As String, _
        ByVal AccountOwner As String, _
        ByVal ForecastOwner As String, _
        ByVal ReportOn As Boolean, ByVal BusinessUnit As Integer, _
        ByVal ReportOrder As Double, ByVal ACCOUNTGROUPID As Double) As Boolean
        'Original_xxxx is passed in, but ignored because the bulk edit datagrid on forces a pass in on an update
        'This is annoying and took a while to debug, but this is the only way I can get the code to pass.

        Dim cmdText As String

        cmdText = "UPDATE tblAccountGroup " & _
                    "  SET " & _
                    "[AccountGroup]=@AccountGroup," & _
                    "[AccountOwner]=@AccountOwner," & _
                    "[ForecastOwner]=@ForecastOwner," & _
                    "[ReportOn]=@ReportOn," & _
                    "[BusinessUnit]=@BusinessUnit," & _
                    "[ReportOrder]=@ReportOrder" & _
                    " WHERE [AccountGroupID]=@ACCOUNTGROUPID"
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@AccountGroup", AccountGroup)
        cmd.Parameters.AddWithValue("@AccountOwner", AccountOwner)
        cmd.Parameters.AddWithValue("@ForecastOwner", ForecastOwner)
        cmd.Parameters.AddWithValue("@ReportOn", ReportOn)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@ReportOrder", ReportOrder)
        cmd.Parameters.AddWithValue("@ACCOUNTGROUPID", ACCOUNTGROUPID)

        If cmd.Parameters("@BusinessUnit").Value Is Nothing Then cmd.Parameters("@BusinessUnit").Value = -1

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
Public Shared Function UpdateSingleIndustry(ByVal ORIGINAL_IndustryID As Double, _
    ByVal Industry As String, ByVal IndustryID As Double) As Boolean
        'Original_xxxx is passed in, but ignored because the bulk edit datagrid on forces a pass in on an update
        'This is annoying and took a while to debug, but this is the only way I can get the code to pass.

        Dim cmdText As String

        cmdText = "UPDATE tblIndustry " & _
                    "  SET " & _
                    "[Industry]=@Industry" & _
                    " WHERE [IndustryID]=@IndustryID"
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Industry", Industry)
        cmd.Parameters.AddWithValue("@IndustryID", IndustryID)
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
Public Shared Function UpdateSingleClient(ByVal ORIGINAL_CLIENTID As Double, _
        ByVal Client As String, _
        ByVal IndustryID As Double, _
        ByVal AccountGroupID As Double, _
        ByVal Anchor As Boolean, ByVal BusinessUnit As Double, ByVal CLIENTID As Double) As Boolean
        'Original_xxxx is passed in, but ignored because the bulk edit datagrid on forces a pass in on an update
        'This is annoying and took a while to debug, but this is the only way I can get the code to pass.
        Dim cmdText As String

        cmdText = "UPDATE tblClients " & _
                    "  SET " & _
                    "[Client]=@Client," & _
                    "[IndustryID]=@IndustryID," & _
                    "[AccountGroupID]=@AccountGroupID," & _
                    "[BusinessUnit]=@BusinessUnit," & _
                    "[Anchor]=@Anchor" & _
                    " WHERE [ClientID]=@CLIENTID"
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Client", Client)
        cmd.Parameters.AddWithValue("@IndustryID", IndustryID)
        cmd.Parameters.AddWithValue("@AccountGroupID", AccountGroupID)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@Anchor", Anchor)
        cmd.Parameters.AddWithValue("@CLIENTID", CLIENTID)


        '-1 values from linked tables are nulls.  Therefore, convert them to nulls
        If cmd.Parameters("@IndustryID").Value = -1 Then cmd.Parameters("@IndustryID").Value = System.DBNull.Value
        If cmd.Parameters("@AccountGroupID").Value = -1 Then cmd.Parameters("@AccountGroupID").Value = System.DBNull.Value
        If cmd.Parameters("@BusinessUnit").Value Is Nothing Then cmd.Parameters("@BusinessUnit").Value = -1

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
Public Shared Function InsertSingleAccountGroup( _
        ByVal AccountGroup As String, _
        ByVal AccountOwner As String, _
        ByVal ForecastOwner As String, _
        ByVal ReportOn As Boolean, _
        ByVal ReportOrder As Double, ByVal BusinessUnit As Integer, _
        ByVal NewItemID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String
        'This would be code for SQL Server
        'cmdText = "Insert Into tblAccountGroup " & _
        '"([AccountGroup],[AccountOwner],[ForecastOwner],[ReportOn],[ReportOrder]) " & _
        '"Values (@AccountGroup,@AccountOwner,@ForecastOwner,@ReportOn,@ReportOrder) SET @AccountGroupID = SCOPE_IDENTITY()"

        'But for MS Access I have to do it this way
        cmdText = "Insert Into tblAccountGroup " & _
        "([AccountGroup],[AccountOwner],[ForecastOwner],[ReportOn],[BusinessUnit], [ReportOrder]) " & _
        "Values (@AccountGroup,@AccountOwner,@ForecastOwner,@ReportOn,@BusinessUnit,@ReportOrder) "


        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@AccountGroup", AccountGroup)
        cmd.Parameters.AddWithValue("@AccountOwner", AccountOwner)
        cmd.Parameters.AddWithValue("@ForecastOwner", ForecastOwner)
        cmd.Parameters.AddWithValue("@ReportOn", ReportOn)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@ReportOrder", ReportOrder)

        'This is how I get the ID back for SQL Server
        'Dim ACCOUNTGROUPID As New OleDbParameter("@AccountGroupID", OleDbType.Double)
        'ACCOUNTGROUPID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(ACCOUNTGROUPID)

        '************************************************
        'Ensure empty strings are stored as nulls
        If cmd.Parameters("@AccountOwner").Value Is Nothing Then cmd.Parameters("@AccountOwner").Value = System.DBNull.Value
        If cmd.Parameters("@ForecastOwner").Value Is Nothing Then cmd.Parameters("@ForecastOwner").Value = System.DBNull.Value
        If cmd.Parameters("@BusinessUnit").Value Is Nothing Then cmd.Parameters("@BusinessUnit").Value = -1
        '************************************************


        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                'This is for SQL Server
                'returnID = CInt(ACCOUNTGROUPID.Value)

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
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertSingleClient( _
ByVal Client As String, _
ByVal IndustryID As String, _
ByVal AccountGroupID As String, _
ByVal Anchor As Boolean, ByVal BusinessUnit As Double, _
ByVal NewItemID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String
        'This would be code for SQL Server
        'cmdText = "Insert Into tblClients " & _
        '"([Client],[IndustryID],[AccountGroupID],[Anchor]) " & _
        '"Values (@Client,@IndustryID,@AccountGroupID,@Anchor) SET @ClientID = SCOPE_IDENTITY()"

        'But for MS Access I have to do it this way
        cmdText = "Insert Into tblClients " & _
        "([Client],[IndustryID],[AccountGroupID],[BusinessUnit],[Anchor]) " & _
        "Values (@Client,@IndustryID,@AccountGroupID,@BusinessUnit,@Anchor)"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Client", Client)
        cmd.Parameters.AddWithValue("@IndustryID", IndustryID)
        cmd.Parameters.AddWithValue("@AccountGroupID", AccountGroupID)
        cmd.Parameters.AddWithValue("@BusinessUnit", BusinessUnit)
        cmd.Parameters.AddWithValue("@Anchor", Anchor)


        ''This would be code for SQL Server This is how I get the ID back
        'Dim ClientID As New OleDbParameter("@ClientID", OleDbType.Double)
        'ClientID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(ClientID)

        '************************************************
        'Ensure empty strings are stored as nulls
        If cmd.Parameters("@IndustryID").Value Is Nothing Then cmd.Parameters("@IndustryID").Value = -1
        If cmd.Parameters("@AccountGroupID").Value Is Nothing Then cmd.Parameters("@AccountGroupID").Value = -1
        If cmd.Parameters("@BusinessUnit").Value Is Nothing Then cmd.Parameters("@BusinessUnit").Value = -1
        '************************************************


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
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertSingleIndustry( _
ByVal Industry As String, _
ByVal NewItemID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String
        'This is the code for SQL Server
        'cmdText = "Insert Into tblIndustry " & _
        '"([Industry]) " & _
        '"Values (@Industry) SET @IndustryID = SCOPE_IDENTITY()"

        'For MS Access
        cmdText = "Insert Into tblIndustry " & _
           "([Industry]) " & _
           "Values (@Industry)"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@Industry", Industry)


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
#End Region
    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
   Public Shared Function DeleteSingleAccountGroup(ByVal ACCOUNTGROUPID As Integer) As Boolean

        If String.IsNullOrEmpty(ACCOUNTGROUPID) Then _
          Throw New ArgumentException("ACCOUNTGROUPID cannot be null or an empty string.")

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Delete from tblAccountGroup where [ACCOUNTGROUPID]=@ACCOUNTGROUPID", connection)
        cmd.Connection = connection
        cmd.Parameters.AddWithValue("@ACCOUNTGROUPID", ACCOUNTGROUPID)
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
    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
Public Shared Function DeleteSingleClient(ByVal CLIENTID As Integer) As Boolean

        If String.IsNullOrEmpty(CLIENTID) Then _
          Throw New ArgumentException("CLIENTID cannot be null or an empty string.")

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Delete from tblclients where [CLIENTID]=@CLIENTID", connection)
        cmd.Connection = connection
        cmd.Parameters.AddWithValue("@CLIENTID", CLIENTID)
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
    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
Public Shared Function DeleteSingleIndustry(ByVal IndustryID As Integer) As Boolean

        If String.IsNullOrEmpty(IndustryID) Then _
          Throw New ArgumentException("IndustryID cannot be null or an empty string.")

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Delete from tblIndustry where [IndustryID]=@IndustryID", connection)
        cmd.Connection = connection
        cmd.Parameters.AddWithValue("@IndustryID", IndustryID)
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
