Imports System
Imports System.Drawing
Imports Azure.Core
Imports Azure.Identity
Imports Azure.Security.KeyVault.Secrets
Imports Microsoft.Graph

Module Program

    Private userAssignedClientId As String = "{msi client id}" 'client id for the user assigned MSI
    Private keyVaultSecretName As String = "{keyvaulte secret name}" 'name for the keyvault secret
    Private keyVaultUri As Uri = New Uri("{uri for your keyvault}")
    Sub Main()

        Do While True
            Get_AccessToken_With_UserAssigned_MSI()
            Get_Secret_With_UserAssigned_MSI()

            Get_AccessToken_With_SystemAssigned_MSI()
            Get_Secret_With_SystemAssigned_MSI()

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
        Try
            Dim secret As KeyVaultSecret = client.GetSecret(keyVaultSecretName).Value
            Console.WriteLine($"KeyVault Secret = {secret.Value}{vbCrLf}")
        Catch ex As Exception
            Console.WriteLine($"Error getting secret: {ex.Message}")
        End Try


    End Sub

    Sub Get_Secret_With_SystemAssigned_MSI()
        Console.WriteLine($"{vbCrLf}Getting secret with system assigned msi:")

        Dim credential As New ManagedIdentityCredential()

        Dim client As SecretClient = New SecretClient(keyVaultUri, credential)
        Try
            Dim secret As KeyVaultSecret = client.GetSecret(keyVaultSecretName).Value
            Console.WriteLine($"KeyVault Secret = {secret.Value}{vbCrLf}")
        Catch ex As Exception
            Console.WriteLine($"Error getting secret: {ex.Message}")
        End Try

    End Sub

    Sub Get_AccessToken_With_UserAssigned_MSI()
        Console.WriteLine($"Getting access token with user assigned msi:")

        Dim credential As New ManagedIdentityCredential(userAssignedClientId)
        Try
            Dim at As AccessToken = credential.GetToken(New TokenRequestContext(New String() {"https://database.windows.net"}))
            Dim accessToken As String = at.Token.ToString()
            Console.WriteLine($"Access Token = {accessToken}")
        Catch ex As Exception
            Console.WriteLine($"{vbCrLf}Error getting access token: {ex.InnerException.Message}")
        End Try


    End Sub

    Sub Get_AccessToken_With_SystemAssigned_MSI()
        Console.WriteLine($"Getting access token with system assigned msi:")

        Dim credential As New ManagedIdentityCredential()
        Try
            Dim at As AccessToken = credential.GetToken(New TokenRequestContext(New String() {"https://database.windows.net"}))
            Dim accessToken As String = at.Token.ToString()
            Console.WriteLine($"Access Token = {accessToken}")
        Catch ex As Exception
            Console.WriteLine($"{vbCrLf}Error getting access token: {ex.InnerException.Message}")
        End Try
    End Sub



End Module
