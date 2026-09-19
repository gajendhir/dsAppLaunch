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
    ''' <summary>
    ''' Launches apps one after another, waiting <paramref name="staggerMs"/> between starts so a network share
    ''' isn't hit by many simultaneous starts. <paramref name="progress"/> is called (index 1-based, total, result)
    ''' after each attempt, on the caller's context.
    ''' </summary>
    Public Shared Async Function LaunchManyAsync(apps As IList(Of AppConfig), staggerMs As Integer, progress As Action(Of Integer, Integer, LaunchResult)) As Task(Of List(Of LaunchResult))
        Dim results As New List(Of LaunchResult)()
        For i As Integer = 0 To apps.Count - 1
            Dim app As AppConfig = apps(i)
            Dim result As LaunchResult = Await Task.Run(Function() Launch(app))
            results.Add(result)
            progress?.Invoke(i + 1, apps.Count, result)
            If i < apps.Count - 1 AndAlso staggerMs > 0 Then Await Task.Delay(staggerMs)
        Next
        Return results
    End Function

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
