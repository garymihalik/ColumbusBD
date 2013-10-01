Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports System.IO

Public Class cFinancial
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    Public Structure ReportMonthData
        Public ReportMonth As Double
        Public ReportMonthName As String
        Public ReportYear As String
        Public MonthStart As Date
        Public MonthEnd As Date
    End Structure
    Public Shared Function GetAccountGroupBookedByBu(ByVal dtStart As Date, ByVal dtEnd As Date, ByVal dblBU_ID As Double, Optional ByVal dblWinPercentage As Double = 1) As DataSet
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
        AddColumn(dsPrime, "PredictedBURevenue", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "PredictedBU1099Costs", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "CostBUVariance", "System.Double", dtBooked.TableName)
        AddColumn(dsPrime, "RevenueBUVariance", "System.Double", dtBooked.TableName)

        'used in the month loop to get booked data
        Dim dsActual, dsPredicted As DataSet 'Gets master raw data of booked information
        Dim ActualRevenue, ForecastedRevenue, Actual1099, Forecasted1099, ForecastedBURevenue, Forecasted1099BU As Double
        Dim tempValue, tempActual, tempPredicted, tempPredictedBU As String

        Dim strBookedShadowSQL As String
        'strBookedShadowSQL = " SELECT tblAccountGroup.AccountGroupID, tblAccountGroup.AccountGroup, tblClients.ClientID, tblClients.Client, tblReportData.MonthlyReportDate, WinPercentage*Sum(tblReportData.Revenue) AS SumOfRevenue, WinPercentage*Sum(tblReportData.Costs) AS SumOfCosts, tblAccountGroup.BusinessUnit " & _
        '" FROM tblAccountGroup LEFT JOIN ((((tblClients LEFT JOIN tblOpportunity ON tblClients.ClientID = tblOpportunity.ClientID) LEFT JOIN tblAssignments ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) LEFT JOIN tblReportData ON tblAssignments.AssignmentID = tblReportData.AssignmentID) LEFT JOIN qrylatestEntry ON tblOpportunity.PK_OpportunityID = qrylatestEntry.FK_OpportunityID) ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID " & _
        '" WHERE tblAssignments.ExcludeFromBURevenueCalculation=False " & _
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
                tempPredicted = Convert.ToString(dsPredicted.Tables(0).Compute("Sum(SumOfRevenue)", "AccountGroupID=" & dr.Item("AccountGroupID") & " and MonthlyReportDate=" & myReportingMonths(x).ReportMonth))
                If IsNumeric(tempPredicted) Then
                    ForecastedRevenue = tempPredicted
                Else
                    ForecastedRevenue = 0
                End If
                drBookedInput.Item("PredictedRevenue") = ForecastedRevenue

                'Calculate Predicted BU Revenue
                tempPredictedBU = Convert.ToString(dsPredicted.Tables(0).Compute("Sum(SumOfRevenue)", "ExcludeFromBURevenueCalculation=false and AccountGroupID=" & dr.Item("AccountGroupID") & " and MonthlyReportDate=" & myReportingMonths(x).ReportMonth))
                If IsNumeric(tempPredictedBU) Then
                    ForecastedBURevenue = tempPredictedBU
                Else
                    ForecastedBURevenue = 0
                End If
                drBookedInput.Item("PredictedBURevenue") = ForecastedBURevenue


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
                drBookedInput.Item("Predicted1099Costs") = Forecasted1099

                'Calculate Predicted BU Revenue
                tempPredictedBU = Convert.ToString(dsPredicted.Tables(0).Compute("Sum(SumOfCosts)", "ExcludeFromBURevenueCalculation=false and AccountGroupID=" & dr.Item("AccountGroupID") & " and MonthlyReportDate=" & myReportingMonths(x).ReportMonth))
                If IsNumeric(tempPredictedBU) Then
                    Forecasted1099BU = tempPredictedBU
                Else
                    Forecasted1099BU = 0
                End If
                drBookedInput.Item("PredictedBU1099Costs") = Forecasted1099BU

                If Actual1099 > -1 Then
                    drBookedInput.Item("Booked1099Costs") = Actual1099
                Else
                    drBookedInput.Item("Booked1099Costs") = Forecasted1099
                End If

                'Add columns for Variances
                drBookedInput.Item("RevenueVariance") = IIf(ActualRevenue = -1, 0, ActualRevenue) - ForecastedRevenue
                drBookedInput.Item("CostVariance") = IIf(Actual1099 = -1, 0, Actual1099) - Forecasted1099
                drBookedInput.Item("RevenueBUVariance") = IIf(ActualRevenue = -1, 0, ActualRevenue) - ForecastedBURevenue
                drBookedInput.Item("CostBUVariance") = IIf(Actual1099 = -1, 0, Actual1099) - Forecasted1099BU

                'Now add row to table
                dsPrime.Tables("Booked").Rows.Add(drBookedInput)
            Next
        End While

        dr.Close()

        dsMaster.Tables.Add(dsPrime.Tables("Booked").Copy)
        dsMaster.Tables(0).TableName = "Booked"

        dsMaster.Tables.Add(GetOperatingCostsByBu(myReportingMonths, dsPrime, dblBU_ID).Copy)
        dsMaster.Tables(1).TableName = "OperatingCosts"

        dsMaster.Tables.Add(GetProfitCalculationsByBU(myReportingMonths, dsPrime, dblBU_ID).Copy)
        dsMaster.Tables(2).TableName = "Profit"

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
        For y = 1 To 4 'For the 4 rows
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

    Public Shared Function GetProfitCalculationsByBU(ByVal myReportingMonths() As ReportMonthData, ByVal dsInput As DataSet, ByVal dblBU_ID As Double, Optional ByVal dblWinPercentage As Double = 1) As DataTable
        Dim ds As New DataSet
        Dim dtOC As New DataTable
        Dim myDR As DataRow
        Dim tempValue, tempExcludedData, tempValue1 As String
        dtOC.TableName = "Profit"
        dsInput.Tables.Add(dtOC)
        AddColumn(dsInput, "ReportMonthYear", "System.String", dtOC.TableName)
        AddColumn(dsInput, "Amount", "System.Double", dtOC.TableName)
        AddColumn(dsInput, "LineItem", "System.String", dtOC.TableName)

        Dim dsRawData2, dsExcludedData, dsActualProfit As DataSet 'Gets master raw data of LineItem information
        Dim TotalBookedRevenue, BUBookedRevenue, TotalForecastedRevenue, TotalRevenueVariance As Double
        Dim BUForecastedRevenue, TotalActualRevenue, TotalBUActualRevenue, TotalBURevenueVariance As Double
        Dim OperatingCosts, FTECosts, BizTax As Double
        Dim ForecastedCosts, ActualCosts, BookedCosts, BUProfit1, BUProfit2 As Double

        dsActualProfit = GetSingleDataset("Select * from tblGlobalMonthReporting")
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
        For y = 0 To 22 'For the rows in the chart
            For x = 0 To UBound(myReportingMonths)
                'Reset all to 0
                TotalBookedRevenue = 0 : BUBookedRevenue = 0 : TotalForecastedRevenue = 0 : TotalRevenueVariance = 0
                BUForecastedRevenue = 0 : TotalActualRevenue = 0 : TotalBUActualRevenue = 0 : TotalBURevenueVariance = 0
                OperatingCosts = 0 : FTECosts = 0
                ActualCosts = 0 : ForecastedCosts = 0 : BookedCosts = 0

                myDR = dsInput.Tables("Profit").NewRow
                myDR.Item("ReportMonthYear") = CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear)
                Select Case y
                    Case 0 'Total Booked Revenue
                        myDR.Item("LineItem") = "Total Booked Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Booked)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 1 'Total Forecasted Revenue
                        myDR.Item("LineItem") = "Total Forecasted Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 2 'Total Actual Revenue
                        myDR.Item("LineItem") = "Total Actual Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(ActualRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 3 'Total Revenue Variance (Actual-Forecasted)
                        myDR.Item("LineItem") = "Total Revenue Variance (Actual-Forecasted)"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(RevenueVariance)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 4 'Total Forecasted BU Revenue
                        myDR.Item("LineItem") = "Total BU Forecasted Revenue"
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

                    Case 5 ' Total BU Booked Revenue
                        myDR.Item("LineItem") = "Total BU Booked Revenue"
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

                    Case 6 ' Total Actual BU Revenue
                        myDR.Item("LineItem") = "Total BU Actual Revenue"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(ActualRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 7 ' Total BU Revenue Variance (Actual-Forecasted)
                        myDR.Item("LineItem") = "Total BU Revenue Variance (Actual-Forecasted)"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(ActualRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        tempValue1 = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        tempExcludedData = Convert.ToString(dsExcludedData.Tables(0).Compute("Sum(PredictedRevenue)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not IsNumeric(tempValue) Then tempValue = 0
                        If Not IsNumeric(tempValue1) Then tempValue1 = 0
                        If Not IsNumeric(tempExcludedData) Then tempExcludedData = 0
                        myDR.Item("Amount") = CDbl(tempValue) - (CDbl(tempValue1) - CDbl(tempExcludedData))
                    Case 8 'OperatingCosts
                        myDR.Item("LineItem") = "Operating Costs"
                        tempValue = Convert.ToString(dsInput.Tables("OperatingCosts").Compute("Sum(Amount)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 9 'Booked 1099 Costs
                        myDR.Item("LineItem") = "Booked 1099 Costs"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Booked1099Costs)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 10 'Forecasted 1099 Costs
                        myDR.Item("LineItem") = "Forecasted 1099 Costs"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Predicted1099Costs)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 11 'Actual 1099 Costs
                        myDR.Item("LineItem") = "Actual 1099 Costs"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(Actual1099Costs)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 12 '1099 Cost Variance
                        myDR.Item("LineItem") = "Variance of 1099 Costs (Actual-Forecasted)"
                        tempValue = Convert.ToString(dsInput.Tables("Booked").Compute("Sum(CostVariance)", "ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 13 'Estimated FTE Cost
                        myDR.Item("LineItem") = "Est FTE Sal"
                        tempValue = Convert.ToString(dsRawData2.Tables(0).Compute("Sum(EstFTESal)", "ReportMonth=" & myReportingMonths(x).ReportMonth))
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = tempValue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 14 'BizTax
                        myDR.Item("LineItem") = "Forecasted Business Tax (calculated)"
                        'For now, base it on Forecasted BU Revenue
                        'estimated $900/$100k revenue or .09%=.009
                        'tempValue = Convert.ToString(dsInput.Tables(0).Compute("Sum(PredictedRevenue)", "ReportMonthYear='" & myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear & "'"))
                        tempValue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If Not String.IsNullOrEmpty(tempValue) Then
                            myDR.Item("Amount") = Math.Round(CDbl(tempValue) * 0.009, 2)
                        Else
                            myDR.Item("Amount") = 0
                        End If
                    Case 15 ' Gross Margin $
                        myDR.Item("LineItem") = "Booked Gross Margin $"
                        TotalBookedRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'TotalBUActualRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ForecastedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Booked 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (TotalBookedRevenue - FTECosts - ForecastedCosts)
                    Case 16 'Net Margin
                        myDR.Item("LineItem") = "Booked Net Margin $"
                        tempValue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        tempValue1 = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ForecastedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Booked 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Forecasted Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (tempValue1 - FTECosts - ForecastedCosts - OperatingCosts - BizTax)
                    Case 17 'Gross Margin %
                        myDR.Item("LineItem") = "Booked Gross Margin %"
                        TotalBookedRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'TotalBUActualRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ForecastedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Booked 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If TotalBookedRevenue > 0 Then
                            myDR.Item("Amount") = (TotalBookedRevenue - FTECosts - ForecastedCosts - OperatingCosts) / TotalBookedRevenue
                        Else
                            myDR.Item("Amount") = 0
                        End If

                    Case 18 'Net Margin %
                        myDR.Item("LineItem") = "Booked Net Margin %"
                        TotalBookedRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        TotalBUActualRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ForecastedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Booked 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Forecasted Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        If TotalBookedRevenue > 0 Then
                            myDR.Item("Amount") = (TotalBUActualRevenue - FTECosts - ForecastedCosts - OperatingCosts - BizTax) / TotalBUActualRevenue 'Need to see if it shold be divided by TotalBookedRevenuevs. BUBookedRevenue
                        Else
                            myDR.Item("Amount") = 0
                        End If
                        'Case 18 'Booked Profit:  THIS WILL USE BOOKED REVENUE AND BOOKED COSTS!!!!
                        'myDR.Item("LineItem") = "Calculated Booked Profit"
                        'TotalBookedRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'TotalBUActualRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'ForecastedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Booked 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'They calculate Profit from BUBookedRevenue instead of from Total Revenue
                        'myDR.Item("Amount") = (TotalBookedRevenue - FTECosts - ForecastedCosts - OperatingCosts)
                    Case 19 'Forecasted Profit: THIS WILL USE FORECASTED REVENUE AND FORECASTED COSTS!!!!!
                        myDR.Item("LineItem") = "BU Forecasted Profit"
                        TotalForecastedRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BUForecastedRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Forecasted Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        ForecastedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Forecasted 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Forecasted Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'They calculate Profit from BUBookedRevenue instead of from Total Revenue
                        myDR.Item("Amount") = (BUForecastedRevenue - FTECosts - ForecastedCosts - OperatingCosts - BizTax)
                    Case 20 'BU Calculated Booked Profit using booked revenue, booked costs  '******************
                        myDR.Item("LineItem") = "BU Calculated Booked Profit"
                        tempValue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        tempValue1 = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Booked Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BookedCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Booked 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        BizTax = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Forecasted Business Tax (calculated)' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'Calculate Profit from BUBookedRevenue instead of from Total Revenue
                        myDR.Item("Amount") = (tempValue1 - FTECosts - BookedCosts - OperatingCosts - BizTax)
                    Case 21 'Actual Profit
                        myDR.Item("LineItem") = "BU Actual Profit"
                        'TotalActualRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total Actual Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'TotalBUActualRevenue = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Total BU Actual Revenue' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'FTECosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Est FTE Sal' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'ActualCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Actual 1099 Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'OperatingCosts = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='Operating Costs' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        'They calculate Profit from BUBookedRevenue instead of from Total Revenue
                        'myDR.Item("Amount") = (TotalBUActualRevenue - FTECosts - ActualCosts - OperatingCosts)

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

                    Case 22 'Profit Variance
                        myDR.Item("LineItem") = "BU Profit Variance (Actual - Forecast)"
                        tempValue = Convert.ToString(dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Actual Profit' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'"))
                        If IsNumeric(tempValue) Then
                            BUProfit1 = CDbl(tempValue)
                        Else
                            BUProfit1 = 0
                        End If
                        BUProfit2 = dsInput.Tables(dtOC.TableName).Compute("Sum(Amount)", "LineItem='BU Forecasted Profit' and ReportMonthYear='" & CStr(myReportingMonths(x).ReportMonthName & " " & myReportingMonths(x).ReportYear) & "'")
                        myDR.Item("Amount") = (BUProfit1 - BUProfit2)
                End Select
                dsInput.Tables(dtOC.TableName).Rows.Add(myDR)
            Next
        Next
        'cDataSetManipulation.WriteDataSetToFile("profit.csv", dsInput.Tables(dtOC.TableName))
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

End Class
