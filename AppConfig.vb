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
    Public Property FirmName As String = ""
    Public Property FinancialYear As String = ""
    Public Property CardColor As String = "#FFFFFF" ' Hex Color Representation

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
