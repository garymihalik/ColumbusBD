Imports Microsoft.VisualBasic

Public Class cCommonDateCode
    Public Shared Sub SetupInitialDate(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim currentYear As Integer = 2007
        currentYear = DatePart(DateInterval.Year, Now)
        Dim x As Integer = 0
        If sender.ID = "cboPrior" Then
            For x = 0 To sender.items.count - 1
                If DatePart(DateInterval.Month, sender.items(x).value) = 1 And DatePart(DateInterval.Year, sender.Items(x).value) = currentYear Then
                    sender.items(x).selected = True
                    Exit For
                End If
            Next
        ElseIf sender.ID = "cboAfter" Then
            For x = 0 To sender.items.count - 1
                If DatePart(DateInterval.Month, sender.items(x).value) = 12 And DatePart(DateInterval.Year, sender.Items(x).value) = currentYear Then
                    sender.items(x).selected = True
                    Exit For
                End If
            Next
        ElseIf sender.ID = "cboReportPeriod" Or sender.id = "cboReportMonth" Then
            Dim currentMonth As Integer = DatePart(DateInterval.Month, Now)
            For x = 0 To sender.items.count - 1
                If DatePart(DateInterval.Month, sender.items(x).text) = currentMonth And DatePart(DateInterval.Year, sender.Items(x).text) = currentYear Then
                    sender.items(x).selected = True
                    Exit For
                End If
            Next
        ElseIf sender.ID = "cboCurrentMonth" Then
            Dim currentMonth As Integer = DatePart(DateInterval.Month, Now)
            For x = 0 To sender.items.count - 1
                If DatePart(DateInterval.Month, sender.items(x).text) = currentMonth And DatePart(DateInterval.Year, sender.Items(x).text) = currentYear Then
                    sender.items(x).selected = True
                    Exit For
                End If
            Next
        End If

    End Sub
End Class
