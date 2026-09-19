Imports System.IO
Imports System.Text.Json

Public Class AppRepository
    Public Const FilePath As String = "apps.json"

    Private Shared ReadOnly ReadOptions As New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
    Private Shared ReadOnly WriteOptions As New JsonSerializerOptions With {.WriteIndented = True}

    Public Shared Sub EnsureExists()
        If Not File.Exists(FilePath) Then File.WriteAllText(FilePath, "[]")
    End Sub

    ''' <summary>Returns an empty list if the file is missing; throws if it cannot be read or parsed.</summary>
    Public Shared Function Load() As List(Of AppConfig)
        If Not File.Exists(FilePath) Then Return New List(Of AppConfig)()
        Dim apps = JsonSerializer.Deserialize(Of List(Of AppConfig))(File.ReadAllText(FilePath), ReadOptions)
        Return If(apps, New List(Of AppConfig)())
    End Function

    Public Shared Sub Save(apps As List(Of AppConfig))
        File.WriteAllText(FilePath, JsonSerializer.Serialize(apps, WriteOptions))
    End Sub
End Class
