Imports System.IO

''' <summary>
''' Everything a card needs from the file system (exe exists/date, icon), gathered in one place so it can
''' be done on background threads (one task per app, so a dead share only delays its own cards). Exe paths are usually UNC shares, where a single missing/offline
''' path can block for seconds.
''' </summary>
Public Class AppProbe
    Public ReadOnly Property App As AppConfig
    Public ReadOnly Property ExeExists As Boolean
    Public ReadOnly Property ExeLastWrite As Date
    Public ReadOnly Property Icon As Image

    Private Sub New(app As AppConfig, exeExists As Boolean, exeLastWrite As Date, icon As Image)
        Me.App = app
        Me.ExeExists = exeExists
        Me.ExeLastWrite = exeLastWrite
        Me.Icon = icon
    End Sub

    Public Shared Function Probe(app As AppConfig) As AppProbe
        Dim exists As Boolean = False
        Dim lastWrite As Date = Date.MinValue

        If Not String.IsNullOrEmpty(app.ExecutablePath) Then
            Try
                Dim info As New FileInfo(app.ExecutablePath)
                exists = info.Exists
                If exists Then lastWrite = info.LastWriteTime
            Catch ex As Exception
                ' invalid path text in a hand-edited config: treat as missing
            End Try
        End If

        Return New AppProbe(app, exists, lastWrite, IconProvider.GetAppIcon(app, exists, lastWrite))
    End Function

    ''' <summary>What a card shows before its real probe completes: no date, generic icon.</summary>
    Public Shared Function Placeholder(app As AppConfig) As AppProbe
        Return New AppProbe(app, False, Date.MinValue, IconProvider.Fallback)
    End Function
End Class
