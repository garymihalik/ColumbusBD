Imports System.Data
Partial Class BUControl
    Inherits System.Web.UI.UserControl
    Private intBU_ID As Integer
    Public Property BU_ID() As Integer
        Get
            intBU_ID = lblLastSelectedBUID.Text
            Return intBU_ID
        End Get
        Set(ByVal value As Integer)
            intBU_ID = value
            lblLastSelectedBUID.Text = intBU_ID
        End Set
    End Property

    Private txtBU As String
    Public Property BU_Text() As String
        Get
            txtBU = lblLastSelectedBUText.Text
            Return txtBU
        End Get
        Set(ByVal value As String)
            txtBU = value
            lblLastSelectedBUText.Text = txtBU
        End Set
    End Property

    Public Event BUSelectionChanged As EventHandler

    Protected Sub cboBU_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboBU.DataBound
        PreSelectBU()
        BU_ID = cboBU.SelectedValue
        BU_Text = cboBU.SelectedItem.Text
        If cboBU.Items.Count = 1 Then
            cboBU.Visible = "false"
        End If
    End Sub

    Protected Sub cboBU_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboBU.SelectedIndexChanged
        BU_ID = cboBU.SelectedValue
        BU_Text = cboBU.SelectedItem.Text
        cCookie.AddValue("UserSettings", "LastSelectedBU", cboBU.SelectedValue)
        RaiseEvent BUSelectionChanged(sender, e)
    End Sub
    Protected Sub PreSelectBU()
        Dim tempID As String = cCookie.ReadSingleCookieValue("UserSettings", "LastSelectedBU")
        Dim x As Integer
        If CBool(InStr(tempID, "Error")) Then
            BU_ID = cboBU.SelectedValue
            BU_Text = cboBU.SelectedItem.Text
            cCookie.AddValue("UserSettings", "LastSelectedBU", cboBU.SelectedValue)
        Else
            For x = 0 To cboBU.Items.Count - 1
                If CStr(cboBU.Items(x).Value) = tempID Then
                    cboBU.Items(x).Selected = True
                End If
            Next
        End If

    End Sub

    Protected Sub ODS_BU_Selecting(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.ObjectDataSourceSelectingEventArgs) Handles ODS_BU.Selecting
        e.InputParameters("strBU") = cCookie.ReadSingleCookieValue("UserSettings", "BU")
    End Sub

End Class
