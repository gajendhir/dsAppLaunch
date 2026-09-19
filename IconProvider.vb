Imports System.Collections.Concurrent
Imports System.IO

Public Class IconProvider
    ' Images are shared between cards for the life of the app and are never disposed. The key includes the
    ' file timestamp, so an updated exe/icon is reloaded. Lazy makes concurrent probes of the same file
    ' (several entries pointing at one exe) share a single extraction; extracting one file's icon from several
    ' threads at once was observed to return the generic document icon.
    Private Shared ReadOnly Cache As New ConcurrentDictionary(Of String, Lazy(Of Image))(StringComparer.OrdinalIgnoreCase)
    Private Shared ReadOnly FallbackIcon As Image = SystemIcons.Application.ToBitmap()

    Public Shared ReadOnly Property Fallback As Image
        Get
            Return FallbackIcon
        End Get
    End Property

    ''' <summary>Custom IconPath -> icon extracted from the exe -> generic system icon. Safe to call off the UI thread.</summary>
    Public Shared Function GetAppIcon(app As AppConfig, exeExists As Boolean, exeLastWrite As Date) As Image
        If Not String.IsNullOrEmpty(app.IconPath) Then
            Dim stamp As Long = FileStamp(app.IconPath)
            If stamp >= 0 Then
                Dim img As Image = Cached("img:" & app.IconPath, stamp, Function() LoadImageCopy(app.IconPath))
                If img IsNot Nothing Then Return img
            End If
        End If

        If exeExists Then
            Dim img As Image = Cached("exe:" & app.ExecutablePath, exeLastWrite.Ticks, Function() ExtractExeIcon(app.ExecutablePath))
            If img IsNot Nothing Then Return img
        End If

        Return FallbackIcon
    End Function

    Private Shared Function Cached(key As String, stamp As Long, load As Func(Of Image)) As Image
        Dim fullKey As String = key & "|" & stamp
        Dim img As Image = Cache.GetOrAdd(fullKey, Function(k) New Lazy(Of Image)(load, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication)).Value
        ' Don't remember failures: retry on the next reload (e.g. a share that was briefly unavailable).
        If img Is Nothing Then
            Dim removed As Lazy(Of Image) = Nothing
            Cache.TryRemove(fullKey, removed)
        End If
        Return img
    End Function

    Private Shared Function FileStamp(path As String) As Long
        Try
            If File.Exists(path) Then Return File.GetLastWriteTimeUtc(path).Ticks
        Catch ex As Exception
            ' unreadable/invalid path: treat as missing
        End Try
        Return -1
    End Function

    ' Copy into an independent bitmap so the source file is not left locked by GDI+.
    Private Shared Function LoadImageCopy(path As String) As Image
        Try
            Using fs As FileStream = File.OpenRead(path)
                Using src As Image = Image.FromStream(fs)
                    Return New Bitmap(src)
                End Using
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Function ExtractExeIcon(path As String) As Image
        Try
            Using ico As Icon = Icon.ExtractAssociatedIcon(path)
                Return If(ico Is Nothing, Nothing, ico.ToBitmap())
            End Using
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
