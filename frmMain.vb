Public Class frmMain

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

    Public Sub LoadApplications()
        EnsureJsonFileExists()

        flowLayout.SuspendLayout()
        flowLayout.Controls.Clear()

        Try
            Dim headerWidth As Integer = Math.Max(200, flowLayout.ClientSize.Width - 35)

            For Each appGroup In AppGrouping.GroupAndSort(AppRepository.Load())
                ' Group Header Label
                Dim lblGroupHeader As New Label With {
                    .Text = appGroup.Title,
                    .Font = New Font("Segoe UI", 12, FontStyle.Bold),
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
                    card.Bind(app)
                    AddHandler card.LaunchRequested, AddressOf Card_LaunchRequested
                    AddHandler card.SettingsRequested, AddressOf Card_SettingsRequested
                    flowLayout.Controls.Add(card)
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
        Dim settingsForm As New frmSettings(targetAppId)
        If settingsForm.ShowDialog() = DialogResult.OK Then
            LoadApplications()
        End If
    End Sub
End Class
