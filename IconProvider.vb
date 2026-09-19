Imports System.IO

Public Class IconProvider
    Public Shared Function GetAppIcon(app As AppConfig) As Image
        If Not String.IsNullOrEmpty(app.IconPath) AndAlso File.Exists(app.IconPath) Then
            Try : Return Image.FromFile(app.IconPath) : Catch : End Try
        End If

        If Not String.IsNullOrEmpty(app.ExecutablePath) AndAlso File.Exists(app.ExecutablePath) Then
            Try
                Dim extractedIcon As Icon = Icon.ExtractAssociatedIcon(app.ExecutablePath)
                If extractedIcon IsNot Nothing Then Return extractedIcon.ToBitmap()
            Catch : End Try
        End If

        Return SystemIcons.Application.ToBitmap()
    End Function
End Class
