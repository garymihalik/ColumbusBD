Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports ProfileCommon
Imports cDataSetManipulation
Imports System.IO
<DataObjectAttribute()> _
Public Class cOpportunity
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetOpportunities(Optional ByVal strFilter As String = "", Optional ByVal bExcludeExclusions As Boolean = False) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "WeightedRevenue, " & _
            "Client, " & _
            "PK_OpportunityID, " & _
            "OpportunityOwner, " & _
            "tblOpportunity.ClientID as ClientID, " & _
            "OpportunityName, " & _
            "Extension, " & _
            "Fit, " & _
            "Anchor, " & _
            "Source, " & _
            "[Date Entered], " & _
            "Skills, " & _
            "PK_OpportunityUpdateID, " & _
            "UpdateDate, " & _
            "OpportunityCloseDate, " & _
            "WinPercentage, " & _
            "NextSteps, " & _
            "EstimatedRevenue, " & _
            "FK_OpportunityID, " & _
            "UpdateNotes, " & _
            "AccountGroup, " & _
            "AccountOwner, " & _
            "ForecastOwner, " & _
            "ReportOn, " & _
            "ReportOrder, " & _
            "Inactive, " & _
            "RolesNeeded, " & _
            "RFPRequired, " & _
            "RFPLead, " & _
            "RFPRiskAssessment, " & _
            "LastUpdateBy, " & _
            "LastUpdateDate, " & _
            "UpdatePerson " & _
            " From qryAllOpportunities where Inactive=False "
        If bExcludeExclusions Then
            sqlCommand &= " and Extension=false "
        End If
        Select Case strFilter
            Case "Un-qualified"
                sqlCommand &= " and (PK_OpportunityUpdateID is Null or WinPercentage is null) "
            Case "All"
                'as is
            Case "All - Won"
                sqlCommand &= " and winpercentage=1 "
            Case "All - Lost"
                sqlCommand &= " and winpercentage=0 "
            Case "Working Pipeline"
                sqlCommand &= " and winpercentage>0 and winpercentage<1 "
            Case "In Last 30 Days"
                sqlCommand &= " and datediff('d',[date entered],now)<=30 "
            Case "Needing Updates"
                sqlCommand &= " and [OpportunityCloseDate]<=#" & DateAdd(DateInterval.Day, 7, Now()) & "# and winpercentage>0 and winpercentage<1 "
            Case "WinLoss"
                sqlCommand &= " and (winpercentage>0 and winpercentage<1) or " & _
                "(winpercentage=0 and datediff('d',[UpdateDate],now)<=7) or " & _
                "(winpercentage=1 and datediff('d',[UpdateDate],now)<=7)"
            Case Else
                If Not String.IsNullOrEmpty(strFilter) Then
                    sqlCommand &= " and " & strFilter
                End If
        End Select
        sqlCommand &= " Order By Client, OpportunityName"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblOpportunities") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_OpportunityID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetOpportunitiesByBU(ByVal dblBU_ID As Double, Optional ByVal strFilter As String = "", Optional ByVal bExcludeExclusions As Boolean = False) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim p As ProfileCommon = DirectCast(HttpContext.Current.Profile, ProfileCommon)
        Dim u As System.Web.Security.RolePrincipal = DirectCast(HttpContext.Current.User, System.Web.Security.RolePrincipal)

        Dim sqlCommand As String = "SELECT " & _
            "WeightedRevenue, " & _
            "Client, " & _
            "PK_OpportunityID, " & _
            "OpportunityOwner, " & _
            "tblOpportunity.ClientID as ClientID, " & _
            "OpportunityName, " & _
            "Extension, " & _
            "Fit, " & _
            "Anchor, " & _
            "Source, " & _
            "[Date Entered], " & _
            "Skills, " & _
            "PK_OpportunityUpdateID, " & _
            "UpdateDate, " & _
            "OpportunityCloseDate, " & _
            "WinPercentage, " & _
            "NextSteps, " & _
            "EstimatedRevenue, " & _
            "FK_OpportunityID, " & _
            "UpdateNotes, " & _
            "AccountGroup, " & _
            "AccountOwner, " & _
            "ForecastOwner, " & _
            "ReportOn, " & _
            "ReportOrder, " & _
            "Inactive, " & _
            "BusinessUnit, " & _
            "RolesNeeded, " & _
            "RFPRequired, " & _
            "RFPLead, " & _
            "RFPRiskAssessment, " & _
            "LastUpdateBy, " & _
            "LastUpdateDate, " & _
            "UpdatePerson " & _
            " From qryAllOpportunities where Inactive=False and BusinessUnit=" & dblBU_ID & " "
        If bExcludeExclusions Then
            sqlCommand &= " and Extension=false "
        End If
        Select Case strFilter
            Case "Un-qualified"
                sqlCommand &= " and (PK_OpportunityUpdateID is Null or WinPercentage is null) "
            Case "All"
                'as is
            Case "All - Won"
                sqlCommand &= " and winpercentage=1 "
            Case "All - Lost"
                sqlCommand &= " and winpercentage=0 "
            Case "Working Pipeline"
                sqlCommand &= " and winpercentage>0 and winpercentage<1 "
            Case "In Last 30 Days"
                sqlCommand &= " and datediff('d',[date entered],now)<=30 "
            Case "Needing Updates"
                sqlCommand &= " and [OpportunityCloseDate]<=#" & DateAdd(DateInterval.Day, 7, Now()) & "# and winpercentage>0 and winpercentage<1 "
            Case "WinLoss"
                sqlCommand &= " and ((winpercentage>0 and winpercentage<1) or " & _
                "(winpercentage=0 and datediff('d',[UpdateDate],now)<=7) or " & _
                "(winpercentage=1 and datediff('d',[UpdateDate],now)<=7))"
            Case Else
                If Not String.IsNullOrEmpty(strFilter) Then
                    sqlCommand &= " and " & strFilter
                End If
        End Select
        sqlCommand &= " Order By Client, OpportunityName"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblOpportunities") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_OpportunityID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetWonOpportunitiesByAccountGroup(ByVal AccountGroupID As Double, Optional ByVal Filter As String = "") As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "WeightedRevenue, " & _
            "Client, " & _
            "PK_OpportunityID, " & _
            "OpportunityOwner, " & _
            "tblOpportunity.ClientID as ClientID, " & _
            "OpportunityName, " & _
            "Extension, " & _
            "Fit, " & _
            "Anchor, " & _
            "Source, " & _
            "[Date Entered], " & _
            "Skills, " & _
            "PK_OpportunityUpdateID, " & _
            "UpdateDate, " & _
            "OpportunityCloseDate, " & _
            "WinPercentage, " & _
            "NextSteps, " & _
            "EstimatedRevenue, " & _
            "FK_OpportunityID, " & _
            "UpdateNotes, " & _
            "AccountGroup, " & _
            "AccountOwner, " & _
            "ForecastOwner, " & _
            "ReportOn, " & _
            "ReportOrder, " & _
            "RolesNeeded, " & _
            "RFPRequired, " & _
            "RFPLead, " & _
            "RFPRiskAssessment, " & _
            "LastUpdateBy, " & _
            "LastUpdateDate, " & _
            "UpdatePerson " & _
            " From qryAllOpportunities "
        sqlCommand &= " where AccountGroupID=" & AccountGroupID
        If Not String.IsNullOrEmpty(Filter) Then
            Select Case Filter
                Case "Won12months"
                    sqlCommand &= " and winpercentage=1 and UpdateDate>=#" & DateAdd("m", -12, Now) & "#"
                Case "Won6months"
                    sqlCommand &= " and winpercentage=1 and UpdateDate>=#" & DateAdd("m", -6, Now) & "#"
                Case "Won3months"
                    sqlCommand &= " and winpercentage=1 and UpdateDate>=#" & DateAdd("m", -3, Now) & "#"
                Case "Won0Resources"
                    sqlCommand &= " and winpercentage=1 and (select count(1) from tblAssignments where FK_OpportunityID=PK_OpportunityID)=0"
                Case "70_12months"
                    sqlCommand &= " and winpercentage>=.7 and UpdateDate>=#" & DateAdd("m", -12, Now) & "#"
                Case "70_6months"
                    sqlCommand &= " and winpercentage>=.7 and UpdateDate>=#" & DateAdd("m", -6, Now) & "#"
                Case "70_3months"
                    sqlCommand &= " and winpercentage>=.7 and UpdateDate>=#" & DateAdd("m", -3, Now) & "#"
                Case "70_0Resources"
                    sqlCommand &= " and winpercentage>=.7 and (select count(1) from tblAssignments where FK_OpportunityID=PK_OpportunityID)=0"
                Case "AllWon"
                    sqlCommand &= " and winpercentage=1 "
                Case "AllAssignments"
                    sqlCommand &= " and (select count(1) from tblAssignments where FK_OpportunityID=PK_OpportunityID)>0"
                Case "10_12months"
                    sqlCommand &= " and winpercentage>=.1 and UpdateDate>=#" & DateAdd("m", -12, Now) & "#"
                Case "10_6months"
                    sqlCommand &= " and winpercentage>=.1 and UpdateDate>=#" & DateAdd("m", -6, Now) & "#"
                Case "10_3months"
                    sqlCommand &= " and winpercentage>=.1 and UpdateDate>=#" & DateAdd("m", -3, Now) & "#"
                Case "10_0Resources"
                    sqlCommand &= " and winpercentage>=.1 and (select count(1) from tblAssignments where FK_OpportunityID=PK_OpportunityID)=0"
                Case "Active"
                    sqlCommand &= " and (select count(1) from tblAssignments where FK_OpportunityID=PK_OpportunityID having max(enddate)>#" & Now & "#)>0"
                Case "All"
                    sqlCommand &= ""
                Case Else
                    sqlCommand &= ""
            End Select
        End If
        sqlCommand &= " Order By Client, UpdateDate"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblOpportunities") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_OpportunityID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function


    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetSingleOpportunities(ByVal PK_OpportunityID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "WeightedRevenue, " & _
            "Client, " & _
            "PK_OpportunityID, " & _
            "OpportunityOwner, " & _
            "tblOpportunity.ClientID as ClientID, " & _
            "OpportunityName, " & _
            "Extension, " & _
            "Fit, " & _
            "Anchor, " & _
            "Source, " & _
            "[Date Entered], " & _
            "Skills, " & _
            "PK_OpportunityUpdateID, " & _
            "UpdateDate, " & _
            "OpportunityCloseDate, " & _
            "WinPercentage, " & _
            "NextSteps, " & _
            "EstimatedRevenue, " & _
            "FK_OpportunityID, " & _
            "UpdateNotes, " & _
            "RolesNeeded, " & _
            "RFPRequired, " & _
            "RFPLead, " & _
            "RFPRiskAssessment, " & _
            "LastUpdateBy, " & _
            "LastUpdateDate, " & _
            "UpdatePerson " & _
            " From qryAllOpportunities " & _
            " where PK_OpportunityID=" & PK_OpportunityID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblOpportunity")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblOpportunity") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_OpportunityID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function

    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetOpportunityUpdates(ByVal FK_OpportunityID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "PK_OpportunityUpdateID, " & _
            "UpdateDate, " & _
            "OpportunityCloseDate, " & _
            "WinPercentage, " & _
            "NextSteps, " & _
            "EstimatedRevenue, " & _
            "FK_OpportunityID, " & _
            "UpdateNotes, " & _
            "UpdatePerson " & _
            " From tblOpportunityUpdates " & _
            " where FK_OpportunityID=" & FK_OpportunityID & " order by UpdateDate Desc"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblOpportunityUpdates")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblOpportunityUpdates") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("PK_OpportunityUpdateID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertSingleOpportunityUpdate( _
    ByVal UpdateDate As Date, _
    ByVal OpportunityCloseDate As Date, _
    ByVal WinPercentage As String, _
    ByVal NextSteps As String, _
    ByVal EstimatedRevenue As Double, _
    ByVal UpdateNotes As String, _
    ByVal UpdatePerson As String, _
    ByVal FK_OpportunityID As Double, _
    ByVal NewItemID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String
        'This would be code for SQL Server
        'cmdText = "Insert Into tblAccountGroup " & _
        '"([UpdateDate],[OpportunityCloseDate],[WinPercentage],[NextSteps],[EstimatedRevenue],[UpdateNotes],[UpdatePerson]) " & _
        '"Values (@UpdateDate,@OpportunityCloseDate,@WinPercentage,@NextSteps,@EstimatedRevenue,@UpdateNotes,@UpdatePerson) SET @PK_OpportunityUpdateID = SCOPE_IDENTITY()"

        'But for MS Access I have to do it this way
        If WinPercentage = "-1" Or String.IsNullOrEmpty(WinPercentage) Then
            cmdText = "Insert Into tblOpportunityUpdates " & _
        "([UpdateDate],[OpportunityCloseDate],[NextSteps],[EstimatedRevenue],[UpdateNotes],[UpdatePerson],[FK_OpportunityID]) " & _
        " Values (@UpdateDate,@OpportunityCloseDate,@NextSteps,@EstimatedRevenue,@UpdateNotes,@UpdatePerson,@FK_OpportunityID) "
        Else
            cmdText = "Insert Into tblOpportunityUpdates " & _
        "([UpdateDate],[OpportunityCloseDate],[WinPercentage],[NextSteps],[EstimatedRevenue],[UpdateNotes],[UpdatePerson],[FK_OpportunityID]) " & _
        " Values (@UpdateDate,@OpportunityCloseDate,@WinPercentage,@NextSteps,@EstimatedRevenue,@UpdateNotes,@UpdatePerson,@FK_OpportunityID) "
        End If
        'cmdText = "Insert Into tblOpportunityUpdates " & _
        '"([UpdateDate],[OpportunityCloseDate],[WinPercentage],[NextSteps],[EstimatedRevenue],[UpdateNotes],[UpdatePerson],[FK_OpportunityID]) " & _
        '" Values (@UpdateDate,@OpportunityCloseDate,@WinPercentage,@NextSteps,@EstimatedRevenue,@UpdateNotes,@UpdatePerson,@FK_OpportunityID) "


        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@UpdateDate", CStr(UpdateDate))
        cmd.Parameters.AddWithValue("@OpportunityCloseDate", CStr(String.Format("{0:d}", OpportunityCloseDate)))
        If WinPercentage = "-1" Or String.IsNullOrEmpty(WinPercentage) Then
            'Don't add the parameter!
        Else
            cmd.Parameters.AddWithValue("@WinPercentage", WinPercentage)
        End If
        cmd.Parameters.AddWithValue("@NextSteps", NextSteps)
        cmd.Parameters.AddWithValue("@EstimatedRevenue", EstimatedRevenue)
        cmd.Parameters.AddWithValue("@UpdateNotes", UpdateNotes)
        cmd.Parameters.AddWithValue("@UpdatePerson", UpdatePerson)
        cmd.Parameters.AddWithValue("@FK_OpportunityID", FK_OpportunityID)

        'This is how I get the ID back for SQL Server
        'Dim PK_OpportunityUpdateID As New OleDbParameter("@PK_OpportunityUpdateID", OleDbType.Double)
        'PK_OpportunityUpdateID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(PK_OpportunityUpdateID)

        '************************************************
        'Ensure empty strings are stored as nulls
        If cmd.Parameters("@UpdateDate").Value Is Nothing Then cmd.Parameters("@UpdateDate").Value = String.Format("{0:d}", Date.Now)
        If cmd.Parameters("@OpportunityCloseDate").Value Is Nothing Then cmd.Parameters("@OpportunityCloseDate").Value = String.Format("{0:d}", Date.Now)
        If cmd.Parameters("@NextSteps").Value Is Nothing Then cmd.Parameters("@NextSteps").Value = "None"
        If cmd.Parameters("@EstimatedRevenue").Value Is Nothing Then cmd.Parameters("@EstimatedRevenue").Value = 0
        If cmd.Parameters("@UpdateNotes").Value Is Nothing Then cmd.Parameters("@UpdateNotes").Value = "None"
        If cmd.Parameters("@UpdatePerson").Value Is Nothing Then cmd.Parameters("@UpdatePerson").Value = "System"
        '************************************************


        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                'This is for SQL Server
                'returnID = CInt(PK_OpportunityUpdateID.Value)

                'This is for MS Access
                cmd.CommandText = "Select @@Identity"
                returnID = cmd.ExecuteScalar()
                'Update for NYQ opportunities, which are denoted by nulls or -1
                If WinPercentage = "-1" Or String.IsNullOrEmpty(WinPercentage) Then ExecuteStatement("Update tblOpportunityUpdates Set Winpercentage = null where PK_OpportunityUpdateID=" & returnID)
                'If WinPercentage=0, then remove all assignments!
                If Not String.IsNullOrEmpty(WinPercentage) Then
                    If WinPercentage = "0" Or WinPercentage = 0 Then
                        cAssignments.DeleteAssignmentsOnOpportunity(FK_OpportunityID)
                    End If
                End If
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
    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
    Public Shared Function UpdateSingleOpportunity(ByVal PK_OpportunityID As Double, _
        ByVal Original_PK_OpportunityID As Double, _
        ByVal OpportunityOwner As String, _
        ByVal ClientID As Double, _
        ByVal OpportunityName As String, _
        ByVal Extension As Boolean, _
        ByVal Fit As String, _
        ByVal Anchor As Boolean, _
        ByVal Source As String, _
        ByVal Skills As String, _
        ByVal LastUpdateBy As String, _
        ByVal RolesNeeded As String, _
        ByVal RFPRequired As Boolean, _
        ByVal RFPLead As String, _
        ByVal RFPRiskAssessment As String) As Boolean

        Dim cmdText As String

        cmdText = "UPDATE tblOpportunity " & _
                    "  SET " & _
                    "[OpportunityOwner]=@OpportunityOwner," & _
                    "[ClientID]=@ClientID," & _
                    "[OpportunityName]=@OpportunityName," & _
                    "[Extension]=@Extension," & _
                    "[Fit]=@Fit," & _
                    "[Anchor]=@Anchor," & _
                    "[Source]=@Source," & _
                    "[Skills]=@Skills, " & _
                    "[LastUpdateBy]=@LastUpdateBy," & _
                    "[LastUpdateDate]=@LastUpdateDate," & _
                    "[RolesNeeded]=@RolesNeeded, " & _
                    "[RFPRequired]=@RFPRequired," & _
                    "[RFPLead]=@RFPLead," & _
                    "[RFPRiskAssessment]=@RFPRiskAssessment " & _
                    " WHERE [PK_OpportunityID]=" & PK_OpportunityID
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        'cmd.Parameters.AddWithValue("@PK_OpportunityID", PK_OpportunityID)
        cmd.Parameters.AddWithValue("@OpportunityOwner", OpportunityOwner)
        cmd.Parameters.AddWithValue("@ClientID", ClientID)
        cmd.Parameters.AddWithValue("@OpportunityName", OpportunityName)
        cmd.Parameters.AddWithValue("@Extension", Extension)
        cmd.Parameters.AddWithValue("@Fit", Fit)
        cmd.Parameters.AddWithValue("@Anchor", Anchor)
        cmd.Parameters.AddWithValue("@Source", Source)
        'cmd.Parameters.AddWithValue("@DateEntered", String.Format("{0:d}", Date.Now))
        cmd.Parameters.AddWithValue("@Skills", Skills)
        cmd.Parameters.AddWithValue("@LastUpdateBy", LastUpdateBy)
        cmd.Parameters.AddWithValue("@LastUpdateDate", String.Format("{0:d}", Date.Now))
        cmd.Parameters.AddWithValue("@RolesNeeded", RolesNeeded)
        cmd.Parameters.AddWithValue("@RFPRequired", RFPRequired)
        cmd.Parameters.AddWithValue("@RFPLead", RFPLead)
        cmd.Parameters.AddWithValue("@RFPRiskAssessment", RFPRiskAssessment)

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
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
Public Shared Function InsertSingleOpportunity( _
    ByVal OpportunityOwner As String, _
    ByVal ClientID As Double, _
    ByVal OpportunityName As String, _
    ByVal Extension As Boolean, _
    ByVal Fit As String, _
    ByVal Anchor As Boolean, _
    ByVal Source As String, _
    ByVal [DateEntered] As Date, _
    ByVal Skills As String, _
    ByVal UpdatePerson As String, _
    ByVal OpportunityCloseDate As Date, _
    ByVal WinPercentage As String, _
    ByVal EstimatedRevenue As Double, _
    ByVal NextSteps As String, _
    ByVal RolesNeeded As String, _
    ByVal RFPRequired As Boolean, _
    ByVal RFPLead As String, _
    ByVal RFPRiskAssessment As String, _
    ByVal NewItemID As Double) As Double

        If String.IsNullOrEmpty(UpdatePerson) Then UpdatePerson = "Not Given"
        If String.IsNullOrEmpty(WinPercentage) Then
            WinPercentage = "-1"
        Else
            If CDbl(WinPercentage) > 1 Then
                WinPercentage /= 100
            End If
        End If



        Dim returnID As Double
        Dim cmdText As String
        'This would be code for SQL Server
        'cmdText = "Insert Into tblAccountGroup " & _
        '"([OpportunityOwner],[ClientID],[OpportunityName],[Extension],[Fit],[Anchor],[Source],[Date Entered],[Skills]) " & _
        '" Values (@OpportunityOwner,@ClientID,@OpportunityName,@Extension,@Fit,@Anchor,@Source,@DateEntered,@Skills) SET @PK_OpportunityUpdateID = SCOPE_IDENTITY()"

        'But for MS Access I have to do it this way
        cmdText = "Insert Into tblOpportunity " & _
        "([OpportunityOwner],[ClientID],[OpportunityName],[Extension],[Fit],[Anchor],[Source],[Date Entered],[Skills],[LastUpdateBy],[LastUpdateDate],[RolesNeeded], [RFPRequired], [RFPLead],[RFPRiskAssessment]) " & _
        " Values (@OpportunityOwner,@ClientID,@OpportunityName,@Extension,@Fit,@Anchor,@Source,@DateEntered,@Skills,@LastUpdateBy,@LastUpdateDate, @RolesNeeded, @RFPRequired, @RFPLead, @RFPRiskAssessment) "

        'Test to see if client id is null or -1.  If so, do not save!
        If ClientID = -1 Then
            Dim intTest As Integer = cErrorManagement.InsertErrorMessage("Owner:" & OpportunityOwner & "|ClientID:" & ClientID & "|OppName:" & OpportunityName & "|Ext?:" & Extension & "|Fit:" & Fit & "|Anchor?:" & Anchor & "|Source:" & Source & "|DateEntered:" & [DateEntered] & "|Skills:" & Skills & "|UpdatePerson:" & UpdatePerson & "|CloseDate:" & OpportunityCloseDate & "|Win%:" & WinPercentage & "|EstRevenue:" & EstimatedRevenue & "|NextSteps:" & NextSteps, "cOpportunity.InsertSingleOpportunity")
            Return False
        End If

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@OpportunityOwner", OpportunityOwner)
        cmd.Parameters.AddWithValue("@ClientID", ClientID)
        cmd.Parameters.AddWithValue("@OpportunityName", OpportunityName)
        cmd.Parameters.AddWithValue("@Extension", Extension)
        cmd.Parameters.AddWithValue("@Fit", Fit)
        cmd.Parameters.AddWithValue("@Anchor", Anchor)
        cmd.Parameters.AddWithValue("@Source", Source)
        cmd.Parameters.AddWithValue("@Date Entered", CStr(DateEntered))
        cmd.Parameters.AddWithValue("@Skills", Skills)
        cmd.Parameters.AddWithValue("@LastUpdateBy", UpdatePerson)
        cmd.Parameters.AddWithValue("@LastUpdateDate", String.Format("{0:d}", Date.Now))
        cmd.Parameters.AddWithValue("@RolesNeeded", RolesNeeded)
        cmd.Parameters.AddWithValue("@RFPRequired", RFPRequired)
        cmd.Parameters.AddWithValue("@RFPLead", RFPLead)
        cmd.Parameters.AddWithValue("@RFPRiskAssessment", RFPRiskAssessment)


        'This is how I get the ID back for SQL Server
        'Dim PK_OpportunityID As New OleDbParameter("@PK_OpportunityID", OleDbType.Double)
        'PK_OpportunityID.Direction = ParameterDirection.Output
        'cmd.Parameters.Add(PK_OpportunityID)

        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                'This is for SQL Server
                'returnID = CInt(PK_OpportunityID.Value)

                'This is for MS Access
                cmd.CommandText = "Select @@Identity"
                returnID = cmd.ExecuteScalar()
                If String.IsNullOrEmpty(NextSteps) Then
                    NextSteps = "Update Opportunity"
                End If
                InsertSingleOpportunityUpdate(Now, OpportunityCloseDate, WinPercentage, NextSteps, EstimatedRevenue, "New Opportunity", UpdatePerson, returnID, -1)
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

    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
   Public Shared Function MarkOpportunityActive(ByVal PK_OpportunityID As Double, _
      ByVal Inactive As Boolean) As Boolean

        Dim cmdText As String

        cmdText = "UPDATE tblOpportunity " & _
                    "  SET " & _
                    "[inactive]=@inactive" & _
                    " WHERE [PK_OpportunityID]=" & PK_OpportunityID
        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'apparently, the parameters need to be in order in which they are used in the SQL statement
        'cmd.Parameters.AddWithValue("@PK_OpportunityID", PK_OpportunityID)
        cmd.Parameters.AddWithValue("@inactive", Inactive)

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
End Class
