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
    Private Shared ReadOnly WriteOptions As New JsonSerializerOptions With {.WriteIndented = True, .Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping}

    ' The empty-file check runs once per app start, so emptying the list in Manager doesn't re-seed on the next reload.
    Private Shared emptyCheckDone As Boolean = False

    Public Shared Sub EnsureExists()
        If File.Exists(FilePath) Then
            ' An empty list ("[]", e.g. created by an earlier version) counts as a first launch too.
            If Not emptyCheckDone Then
                emptyCheckDone = True
                If IsEmptyList() Then Save(SeedApps())
            End If
            Return
        End If
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath))
        If File.Exists(LegacyPath) Then
            File.Copy(LegacyPath, FilePath)
        Else
            Save(SeedApps())
        End If
    End Sub

    ''' <summary>True only for a readable file holding an empty JSON array; a damaged file is left for Load to report.</summary>
    Private Shared Function IsEmptyList() As Boolean
        Try
            Dim apps = JsonSerializer.Deserialize(Of List(Of AppConfig))(File.ReadAllText(FilePath), ReadOptions)
            Return apps IsNot Nothing AndAlso apps.Count = 0
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' First-launch content: cards that open the bundled how-to guides (howto\howto-en.html and howto-hi.html next
    ''' to the exe). Only guides that are actually present are seeded; with none (a bare build) the list starts empty.
    ''' </summary>
    Private Shared Function SeedApps() As List(Of AppConfig)
        Dim guides As (FileName As String, Name As String, Description As String, Color As String, Order As Integer)() = {
            ("howto-en.html", "How To (English)", "Step-by-step guide", "#E8F1FB", 1),
            ("howto-hi.html", "कैसे करें (हिन्दी)", "चरण-दर-चरण गाइड", "#FDF1E3", 2)
        }
        Dim seeds As New List(Of AppConfig)()
        For Each g In guides
            Dim path As String = System.IO.Path.Combine(AppContext.BaseDirectory, "howto", g.FileName)
            If Not File.Exists(path) Then Continue For
            seeds.Add(New AppConfig With {
                .Name = g.Name,
                .ExecutablePath = path,
                .IconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "howto", "images", "applogo.png"),
                .Category = "Help",
                .Description = g.Description,
                .Group = "Getting Started",
                .SubGroup = "How To",
                .CardColor = g.Color,
                .SortOrder = g.Order
            })
        Next
        Return seeds
    End Function

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
