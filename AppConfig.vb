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
End Class