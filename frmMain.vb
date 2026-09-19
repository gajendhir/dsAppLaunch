Public Class frmMain
    Private Shared ReadOnly HeaderFont As New Font("Segoe UI", 12, FontStyle.Bold)

    ' Incremented per reload; probe results from an older load are ignored.
    Private loadVersion As Integer

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadApplications()
    End Sub

    Private Sub btnReload_Click(sender As Object, e As EventArgs) Handles btnReload.Click
        LoadApplications()
    End Sub

    Private Sub btnSettings_Click(sender As Object, e As EventArgs) Handles btnSettings.Click
        OpenSettings()
    End Sub

    Private Sub lblSupportFooter_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lblSupportFooter.LinkClicked
        Process.Start(New ProcessStartInfo With {.FileName = "https://www.dataspec.info", .UseShellExecute = True})
    End Sub

    Private Sub EnsureJsonFileExists()
        Try
            AppRepository.EnsureExists()
        Catch ex As Exception
            MessageBox.Show($"Could not create missing JSON file: {ex.Message}", "File System Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End Try
    End Sub

    Private Sub flowLayout_Resize(sender As Object, e As EventArgs) Handles flowLayout.Resize
        If IsNothing(flowLayout) OrElse flowLayout.Controls.Count = 0 Then
            Return
        End If

        flowLayout.SuspendLayout()
        Dim targetWidth As Integer = Math.Max(200, flowLayout.ClientSize.Width - 35)

        For Each ctrl As Control In flowLayout.Controls
            If TypeOf ctrl Is Label AndAlso Equals(ctrl.Tag, "HeaderLabel") Then
                ctrl.Width = targetWidth
            ElseIf TypeOf ctrl Is Panel AndAlso Equals(ctrl.Tag, "BreakPanel") Then
                ctrl.Width = targetWidth
            End If
        Next
        flowLayout.ResumeLayout()
    End Sub

    Private Sub ClearCards()
        Dim old As List(Of Control) = flowLayout.Controls.Cast(Of Control)().ToList()
        flowLayout.Controls.Clear()
        For Each ctrl As Control In old
            ctrl.Dispose()
        Next
    End Sub

    ''' <summary>
    ''' Builds the cards immediately, then fills in each card's exe date/icon as its background probe
    ''' finishes. Exe paths are usually network shares; probing them on the UI thread froze the window.
    ''' </summary>
    Public Sub LoadApplications()
        EnsureJsonFileExists()
        loadVersion += 1
        Dim version As Integer = loadVersion

        flowLayout.SuspendLayout()
        ClearCards()

        Try
            Dim apps As List(Of AppConfig) = AppRepository.Load()
            Dim headerWidth As Integer = Math.Max(200, flowLayout.ClientSize.Width - 35)

            For Each appGroup In AppGrouping.GroupAndSort(apps)
                ' Group Header Label
                Dim lblGroupHeader As New Label With {
                    .Text = appGroup.Title,
                    .Font = HeaderFont,
                    .ForeColor = Color.FromArgb(41, 128, 185),
                    .Size = New Size(headerWidth, 24),
                    .Margin = New Padding(5, 6, 5, 2),
                    .AutoSize = False,
                    .AutoEllipsis = True,
                    .Tag = "HeaderLabel"
                }
                flowLayout.Controls.Add(lblGroupHeader)
                flowLayout.SetFlowBreak(lblGroupHeader, True)

                For Each app In appGroup.Apps
                    Dim card As New AppCard()
                    card.Bind(AppProbe.Placeholder(app))
                    AddHandler card.LaunchRequested, AddressOf Card_LaunchRequested
                    AddHandler card.SettingsRequested, AddressOf Card_SettingsRequested
                    flowLayout.Controls.Add(card)

                    Dim target As AppCard = card
                    Dim current As AppConfig = app
                    Task.Run(Function() AppProbe.Probe(current)).ContinueWith(
                        Sub(t)
                            If t.Status = TaskStatus.RanToCompletion Then PostProbe(target, t.Result, version)
                        End Sub)
                Next

                ' Break panel forces the next group section to start on a NEW line below cards
                Dim breakPanel As New Panel With {
                    .Size = New Size(headerWidth, 10),
                    .Margin = New Padding(0),
                    .Tag = "BreakPanel"
                }
                flowLayout.Controls.Add(breakPanel)
                flowLayout.SetFlowBreak(breakPanel, True)
            Next
        Catch ex As Exception
            MessageBox.Show("Failed to load applications: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            flowLayout.ResumeLayout()
        End Try
    End Sub

    ' Marshals a finished probe to the UI thread explicitly (not via the ambient SynchronizationContext).
    Private Sub PostProbe(card As AppCard, probe As AppProbe, version As Integer)
        Try
            BeginInvoke(Sub()
                            If version = loadVersion AndAlso Not card.IsDisposed Then card.Bind(probe)
                        End Sub)
        Catch ex As InvalidOperationException
            ' form closed/disposing before the probe finished
        End Try
    End Sub

    Private Async Sub Card_LaunchRequested(card As AppCard)
        Dim app As AppConfig = card.App
        Dim result As LaunchResult = AppLauncher.Launch(app)
        Select Case result.Status
            Case LaunchStatus.NotFound
                MessageBox.Show("Target executable not found at:" & vbCrLf & app.ExecutablePath, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            Case LaunchStatus.Failed
                MessageBox.Show("Error launching application: " & result.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
        End Select

        card.SetBusy(True)
        Try
            Await Task.Delay(1500)
        Finally
            If Not card.IsDisposed Then card.SetBusy(False)
        End Try
    End Sub

    Private Sub Card_SettingsRequested(card As AppCard)
        OpenSettings(card.App.Id)
    End Sub

    Private Sub OpenSettings(Optional targetAppId As String = "")
        Using settingsForm As New frmSettings(targetAppId)
            If settingsForm.ShowDialog() <> DialogResult.OK Then Return
        End Using
        LoadApplications()
    End Sub
End Class
