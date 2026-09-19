Imports System.Collections.Concurrent
Imports System.IO

Public Class IconProvider
    Private Class CacheEntry
        Public ReadOnly Stamp As Long
        Public ReadOnly Image As Image

        Public Sub New(stamp As Long, image As Image)
            Me.Stamp = stamp
            Me.Image = image
        End Sub
    End Class

    ' Images are shared between cards for the life of the app and are never disposed; entries are
    ' revalidated by file timestamp so an updated exe/icon is picked up on the next reload.
    Private Shared ReadOnly Cache As New ConcurrentDictionary(Of String, CacheEntry)(StringComparer.OrdinalIgnoreCase)
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
        Dim entry As CacheEntry = Nothing
        If Cache.TryGetValue(key, entry) AndAlso entry.Stamp = stamp Then Return entry.Image

        Dim img As Image = load()
        If img IsNot Nothing Then Cache(key) = New CacheEntry(stamp, img)
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
