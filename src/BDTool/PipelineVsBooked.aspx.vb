Imports System.Data
Imports cCommonChart
Imports System.Drawing
Imports cForecast
Imports cCommon
Imports cCommonDateCode
Partial Class PipelineVsBooked
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
        Dim tempDS As DataSet = GetPipelineVsBookedByBu(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID)
        CreateGridView(tempDS)
        CreateChart(tempDS)

    End Sub
    Private Sub CreateGridView(ByVal ds As DataSet)
        Dim dgBooked As New GridView
        dgBooked.DataSource = ds.Tables("PipelineVsBooked")
        dgBooked.AutoGenerateColumns = False
        'dgBooked.Width = "99"
        For Each cColumn In ds.Tables("PipelineVsBooked").Columns
            Dim tempCol As New BoundField
            If IsDate(cColumn.ColumnName) Then
                'Format date columns as Month Year
                tempCol.HeaderText = String.Format("{0:MMM yy}", CDate(cColumn.ColumnName))
                tempCol.DataField = cColumn.ColumnName
                tempCol.DataFormatString = "{0:c0}"
                tempCol.HtmlEncode = False
                tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
                tempCol.ItemStyle.Wrap = False
                'tempCol.ItemStyle.Width = Unit.Percentage(5%)
            Else
                'Format non-date columns as currency
                tempCol.HeaderText = cColumn.ColumnName
                tempCol.DataField = cColumn.ColumnName
                tempCol.DataFormatString = "{0:c0}"
                tempCol.HtmlEncode = False
                'tempCol.ItemStyle.Width = Unit.Percentage(10%)
                tempCol.ItemStyle.Wrap = False
                tempCol.ItemStyle.HorizontalAlign = HorizontalAlign.Right
            End If
            dgBooked.Columns.Add(tempCol)
        Next
        'Add spacer row?
        Dim dr1 As DataRow = ds.Tables("PipelineVsBooked").NewRow
        ds.Tables("PipelineVsBooked").Rows.Add(dr1)

        'Now try to add totals column
        Dim dr As DataRow = ds.Tables("PipelineVsBooked").NewRow
        For Each cColumn In ds.Tables("PipelineVsBooked").Columns
            If (cColumn.ColumnName <> "ReportMonthYear" And Not CBool(InStr(cColumn.ColumnName, "Running"))) Then
                dr.Item(cColumn.ColumnName) = ds.Tables("PipelineVsBooked").Compute("Sum([" & cColumn.ColumnName & "])", String.Empty)
            End If
        Next
        ds.Tables("PipelineVsBooked").Rows.Add(dr)



        dgBooked.DataBind()
        pnlgridview.Controls.Add(dgBooked)
        pnlgridview.Controls.Add(New LiteralControl("</br>"))
        '*****************************

    End Sub
    Private Sub CreateChart(ByVal inputDS As DataSet)
        SetMSChartDefaults(MSNChart1)
        MSNChart1.DataSource = inputDS
        MSNChart1.Height = Charty
        MSNChart1.Width = Chartx

        Select Case rblTypes.SelectedValue
            Case "Trend"
                MSNChart1.Titles(0).Text = "Pipeline Vs. Booked"
                MSNChart1.ChartAreas(0).AxisX.TextOrientation = DataVisualization.Charting.TextOrientation.Rotated90
                MSNChart1.ChartAreas(0).AxisY.Title = "Revenue"
                MSNChart1.ChartAreas(0).AxisY.TitleFont = TitleFont
                MSNChart1.ChartAreas(0).AxisY.LabelStyle.Format = "C0"
                AddYSeries("Running Pipeline Revenue", "RunningPipeline", True)
                AddYSeries("Running Booked Revenue", "RunningBooked", True)
            Case "Block"
                MSNChart1.Titles(0).Text = "Pipeline Vs. Booked"
                MSNChart1.ChartAreas(0).AxisX.TextOrientation = DataVisualization.Charting.TextOrientation.Rotated90
                MSNChart1.ChartAreas(0).AxisY.Title = "Revenue"
                MSNChart1.ChartAreas(0).AxisY.TitleFont = TitleFont
                MSNChart1.ChartAreas(0).AxisY.LabelStyle.Format = "C0"
                AddYSeries("Monthly Pipeline Revenue", "Pipeline", True)
                AddYSeries("Monthly Booked Revenue", "Booked", True)
            Case Else
                MSNChart1.Visible = False
        End Select
        MSNChart1.Visible = True
        MSNChart1.DataBind()
    End Sub

    Private Sub AddYSeries(ByVal strSeriesName As String, ByVal strSeriesY As String, ByVal bPrimaryAxis As Boolean)
        MSNChart1.Series.Add(strSeriesName)
        MSNChart1.Series(strSeriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(strSeriesName).XValueMember = "ReportMonthYear"
        MSNChart1.Series(strSeriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(strSeriesName).XValueMember = "ReportMonthYear"
        MSNChart1.Series(strSeriesName).YValueMembers = strSeriesY
        MSNChart1.Series(strSeriesName).ToolTip = "#VALY{C0}"
        If rblTypes.SelectedValue = "Trend" Then
            MSNChart1.Series(strSeriesName).ChartType = DataVisualization.Charting.SeriesChartType.Line
        Else
            MSNChart1.Series(strSeriesName).ChartType = DataVisualization.Charting.SeriesChartType.Column
        End If

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
    Sub GridViewSetWidth(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            'e.Row.Cells(1).Width = 500
            ' Display the company name in italics.
            ' e.Row.Cells(1).Text = "<i>" & e.Row.Cells(1).Text & "</i>"

        End If
    End Sub

    Protected Sub SetupInitialDate(ByVal sender As Object, ByVal e As System.EventArgs)
        cCommonDateCode.SetupInitialDate(sender, e)
    End Sub
    
End Class