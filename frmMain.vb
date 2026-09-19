Public Class frmMain
    Private Shared ReadOnly HeaderFont As New Font("Segoe UI", 12, FontStyle.Bold)

    Private Const LaunchStaggerMs As Integer = 1000

    ' Incremented per reload; probe results from an older load are ignored.
    Private loadVersion As Integer

    ' Ticks for "Launch Selected": session-only, re-seeded from AppConfig.Selected on every (re)load.
    Private ReadOnly selection As New SelectionState()
    Private screenOrder As New List(Of AppConfig)()
    Private cardByApp As New Dictionary(Of AppConfig, AppCard)()
    Private isLaunchingSelected As Boolean

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
        selection.ResetFrom(Enumerable.Empty(Of AppConfig)())
        screenOrder = New List(Of AppConfig)()
        cardByApp = New Dictionary(Of AppConfig, AppCard)()

        Try
            Dim apps As List(Of AppConfig) = AppRepository.Load()
            Dim groups As List(Of AppGroup) = AppGrouping.GroupAndSort(apps)
            screenOrder = AppGrouping.ScreenOrder(groups)
            selection.ResetFrom(apps)
            Dim headerWidth As Integer = Math.Max(200, flowLayout.ClientSize.Width - 35)

            For Each appGroup In groups
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
                    card.IsSelected = selection.IsSelected(app)
                    cardByApp(app) = card
                    AddHandler card.SelectionChanged, AddressOf Card_SelectionChanged
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
            UpdateLaunchSelectedButton()
        End Try
    End Sub

    Private Sub UpdateLaunchSelectedButton()
        btnLaunchSelected.Text = $"▶ Launch Selected ({selection.Count})"
        btnLaunchSelected.Enabled = selection.Count > 0 AndAlso Not isLaunchingSelected
    End Sub

    Private Sub Card_SelectionChanged(card As AppCard)
        selection.SetSelected(card.App, card.IsSelected)
        UpdateLaunchSelectedButton()
    End Sub

    Private Sub SetAllSelected(value As Boolean)
        selection.SetAll(screenOrder, value)
        For Each card As AppCard In cardByApp.Values
            card.IsSelected = value
        Next
        UpdateLaunchSelectedButton()
    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        SetAllSelected(True)
    End Sub

    Private Sub btnSelectNone_Click(sender As Object, e As EventArgs) Handles btnSelectNone.Click
        SetAllSelected(False)
    End Sub

    Private Async Sub btnLaunchSelected_Click(sender As Object, e As EventArgs) Handles btnLaunchSelected.Click
        Dim toLaunch As List(Of AppConfig) = selection.SelectedIn(screenOrder)
        If toLaunch.Count = 0 OrElse isLaunchingSelected Then Return

        isLaunchingSelected = True
        UpdateLaunchSelectedButton()
        Try
            Dim results As List(Of LaunchResult) = Await AppLauncher.LaunchManyAsync(toLaunch, LaunchStaggerMs,
                Sub(index, total, result)
                    If IsDisposed Then Return
                    btnLaunchSelected.Text = $"⏳ Launched {index}/{total}..."
                    Dim card As AppCard = Nothing
                    If result.Status = LaunchStatus.Started AndAlso cardByApp.TryGetValue(result.App, card) Then ShowBusy(card)
                End Sub)

            If IsDisposed Then Return
            Dim problems As List(Of LaunchResult) = results.Where(Function(r) r.Status <> LaunchStatus.Started).ToList()
            If problems.Count > 0 Then
                Dim lines As IEnumerable(Of String) = problems.Select(Function(r) If(r.Status = LaunchStatus.NotFound,
                    $"• {r.App.Name}: not found at {r.App.ExecutablePath}",
                    $"• {r.App.Name}: {r.Message}"))
                MessageBox.Show($"{results.Count - problems.Count} of {results.Count} applications started. Problems:" & vbCrLf & vbCrLf & String.Join(vbCrLf, lines),
                                "Launch Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        Finally
            isLaunchingSelected = False
            If Not IsDisposed Then UpdateLaunchSelectedButton()
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

    Private Sub Card_LaunchRequested(card As AppCard)
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

        ShowBusy(card)
    End Sub

    Private Async Sub ShowBusy(card As AppCard)
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
