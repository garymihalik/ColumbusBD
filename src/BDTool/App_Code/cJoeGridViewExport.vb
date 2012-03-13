Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

'For the ExportGridView class
'Imports System.Data
'Imports System.Configuration
Imports System.IO
Imports System.Web.Security
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls 'Till here
Namespace Joe1.Web.Controls
    <DefaultProperty("Text"), ToolboxData("<{0}:ExportToExcel Charset=""utf-8"" ContentEncoding=""windows-1250"" Text=""Export To Excel"" EnableHyperLinks=""True"" ApplyStyleInExcel=""True"" IncludeTimeStamp=""True"" ExportFileName=""FileName.xls"" PageSize=""All"" runat=server></{0}:ExportToExcel>" & vbNewLine & "<!--Author: Rajesh K, Copyright 2007 All rights reserved. Use this control at your own risk.  Author is not responsible for any defects or damages to your code.-->" & vbNewLine)> _
    Public Class ExportToExcelXML
        Inherits System.Web.UI.WebControls.Button
        Implements IPostBackEventHandler
        Public Shadows Event OnClick(ByVal Sender As Object, ByVal e As EventArgs)
        Public Event PreExport(ByVal Sender As Object, ByVal e As EventArgs)
        Private m_GridView As GridView
        <DefaultValue(""), Localizable(True), Category("Xcel Export Settings"), Description("If you have more than one 'GridView' in your page, set the ID of the GridView you want to export. If you just have one GridView, leave this blank.")> _
        Public Property GridViewID() As String
            Get
                Return ViewState("m_GridViewID")
            End Get
            Set(ByVal value As String)
                ViewState("m_GridViewID") = value
            End Set
        End Property
        <DefaultValue(""), Localizable(True), Category("Xcel Export Settings"), Description("Enter comma separated column(s) to exclude [0 based index] in the exported file. E.g.) 0,2,9. If you need all columns, leave this blank.")> _
        Public Property ColumnsToExclude() As String
            Get
                Return ViewState("m_ColumnsToExclude")
            End Get
            Set(ByVal value As String)
                ViewState("m_ColumnsToExclude") = value
            End Set
        End Property
        <DefaultValue("utf-8"), Localizable(True), Category("Xcel Export Settings"), Description("Charset of the output.")> _
        Public Property Charset() As String
            Get
                Return ViewState("m_Charset")
            End Get
            Set(ByVal value As String)
                ViewState("m_Charset") = value
            End Set
        End Property
        <DefaultValue("windows-1250"), Localizable(True), Category("Xcel Export Settings"), Description("Content Encoding.")> _
        Public Property ContentEncoding() As String
            Get
                Return ViewState("m_ContentEncoding")
            End Get
            Set(ByVal value As String)
                ViewState("m_ContentEncoding") = value
            End Set
        End Property
        <DefaultValue("FileName.xls"), Category("Xcel Export Settings"), Localizable(True), Description("This will be the file name of the generated file.")> _
        Public Property ExportFileName() As String
            Get
                Return ViewState("m_FileName")
            End Get
            Set(ByVal value As String)
                If value = "" Then
                Else
                    If Len(value) > 4 Then
                        If LCase(Right(value, 4)) = ".xls" Then
                        Else
                            Throw New Exception("Please enter a file name with '.xls' extension")
                            Exit Property
                        End If
                    Else
                        Throw New Exception("Please enter a file name with '.xls' extension")
                        Exit Property
                    End If
                End If
                ViewState("m_FileName") = Replace(value, " ", "_")
            End Set
        End Property
        <DefaultValue("True"), Category("Xcel Export Settings"), Localizable(True), Description("If set to 'True', hyperlink anchor tags <a href=..> will be enabled in the exported excel.")> _
        Public Property EnableHyperLinks() As String
            Get
                Return ViewState("m_EnableHyperLinks")
            End Get
            Set(ByVal value As String)
                If UCase(value) = "TRUE" Or UCase(value) = "YES" Then
                    ViewState("m_EnableHyperLinks") = True
                Else
                    ViewState("m_EnableHyperLinks") = False
                End If

            End Set
        End Property
        <DefaultValue("True"), Category("Xcel Export Settings"), Localizable(True), Description("If set to 'True', the GridView styles will be applied while exporting.")> _
        Public Property ApplyStyleInExcel() As String
            Get
                Return ViewState("m_ApplyStyleInExcel")
            End Get
            Set(ByVal value As String)
                If UCase(value) = "TRUE" Or UCase(value) = "YES" Then
                    ViewState("m_ApplyStyleInExcel") = True
                Else
                    ViewState("m_ApplyStyleInExcel") = False
                End If

            End Set
        End Property
        <DefaultValue("True"), Category("Xcel Export Settings"), Localizable(True), Description("Set this to True if you would like a timestamp to be appended to the filename.")> _
        Public Property IncludeTimeStamp() As String
            Get
                Return ViewState("m_IncludeTimeStamp").ToString
            End Get
            Set(ByVal value As String)
                If UCase(value) = "TRUE" Or UCase(value) = "YES" Then
                    ViewState("m_IncludeTimeStamp") = True
                Else
                    ViewState("m_IncludeTimeStamp") = False
                End If
            End Set
        End Property
        <DefaultValue("All"), Category("Xcel Export Settings"), Localizable(True), Description("Enter number of records to export.  To export the entire grid, set this to 'All'.  To export only the current page, set this to 'Current'.")> _
        Public Property PageSize() As String
            Get
                Return ViewState("m_PageSize")
            End Get
            Set(ByVal value As String)
                If LCase(value) = "all" Or LCase(value) = "current" Then
                ElseIf IsNumeric(value) Then
                Else
                    Throw New Exception("Please enter 'All' or 'Current' or a valid number.")
                    Exit Property
                End If
                ViewState("m_PageSize") = value
            End Set
        End Property

        Protected Overrides Sub RenderContents(ByVal output As HtmlTextWriter)
            'output.Write("Author: Rajesh K, Copyright &#0169 2007 All rights reserved.")
            'Me.EnableViewState = True
            output.Write("<!--Author: Rajesh K, Copyright 2007 All rights reserved. Use this control at your own risk.  Author is not responsible for any defects or damages to your code.-->")
        End Sub

        Private Sub ExportToExcel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
            RaiseEvent OnClick(sender, e)
        End Sub


        Private Sub ExportToExcel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                m_GridView = FindControlRecursive(Me.Page, ViewState("m_GridViewID"))
            Catch ex As Exception

            End Try

        End Sub
        Private Sub ExportToExcel_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            Try
                If m_GridView Is Nothing OrElse m_GridView.Rows.Count = 0 Then
                    Me.Enabled = False
                Else
                    Me.Enabled = True
                End If
            Catch ex As Exception

            End Try
        End Sub


        Private Sub ExportToExcel_OnClick(ByVal Sender As Object, ByVal e As System.EventArgs) Handles Me.OnClick
            Dim SortDirection As SortDirection = m_GridView.SortDirection
            Dim SortExpression As String = m_GridView.SortExpression


            Select Case UCase(PageSize)
                Case "ALL"
                    m_GridView.AllowPaging = False
                    m_GridView.DataBind()
                    If SortExpression = "" Then
                    Else
                        m_GridView.Sort(SortExpression, SortDirection)
                    End If
                Case "CURRENT"
                    'No action needed
                Case Else
                    If IsNumeric(PageSize) Then
                        m_GridView.PageSize = CInt(PageSize)
                        m_GridView.DataBind()
                    End If
            End Select
            Dim TempFileName As String = ExportFileName
            If UCase(IncludeTimeStamp) = "YES" Or UCase(IncludeTimeStamp) = "TRUE" Then

                Dim FileExtension As String = ""
                If ExportFileName = "" Then
                    ExportFileName = "FileName.xls"
                End If
                FileExtension = Right(ExportFileName, 4)
                Try
                    TempFileName = Left(ExportFileName, Len(ExportFileName) - 4) & Format(Now, "yyyy_MM_dd_HH_mm_ss") & FileExtension
                Catch ex As Exception
                    TempFileName = ExportFileName
                End Try
            End If

            Dim sTemp() As String
            Dim sSortedList As New SortedList(Of String, Integer)
            If ColumnsToExclude = "" Then
            Else
                Try
                    sTemp = Split(ColumnsToExclude, ",")
                    For i As Integer = 0 To sTemp.Length - 1
                        If IsNumeric(Trim(sTemp(i))) Then
                            sSortedList.Add(Trim(sTemp(i)), CInt(Trim(sTemp(i))))
                        End If
                    Next
                Catch ex As Exception
                End Try

            End If

            If m_GridView IsNot Nothing Then
                Me.Enabled = True
                If EnableHyperLinks = "" Then
                    EnableHyperLinks = "False"
                End If
                If ApplyStyleInExcel = "" Then
                    ApplyStyleInExcel = "False"
                End If
                ExportGridView.Export(TempFileName, m_GridView, CBool(ApplyStyleInExcel), CBool(EnableHyperLinks), sSortedList, ContentEncoding, Charset)
            Else
                Me.Enabled = False
            End If
            'RaiseEvent OnClick(Sender, e)
        End Sub

        Private Function FindControlRecursive(ByVal RootControl As Control, ByVal id As String) As Control
            'If root.ID = id Then
            If GetType(GridView).ToString = RootControl.GetType.ToString Then
                If id = "" Then
                    Return RootControl
                ElseIf UCase(RootControl.ID) = UCase(id) Then
                    Return RootControl
                Else
                    Return Nothing
                End If

            End If

            For Each ChildControl As Control In RootControl.Controls
                Dim localControl As Control = FindControlRecursive(ChildControl, id)
                If Not localControl Is Nothing Then
                    Return localControl
                    Exit Function
                End If
            Next
            Return Nothing
        End Function
        Protected Overrides Sub RaisePostBackEvent(ByVal eventArgument As String)
            RaiseEvent PreExport(Me, New EventArgs)
            MyBase.RaisePostBackEvent(eventArgument)
        End Sub
        Protected Class ExportGridView
            Public Shared Sub Export(ByVal fileName As String, ByVal gv As GridView, ByVal ApplyStyling As Boolean, ByVal EnableHyperLinks As Boolean, ByVal ColumnsToExclude As SortedList(Of String, Integer), ByVal ContentEncoding As String, ByVal Charset As String)
                HttpContext.Current.Response.Clear()
                HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", fileName))
                HttpContext.Current.Response.ContentType = "application/ms-excel"
                Try
                    HttpContext.Current.Response.Charset = charset
                Catch ex As Exception
                    HttpContext.Current.Response.Charset = "utf-8"
                End Try

                Try
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding(ContentEncoding.ToString)
                Catch ex As Exception
                    HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.GetEncoding("windows-1250")
                End Try


                Dim sw As StringWriter = New StringWriter
                Dim htw As HtmlTextWriter = New HtmlTextWriter(sw)
                '  Create a form to contain the grid

                Dim table As Table = New Table
                table.GridLines = gv.GridLines
                SetStyles(table, gv, ApplyStyling)

                'This does not work
                'If ApplyStyling Then
                '    table.SkinID = gv.SkinID
                'End If
                '  Header of the table
                If (Not (gv.HeaderRow) Is Nothing) Then
                    ExportGridView.Massage(gv.HeaderRow, EnableHyperLinks, ColumnsToExclude)
                    Dim i As Integer = 0
                    i = table.Rows.Add(gv.HeaderRow)
                    SetStyles(table.Rows(i), gv.HeaderStyle, ApplyStyling)
                End If
                '  Data rows of the table
                For Each row As GridViewRow In gv.Rows
                    Dim i As Integer = 0
                    ExportGridView.Massage(row, EnableHyperLinks, ColumnsToExclude)
                    i = table.Rows.Add(row)
                    If i Mod 2 = 0 Then
                        SetStyles(table.Rows(i), gv.RowStyle, ApplyStyling)
                    Else
                        SetStyles(table.Rows(i), gv.AlternatingRowStyle, ApplyStyling)
                    End If

                Next
                '  Footer of the table
                If (Not (gv.FooterRow) Is Nothing) Then
                    Dim i As Integer = 0
                    ExportGridView.Massage(gv.FooterRow, EnableHyperLinks, ColumnsToExclude)
                    i = table.Rows.Add(gv.FooterRow)
                    SetStyles(table.Rows(i), gv.FooterStyle, ApplyStyling)
                End If
                '  render the table into the htmlwriter
                table.RenderControl(htw)
                '  render the htmlwriter into the response
                HttpContext.Current.Response.Write(sw.ToString)
                HttpContext.Current.Response.End()
            End Sub
            Private Shared Sub Massage(ByVal localControl As Control, ByVal EnableHyperLinks As Boolean, ByVal ColumnsToExclude As SortedList(Of String, Integer))
                Dim iCount As Integer = 0
                Do While (iCount < localControl.Controls.Count)
                    Dim currentControl As Control = localControl.Controls(iCount)
                    If ColumnsToExclude.ContainsValue(iCount) AndAlso (currentControl.GetType.ToString = "System.Web.UI.WebControls.DataControlFieldHeaderCell" OrElse currentControl.GetType.ToString = "System.Web.UI.WebControls.DataControlFieldCell") Then
                        currentControl.Visible = False
                        iCount += 1
                        Continue Do
                    End If
                    If (TypeOf currentControl Is LinkButton) Then
                        localControl.Controls.Remove(currentControl)
                        localControl.Controls.AddAt(iCount, New LiteralControl(CType(currentControl, LinkButton).Text))
                    ElseIf (TypeOf currentControl Is DropDownList) Then
                        localControl.Controls.Remove(currentControl)
                        localControl.Controls.AddAt(iCount, New LiteralControl(CType(currentControl, DropDownList).SelectedItem.Text))
                    ElseIf (TypeOf currentControl Is HyperLink) Then
                        If Not CBool(EnableHyperLinks) Then
                            localControl.Controls.Remove(currentControl)
                            localControl.Controls.AddAt(iCount, New LiteralControl(CType(currentControl, HyperLink).Text))
                        End If
                    ElseIf (TypeOf currentControl Is HtmlAnchor) Then
                        If Not CBool(EnableHyperLinks) Then
                            localControl.Controls.Remove(currentControl)
                            localControl.Controls.AddAt(iCount, New LiteralControl(CType(currentControl, HtmlAnchor).InnerText))
                        End If
                    ElseIf (TypeOf currentControl Is CheckBox) Then
                        localControl.Controls.Remove(currentControl)
                        localControl.Controls.AddAt(iCount, New LiteralControl(CType(currentControl, CheckBox).Checked))
                    ElseIf (TypeOf currentControl Is ImageButton) Then
                        localControl.Controls.Remove(currentControl)
                        localControl.Controls.AddAt(iCount, New LiteralControl(CType(currentControl, ImageButton).AlternateText))
                    ElseIf (TypeOf currentControl Is GridView) Then
                        Dim gv As GridView = CType(currentControl, GridView)
                        Dim Table As New Table
                        Table.GridLines = gv.GridLines
                        SetStyles(Table, gv, True)
                        If (Not (gv.HeaderRow) Is Nothing) Then
                            ExportGridView.Massage(gv.HeaderRow, EnableHyperLinks, ColumnsToExclude)
                            Dim i As Integer = 0
                            i = Table.Rows.Add(gv.HeaderRow)
                            SetStyles(Table.Rows(i), gv.HeaderStyle, True)
                        End If
                        '  Data rows of the table
                        For Each row As GridViewRow In gv.Rows
                            Dim i As Integer = 0
                            ExportGridView.Massage(row, EnableHyperLinks, ColumnsToExclude)
                            i = Table.Rows.Add(row)
                            If i Mod 2 = 0 Then
                                SetStyles(Table.Rows(i), gv.RowStyle, True)
                            Else
                                SetStyles(Table.Rows(i), gv.AlternatingRowStyle, True)
                            End If

                        Next
                        '  Footer of the table
                        If (Not (gv.FooterRow) Is Nothing) Then
                            Dim i As Integer = 0
                            ExportGridView.Massage(gv.FooterRow, EnableHyperLinks, ColumnsToExclude)
                            i = Table.Rows.Add(gv.FooterRow)
                            SetStyles(Table.Rows(i), gv.FooterStyle, True)
                        End If
                        localControl.Controls.Remove(currentControl)
                        localControl.Controls.AddAt(iCount, Table)
                    End If

                    If currentControl.HasControls Then
                        ExportGridView.Massage(currentControl, EnableHyperLinks, ColumnsToExclude)
                    End If
                    iCount += 1
                Loop
            End Sub
            Private Shared Sub SetStyles(ByRef HTMLTable As Object, ByVal FromObject As Object, ByVal ApplyStyling As Boolean)

                Try
                    If Not ApplyStyling Then Exit Sub
                    HTMLTable.BackColor = FromObject.BackColor
                    HTMLTable.BorderColor = FromObject.BorderColor
                    HTMLTable.ForeColor = FromObject.ForeColor
                    HTMLTable.BackImageUrl = FromObject.BackImageUrl
                    HTMLTable.Font.Name = FromObject.Font.Name
                    HTMLTable.Font.Italic = FromObject.Font.Italic
                    HTMLTable.Font.Bold = FromObject.Font.Bold
                    HTMLTable.Font.Size = FromObject.Font.Size
                    HTMLTable.Font.Underline = FromObject.Font.Underline
                    HTMLTable.Font.Strikeout = FromObject.Font.Strikeout

                Catch ex As Exception

                End Try
            End Sub
        End Class
    End Class
End Namespace


