Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Reflection

''' <summary>
''' Single source for the visual style: palette (drawn from the Data Spec logo), font names, the
''' logo, rounded-rectangle drawing and the button looks (which need hover/disabled handling that
''' the designer cannot express). Layout and static properties stay in the *.Designer.vb files.
''' </summary>
Public NotInheritable Class Theme
    Private Sub New()
    End Sub

    ' ---- Palette -------------------------------------------------------------------------------
    Public Shared ReadOnly PageBackground As Color = Color.FromArgb(246, 247, 249)
    Public Shared ReadOnly Surface As Color = Color.White
    Public Shared ReadOnly Border As Color = Color.FromArgb(229, 231, 235)
    Public Shared ReadOnly BorderStrong As Color = Color.FromArgb(209, 213, 219)
    Public Shared ReadOnly TextPrimary As Color = Color.FromArgb(31, 41, 55)
    Public Shared ReadOnly TextMuted As Color = Color.FromArgb(107, 114, 128)

    ' Logo orange is #EB3D00; button fills use a slightly darker shade so white text reaches 4.5:1.
    Public Shared ReadOnly Accent As Color = Color.FromArgb(217, 54, 0)
    Public Shared ReadOnly AccentHover As Color = Color.FromArgb(185, 46, 0)
    Public Shared ReadOnly AccentPressed As Color = Color.FromArgb(154, 38, 0)
    ' Yellow is too pale for text or fills on white: thin accents and the busy tint only.
    Public Shared ReadOnly AccentYellow As Color = Color.FromArgb(250, 204, 21)
    Public Shared ReadOnly BusyTint As Color = Color.FromArgb(255, 248, 219)

    Public Shared ReadOnly DisabledBackground As Color = Color.FromArgb(229, 231, 235)
    Public Shared ReadOnly DisabledText As Color = Color.FromArgb(156, 163, 175)
    Public Shared ReadOnly GhostHover As Color = Color.FromArgb(243, 244, 246)

    ' ---- Fonts (family names; Segoe UI ships with every supported Windows) --------------------
    Public Const UiFont As String = "Segoe UI"
    Public Const UiSemibold As String = "Segoe UI Semibold"
    ' Icon glyph font: Windows 10 1607+ (Segoe MDL2 Assets, still present on Windows 11).
    Public Const IconFont As String = "Segoe MDL2 Assets"

    Public Const GlyphRefresh As String = ChrW(&HE72C)
    Public Const GlyphSettings As String = ChrW(&HE713)
    Public Const GlyphPlay As String = ChrW(&HE768)

    ' ---- Logo and icon -------------------------------------------------------------------------
    Private Shared logoImage As Image
    Private Shared appIconCache As Icon

    ''' <summary>The Data Spec window icon (multi-size), loaded once from the embedded resource. Never dispose it.</summary>
    Public Shared Function AppIcon() As Icon
        If appIconCache Is Nothing Then
            Using s As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DataSpec.ico")
                If s IsNot Nothing Then appIconCache = New Icon(s)
            End Using
        End If
        Return appIconCache
    End Function

    ''' <summary>The Data Spec logo (300 px, transparent), loaded once from the embedded resource. Never dispose it.</summary>
    Public Shared Function Logo() As Image
        If logoImage Is Nothing Then
            Using s As Stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("dslogo.png")
                If s IsNot Nothing Then logoImage = Image.FromStream(s)
            End Using
        End If
        Return logoImage
    End Function

    ''' <summary>Draws the logo scaled into <paramref name="bounds"/> with high-quality resampling.</summary>
    Public Shared Sub DrawLogo(g As Graphics, bounds As Rectangle)
        Dim img As Image = Logo()
        If img Is Nothing Then Return
        Dim previous = g.InterpolationMode
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.DrawImage(img, bounds)
        g.InterpolationMode = previous
    End Sub

    ' ---- Drawing helpers -----------------------------------------------------------------------
    ''' <summary>Rounded rectangle path. The caller disposes it.</summary>
    Public Shared Function RoundedRect(r As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d As Integer = Math.Max(1, radius * 2)
        path.AddArc(r.X, r.Y, d, d, 180, 90)
        path.AddArc(r.Right - d, r.Y, d, d, 270, 90)
        path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90)
        path.AddArc(r.X, r.Bottom - d, d, d, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    ''' <summary>Mixes <paramref name="tint"/> into <paramref name="baseColor"/> by <paramref name="amount"/> (0..1).</summary>
    Public Shared Function Blend(baseColor As Color, tint As Color, amount As Double) As Color
        Dim a As Double = Math.Max(0.0, Math.Min(1.0, amount))
        ' Channels are Bytes: widen first, or a negative difference throws OverflowException.
        Return Color.FromArgb(
            CInt(CInt(baseColor.R) + (CInt(tint.R) - CInt(baseColor.R)) * a),
            CInt(CInt(baseColor.G) + (CInt(tint.G) - CInt(baseColor.G)) * a),
            CInt(CInt(baseColor.B) + (CInt(tint.B) - CInt(baseColor.B)) * a))
    End Function

    ''' <summary>Pixel size for a design value at the control's current DPI.</summary>
    Public Shared Function Scale(ctrl As Control, designPx As Integer) As Integer
        Return CInt(Math.Round(designPx * ctrl.DeviceDpi / 96.0))
    End Function

    ' ---- Button looks --------------------------------------------------------------------------
    ''' <summary>Filled accent button (the main action on a screen). Turns grey when disabled.</summary>
    Public Shared Sub StylePrimary(btn As Button)
        FlatBase(btn)
        btn.FlatAppearance.BorderSize = 0
        btn.FlatAppearance.MouseOverBackColor = AccentHover
        btn.FlatAppearance.MouseDownBackColor = AccentPressed
        Dim apply As EventHandler = Sub()
                                        btn.BackColor = If(btn.Enabled, Accent, DisabledBackground)
                                        btn.ForeColor = If(btn.Enabled, Color.White, DisabledText)
                                    End Sub
        AddHandler btn.EnabledChanged, apply
        apply(btn, EventArgs.Empty)
    End Sub

    ''' <summary>Neutral bordered button for secondary actions.</summary>
    Public Shared Sub StyleGhost(btn As Button)
        FlatBase(btn)
        btn.BackColor = Surface
        btn.ForeColor = TextPrimary
        btn.FlatAppearance.BorderSize = 1
        btn.FlatAppearance.BorderColor = BorderStrong
        btn.FlatAppearance.MouseOverBackColor = GhostHover
        btn.FlatAppearance.MouseDownBackColor = Border
    End Sub

    ''' <summary>Borderless icon-only button (glyph font expected on the button).</summary>
    Public Shared Sub StyleIcon(btn As Button)
        FlatBase(btn)
        btn.BackColor = Color.Transparent
        btn.FlatAppearance.BorderSize = 0
        btn.ForeColor = TextMuted
        btn.FlatAppearance.MouseOverBackColor = GhostHover
        btn.FlatAppearance.MouseDownBackColor = Border
        AddHandler btn.MouseEnter, Sub() btn.ForeColor = TextPrimary
        AddHandler btn.MouseLeave, Sub() btn.ForeColor = TextMuted
    End Sub

    ''' <summary>Accent outline that fills on hover: an action repeated on many cards without a wall of solid colour.</summary>
    Public Shared Sub StyleAccentOutline(btn As Button)
        FlatBase(btn)
        btn.BackColor = Color.Transparent
        btn.ForeColor = Accent
        btn.FlatAppearance.BorderSize = 1
        btn.FlatAppearance.BorderColor = Accent
        btn.FlatAppearance.MouseOverBackColor = Accent
        btn.FlatAppearance.MouseDownBackColor = AccentPressed
        AddHandler btn.MouseEnter, Sub() If btn.Enabled Then btn.ForeColor = Color.White
        AddHandler btn.MouseLeave, Sub() btn.ForeColor = If(btn.Enabled, Accent, DisabledText)
        AddHandler btn.EnabledChanged, Sub()
                                           btn.FlatAppearance.BorderColor = If(btn.Enabled, Accent, BorderStrong)
                                           btn.ForeColor = If(btn.Enabled, Accent, DisabledText)
                                       End Sub
    End Sub

    Private Shared Sub FlatBase(btn As Button)
        btn.FlatStyle = FlatStyle.Flat
        btn.UseVisualStyleBackColor = False
        btn.Cursor = Cursors.Hand
    End Sub
End Class
