Imports System.IO
Imports System.Text.Json

Public Class frmMain
    Private WithEvents flowLayout As FlowLayoutPanel
    Private topPanel As Panel
    Private bottomPanel As Panel
    Private lblHeaderTitle As Label
    Private lblSupportFooter As LinkLabel
    Private btnReload As Button
    Private btnSettings As Button
    Private appsList As List(Of AppConfig)
    Private toolTip As New ToolTip With {.AutoPopDelay = 5000, .InitialDelay = 400, .ReshowDelay = 200}
    Private FontName As String, FontSize As Single

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Data Spec Application Launcher"
        Me.Size = New Size(950, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb(245, 246, 250)

        FontName = Me.Font.Name
        FontSize = Me.Font.Size
        ' --- Ensure apps.json exists on startup ---
        EnsureJsonFileExists("apps.json")

        ' --- Top Header Panel ---
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 75,
            .BackColor = Color.FromArgb(28, 40, 51),
            .Padding = New Padding(10)
        }

        lblHeaderTitle = New Label With {
            .Text = "Data Spec Application Launcher",               '.Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .Font = New Font(FontName, FontSize * 1.5F, FontStyle.Bold),
            .BackColor = Color.Transparent,
            .ForeColor = Color.White,
            .Dock = DockStyle.Top,
            .Height = 30,
            .TextAlign = ContentAlignment.MiddleCenter
        }

        btnReload = New Button With {
            .Text = "🔄",
            .Size = New Size(30, 30),
            .Location = New Point(Me.ClientSize.Width - 30 - 8, 37),        '            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .Font = New Font(FontName, FontSize * 1.25F, FontStyle.Regular),
            .BackColor = Color.Transparent,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }
        AddHandler btnReload.Click, Sub(s, ev) LoadApplications("apps.json")
        toolTip.SetToolTip(btnReload, "Reload application list")

        btnSettings = New Button With {
            .Text = "⚙️",
            .Size = New Size(30, 30),
            .Location = New Point(Me.ClientSize.Width - 30 * 2 - 8 * 2, 37),            '            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .Font = New Font(FontName, FontSize * 1.25F, FontStyle.Regular),
            .BackColor = Color.Transparent,
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }
        AddHandler btnSettings.Click, Sub(s, ev) OpenSettings()
        toolTip.SetToolTip(btnSettings, "Open settings to manage applications")

        topPanel.Controls.Add(btnReload)
        topPanel.Controls.Add(btnSettings)
        topPanel.Controls.Add(lblHeaderTitle)
        Me.Controls.Add(topPanel)
        btnSettings.Anchor = CType(AnchorStyles.Right Or AnchorStyles.Top, AnchorStyles)
        btnReload.Anchor = CType(AnchorStyles.Right Or AnchorStyles.Top, AnchorStyles)
        ' --- Bottom Support Footer ---
        bottomPanel = New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 30,
            .BackColor = Color.FromArgb(235, 238, 242),
            .Padding = New Padding(0, 0, 20, 0)
        }

        lblSupportFooter = New LinkLabel With {
            .Text = "S/W Support By Data Spec",
            .Font = New Font(FontName, FontSize, FontStyle.Regular),
            .LinkColor = Color.Red,
            .ActiveLinkColor = Color.Red,
            .VisitedLinkColor = Color.Red,
            .LinkBehavior = LinkBehavior.HoverUnderline,
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleRight
        }
        AddHandler lblSupportFooter.LinkClicked, Sub(s, ev) Process.Start(New ProcessStartInfo With {.FileName = "https://www.dataspec.info", .UseShellExecute = True})
        toolTip.SetToolTip(lblSupportFooter, "Visit https://www.dataspec.info")

        bottomPanel.Controls.Add(lblSupportFooter)
        Me.Controls.Add(bottomPanel)

        ' --- Center Scrollable Layout ---
        flowLayout = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .Padding = New Padding(15, 10, 15, 15),
            .WrapContents = True
        }
        Me.Controls.Add(flowLayout)
        flowLayout.BringToFront()

        LoadApplications("apps.json")
    End Sub

    ''' <summary>
    ''' Automatically creates the apps.json file if it does not exist.
    ''' </summary>
    Private Sub EnsureJsonFileExists(jsonFilePath As String)
        Try
            If Not File.Exists(jsonFilePath) Then
                ' Create default empty JSON structure
                File.WriteAllText(jsonFilePath, "[]")
            End If
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

    Public Sub LoadApplications(jsonFilePath As String)
        EnsureJsonFileExists(jsonFilePath)

        flowLayout.SuspendLayout()
        flowLayout.Controls.Clear()

        Try
            Dim jsonText As String = File.ReadAllText(jsonFilePath)
            Dim options = New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
            appsList = JsonSerializer.Deserialize(Of List(Of AppConfig))(jsonText, options)

            If appsList Is Nothing Then appsList = New List(Of AppConfig)()

            Dim headerWidth As Integer = Math.Max(200, flowLayout.ClientSize.Width - 35)

            ' Grouping by Firm Name
            Dim firmGroups = appsList.GroupBy(Function(x) If(String.IsNullOrWhiteSpace(x.FirmName), "", x.FirmName))

            For Each firmGroup In firmGroups
                ' Firm Header Label
                Dim lblFirmHeader As New Label With {
                    .Text = If(String.IsNullOrEmpty(firmGroup.Key), "(Ungrouped)", firmGroup.Key),
                    .Font = New Font("Segoe UI", 12, FontStyle.Bold),
                    .ForeColor = Color.FromArgb(41, 128, 185),
                    .Size = New Size(headerWidth, 24),
                    .Margin = New Padding(5, 6, 5, 2),
                    .AutoSize = False,
                    .AutoEllipsis = True,
                    .Tag = "HeaderLabel"
                }
                flowLayout.Controls.Add(lblFirmHeader)
                flowLayout.SetFlowBreak(lblFirmHeader, True)

                ' Arrange cards sequentially by Financial Year in DESCENDING order
                Dim orderedApps = firmGroup.OrderByDescending(Function(x) If(String.IsNullOrWhiteSpace(x.FinancialYear), "", x.FinancialYear)).ToList()

                For Each app In orderedApps
                    Dim card As Panel = CreateAppCard(app)
                    flowLayout.Controls.Add(card)
                Next

                ' Break panel forces the next firm section to start on a NEW line below cards
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

    Private Function CreateAppCard(app As AppConfig) As Panel
        Dim baseColor As Color = ParseColor(app.CardColor, Color.White)

        Dim card As New Panel With {
            .Size = New Size(225, 125),
            .Margin = New Padding(8),
            .BackColor = baseColor,
            .BorderStyle = BorderStyle.FixedSingle,
            .Tag = app
        }

        Dim exeFileName As String = If(String.IsNullOrEmpty(app.ExecutablePath), "", Path.GetFileName(app.ExecutablePath))

        Dim picIcon As New PictureBox With {
            .Size = New Size(32, 32),
            .Location = New Point(8, 8),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Image = GetAppIcon(app)
        }
        toolTip.SetToolTip(picIcon, exeFileName)

        Dim lblName As New Label With {
            .Text = app.Name,
            .Font = New Font(FontName, FontSize * 1.25F, FontStyle.Bold),
            .ForeColor = Color.FromArgb(44, 62, 80),
            .Location = New Point(48, 6),
            .Size = New Size(150, 18),
            .AutoEllipsis = True
        }
        toolTip.SetToolTip(lblName, exeFileName)

        Dim nextTop As Integer = 25

        ' 1. Financial Year Label
        Dim hasFy As Boolean = Not String.IsNullOrWhiteSpace(app.FinancialYear)
        Dim lblFinancialYear As Label = Nothing
        If hasFy Then
            lblFinancialYear = New Label With {
                .Text = app.FinancialYear,
                .Font = New Font(FontName, FontSize * 1.1F, FontStyle.Italic),
                .ForeColor = Color.FromArgb(127, 140, 141),
                .Location = New Point(48, nextTop),
                .Size = New Size(150, 15),
                .AutoEllipsis = True
            }
            nextTop += 16
        End If

        ' 2. Application Date Label (Double-click opens target folder)
        Dim hasExeDate As Boolean = Not String.IsNullOrEmpty(app.ExecutablePath) AndAlso File.Exists(app.ExecutablePath)
        Dim lblDate As Label = Nothing
        If hasExeDate Then
            Dim exeDateStr As String = File.GetLastWriteTime(app.ExecutablePath).ToString("dd-MM-yyyy HH:mm")
            lblDate = New Label With {
                .Text = $"📅 {exeDateStr}",
                .Font = New Font(FontName, FontSize, FontStyle.Regular),
                .ForeColor = Color.FromArgb(39, 174, 96),
                .Location = New Point(48, nextTop),
                .Size = New Size(150, 15),
                .AutoEllipsis = True,
                .Cursor = Cursors.Hand
            }

            ' Open File Explorer and select the file on double click
            AddHandler lblDate.DoubleClick, Sub(s, e)
                                                If File.Exists(app.ExecutablePath) Then
                                                    Process.Start("explorer.exe", $"/select,""{app.ExecutablePath}""")
                                                End If
                                            End Sub
            nextTop += 16
        End If

        ' 3. Description Label
        Dim descHeight As Integer = Math.Max(20, 80 - nextTop)
        Dim lblDesc As New Label With {
            .Text = If(String.IsNullOrEmpty(app.Category), app.Description, $"[{app.Category}] {app.Description}"),
            .Font = New Font(FontName, FontSize, FontStyle.Regular),
            .ForeColor = Color.DimGray,
            .Location = New Point(48, nextTop),
            .Size = New Size(150, descHeight),
            .AutoEllipsis = True
        }

        ' Action Buttons
        Dim btnLaunchCard As New Button With {
            .Text = "▶ Launch",
            .Size = New Size(95, 30),
            .Location = New Point(118, 85),
            .Font = New Font(FontName, FontSize * 1.1F, FontStyle.Regular),
            .BackColor = Color.FromArgb(46, 204, 113),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }
        btnLaunchCard.FlatAppearance.BorderSize = 0
        AddHandler btnLaunchCard.Click, Async Sub(s, e) Await LaunchAppWithBusyState(card, btnLaunchCard, app)
        toolTip.SetToolTip(btnLaunchCard, $"Launch {app.Name}")

        Dim btnSettingsCard As New Button With {
            .Text = "⚙",         ' "⚙ Settings",
            .Size = New Size(32, 30),
            .Location = New Point(8, 85),
            .Font = New Font(FontName, FontSize * 1.1F, FontStyle.Regular),
            .BackColor = Color.Transparent,      ' Color.FromArgb(225, 225, 225),
            .ForeColor = Color.FromArgb(50, 50, 50),
            .FlatStyle = FlatStyle.Flat,
            .Cursor = Cursors.Hand
        }
        btnSettingsCard.FlatAppearance.BorderSize = 0
        AddHandler btnSettingsCard.Click, Sub(s, e) OpenSettings(app.Id)
        toolTip.SetToolTip(btnSettingsCard, $"Edit settings for {app.Name}")

        ' Add controls to card
        card.Controls.Add(picIcon)
        card.Controls.Add(lblName)
        If hasFy Then card.Controls.Add(lblFinancialYear)
        If hasExeDate Then card.Controls.Add(lblDate)
        card.Controls.Add(lblDesc)
        card.Controls.Add(btnLaunchCard)
        card.Controls.Add(btnSettingsCard)

        Return card
    End Function

    Private Async Function LaunchAppWithBusyState(card As Panel, btnLaunch As Button, app As AppConfig) As Task
        If String.IsNullOrEmpty(app.ExecutablePath) OrElse Not File.Exists(app.ExecutablePath) Then
            MessageBox.Show($"Target executable not found at:" & vbCrLf & app.ExecutablePath, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim origColor As Color = card.BackColor
        card.BackColor = Color.FromArgb(254, 249, 231)
        btnLaunch.Enabled = False
        btnLaunch.Text = "⏳ Busy..."

        Try
            Dim pInfo As New ProcessStartInfo With {
                .FileName = app.ExecutablePath,
                .WorkingDirectory = Path.GetDirectoryName(app.ExecutablePath),
                .UseShellExecute = True
            }
            Process.Start(pInfo)
            Await Task.Delay(1500)
        Catch ex As Exception
            MessageBox.Show("Error launching application: " & ex.Message, "Execution Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            card.BackColor = origColor
            btnLaunch.Text = "▶ Launch"
            btnLaunch.Enabled = True
        End Try
    End Function

    Private Function GetAppIcon(app As AppConfig) As Image
        If Not String.IsNullOrEmpty(app.IconPath) AndAlso File.Exists(app.IconPath) Then
            Try : Return Image.FromFile(app.IconPath) : Catch : End Try
        End If

        If Not String.IsNullOrEmpty(app.ExecutablePath) AndAlso File.Exists(app.ExecutablePath) Then
            Try
                Dim extractedIcon As Icon = Icon.ExtractAssociatedIcon(app.ExecutablePath)
                If extractedIcon IsNot Nothing Then Return extractedIcon.ToBitmap()
            Catch : End Try
        End If

        Return SystemIcons.Application.ToBitmap()
    End Function

    Private Sub OpenSettings(Optional targetAppId As String = "")
        Dim settingsForm As New frmSettings(targetAppId)
        If settingsForm.ShowDialog() = DialogResult.OK Then
            LoadApplications("apps.json")
        End If
    End Sub

    Private Function ParseColor(hexColor As String, defaultColor As Color) As Color
        If String.IsNullOrWhiteSpace(hexColor) Then Return defaultColor
        Try
            Return ColorTranslator.FromHtml(hexColor)
        Catch
            Return defaultColor
        End Try
    End Function
End Class