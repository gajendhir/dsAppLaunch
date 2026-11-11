Imports System.Drawing.Drawing2D
Imports System.IO

Public Class AppCard
    Public Event LaunchRequested(card As AppCard)
    Public Event SettingsRequested(card As AppCard)
    Public Event SelectionChanged(card As AppCard)

    ' Card surface geometry, in 96-dpi design pixels (scaled per DPI in SurfaceBounds).
    Private Const InsetLeft As Integer = 3
    Private Const InsetTop As Integer = 2
    Private Const InsetRight As Integer = 4
    Private Const InsetBottom As Integer = 6
    Private Const CornerRadius As Integer = 8
    Private Const StripeWidth As Integer = 3

    Private accentColor As Color = Color.White
    Private isBusy As Boolean
    Private isHovered As Boolean
    Private suppressSelectionEvent As Boolean

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property App As AppConfig

    Public Sub New()
        InitializeComponent()
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint Or
                 ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)

        ' Glyphs and interactive colours live in code: the designer can't express hover/disabled looks.
        Theme.StyleIcon(btnSettings)
        btnSettings.Text = Theme.GlyphSettings
        Theme.StyleAccentOutline(btnLaunch)
        btnLaunch.Text = "▶ Launch"

        HookHover(Me)
    End Sub

    ''' <summary>The "Launch Selected" tick. Setting it programmatically does not raise SelectionChanged.</summary>
    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property IsSelected As Boolean
        Get
            Return chkSelect.Checked
        End Get
        Set(value As Boolean)
            suppressSelectionEvent = True
            chkSelect.Checked = value
            suppressSelectionEvent = False
            Invalidate()
        End Set
    End Property

    ''' <summary>Fills the card from an already-gathered probe, so no file system access happens on the UI thread.</summary>
    Public Sub Bind(probe As AppProbe)
        Dim app As AppConfig = probe.App
        Me.App = app
        accentColor = ParseColor(app.CardColor, Color.White)

        Dim exeFileName As String = If(String.IsNullOrEmpty(app.ExecutablePath), "", Path.GetFileName(app.ExecutablePath))
        picIcon.Image = probe.Icon
        toolTip.SetToolTip(picIcon, exeFileName)

        lblName.Text = app.Name
        toolTip.SetToolTip(lblName, exeFileName)

        lblSubGroup.Text = app.SubGroup
        lblSubGroup.Visible = Not String.IsNullOrWhiteSpace(app.SubGroup)

        lblDate.Visible = probe.ExeExists
        If probe.ExeExists Then
            lblDate.Text = $"{probe.ExeLastWrite:dd-MM-yy HH:mm}"
            toolTip.SetToolTip(lblDate, $"Modified {probe.ExeLastWrite:dd-MM-yyyy HH:mm} (double-click to show in Explorer)")
        End If

        lblDesc.Text = If(String.IsNullOrEmpty(app.Category), app.Description, $"[{app.Category}] {app.Description}")
        toolTip.SetToolTip(lblDesc, lblDesc.Text)

        toolTip.SetToolTip(btnLaunch, $"Launch {app.Name}")
        toolTip.SetToolTip(btnSettings, $"Edit settings for {app.Name}")

        LayoutText()
        Invalidate()
    End Sub

    Public Sub SetBusy(busy As Boolean)
        isBusy = busy
        btnLaunch.Enabled = Not busy
        btnLaunch.Text = If(busy, "Starting...", "▶ Launch")
        Invalidate()
    End Sub

    ' The sub-group (when present) sits under the name and the description follows it (one line,
    ' ellipsized). The modified date lives in the bottom action row, so it never moves.
    Private Sub LayoutText()
        Dim nextTop As Integer = lblName.Bottom + 1
        If lblSubGroup.Visible Then
            lblSubGroup.Top = nextTop
            nextTop += lblSubGroup.Height + 1
        End If
        lblDesc.Top = nextTop
    End Sub

    ' ---- Painting ------------------------------------------------------------------------------
    Private Function SurfaceBounds() As Rectangle
        Dim x As Integer = Theme.Scale(Me, InsetLeft)
        Dim y As Integer = Theme.Scale(Me, InsetTop)
        Dim w As Integer = Width - x - Theme.Scale(Me, InsetRight)
        Dim h As Integer = Height - y - Theme.Scale(Me, InsetBottom)
        Return New Rectangle(x, y, w - 1, h - 1)
    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias

        Dim surface As Rectangle = SurfaceBounds()
        Dim radius As Integer = Theme.Scale(Me, CornerRadius)

        ' Soft shadow: a few translucent copies stacked downwards (darkest next to the card).
        Dim shadowLayers As Integer = If(isHovered, 4, 3)
        Dim shadowAlpha As Integer = If(isHovered, 11, 8)
        For i As Integer = shadowLayers To 1 Step -1
            Dim r As New Rectangle(surface.X, surface.Y + Theme.Scale(Me, i), surface.Width, surface.Height)
            Using p As GraphicsPath = Theme.RoundedRect(r, radius)
                Using b As New SolidBrush(Color.FromArgb(shadowAlpha, 17, 24, 39))
                    g.FillPath(b, p)
                End Using
            End Using
        Next

        Using path As GraphicsPath = Theme.RoundedRect(surface, radius)
            Dim hasAccent As Boolean = HasVisibleAccent()
            Dim fill As Color = If(isBusy, Theme.BusyTint,
                                   If(hasAccent, Theme.Blend(Theme.Surface, accentColor, 0.4), Theme.Surface))
            Using b As New SolidBrush(fill)
                g.FillPath(b, path)
            End Using

            If hasAccent Then
                g.SetClip(path)
                Using b As New SolidBrush(Theme.Blend(accentColor, Color.Black, 0.12))
                    g.FillRectangle(b, surface.X, surface.Y, Theme.Scale(Me, StripeWidth), surface.Height + 1)
                End Using
                g.ResetClip()
            End If

            Dim borderColor As Color = If(IsSelected, Theme.Accent, If(isHovered, Theme.BorderStrong, Theme.Border))
            Using pen As New Pen(borderColor, If(IsSelected, 2.0F, 1.0F))
                g.DrawPath(pen, path)
            End Using
        End Using
    End Sub

    ' White (the default CardColor) means "no colour coding": plain white card, no stripe.
    Private Function HasVisibleAccent() As Boolean
        Return Not (accentColor.R > 245 AndAlso accentColor.G > 245 AndAlso accentColor.B > 245)
    End Function

    ' ---- Hover tracking (child controls swallow the mouse, so every control reports in) -------
    Private Sub HookHover(ctrl As Control)
        AddHandler ctrl.MouseEnter, AddressOf HoverChanged
        AddHandler ctrl.MouseLeave, AddressOf HoverChanged
        For Each child As Control In ctrl.Controls
            HookHover(child)
        Next
    End Sub

    Private Sub HoverChanged(sender As Object, e As EventArgs)
        Dim over As Boolean = ClientRectangle.Contains(PointToClient(Cursor.Position))
        If over <> isHovered Then
            isHovered = over
            Invalidate()
        End If
    End Sub

    Private Shared Function ParseColor(hexColor As String, defaultColor As Color) As Color
        If String.IsNullOrWhiteSpace(hexColor) Then Return defaultColor
        Try
            Return ColorTranslator.FromHtml(hexColor)
        Catch
            Return defaultColor
        End Try
    End Function

    Private Sub btnLaunch_Click(sender As Object, e As EventArgs) Handles btnLaunch.Click
        RaiseEvent LaunchRequested(Me)
    End Sub

    Private Sub chkSelect_CheckedChanged(sender As Object, e As EventArgs) Handles chkSelect.CheckedChanged
        Invalidate()
        If Not suppressSelectionEvent Then RaiseEvent SelectionChanged(Me)
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click
        RaiseEvent SettingsRequested(Me)
    End Sub

    ' Open File Explorer and select the file on double click
    Private Sub lblDate_DoubleClick(sender As Object, e As EventArgs) Handles lblDate.DoubleClick
        If App IsNot Nothing AndAlso File.Exists(App.ExecutablePath) Then
            Process.Start("explorer.exe", $"/select,""{App.ExecutablePath}""")
        End If
    End Sub
End Class
