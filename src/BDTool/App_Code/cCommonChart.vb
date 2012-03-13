Imports Microsoft.VisualBasic
Imports System.Drawing

Public Class cCommonChart
    Public Const Chartx As Integer = 1000
    Public Const Charty As Integer = 600
    'MSDN Chart Properties
    Public Shared myPerspective As Integer = 0 'Along Z Axis
    Public Shared myRotation As Integer = 30 'Along Y Axis
    Public Shared myInclination As Integer = 30 'Along X Axis
    Public Shared TitleFont As New System.Drawing.Font("Trebuchet MS", 8.25, FontStyle.Bold)
    Public Shared MainTitleFont As New System.Drawing.Font("Trebuchet MS", 12, FontStyle.Bold)
    Public Shared BorderColor As Color = Color.FromArgb(116, 118, 120) 'grey
    Public Shared BackColor As Color = Color.FromArgb(211, 208, 232) 'lght blue
    Public Shared InterlacedColor As Color = Color.FromArgb(128, 151, 144, 202) 'drk blue
    Public Shared ShadowColor As Color = Color.FromArgb(32, 0, 0, 0) 'black
    Public Shared ForecastShadowColor As Color = Color.FromArgb(64, 0, 0, 0)
    Public Shared ForecastColor As Color = Color.FromArgb(252, 180, 65)
    Public Shared TitleForeColor As Color = Color.FromArgb(54, 48, 100) 'greyisblue
    Public Shared b3D As Boolean = False
    Public Shared RangeBorderColor As Color = Color.FromArgb(180, 26, 59, 105)
    Public Shared RangeColor As Color = Color.FromArgb(128, 65, 140, 240)

    Public Shared Sub SetMSChartDefaults(ByVal myChart As DataVisualization.Charting.Chart)
        myChart.Titles(0).ShadowColor = ShadowColor
        myChart.Titles(0).Font = MainTitleFont
        myChart.Titles(0).ShadowOffset = "3"
        myChart.Titles(0).Alignment = ContentAlignment.MiddleLeft
        myChart.Titles(0).ForeColor = TitleForeColor

        'Legend
        myChart.Legends.Add("default")
        With myChart.Legends(0)
            .Enabled = True
            .IsTextAutoFit = True
            .BackColor = Color.Transparent
        End With
        myChart.Legends(0).Font = TitleFont

        'BorderSkin
        myChart.BorderSkin.SkinStyle = DataVisualization.Charting.BorderSkinStyle.Emboss
        myChart.BackColor = BackColor
        'Chart Area Configuration
        With myChart.ChartAreas(0)
            .BorderColor = BorderColor
            .BackSecondaryColor = Color.Transparent
            .BackColor = BackColor
            .ShadowColor = Color.Transparent
            .BackGradientStyle = DataVisualization.Charting.GradientStyle.TopBottom
            .AxisY.IsInterlaced = True
            .AxisY.InterlacedColor = InterlacedColor
        End With
        With myChart.ChartAreas(0).Area3DStyle
            .Rotation = myRotation
            .Perspective = myPerspective
            .Enable3D = b3D
            .Inclination = myInclination
            .IsRightAngleAxes = False
            .WallWidth = 0
            .IsClustered = False
        End With

    End Sub

End Class
