Imports System
Imports System.Drawing
Imports Azure.Core
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets
Imports Microsoft.Graph

Module Program

    Private userAssignedClientId As String = "e16791{redacted}49d141978" 'client id for the user assigned MSI
    Private keyVaultSecretName As String = "Rays-{redacted}Secret" 'name for the keyvault secret
    Private keyVaultUri As Uri = New Uri("https://rays-{redacted}.vault.azure.net/")
    Sub Main()

        Do While True
            Get_AccessToken_With_UserAssigned_MSI()
            Make_GraphRequest_withUserMSI_Token()
            Get_Secret_With_UserAssigned_MSI()

            Get_AccessToken_With_SystemAssigned_MSI()
            Get_Secret_With_SystemAssigned_MSI()
            Make_GraphRequest_withSystemMSI_Token()

            Console.WriteLine("Press Enter to try again or any other key to exit")
            Dim key As ConsoleKeyInfo = Console.ReadKey()

            If Not key.Key = ConsoleKey.Enter Then
                Return
            End If
        Loop


    End Sub

    Sub Get_Secret_With_UserAssigned_MSI()
        Console.WriteLine($"{vbCrLf}Getting secret with user assigned msi:")

        Dim credential As New ManagedIdentityCredential(userAssignedClientId)

        Dim client As SecretClient = New SecretClient(keyVaultUri, credential)
        Dim secret As KeyVaultSecret = client.GetSecret(keyVaultSecretName).Value

        Console.WriteLine($"KeyVault Secret = {secret.Value}{vbCrLf}")

    End Sub

    Sub Get_Secret_With_SystemAssigned_MSI()
        Console.WriteLine($"{vbCrLf}Getting secret with system assigned msi:")

        Dim credential As New ManagedIdentityCredential()

        Dim client As SecretClient = New SecretClient(keyVaultUri, credential)
        Dim secret As KeyVaultSecret = client.GetSecret(keyVaultSecretName).Value

        Console.WriteLine($"KeyVault Secret = {secret.Value}{vbCrLf}")
    End Sub

    Sub Get_AccessToken_With_UserAssigned_MSI()
        Console.WriteLine($"Getting access token with user assigned msi:")

        Dim credential As New ManagedIdentityCredential(userAssignedClientId)

        Dim at As AccessToken = credential.GetToken(New TokenRequestContext(New String() {"https://graph.microsoft.com"}))
        Dim accessToken As String = at.Token.ToString()
        Console.WriteLine($"Access Token = {accessToken}")

    End Sub

    Sub Get_AccessToken_With_SystemAssigned_MSI()
        Console.WriteLine($"Getting access token with system assigned msi:")

        Dim credential As New ManagedIdentityCredential()
        Dim at As AccessToken = credential.GetToken(New TokenRequestContext(New String() {"https://graph.microsoft.com"}))

        Dim accessToken As String = at.Token.ToString()
        Console.WriteLine($"Access Token = {accessToken}")
    End Sub

    Sub Make_GraphRequest_withUserMSI_Token()
        Console.WriteLine($"Making graph request with User MSI Token:")

        Dim credential As New ManagedIdentityCredential(userAssignedClientId)

        Dim graphClient As New GraphServiceClient(credential)

        Dim users As IGraphServiceUsersCollectionPage
        Try
            users = graphClient.Users().Request.GetAsync().Result
            Console.WriteLine($"Number of users in tenant: {users.Count}{vbCrLf}")
        Catch ex As Exception
            Console.WriteLine($"Exception: {ex.Message}")
        End Try


    End Sub


    Sub Make_GraphRequest_withSystemMSI_Token()
        Console.WriteLine($"Making graph request with system MSI token:")

        Dim credential As New ManagedIdentityCredential()

        Dim graphClient As New GraphServiceClient(credential)

        Dim users As IGraphServiceUsersCollectionPage
        Try
            users = graphClient.Users().Request.GetAsync().Result
            Console.WriteLine($"Number of users in tenant: {users.Count}{vbCrLf}")
        Catch ex As Exception
            Console.WriteLine($"Exception: {ex.Message}")
        End Try


    End Sub

End Module
