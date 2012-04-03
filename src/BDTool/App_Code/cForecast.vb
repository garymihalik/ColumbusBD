Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports System.IO
Public Class cForecast
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    Public Structure ReportMonthData
        Public ReportMonth As Double
        Public ReportMonthName As String
        Public ReportYear As String
        Public MonthStart As Date
        Public MonthEnd As Date
    End Structure
    Public Shared Function GetAccountGroupBookedByBu(ByVal dtStart As Date, ByVal dtEnd As Date, ByVal dblBU_ID As Double, Optional ByVal dblWinPercentage As Double = 1, Optional ByVal bForecast As Boolean = True) As DataSet
        'Get Accounting Groups
        Dim strSQL As String = "SELECT AccountGroup,AccountGroupID,AccountOwner, ForecastOwner FROM tblAccountGroup WHERE ReportOn=True and BusinessUnit=" & dblBU_ID & " ORDER BY tblAccountGroup.ReportOrder"
        Dim connection As New OleDbConnection(conString)
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

        'Get Reporting TimeFrame
        Dim myReportingMonths() As ReportMonthData = GetReportingMonths(dtStart, dtEnd)

        'Loop through each account group and each reporting month!
        Dim strTempReturnValue As String = ""
        Dim BookedRevenue As Double = 0

        'Build TempTable for Usage
        Dim dsPrime, dsMaster As New DataSet

        'This is booked data storage
        Dim dtBooked As New DataTable
        dtBooked.TableName = "Booked"
        dsPrime.Tables.Add(dtBooked)
        AddColumn(dsPrime, "AccountGroupID", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "AccountGroup", "System.String", dtBooked.TableName)
        AddColumn(dsPrime, "AccountOwner", "System.String", dtBooked.TableName)
        AddColumn(dsPrime, "ForecastOwner", "System.String", dtBooked.TableName)
        AddColumn(dsPrime, "ReportMonthYear", "System.String", dtBooked.TableName)
        AddColumn(dsPrime, "Booked", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "ActualRevenue", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "PredictedRevenue", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "RevenueVariance", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "Actual1099Costs", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "Predicted1099Costs", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "Booked1099Costs", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "CostVariance", "System.Double", dtBooked.TableName)

        'used in the month loop to get booked data
        Dim dsActual, dsPredicted As DataSet 'Gets master raw data of booked information
        Dim ActualRevenue, ForecastedRevenue, Actual1099, Forecasted1099 As Double
        Dim tempValue, tempActual, tempPredicted As String

        Dim strBookedShadowSQL As String
        'strBookedShadowSQL = " SELECT tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate, WinPercentage*Sum(tblReportData.Revenue) AS SumOfRevenue, WinPercentage*Sum(tblReportData.Costs) AS SumOfCosts, tblAccountGroup.BusinessUnit " & _
        '" FROM tblAccountGroup LEFT JOIN ((((tblClients LEFT JOIN tblOpportunity ON tblClients.ClientID = tblOpportunity.ClientID) LEFT JOIN tblAssignments ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) LEFT JOIN tblReportData ON tblAssignments.AssignmentID = tblReportData.AssignmentID) LEFT JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID " & _
        '" GROUP BY tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate, tblAccountGroup.BusinessUnit, qrylatestEntry.WinPercentage " & _
        '" HAVING (((tblAccountGroup.BusinessUnit)=" & dblBU_ID & ") AND ((qrylatestEntry.WinPercentage)>=" & dblWinPercentage & ")); "
        strBookedShadowSQL = "Select * from qry_cFinancial_strBookedShadowSQL where BusinessUnit=" & dblBU_ID & " and WinPercentage>=" & dblWinPercentage

        Dim strActualSQL As String
        strActualSQL = " SELECT tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblActuals.ReportMonth, Sum(tblActuals.ActualRevenue) AS SumOfActualRevenue, Sum(tblActuals.ActualCost) AS SumOfActualCost, tblAccountGroup.BusinessUnit " & _
        " FROM tblAccountGroup INNER JOIN (tblClients INNER JOIN tblActuals ON tblClients.ClientID = tblActuals.ClientID) ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID " & _
        " GROUP BY tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblActuals.ReportMonth, tblAccountGroup.BusinessUnit" & _
        " HAVING tblAccountGroup.BusinessUnit=" & dblBU_ID

        dsActual = GetSingleDataset(strActualSQL)
        dsPredicted = GetSingleDataset(strBookedShadowSQL)

        While dr.Read
            For x = 0 To UBound(myReportingMonths)
                'Booked
                ActualRevenue = 0 : ForecastedRevenue = 0 : Actual1099 = 0 : Forecasted1099 = 0
                Dim drBookedInput As DataRow = dsPrime.Tables("Booked").NewRow
                drBookedInput.Item("AccountGroupID") = dr.Item("AccountGroupID")
                drBookedInput.Item("AccountGroup") = dr.Item("AccountGroup")
                drBookedInput.Item("AccountOwner") = dr.Item("AccountOwner")
                drBookedInput.Item("ForecastOwner") = dr.Item("ForecastOwner")
                drBookedInput.Item("ReportMonthYear") = CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear)

                '************************New Code
                'I'm going to query actuals separate and apart from predicted instead of via a Union Query.
                'My logic is, if there are actual records, then use the actual sum as the booked.
                'Otherwise, use the predicted values as the booked.  If predicted is null, then use a value of 0

                'Okay, to use the actual, I need a count
                tempActual = Convert.ToString(dsActual.Tables(0).Compute("Sum(SumOfActualRevenue)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and ReportMonth=" & myReportingMonths(x).ReportMonth))
                If IsNumeric(tempActual) Then
                    If CDbl(tempActual) = 0 Then
                        'Since actual is 0, I need to see if it is 0 because there are 0 records.  If so, I need to null it out
                        'otherwise I'll use it.
                        tempValue = Convert.ToString(dsActual.Tables(0).Compute("Count(clientid)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If IsNumeric(tempValue) Then
                            If CDbl(tempValue) > 0 Then
                                'Both sum is 0 and count is >0, so I have actuals of 0
                                ActualRevenue = 0
                                drBookedInput.Item("ActualRevenue") = 0
                            Else
                                'I'll use -1 to flag as a null and to ignore in booking calculations.
                                ActualRevenue = -1
                            End If
                        Else
                            'no records to count against, so "fake" null it.
                            ActualRevenue = -1
                        End If
                    Else
                        'Since Actual is >0, use it.
                        ActualRevenue = tempActual
                        drBookedInput.Item("ActualRevenue") = ActualRevenue
                    End If
                Else
                    'No records to sum, so "fake" null it.
                    ActualRevenue = -1
                End If

                'tempPredicted will contain a value, including 0.  If it is null, then the sum function will null it out.
				tempPredicted = Convert.ToString(dsPredicted.Tables(0).Compute("Sum(SumOfRevenue)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and MonthlyReportDate=" & myReportingMonths(x).ReportMonth & " and ExcludeFromBURevenueCalculation = False"))
                If IsNumeric(tempPredicted) Then
                    ForecastedRevenue = tempPredicted
                Else
                    ForecastedRevenue = 0
                End If
                drBookedInput.Item("PredictedRevenue") = ForecastedRevenue

                If ActualRevenue > -1 Then
                    drBookedInput.Item("Booked") = ActualRevenue
                Else
                    drBookedInput.Item("Booked") = ForecastedRevenue
                End If
                '***************************************************************
                'Okay, now do 1099 Costs! same approach
                tempActual = Convert.ToString(dsActual.Tables(0).Compute("Sum(SumOfActualCost)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and ReportMonth=" & myReportingMonths(x).ReportMonth))
                If IsNumeric(tempActual) Then
                    If CDbl(tempActual) = 0 Then
                        'Since actual is 0, I need to see if it is 0 because there are 0 records.  If so, I need to null it out
                        'otherwise I'll use it.
                        tempValue = Convert.ToString(dsActual.Tables(0).Compute("Count(clientid)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If IsNumeric(tempValue) Then
                            If CDbl(tempValue) > 0 Then
                                'Both sum is 0 and count is >0, so I have actuals of 0
                                Actual1099 = 0
                                drBookedInput.Item("Actual1099Costs") = 0
                            Else
                                'I'll use -1 to flag as a null and to ignore in booking calculations.
                                Actual1099 = -1
                            End If
                        Else
                            'no records to count against, so "fake" null it.
                            Actual1099 = -1
                        End If
                    Else
                        'Since Actual is >0, use it.
                        Actual1099 = tempActual
                        drBookedInput.Item("Actual1099Costs") = Actual1099
                    End If
                Else
                    'No records to sum, so "fake" null it.
                    Actual1099 = -1
                End If

                'tempPredicted will contain a value, including 0.  If it is null, then the sum function will null it out.
                tempPredicted = Convert.ToString(dsPredicted.Tables(0).Compute("Sum(SumOfCosts)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and MonthlyReportDate=" & myReportingMonths(x).ReportMonth))
                If IsNumeric(tempPredicted) Then
                    Forecasted1099 = tempPredicted
                Else
                    Forecasted1099 = 0
                End If

                If Actual1099 > -1 Then
                    'drBookedInput.Item("Booked1099Costs") = Actual1099
                    'Because no one is tracking 1099 costs yet, just use the forecasted
                    drBookedInput.Item("Booked1099Costs") = Actual1099
                Else
                    drBookedInput.Item("Booked1099Costs") = Forecasted1099
                End If
                drBookedInput.Item("Predicted1099Costs") = Forecasted1099
                'Add columns for Variances
                drBookedInput.Item("RevenueVariance") = IIf(ActualRevenue = -1, 0, ActualRevenue) - ForecastedRevenue
                drBookedInput.Item("CostVariance") = IIf(Actual1099 = -1, 0, Actual1099) - Forecasted1099

                'Now add row to table
                dsPrime.Tables("Booked").Rows.Add(drBookedInput)
            Next
        End While

        dr.Close()

        dsMaster.Tables.Add(dsPrime.Tables("Booked").Copy)
        dsMaster.Tables(0).TableName = "Booked"

        dsMaster.Tables.Add(GetOperatingCostsByBu(myReportingMonths, dsPrime, dblBU_ID).Copy)
        dsMaster.Tables(1).TableName = "OperatingCosts"

        If bForecast Then
            dsMaster.Tables.Add(GetForecastedProfitCalculationsByBU(myReportingMonths, dsPrime, dblBU_ID).Copy)
            dsMaster.Tables(2).TableName = "Profit"
        Else
            dsMaster.Tables.Add(GetBookedProfitCalculationsByBU(myReportingMonths, dsPrime, dblBU_ID).Copy)
            dsMaster.Tables(2).TableName = "Profit"
        End If
        
        Return dsMaster
    End Function

    Public Shared Function GetOperatingCostsByBu(ByVal myReportingMonths() As ReportMonthData, ByVal dsInput As DataSet, ByVal dblBU_ID As Double) As DataTable
        Dim ds As New DataSet
        Dim dtOC As New DataTable
        Dim drOCInput As DataRow
        Dim tempValue As String
        dtOC.TableName = "OperatingCosts"
        dsInput.Tables.Add(dtOC)
        AddColumn(dsInput, "ReportMonthYear", "System.String", dtOC.TableName)
        AddColumn(dsInput, "Amount", "System.Double", dtOC.TableName)
        AddColumn(dsInput, "Operating Costs", "System.String", dtOC.TableName)

        Dim dsRawData As DataSet 'Gets master raw data of operating costs information
        Dim Expenses, TransfersInOut, MiscCOGS, ForecastedFTECount, NonSalaryCosts As Double
        dsRawData = GetSingleDataset("SELECT  " & _
"tblGlobalMonthReporting.ReportMonth,  " & _
"tblGlobalMonthReporting.Expenses,  " & _
"tblGlobalMonthReporting.TransfersInOut,  " & _
"tblGlobalMonthReporting.MiscCOGS,  " & _
"tblGlobalMonthReporting.ForecastedFTECount,  " & _
"ForecastedFTECount*AvgFTEComp/12*NonSalaryEmpFactor AS NonSalaryCosts,  " & _
"ForecastedFTECount*AvgFTEComp/12 AS EstFTESal " & _
"FROM  " & _
"tblGlobalAnnualReporting,  " & _
"tblMonthlyReportDates INNER JOIN tblGlobalMonthReporting ON tblMonthlyReportDates.ReportOrder = tblGlobalMonthReporting.ReportMonth " & _
"WHERE (((tblGlobalMonthReporting.[BusinessUnit])=" & dblBU_ID & ")) " & _
"and tblGlobalAnnualReporting.ReportYear=tblMonthlyReportDates.Year ")
        For y = 1 To 4 'For the rows
            For x = 0 To UBound(myReportingMonths)
                Expenses = 0 : TransfersInOut = 0 : MiscCOGS = 0 : ForecastedFTECount = 0 : NonSalaryCosts = 0

                drOCInput = dsInput.Tables("OperatingCosts").NewRow
                drOCInput.Item("ReportMonthYear") = CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear)
                Select Case y
                    Case 0 'FTE Count (Let's nto include this!)
                        'The y=1 prevents this case from running!
                        drOCInput.Item("Operating Costs") = "FTE Count"
                        tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(ForecastedFTECount)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            drOCInput.Item("Amount") = tempValue
                        Else
                            drOCInput.Item("Amount") = 0
                        End If
                    Case 1 'Expenses
                        drOCInput.Item("Operating Costs") = "Expenses"
                        tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(Expenses)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            drOCInput.Item("Amount") = tempValue
                        Else
                            drOCInput.Item("Amount") = 0
                        End If
                    Case 2 'Nat'l Allocation (Calculated)
                        drOCInput.Item("Operating Costs") = "Nat'l Allocation (Calculated)"
                        tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(NonSalaryCosts)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            drOCInput.Item("Amount") = tempValue
                        Else
                            drOCInput.Item("Amount") = 0
                        End If
                    Case 3 'Transfers IN/OUT
                        drOCInput.Item("Operating Costs") = "Transfers IN/OUT"
                        tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(TransfersInOut)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            drOCInput.Item("Amount") = tempValue
                        Else
                            drOCInput.Item("Amount") = 0
                        End If
                    Case 4 'Misc COGS
                        drOCInput.Item("Operating Costs") = "Misc COGS"
                        tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(MiscCOGS)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            drOCInput.Item("Amount") = tempValue
                        Else
                            drOCInput.Item("Amount") = 0
                        End If
                End Select
                dsInput.Tables("OperatingCosts").Rows.Add(drOCInput)
            Next
            'dsInput.Tables("OperatingCosts").Rows.Add(drOCInput)
        Next
        Return (dsInput.Tables("OperatingCosts"))
    End Function

    Public Shared Function GetBookedProfitCalculationsByBU(ByVal myReportingMonths() As ReportMonthData, ByVal dsInput As DataSet, ByVal dblBU_ID As Double, Optional ByVal dblWinPercentage As Double = 1) As DataTable
        Dim ds As New DataSet
        Dim dtOC As New DataTable
        Dim myDR As DataRow
        Dim tempValue, tempExcludedData As String
        dtOC.TableName = "Profit"
        dsInput.Tables.Add(dtOC)
        AddColumn(dsInput, "ReportMonthYear", "System.String", dtOC.TableName)
        AddColumn(dsInput, "Amount", "System.Double", dtOC.TableName)
        AddColumn(dsInput, "LineItem", "System.String", dtOC.TableName)

        Dim dsRawData2, dsExcludedData As DataSet 'Gets master raw data of LineItem information
        Dim dsActualProfit As DataSet 'Gets actual profit, if entered
        Dim TotalRevenue, BURevenue, OperatingCosts, ContractorCosts, ActualCosts, PredictedCosts, FTECosts, ActualProfit, BizTax As Double
        dsRawData2 = GetSingleDataset("SELECT  " & _
"tblGlobalMonthReporting.ReportMonth,  " & _
"tblGlobalMonthReporting.Expenses,  " & _
"tblGlobalMonthReporting.TransfersInOut,  " & _
"tblGlobalMonthReporting.MiscCOGS,  " & _
"tblGlobalMonthReporting.ForecastedFTECount,  " & _
"ForecastedFTECount*AvgFTEComp/12*NonSalaryEmpFactor AS NonSalaryCosts,  " & _
"ForecastedFTECount*AvgFTEComp/12 AS EstFTESal " & _
"FROM  " & _
"tblGlobalAnnualReporting,  " & _
"tblMonthlyReportDates INNER JOIN tblGlobalMonthReporting ON tblMonthlyReportDates.ReportOrder = tblGlobalMonthReporting.ReportMonth " & _
"WHERE (((tblGlobalMonthReporting.[BusinessUnit])=" & dblBU_ID & ")) " & _
"and tblGlobalAnnualReporting.ReportYear=tblMonthlyReportDates.Year ")
        Dim strBookedShadowExclusion As String
        strBookedShadowExclusion = " SELECT tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate AS ReportMonth, WinPercentage*Sum(tblReportData.Revenue) AS PredictedRevenue, WinPercentage*Sum(tblReportData.Costs) AS SumOfCosts, tblAccountGroup.BusinessUnit " & _
        " FROM tblAccountGroup LEFT JOIN ((((tblClients LEFT JOIN tblOpportunity ON tblClients.ClientID = tblOpportunity.ClientID) LEFT JOIN tblAssignments ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) LEFT JOIN tblReportData ON tblAssignments.AssignmentID = tblReportData.AssignmentID) LEFT JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID " & _
        " WHERE (((tblAssignments.ExcludeFromBURevenueCalculation)=True)) " & _
        " GROUP BY tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate, tblAccountGroup.BusinessUnit, qrylatestEntry.WinPercentage " & _
        " HAVING (((tblAccountGroup.BusinessUnit)=" & dblBU_ID & ") AND ((qrylatestEntry.WinPercentage)>=" & dblWinPercentage & "))"

        dsExcludedData = GetSingleDataset(strBookedShadowExclusion)

        dsActualProfit = GetSingleDataset("Select * from tblGlobalMonthReporting")
        For y = 0 To 11 'For the rows
            For x = 0 To UBound(myReportingMonths)
                TotalRevenue = 0 : BURevenue = 0 : OperatingCosts = 0 : ContractorCosts = 0 : ActualCosts = 0
                PredictedCosts = 0 : FTECosts = 0 : ActualProfit = 0
                myDR = dsInput.Tables("Profit").NewRow
                myDR.Item("ReportMonthYear") = CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear)
                Select Case y
                    Case 0 'TotalRevenue
                        'The y=1 prevents this case from running!
                        myDR.Item("LineItem") = "Total Booked Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Booked)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 1 'BU Revenue
                        myDR.Item("LineItem") = "BU Booked Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Booked)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        tempExcludedData = Convert.ToString(dsExcludedData.Tables(0).Compute("Sum(PredictedRevenue)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            'Remove Excluded Revenue from this BU Revenue
                            If Not String.IsNullOrEmpty(tempExcludedData) Then
                                myDR.Item("Amount") = CDbl(tempValue) - CDbl(tempExcludedData)
                            Else
                                myDR.Item("Amount") = CDbl(tempValue)
                            End If
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 2 'OperatingCosts
                        myDR.Item("LineItem") = "Operating Costs"
                        tempValue = Convert.ToString(dsInput.Tables("OperatingCosts").Compute("Sum(Amount)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 3 'ContractorCosts
                        myDR.Item("LineItem") = "1099 Costs"
                        'tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(ActualContractorCosts)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        'If Not String.IsNullOrEmpty(tempValue) Then ActualCosts = tempValue
                        'tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(PredictedCosts)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        'If Not String.IsNullOrEmpty(tempValue) Then PredictedCosts = tempValue
                        'If ActualCosts > 0 Then
                        ' myDR.Item("Amount") = ActualCosts
                        ' Else
                        ' myDR.Item("Amount") = PredictedCosts
                        ' End If
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Booked1099Costs)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 4 'Est FTE Sal
                        myDR.Item("LineItem") = "Est FTE Sal"
                        tempValue = Convert.ToString(dsRawData2.Tables(0).Compute("Sum(EstFTESal)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 5 'Business Tax
                        myDR.Item("LineItem") = "Business Tax (calculated)"
                        'For now, base it on BU Revenue
                        'estimated $900/$100k revenue or .09%=.009
                        'tempValue = Convert.ToString(dsInput.Tables(0).Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear & "'"))
                        tempValue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = Math.Round(CDbl(tempValue) * 0.009, 2)
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 6 ' Gross Margin $
                        myDR.Item("LineItem") = "Gross Margin $"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (TotalRevenue - FTECosts - ContractorCosts)
                    Case 7 'Net Margin
                        myDR.Item("LineItem") = "Net Margin $"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (BURevenue - FTECosts - ContractorCosts - OperatingCosts - BizTax)
                    Case 8 'Gross Margin %
                        myDR.Item("LineItem") = "Gross Margin %"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If TotalRevenue > 0 Then
                            myDR.Item("Amount") = (TotalRevenue - FTECosts - ContractorCosts - OperatingCosts) / TotalRevenue
                        Else
                            myDR.Item("Amount") = 0
                        End If

                    Case 9 'Net Margin %
                        myDR.Item("LineItem") = "Net Margin %"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If TotalRevenue > 0 Then
                            myDR.Item("Amount") = (BURevenue - FTECosts - ContractorCosts - OperatingCosts - BizTax) / BURevenue 'Need to see if it shold be divided by totalrevenue vs. burevenue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 10 'Profit
                        myDR.Item("LineItem") = "BU Calculated Booked Profit"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'They calculate Profit from BURevenue instead of from Total Revenue
                        myDR.Item("Amount") = (BURevenue - FTECosts - ContractorCosts - OperatingCosts - BizTax)
                    Case 11 'Actual Profit
                        myDR.Item("LineItem") = "BU Actual Profit"
                        tempValue = Convert.ToString(dsActualProfit.Tables(0).Compute("Count(ActualProfit)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If CDbl(tempValue) > 0 Then
                            tempValue = Convert.ToString(dsActualProfit.Tables(0).Compute("Sum(ActualProfit)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                            If IsNumeric(tempValue) Then
                                myDR.Item("Amount") = tempValue
                            Else
                                myDR.Item("Amount") = System.DBNull.Value
                            End If
                        Else
                            myDR.Item("Amount") = System.DBNull.Value
                        End If
                End Select
                dsInput.Tables(dtOC.TableName).Rows.Add(myDR)
            Next
        Next
        Return (dsInput.Tables(dtOC.TableName))
    End Function
    Public Shared Function GetForecastedProfitCalculationsByBU(ByVal myReportingMonths() As ReportMonthData, ByVal dsInput As DataSet, ByVal dblBU_ID As Double, Optional ByVal dblWinPercentage As Double = 1) As DataTable
        Dim ds As New DataSet
        Dim dtOC As New DataTable
        Dim myDR As DataRow
        Dim tempValue, tempExcludedData As String
        dtOC.TableName = "Profit"
        dsInput.Tables.Add(dtOC)
        AddColumn(dsInput, "ReportMonthYear", "System.String", dtOC.TableName)
        AddColumn(dsInput, "Amount", "System.Double", dtOC.TableName)
        AddColumn(dsInput, "LineItem", "System.String", dtOC.TableName)

        Dim dsRawData2, dsExcludedData As DataSet 'Gets master raw data of LineItem information
        Dim dsActualProfit As DataSet 'Gets actual profit, if entered
        Dim TotalRevenue, BURevenue, OperatingCosts, ContractorCosts, ActualCosts, PredictedCosts, FTECosts, ActualProfit, BizTax As Double
        dsRawData2 = GetSingleDataset("SELECT  " & _
"tblGlobalMonthReporting.ReportMonth,  " & _
"tblGlobalMonthReporting.Expenses,  " & _
"tblGlobalMonthReporting.TransfersInOut,  " & _
"tblGlobalMonthReporting.MiscCOGS,  " & _
"tblGlobalMonthReporting.ForecastedFTECount,  " & _
"ForecastedFTECount*AvgFTEComp/12*NonSalaryEmpFactor AS NonSalaryCosts,  " & _
"ForecastedFTECount*AvgFTEComp/12 AS EstFTESal " & _
"FROM  " & _
"tblGlobalAnnualReporting,  " & _
"tblMonthlyReportDates INNER JOIN tblGlobalMonthReporting ON tblMonthlyReportDates.ReportOrder = tblGlobalMonthReporting.ReportMonth " & _
"WHERE (((tblGlobalMonthReporting.[BusinessUnit])=" & dblBU_ID & ")) " & _
"and tblGlobalAnnualReporting.ReportYear=tblMonthlyReportDates.Year ")
        Dim strBookedShadowExclusion As String
        strBookedShadowExclusion = " SELECT tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate AS ReportMonth, WinPercentage*Sum(tblReportData.Revenue) AS PredictedRevenue, WinPercentage*Sum(tblReportData.Costs) AS SumOfCosts, tblAccountGroup.BusinessUnit " & _
        " FROM tblAccountGroup LEFT JOIN ((((tblClients LEFT JOIN tblOpportunity ON tblClients.ClientID = tblOpportunity.ClientID) LEFT JOIN tblAssignments ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) LEFT JOIN tblReportData ON tblAssignments.AssignmentID = tblReportData.AssignmentID) LEFT JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID " & _
        " WHERE (((tblAssignments.ExcludeFromBURevenueCalculation)=True)) " & _
        " GROUP BY tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate, tblAccountGroup.BusinessUnit, qrylatestEntry.WinPercentage " & _
        " HAVING (((tblAccountGroup.BusinessUnit)=" & dblBU_ID & ") AND ((qrylatestEntry.WinPercentage)>=" & dblWinPercentage & "))"

        dsExcludedData = GetSingleDataset(strBookedShadowExclusion)

        dsActualProfit = GetSingleDataset("Select * from tblGlobalMonthReporting")
        For y = 0 To 11 'For the rows
            For x = 0 To UBound(myReportingMonths)
                TotalRevenue = 0 : BURevenue = 0 : OperatingCosts = 0 : ContractorCosts = 0 : ActualCosts = 0
                PredictedCosts = 0 : FTECosts = 0 : ActualProfit = 0
                myDR = dsInput.Tables("Profit").NewRow
                myDR.Item("ReportMonthYear") = CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear)
                Select Case y
                    Case 0 'TotalRevenue
                        'The y=1 prevents this case from running!
                        myDR.Item("LineItem") = "Total Forecasted Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 1 'BU Revenue
                        myDR.Item("LineItem") = "BU Forecasted Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        tempExcludedData = Convert.ToString(dsExcludedData.Tables(0).Compute("Sum(PredictedRevenue)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            'Remove Excluded Revenue from this BU Revenue
                            If Not String.IsNullOrEmpty(tempExcludedData) Then
                                myDR.Item("Amount") = CDbl(tempValue) - CDbl(tempExcludedData)
                            Else
                                myDR.Item("Amount") = CDbl(tempValue)
                            End If
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 2 'OperatingCosts
                        myDR.Item("LineItem") = "Operating Costs"
                        tempValue = Convert.ToString(dsInput.Tables("OperatingCosts").Compute("Sum(Amount)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 3 'ContractorCosts
                        myDR.Item("LineItem") = "1099 Costs"
                        'tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(ActualContractorCosts)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        'If Not String.IsNullOrEmpty(tempValue) Then ActualCosts = tempValue
                        'tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(PredictedCosts)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        'If Not String.IsNullOrEmpty(tempValue) Then PredictedCosts = tempValue
                        'If ActualCosts > 0 Then
                        ' myDR.Item("Amount") = ActualCosts
                        ' Else
                        ' myDR.Item("Amount") = PredictedCosts
                        ' End If
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Booked1099Costs)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 4 'Est FTE Sal
                        myDR.Item("LineItem") = "Est FTE Sal"
                        tempValue = Convert.ToString(dsRawData2.Tables(0).Compute("Sum(EstFTESal)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 5 'Business Tax
                        myDR.Item("LineItem") = "Business Tax (calculated)"
                        'For now, base it on BU Revenue
                        'estimated $900/$100k revenue or .09%=.0009
                        'tempValue = Convert.ToString(dsInput.Tables(0).Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear & "'"))
                        tempValue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = Math.Round(CDbl(tempValue) * 0.009, 2)
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 6 ' Gross Margin $
                        myDR.Item("LineItem") = "Gross Margin $"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (TotalRevenue - FTECosts - ContractorCosts)
                    Case 7 'Net Margin
                        myDR.Item("LineItem") = "Net Margin $"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (BURevenue - FTECosts - ContractorCosts - OperatingCosts - BizTax)
                    Case 8 'Gross Margin %
                        myDR.Item("LineItem") = "Gross Margin %"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If TotalRevenue > 0 Then
                            myDR.Item("Amount") = (TotalRevenue - FTECosts - ContractorCosts - OperatingCosts) / TotalRevenue
                        Else
                            myDR.Item("Amount") = 0
                        End If

                    Case 9 'Net Margin %
                        myDR.Item("LineItem") = "Net Margin %"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If TotalRevenue > 0 Then
                            myDR.Item("Amount") = (BURevenue - FTECosts - ContractorCosts - OperatingCosts - BizTax) / BURevenue 'Need to see if it shold be divided by totalrevenue vs. burevenue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 10 'Profit
                        myDR.Item("LineItem") = "BU Forecasted Profit"
                        TotalRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BURevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ContractorCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'They calculate Profit from BURevenue instead of from Total Revenue
                        myDR.Item("Amount") = (BURevenue - FTECosts - ContractorCosts - OperatingCosts - BizTax)
                    Case 11 'Actual Profit
                        myDR.Item("LineItem") = "BU Actual Profit"
                        tempValue = Convert.ToString(dsActualProfit.Tables(0).Compute("Count(ActualProfit)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If CDbl(tempValue) > 0 Then
                            tempValue = Convert.ToString(dsActualProfit.Tables(0).Compute("Sum(ActualProfit)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                            If IsNumeric(tempValue) Then
                                myDR.Item("Amount") = tempValue
                            Else
                                myDR.Item("Amount") = System.DBNull.Value
                            End If
                        Else
                            myDR.Item("Amount") = System.DBNull.Value
                        End If
                End Select
                dsInput.Tables(dtOC.TableName).Rows.Add(myDR)
            Next
        Next
        Return (dsInput.Tables(dtOC.TableName))
    End Function
    Public Shared Function GetReportingMonths(ByVal StartDate As Date, ByVal EndDate As Date) As ReportMonthData()
        Dim meReportMonthData(0) As ReportMonthData
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
        Dim x As Integer = 0
        While dr.Read
            ReDim Preserve meReportMonthData(UBound(meReportMonthData) + 1)
            meReportMonthData(x).ReportMonth = dr.Item("ReportOrder")
            meReportMonthData(x).ReportMonthName = dr.Item("MontlyReportDates")
            meReportMonthData(x).ReportYear = dr.Item("Year")
            meReportMonthData(x).MonthStart = dr.Item("StartDate")
            meReportMonthData(x).MonthEnd = dr.Item("EndDate")
            x += 1
        End While
        ReDim Preserve meReportMonthData(UBound(meReportMonthData) - 1)
        Return meReportMonthData
    End Function
    Public Shared Function GetReportMonths() As DataSet
        Dim da As New OleDbDataAdapter()
        Dim ds As DataSet = New DataSet()
        Dim connection As New OleDbConnection(conString)

        da.SelectCommand = New OleDbCommand("Select * from tblMonthlyReportDates order by startdate", connection)
        Try
            da.Fill(ds, "tblMonthlyReportDates")
        Catch e As OleDbException
            ' Handle exception.
            Throw New ArgumentException(e.ToString)
        Finally
            connection.Close()
        End Try

        If ds.Tables("tblMonthlyReportDates") IsNot Nothing Then
            If ds.Tables(0).Rows.Count = 0 Then
                'This section adds a fake row to teh dataset with a predetermined
                'fake value for hte primary key field.  In this case, I am using -1
                'On the pages taht use this datasourceclass, I check for -1 and ignore
                'spurious actions like delete and edit.
                Dim dr As DataRow = ds.Tables(0).Rows.Add
                dr.Item("ReportOrder") = -1
            End If
            Return ds
        Else
            Return Nothing
        End If
    End Function
    Public Shared Function GetPipelineVsBookedByBu(ByVal dtStart As Date, ByVal dtEnd As Date, ByVal dblBU_ID As Double) As DataSet
        'Get Reporting TimeFrame
        Dim myReportingMonths() As ReportMonthData = GetReportingMonths(dtStart, dtEnd)

        'Build TempTable for Usage
        Dim dsMaster As New DataSet
        Dim dtCompare As New DataTable
        dtCompare.TableName = "PipelineVsBooked"
        dsMaster.Tables.Add(dtCompare)
        AddColumn(dsMaster, "ReportMonthYear", "System.String", dtCompare.TableName)
        AddColumn(dsMaster, "Pipeline", "System.Double", dtCompare.TableName)
        AddColumn(dsMaster, "Booked", "System.Double", dtCompare.TableName)
        AddColumn(dsMaster, "RunningPipeline", "System.Double", dtCompare.TableName)
        AddColumn(dsMaster, "RunningBooked", "System.Double", dtCompare.TableName)
        'Get Raw Data
        Dim dsRawData As DataSet = GetSingleDataset("SELECT qryAllOpportunities.Client, qryAllOpportunities.OpportunityName, qryAllOpportunities.WinPercentage, " & _
                " qryAllOpportunities.[Date Entered], qryAllOpportunities.OpportunityCloseDate, qryAllOpportunities.EstimatedRevenue, " & _
                " (select sum (calculatedRevenue) from tblAssignments where FK_OpportunityID=PK_OpportunityID) AS BookedRevenue" & _
                " FROM qryAllOpportunities " & _
                " WHERE qryAllOpportunities.WinPercentage=1 and BusinessUnit=" & dblBU_ID)

        Dim tempValue As String

        For x = 0 To UBound(myReportingMonths)
            'create row
            Dim drInput As DataRow = dsMaster.Tables(dtCompare.TableName).NewRow
            'add vales to row
            drInput.Item("ReportMonthYear") = CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear)

            tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(EstimatedRevenue)", "OpportunityCloseDate>=#" & myReportingMonths(x).MonthStart & "# and OpportunityCloseDate<=#" & myReportingMonths(x).MonthEnd & "#"))
            If String.IsNullOrEmpty(tempValue) Then tempValue = 0
            drInput.Item("Pipeline") = tempValue

            tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(BookedRevenue)", "OpportunityCloseDate>=#" & myReportingMonths(x).MonthStart & "# and OpportunityCloseDate<=#" & myReportingMonths(x).MonthEnd & "#"))
            If String.IsNullOrEmpty(tempValue) Then tempValue = 0
            drInput.Item("Booked") = tempValue


            tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(EstimatedRevenue)", "OpportunityCloseDate>=#" & myReportingMonths(0).MonthStart & "# and OpportunityCloseDate<=#" & myReportingMonths(x).MonthEnd & "#"))
            If String.IsNullOrEmpty(tempValue) Then tempValue = 0
            drInput.Item("RunningPipeline") = tempValue

            tempValue = Convert.ToString(dsRawData.Tables(0).Compute("Sum(BookedRevenue)", "OpportunityCloseDate>=#" & myReportingMonths(0).MonthStart & "# and OpportunityCloseDate<=#" & myReportingMonths(x).MonthEnd & "#"))
            If String.IsNullOrEmpty(tempValue) Then tempValue = 0
            drInput.Item("RunningBooked") = tempValue


            'add row to dataset
            dsMaster.Tables(dtCompare.TableName).Rows.Add(drInput)
        Next


        Return dsMaster
    End Function
    Public Shared Function GetBookedWithoutAssignments(ByVal dtStart As Date, ByVal dtEnd As Date) As DataSet
        Dim dsRawData As DataSet = GetSingleDataset(" SELECT qryAllOpportunities.Client, qryAllOpportunities.OpportunityName, qryAllOpportunities.WinPercentage, qryAllOpportunities.[Date Entered] as DateEntered, " & _
                " qryAllOpportunities.OpportunityCloseDate, qryAllOpportunities.EstimatedRevenue, qryAllOpportunities.PK_OpportunityID, qryAllOpportunities.OpportunityOwner  " & _
                " FROM qryAllOpportunities " & _
                " WHERE ((((select count(1) from tblAssignments where FK_OpportunityID=PK_OpportunityID))=0) AND ((qryAllOpportunities.WinPercentage)>=.7)) and OpportunityCloseDate>=#" & dtStart & "# and OpportunityCloseDate<=#" & dtEnd & "#")
        Return dsRawData
    End Function
End Class
