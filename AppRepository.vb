Imports System.IO
Imports System.Text.Json

Public Class AppRepository
    Private Const FileName As String = "apps.json"

    ''' <summary>Per-user, writable config location: %AppData%\Data Spec\dsAppLaunch\apps.json.</summary>
    Public Shared ReadOnly FilePath As String = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Data Spec", "dsAppLaunch", FileName)

    Private Shared ReadOnly BackupPath As String = FilePath & ".bak"

    ' Pre-2.3 builds kept apps.json next to the exe; it is copied to FilePath once, on first run.
    Private Shared ReadOnly LegacyPath As String = Path.Combine(AppContext.BaseDirectory, FileName)

    Private Shared ReadOnly ReadOptions As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
    Private Shared ReadOnly WriteOptions As New JsonSerializerOptions With {.WriteIndented = True}

    Public Shared Sub EnsureExists()
        If File.Exists(FilePath) Then Return
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath))
        If File.Exists(LegacyPath) Then
            File.Copy(LegacyPath, FilePath)
        Else
            File.WriteAllText(FilePath, "[]")
        End If
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

    Public Shared Sub Save(apps As IEnumerable(Of AppConfig))
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath))
        File.WriteAllText(FilePath, JsonSerializer.Serialize(apps.ToList(), WriteOptions))
    End Sub
End Class
