Imports System.Data
Imports cCommonChart
Imports System.Drawing
Imports cForecast
Imports cCommon
Imports cDataSetManipulation
Partial Class BURevenueVarianceReport
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If User.Identity.IsAuthenticated Then
            If Not cCookie.bCookieExist("UserSettings") Then
                Response.Redirect("login.aspx")
            End If
        Else
            Response.Redirect("login.aspx")
        End If
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit
        Me.Theme = cCommon.GetUserTheme
        If Not Page.IsPostBack Then
            If ConfigurationManager.AppSettings("TrackUserEveryPage").ToUpper = "ON" Then
                InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If
    End Sub

    Protected Sub btnUpdateChart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateChart.Click
        'Get the data
        Dim masterDS As New DataSet
        Dim copyDS As New DataSet

        masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 1)

        copyDS = masterDS.Copy
        'Remove a few columns we won't be using
        masterDS.Tables("Booked").Columns.Remove("ActualRevenue")
        masterDS.Tables("Booked").Columns.Remove("PredictedRevenue")
        masterDS.Tables("Booked").Columns.Remove("Actual1099Costs")
        masterDS.Tables("Booked").Columns.Remove("Predicted1099Costs")
        masterDS.Tables("Booked").Columns.Remove("Booked1099Costs")
        masterDS.Tables("Booked").Columns.Remove("RevenueVariance")
        masterDS.Tables("Booked").Columns.Remove("CostVariance")


        'Now Pivot the Data
        Dim newMasterDS As New DataSet
        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "Booked"))
        newMasterDS.Tables(0).TableName = "Booked"

        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("OperatingCosts"), "Operating Costs", "ReportMonthYear", "Amount"))
        newMasterDS.Tables(1).TableName = "OperatingCosts"

        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Profit"), "LineItem", "ReportMonthYear", "Amount"))
        newMasterDS.Tables(2).TableName = "Profit"


        CreateRevenueVarianceGrid(copyDS)
        CreateGraph(copyDS)
    End Sub

    Sub SetCellFormatsForMixedGrid(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim x As Integer
            If CBool(InStr(e.Row.Cells(0).Text, "%")) Then
                For x = 1 To e.Row.Cells.Count - 1 '(start at 1st column, and format to last column)
                    e.Row.Cells(x).Text = Format(e.Row.Cells(x).Text, "Percent")
                Next
            Else
                For x = 1 To e.Row.Cells.Count - 1 '(start at 1st column, and format to last column)
                    If IsNumeric(e.Row.Cells(x).Text) Then
                        e.Row.Cells(x).Text = FormatCurrency(e.Row.Cells(x).Text, 0)
                    End If
                Next
            End If
            'e.Row.Cells(1).Width = 500
            ' Display the company name in italics.
            ' e.Row.Cells(1).Text = "<i>" & e.Row.Cells(1).Text & "</i>"

        End If
    End Sub

    Private Sub CreateGraph(ByVal dsInput As DataSet)
        'MSNChart1
        SetMSChartDefaults(MSNChart1)

        ' For each Row add a new series
        Dim seriesName As String = "Forecast"
        MSNChart1.Series.Add(seriesName)
        MSNChart1.Series(seriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(seriesName).MarkerStyle = DataVisualization.Charting.MarkerStyle.Diamond
        MSNChart1.Series(seriesName).MarkerSize = 12
        MSNChart1.Series(seriesName).ChartType = DataVisualization.Charting.SeriesChartType.Line
        MSNChart1.Series(seriesName).BorderWidth = 2
        MSNChart1.Series(seriesName).ToolTip = "#VALY{C0}"
        MSNChart1.Series(seriesName).SmartLabelStyle.MovingDirection = DataVisualization.Charting.LabelAlignmentStyles.Bottom
        MSNChart1.Series(seriesName).SmartLabelStyle.Enabled = True
        MSNChart1.Series(seriesName)("LabelStyle") = "Bottom"
        Dim dsNew As New DataSet
        dsNew.Tables.Add(PivotDataTable(dsInput.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "PredictedRevenue"))
        For Each dc In dsNew.Tables(0).Columns
            If IsDate(dc.ColumnName) Then
                MSNChart1.Series(seriesName).Points.AddXY(dc.ColumnName, dsNew.Tables(0).Compute("Sum([" & dc.ColumnName & "])", ""))
            End If
        Next


        ' For each Row add a new series
        Dim seriesName1 As String = "Actual"
        MSNChart1.Series.Add(seriesName1)
        MSNChart1.Series(seriesName1).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(seriesName1).MarkerStyle = DataVisualization.Charting.MarkerStyle.Diamond
        MSNChart1.Series(seriesName1).MarkerSize = 12
        MSNChart1.Series(seriesName1).ChartType = DataVisualization.Charting.SeriesChartType.Line
        MSNChart1.Series(seriesName1).BorderWidth = 2
        MSNChart1.Series(seriesName1).ToolTip = "#VALY{C0}"
        MSNChart1.Series(seriesName1).SmartLabelStyle.MovingDirection = DataVisualization.Charting.LabelAlignmentStyles.Bottom
        MSNChart1.Series(seriesName1).SmartLabelStyle.Enabled = True
        MSNChart1.Series(seriesName1)("LabelStyle") = "Bottom"
        Dim dsNew1 As New DataSet
        dsNew1.Tables.Add(PivotDataTable(dsInput.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "ActualRevenue"))
        For Each dc In dsNew1.Tables(0).Columns
            If IsDate(dc.ColumnName) Then
                MSNChart1.Series(seriesName1).Points.AddXY(dc.ColumnName, dsNew1.Tables(0).Compute("Sum([" & dc.ColumnName & "])", ""))
            End If
        Next


        ' For each Row add a new series
        Dim seriesName2 As String = "Variance"
        MSNChart1.Series.Add(seriesName2)
        MSNChart1.Series(seriesName2).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(seriesName2).MarkerStyle = DataVisualization.Charting.MarkerStyle.Diamond
        MSNChart1.Series(seriesName2).MarkerSize = 12
        MSNChart1.Series(seriesName2).ChartType = DataVisualization.Charting.SeriesChartType.Column
        MSNChart1.Series(seriesName2).BorderWidth = 2
        MSNChart1.Series(seriesName2).ToolTip = "#VALY{C0}"
        MSNChart1.Series(seriesName2).SmartLabelStyle.MovingDirection = DataVisualization.Charting.LabelAlignmentStyles.Bottom
        MSNChart1.Series(seriesName2).SmartLabelStyle.Enabled = True
        MSNChart1.Series(seriesName2)("LabelStyle") = "Bottom"
        Dim dsNew2 As New DataSet
        dsNew2.Tables.Add(PivotDataTable(dsInput.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "RevenueVariance"))
        For Each dc In dsNew2.Tables(0).Columns
            If IsDate(dc.ColumnName) Then
                MSNChart1.Series(seriesName2).Points.AddXY(dc.ColumnName, dsNew2.Tables(0).Compute("Sum([" & dc.ColumnName & "])", ""))
            End If
        Next


        MSNChart1.Height = Charty
        MSNChart1.Width = Chartx
        MSNChart1.Titles(0).Text = "Business Forecast vs Actual Revenue"
        MSNChart1.ChartAreas(0).AxisX.TextOrientation = DataVisualization.Charting.TextOrientation.Rotated90
        MSNChart1.ChartAreas(0).AxisY.Title = "Revenue"
        MSNChart1.ChartAreas(0).AxisY.TitleFont = TitleFont
        MSNChart1.ChartAreas(0).AxisY.LabelStyle.Format = "C0"

    End Sub
    Private Sub AddYSeries(ByVal strSeriesName As String, ByVal strSeriesY As String, ByVal bPrimaryAxis As Boolean)
        MSNChart1.Series.Add(strSeriesName)
        MSNChart1.Series(strSeriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(strSeriesName).XValueMember = "ReportMonthYear"
        MSNChart1.Series(strSeriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(strSeriesName).XValueMember = "ReportMonthYear"

        MSNChart1.Series(strSeriesName).YValueMembers = strSeriesY
        MSNChart1.Series(strSeriesName).ToolTip = "#VALY{C0}"
        MSNChart1.Series(strSeriesName).ChartType = DataVisualization.Charting.SeriesChartType.Column

        MSNChart1.Series(strSeriesName).MarkerStyle = DataVisualization.Charting.MarkerStyle.Diamond
        MSNChart1.Series(strSeriesName).MarkerSize = 12
        MSNChart1.Series(strSeriesName).SmartLabelStyle.MovingDirection = DataVisualization.Charting.LabelAlignmentStyles.Bottom
        MSNChart1.Series(strSeriesName).SmartLabelStyle.Enabled = True
        MSNChart1.Series(strSeriesName)("LabelStyle") = "Bottom"
        MSNChart1.Series(strSeriesName).BorderWidth = 3
        If Not bPrimaryAxis Then
            MSNChart1.Series(strSeriesName).YAxisType = DataVisualization.Charting.AxisType.Secondary
            MSNChart1.ChartAreas(0).AxisY2.Minimum = 0
            MSNChart1.ChartAreas(0).AxisY2.Maximum = 1
            MSNChart1.ChartAreas(0).AxisY2.LabelStyle.Format = "P2"
            MSNChart1.Series(strSeriesName).ToolTip = "#VALY{P2}"
            MSNChart1.Series(strSeriesName).MarkerSize = 10
        End If
    End Sub
    Protected Sub SetupInitialDate(ByVal sender As Object, ByVal e As System.EventArgs)
        cCommonDateCode.SetupInitialDate(sender, e)
    End Sub
    Private Sub CreateRevenueVarianceGrid(ByVal dsInput As DataSet)
        Dim dsBookedRevenue As DataSet = dsInput.Copy
        Dim dsForecastedRevenue As DataSet = dsInput.Copy
        Dim dsActualRevenue As DataSet = dsInput.Copy
        Dim dsRevenueVariance As DataSet = dsInput.Copy

        Dim masterDS As New DataSet
        'Create and Pivot Booked
        'dsBookedRevenue.Tables("Booked").Columns.Remove("Booked")
        dsBookedRevenue.Tables("Booked").Columns.Remove("ActualRevenue")
        dsBookedRevenue.Tables("Booked").Columns.Remove("PredictedRevenue")
        dsBookedRevenue.Tables("Booked").Columns.Remove("Actual1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("Predicted1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("Booked1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("RevenueVariance")
        dsBookedRevenue.Tables("Booked").Columns.Remove("CostVariance")
        masterDS.Tables.Add(PivotDataTable(dsBookedRevenue.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "Booked"))
        masterDS.Tables(0).TableName = "BookedRevenue"
        Dim pColumn() As DataColumn = {masterDS.Tables(0).Columns("AccountGroupID")} 'Set primary key (needed for merge)
        masterDS.Tables(0).PrimaryKey = pColumn


        'Create and Pivot Forecasted
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Booked")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("ActualRevenue")
        'dsForecastedRevenue.Tables("Booked").Columns.Remove("PredictedRevenue")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Actual1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Predicted1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Booked1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("RevenueVariance")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("CostVariance")
        masterDS.Tables.Add(PivotDataTable(dsForecastedRevenue.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "PredictedRevenue"))
        masterDS.Tables(1).TableName = "ForecastedRevenue"
        Dim pColumn1() As DataColumn = {masterDS.Tables(1).Columns("AccountGroupID")} 'Set primary key (needed for merge)
        masterDS.Tables(1).PrimaryKey = pColumn1

        'Create and Pivot Actual
        dsActualRevenue.Tables("Booked").Columns.Remove("Booked")
        'dsActualRevenue.Tables("Booked").Columns.Remove("ActualRevenue")
        dsActualRevenue.Tables("Booked").Columns.Remove("PredictedRevenue")
        dsActualRevenue.Tables("Booked").Columns.Remove("Actual1099Costs")
        dsActualRevenue.Tables("Booked").Columns.Remove("Predicted1099Costs")
        dsActualRevenue.Tables("Booked").Columns.Remove("Booked1099Costs")
        dsActualRevenue.Tables("Booked").Columns.Remove("RevenueVariance")
        dsActualRevenue.Tables("Booked").Columns.Remove("CostVariance")
        masterDS.Tables.Add(PivotDataTable(dsActualRevenue.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "ActualRevenue"))
        masterDS.Tables(2).TableName = "ActualRevenue"
        Dim pColumn2() As DataColumn = {masterDS.Tables(2).Columns("AccountGroupID")} 'Set primary key (needed for merge)
        masterDS.Tables(2).PrimaryKey = pColumn2

        'Create and Pivot Variance
        dsRevenueVariance.Tables("Booked").Columns.Remove("Booked")
        dsRevenueVariance.Tables("Booked").Columns.Remove("ActualRevenue")
        dsRevenueVariance.Tables("Booked").Columns.Remove("PredictedRevenue")
        dsRevenueVariance.Tables("Booked").Columns.Remove("Actual1099Costs")
        dsRevenueVariance.Tables("Booked").Columns.Remove("Predicted1099Costs")
        dsRevenueVariance.Tables("Booked").Columns.Remove("Booked1099Costs")
        'dsRevenueVariance.Tables("Booked").Columns.Remove("RevenueVariance")
        dsRevenueVariance.Tables("Booked").Columns.Remove("CostVariance")
        masterDS.Tables.Add(PivotDataTable(dsRevenueVariance.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "RevenueVariance"))
        masterDS.Tables(3).TableName = "RevenueVariance"
        Dim pColumn3() As DataColumn = {masterDS.Tables(3).Columns("AccountGroupID")} 'Set primary key (needed for merge)
        masterDS.Tables(3).PrimaryKey = pColumn3

        Dim _pnl As New Panel 'My internal panel to add items
        Dim trStart As LiteralControl
        Dim trEnd As LiteralControl
        Dim tdStartNorm As String
        Dim tdLeft, tdRight, tdMid, tdEnd As String
        tdLeft = "<td class=""left"">"
        tdRight = "<td class=""right"">"
        tdMid = "<td class=""mid"">"
        tdEnd = "</td>"
        tdStartNorm = "<td> "

        Dim _Placeholder As New PlaceHolder

        Dim colCount As Integer = masterDS.Tables(0).Columns.Count - 4

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
        For y = 4 To colCount + 3
            _Placeholder.Controls.Add(New LiteralControl("<th colspan=3>" & masterDS.Tables(0).Columns(y).ColumnName & "</th>"))
        Next
        trEnd = New LiteralControl("</tr>") 'End Table Row
        _Placeholder.Controls.Add(trEnd)
        intCounter += 1 'Next Row!
        '********************Empty Data Rows**********************
        'start table row!
        If intCounter Mod 2 <> 0 Then
            trStart = New LiteralControl("<tr class=""myNorm"">")
        Else
            trStart = New LiteralControl("<tr class=""myAlt"">")
        End If
        _Placeholder.Controls.Add((trStart))
        _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & "&nbsp;" & tdEnd))
        For y = 4 To colCount + 3
            _Placeholder.Controls.Add(New LiteralControl(tdLeft & "Forecast" & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdLeft & "Actual" & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdLeft & "Variance" & tdEnd))
        Next
        trEnd = New LiteralControl("</tr>") 'End Table Row
        _Placeholder.Controls.Add(trEnd)
        intCounter += 1
        '********************Data Rows**********************
        For Each dr In masterDS.Tables(0).Rows
            'start table row!
            If intCounter Mod 2 <> 0 Then
                trStart = New LiteralControl("<tr class=""myNorm"">")
            Else
                trStart = New LiteralControl("<tr class=""myAlt"">")
            End If
            _Placeholder.Controls.Add((trStart))
            _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & dr.Item("AccountGroup") & tdEnd))
            For y = 4 To colCount + 3
                _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:c0}", ReturnNumber(Convert.ToString(masterDS.Tables("ForecastedRevenue").Compute("SUM([" & masterDS.Tables(0).Columns(y).ColumnName & "])", "AccountGroupID=" & dr.Item("AccountGroupID"))))) & tdEnd))
                _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:c0}", ReturnNumber(Convert.ToString(masterDS.Tables("ActualRevenue").Compute("SUM([" & masterDS.Tables(0).Columns(y).ColumnName & "])", "AccountGroupID=" & dr.Item("AccountGroupID"))))) & tdEnd))
                _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:c0}", ReturnNumber(Convert.ToString(masterDS.Tables("RevenueVariance").Compute("SUM([" & masterDS.Tables(0).Columns(y).ColumnName & "])", "AccountGroupID=" & dr.Item("AccountGroupID"))))) & tdEnd))
            Next
            trEnd = New LiteralControl("</tr>") 'End Table Row
            _Placeholder.Controls.Add(trEnd)
            intCounter += 1
        Next
        'Add Spacer Row
        _Placeholder.Controls.Add(New LiteralControl("<tr class=""myAlt""><td colspan=""" & 1 + (3 * colCount) & """>&nbsp;</td>"))

        '********************Total Rows**********************
        'start table row!
        If intCounter Mod 2 <> 0 Then
            trStart = New LiteralControl("<tr class=""myNorm"">")
        Else
            trStart = New LiteralControl("<tr class=""myAlt"">")
        End If
        _Placeholder.Controls.Add((trStart))
        _Placeholder.Controls.Add(New LiteralControl(tdStartNorm & "Monthly Variance" & tdEnd))
        For y = 4 To colCount + 3
            _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:c0}", ReturnNumber(Convert.ToString(masterDS.Tables("ForecastedRevenue").Compute("SUM([" & masterDS.Tables(0).Columns(y).ColumnName & "])", "")))) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:c0}", ReturnNumber(Convert.ToString(masterDS.Tables("ActualRevenue").Compute("SUM([" & masterDS.Tables(0).Columns(y).ColumnName & "])", "")))) & tdEnd))
            _Placeholder.Controls.Add(New LiteralControl(tdLeft & String.Format("{0:c0}", ReturnNumber(Convert.ToString(masterDS.Tables("RevenueVariance").Compute("SUM([" & masterDS.Tables(0).Columns(y).ColumnName & "])", "")))) & tdEnd))
        Next
        trEnd = New LiteralControl("</tr>") 'End Table Row
        _Placeholder.Controls.Add(trEnd)
        intCounter += 1

        _Placeholder.Controls.Add(New LiteralControl("</table>"))
        pnlVariancegrid.Controls.Add(_Placeholder)

    End Sub
End Class