Imports System.IO

Public Enum LaunchStatus
    Started
    NotFound
    Failed
End Enum

Public Class LaunchResult
    Public ReadOnly Property App As AppConfig
    Public ReadOnly Property Status As LaunchStatus
    Public ReadOnly Property Message As String

    Public Sub New(app As AppConfig, status As LaunchStatus, Optional message As String = "")
        Me.App = app
        Me.Status = status
        Me.Message = message
    End Sub
End Class

Public Class AppLauncher
    Public Shared Function Launch(app As AppConfig) As LaunchResult
        If String.IsNullOrEmpty(app.ExecutablePath) OrElse Not File.Exists(app.ExecutablePath) Then
            Return New LaunchResult(app, LaunchStatus.NotFound)
        End If

        Try
            Process.Start(New ProcessStartInfo With {
                .FileName = app.ExecutablePath,
                .WorkingDirectory = Path.GetDirectoryName(app.ExecutablePath),
                .UseShellExecute = True
            })
            Return New LaunchResult(app, LaunchStatus.Started)
        Catch ex As Exception
            Return New LaunchResult(app, LaunchStatus.Failed, ex.Message)
        End Try
    End Function
End Class
