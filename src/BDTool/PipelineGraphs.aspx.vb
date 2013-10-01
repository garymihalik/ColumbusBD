Imports System.Data
Imports cCommonChart
Imports System.Drawing

Partial Class PipelineGraphs
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
                cCommon.InsertPageVisit(User.Identity.Name, Request.UserHostAddress, Request.RawUrl, "", "")
            End If
        End If

    End Sub

    Protected Sub btnUpdateChart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateChart.Click
        If rblTypes.SelectedValue = "Pipeline" Then
            CreatePipelineTrend()
        ElseIf rblTypes.SelectedValue = "WinLoss" Then
            CreateWinLossTrend()
        End If
        'cPipelineData.GetPipelineBrokenDownData(cboPrior.SelectedValue, cboAfter.SelectedValue)

    End Sub
    Private Sub SetMSNChartDateBased()
        MSNChart1.Titles(0).ShadowColor = ShadowColor
        MSNChart1.Titles(0).Font = MainTitleFont
        MSNChart1.Titles(0).ShadowOffset = "3"
        MSNChart1.Titles(0).Alignment = ContentAlignment.MiddleLeft
        MSNChart1.Titles(0).ForeColor = TitleForeColor

        'Legend
        MSNChart1.Legends.Add("default")
        With MSNChart1.Legends(0)
            .Enabled = True
            .IsTextAutoFit = True
            .BackColor = Color.Transparent
        End With
        MSNChart1.Legends(0).Font = TitleFont

        'BorderSkin
        MSNChart1.BorderSkin.SkinStyle = DataVisualization.Charting.BorderSkinStyle.Emboss
        MSNChart1.BackColor = BackColor
        'Chart Area Configuration
        With MSNChart1.ChartAreas(0)
            .BorderColor = BorderColor
            .BackSecondaryColor = Color.Transparent
            .BackColor = BackColor
            .ShadowColor = Color.Transparent
            .BackGradientStyle = DataVisualization.Charting.GradientStyle.TopBottom
            .AxisY.IsInterlaced = True
            .AxisY.InterlacedColor = InterlacedColor
        End With
        With MSNChart1.ChartAreas(0).Area3DStyle
            .Rotation = myRotation
            .Perspective = myPerspective
            .Enable3D = b3D
            .Inclination = myInclination
            .IsRightAngleAxes = False
            .WallWidth = 0
            .IsClustered = False
        End With

    End Sub
    Private Sub CreateGridView(ByVal ds As DataSet)
        Dim dg As New GridView
        dg.DataSource = ds

        dg.AutoGenerateColumns = False
        Dim ReportDate As New BoundField
        With ReportDate
            .HeaderText = "Week"
            .DataField = "WeeklyReportDates"
            .DataFormatString = "{0:MM/dd/yy}"
            .HtmlEncode = False
            .ItemStyle.HorizontalAlign = HorizontalAlign.Right
        End With
        dg.Columns.Add(ReportDate)

        Select Case cboGraphType.SelectedValue
            Case "AnchorMoney"
                AddNewColumn("{0:c0}", "Anchor Estimated Revenue", "AnchorEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Secondary Estimated Revenue", "SecondaryEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Anchor Weighted Revenue", "AnchorWeightedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Anchor Weighted Revenue", "SecondaryWeightedRevenue", dg, True)

                AddNewColumn("{0:c0}", "Total Pipeline Revenue", "TotalEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Weighted Revenue", "TotalWeightedRevenue", dg, True)
            Case "HuntedMoney"
                AddNewColumn("{0:c0}", "Hunted Estimated Revenue", "HuntedEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Farmed Estimated Revenue", "FarmedEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Hunted Weighted Revenue", "HuntedWeightedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Farmed Weighted Revenue", "FarmedWeightedRevenue", dg, True)

                AddNewColumn("{0:c0}", "Total Pipeline Revenue", "TotalEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Weighted Revenue", "TotalWeightedRevenue", dg, True)
            Case "NewMoney"
                AddNewColumn("{0:c0}", "New Estimated Revenue", "NewEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Extension Estimated Revenue", "ExtensionEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "New Weighted Revenue", "NewWeightedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Extension Weighted Revenue", "ExtensionWeightedRevenue", dg, True)

                AddNewColumn("{0:c0}", "Total Pipeline Revenue", "TotalEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Weighted Revenue", "TotalWeightedRevenue", dg, True)
            Case Else
                AddNewColumn("{0:c0}", "Total Pipeline Revenue", "TotalEstimatedRevenue", dg, True)
                AddNewColumn("{0:c0}", "Weighted Revenue", "TotalWeightedRevenue", dg, True)
                AddNewColumn("{0:p2}", "Pipeline Confidenc", "TotalPipelineConfidence", dg, True)
        End Select



        dg.DataBind()
        pnlgridview.Controls.Add(dg)
    End Sub
    Private Sub CreatePipelineTrend()
        Dim ds As System.Data.DataSet
        ds = cPipelineData.GetPipelineBrokenDownDataByBU(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID)


        MSNChart1.DataSource = ds
        MSNChart1.Height = Charty
        MSNChart1.Width = Chartx

        MSNChart1.Titles(0).Text = "Pipeline"
        MSNChart1.ChartAreas(0).AxisY.Title = "Revenue"
        MSNChart1.ChartAreas(0).AxisY.TitleFont = TitleFont
        MSNChart1.ChartAreas(0).AxisY.LabelStyle.Format = "C0"

        Select Case cboGraphType.SelectedValue
            Case "AnchorMoney"
                AddYSeries("Estimated Anchor Revenue", "AnchorEstimatedRevenue", True)
                AddYSeries("Estimated Secondary Revenue", "SecondaryEstimatedRevenue", True)
                AddYSeries("Weighted Anchor Revenue", "AnchorWeightedRevenue", True)
                AddYSeries("Weighted Secondary Revenue", "SecondaryWeightedRevenue", True)
            Case "HuntedMoney"
                AddYSeries("Estimated Hunted Revenue", "HuntedEstimatedRevenue", True)
                AddYSeries("Estimated Farmed Revenue", "FarmedEstimatedRevenue", True)
                AddYSeries("Weighted Hunted Revenue", "HuntedWeightedRevenue", True)
                AddYSeries("Weighted Farmed Revenue", "FarmedWeightedRevenue", True)
            Case "NewMoney"
                AddYSeries("Estimated New Revenue", "NewEstimatedRevenue", True)
                AddYSeries("Estimated Extension Revenue", "ExtensionEstimatedRevenue", True)
                AddYSeries("Weighted New Revenue", "NewWeightedRevenue", True)
                AddYSeries("Weighted Extension Revenue", "ExtensionWeightedRevenue", True)
            Case Else
                AddYSeries("Estimated Revenue", "TotalEstimatedRevenue", True)
                AddYSeries("Weighted Revenue", "TotalWeightedRevenue", True)
                AddYSeries("Pipeline Confidence", "TotalPipelineConfidence", False)
        End Select



        MSNChart1.Visible = True
        MSNChart1.DataBind()
        SetMSNChartDateBased()
        CreateGridView(ds)
    End Sub

    Private Sub CreateWinLossTrend()
        Dim ds As System.Data.DataSet
        'ds = cPipelineData.GetWinLossTrend(cboPrior.SelectedValue, cboAfter.SelectedValue)
        ds = cPipelineData.GetWinLossBrokenDownDataByBU(cboPrior.SelectedValue, cboAfter.SelectedValue, BU1.BU_ID)
        MSNChart1.DataSource = ds
        MSNChart1.Height = Charty
        MSNChart1.Width = Chartx

        MSNChart1.Titles(0).Text = "Win/Loss Trend"
        MSNChart1.ChartAreas(0).AxisY.Title = "Revenue"
        MSNChart1.ChartAreas(0).AxisY.TitleFont = TitleFont
        MSNChart1.ChartAreas(0).AxisY.LabelStyle.Format = "C0"

        Select Case cboGraphType.SelectedValue
            Case "AnchorMoney"
                AddYSeries("Won Anchor Revenue", "AnchorWin", True)
                AddYSeries("Lost Anchor Revenue", "AnchorLoss", True)
                AddYSeries("Won Secondary Revenue", "SecondaryWin", True)
                AddYSeries("Lost Secondary Revenue", "SecondaryLoss", True)
            Case "HuntedMoney"
                AddYSeries("Won Hunted Revenue", "HuntedWin", True)
                AddYSeries("Lost Hunted Revenue", "HuntedLoss", True)
                AddYSeries("Won Farmed Revenue", "FarmedWin", True)
                AddYSeries("Lost Farmed Revenue", "FarmedLoss", True)
            Case "NewMoney"
                AddYSeries("Won New Revenue", "NewWin", True)
                AddYSeries("Lost New Revenue", "NewLoss", True)
                AddYSeries("Won Extension Revenue", "ExtensionWin", True)
                AddYSeries("Lost Extension Revenue", "ExtensionLoss", True)
            Case Else
                AddYSeries("Won Revenue", "CumWin", True)
                AddYSeries("Lost Revenue", "CumLoss", True)
                AddYSeries("Win Lost Ratio", "CumWinLossRatio", False)
        End Select




        MSNChart1.Visible = True
        MSNChart1.DataBind()
        SetMSNChartDateBased()
    End Sub
    Private Sub AddYSeries(ByVal strSeriesName As String, ByVal strSeriesY As String, ByVal bPrimaryAxis As Boolean)
        MSNChart1.Series.Add(strSeriesName)
        MSNChart1.Series(strSeriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(strSeriesName).XValueMember = "WeeklyReportDates"
        MSNChart1.Series(strSeriesName).XValueType = DataVisualization.Charting.ChartValueType.Date
        MSNChart1.Series(strSeriesName).XValueMember = "WeeklyReportDates"
        MSNChart1.Series(strSeriesName).YValueMembers = strSeriesY
        MSNChart1.Series(strSeriesName).ToolTip = "#VALY{C0}"
        MSNChart1.Series(strSeriesName).ChartType = DataVisualization.Charting.SeriesChartType.Line
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
    Private Sub AddNewColumn(ByVal strFormat As String, ByVal strName As String, ByVal strBoundField As String, ByVal dgInput As GridView, ByVal bAlignRight As Boolean)
        Dim tempCol As New BoundField
        With tempCol
            .HeaderText = strName
            .DataField = strBoundField
            .DataFormatString = strFormat
            If bAlignRight Then
                .ItemStyle.HorizontalAlign = HorizontalAlign.Right
            End If
            .HtmlEncode = False
        End With
        dgInput.Columns.Add(tempCol)

    End Sub
End Class