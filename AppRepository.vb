Imports System.IO
Imports System.Text.Json

Public Class AppRepository
    Public Const FilePath As String = "apps.json"
    Private Const BackupPath As String = "apps.json.bak"

    Private Shared ReadOnly ReadOptions As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
    Private Shared ReadOnly WriteOptions As New JsonSerializerOptions With {.WriteIndented = True}

    Public Shared Sub EnsureExists()
        If Not File.Exists(FilePath) Then File.WriteAllText(FilePath, "[]")
    End Sub

    ''' <summary>Returns an empty list if the file is missing; throws if it cannot be read or parsed.</summary>
    Public Shared Function Load() As List(Of AppConfig)
        If Not File.Exists(FilePath) Then Return New List(Of AppConfig)()
        Dim jsonText As String = File.ReadAllText(FilePath)
        BackupIfLegacy(jsonText)
        Dim apps = JsonSerializer.Deserialize(Of List(Of AppConfig))(jsonText, ReadOptions)
        Return If(apps, New List(Of AppConfig)())
    End Function

    ' A save after loading a legacy file rewrites it with the new keys (Group/SubGroup); keep the
    ' original once so a downgrade to an older build doesn't lose grouping. Best effort only.
    Private Shared Sub BackupIfLegacy(jsonText As String)
        If jsonText.IndexOf("""FirmName""", StringComparison.OrdinalIgnoreCase) < 0 AndAlso
           jsonText.IndexOf("""FinancialYear""", StringComparison.OrdinalIgnoreCase) < 0 Then Return
        Try
            If Not File.Exists(BackupPath) Then File.Copy(FilePath, BackupPath)
        Catch ex As IOException
        Catch ex As UnauthorizedAccessException
        End Try
    End Sub

    Public Shared Sub Save(apps As List(Of AppConfig))
        File.WriteAllText(FilePath, JsonSerializer.Serialize(apps, WriteOptions))
    End Sub
End Class
