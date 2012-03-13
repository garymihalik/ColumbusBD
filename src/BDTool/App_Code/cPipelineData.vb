Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports System.IO
Public Class cPipelineData
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    Public Shared Function oldGetPipelineData(ByVal dtStart As Date, ByVal dtEnd As Date) As DataSet
        Dim strSQL As String = "SELECT WeeklyReportDates, sum(estimatedRevenue) AS TotalPipelineRevenue, sum(winpercentage*estimatedRevenue) AS WeightedRevenue, sum(winpercentage*estimatedRevenue)/sum(estimatedRevenue) AS PipelineConfidence " & _
" FROM tblOpportunityUpdates, tblWeeklyReportDates " & _
" WHERE (((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates)) AND ((tblOpportunityUpdates.WinPercentage)>0 And (tblOpportunityUpdates.WinPercentage)<1)) " & _
" GROUP BY WeeklyReportDates " & _
" HAVING WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=Now()+7; " & _
" Union " & _
" SELECT tblWeeklyReportDates.WeeklyReportDates, iif(sum(estimatedRevenue) is null,0,sum(estimatedRevenue)) AS TotalPipelineRevenue, IIF(sum(winpercentage*estimatedRevenue) is null,0,sum(winpercentage*estimatedRevenue)) AS WeightedRevenue, IIF((sum(winpercentage*estimatedRevenue)/sum(estimatedRevenue)) is null,0,sum(winpercentage*estimatedRevenue)/sum(estimatedRevenue)) AS PipelineConfidence " & _
" FROM tblWeeklyReportDates LEFT JOIN qryPipeLineForecastBase ON qryPipeLineForecastBase.OpportunityCloseDate>tblWeeklyReportDates.WeeklyReportDates " & _
" GROUP BY WeeklyReportDates " & _
" HAVING WeeklyReportDates<=#" & dtEnd & "# and WeeklyReportDates>now()+7; "
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        da.SelectCommand = New OleDbCommand(strSQL, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return ds
    End Function
    Public Shared Function oldGetWinLossTrend(ByVal dtStart As Date, ByVal dtEnd As Date) As DataSet
        Dim strSQL As String = " SELECT WeeklyReportDates, sum(IIF(WinPercentage,estimatedRevenue,0)) AS CumalativeWin, sum(IIF(WinPercentage,0,estimatedRevenue)) AS CumalativeLoss, sum(IIF(WinPercentage,estimatedRevenue,0))/(sum(IIF(WinPercentage,0,estimatedRevenue)) +sum(IIF(WinPercentage,estimatedRevenue,0))) as WinLossRatio " & _
            " FROM tblOpportunityUpdates, tblWeeklyReportDates " & _
            " WHERE (((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "#)) AND ((tblOpportunityUpdates.WinPercentage)=0 or (tblOpportunityUpdates.WinPercentage)=1 )) " & _
            " GROUP BY WeeklyReportDates " & _
            " HAVING WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "# "
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        da.SelectCommand = New OleDbCommand(strSQL, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return ds
    End Function
    Public Shared Function GetDates(ByVal strPrior As String) As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        Dim strSQL As String
        If strPrior = "Before" Then
            strSQL = "Select Format(WeeklyReportDates,'Short Date') as WeeklyReportDates from tblWeeklyReportDates where WeeklyReportDates<=now()"
        Else
            strSQL = "Select Format(WeeklyReportDates,'Short Date')as WeeklyReportDates from tblWeeklyReportDates where WeeklyReportDates>=now()"
        End If

        da.SelectCommand = New OleDbCommand(strSQL, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return ds
    End Function

    Public Shared Function GetWorkingPipeLineForExcel() As DataSet
        Dim strSQL As String = "SELECT PK_OpportunityID, qryWorkingPipelineWithWinLoss.OpportunityOwner, " & _
            "qryWorkingPipelineWithWinLoss.Client, " & _
            "qryWorkingPipelineWithWinLoss.OpportunityName, " & _
            "qryWorkingPipelineWithWinLoss.Fit, " & _
            "IIF(qryWorkingPipelineWithWinLoss.Anchor,'Yes','No') as Anchor, " & _
            "qryWorkingPipelineWithWinLoss.Source, " & _
            "qryWorkingPipelineWithWinLoss.[Date Entered], " & _
            "qryWorkingPipelineWithWinLoss.OpportunityCloseDate, " & _
            "DateDiff('d',[Date Entered],OpportunityCloseDate) AS DaysOpen, " & _
            "qryWorkingPipelineWithWinLoss.WinPercentage, " & _
            "qryWorkingPipelineWithWinLoss.EstimatedRevenue, " & _
            "qryWorkingPipelineWithWinLoss.WeightedRevenue, " & _
            "qryWorkingPipelineWithWinLoss.NextSteps,UpdateDate, RolesNeeded, RFPRequired, RFPLead, RFPRiskAssessment " & _
            "FROM qryWorkingPipelineWithWinLoss order by Client, OpportunityName;"
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        da.SelectCommand = New OleDbCommand(strSQL, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return ds

    End Function

    Public Shared Function GetPipelineBrokenDownData(ByVal dtStart As Date, ByVal dtEnd As Date) As DataSet
        'Dim strSQL As String = "SELECT tblWeeklyReportDates.WeeklyReportDates, tblOpportunity.OpportunityName, tblOpportunityUpdates.EstimatedRevenue, tblOpportunityUpdates.WinPercentage, tblOpportunity.Extension, tblOpportunity.Anchor, tblOpportunity.Source, tblOpportunity.PK_OpportunityID, tblOpportunityUpdates.PK_OpportunityUpdateID, EstimatedRevenue*WinPercentage as WeightedRevenue" & _
        '    " FROM tblWeeklyReportDates, tblOpportunity INNER JOIN tblOpportunityUpdates ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID " & _
        '    " WHERE ((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "#)) AND ((tblOpportunityUpdates.WinPercentage)>0 And (tblOpportunityUpdates.WinPercentage)<1) " & _
        '    " and WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=Now()+7 "
        Dim strSQL As String = " SELECT tblWeeklyReportDates.WeeklyReportDates, tblOpportunity.OpportunityName, tblOpportunityUpdates.EstimatedRevenue, tblOpportunityUpdates.WinPercentage,  " & _
                " tblOpportunity.Extension, tblOpportunity.Source, tblOpportunity.PK_OpportunityID, tblOpportunityUpdates.PK_OpportunityUpdateID, EstimatedRevenue*WinPercentage AS  " & _
                " WeightedRevenue, tblClients.Anchor " & _
                " FROM tblWeeklyReportDates, tblClients INNER JOIN (tblOpportunity INNER JOIN tblOpportunityUpdates ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID) ON  " & _
                " tblClients.ClientID = tblOpportunity.ClientID " & _
                " WHERE (((tblOpportunityUpdates.WinPercentage)>0 And (tblOpportunityUpdates.WinPercentage)<1) AND ((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from  " & _
                " tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "# )) " & _
                " AND ((tblWeeklyReportDates.[WeeklyReportDates])>=#" & dtStart & "# And (tblWeeklyReportDates.[WeeklyReportDates])<=Now()+7)); "

        Dim strSQL2 As String = "SELECT tblWeeklyReportDates.WeeklyReportDates, estimatedRevenue,winpercentage, anchor, source, extension,EstimatedRevenue*WinPercentage as WeightedRevenue " & _
            " FROM tblWeeklyReportDates LEFT JOIN qryPipeLineForecastBase ON qryPipeLineForecastBase.OpportunityCloseDate>tblWeeklyReportDates.WeeklyReportDates " & _
            " Where WeeklyReportDates<=#" & dtEnd & "# and WeeklyReportDates>now()+7; "

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Select * from tblWeeklyReportDates where WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "#", connection)

        Dim dr As OleDbDataReader
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            dr = cmd.ExecuteReader
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        End Try

        Dim _PastDS As DataSet = GetSingleDataset(strSQL)
        Dim _FutureDS As DataSet = GetSingleDataset(strSQL2)


        Dim _buildDS As New DataSet
        Dim _buildDT As New DataTable
        _buildDT.TableName = "BrokenDown"
        _buildDS.Tables.Add(_buildDT)
        AddColumn(_buildDS, "WeeklyReportDates", "System.DateTime", _buildDT.TableName)
        AddColumn(_buildDS, "TotalEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "TotalWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "TotalPipelineConfidence", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "HuntedEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedPipelineConfidence", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedPipelineConfidence", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "AnchorEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorPipelineConfidence", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryPipelineConfidence", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "NewEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewPipelineConfidence", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionPipelineConfidence", "System.Double", _buildDT.TableName)

        While dr.Read
            Dim _drInput As DataRow = _buildDS.Tables(_buildDT.TableName).NewRow
            _drInput.Item("WeeklyReportDates") = dr.Item("WeeklyReportDates")
            If dr.Item("WeeklyReportDates") <= DateAdd(DateInterval.Day, 7, Now) Then
                'use Past
                _drInput.Item("TotalEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))
                _drInput.Item("TotalWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))

                _drInput.Item("HuntedEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("HuntedWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("FarmedEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))
                _drInput.Item("FarmedWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))


                _drInput.Item("AnchorEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("AnchorWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("SecondaryEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))
                _drInput.Item("SecondaryWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))

                _drInput.Item("NewEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("NewWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("ExtensionEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
                _drInput.Item("ExtensionWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
            Else
                'Add future
                _drInput.Item("TotalEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))
                _drInput.Item("TotalWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))
                _drInput.Item("TotalPipelineConfidence") = _drInput.Item("TotalWeightedRevenue") / _drInput.Item("TotalEstimatedRevenue")

                _drInput.Item("HuntedEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("HuntedWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("FarmedEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))
                _drInput.Item("FarmedWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))

                _drInput.Item("AnchorEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("AnchorWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("SecondaryEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))
                _drInput.Item("SecondaryWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))

                _drInput.Item("NewEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("NewWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("ExtensionEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
                _drInput.Item("ExtensionWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
            End If
            If _drInput.Item("TotalEstimatedRevenue") <> 0 Then
                _drInput.Item("TotalPipelineConfidence") = _drInput.Item("TotalWeightedRevenue") / _drInput.Item("TotalEstimatedRevenue")
            Else
                _drInput.Item("TotalPipelineConfidence") = 0
            End If
            If _drInput.Item("HuntedEstimatedRevenue") <> 0 Then
                _drInput.Item("HuntedPipelineConfidence") = _drInput.Item("HuntedWeightedRevenue") / _drInput.Item("HuntedEstimatedRevenue")
            Else
                _drInput.Item("HuntedEstimatedRevenue") = 0
            End If
            If _drInput.Item("FarmedEstimatedRevenue") <> 0 Then
                _drInput.Item("FarmedPipelineConfidence") = _drInput.Item("FarmedWeightedRevenue") / _drInput.Item("FarmedEstimatedRevenue")
            Else
                _drInput.Item("FarmedEstimatedRevenue") = 0
            End If
            If _drInput.Item("AnchorEstimatedRevenue") <> 0 Then
                _drInput.Item("AnchorPipelineConfidence") = _drInput.Item("AnchorWeightedRevenue") / _drInput.Item("AnchorEstimatedRevenue")
            Else
                _drInput.Item("AnchorEstimatedRevenue") = 0
            End If
            If _drInput.Item("SecondaryEstimatedRevenue") <> 0 Then
                _drInput.Item("SecondaryPipelineConfidence") = _drInput.Item("SecondaryWeightedRevenue") / _drInput.Item("SecondaryEstimatedRevenue")
            Else
                _drInput.Item("SecondaryEstimatedRevenue") = 0
            End If
            If _drInput.Item("NewEstimatedRevenue") <> 0 Then
                _drInput.Item("NewPipelineConfidence") = _drInput.Item("NewWeightedRevenue") / _drInput.Item("NewEstimatedRevenue")
            Else
                _drInput.Item("NewEstimatedRevenue") = 0
            End If
            If _drInput.Item("ExtensionEstimatedRevenue") <> 0 Then
                _drInput.Item("ExtensionPipelineConfidence") = _drInput.Item("ExtensionWeightedRevenue") / _drInput.Item("ExtensionEstimatedRevenue")
            Else
                _drInput.Item("ExtensionEstimatedRevenue") = 0
            End If



            _buildDS.Tables(_buildDT.TableName).Rows.Add(_drInput)
        End While


        connection.Close()
        'WriteDataSetToFile("pipelinedata.csv", _buildDS.Tables(0))
        Return _buildDS
    End Function

    Public Shared Function GetWinLossBrokenDownData(ByVal dtStart As Date, ByVal dtEnd As Date) As DataSet
        Dim strSQL As String = "SELECT tblWeeklyReportDates.WeeklyReportDates, tblOpportunity.OpportunityName, tblOpportunityUpdates.EstimatedRevenue, tblOpportunityUpdates.WinPercentage, tblOpportunity.Extension, tblOpportunity.Anchor, tblOpportunity.Source, tblOpportunity.PK_OpportunityID, tblOpportunityUpdates.PK_OpportunityUpdateID, EstimatedRevenue*WinPercentage as WeightedRevenue, RolesNeeded,  " & _
            " RFPRequired, RFPLead, RFPRiskAssessment " & _
            " FROM tblWeeklyReportDates, tblOpportunity INNER JOIN tblOpportunityUpdates ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID " & _
            " WHERE ((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "#)) AND (WinPercentage=0 or WinPercentage=1) " & _
            " and WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "#"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Select * from tblWeeklyReportDates where WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "#", connection)

        Dim dr As OleDbDataReader
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            dr = cmd.ExecuteReader
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        End Try

        Dim _WinLossDS As DataSet = GetSingleDataset(strSQL)
        'WriteDataSetToFile("test4.csv", _WinLossDS.Tables(0))

        Dim _buildDS As New DataSet
        Dim _buildDT As New DataTable
        _buildDT.TableName = "BrokenDown"
        _buildDS.Tables.Add(_buildDT)
        AddColumn(_buildDS, "WeeklyReportDates", "System.DateTime", _buildDT.TableName)
        AddColumn(_buildDS, "CumWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumWinLossRatio", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumWinChange", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumLossChange", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "HuntedWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedWinLossRation", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedWinLossRatio", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "AnchorWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorWinLossRatio", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryWinLossRatio", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "NewWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewWinLossRatio", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionWinLossRatio", "System.Double", _buildDT.TableName)

        While dr.Read
            Dim _drInput As DataRow = _buildDS.Tables(_buildDT.TableName).NewRow
            _drInput.Item("WeeklyReportDates") = dr.Item("WeeklyReportDates")
            'use Past
            _drInput.Item("CumWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and WinPercentage=1")))
            _drInput.Item("CumLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and WinPercentage=0")))
            If _buildDS.Tables(_buildDT.TableName).Rows.Count = 0 Then
                _drInput.Item("CumWinChange") = 0
                _drInput.Item("CumLossChange") = 0
            Else
                _drInput.Item("CumWinChange") = _drInput.Item("CumWin") - _buildDS.Tables(_buildDT.TableName).Rows(_buildDS.Tables(_buildDT.TableName).Rows.Count - 1).Item("CumWin")
                _drInput.Item("CumLossChange") = _drInput.Item("CumLoss") - _buildDS.Tables(_buildDT.TableName).Rows(_buildDS.Tables(_buildDT.TableName).Rows.Count - 1).Item("CumLoss")
            End If
            _drInput.Item("HuntedWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted' and WinPercentage=1")))
            _drInput.Item("HuntedLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted' and WinPercentage=0")))
            _drInput.Item("FarmedWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed' and WinPercentage=1")))
            _drInput.Item("FarmedLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed' and WinPercentage=0")))


            _drInput.Item("AnchorWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True and WinPercentage=1")))
            _drInput.Item("AnchorLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True and WinPercentage=0")))
            _drInput.Item("SecondaryWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False and WinPercentage=1")))
            _drInput.Item("SecondaryLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False and WinPercentage=0")))

            _drInput.Item("NewWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False and WinPercentage=1")))
            _drInput.Item("NewLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False and WinPercentage=0")))
            _drInput.Item("ExtensionWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True and WinPercentage=1")))
            _drInput.Item("ExtensionLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True and WinPercentage=0")))

            If _drInput.Item("CumWin") + _drInput.Item("CumLoss") <> 0 Then
                _drInput.Item("CumWinLossRatio") = _drInput.Item("CumWin") / (_drInput.Item("CumWin") + _drInput.Item("CumLoss"))
            Else
                _drInput.Item("CumWinLossRatio") = 0
            End If
            If _drInput.Item("HuntedWin") + _drInput.Item("HuntedLoss") <> 0 Then
                _drInput.Item("HuntedWinLossRation") = _drInput.Item("HuntedWin") / (_drInput.Item("HuntedLoss") + _drInput.Item("HuntedWin"))
            Else
                _drInput.Item("HuntedWinLossRation") = 0
            End If
            If _drInput.Item("FarmedWin") + _drInput.Item("FarmedLoss") <> 0 Then
                _drInput.Item("FarmedWinLossRatio") = _drInput.Item("FarmedWin") / (_drInput.Item("FarmedLoss") + _drInput.Item("FarmedWin"))
            Else
                _drInput.Item("FarmedWinLossRatio") = 0
            End If
            If _drInput.Item("AnchorWin") + _drInput.Item("AnchorLoss") <> 0 Then
                _drInput.Item("AnchorWinLossRatio") = _drInput.Item("AnchorWin") / (_drInput.Item("AnchorWin") + _drInput.Item("AnchorLoss"))
            Else
                _drInput.Item("AnchorWinLossRatio") = 0
            End If
            If _drInput.Item("SecondaryWin") + _drInput.Item("SecondaryLoss") <> 0 Then
                _drInput.Item("SecondaryWinLossRatio") = _drInput.Item("SecondaryWin") / (_drInput.Item("SecondaryWin") + _drInput.Item("SecondaryLoss"))
            Else
                _drInput.Item("SecondaryWinLossRatio") = 0
            End If
            If _drInput.Item("NewWin") + _drInput.Item("NewLoss") <> 0 Then
                _drInput.Item("NewWinLossRatio") = _drInput.Item("NewWin") / (_drInput.Item("NewLoss") + _drInput.Item("NewWin"))
            Else
                _drInput.Item("NewWinLossRatio") = 0
            End If
            If _drInput.Item("ExtensionWin") + _drInput.Item("ExtensionLoss") <> 0 Then
                _drInput.Item("ExtensionWinLossRatio") = _drInput.Item("ExtensionWin") / (_drInput.Item("ExtensionLoss") + _drInput.Item("ExtensionWin"))
            Else
                _drInput.Item("ExtensionWinLossRatio") = 0
            End If



            _buildDS.Tables(_buildDT.TableName).Rows.Add(_drInput)
        End While


        connection.Close()
        'WriteDataSetToFile("test4.csv", _buildDS.Tables(0))
        Return _buildDS
    End Function
#Region "By BU Code"
    Public Shared Function GetPipelineBrokenDownDataByBU(ByVal dtStart As Date, ByVal dtEnd As Date, ByVal dblBU_ID As Double) As DataSet
        'Dim strSQL As String = "SELECT tblWeeklyReportDates.WeeklyReportDates, tblOpportunity.OpportunityName, tblOpportunityUpdates.EstimatedRevenue, tblOpportunityUpdates.WinPercentage, tblOpportunity.Extension, tblOpportunity.Anchor, tblOpportunity.Source, tblOpportunity.PK_OpportunityID, tblOpportunityUpdates.PK_OpportunityUpdateID, EstimatedRevenue*WinPercentage as WeightedRevenue" & _
        '    " FROM tblWeeklyReportDates, tblOpportunity INNER JOIN tblOpportunityUpdates ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID " & _
        '    " WHERE ((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "#)) AND ((tblOpportunityUpdates.WinPercentage)>0 And (tblOpportunityUpdates.WinPercentage)<1) " & _
        '    " and WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=Now()+7 "
        Dim strSQL As String = " SELECT tblWeeklyReportDates.WeeklyReportDates, tblOpportunity.OpportunityName, tblOpportunityUpdates.EstimatedRevenue, tblOpportunityUpdates.WinPercentage,  " & _
                " tblOpportunity.Extension, tblOpportunity.Source, tblOpportunity.PK_OpportunityID, tblOpportunityUpdates.PK_OpportunityUpdateID, EstimatedRevenue*WinPercentage AS  " & _
                " WeightedRevenue, RolesNeeded, RFPRequired, RFPLead, RFPRiskAssessment, tblClients.Anchor " & _
                " FROM tblWeeklyReportDates, tblClients INNER JOIN (tblOpportunity INNER JOIN tblOpportunityUpdates ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID) ON  " & _
                " tblClients.ClientID = tblOpportunity.ClientID " & _
                " WHERE BusinessUnit=" & dblBU_ID & " and (((tblOpportunityUpdates.WinPercentage)>0 And (tblOpportunityUpdates.WinPercentage)<1) AND ((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from  " & _
                " tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "# )) " & _
                " AND ((tblWeeklyReportDates.[WeeklyReportDates])>=#" & dtStart & "# And (tblWeeklyReportDates.[WeeklyReportDates])<=Now()+7)); "

        Dim strSQL2 As String = "SELECT tblWeeklyReportDates.WeeklyReportDates, estimatedRevenue,winpercentage, anchor, source, extension,EstimatedRevenue*WinPercentage as WeightedRevenue " & _
            " FROM tblWeeklyReportDates LEFT JOIN qryPipeLineForecastBase ON qryPipeLineForecastBase.OpportunityCloseDate>tblWeeklyReportDates.WeeklyReportDates " & _
            " Where BusinessUnit=" & dblBU_ID & " and WeeklyReportDates<=#" & dtEnd & "# and WeeklyReportDates>now()+7; "

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Select * from tblWeeklyReportDates where WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "#", connection)

        Dim dr As OleDbDataReader
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            dr = cmd.ExecuteReader
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        End Try

        Dim _PastDS As DataSet = GetSingleDataset(strSQL)
        Dim _FutureDS As DataSet = GetSingleDataset(strSQL2)


        Dim _buildDS As New DataSet
        Dim _buildDT As New DataTable
        _buildDT.TableName = "BrokenDown"
        _buildDS.Tables.Add(_buildDT)
        AddColumn(_buildDS, "WeeklyReportDates", "System.DateTime", _buildDT.TableName)
        AddColumn(_buildDS, "TotalEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "TotalWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "TotalPipelineConfidence", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "HuntedEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedPipelineConfidence", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedPipelineConfidence", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "AnchorEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorPipelineConfidence", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryPipelineConfidence", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "NewEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewPipelineConfidence", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionEstimatedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionWeightedRevenue", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionPipelineConfidence", "System.Double", _buildDT.TableName)

        While dr.Read
            Dim _drInput As DataRow = _buildDS.Tables(_buildDT.TableName).NewRow
            _drInput.Item("WeeklyReportDates") = dr.Item("WeeklyReportDates")
            If dr.Item("WeeklyReportDates") <= DateAdd(DateInterval.Day, 7, Now) Then
                'use Past
                _drInput.Item("TotalEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))
                _drInput.Item("TotalWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))

                _drInput.Item("HuntedEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("HuntedWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("FarmedEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))
                _drInput.Item("FarmedWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))


                _drInput.Item("AnchorEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("AnchorWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("SecondaryEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))
                _drInput.Item("SecondaryWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))

                _drInput.Item("NewEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("NewWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("ExtensionEstimatedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
                _drInput.Item("ExtensionWeightedRevenue") = ReturnNumber(Convert.ToString(_PastDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
            Else
                'Add future
                _drInput.Item("TotalEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))
                _drInput.Item("TotalWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "#")))
                _drInput.Item("TotalPipelineConfidence") = _drInput.Item("TotalWeightedRevenue") / _drInput.Item("TotalEstimatedRevenue")

                _drInput.Item("HuntedEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("HuntedWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted'")))
                _drInput.Item("FarmedEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))
                _drInput.Item("FarmedWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed'")))

                _drInput.Item("AnchorEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("AnchorWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True")))
                _drInput.Item("SecondaryEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))
                _drInput.Item("SecondaryWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False")))

                _drInput.Item("NewEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("NewWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False")))
                _drInput.Item("ExtensionEstimatedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
                _drInput.Item("ExtensionWeightedRevenue") = ReturnNumber(Convert.ToString(_FutureDS.Tables(0).Compute("Sum(WeightedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True")))
            End If
            If _drInput.Item("TotalEstimatedRevenue") <> 0 Then
                _drInput.Item("TotalPipelineConfidence") = _drInput.Item("TotalWeightedRevenue") / _drInput.Item("TotalEstimatedRevenue")
            Else
                _drInput.Item("TotalPipelineConfidence") = 0
            End If
            If _drInput.Item("HuntedEstimatedRevenue") <> 0 Then
                _drInput.Item("HuntedPipelineConfidence") = _drInput.Item("HuntedWeightedRevenue") / _drInput.Item("HuntedEstimatedRevenue")
            Else
                _drInput.Item("HuntedEstimatedRevenue") = 0
            End If
            If _drInput.Item("FarmedEstimatedRevenue") <> 0 Then
                _drInput.Item("FarmedPipelineConfidence") = _drInput.Item("FarmedWeightedRevenue") / _drInput.Item("FarmedEstimatedRevenue")
            Else
                _drInput.Item("FarmedEstimatedRevenue") = 0
            End If
            If _drInput.Item("AnchorEstimatedRevenue") <> 0 Then
                _drInput.Item("AnchorPipelineConfidence") = _drInput.Item("AnchorWeightedRevenue") / _drInput.Item("AnchorEstimatedRevenue")
            Else
                _drInput.Item("AnchorEstimatedRevenue") = 0
            End If
            If _drInput.Item("SecondaryEstimatedRevenue") <> 0 Then
                _drInput.Item("SecondaryPipelineConfidence") = _drInput.Item("SecondaryWeightedRevenue") / _drInput.Item("SecondaryEstimatedRevenue")
            Else
                _drInput.Item("SecondaryEstimatedRevenue") = 0
            End If
            If _drInput.Item("NewEstimatedRevenue") <> 0 Then
                _drInput.Item("NewPipelineConfidence") = _drInput.Item("NewWeightedRevenue") / _drInput.Item("NewEstimatedRevenue")
            Else
                _drInput.Item("NewEstimatedRevenue") = 0
            End If
            If _drInput.Item("ExtensionEstimatedRevenue") <> 0 Then
                _drInput.Item("ExtensionPipelineConfidence") = _drInput.Item("ExtensionWeightedRevenue") / _drInput.Item("ExtensionEstimatedRevenue")
            Else
                _drInput.Item("ExtensionEstimatedRevenue") = 0
            End If



            _buildDS.Tables(_buildDT.TableName).Rows.Add(_drInput)
        End While


        connection.Close()
        'WriteDataSetToFile("pipelinedata.csv", _buildDS.Tables(0))
        Return _buildDS
    End Function
    Public Shared Function GetWinLossBrokenDownDataByBU(ByVal dtStart As Date, ByVal dtEnd As Date, ByVal dblBU_ID As Double) As DataSet
        Dim strSQL As String = "SELECT tblWeeklyReportDates.WeeklyReportDates,  " & _
            "tblOpportunity.OpportunityName,  " & _
            "tblOpportunityUpdates.EstimatedRevenue,  " & _
            "tblOpportunityUpdates.WinPercentage,  " & _
            "tblOpportunity.Extension,  " & _
            "tblOpportunity.Anchor,  " & _
            "tblOpportunity.Source, RolesNeeded, RFPRequired, RFPLead, RFPRiskAssessment, " & _
            "tblOpportunity.PK_OpportunityID,  " & _
            "tblOpportunityUpdates.PK_OpportunityUpdateID,  " & _
            "EstimatedRevenue*WinPercentage AS WeightedRevenue,  " & _
            "tblClients.BusinessUnit " & _
            "FROM tblWeeklyReportDates,  " & _
            "tblClients INNER JOIN (tblOpportunity INNER JOIN tblOpportunityUpdates  " & _
            "ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID)  " & _
            "ON tblClients.ClientID = tblOpportunity.ClientID " & _
            "WHERE (((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#1/6/2009#))  " & _
            "AND ((tblOpportunityUpdates.[WinPercentage])=0 Or (tblOpportunityUpdates.[WinPercentage])=1)  " & _
            "AND ((tblWeeklyReportDates.[WeeklyReportDates])>=#" & dtStart & "# And (tblWeeklyReportDates.[WeeklyReportDates])<=#" & dtEnd & "#)  " & _
            "AND ((tblClients.BusinessUnit)=" & dblBU_ID & "))"



        '"SELECT tblWeeklyReportDates.WeeklyReportDates, tblOpportunity.OpportunityName, tblOpportunityUpdates.EstimatedRevenue, tblOpportunityUpdates.WinPercentage, tblOpportunity.Extension, tblOpportunity.Anchor, tblOpportunity.Source, tblOpportunity.PK_OpportunityID, tblOpportunityUpdates.PK_OpportunityUpdateID, EstimatedRevenue*WinPercentage as WeightedRevenue" & _
        '" FROM tblWeeklyReportDates, tblOpportunity INNER JOIN tblOpportunityUpdates ON tblOpportunity.PK_OpportunityID = tblOpportunityUpdates.FK_OpportunityID " & _
        '" WHERE ((tblOpportunityUpdates.UpdateDate)=(Select max(UpdateDate) from tblOpportunityUpdates as t1 where t1.FK_OpportunityID=tblOpportunityUpdates.FK_OpportunityID and UpdateDate<=tblWeeklyReportDates.WeeklyReportDates and UpdateDate>=#" & dtStart & "#)) AND (WinPercentage=0 or WinPercentage=1) " & _
        '" and WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "#"

        Dim connection As New OleDbConnection(conString)
        Dim cmd As OleDbCommand = New OleDbCommand("Select * from tblWeeklyReportDates where WeeklyReportDates>=#" & dtStart & "# and WeeklyReportDates<=#" & dtEnd & "#", connection)

        Dim dr As OleDbDataReader
        Try
            If connection.State <> ConnectionState.Open Then
                connection.Open()
            End If
            dr = cmd.ExecuteReader
        Catch e As OleDbException
            Throw New ArgumentException(e.ToString)
        End Try

        Dim _WinLossDS As DataSet = GetSingleDataset(strSQL)
        'WriteDataSetToFile("test4.csv", _WinLossDS.Tables(0))

        Dim _buildDS As New DataSet
        Dim _buildDT As New DataTable
        _buildDT.TableName = "BrokenDown"
        _buildDS.Tables.Add(_buildDT)
        AddColumn(_buildDS, "WeeklyReportDates", "System.DateTime", _buildDT.TableName)
        AddColumn(_buildDS, "CumWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumWinLossRatio", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumWinChange", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "CumLossChange", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "HuntedWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "HuntedWinLossRation", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "FarmedWinLossRatio", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "AnchorWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "AnchorWinLossRatio", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "SecondaryWinLossRatio", "System.Double", _buildDT.TableName)

        AddColumn(_buildDS, "NewWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "NewWinLossRatio", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionWin", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionLoss", "System.Double", _buildDT.TableName)
        AddColumn(_buildDS, "ExtensionWinLossRatio", "System.Double", _buildDT.TableName)

        While dr.Read
            Dim _drInput As DataRow = _buildDS.Tables(_buildDT.TableName).NewRow
            _drInput.Item("WeeklyReportDates") = dr.Item("WeeklyReportDates")
            'use Past
            _drInput.Item("CumWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and WinPercentage=1")))
            _drInput.Item("CumLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and WinPercentage=0")))
            If _buildDS.Tables(_buildDT.TableName).Rows.Count = 0 Then
                _drInput.Item("CumWinChange") = 0
                _drInput.Item("CumLossChange") = 0
            Else
                _drInput.Item("CumWinChange") = _drInput.Item("CumWin") - _buildDS.Tables(_buildDT.TableName).Rows(_buildDS.Tables(_buildDT.TableName).Rows.Count - 1).Item("CumWin")
                _drInput.Item("CumLossChange") = _drInput.Item("CumLoss") - _buildDS.Tables(_buildDT.TableName).Rows(_buildDS.Tables(_buildDT.TableName).Rows.Count - 1).Item("CumLoss")
            End If
            _drInput.Item("HuntedWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted' and WinPercentage=1")))
            _drInput.Item("HuntedLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Hunted' and WinPercentage=0")))
            _drInput.Item("FarmedWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed' and WinPercentage=1")))
            _drInput.Item("FarmedLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Source='Farmed' and WinPercentage=0")))


            _drInput.Item("AnchorWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True and WinPercentage=1")))
            _drInput.Item("AnchorLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=True and WinPercentage=0")))
            _drInput.Item("SecondaryWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False and WinPercentage=1")))
            _drInput.Item("SecondaryLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Anchor=False and WinPercentage=0")))

            _drInput.Item("NewWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False and WinPercentage=1")))
            _drInput.Item("NewLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=False and WinPercentage=0")))
            _drInput.Item("ExtensionWin") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True and WinPercentage=1")))
            _drInput.Item("ExtensionLoss") = ReturnNumber(Convert.ToString(_WinLossDS.Tables(0).Compute("Sum(EstimatedRevenue)", "WeeklyReportDates=#" & dr.Item("WeeklyReportDates") & "# and Extension=True and WinPercentage=0")))

            If _drInput.Item("CumWin") + _drInput.Item("CumLoss") <> 0 Then
                _drInput.Item("CumWinLossRatio") = _drInput.Item("CumWin") / (_drInput.Item("CumWin") + _drInput.Item("CumLoss"))
            Else
                _drInput.Item("CumWinLossRatio") = 0
            End If
            If _drInput.Item("HuntedWin") + _drInput.Item("HuntedLoss") <> 0 Then
                _drInput.Item("HuntedWinLossRation") = _drInput.Item("HuntedWin") / (_drInput.Item("HuntedLoss") + _drInput.Item("HuntedWin"))
            Else
                _drInput.Item("HuntedWinLossRation") = 0
            End If
            If _drInput.Item("FarmedWin") + _drInput.Item("FarmedLoss") <> 0 Then
                _drInput.Item("FarmedWinLossRatio") = _drInput.Item("FarmedWin") / (_drInput.Item("FarmedLoss") + _drInput.Item("FarmedWin"))
            Else
                _drInput.Item("FarmedWinLossRatio") = 0
            End If
            If _drInput.Item("AnchorWin") + _drInput.Item("AnchorLoss") <> 0 Then
                _drInput.Item("AnchorWinLossRatio") = _drInput.Item("AnchorWin") / (_drInput.Item("AnchorWin") + _drInput.Item("AnchorLoss"))
            Else
                _drInput.Item("AnchorWinLossRatio") = 0
            End If
            If _drInput.Item("SecondaryWin") + _drInput.Item("SecondaryLoss") <> 0 Then
                _drInput.Item("SecondaryWinLossRatio") = _drInput.Item("SecondaryWin") / (_drInput.Item("SecondaryWin") + _drInput.Item("SecondaryLoss"))
            Else
                _drInput.Item("SecondaryWinLossRatio") = 0
            End If
            If _drInput.Item("NewWin") + _drInput.Item("NewLoss") <> 0 Then
                _drInput.Item("NewWinLossRatio") = _drInput.Item("NewWin") / (_drInput.Item("NewLoss") + _drInput.Item("NewWin"))
            Else
                _drInput.Item("NewWinLossRatio") = 0
            End If
            If _drInput.Item("ExtensionWin") + _drInput.Item("ExtensionLoss") <> 0 Then
                _drInput.Item("ExtensionWinLossRatio") = _drInput.Item("ExtensionWin") / (_drInput.Item("ExtensionLoss") + _drInput.Item("ExtensionWin"))
            Else
                _drInput.Item("ExtensionWinLossRatio") = 0
            End If



            _buildDS.Tables(_buildDT.TableName).Rows.Add(_drInput)
        End While


        connection.Close()
        'WriteDataSetToFile("test4.csv", _buildDS.Tables(0))
        Return _buildDS
    End Function
    Public Shared Function GetWorkingPipeLineForExcelByBU(ByVal dblBU_ID As Double, Optional ByVal strFilter As String = "", Optional ByVal bExcludeExclusions As Boolean = False) As DataSet
        Dim strSQL As String = "SELECT PK_OpportunityID, qryWorkingPipelineWithWinLoss.OpportunityOwner, " & _
            "qryWorkingPipelineWithWinLoss.Client, " & _
            "qryWorkingPipelineWithWinLoss.OpportunityName, " & _
            "qryWorkingPipelineWithWinLoss.Fit, " & _
            "IIF(qryWorkingPipelineWithWinLoss.Anchor,'Yes','No') as Anchor, " & _
            "qryWorkingPipelineWithWinLoss.Source, " & _
            "qryWorkingPipelineWithWinLoss.[Date Entered], " & _
            "qryWorkingPipelineWithWinLoss.OpportunityCloseDate, " & _
            "DateDiff('d',[Date Entered],Now()) AS DaysOpen, " & _
            "qryWorkingPipelineWithWinLoss.WinPercentage, " & _
            "qryWorkingPipelineWithWinLoss.EstimatedRevenue, " & _
            "qryWorkingPipelineWithWinLoss.WeightedRevenue, RolesNeeded, RFPRequired, RFPLead, RFPRiskAssessment, " & _
            "qryWorkingPipelineWithWinLoss.NextSteps,UpdateDate " & _
            "FROM qryWorkingPipelineWithWinLoss Where BusinessUnit=" & dblBU_ID & " order by Client, OpportunityName;"
        strSQL = "SELECT " & _
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
            "DateDiff('d',[Date Entered],Now()) AS DaysOpen, " & _
            "UpdatePerson, " & _
            "RolesNeeded, RFPRequired, RFPLead, RFPRiskAssessment " & _
            " From qryAllOpportunities where Inactive=False and BusinessUnit=" & dblBU_ID & " "
        If bExcludeExclusions Then
            strSQL &= " and Extension=false "
        End If
        Select Case strFilter
            Case "Un-qualified"
                strSQL &= " and (PK_OpportunityUpdateID is Null or WinPercentage is null) "
            Case "All"
                'as is
            Case "All - Won"
                strSQL &= " and winpercentage=1 "
            Case "All - Lost"
                strSQL &= " and winpercentage=0 "
            Case "Working Pipeline"
                strSQL &= " and winpercentage>0 and winpercentage<1 "
            Case "In Last 30 Days"
                strSQL &= " and datediff('d',[date entered],now)<=30 "
            Case "Needing Updates"
                strSQL &= " and [OpportunityCloseDate]<=#" & DateAdd(DateInterval.Day, 7, Now()) & "# and winpercentage>0 and winpercentage<1 "
            Case "WinLoss"
                strSQL &= " and ((winpercentage>0 and winpercentage<1) or " & _
                "(winpercentage=0 and datediff('d',[UpdateDate],now)<=7) or " & _
                "(winpercentage=1 and datediff('d',[UpdateDate],now)<=7))"
            Case Else
                If Not String.IsNullOrEmpty(strFilter) Then
                    strSQL &= " and " & strFilter
                End If
        End Select
        strSQL &= " Order By Client, OpportunityName"

        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)
        da.SelectCommand = New OleDbCommand(strSQL, connection)
        Try
            da.Fill(ds, "tblOpportunities")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try
        Return ds

    End Function

#End Region
End Class
