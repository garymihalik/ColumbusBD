Imports Microsoft.VisualBasic

'From http://aspnet.4guysfromrolla.com/articles/083105-1.aspx
Public Class cTamperProof
    'The secret salt...
    Private Const SecretSalt As String = "H3#@*ALMLLlk31q4l1ncL#@..."

    Public Shared Function CreateTamperProofURL(ByVal url As String, ByVal nonTamperProofParams As String, ByVal tamperProofParams As String) As String
        Dim tpURL As String = url
        If nonTamperProofParams.Length > 0 OrElse tamperProofParams.Length > 0 Then
            url &= "?"
        End If

        'Add on the tamper & non-tamper proof parameters, if any
        If nonTamperProofParams.Length > 0 Then
            url &= nonTamperProofParams

            If tamperProofParams.Length > 0 Then url &= "&"
        End If

        If tamperProofParams.Length > 0 Then url &= tamperProofParams

        'Add on the tamper-proof digest, if needed
        If tamperProofParams.Length > 0 Then
            url &= String.Concat("&Digest=", GetDigest(tamperProofParams))
        End If

        Return url
    End Function


    Public Shared Function GetDigest(ByVal tamperProofParams As String) As String
        Dim Digest As String = String.Empty
        Dim input As String = String.Concat(SecretSalt, tamperProofParams, SecretSalt)

        'The array of bytes that will contain the encrypted value of input
        Dim hashedDataBytes As Byte()

        'The encoder class used to convert strPlainText to an array of bytes
        Dim encoder As New System.Text.UTF8Encoding

        'Create an instance of the MD5CryptoServiceProvider class
        Dim md5Hasher As New System.Security.Cryptography.MD5CryptoServiceProvider

        'Call ComputeHash, passing in the plain-text string as an array of bytes
        'The return value is the encrypted value, as an array of bytes
        hashedDataBytes = md5Hasher.ComputeHash(encoder.GetBytes(input))

        'Base-64 Encode the results and strip off ending '==', if it exists
        Digest = Convert.ToBase64String(hashedDataBytes).TrimEnd("=".ToCharArray())

        Return Digest
    End Function
    Public Shared Function TamperedURL(ByVal tamperProofParams As String, ByVal strDigest As String) As Boolean
        'Determine what the digest SHOULD be
        Dim expectedDigest As String = GetDigest(tamperProofParams)

        'Any + in the digest passed through the querystring would be
        'convereted into spaces, so 'uncovert' them
        Dim receivedDigest As String = strDigest
        If receivedDigest Is Nothing Then
            'Oh my, we didn't get a Digest!
            'MsgBox("YOU MUST PASS IN A DIGEST!")
            cErrorManagement.InsertErrorMessage("No Digest Passed In", "cTamperProof.TamperedURL")
            Return True
            Exit Function
        Else
            receivedDigest = receivedDigest.Replace(" ", "+")

            'Now, see if the received and expected digests match up
            If String.Compare(expectedDigest, receivedDigest) <> 0 Then
                'Don't match up, egad
                'MsgBox("THE URL HAS BEEN TAMPERED WITH.")
                cErrorManagement.InsertErrorMessage("Expected-" & expectedDigest & ":Receive-" & receivedDigest, "cTamperProof.TamperedURL")
                Return True
                Exit Function
            Else
                Return False
            End If
        End If
    End Function
End Class
