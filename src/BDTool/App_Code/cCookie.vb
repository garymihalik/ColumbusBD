Imports Microsoft.VisualBasic
Imports System.Web
'==================================================================
'Generic class for handling cookie information
'Assumption:  Only 1 cookie will exist for the site.  Cookie name is "UserSettings"
'
'Avaliable Functions
'-SetExpire:  pass in a date to expire the cookie.  Creates cookie if it does not exist
'-AddValue:  pass in key and value to add to the cookie.  Creates cookie if it does not exist
'-ReadSingleCookieValue: returns the value based on the key passed
'-ReadAllCookieValues: returns all keys and values stored in the cookie in key1=value1&key2=value2 format
'-DeleteCookieValue: deletes the key/value pair in a cookie
'-DeleteCookie: deletes the entire cookie (only if the browser window is closed)
'==================================================================
Public Class cCookie
    Const DefaultCookieName As String = "UserSettings"
    Public Shared Function bCookieExist(ByVal strCookieName As String) As Boolean
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        bCookieExist = False
        If Not System.Web.HttpContext.Current.Request.Cookies(strCookieName) Is Nothing Then
            bCookieExist = True
        Else
            bCookieExist = False
        End If
    End Function
    Public Shared Sub SetExpire(ByVal strCookieName As String, ByVal dtExpireDate As Date)
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        Dim MyCookie As New HttpCookie(strCookieName)
        If bCookieExist(strCookieName) Then
            'Need to retrieve cookie and re-set
            MyCookie = System.Web.HttpContext.Current.Request.Cookies(strCookieName)
            MyCookie.Expires = dtExpireDate
        Else
            'Can just create a new one
            MyCookie.Expires = dtExpireDate
            System.Web.HttpContext.Current.Response.Cookies.Add(MyCookie)
        End If
    End Sub
    Public Shared Sub AddValue(ByVal strCookieName As String, ByVal strKey As String, ByVal strValue As String)
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        Dim MyCookie As New HttpCookie(strCookieName)
        If bCookieExist(strCookieName) Then
            'Need to copy cookie and re-set
            MyCookie = System.Web.HttpContext.Current.Request.Cookies(strCookieName)
            MyCookie(strKey) = strValue
            MyCookie.Expires = Date.Now.AddDays(14)
        Else
            'Can just create a new value pair
            MyCookie(strKey) = strValue
            MyCookie.Expires = Date.Now.AddDays(14)
        End If
        System.Web.HttpContext.Current.Response.Cookies.Add(MyCookie)
    End Sub
    Public Shared Function ReadSingleCookieValue(ByVal strCookieName As String, ByVal strNameValue As String) As String
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        If bCookieExist(strCookieName) Then
            Dim MyCookie As New HttpCookie(strCookieName)
            'Dim tempInfo As String
            'tempInfo = System.Web.HttpContext.Current.Request.Cookies(strCookieName).Value
            Dim UserInfoCookieCollection As System.Collections.Specialized.NameValueCollection
            UserInfoCookieCollection = System.Web.HttpContext.Current.Request.Cookies(strCookieName).Values
            If Not UserInfoCookieCollection.Item(strNameValue) Is Nothing Then
                ReadSingleCookieValue = UserInfoCookieCollection.Item(strNameValue).ToString
            Else
                ReadSingleCookieValue = "Error: Key " & strNameValue & " was not found in cookie"
            End If
        Else
            ReadSingleCookieValue = "Error: Cookie was not found"
        End If
    End Function
    Public Shared Function ReadAllCookieValues(ByVal strCookieName As String) As String
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        If bCookieExist(strCookieName) Then
            Dim MyCookie As New HttpCookie(strCookieName)
            ReadAllCookieValues = System.Web.HttpContext.Current.Request.Cookies(strCookieName).Value
        Else
            ReadAllCookieValues = "Cookie was not found"
        End If
    End Function
    Public Shared Sub DeleteCookieValue(ByVal strCookieName As String, ByVal strKey As String)
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        If Left(ReadSingleCookieValue(strCookieName, strKey), 5) <> "Error" Then
            Dim MyCookie As New HttpCookie(strCookieName)
            MyCookie = System.Web.HttpContext.Current.Request.Cookies(strCookieName)
            Dim UserInfoCookieCollection As System.Collections.Specialized.NameValueCollection
            UserInfoCookieCollection = System.Web.HttpContext.Current.Request.Cookies(strCookieName).Values
            UserInfoCookieCollection.Remove(strKey)
            System.Web.HttpContext.Current.Response.Cookies.Add(MyCookie)
        End If
    End Sub
    Public Shared Sub DeleteCookie(ByVal strCookieName As String)
        If String.IsNullOrEmpty(strCookieName) Then strCookieName = DefaultCookieName
        'can't delete cookie directly, but we can expire it.
        If bCookieExist(strCookieName) Then
            Dim MyCookie As New HttpCookie(strCookieName)
            MyCookie = System.Web.HttpContext.Current.Request.Cookies(strCookieName)
            MyCookie.Expires = Date.Now.AddDays(-1)
            System.Web.HttpContext.Current.Response.Cookies.Add(MyCookie)
        End If
    End Sub
End Class

