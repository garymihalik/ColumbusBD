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
Public Class cAssignments
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetAssignments(ByVal OpportunityID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "AssignmentID, " & _
            "FK_OpportunityID, " & _
            "FK_PersonID, " & _
            "StartDate, " & _
            "EndDate, " & _
            "BillRate, " & _
            "Costs, " & _
            "CalculatedRevenue, ExcludeFromBURevenueCalculation, " & _
            " FirstName & ' ' & LastName as PersonName, " & _
            "PeriodUtilizationRate " & _
            " FROM tblAssignments LEFT JOIN tblResources ON tblResources.PK_PersonID = tblAssignments.FK_PersonID where FK_OpportunityID=" & OpportunityID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblAssignments")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblAssignments") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("AssignmentID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetAssignmentsWithNames(ByVal OpportunityID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "AssignmentID, " & _
            "FK_OpportunityID, " & _
            "FK_PersonID, " & _
            "StartDate, " & _
            "EndDate, " & _
            "BillRate, " & _
            "FirstName, LastName, " & _
            "Costs, " & _
            "CalculatedRevenue, " & _
            " FirstName & ' ' & LastName as PersonName, " & _
            "PeriodUtilizationRate,ExcludeFromBURevenueCalculation " & _
            " FROM tblAssignments LEFT JOIN tblResources ON tblResources.PK_PersonID = tblAssignments.FK_PersonID where FK_OpportunityID=" & OpportunityID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblAssignments")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblAssignments") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("AssignmentID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetSingleAssignmentWithNames(ByVal AssignmentID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "AssignmentID, " & _
            "FK_OpportunityID, " & _
            "FK_PersonID, " & _
            "StartDate, " & _
            "EndDate, " & _
            "BillRate, " & _
            "FirstName, LastName, " & _
            "Costs, " & _
            "CalculatedRevenue, " & _
            " FirstName & ' ' & LastName as PersonName, " & _
            "PeriodUtilizationRate, ExcludeFromBURevenueCalculation " & _
            " FROM tblAssignments LEFT JOIN tblResources ON tblResources.PK_PersonID = tblAssignments.FK_PersonID where AssignmentID=" & AssignmentID

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblAssignments")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblAssignments") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("AssignmentID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
    Public Shared Function GetSingleAssignmentReportData(ByVal AssignmentID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT * " & _
            " FROM tblReportData where AssignmentID=" & AssignmentID & " order by MonthlyReportDate"

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "tblReportData")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblReportData") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("EntryID") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
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
            " From tblResources Where Inactive=False Order By FirstName,LastName "

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
            " From tblResources Where Inactive=False and BusinessUnit=" & dblBU_ID & " Order By FirstName,LastName "

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
    Public Shared Function GetClientAssignmentsForMonth(ByVal BUID As Double, ByVal AccountGroupID As Double, ByVal ReportMonth As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim strSQL As String = " SELECT tblAccountGroup.AccountGroup,  " & _
" tblResources.FirstName, EntryID, " & _
" tblResources.LastName,  " & _
" tblReportData.MonthlyReportDate,  " & _
" tblReportData.HoursInMonth,  " & _
" tblReportData.PeriodUtilization,  " & _
" tblReportData.BillRate,  " & _
" tblReportData.CompCosts,  " & _
" tblReportData.Revenue,  " & _
" tblReportData.Costs " & _
" FROM tblBusinessUnit INNER JOIN ((tblAccountGroup INNER JOIN (tblClients INNER JOIN tblOpportunity ON tblClients.ClientID = tblOpportunity.ClientID)  " & _
" ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID) INNER JOIN (tblResources INNER JOIN tblReportData ON tblResources.PK_PersonID = tblReportData.PersonID)  " & _
" ON tblOpportunity.PK_OpportunityID = tblReportData.OpportunityID) ON tblBusinessUnit.ID = tblAccountGroup.BusinessUnit " & _
" WHERE tblAccountGroup.AccountGroupID=@AccountGroupID AND tblBusinessUnit.ID=@BUID AND tblReportData.MonthlyReportDate=@ReportMonth;"
        da.SelectCommand = New OleDbCommand(strSQL, connection)
        da.SelectCommand.Parameters.AddWithValue("@AccountGroupID", AccountGroupID)
        da.SelectCommand.Parameters.AddWithValue("@BUID", BUID)
        da.SelectCommand.Parameters.AddWithValue("@ReportMonth", ReportMonth)
        Try
            da.Fill(ds, "tblAssignments")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return ds
    End Function
    'Get BenchReport
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetBenchReport(ByVal dtEndDate As Date, ByVal strIncluded As String) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "PK_PersonID, " & _
            "LastName, " & _
            "FirstName, " & _
            "ActualRollOff, " & _
            "BookedRollOff, " & _
            "Employee " & _
            " From qryRollOffMaster where (ActualRollOff<=#" & dtEndDate & "# or " & _
            " BookedRollOff<=#" & dtEndDate & "#) "
        If Not String.IsNullOrEmpty(strIncluded) Then
            Select Case strIncluded
                Case "Employee"
                    sqlCommand &= " and Employee=True"
                Case "1099"
                    sqlCommand &= " and Employee=False"
                Case Else 'or "All"
                    'Change nothing
            End Select
        End If
        sqlCommand &= " Order By ActualRollOff "
        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "qryRollOffMaster")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("qryRollOffMaster") IsNot Nothing Then
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
Public Shared Function GetBenchReportByBU(ByVal dtEndDate As Date, ByVal strIncluded As String, ByVal dblBU_ID As Double) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "SELECT " & _
            "PK_PersonID, " & _
            "LastName, " & _
            "FirstName, " & _
            "ActualRollOff, " & _
            "BookedRollOff, " & _
            "Employee " & _
            " From qryRollOffMaster where BusinessUnit=" & dblBU_ID & " and (ActualRollOff<=#" & dtEndDate & "# or " & _
            " BookedRollOff<=#" & dtEndDate & "#) "
        If Not String.IsNullOrEmpty(strIncluded) Then
            Select Case strIncluded
                Case "Employee"
                    sqlCommand &= " and Employee=True"
                Case "1099"
                    sqlCommand &= " and Employee=False"
                Case Else 'or "All"
                    'Change nothing
            End Select
        End If
        sqlCommand &= " Order By ActualRollOff "
        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(ds, "qryRollOffMaster")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("qryRollOffMaster") IsNot Nothing Then
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
Public Shared Function GetPersonBenchDetails(ByVal PK_PersonID As Double, ByVal strSort As String, ByVal dtEndDate As Date, Optional ByVal bLimitedResults As Boolean = False) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim dsBooked As DataSet = New DataSet
        Dim dsForecasted As DataSet = New DataSet
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String
        '
        If bLimitedResults Then
            sqlCommand = " SELECT tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, Max(tblAssignments.EndDate) AS myEnd, Min(tblAssignments.StartDate) AS myStart, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, 'Booked' as RollOffType, tblAssignments.AssignmentID , tblClients.Client, tblClients.ClientID " & _
    " FROM tblClients INNER JOIN ((tblOpportunity INNER JOIN (tblResources INNER JOIN tblAssignments ON tblResources.PK_PersonID = tblAssignments.FK_PersonID) ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) INNER JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID " & _
    " WHERE (((tblResources.PK_PersonID)=" & PK_PersonID & ") AND tblAssignments.EndDate>=Now and ((qrylatestEntry.WinPercentage)=1) AND ((tblAssignments.PeriodUtilizationRate) Is Not Null))" & _
    " GROUP BY tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, tblAssignments.AssignmentID, tblClients.Client, tblClients.ClientID  " & _
    " HAVING (((tblResources.Inactive)=False))"
        Else
            sqlCommand = " SELECT tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, Max(tblAssignments.EndDate) AS myEnd, Min(tblAssignments.StartDate) AS myStart, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, 'Booked' as RollOffType, tblAssignments.AssignmentID , tblClients.Client, tblClients.ClientID " & _
" FROM tblClients INNER JOIN ((tblOpportunity INNER JOIN (tblResources INNER JOIN tblAssignments ON tblResources.PK_PersonID = tblAssignments.FK_PersonID) ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) INNER JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID " & _
" WHERE (((tblResources.PK_PersonID)=" & PK_PersonID & ") AND ((qrylatestEntry.WinPercentage)=1) AND ((tblAssignments.PeriodUtilizationRate) Is Not Null))" & _
" GROUP BY tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, tblAssignments.AssignmentID, tblClients.Client, tblClients.ClientID  " & _
" HAVING (((tblResources.Inactive)=False))"

        End If
        If String.IsNullOrEmpty(strSort) Then
            sqlCommand &= " ORDER BY tblResources.Employee, tblResources.LastName, Max(tblAssignments.EndDate);"
        Else
            sqlCommand &= strSort
        End If



        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(dsBooked, "tblRollOff")
            If bLimitedResults Then
                If dsBooked.Tables("tblRollOff").Rows.Count > 1 Then
                    'Delete other rows
                    'Remove if assignment end date is in the past

                End If
            End If
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        Dim sqlCommand2 As String
        If bLimitedResults Then
            sqlCommand2 = " SELECT tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, Max(tblAssignments.EndDate) AS myEnd, Min(tblAssignments.StartDate) AS myStart, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, 'Shadow' as RollOffType, tblAssignments.AssignmentID,tblClients.Client, tblClients.ClientID " & _
    " FROM tblClients INNER JOIN ((tblOpportunity INNER JOIN (tblResources INNER JOIN tblAssignments ON tblResources.PK_PersonID = tblAssignments.FK_PersonID) ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) INNER JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID " & _
    " WHERE (((tblResources.PK_PersonID)=" & PK_PersonID & ") AND tblAssignments.EndDate>=Now AND ((qrylatestEntry.WinPercentage)<1 And (qrylatestEntry.WinPercentage)>=0.7) AND ((tblAssignments.PeriodUtilizationRate) Is Not Null))" & _
    " GROUP BY tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, tblAssignments.AssignmentID,tblClients.Client, tblClients.ClientID " & _
    " HAVING (((tblResources.Inactive)=False))"
        Else
            sqlCommand2 = " SELECT tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, Max(tblAssignments.EndDate) AS myEnd, Min(tblAssignments.StartDate) AS myStart, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, 'Shadow' as RollOffType, tblAssignments.AssignmentID,tblClients.Client, tblClients.ClientID " & _
    " FROM tblClients INNER JOIN ((tblOpportunity INNER JOIN (tblResources INNER JOIN tblAssignments ON tblResources.PK_PersonID = tblAssignments.FK_PersonID) ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) INNER JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID " & _
    " WHERE (((tblResources.PK_PersonID)=" & PK_PersonID & ") AND ((qrylatestEntry.WinPercentage)<1 And (qrylatestEntry.WinPercentage)>=0.7) AND ((tblAssignments.PeriodUtilizationRate) Is Not Null))" & _
    " GROUP BY tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, tblAssignments.AssignmentID,tblClients.Client, tblClients.ClientID " & _
    " HAVING (((tblResources.Inactive)=False))"
        End If
        If String.IsNullOrEmpty(strSort) Then
            sqlCommand2 &= " ORDER BY tblResources.Employee, tblResources.LastName, Max(tblAssignments.EndDate);"
        Else
            sqlCommand2 &= strSort
        End If

        da.SelectCommand = New OleDbCommand(sqlCommand2, connection)
        Try
            da.Fill(dsForecasted, "tblRollOff")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        dsBooked.Merge(dsForecasted, True, MissingSchemaAction.AddWithKey)
        Return dsBooked
    End Function
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function GetPersonBenchDetailsByWinPercentage(ByVal PK_PersonID As Double, ByVal strSort As String, ByVal dtEndDate As Date, Optional ByVal bLimitedResults As Boolean = False, Optional ByVal Winpercentage As Double = 1) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim dsData As DataSet = New DataSet
        Dim dsForecasted As DataSet = New DataSet
        Dim connection As New OleDbConnection(conString)
        Dim strWinPercentage As String
        If Winpercentage = 1 Then
            strWinPercentage = "qrylatestEntry.WinPercentage=1"
        Else
            strWinPercentage = "((qrylatestEntry.WinPercentage)<1 And (qrylatestEntry.WinPercentage)>=" & Winpercentage & ")"
        End If
        Dim sqlCommand As String
        '
        If bLimitedResults Then
            sqlCommand = " SELECT tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, Max(tblAssignments.EndDate) AS myEnd, Min(tblAssignments.StartDate) AS myStart, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, 'Booked' as RollOffType, tblAssignments.AssignmentID , tblClients.Client, tblClients.ClientID,qrylatestEntry.WinPercentage " & _
    " FROM tblClients INNER JOIN ((tblOpportunity INNER JOIN (tblResources INNER JOIN tblAssignments ON tblResources.PK_PersonID = tblAssignments.FK_PersonID) ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) INNER JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID " & _
    " WHERE (((tblResources.PK_PersonID)=" & PK_PersonID & ") AND tblAssignments.EndDate>=Now and " & strWinPercentage & " AND ((tblAssignments.PeriodUtilizationRate) Is Not Null))" & _
    " GROUP BY tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, tblAssignments.AssignmentID, tblClients.Client, tblClients.ClientID,qrylatestEntry.WinPercentage" & _
    " HAVING (((tblResources.Inactive)=False))"
        Else
            sqlCommand = " SELECT tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, Max(tblAssignments.EndDate) AS myEnd, Min(tblAssignments.StartDate) AS myStart, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, 'Booked' as RollOffType, tblAssignments.AssignmentID , tblClients.Client, tblClients.ClientID,qrylatestEntry.WinPercentage " & _
" FROM tblClients INNER JOIN ((tblOpportunity INNER JOIN (tblResources INNER JOIN tblAssignments ON tblResources.PK_PersonID = tblAssignments.FK_PersonID) ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) INNER JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID " & _
" WHERE (((tblResources.PK_PersonID)=" & PK_PersonID & ") AND " & strWinPercentage & " AND ((tblAssignments.PeriodUtilizationRate) Is Not Null))" & _
" GROUP BY tblResources.PK_PersonID, tblResources.LastName, tblResources.FirstName, tblResources.Employee, tblResources.Inactive, tblOpportunity.OpportunityName, tblOpportunity.PK_OpportunityID, tblAssignments.PeriodUtilizationRate, tblAssignments.AssignmentID, tblClients.Client, tblClients.ClientID,qrylatestEntry.WinPercentage" & _
" HAVING (((tblResources.Inactive)=False))"

        End If
        If String.IsNullOrEmpty(strSort) Then
            sqlCommand &= " ORDER BY tblResources.Employee, tblResources.LastName, Max(tblAssignments.EndDate);"
        Else
            sqlCommand &= strSort
        End If

        da.SelectCommand = New OleDbCommand(sqlCommand, connection)
        Try
            da.Fill(dsData, "tblData")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        Return dsData
    End Function

    'Insert new assignment
    <DataObjectMethodAttribute(DataObjectMethodType.Insert, True)> _
    Public Shared Function InsertNewAssignment( _
        ByVal FK_OpportunityID As Double, _
        ByVal FK_PersonID As Double, _
        ByVal StartDate As Date, _
        ByVal EndDate As Date, _
        ByVal BillRate As Double, _
        ByVal Costs As Double, _
        ByVal PeriodUtilizationRate As Double, _
        ByVal ExcludeFromBURevenueCalculation As Boolean, _
        ByVal NewItemID As Double) As Double

        Dim returnID As Double
        Dim cmdText As String
        'This is the code for SQL Server
        'cmdText = "Insert Into tblAssignments " & _
        '   "(FK_OpportunityID, FK_PersonID, StartDate, EndDate, BillRate, Costs, CalculatedRevenue, PeriodUtilizationRate) " & _
        '   "Values (@FK_OpportunityID, @FK_PersonID, @StartDate, @EndDate, @BillRate, @Costs, @CalculatedRevenue, @PeriodUtilizationRate) SET @IndustryID = SCOPE_IDENTITY()"

        'For MS Access
        cmdText = "Insert Into tblAssignments " & _
           "(FK_OpportunityID, FK_PersonID, StartDate, EndDate, BillRate, Costs, PeriodUtilizationRate,ExcludeFromBURevenueCalculation) " & _
           "Values (@FK_OpportunityID, @FK_PersonID, @StartDate, @EndDate, @BillRate, @Costs, @PeriodUtilizationRate,@ExcludeFromBURevenueCalculation)"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'Adjust utilizationpercentage to true percentage
        If PeriodUtilizationRate > 1 Then PeriodUtilizationRate /= 100
        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@FK_OpportunityID", FK_OpportunityID)
        cmd.Parameters.AddWithValue("@FK_PersonID", FK_PersonID)
        cmd.Parameters.AddWithValue("@StartDate", StartDate)
        cmd.Parameters.AddWithValue("@EndDate", EndDate)
        cmd.Parameters.AddWithValue("@BillRate", BillRate)
        cmd.Parameters.AddWithValue("@Costs", Costs)
        cmd.Parameters.AddWithValue("@PeriodUtilizationRate", PeriodUtilizationRate)
        cmd.Parameters.AddWithValue("@ExcludeFromBURevenueCalculation", ExcludeFromBURevenueCalculation)

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
                'Now, cycle through months!
                Dim CalculatedRevenue As Double = UpdateReportingData(returnID, StartDate, EndDate, BillRate, Costs, PeriodUtilizationRate, FK_OpportunityID, FK_PersonID)
                'Update CalculatedRevenue for new amts!
                ExecuteStatement("Update tblAssignments set CalculatedRevenue=" & CalculatedRevenue & " where AssignmentID=" & returnID)
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
    Private Shared Function UpdateReportingData(ByVal AssignmentID As Double, ByVal StartDate As Date, _
        ByVal EndDate As Date, _
        ByVal BillRate As Double, _
        ByVal Costs As Double, _
        ByVal PeriodUtilizationRate As Double, ByVal FK_OpportunityID As Double, ByVal FK_PersonID As Double) As Double

        'Delete all old values
        ExecuteStatement("Delete from tblReportData where AssignmentID=" & AssignmentID)
        Dim currentHours As Double = 0

        Dim connection As New OleDbConnection(conString)
        Dim strSQL As String = "SELECT ReportOrder, MontlyReportDates, StartDate, EndDate, HoursInMonth, archive, Year " & _
            " FROM tblMonthlyReportDates " & _
            " WHERE (tblMonthlyReportDates.StartDate>=#" & StartDate & "# AND tblMonthlyReportDates.EndDate<=#" & EndDate & "#) OR (#" & StartDate & "# Between [StartDate] And [EndDate]) OR (#" & EndDate & "# Between [StartDate] And [EndDate]) " & _
            " ORDER BY StartDate"

        Dim cmd As OleDbCommand = New OleDbCommand(strSQL, connection)
        Dim dr As OleDbDataReader
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            dr = cmd.ExecuteReader
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        End Try
        Dim ratio As Double = 1
        Dim NumDaysInMonth As Double
        Dim tempDateDiff As Double = 0
        Dim runningRevenue As Double = 0
        Dim tempBillable As Double = 0
        While dr.Read
            'Check for Partial
            If StartDate >= dr.Item("StartDate") And EndDate >= dr.Item("EndDate") Then
                NumDaysInMonth = GetWorkDays(dr.Item("StartDate"), dr.Item("EndDate"))
                tempDateDiff = GetWorkDays(StartDate, dr.Item("EndDate"))
                tempBillable = tempDateDiff
                ratio = Math.Round(((tempDateDiff) / NumDaysInMonth), 2)
            ElseIf StartDate >= dr.Item("StartDate") And StartDate <= dr.Item("EndDate") Then
                NumDaysInMonth = GetWorkDays(dr.Item("StartDate"), dr.Item("EndDate"))
                tempDateDiff = GetWorkDays(StartDate, EndDate)
                tempBillable = tempDateDiff
                ratio = Math.Round(((tempDateDiff) / NumDaysInMonth), 2)
            ElseIf EndDate <= dr.Item("EndDate") Then
                NumDaysInMonth = GetWorkDays(dr.Item("StartDate"), dr.Item("EndDate"))
                tempDateDiff = GetWorkDays(EndDate, dr.Item("EndDate")) - 1
                If tempDateDiff < 0 Then tempDateDiff = 0
                tempBillable = NumDaysInMonth - tempDateDiff
                ratio = Math.Round(((NumDaysInMonth - tempDateDiff) / NumDaysInMonth), 2)
            Else
                NumDaysInMonth = GetWorkDays(dr.Item("StartDate"), dr.Item("EndDate"))
                tempBillable = NumDaysInMonth
                ratio = 1
            End If
            runningRevenue += tempBillable * 8 * BillRate * PeriodUtilizationRate
            ExecuteStatement("Insert Into tblReportData (MonthlyReportDate,Hours,Revenue,Costs,AssignmentID,OpportunityID,personID,HoursInMonth,PeriodUtilization,BillRate,CompCosts) values (" & dr.Item("ReportOrder") & "," & NumDaysInMonth * 8 & "," & tempBillable * 8 * BillRate * PeriodUtilizationRate & "," & tempBillable * 8 * Costs * PeriodUtilizationRate & "," & AssignmentID & "," & FK_OpportunityID & "," & FK_PersonID & "," & NumDaysInMonth * 8 & "," & PeriodUtilizationRate * ratio & "," & BillRate & "," & Costs & ")")
        End While
        dr.Close()
        Return runningRevenue
    End Function
    Public Shared Function GetWorkDaysForCalendarMonth(ByVal dblReportPeriod As Double) As Double
        Dim connection As New OleDbConnection(conString)
        Dim NumDaysInMonth As Double = 0
        Dim strSQL As String = "SELECT ReportOrder, MontlyReportDates, StartDate, EndDate, HoursInMonth, archive, Year " & _
            " FROM tblMonthlyReportDates " & _
            " WHERE ReportOrder=" & dblReportPeriod

        Dim cmd As OleDbCommand = New OleDbCommand(strSQL, connection)
        Dim dr As OleDbDataReader
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            dr = cmd.ExecuteReader
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        End Try
        While dr.Read
            NumDaysInMonth = GetWorkDays(dr.Item("StartDate"), dr.Item("EndDate"))
        End While
        Return NumDaysInMonth
    End Function
    Private Shared Function GetWorkDays(ByVal dtStart As Date, ByVal dtEnd As Date) As Double
        Dim NumDaysInMonth As Double = 0
        Dim countHoliday As Double = GetSingleSQLValue("Select count(1) from tblHolidays where Holiday>=#" & dtStart & "# and Holiday<=#" & dtEnd & "#")
        Dim tempDays As Double = 0
        tempDays = DateDiff(DateInterval.Day, dtStart, dtEnd) + 1
        For x = 0 To DateDiff(DateInterval.Day, dtStart, dtEnd) + 1
            If Weekday(DateAdd(DateInterval.Day, x, dtStart)) = 1 Or Weekday(DateAdd(DateInterval.Day, x, dtStart)) = 7 Then
                tempDays -= 1
            End If
        Next
        'Add # of days in beginning of week
        'tempDays += 7 - Weekday(dtStart)
        'Add number of full weeks between date
        'tempDays += 5 * (DateDiff("ww", dtStart, dtEnd) - 1)
        'Add # of days in last week of month
        'If Weekday(dtEnd) >= 6 Then
        ' tempDays += 5
        ' Else
        'tempDays += Weekday(dtEnd) - 1
        'End If
        NumDaysInMonth = tempDays - countHoliday
        Return NumDaysInMonth
    End Function
    'Update assignment
    <DataObjectMethodAttribute(DataObjectMethodType.Update, True)> _
Public Shared Function UpdateSingleAssignment( _
    ByVal AssignmentID As Double, _
    ByVal FK_OpportunityID As Double, _
    ByVal FK_PersonID As Double, _
    ByVal StartDate As Date, _
    ByVal EndDate As Date, _
    ByVal BillRate As Double, _
    ByVal Costs As Double, _
    ByVal ExcludeFromBURevenueCalculation As Boolean, _
    ByVal PeriodUtilizationRate As Double) As Boolean

        Dim cmdText As String
        cmdText = "Update tblAssignments Set " & _
            "[FK_OpportunityID]=@FK_OpportunityID," & _
            "[FK_PersonID]=@FK_PersonID," & _
            "[StartDate]=@StartDate," & _
            "[EndDate]=@EndDate," & _
            "[BillRate]=@BillRate," & _
            "[Costs]=@Costs," & _
            "[PeriodUtilizationRate]=@PeriodUtilizationRate, " & _
            "[ExcludeFromBURevenueCalculation]=@ExcludeFromBURevenueCalculation " & _
            "Where [AssignmentID]=@AssignmentID"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)

        'Adjust utilizationpercentage to true percentage
        If PeriodUtilizationRate > 1 Then PeriodUtilizationRate /= 100
        'apparently, the parameters need to be in order in which they are used in the SQL statement
        cmd.Parameters.AddWithValue("@FK_OpportunityID", FK_OpportunityID)
        cmd.Parameters.AddWithValue("@FK_PersonID", FK_PersonID)
        cmd.Parameters.AddWithValue("@StartDate", StartDate)
        cmd.Parameters.AddWithValue("@EndDate", EndDate)
        cmd.Parameters.AddWithValue("@BillRate", BillRate)
        cmd.Parameters.AddWithValue("@Costs", Costs)
        cmd.Parameters.AddWithValue("@PeriodUtilizationRate", PeriodUtilizationRate)
        cmd.Parameters.AddWithValue("@ExcludeFromBURevenueCalculation", ExcludeFromBURevenueCalculation)
        cmd.Parameters.AddWithValue("@AssignmentID", AssignmentID)

        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            If cmd.ExecuteNonQuery() <> 0 Then
                'Now, cycle through months!
                Dim CalculatedRevenue As Double = UpdateReportingData(AssignmentID, StartDate, EndDate, BillRate, Costs, PeriodUtilizationRate, FK_OpportunityID, FK_PersonID)
                'Update CalculatedRevenue for new amts!
                ExecuteStatement("Update tblAssignments set CalculatedRevenue=" & CalculatedRevenue & " where AssignmentID=" & AssignmentID)
                Return True 'Rows updated
            Else
                Return False '0 rows updated.
            End If
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return True
    End Function

    'Delete Assignment
    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
    Public Shared Function DeleteSingleAssignment(ByVal AssignmentID As Integer) As Boolean
        Dim testComplete As Boolean = False

        If String.IsNullOrEmpty(AssignmentID) Then Throw New ArgumentException("AssignmentID cannot be null or an empty string.")
        testComplete = (ExecuteStatement("Delete from tblReportData where AssignmentID=" & AssignmentID))
        testComplete = ExecuteStatement("Delete from tblAssignments where [AssignmentID]=" & AssignmentID)
        Return testComplete
    End Function
    'Delete Assignment
    <DataObjectMethodAttribute(DataObjectMethodType.Delete, True)> _
    Public Shared Function DeleteAssignmentsOnOpportunity(ByVal OpportunityID As Integer) As Boolean
        Dim testComplete As Boolean = False
        If String.IsNullOrEmpty(OpportunityID) Then Throw New ArgumentException("AssignmentID cannot be null or an empty string.")
        testComplete = ExecuteStatement("Delete from tblReportData where OpportunityID=" & OpportunityID)
        testComplete = ExecuteStatement("Delete from tblAssignments where [FK_OpportunityID]=" & OpportunityID)
        Return testComplete
    End Function

    'Update Resources
    <DataObjectMethodAttribute(DataObjectMethodType.Select, False)> _
Public Shared Function UpdatePeople(ByVal PK_PersonID As Double, _
ByVal LastName As String, _
ByVal FirstName As String, _
ByVal Office As String, _
ByVal Cell As String, _
ByVal Home As String, _
ByVal Email As String, _
ByVal Employee As Boolean, _
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
            "[Email]=@Email" & _
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

    'This is for the advanced month by month forecasting modifications
    <DataObjectMethodAttribute(DataObjectMethodType.Update, False)> _
Public Shared Function UpdateSingleReportMonthData( _
ByVal EntryID As Double, _
ByVal MonthlyReportDate As Double, _
ByVal MonthlyHours As Double, _
ByVal Revenue As Double, _
ByVal MonthlyCosts As Double, _
ByVal AssignmentID As Double, _
ByVal OpportunityID As Double, _
ByVal PersonID As Double, _
ByVal HoursInMonth As Double, _
ByVal PeriodUtilization As Double, _
ByVal BillRate As Double, _
ByVal CompMonthlyCosts As Double) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "Update tblReportData Set " & _
            "[MonthlyReportDate]=@MonthlyReportDate," & _
            "[Hours]=@MonthlyHours," & _
            "[Revenue]=@Revenue," & _
            "[Costs]=@MonthlyCosts," & _
            "[AssignmentID]=@AssignmentID," & _
            "[OpportunityID]=@OpportunityID," & _
            "[PersonID]=@PersonID," & _
            "[HoursInMonth]=@HoursInMonth," & _
            "[PeriodUtilization]=@PeriodUtilization," & _
            "[BillRate]=@BillRate," & _
            "[CompCosts]=@CompMonthlyCosts " & _
            " Where [EntryID]=" & EntryID



        Dim cmd As OleDbCommand = New OleDbCommand(sqlCommand, connection)
        cmd.Parameters.AddWithValue("@MonthlyReportDate", MonthlyReportDate)
        cmd.Parameters.AddWithValue("@MonthlyHours", MonthlyHours)
        cmd.Parameters.AddWithValue("@Revenue", Revenue)
        cmd.Parameters.AddWithValue("@MonthlyCosts", MonthlyCosts)
        cmd.Parameters.AddWithValue("@AssignmentID", AssignmentID)
        cmd.Parameters.AddWithValue("@OpportunityID", OpportunityID)
        cmd.Parameters.AddWithValue("@PersonID", PersonID)
        cmd.Parameters.AddWithValue("@HoursInMonth", HoursInMonth)
        cmd.Parameters.AddWithValue("@PeriodUtilization", PeriodUtilization)
        cmd.Parameters.AddWithValue("@BillRate", BillRate)
        cmd.Parameters.AddWithValue("@CompMonthlyCosts", CompMonthlyCosts)


        'If cmd.Parameters("@MonthlyReportDate").Value = "" Then cmd.Parameters("@MonthlyReportDate").Value = System.DBNull.Value
        'If cmd.Parameters("@MonthlyHours").Value = "" Then cmd.Parameters("@MonthlyHours").Value = 0
        'If cmd.Parameters("@Revenue").Value = "" Then cmd.Parameters("@Revenue").Value = 0
        'If cmd.Parameters("@MonthlyCosts").Value = "" Then cmd.Parameters("@MonthlyCosts").Value = 0
        'If cmd.Parameters("@AssignmentID").Value = "" Then cmd.Parameters("@AssignmentID").Value = System.DBNull.Value
        'If cmd.Parameters("@OpportunityID").Value = "" Then cmd.Parameters("@OpportunityID").Value = System.DBNull.Value
        'If cmd.Parameters("@PersonID").Value = "" Then cmd.Parameters("@PersonID").Value = System.DBNull.Value
        'If cmd.Parameters("@HoursInMonth").Value = "" Then cmd.Parameters("@HoursInMonth").Value = 0
        'If cmd.Parameters("@PeriodUtilization").Value = "" Then cmd.Parameters("@PeriodUtilization").Value = 0
        'If cmd.Parameters("@BillRate").Value = "" Then cmd.Parameters("@BillRate").Value = 0
        'If cmd.Parameters("@CompMonthlyCosts").Value = "" Then cmd.Parameters("@CompMonthlyCosts").Value = 0
        'Do in business layer
        'cmd.Parameters("@Revenue").Value = cmd.Parameters("@PeriodUtilization").Value * cmd.Parameters("@BillRate").Value * cmd.Parameters("@MonthlyHours").Value
        'cmd.Parameters("@MonthlyCosts").Value = cmd.Parameters("@PeriodUtilization").Value * cmd.Parameters("@CompMonthlyCosts").Value * cmd.Parameters("@MonthlyHours").Value


        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            'cmd.ExecuteScalar()
            If cmd.ExecuteNonQuery() <> 0 Then
                If connection.State <> ConnectionState.Closed Then
                    connection.Close()
                End If
                UpdateAssignmentSummation(AssignmentID)
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
    <DataObjectMethodAttribute(DataObjectMethodType.Update, False)> _
Public Shared Function UpdateAssignmentSummation( _
ByVal AssignmentID As Double) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        'Changed all variables to strings.  This is because on the ForecastingUpdateAdvanced, you can delete the last entry
        'resulint in this update code being triggered.  But if I delete all assignments, I get null retrun values from the below queries
        'So, I converted them to strings.  Then I test to see if any of the date ones are null, if so, I set all variables to now and zero's.  I don't kill the assignment, only the values in the assignment
        'Then, for the other variables, if they're null i set them to zero.
        'When I do the update statement, I typecast the variable to what I need it to be.
        Dim tempStart As String = GetSingleSQLValue("(Select startdate from tblMonthlyReportDates where ReportOrder=(Select min(MonthlyReportDate) from tblReportData where tblReportData.assignmentID=" & AssignmentID & "))")
        Dim tempEnd As String = GetSingleSQLValue("(Select enddate from tblMonthlyReportDates where ReportOrder=(Select max(MonthlyReportDate) from tblReportData where tblReportData.assignmentID=" & AssignmentID & "))")
        Dim AvgBillRate As String = GetSingleSQLValue("(Select avg(BillRate) from tblReportData where tblReportData.assignmentID=" & AssignmentID & ")")
        Dim avgCost As String = GetSingleSQLValue("(Select avg(CompCosts) from tblReportData where tblReportData.assignmentID=" & AssignmentID & ")")
        Dim totalRevenue As String = GetSingleSQLValue("(Select sum(Revenue) from tblReportData where tblReportData.assignmentID=" & AssignmentID & ")")
        Dim totalHours As String = GetSingleSQLValue("(Select sum(Hours) from tblReportData where tblReportData.assignmentID=" & AssignmentID & ")")
        Dim PeriodUtilizationRate As Double

        If String.IsNullOrEmpty(tempStart) Or String.IsNullOrEmpty(tempEnd) Then
            tempStart = String.Format("{0:d}", Date.Now)
            tempEnd = String.Format("{0:d}", Date.Now)
            totalRevenue = 0
            avgCost = 0
            AvgBillRate = 0
            totalHours = 0
        Else
            If String.IsNullOrEmpty(AvgBillRate) Then AvgBillRate = 0
            If String.IsNullOrEmpty(avgCost) Then avgCost = 0
            If String.IsNullOrEmpty(totalRevenue) Then totalRevenue = 0
            If String.IsNullOrEmpty(totalHours) Then totalHours = 0
        End If


        If AvgBillRate = 0 Or totalHours = 0 Then
            PeriodUtilizationRate = 0
        Else
            PeriodUtilizationRate = totalRevenue / (AvgBillRate * totalHours)
        End If


        Dim cmdText As String
        cmdText = "Update tblAssignments Set " & _
            "[StartDate]=@StartDate," & _
            "[EndDate]=@EndDate," & _
            "[BillRate]=@BillRate," & _
            "[Costs]=@Costs," & _
            "[PeriodUtilizationRate]=@PeriodUtilizationRate, " & _
            "[CalculatedRevenue]=@CalculatedRevenue " & _
            "Where [AssignmentID]=@AssignmentID"



        Dim cmd As OleDbCommand = New OleDbCommand(cmdText, connection)
        cmd.Parameters.AddWithValue("@StartDate", CDate(tempStart))
        cmd.Parameters.AddWithValue("@EndDate", CDate(tempEnd))
        cmd.Parameters.AddWithValue("@BillRate", CDbl(AvgBillRate))
        cmd.Parameters.AddWithValue("@Costs", CDbl(avgCost))
        cmd.Parameters.AddWithValue("@PeriodUtilizationRate", CDbl(PeriodUtilizationRate))
        cmd.Parameters.AddWithValue("@CalculatedRevenue", CDbl(totalRevenue))
        cmd.Parameters.AddWithValue("@AssignmentID", AssignmentID)


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
    <DataObjectMethodAttribute(DataObjectMethodType.Update, False)> _
Public Shared Function DeleteSingleReportMonthData(ByVal EntryID As Double) As Boolean
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        Dim tempAssignmentID As Double = GetSingleSQLValue("Select AssignmentID from tblReportData where EntryID=" & EntryID)
        Dim sqlCommand As String = "Delete from tblReportData Where [EntryID]=" & EntryID
        Dim cmd As OleDbCommand = New OleDbCommand(sqlCommand, connection)

        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            'cmd.ExecuteScalar()
            If cmd.ExecuteNonQuery() <> 0 Then
                If connection.State <> ConnectionState.Closed Then
                    connection.Close()
                End If
                UpdateAssignmentSummation(tempAssignmentID)
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
    <DataObjectMethodAttribute(DataObjectMethodType.Update, False)> _
Public Shared Function InsertSingleReportMonthData( _
ByVal MonthlyReportDate As Double, _
ByVal MonthlyHours As Double, _
ByVal Revenue As Double, _
ByVal MonthlyCosts As Double, _
ByVal AssignmentID As Double, _
ByVal OpportunityID As Double, _
ByVal PersonID As Double, _
ByVal HoursInMonth As Double, _
ByVal PeriodUtilization As Double, _
ByVal BillRate As Double, _
ByVal CompMonthlyCosts As Double) As Boolean

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        Dim sqlCommand As String = "Insert Into tblReportData ([MonthlyReportDate],[Hours],[Revenue],[Costs],[AssignmentID],[OpportunityID],[PersonID],[HoursInMonth],[PeriodUtilization],[BillRate],[CompCosts]) values " & _
            "(@MonthlyReportDate,@Hours,@Revenue,@Costs,@AssignmentID,@OpportunityID,@PersonID,@HoursInMonth,@PeriodUtilization,@BillRate,@CompCosts)" 


        Dim cmd As OleDbCommand = New OleDbCommand(sqlCommand, connection)
        cmd.Parameters.AddWithValue("@MonthlyReportDate", MonthlyReportDate)
        cmd.Parameters.AddWithValue("@Hours", MonthlyHours)
        cmd.Parameters.AddWithValue("@Revenue", Revenue)
        cmd.Parameters.AddWithValue("@Costs", MonthlyCosts)
        cmd.Parameters.AddWithValue("@AssignmentID", AssignmentID)
        cmd.Parameters.AddWithValue("@OpportunityID", OpportunityID)
        cmd.Parameters.AddWithValue("@PersonID", PersonID)
        cmd.Parameters.AddWithValue("@HoursInMonth", HoursInMonth)
        cmd.Parameters.AddWithValue("@PeriodUtilization", PeriodUtilization)
        cmd.Parameters.AddWithValue("@BillRate", BillRate)
        cmd.Parameters.AddWithValue("@CompCosts", CompMonthlyCosts)



        'If cmd.Parameters("@MonthlyReportDate").Value = "" Then cmd.Parameters("@MonthlyReportDate").Value = System.DBNull.Value
        'If cmd.Parameters("@MonthlyHours").Value = "" Then cmd.Parameters("@MonthlyHours").Value = 0
        'If cmd.Parameters("@Revenue").Value = "" Then cmd.Parameters("@Revenue").Value = 0
        'If cmd.Parameters("@MonthlyCosts").Value = "" Then cmd.Parameters("@MonthlyCosts").Value = 0
        'If cmd.Parameters("@AssignmentID").Value = "" Then cmd.Parameters("@AssignmentID").Value = System.DBNull.Value
        'If cmd.Parameters("@OpportunityID").Value = "" Then cmd.Parameters("@OpportunityID").Value = System.DBNull.Value
        'If cmd.Parameters("@PersonID").Value = "" Then cmd.Parameters("@PersonID").Value = System.DBNull.Value
        'If cmd.Parameters("@HoursInMonth").Value = "" Then cmd.Parameters("@HoursInMonth").Value = 0
        'If cmd.Parameters("@PeriodUtilization").Value = "" Then cmd.Parameters("@PeriodUtilization").Value = 0
        'If cmd.Parameters("@BillRate").Value = "" Then cmd.Parameters("@BillRate").Value = 0
        'If cmd.Parameters("@CompMonthlyCosts").Value = "" Then cmd.Parameters("@CompMonthlyCosts").Value = 0
        'Do in business layer
        'cmd.Parameters("@Revenue").Value = cmd.Parameters("@PeriodUtilization").Value * cmd.Parameters("@BillRate").Value * cmd.Parameters("@MonthlyHours").Value
        'cmd.Parameters("@MonthlyCosts").Value = cmd.Parameters("@PeriodUtilization").Value * cmd.Parameters("@CompMonthlyCosts").Value * cmd.Parameters("@MonthlyHours").Value


        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            'cmd.ExecuteScalar()
            If cmd.ExecuteNonQuery() <> 0 Then
                If connection.State <> ConnectionState.Closed Then
                    connection.Close()
                End If
                UpdateAssignmentSummation(AssignmentID)
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
