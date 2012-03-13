Imports System.Data
Imports cCommonChart
Imports System.Drawing
Imports cFinancial
Imports cCommon
Imports cDataSetManipulation
Partial Class BUFinancialVariance
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

        Select Case rblShadowOrBooked.SelectedValue
            Case "Shadow"
                masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 0.7)
            Case "Booked"
                masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 1)
            Case Else
                masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 1)
        End Select

        copyDS = masterDS.Copy
        'Remove a few columns we won't be using
        masterDS.Tables("Booked").Columns.Remove("ActualRevenue")
        masterDS.Tables("Booked").Columns.Remove("PredictedRevenue")
        masterDS.Tables("Booked").Columns.Remove("Actual1099Costs")
        masterDS.Tables("Booked").Columns.Remove("Predicted1099Costs")
        masterDS.Tables("Booked").Columns.Remove("Booked1099Costs")
        masterDS.Tables("Booked").Columns.Remove("RevenueVariance")
        masterDS.Tables("Booked").Columns.Remove("CostVariance")
        masterDS.Tables("Booked").Columns.Remove("PredictedBURevenue")
        masterDS.Tables("Booked").Columns.Remove("PredictedBU1099Costs")
        masterDS.Tables("Booked").Columns.Remove("CostBUVariance")
        masterDS.Tables("Booked").Columns.Remove("RevenueBUVariance")

        'Now Pivot the Data
        Dim newMasterDS As New DataSet
        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "Booked"))
        newMasterDS.Tables(0).TableName = "Booked"

        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("OperatingCosts"), "Operating Costs", "ReportMonthYear", "Amount"))
        newMasterDS.Tables(1).TableName = "OperatingCosts"

        'This is new to accomodate for Chad's request to remove "BOOKED" line items
        '***Old
        'newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Profit"), "LineItem", "ReportMonthYear", "Amount"))
        'newMasterDS.Tables(2).TableName = "Profit"
        '*****New below
        Dim myProfitView As DataView : Dim newProfitDS As New DataSet

        myProfitView = masterDS.Tables("Profit").DefaultView
        myProfitView.RowFilter = "LineItem not like '%Booked%'"
        newProfitDS.Tables.Add(myProfitView.ToTable.Copy)

        newMasterDS.Tables.Add(PivotDataTable(newProfitDS.Tables("Profit"), "LineItem", "ReportMonthYear", "Amount"))
        newMasterDS.Tables(2).TableName = "Profit"


        CreateRevenueVarianceGrid(copyDS)
        CreateGraph(copyDS)
        CreateFinancialGrids(newMasterDS)
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
        Dim seriesName As String = "Forecasted Revenue"
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
        Dim seriesName1 As String = "Actual Revenue"
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
        Dim seriesName2 As String = "Revenue Variance"
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


        MSNChart1.Height = Charty * 0.7
        MSNChart1.Width = Chartx * 0.7
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
        'I have to remove all columns that I'm not using, so here I am pivoting on booked and removing the others
        'I do a similar thing in the sections below.
        'dsBookedRevenue.Tables("Booked").Columns.Remove("Booked")
        dsBookedRevenue.Tables("Booked").Columns.Remove("ActualRevenue")
        dsBookedRevenue.Tables("Booked").Columns.Remove("PredictedRevenue")
        dsBookedRevenue.Tables("Booked").Columns.Remove("Actual1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("Predicted1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("Booked1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("RevenueVariance")
        dsBookedRevenue.Tables("Booked").Columns.Remove("CostVariance")
        dsBookedRevenue.Tables("Booked").Columns.Remove("PredictedBURevenue")
        dsBookedRevenue.Tables("Booked").Columns.Remove("PredictedBU1099Costs")
        dsBookedRevenue.Tables("Booked").Columns.Remove("CostBUVariance")
        dsBookedRevenue.Tables("Booked").Columns.Remove("RevenueBUVariance")
        masterDS.Tables.Add(PivotDataTable(dsBookedRevenue.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "Booked"))
        masterDS.Tables(0).TableName = "BookedRevenue"
        Dim pColumn() As DataColumn = {masterDS.Tables(0).Columns("AccountGroupID")} 'Set primary key (needed for merge)
        masterDS.Tables(0).PrimaryKey = pColumn


        'Create and Pivot Forecasted
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Booked")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("ActualRevenue")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("PredictedRevenue")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Actual1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Predicted1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("Booked1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("RevenueVariance")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("CostVariance")
        'dsForecastedRevenue.Tables("Booked").Columns.Remove("PredictedBURevenue")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("PredictedBU1099Costs")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("CostBUVariance")
        dsForecastedRevenue.Tables("Booked").Columns.Remove("RevenueBUVariance")
        masterDS.Tables.Add(PivotDataTable(dsForecastedRevenue.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "PredictedBURevenue"))
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
        dsActualRevenue.Tables("Booked").Columns.Remove("PredictedBURevenue")
        dsActualRevenue.Tables("Booked").Columns.Remove("PredictedBU1099Costs")
        dsActualRevenue.Tables("Booked").Columns.Remove("CostBUVariance")
        dsActualRevenue.Tables("Booked").Columns.Remove("RevenueBUVariance")
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
        dsRevenueVariance.Tables("Booked").Columns.Remove("RevenueVariance")
        dsRevenueVariance.Tables("Booked").Columns.Remove("CostVariance")
        dsRevenueVariance.Tables("Booked").Columns.Remove("PredictedBURevenue")
        dsRevenueVariance.Tables("Booked").Columns.Remove("PredictedBU1099Costs")
        dsRevenueVariance.Tables("Booked").Columns.Remove("CostBUVariance")
        'dsRevenueVariance.Tables("Booked").Columns.Remove("RevenueBUVariance")
        masterDS.Tables.Add(PivotDataTable(dsRevenueVariance.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "RevenueBUVariance"))
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
        _Placeholder.Controls.Add(New LiteralControl("<h4>Revenue Variance by Account Group - Omits excluded revenue</h4><br/>"))
        _Placeholder.Controls.Add(New LiteralControl("<table>"))
        _Placeholder.Controls.Add(trStart)
        'Add Header Rows!
        _Placeholder.Controls.Add(New LiteralControl("<th>" & "Account Group" & "</th>"))
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
    Private Sub CreateFinancialGrids(ByVal ds As DataSet)

        '*******************************THIS IS FOR EXPORT ONLY*****************************
        'create a string writer
        Dim stringWrite As New System.IO.StringWriter
        'create an htmltextwriter which uses the stringwriter
        Dim htmlWrite As New System.Web.UI.HtmlTextWriter(stringWrite)

        If ckExport.Checked Then
            'This exports vice going to web page
            'first let's clean up the response.object
            Response.Clear()
            Response.Charset = ""
            'set the response mime type for excel
            Response.ContentType = "application/vnd.ms-excel"
        End If
        '**********************************************************************************
        pnlgridview.Controls.Add(New LiteralControl("<h4>Booked Revenue By Anchor Client and Account Group</h4><br/>"))
        '''''THIS IS CODE FOR THE GRIDVIEW
        Dim dgBooked As New GridView
        'Let's add a total entry for the ds.Tables("Booked")
        Dim tCol As New DataColumn
        tCol.ColumnName = "Period Total"
        tCol.DataType = System.Type.GetType("System.Double")
        ds.Tables("Booked").Columns.Add(tCol)

        Dim intCounter, intCounter2 As Integer
        Dim tempValue As Double
        For intCounter = 0 To ds.Tables(0).Rows.Count - 1
            tempValue = 0
            For intCounter2 = 0 To ds.Tables(0).Columns.Count - 1
                If IsDate(ds.Tables(0).Columns(intCounter2).ColumnName) Then
                    tempValue += ds.Tables(0).Rows(intCounter).Item(intCounter2)
                End If
            Next
            ds.Tables(0).Rows(intCounter).Item("Period Total") = tempValue
        Next
        'Now bind dgBooked to datagrid and format columns

        dgBooked.DataSource = ds.Tables("Booked")
        dgBooked.AutoGenerateColumns = False
        For Each cColumn In ds.Tables("Booked").Columns
            If cColumn.ColumnName <> "AccountGroupID" Then
                Dim tempCol As New BoundField
                If IsDate(cColumn.ColumnName) Then
                    'Format date columns as Currency
                    tempCol.HeaderText = String.Format("{0:MMM yy}", CDate(cColumn.ColumnName))
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:c0}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                    tempCol.ItemStyle.Wrap = False
                ElseIf cColumn.ColumnName = "Period Total" Then
                    tempCol.HeaderText = cColumn.ColumnName
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:c0}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                    tempCol.ItemStyle.Wrap = False
                Else
                    'Format non-date columns as strings
                    tempCol.HeaderText = cColumn.ColumnName
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:g}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.Wrap = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                End If
                tempCol.ItemStyle.Width = Unit.Pixel(100)
                dgBooked.Columns.Add(tempCol)
            End If
        Next


        'Now try to add totals column
        Dim dr As DataRow = ds.Tables("Booked").NewRow
        For Each cColumn In ds.Tables("Booked").Columns
            If IsDate(cColumn.ColumnName) Or cColumn.ColumnName = "Period Total" Then
                dr.Item(cColumn.ColumnName) = ds.Tables("Booked").Compute("Sum([" & cColumn.ColumnName & "])", String.Empty)
            End If
        Next
        ds.Tables("Booked").Rows.Add(dr)

        'Add spacer row?
        'Dim dr1 As DataRow = ds.Tables("Booked").NewRow
        'ds.Tables("Booked").Rows.Add(dr1)

        dgBooked.DataBind()
        If ckExport.Checked Then
            'tell the datagrid to render itself to our htmltextwriter
            dgBooked.RenderControl(htmlWrite)
        Else
            'OR Write to web page
            pnlgridview.Controls.Add(dgBooked)
            pnlgridview.Controls.Add(New LiteralControl("</br>"))
            pnlgridview.Controls.Add(New LiteralControl("<h4>Operating and Overhead Costs</h4><br/>"))
        End If

        '*****************************
        'Add Total for Operation Costs
        'Let's add a total entry for the ds.Tables("Booked")
        Dim tOCTotal As New DataColumn
        tOCTotal.ColumnName = "Period Total"
        tOCTotal.DataType = System.Type.GetType("System.Double")
        ds.Tables("OperatingCosts").Columns.Add(tOCTotal)

        For intCounter = 0 To ds.Tables("OperatingCosts").Rows.Count - 1
            tempValue = 0
            For intCounter2 = 0 To ds.Tables("OperatingCosts").Columns.Count - 1
                If IsDate(ds.Tables("OperatingCosts").Columns(intCounter2).ColumnName) Then
                    tempValue += ds.Tables("OperatingCosts").Rows(intCounter).Item(intCounter2)
                End If
            Next
            ds.Tables("OperatingCosts").Rows(intCounter).Item("Period Total") = tempValue
        Next

        Dim dgOperatingCosts As New GridView
        dgOperatingCosts.DataSource = ds.Tables("OperatingCosts")
        dgOperatingCosts.AutoGenerateColumns = False
        If ckExport.Checked Then
            'This insertion is just to make excel line up on exporting
            Dim t1, t2 As New TemplateField
            dgOperatingCosts.Columns.Insert(0, t1)
            dgOperatingCosts.Columns.Insert(0, t2)
        End If
        For Each cColumn In ds.Tables("OperatingCosts").Columns
            If cColumn.ColumnName <> "FTE Count" Then
                Dim tempCol As New BoundField
                Dim tempCol2 As New TemplateColumn
                If IsDate(cColumn.ColumnName) Then
                    'Format date columns as Currency
                    tempCol.HeaderText = String.Format("{0:MMM yy}", CDate(cColumn.ColumnName))
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:c0}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                    tempCol.ItemStyle.Wrap = False
                    tempCol.ItemStyle.Width = Unit.Pixel(100)
                ElseIf cColumn.ColumnName = "Period Total" Then
                    tempCol.HeaderText = cColumn.ColumnName
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:c0}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                    tempCol.ItemStyle.Wrap = False
                    tempCol.ItemStyle.Width = Unit.Pixel(100)
                Else
                    'Format non-date columns as strings
                    tempCol.HeaderText = cColumn.ColumnName
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:g}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                    tempCol.ItemStyle.Width = Unit.Pixel(310)
                    tempCol.ItemStyle.Wrap = False
                End If
                dgOperatingCosts.Columns.Add(tempCol)
            End If
        Next
        'Now try to add totals column
        Dim drOC As DataRow = ds.Tables("OperatingCosts").NewRow
        For Each cColumn In ds.Tables("OperatingCosts").Columns
            If IsDate(cColumn.ColumnName) Or cColumn.ColumnName = "Period Total" Then
                drOC.Item(cColumn.ColumnName) = ds.Tables("OperatingCosts").Compute("Sum([" & cColumn.ColumnName & "])", String.Empty)
            End If
        Next
        ds.Tables("OperatingCosts").Rows.Add(drOC)

        dgOperatingCosts.DataBind()
        If ckExport.Checked Then
            'tell the datagrid to render itself to our htmltextwriter
            dgOperatingCosts.RenderControl(htmlWrite)
            'all that's left is to output the html
            'Response.Write(stringWrite.ToString)
        Else
            'Write to web page
            pnlgridview.Controls.Add(dgOperatingCosts)
            pnlgridview.Controls.Add(New LiteralControl("</br>"))
            pnlgridview.Controls.Add(New LiteralControl("<h4>Revenue, Costs, Margins, and Profits</h4><br/>"))
        End If

        '*****************************
        'Add Profit Total Line Column
        Dim tPTotal As New DataColumn
        tPTotal.ColumnName = "Period Total"
        tPTotal.DataType = System.Type.GetType("System.Double")
        ds.Tables("Profit").Columns.Add(tPTotal)

        For intCounter = 0 To ds.Tables("Profit").Rows.Count - 1
            tempValue = 0
            For intCounter2 = 0 To ds.Tables("Profit").Columns.Count - 1
                If IsDate(ds.Tables("Profit").Columns(intCounter2).ColumnName) Then
                    If IsNumeric(ds.Tables("Profit").Rows(intCounter).Item(intCounter2)) Then
                        tempValue += ds.Tables("Profit").Rows(intCounter).Item(intCounter2)
                        'Else
                        'tempValue +=0
                    End If
                End If
            Next
            'If this is the percentage line, then average the summation
            If CBool(InStr(ds.Tables("Profit").Rows(intCounter).Item("LineItem"), "%")) Then
                ds.Tables("Profit").Rows(intCounter).Item("Period Total") = tempValue / (ds.Tables("Profit").Columns.Count - 2)
            Else
                ds.Tables("Profit").Rows(intCounter).Item("Period Total") = tempValue
            End If
        Next
        Dim dgProfit As New GridView
        dgProfit.DataSource = ds.Tables("Profit")
        dgProfit.AutoGenerateColumns = False
        If ckExport.Checked Then
            'This insertion is just to make excel line up on exporting
            Dim t3, t4 As New TemplateField
            dgProfit.Columns.Insert(0, t3)
            dgProfit.Columns.Insert(0, t4)
        End If
        For Each cColumn In ds.Tables("Profit").Columns
            If cColumn.ColumnName <> "Ignore" Then
                Dim tempCol As New BoundField
                Dim tempCol2 As New TemplateColumn
                If IsDate(cColumn.ColumnName) Then
                    'Format date columns as Currency
                    tempCol.HeaderText = String.Format("{0:MMM yy}", CDate(cColumn.ColumnName))
                    tempCol.DataField = cColumn.ColumnName
                    'tempCol.DataFormatString = "{0:c0}" 'Handling through event
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                    tempCol.ItemStyle.Wrap = False
                    tempCol.ItemStyle.Width = Unit.Pixel(100)
                ElseIf cColumn.ColumnName = "Period Total" Then
                    tempCol.HeaderText = cColumn.ColumnName
                    tempCol.DataField = cColumn.ColumnName
                    'tempCol.DataFormatString = "{0:c0}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                    tempCol.ItemStyle.Wrap = False
                    tempCol.ItemStyle.Width = Unit.Pixel(100)
                Else
                    'Format non-date columns as strings
                    tempCol.HeaderText = cColumn.ColumnName
                    tempCol.DataField = cColumn.ColumnName
                    tempCol.DataFormatString = "{0:g}"
                    tempCol.HtmlEncode = False
                    tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Left
                    tempCol.ItemStyle.Wrap = False
                    tempCol.ItemStyle.Width = Unit.Pixel(310)
                End If
                dgProfit.Columns.Add(tempCol)
            End If
        Next
        'Now try to add totals column
        'Dim drProfit As DataRow = ds.Tables("Profit").NewRow
        'For Each cColumn In ds.Tables("Profit").Columns
        ' If IsDate(cColumn.ColumnName) Then
        ' drProfit.Item(cColumn.ColumnName) = ds.Tables("Profit").Compute("Sum([" & cColumn.ColumnName & "])", "LineItem='Total Revenue'") - ds.Tables("Profit").Compute("Sum([" & cColumn.ColumnName & "])", "LineItem='Operating Costs'") - ds.Tables("Profit").Compute("Sum([" & cColumn.ColumnName & "])", "LineItem='1099 Costs'") - ds.Tables("Profit").Compute("Sum([" & cColumn.ColumnName & "])", "LineItem='Est FTE Sal'")
        ' ElseIf cColumn.ColumnName = "LineItem" Then
        ' drProfit.Item(cColumn.ColumnName) = "BU Profit"
        ' End If
        ' Next
        'ds.Tables("Profit").Rows.Add(drProfit)
        ' Programmatically register the event-handling method.
        AddHandler dgProfit.RowDataBound, AddressOf SetCellFormatsForMixedGrid

        dgProfit.DataBind()
        If ckExport.Checked Then
            'tell the datagrid to render itself to our htmltextwriter
            dgProfit.RenderControl(htmlWrite)
            'all that's left is to output the html
            Response.Write(stringWrite.ToString)
            Response.End()
        Else
            'Write to web page
            pnlgridview.Controls.Add(dgProfit)
        End If
        'BU Actual Profit (Psuedo)
        'pnlgridview.Controls.Add(New LiteralControl("*Psuedo Profit is calculated using actual revenue minus booked costs"))
        pnlgridview.Controls.Add(New LiteralControl("</br>"))
    End Sub

End Class