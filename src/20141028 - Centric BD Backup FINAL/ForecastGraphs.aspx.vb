Imports System.Data
Imports cCommonChart
Imports System.Drawing
Imports cForecast
Imports cCommon
Imports cDataSetManipulation
Partial Class ForecastGraphs
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
        Dim bType As Boolean = True
        If rblType.SelectedValue = "Forecast" Then
            bType = True
        Else
            bType = False
        End If
        Select Case rblShadowOrBooked.SelectedValue
            Case "Shadow"
                masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 0.7, bType)
            Case "Booked"
                masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 1, bType)
            Case Else
                masterDS = GetAccountGroupBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID, 1, bType)
        End Select
        copyDS = masterDS.Copy
        Dim newMasterDS As New DataSet

        'Remove a few columns we won't be using
        'Remove all non-revenue data from the first table.  It is only client/revenue/by month
        masterDS.Tables("Booked").Columns.Remove("Actual1099Costs")
        masterDS.Tables("Booked").Columns.Remove("Predicted1099Costs")
        masterDS.Tables("Booked").Columns.Remove("Booked1099Costs")
        masterDS.Tables("Booked").Columns.Remove("RevenueVariance")
        masterDS.Tables("Booked").Columns.Remove("CostVariance")

        Select Case rblType.SelectedValue
            Case "Forecast"
                masterDS.Tables("Booked").Columns.Remove("ActualRevenue")
                masterDS.Tables("Booked").Columns.Remove("Booked")
                'Now Pivot the Data
                newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "PredictedRevenue"))
                newMasterDS.Tables(0).TableName = "Booked"
            Case "Booked"
                masterDS.Tables("Booked").Columns.Remove("ActualRevenue")
                masterDS.Tables("Booked").Columns.Remove("PredictedRevenue")
                'Now Pivot the Data
                newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "Booked"))
                newMasterDS.Tables(0).TableName = "Booked"
            Case "Actual"
                masterDS.Tables("Booked").Columns.Remove("PredictedRevenue")
                masterDS.Tables("Booked").Columns.Remove("Booked")
                'Now Pivot the Data
                newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Booked"), "AccountGroupID", "ReportMonthYear", "ActualRevenue"))
                newMasterDS.Tables(0).TableName = "Booked"
        End Select

        'Operating costs table
        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("OperatingCosts"), "Operating Costs", "ReportMonthYear", "Amount"))
        newMasterDS.Tables(1).TableName = "OperatingCosts"

        'Profit table
        newMasterDS.Tables.Add(PivotDataTable(masterDS.Tables("Profit"), "LineItem", "ReportMonthYear", "Amount"))
        newMasterDS.Tables(2).TableName = "Profit"

        CreateGridView(newMasterDS)
        CreateGraph(masterDS)
    End Sub
    Private Sub CreateGridView(ByVal ds As DataSet)

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
                        'tempValue= +=0
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

    Private Sub CreateGraph(ByVal ds As DataSet)
        'MSNChart1
        SetMSChartDefaults(MSNChart1)
        'Ok, first filter data to Total Revenue
        Dim myTotalRevenueView, myBUProfitView As DataView
        myTotalRevenueView = ds.Tables("Profit").DefaultView
        If rblType.SelectedValue = "Forecast" Then
            myTotalRevenueView.RowFilter = "LineItem='Total Forecasted Revenue'"
        Else
            myTotalRevenueView.RowFilter = "LineItem='Total Booked Revenue'"
        End If

        Dim tDs As New DataSet
        'Add total revenue data to a temp talb
        tDs.Tables.Add(myTotalRevenueView.ToTable.Copy)
        If rblType.SelectedValue = "Forecast" Then
            tDs.Tables(0).Columns("Amount").ColumnName = "Total Forecasted Revenue"
        Else
            tDs.Tables(0).Columns("Amount").ColumnName = "Total Booked Revenue"
        End If

        Dim pColumn() As DataColumn = {tDs.Tables(0).Columns("ReportMonthYear")} 'Set primary key (needed for merge)
        tDs.Tables(0).PrimaryKey = pColumn
        'Filter BU Profit Data 
        myBUProfitView = ds.Tables("Profit").DefaultView
        If rblType.SelectedValue = "Forecast" Then
            myBUProfitView.RowFilter = "LineItem='BU Forecasted Profit'"
            'Merge BU Profit to Total Revenue in temp Table
            tDs.Merge(myBUProfitView.ToTable)
            tDs.Tables(0).Columns("Amount").ColumnName = "BU Forecasted Profit"
        Else
            myBUProfitView.RowFilter = "LineItem='BU Calculated Booked Profit'"
            'Merge BU Profit to Total Revenue in temp Table
            tDs.Merge(myBUProfitView.ToTable)
            tDs.Tables(0).Columns("Amount").ColumnName = "BU Calculated Booked Profit"
        End If

        'Merge BU Profit to Total Revenue in temp Table
        'tDs.Merge(myBUProfitView.ToTable)
        'tDs.Tables(0).Columns("Amount").ColumnName = "BU Calculated Booked Profit"

        'Create Graph
        MSNChart1.DataSource = tDs
        MSNChart1.Height = Charty
        MSNChart1.Width = Chartx
        MSNChart1.Titles(0).Text = "Business Unit Revenue and Profit"
        MSNChart1.ChartAreas(0).AxisX.TextOrientation = DataVisualization.Charting.TextOrientation.Rotated90
        MSNChart1.ChartAreas(0).AxisY.Title = "Revenue"
        MSNChart1.ChartAreas(0).AxisY.TitleFont = TitleFont
        MSNChart1.ChartAreas(0).AxisY.LabelStyle.Format = "C0"
        If rblType.SelectedValue = "Forecast" Then
            AddYSeries("Total Forecasted Revenue", "Total Forecasted Revenue", True)
            AddYSeries("BU Forecasted Profit", "BU Forecasted Profit", True)
        Else
            AddYSeries("Total Booked Revenue", "Total Booked Revenue", True)
            AddYSeries("BU Calculated Booked Profit", "BU Calculated Booked Profit", True)
        End If
        
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


End Class