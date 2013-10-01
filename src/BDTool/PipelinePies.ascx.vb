Imports System.Web.UI.DataVisualization.Charting
Imports cCommonChart
Imports cCommon
Imports System.Drawing
Imports System.Data
Imports cDataSetManipulation
Partial Class PipelinePies
    Inherits System.Web.UI.UserControl
    Public strSQL As String = ""
    Public strItemChartType As String = ""
    Public strYType As String = ""
    Public dblBU_ID As Double
    Private strchartSize As String = "600x400"
#Region "Properties"
    Public Property BusinessUnit() As Double
        Get
            Return dblBU_ID
        End Get
        Set(ByVal value As Double)
            dblBU_ID = value
        End Set
    End Property
    Public Property ItemChartType() As String
        Get
            Return strItemChartType
        End Get
        Set(ByVal value As String)
            strItemChartType = value
        End Set
    End Property
    Public Property RevenueType() As String
        Get
            Return strYType
        End Get
        Set(ByVal value As String)
            strYType = value
        End Set
    End Property
    Public Property ChartSize() As String
        Get
            Return strchartSize
        End Get
        Set(ByVal value As String)
            strchartSize = value
        End Set
    End Property
#End Region
    Public Sub CreateChart()
        'New MSDN Chart Control.....
        'Data
        Dim myData As DataSet = GetSingleDataset(GetChartSQL())
        Dim firstView As DataView = New DataView(myData.Tables(0))
        'Chart Title
        Chart1.Titles().Add("Pipeline Category Breakdown")
        Chart1.Titles(0).ShadowColor = ShadowColor
        Chart1.Titles(0).Font = MainTitleFont
        Chart1.Titles(0).ShadowOffset = "3"
        Chart1.Titles(0).Alignment = ContentAlignment.MiddleLeft
        Chart1.Titles(0).ForeColor = TitleForeColor

        'Legend
        Chart1.Legends.Add("default")
        With Chart1.Legends(0)
            .Enabled = True
            .IsTextAutoFit = True
            .BackColor = Color.Transparent
        End With
        Chart1.Legends(0).Font = TitleFont

        'BorderSkin
        Chart1.BorderSkin.SkinStyle = BorderSkinStyle.Emboss

        'Chart Area Configuration
        With Chart1.ChartAreas(0)
            .BorderColor = BorderColor
            .BackSecondaryColor = Color.Transparent
            .BackColor = BackColor
            .ShadowColor = Color.Transparent
            .BackGradientStyle = GradientStyle.TopBottom
        End With
        With Chart1.ChartAreas(0).Area3DStyle
            .Rotation = -21
            .Perspective = 10
            .Enable3D = True
            .Inclination = 48
            .IsRightAngleAxes = False
            .WallWidth = 0
            .IsClustered = False
        End With


        'Final Chart Details
        Chart1.DataSource = myData
        Chart1.Series(0).XValueType = ChartValueType.String
        Chart1.Series(0).YValuesPerPoint = 1
        If strYType = "Estimated" Then
            Chart1.Series(0).YValueMembers = "ERevenue"
        ElseIf strYType = "Weighted" Then
            Chart1.Series(0).YValueMembers = "WRevenue"
        ElseIf strYType = "Count" Then
            Chart1.Series(0).YValueMembers = "NumberOfItems"
        End If

        Chart1.Series(0).IsValueShownAsLabel = False
        Chart1.Series(0).Label = " "
        Chart1.Series(0).LegendText = "#VALX"
        If strYType <> "Count" Then
            Chart1.Series(0).LegendToolTip = "#VALX: #VALY{C0} - #PERCENT"
        Else
            Chart1.Series(0).LegendToolTip = "#VALX: #VALY{G0} - #PERCENT"
        End If
        Chart1.Series(0).XValueMember = "ByType"
        Select Case strItemChartType.ToUpper
            Case "ANCHOR"
                Chart1.Titles(0).Text = "By Anchor/Secondary"
            Case "HUNTED"
                Chart1.Titles(0).Text = "By Hunted/Farmed"
            Case "NEW"
                Chart1.Titles(0).Text = "By Extension/New"
            Case Else
                Chart1.Series(0).Points.DataBindXY(firstView, "", firstView, "")
                Chart1.Titles(0).Text = "Invalid Chart"
        End Select
        Chart1.DataBind()
        Chart1.Legends(0).Docking = DataVisualization.Charting.Docking.Bottom
        Chart1.Series(0).ChartType = DataVisualization.Charting.SeriesChartType.Pie
        Dim s1 As System.Web.UI.DataVisualization.Charting.Series
        For Each s1 In Chart1.Series
            If strYType <> "Count" Then
                s1.ToolTip = "#VALX - #VALY{C0} - #PERCENT"
            Else
                s1.ToolTip = "#VALX - #VALY{G0} - #PERCENT"
            End If

            ' s1.Url = "actionitemmanagement1.aspx?tempListId=#VALY2&tempListType=" & strEntityType & "&desc=#VALX"
        Next
        Chart1.Height = Left(strchartSize, InStr(strchartSize, "x") - 1)
        Chart1.Width = Right(strchartSize, Len(strchartSize) - InStrRev(strchartSize, "x"))

        CreateGridView(myData)
    End Sub
    Private Function GetChartSQL() As String
        'available y_columns must be WRevenue, Erevenue, NumberOfItems, x_Columns=ByType
        Select Case strItemChartType.ToUpper
            Case "ANCHOR"
                strSQL = "SELECT Sum(qryWorkingPipeline.WeightedRevenue) AS WRevenue, IIF(Anchor,'Anchor','Secondary') as ByType, Sum(qryWorkingPipeline.EstimatedRevenue) AS ERevenue, Count(qryWorkingPipeline.PK_OpportunityID) AS NumberOfItems FROM qryWorkingPipeline where BusinessUnit=" & dblBU_ID & " GROUP BY qryWorkingPipeline.Anchor "
            Case "HUNTED"
                strSQL = "SELECT Sum(qryWorkingPipeline.WeightedRevenue) AS WRevenue, Source as ByType, Sum(qryWorkingPipeline.EstimatedRevenue) AS ERevenue, Count(qryWorkingPipeline.PK_OpportunityID) AS NumberOfItems FROM qryWorkingPipeline where BusinessUnit=" & dblBU_ID & " GROUP BY qryWorkingPipeline.Source "
            Case "NEW"
                strSQL = "SELECT Sum(qryWorkingPipeline.WeightedRevenue) AS WRevenue, IIF(Extension,'Extension','New') as ByType, Sum(qryWorkingPipeline.EstimatedRevenue) AS ERevenue, Count(qryWorkingPipeline.PK_OpportunityID) AS NumberOfItems FROM qryWorkingPipeline where BusinessUnit=" & dblBU_ID & " GROUP BY qryWorkingPipeline.Extension "
            Case Else
                strSQL = "SELECT Sum(qryWorkingPipeline.WeightedRevenue) AS WRevenue, IIF(Anchor,'Anchor','Secondary') as ByType, Sum(qryWorkingPipeline.EstimatedRevenue) AS ERevenue, Count(qryWorkingPipeline.PK_OpportunityID) AS NumberOfItems FROM qryWorkingPipeline where BusinessUnit=" & dblBU_ID & " GROUP BY qryWorkingPipeline.Anchor "
        End Select
        Return strSQL
    End Function

    Private Sub CreateGridView(ByVal ds As DataSet)
        Dim dg As New GridView
        dg.DataSource = ds
        dg.AutoGenerateColumns = False
        
        Select Case strItemChartType.ToUpper
            Case "ANCHOR"
                AddNewColumn("{0:g}", "Anchor/Secondary", "ByType", dg, False)
                If strYType = "Estimated" Then
                    AddNewColumn("{0:c0}", "Estimated Revenue", "ERevenue", dg, True)
                ElseIf strYType = "Weighted" Then
                    AddNewColumn("{0:c0}", "Weighted Revenue", "WRevenue", dg, True)
                ElseIf strYType = "Count" Then
                    AddNewColumn("{0:g}", "Count", "NumberOfItems", dg, True)
                End If
            Case "HUNTED"
                AddNewColumn("{0:g}", "Hunted/Farmed", "ByType", dg, False)
                If strYType = "Estimated" Then
                    AddNewColumn("{0:c0}", "Estimated Revenue", "ERevenue", dg, True)
                ElseIf strYType = "Weighted" Then
                    AddNewColumn("{0:c0}", "Weighted Revenue", "WRevenue", dg, True)
                ElseIf strYType = "Count" Then
                    AddNewColumn("{0:g}", "Count", "NumberOfItems", dg, True)
                End If
            Case "NEW"
                AddNewColumn("{0:g}", "New Work/Extension", "ByType", dg, False)
                If strYType = "Estimated" Then
                    AddNewColumn("{0:c0}", "Estimated Revenue", "ERevenue", dg, True)
                ElseIf strYType = "Weighted" Then
                    AddNewColumn("{0:c0}", "Weighted Revenue", "WRevenue", dg, True)
                ElseIf strYType = "Count" Then
                    AddNewColumn("{0:g}", "Count", "NumberOfItems", dg, True)
                End If
        End Select
        Dim dr As DataRow = ds.Tables(0).NewRow
        For Each cColumn In ds.Tables(0).Columns
            If cColumn.ColumnName = "ERevenue" Or cColumn.ColumnName = "WRevenue" Or cColumn.ColumnName = "NumberOfItems" Then
                dr.Item(cColumn.ColumnName) = ds.Tables(0).Compute("Sum([" & cColumn.ColumnName & "])", String.Empty)
            End If
        Next
        ds.Tables(0).Rows.Add(dr)

        dg.DataBind()
        CType(Me.FindControl("pnlgridview"), Panel).Controls.Add(dg)
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
