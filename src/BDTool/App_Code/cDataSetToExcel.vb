Imports Microsoft.VisualBasic
Imports System.Data

Public Class cDataSetToExcel
    'Class to convert a dataset to an html stream which can be used to display the dataset
    'in MS Excel
    'The Convert method is overloaded three times as follows
    ' 1) Default to first table in dataset
    ' 2) Pass an index to tell us which table in the dataset to use
    ' 3) Pass a table name to tell us which table in the dataset to use

    Public Shared Sub Convert(ByVal ds As DataSet, ByVal response As HttpResponse)
        'first let's clean up the response.object
        response.Clear()
        response.Charset = ""
        'set the response mime type for excel
        'Excel 2003 : "application/vnd.ms-excel"
        'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'create a string writer
        Dim stringWrite As New System.IO.StringWriter
        'create an htmltextwriter which uses the stringwriter
        Dim htmlWrite As New System.Web.UI.HtmlTextWriter(stringWrite)
        'instantiate a datagrid
        Dim dg As New DataGrid
        'set the datagrid datasource to the dataset passed in
        dg.DataSource = ds.Tables(0)
        'bind the datagrid
        dg.DataBind()
        'tell the datagrid to render itself to our htmltextwriter
        dg.RenderControl(htmlWrite)
        'all that's left is to output the html
        response.Write(stringWrite.ToString)
        response.End()
    End Sub

    Public Shared Sub Convert(ByVal ds As DataSet, ByVal TableIndex As Integer, ByVal response As HttpResponse)
        'lets make sure a table actually exists at the passed in value
        'if it is not call the base method
        If TableIndex > ds.Tables.Count - 1 Then
            Convert(ds, response)
        End If
        'we've got a good table so
        'let's clean up the response.object
        response.Clear()
        response.Charset = ""
        'set the response mime type for excel
        'Excel 2003 : "application/vnd.ms-excel"
        'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'create a string writer
        Dim stringWrite As New System.IO.StringWriter
        'create an htmltextwriter which uses the stringwriter
        Dim htmlWrite As New System.Web.UI.HtmlTextWriter(stringWrite)
        'instantiate a datagrid
        Dim dg As New DataGrid
        'set the datagrid datasource to the dataset passed in
        dg.DataSource = ds.Tables(TableIndex)
        'bind the datagrid
        dg.DataBind()
        'tell the datagrid to render itself to our htmltextwriter
        dg.RenderControl(htmlWrite)
        'all that's left is to output the html
        response.Write(stringWrite.ToString)
        response.End()
    End Sub

    Public Shared Sub Convert(ByVal ds As DataSet, ByVal TableName As String, ByVal response As HttpResponse)
        'let's make sure the table name exists
        'if it does not then call the default method
        If ds.Tables(TableName) Is Nothing Then
            Convert(ds, response)
        End If
        'we've got a good table so
        'let's clean up the response.object
        response.Clear()
        response.Charset = ""
        'set the response mime type for excel
        'Excel 2003 : "application/vnd.ms-excel"
        'Excel 2007 : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        'create a string writer
        Dim stringWrite As New System.IO.StringWriter
        'create an htmltextwriter which uses the stringwriter
        Dim htmlWrite As New System.Web.UI.HtmlTextWriter(stringWrite)
        'instantiate a datagrid
        Dim dg As New DataGrid
        'set the datagrid datasource to the dataset passed in
        dg.DataSource = ds.Tables(TableName)
        'bind the datagrid
        dg.DataBind()
        'tell the datagrid to render itself to our htmltextwriter
        dg.RenderControl(htmlWrite)
        'all that's left is to output the html
        response.Write(stringWrite.ToString)
        response.End()
    End Sub

End Class
