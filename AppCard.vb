Imports System.IO

Public Class AppCard
    Public Event LaunchRequested(card As AppCard)
    Public Event SettingsRequested(card As AppCard)
    Public Event SelectionChanged(card As AppCard)

    Private idleColor As Color
    Private suppressSelectionEvent As Boolean

    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Public Property App As AppConfig

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
        End Set
    End Property

    ''' <summary>Fills the card from an already-gathered probe, so no file system access happens on the UI thread.</summary>
    Public Sub Bind(probe As AppProbe)
        Dim app As AppConfig = probe.App
        Me.App = app
        BackColor = ParseColor(app.CardColor, Color.White)

        Dim exeFileName As String = If(String.IsNullOrEmpty(app.ExecutablePath), "", Path.GetFileName(app.ExecutablePath))
        picIcon.Image = probe.Icon
        toolTip.SetToolTip(picIcon, exeFileName)

        lblName.Text = app.Name
        toolTip.SetToolTip(lblName, exeFileName)

        lblSubGroup.Text = app.SubGroup
        lblSubGroup.Visible = Not String.IsNullOrWhiteSpace(app.SubGroup)

        lblDate.Visible = probe.ExeExists
        If probe.ExeExists Then lblDate.Text = $"📅 {probe.ExeLastWrite:dd-MM-yyyy HH:mm}"

        lblDesc.Text = If(String.IsNullOrEmpty(app.Category), app.Description, $"[{app.Category}] {app.Description}")

        toolTip.SetToolTip(btnLaunch, $"Launch {app.Name}")
        toolTip.SetToolTip(btnSettings, $"Edit settings for {app.Name}")

        LayoutText()
    End Sub

    Public Sub SetBusy(busy As Boolean)
        If busy Then
            idleColor = BackColor
            BackColor = Color.FromArgb(254, 249, 231)
            btnLaunch.Enabled = False
            btnLaunch.Text = "⏳ Busy..."
        Else
            BackColor = idleColor
            btnLaunch.Text = "▶ Launch"
            btnLaunch.Enabled = True
        End If
    End Sub

    ' Stacks the visible text labels under the name; the description takes the space left above the buttons.
    Private Sub LayoutText()
        Dim nextTop As Integer = lblName.Bottom + 1
        For Each lbl As Label In {lblSubGroup, lblDate}
            If lbl.Visible Then
                lbl.Top = nextTop
                nextTop += lbl.Height + 1
            End If
        Next
        lblDesc.Top = nextTop
        lblDesc.Height = Math.Max(20, btnLaunch.Top - 5 - nextTop)
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
