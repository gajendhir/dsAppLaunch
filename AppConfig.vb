Imports System.IO
Imports System.Text.Json.Serialization

Public Class AppConfig
    Public Property Id As String = Guid.NewGuid().ToString()
    Public Property Name As String = ""
    Public Property ExecutablePath As String = ""
    Public Property IconPath As String = ""
    Public Property Category As String = ""
    Public Property Description As String = ""

    ' Extra Optional Fields
    Public Property [Group] As String = ""
    Public Property SubGroup As String = ""
    Public Property CardColor As String = "#FFFFFF" ' Hex Color Representation

    ' Position within its group on the main screen (lower first); ties fall back to SubGroup order.
    Public Property SortOrder As Integer = 0
    ' Persistent default for the main screen's "Launch Selected" ticks (ticks there are per-session).
    Public Property Selected As Boolean = False

    ' Legacy keys (FirmName / FinancialYear): read-only aliases so older apps.json files still
    ' load. Set-only, so they are never serialized and the next save writes the new keys.
    <JsonPropertyName("FirmName")>
    Public WriteOnly Property LegacyFirmName As String
        Set(value As String)
            If String.IsNullOrEmpty([Group]) Then [Group] = value
        End Set
    End Property

    <JsonPropertyName("FinancialYear")>
    Public WriteOnly Property LegacyFinancialYear As String
        Set(value As String)
            If String.IsNullOrEmpty(SubGroup) Then SubGroup = value
        End Set
    End Property

    Public Shared Function Create(name As String, executablePath As String) As AppConfig
        Return New AppConfig With {
            .Id = Guid.NewGuid().ToString(),
            .Name = name,
            .ExecutablePath = executablePath,
            .Category = "General",
            .CardColor = "#FFFFFF"
        }
    End Function

    Public Shared Function FromFile(filePath As String) As AppConfig
        Return Create(Path.GetFileNameWithoutExtension(filePath), filePath)
    End Function
End Class
