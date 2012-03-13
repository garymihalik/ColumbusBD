Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Data.OleDb
Imports System.ComponentModel
Imports cCommon
Imports cDataSetManipulation
Imports cForecast
Imports System.IO
Public Class cAccountGroupForecast
    Public Shared conString As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/"), ConfigurationManager.AppSettings("DBConnection"))
    Public Structure RunningTotals
        Public ReportMonth As Double
        Public ReportMonthName As String
        Public ReportYear As String
        Public Revenue As Double
        Public Cost1099 As Double
        Public Actual As Double
    End Structure
    Private Shared Function CreateTotalDataset() As DataSet
        Dim dt As New DataTable
        dt.TableName = "ClientTotals"
        Dim ds As New DataSet
        ds.Tables.Add(dt)
        AddColumn(ds, "ReportMonth", "System.Double", dt.TableName) 'This is psuedokey
        AddColumn(ds, "ReportMonthName", "System.String", dt.TableName)
        AddColumn(ds, "ReportMonthYear", "System.String", dt.TableName)
        AddColumn(ds, "Forecast", "System.Double", dt.TableName)
        AddColumn(ds, "Cost1099", "System.Double", dt.TableName)
        AddColumn(ds, "Actual", "System.Double", dt.TableName)
        Return ds
    End Function
    Public Shared Function GetAccountGroupForecast(ByVal dtStart As Date, ByVal dtEnd As Date, ByVal dblAccountGroupID As Double) As PlaceHolder
        Dim _pnl As New Panel 'My internal panel to add items
        Dim trStart As LiteralControl
        Dim trEnd As LiteralControl
        Dim tdStartNorm As String
        Dim tdLeft, tdRight, tdMid, tdEnd As String
        tdLeft = "<td class=""left"">"
        tdRight = "<td class=""right"">"
        tdMid = "<td class=""mid"">"
        tdEnd = "</td>"
        Dim _Placeholder As New PlaceHolder

        'Get Reporting TimeFrame
        Dim myReportingMonths() As ReportMonthData = GetReportingMonths(dtStart, dtEnd)
        'This is the resource side of the dataset
        Dim _dsResources As DataSet = GetSingleDataset("SELECT tblAccountGroup.AccountGroupID, tblClients.Client, tblOpportunity.OpportunityName, tblResources.FirstName, tblResources.LastName, tblAssignments.BillRate, tblAssignments.Costs AS 1099Costs, AssignmentID FROM tblResources INNER JOIN (tblAccountGroup INNER JOIN (tblClients INNER JOIN (tblOpportunity INNER JOIN tblAssignments ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID) ON tblClients.ClientID = tblOpportunity.ClientID) ON tblAccountGroup.AccountGroupID = tblClients.AccountGroupID) ON tblResources.PK_PersonID = tblAssignments.FK_PersonID WHERE tblAccountGroup.AccountGroupID=" & dblAccountGroupID & _
                                                       " and ((startdate>=#" & myReportingMonths.First.MonthStart & "# and startdate<=#" & myReportingMonths.Last.MonthEnd & "#) or " & _
                                                       " (startdate<#" & myReportingMonths.First.MonthStart & "# and enddate>#" & myReportingMonths.First.MonthStart & "#)) " & _
                                                       " order by FirstName, LastName, StartDate;")
        Dim _AssignmentList As String = ""
        Dim _dsData As DataSet = GetSingleDataset("Select * from tblReportData")

        Dim colCount As Integer = (UBound(myReportingMonths) * 3) + 5
        'Set Up Spans
        tdStartNorm = "<td> "

        Dim intCounter As Integer = 0 'RowCounter!
        '********************Header Row**********************
        If intCounter Mod 2 <> 0 Then
            trStart = New LiteralControl("<tr class=""myNorm"">")
        Else
            trStart = New LiteralControl("<tr class=""myAlt"">")
        End If
        _Placeholder.Controls.Add(New LiteralControl("<table>"))
        _Placeholder.Controls.Add(trStart)
        'Add Header Rows!
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Client" & "</th>"))
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Opportunity" & "</th>"))
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Resource" & "</th>"))
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Bill Rate" & "</th>"))
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "1099 Cost" & "</th>"))
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Period Summed" & "</th>"))
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Opp Win %" & "</th>"))
        For y = 0 To UBound(myReportingMonths)
            _Placeholder.Controls.Add(New LiteralControl("<th colspan=3>" & myReportingMonths(y).ReportMonthName & " " & myReportingMonths(y).ReportYear & "</th>"))
        Next
        trEnd = New LiteralControl("</tr>") 'End Table Row
        _Placeholder.Controls.Add(trEnd)
        intCounter += 1 'Next Row!
        '**************************Establish Object to track totals******************
        'Dim myTotals2 As New List(Of RunningTotals)
        Dim newDS As DataSet = CreateTotalDataset()


        Dim myTotals() As RunningTotals 'Used to keep track of totals
        ReDim Preserve myTotals(UBound(myReportingMonths) + 1)
        For y = 0 To UBound(myReportingMonths)
            Dim drInput As DataRow = newDS.Tables(0).NewRow
            drInput.Item("ReportMonth") = myReportingMonths(y).ReportMonth
            drInput.Item("ReportMonthName") = myReportingMonths(y).ReportMonthName
            drInput.Item("ReportMonthYear") = myReportingMonths(y).ReportYear
            drInput.Item("Forecast") = 0
            drInput.Item("Cost1099") = 0
            drInput.Item("Actual") = 0
            newDS.Tables(0).Rows.Add(drInput)
        Next
        '**************************Data Rows********************
        For Each dr In _dsResources.Tables(0).Rows
            _AssignmentList &= "," & dr.item("AssignmentID")
            'Gets Win % for each assignment
            Dim strWinPercentage As String = cDataSetManipulation.GetSingleSQLValue("Select winpercentage from qryLatestEntry where FK_opportunityID=(Select FK_opportunityID from tblAssignments where assignmentID=" & dr.Item("AssignmentID") & ")")
            If String.IsNullOrEmpty(strWinPercentage) Then
                strWinPercentage = 0
            End If
            'Temporarily stores revenue and cost values
            Dim dblRevenue As Double = 0
            Dim dblCost As Double = 0

            'Beginning of the row
            If intCounter Mod 2 <> 0 Then
                trStart = New LiteralControl("<tr class=""myNorm"">")
            Else
                trStart = New LiteralControl("<tr class=""myAlt"">")
            End If
            'start table row!
            _Placeholder.Controls.Add((trStart))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & dr.Item("Client") & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & dr.Item("OpportunityName") & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & dr.Item("FirstName") & " " & dr.Item("LastName") & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & String.Format("{0:c2}", dr.Item("BilLRate")) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & String.Format("{0:c2}", dr.Item("1099Costs")) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Revenue)", "AssignmentID=" & dr.Item("AssignmentID") & " and MonthlyReportDate>=" & myReportingMonths.First.ReportMonth & " and MonthlyReportDate<=" & myReportingMonths.Last.ReportMonth)))) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & String.Format("{0:p2}", ReturnNumber(Convert.ToString(strWinPercentage))) & tdEnd))
            For y = 0 To UBound(myReportingMonths)
                dblRevenue = CDbl(strWinPercentage) * ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Revenue)", "AssignmentID=" & dr.Item("AssignmentID") & " and MonthlyReportDate=" & myReportingMonths(y).ReportMonth)))
                dblCost = CDbl(strWinPercentage) * ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Costs)", "AssignmentID=" & dr.Item("AssignmentID") & " and MonthlyReportDate=" & myReportingMonths(y).ReportMonth)))
                'Store sums into dataset
                Dim dv As DataView = newDS.Tables(0).DefaultView
                dv.Sort = "ReportMonth"
                Dim I As Integer = dv.Find(myReportingMonths(y).ReportMonth)
                dv.Table.Rows(I).Item("Forecast") += dblRevenue
                dv.Table.Rows(I).Item("Cost1099") += dblCost

                'Write Utlizaiton %, Revenue, and Costs to the display
                _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:p0}", ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(PeriodUtilization)", "AssignmentID=" & dr.Item("AssignmentID") & " and MonthlyReportDate=" & myReportingMonths(y).ReportMonth)))) & tdEnd))
                _Placeholder.Controls.Add(New LiteralControl(tdMid & String.Format("{0:c0}", dblRevenue) & tdEnd))
                _Placeholder.Controls.Add(New LiteralControl(tdRight & String.Format("{0:c0}", dblCost) & tdEnd))
            Next
            trEnd = New LiteralControl("</tr>") 'End Table Row
            _Placeholder.Controls.Add(trEnd)
            intCounter += 1
        Next
        '**************************New Varables for 3 spread columns****************
        'Add Spacer Row
        _Placeholder.Controls.Add(New LiteralControl("<tr class=""myAlt""><td colspan=""" & 6 + (3 * UBound(myReportingMonths)) & """>&nbsp;</td>"))
        Dim td2, td3 As String
        td2 = "<td colspan=2>"
        td3 = "<td colspan=3>"
        '********************************Get Actuals********************************
        If Not String.IsNullOrEmpty(_AssignmentList) Then _AssignmentList = Right(_AssignmentList, Len(_AssignmentList) - 1)
        'Dim _dsActuals As DataSet = GetSingleDataset("Select * from tblActuals where clientid in (SELECT distinct(ClientID) FROM tblOpportunity INNER JOIN tblAssignments ON tblOpportunity.PK_OpportunityID = tblAssignments.FK_OpportunityID where AssignmentID in (" & _AssignmentList & "))")
        Dim _dsActuals As DataSet = GetSingleDataset("Select * from tblActuals where clientid in (SELECT distinct(ClientID) FROM tblClients where AccountGroupID=" & dblAccountGroupID & ")")
        '********************************Monthly Booked Total Row*********************

        _Placeholder.Controls.Add(New LiteralControl("<tr class=""myAlt"">"))
        _Placeholder.Controls.Add(New LiteralControl("<td colspan=5>Booked Total" & tdEnd))
        'Booked Total by Period
        If Not String.IsNullOrEmpty(_AssignmentList) Then
            'This is the old way where the bug exists 
            '_Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Revenue)", "AssignmentID in (" & _AssignmentList & ") and MonthlyReportDate>=" & myReportingMonths.First.ReportMonth & " and MonthlyReportDate<=" & myReportingMonths.Last.ReportMonth)))) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", newDS.Tables(0).Compute("Sum(Forecast)", "1=1")) & tdEnd))
        Else
            _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", 0)))
        End If
        'Booked Total by Month
        If Not String.IsNullOrEmpty(_AssignmentList) Then
            For y = 0 To UBound(myReportingMonths)
                '_Placeholder.Controls.Add(New LiteralControl(td3 & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Revenue)", "AssignmentID in (" & _AssignmentList & ") and MonthlyReportDate=" & myReportingMonths(y).ReportMonth)))) & tdEnd))
                _Placeholder.Controls.Add(New LiteralControl(td3 & String.Format("{0:c0}", ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Forecast)", "ReportMonth=" & myReportingMonths(y).ReportMonth)))) & tdEnd))
            Next
        End If
        _Placeholder.Controls.Add(New LiteralControl("</tr>"))


        '********************************Monthly Actual Total Row*********************
        _Placeholder.Controls.Add(New LiteralControl("<tr class=""myAlt"">"))
        _Placeholder.Controls.Add(New LiteralControl("<td colspan=5>Actual" & tdEnd))
        _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsActuals.Tables(0).Compute("Sum(ActualRevenue)", "ReportMonth>=" & myReportingMonths.First.ReportMonth & " and ReportMonth<=" & myReportingMonths.Last.ReportMonth)))) & tdEnd))
        If Not String.IsNullOrEmpty(_AssignmentList) Then
            Dim dv As DataView = newDS.Tables(0).DefaultView
            dv.Sort = "ReportMonth"
            
            For y = 0 To UBound(myReportingMonths)
                Dim dblActuals As Double = ReturnNumber(Convert.ToString(_dsActuals.Tables(0).Compute("Sum(ActualRevenue)", "ReportMonth=" & myReportingMonths(y).ReportMonth)))
                Dim I As Integer = dv.Find(myReportingMonths(y).ReportMonth)
                dv.Table.Rows(I).Item("Actual") = dblActuals
                '_Placeholder.Controls.Add(New LiteralControl(td3 & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsActuals.Tables(0).Compute("Sum(ActualRevenue)", "ReportMonth=" & myReportingMonths(y).ReportMonth)))) & tdEnd))
                _Placeholder.Controls.Add(New LiteralControl(td3 & String.Format("{0:c0}", newDS.Tables(0).Compute("Sum(Actual)", "ReportMonth=" & myReportingMonths(y).ReportMonth)) & tdEnd))
            Next
        End If
        _Placeholder.Controls.Add(New LiteralControl("</tr>"))


        '********************************Monthly Variance Actual-Booked Total Row*********************
        _Placeholder.Controls.Add(New LiteralControl("<tr class=""myAlt"">"))
        _Placeholder.Controls.Add(New LiteralControl("<td colspan=5>Variance A-B " & tdEnd))
        'Period Variance
        If Not String.IsNullOrEmpty(_AssignmentList) Then
            '_Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsActuals.Tables(0).Compute("Sum(ActualRevenue)", "ReportMonth>=" & myReportingMonths.First.ReportMonth & " and ReportMonth<=" & myReportingMonths.Last.ReportMonth))) - ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Revenue)", "AssignmentID in (" & _AssignmentList & ") and MonthlyReportDate>=" & myReportingMonths.First.ReportMonth & " and MonthlyReportDate<=" & myReportingMonths.Last.ReportMonth)))) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Actual)", "1=1"))) - ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Forecast)", "1=1")))) & tdEnd))
        Else
            _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", 0)))
        End If
        'Month By Month Variance
        If Not String.IsNullOrEmpty(_AssignmentList) Then
            For y = 0 To UBound(myReportingMonths)
                _Placeholder.Controls.Add(New LiteralControl(td3 & String.Format("{0:c0}", ReturnNumber(Convert.ToString(_dsActuals.Tables(0).Compute("Sum(ActualRevenue)", "ReportMonth=" & myReportingMonths(y).ReportMonth))) - ReturnNumber(Convert.ToString(_dsData.Tables(0).Compute("Sum(Revenue)", "AssignmentID in (" & _AssignmentList & ") and MonthlyReportDate=" & myReportingMonths(y).ReportMonth)))) & tdEnd))
            Next
        End If
        _Placeholder.Controls.Add(New LiteralControl("</tr>"))


        '********************************Monthly 1099 Costs Total Row*********************
        _Placeholder.Controls.Add(New LiteralControl("<tr class=""myAlt"">"))
        _Placeholder.Controls.Add(New LiteralControl("<td colspan=5>Contractor Costs " & tdEnd))
        If Not String.IsNullOrEmpty(_AssignmentList) Then
            _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Cost1099)", "1=1")))) & tdEnd))
        Else
            _Placeholder.Controls.Add(New LiteralControl("<td>" & String.Format("{0:c0}", 0)))
        End If

        If Not String.IsNullOrEmpty(_AssignmentList) Then
            For y = 0 To UBound(myReportingMonths)
                _Placeholder.Controls.Add(New LiteralControl(td3 & String.Format("{0:c0}", ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Cost1099)", "ReportMonth=" & myReportingMonths(y).ReportMonth)))) & tdEnd))
            Next
        End If
        _Placeholder.Controls.Add(New LiteralControl("</tr>"))

        '********************************Monthly Forecasted Row*********************
        'I have to manually build these two rows!!!!!!
        Dim tempActual, tempBooked, tempRunningForecast As Double
        Dim sAccumFore As String = ""
        Dim sForecast As String = ""

        If Not String.IsNullOrEmpty(_AssignmentList) Then
            For y = 0 To UBound(myReportingMonths)
                tempActual = ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Actual)", "ReportMonth=" & myReportingMonths(y).ReportMonth)))
                tempBooked = ReturnNumber(Convert.ToString(newDS.Tables(0).Compute("Sum(Forecast)", "ReportMonth=" & myReportingMonths(y).ReportMonth)))
                If tempActual = 0 Then
                    tempRunningForecast += tempBooked
                    sAccumFore &= (td3 & String.Format("{0:c0}", tempRunningForecast) & tdEnd)
                    sForecast &= td3 & String.Format("{0:c0}", tempBooked) & tdEnd
                Else
                    tempRunningForecast += tempActual
                    sAccumFore &= (td3 & String.Format("{0:c0}", tempRunningForecast) & tdEnd)
                    sForecast &= td3 & String.Format("{0:c0}", tempActual) & tdEnd
                End If
            Next
            'Now add in
            sForecast = "<tr class=""myAlt""><td colspan=5>Booked+Forecasted </td><td>" & String.Format("{0:c0}", tempRunningForecast) & "</td>" & sForecast & "</tr>"
            sAccumFore = "<tr class=""myAlt""><td colspan=5>Accum Booked+Forecasted </td><td>N/A</td>" & sAccumFore & "</tr>"
        End If
        _Placeholder.Controls.Add(New LiteralControl(sForecast))
        _Placeholder.Controls.Add(New LiteralControl(sAccumFore))

        _Placeholder.Controls.Add(New LiteralControl("</table>"))
        'Dim t As String = System.Reflection.MethodBase.GetCurrentMethod().Name
        Return _Placeholder
    End Function
    Public Function ReturnNumberFormat(ByVal strInput As String, ByVal strFormat As String) As String
        Dim tempValue As Double
        If String.IsNullOrEmpty(strInput) Then
            tempValue = 0
        Else
            tempValue = strInput
        End If
        If IsNumeric(tempValue) Then
            If tempValue = 0 Then
                Select Case strFormat
                    Case "C0"
                        Return "$ - "
                    Case "C2"
                        Return "$ - "
                    Case "P0"
                        Return " - %"
                    Case "P2"
                        Return " - %"
                    Case "String"
                        Return " - "
                    Case Else
                        Return " - "
                End Select
            Else
                Select Case strFormat
                    Case "C0"
                        Return "$ - "
                    Case "C2"
                        Return "$ - "
                    Case "P0"
                        Return " - %"
                    Case "P2"
                        Return " - %"
                    Case "String"
                        Return " - "
                    Case Else
                        Return " - "
                End Select
            End If
        End If
        Return "1"
    End Function
End Class
