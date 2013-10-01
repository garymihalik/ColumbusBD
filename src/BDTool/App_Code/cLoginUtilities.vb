Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.OleDb
Imports System.Drawing
Imports Samples.AccessProviders.AccessMembershipProvider

Public Class cLoginUtilities
    Public Shared bLogDetails As Boolean = ConfigurationManager.AppSettings("LogDetails")
    Public Shared Function AuthenticateLoginID(ByVal strUserID As String) As String
        'Get all the roles of the user
        Dim myroles As String()
        myroles = Roles.GetAllRoles
        Dim myListofRoles As String = ""
        Dim x As Integer = 0
        For x = 0 To myroles.Count - 1
            If Roles.IsUserInRole(strUserID, myroles(x)) Then
                myListofRoles &= myroles(x).ToString & ","
            End If
        Next
        If Len(myListofRoles) > 0 Then
            'Remove "," from trailing the list
            myListofRoles = Left(myListofRoles, Len(myListofRoles) - 1)
        End If
        cCookie.AddValue("UserSettings", "BU", myListofRoles)

        'Now, if I only have 1 role, then set the cookie of last selected
        Dim aryRoles As String()
        aryRoles = Split(myListofRoles, ",")
        If aryRoles.Count = 1 Then
            cCookie.AddValue("UserSettings", "LastSelectedBU", cDataSetManipulation.GetSingleSQLValue("select ID from tblBusinessUnit where BusinessUnit='" & aryRoles(0).ToString & "'"))
        End If
        Dim tempLastBU As String = cCookie.ReadSingleCookieValue("UserSettings", "LastSelectedBU")
        'Add Default Theme
        cCookie.AddValue("UserSettings", "Theme", "Centric4")
        'Add Better Name if found
        Dim tempEmail As String = cDataSetManipulation.GetSingleSQLValue("Select email from vw_aspnet_MembershipUsers where username='" & strUserID & "'")
        Dim betterName As String = cDataSetManipulation.GetSingleSQLValue("Select firstname & ' ' & lastname as betterName from tblResources where email='" & tempEmail & "'")
        cCookie.AddValue("UserSettings", "betterName", betterName)


        ' Create an empty Profile for the newly created user
        Dim p As ProfileCommon = DirectCast(ProfileCommon.Create(strUserID, True), ProfileCommon)

        ' Populate some Profile properties off of the create user wizard
        If String.IsNullOrEmpty(p.FriendlyName) Then p.FriendlyName = betterName
        If p.BU = 0 And IsNumeric(tempLastBU) Then
            p.BU = tempLastBU
        Else
            p.BU = 0
        End If
        ' Save profile - must be done since we explicitly created it
        p.Save()

        Return myListofRoles
    End Function
    Public Shared Function GetUserTheme() As String
        Dim strTheme As String = cCookie.ReadSingleCookieValue("UserSettings", "Theme")
        If String.IsNullOrEmpty(strTheme) Or strTheme.IndexOf("Error") >= 0 Then
            GetUserTheme = "Centric4"
        Else
            GetUserTheme = strTheme
        End If
    End Function
End Class
