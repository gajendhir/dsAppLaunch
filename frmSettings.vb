Imports System.IO
Imports System.Text.Json

Public Class frmSettings
    Inherits Form

    Private lstApps As ListBox
    Private txtName As TextBox
    Private txtExePath As TextBox
    Private txtIconPath As TextBox
    Private txtCategory As TextBox
    Private txtDescription As TextBox
    Private txtFirmName As TextBox
    Private txtFinancialYear As TextBox
    Private pnlColorPreview As Panel

    Private btnBrowseExe As Button
    Private btnBrowseIcon As Button
    Private btnPickColor As Button
    Private btnAdd As Button
    Private btnRemove As Button
    Private btnSave As Button
    Private lblFooterSupport As Label
    Private toolTip As New ToolTip With {.AutoPopDelay = 5000, .InitialDelay = 400, .ReshowDelay = 200}

    Private currentApps As List(Of AppConfig)
    Private isBinding As Boolean = False
    Private selectedAppId As String = ""

    Public Sub New(Optional appIdToSelect As String = "")
        Me.selectedAppId = appIdToSelect
        InitializeCustomComponents()
        LoadData()
    End Sub

    Private Sub InitializeCustomComponents()
        Me.Text = "Data Spec Application Launcher - Manager"
        Me.Size = New Size(720, 560)
        Me.StartPosition = FormStartPosition.CenterParent
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.FromArgb(245, 246, 250)

        Me.Controls.Add(New Label With {.Text = "Configured Applications:", .Location = New Point(20, 15), .AutoSize = True, .Font = New Font("Segoe UI", 9, FontStyle.Bold)})

        lstApps = New ListBox With {
            .Location = New Point(20, 40),
            .Size = New Size(200, 390),
            .DisplayMember = "Name",
            .Font = New Font("Segoe UI", 9.5F),
            .AllowDrop = True
        }
        AddHandler lstApps.SelectedIndexChanged, AddressOf LstApps_SelectedIndexChanged
        AddHandler lstApps.DragEnter, AddressOf LstApps_DragEnter
        AddHandler lstApps.DragLeave, AddressOf LstApps_DragLeave
        AddHandler lstApps.DragDrop, AddressOf LstApps_DragDrop
        Me.Controls.Add(lstApps)
        toolTip.SetToolTip(lstApps, "Drag & drop .exe files (or any file/shortcut) here to add them as new applications")

        btnAdd = New Button With {.Text = "+ Add New", .Location = New Point(20, 440), .Size = New Size(95, 32), .Font = New Font("Segoe UI", 9, FontStyle.Bold), .BackColor = Color.White}
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        Me.Controls.Add(btnAdd)
        toolTip.SetToolTip(btnAdd, "Add a new application entry")

        btnRemove = New Button With {.Text = "- Remove", .Location = New Point(125, 440), .Size = New Size(95, 32), .Font = New Font("Segoe UI", 9, FontStyle.Bold), .ForeColor = Color.DarkRed, .BackColor = Color.White}
        AddHandler btnRemove.Click, AddressOf BtnRemove_Click
        Me.Controls.Add(btnRemove)
        toolTip.SetToolTip(btnRemove, "Remove the selected application")

        Dim startX As Integer = 240
        Dim fieldWidth As Integer = 320

        ' Fields
        Me.Controls.Add(New Label With {.Text = "Application Name:", .Location = New Point(startX, 15), .AutoSize = True})
        txtName = New TextBox With {.Location = New Point(startX, 35), .Size = New Size(fieldWidth + 80, 25)}
        AddHandler txtName.TextChanged, AddressOf UpdateCurrentModel
        Me.Controls.Add(txtName)

        Me.Controls.Add(New Label With {.Text = "Executable (.exe) Path:", .Location = New Point(startX, 65), .AutoSize = True})
        txtExePath = New TextBox With {.Location = New Point(startX, 85), .Size = New Size(fieldWidth, 25), .AllowDrop = True}
        AddHandler txtExePath.TextChanged, AddressOf UpdateCurrentModel
        AddHandler txtExePath.DragEnter, AddressOf DropTarget_DragEnter
        AddHandler txtExePath.DragDrop, AddressOf TxtExePath_DragDrop
        Me.Controls.Add(txtExePath)
        toolTip.SetToolTip(txtExePath, "Browse, or drag & drop a file here")

        btnBrowseExe = New Button With {.Text = "Browse...", .Location = New Point(startX + fieldWidth + 5, 84), .Size = New Size(75, 27)}
        AddHandler btnBrowseExe.Click, AddressOf BtnBrowseExe_Click
        Me.Controls.Add(btnBrowseExe)
        toolTip.SetToolTip(btnBrowseExe, "Browse for the application's executable (.exe) file")

        Me.Controls.Add(New Label With {.Text = "Custom Icon Path (Optional):", .Location = New Point(startX, 115), .AutoSize = True})
        txtIconPath = New TextBox With {.Location = New Point(startX, 135), .Size = New Size(fieldWidth, 25), .AllowDrop = True}
        AddHandler txtIconPath.TextChanged, AddressOf UpdateCurrentModel
        AddHandler txtIconPath.DragEnter, AddressOf DropTarget_DragEnter
        AddHandler txtIconPath.DragDrop, AddressOf TxtIconPath_DragDrop
        Me.Controls.Add(txtIconPath)
        toolTip.SetToolTip(txtIconPath, "Browse, or drag & drop an icon/image file here")

        btnBrowseIcon = New Button With {.Text = "Browse...", .Location = New Point(startX + fieldWidth + 5, 134), .Size = New Size(75, 27)}
        AddHandler btnBrowseIcon.Click, AddressOf BtnBrowseIcon_Click
        Me.Controls.Add(btnBrowseIcon)
        toolTip.SetToolTip(btnBrowseIcon, "Browse for a custom icon image")

        Me.Controls.Add(New Label With {.Text = "Group By (Optional):", .Location = New Point(startX, 165), .AutoSize = True})
        txtFirmName = New TextBox With {.Location = New Point(startX, 185), .Size = New Size(200, 25)}
        AddHandler txtFirmName.TextChanged, AddressOf UpdateCurrentModel
        Me.Controls.Add(txtFirmName)

        Me.Controls.Add(New Label With {.Text = "Sub Title (Optional):", .Location = New Point(startX + 210, 165), .AutoSize = True})
        txtFinancialYear = New TextBox With {.Location = New Point(startX + 210, 185), .Size = New Size(190, 25)}
        AddHandler txtFinancialYear.TextChanged, AddressOf UpdateCurrentModel
        Me.Controls.Add(txtFinancialYear)

        Me.Controls.Add(New Label With {.Text = "Category:", .Location = New Point(startX, 215), .AutoSize = True})
        txtCategory = New TextBox With {.Location = New Point(startX, 235), .Size = New Size(200, 25)}
        AddHandler txtCategory.TextChanged, AddressOf UpdateCurrentModel
        Me.Controls.Add(txtCategory)

        Me.Controls.Add(New Label With {.Text = "Card Background Color:", .Location = New Point(startX + 210, 215), .AutoSize = True})
        pnlColorPreview = New Panel With {.Location = New Point(startX + 210, 235), .Size = New Size(30, 25), .BorderStyle = BorderStyle.FixedSingle, .BackColor = Color.White}
        btnPickColor = New Button With {.Text = "Pick Color...", .Location = New Point(startX + 245, 234), .Size = New Size(155, 27)}
        AddHandler btnPickColor.Click, AddressOf BtnPickColor_Click
        Me.Controls.Add(pnlColorPreview)
        Me.Controls.Add(btnPickColor)
        toolTip.SetToolTip(btnPickColor, "Choose the card's background color")

        Me.Controls.Add(New Label With {.Text = "Description:", .Location = New Point(startX, 265), .AutoSize = True})
        txtDescription = New TextBox With {.Location = New Point(startX, 285), .Size = New Size(fieldWidth + 80, 60), .Multiline = True}
        AddHandler txtDescription.TextChanged, AddressOf UpdateCurrentModel
        Me.Controls.Add(txtDescription)

        btnSave = New Button With {
            .Text = "💾 Save & Apply Changes",
            .Location = New Point(startX + 180, 440),
            .Size = New Size(220, 35),
            .Font = New Font("Segoe UI", 10, FontStyle.Bold),
            .BackColor = Color.FromArgb(46, 204, 113),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        Me.Controls.Add(btnSave)
        toolTip.SetToolTip(btnSave, "Save changes to apps.json")

        lblFooterSupport = New Label With {
            .Text = "S/W Support By Data Spec",
            .Font = New Font("Segoe UI", 8.5F, FontStyle.Bold),
            .ForeColor = Color.Red,
            .Location = New Point(0, 495),
            .Size = New Size(705, 20),
            .TextAlign = ContentAlignment.MiddleRight
        }
        Me.Controls.Add(lblFooterSupport)
    End Sub

    Private Sub LoadData()
        If File.Exists("apps.json") Then
            Try
                Dim jsonText As String = File.ReadAllText("apps.json")
                Dim options = New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
                currentApps = JsonSerializer.Deserialize(Of List(Of AppConfig))(jsonText, options)
            Catch
                currentApps = New List(Of AppConfig)()
            End Try
        Else
            currentApps = New List(Of AppConfig)()
        End If

        If currentApps Is Nothing Then currentApps = New List(Of AppConfig)()

        ' Bind ListBox
        isBinding = True
        lstApps.DataSource = Nothing
        lstApps.DataSource = currentApps
        lstApps.DisplayMember = "Name"
        isBinding = False

        ' Select requested app or default to first
        If currentApps.Count > 0 Then
            Dim targetApp As AppConfig = Nothing
            If Not String.IsNullOrEmpty(selectedAppId) Then
                targetApp = currentApps.FirstOrDefault(Function(x) x.Id = selectedAppId)
            End If

            If targetApp Is Nothing Then
                targetApp = currentApps(0)
            End If

            lstApps.SelectedItem = targetApp
            PopulateFields(targetApp)
        Else
            ClearFields()
        End If
    End Sub

    Private Sub PopulateFields(selectedApp As AppConfig)
        If selectedApp Is Nothing Then Return

        isBinding = True
        txtName.Text = selectedApp.Name
        txtExePath.Text = selectedApp.ExecutablePath
        txtIconPath.Text = selectedApp.IconPath
        txtFirmName.Text = selectedApp.FirmName
        txtFinancialYear.Text = selectedApp.FinancialYear
        txtCategory.Text = selectedApp.Category
        txtDescription.Text = selectedApp.Description

        Try
            pnlColorPreview.BackColor = ColorTranslator.FromHtml(selectedApp.CardColor)
        Catch
            pnlColorPreview.BackColor = Color.White
        End Try
        isBinding = False
    End Sub

    Private Sub LstApps_SelectedIndexChanged(sender As Object, e As EventArgs)
        If isBinding OrElse lstApps.SelectedItem Is Nothing Then Return
        PopulateFields(CType(lstApps.SelectedItem, AppConfig))
    End Sub

    Private Sub UpdateCurrentModel(sender As Object, e As EventArgs)
        If isBinding OrElse lstApps.SelectedItem Is Nothing Then Return

        Dim selectedApp As AppConfig = CType(lstApps.SelectedItem, AppConfig)
        selectedApp.Name = txtName.Text
        selectedApp.ExecutablePath = txtExePath.Text
        selectedApp.IconPath = txtIconPath.Text
        selectedApp.FirmName = txtFirmName.Text
        selectedApp.FinancialYear = txtFinancialYear.Text
        selectedApp.Category = txtCategory.Text
        selectedApp.Description = txtDescription.Text
        selectedApp.CardColor = ColorTranslator.ToHtml(pnlColorPreview.BackColor)

        isBinding = True
        Dim idx As Integer = lstApps.SelectedIndex
        lstApps.DataSource = Nothing
        lstApps.DataSource = currentApps
        lstApps.DisplayMember = "Name"
        lstApps.SelectedIndex = idx
        isBinding = False
    End Sub

    Private Sub DropTarget_DragEnter(sender As Object, e As DragEventArgs)
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.Copy, DragDropEffects.None)
    End Sub

    Private Sub LstApps_DragEnter(sender As Object, e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
            lstApps.BackColor = Color.FromArgb(230, 244, 255)
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub LstApps_DragLeave(sender As Object, e As EventArgs)
        lstApps.BackColor = SystemColors.Window
    End Sub

    ''' <summary>
    ''' Adds one new application entry per dropped file (folders are ignored).
    ''' ProcessStartInfo.UseShellExecute in frmMain launches any file type via its
    ''' shell association, so the dropped path is used as-is (including .lnk shortcuts)
    ''' without needing to resolve shortcut targets.
    ''' </summary>
    Private Sub LstApps_DragDrop(sender As Object, e As DragEventArgs)
        lstApps.BackColor = SystemColors.Window
        If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim lastAdded As AppConfig = Nothing

        For Each filePath As String In droppedPaths
            If Not File.Exists(filePath) Then Continue For

            Dim newApp As New AppConfig With {
                .Id = Guid.NewGuid().ToString(),
                .Name = Path.GetFileNameWithoutExtension(filePath),
                .ExecutablePath = filePath,
                .Category = "General",
                .CardColor = "#FFFFFF"
            }
            currentApps.Add(newApp)
            lastAdded = newApp
        Next

        If lastAdded Is Nothing Then Return

        isBinding = True
        lstApps.DataSource = Nothing
        lstApps.DataSource = currentApps
        lstApps.DisplayMember = "Name"
        isBinding = False

        lstApps.SelectedItem = lastAdded
        PopulateFields(lastAdded)
    End Sub

    Private Sub TxtExePath_DragDrop(sender As Object, e As DragEventArgs)
        If lstApps.SelectedItem Is Nothing OrElse Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If droppedPaths.Length = 0 OrElse Not File.Exists(droppedPaths(0)) Then Return

        txtExePath.Text = droppedPaths(0)
        If String.IsNullOrEmpty(txtName.Text) Then
            txtName.Text = Path.GetFileNameWithoutExtension(droppedPaths(0))
        End If
    End Sub

    Private Sub TxtIconPath_DragDrop(sender As Object, e As DragEventArgs)
        If lstApps.SelectedItem Is Nothing OrElse Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If droppedPaths.Length = 0 OrElse Not File.Exists(droppedPaths(0)) Then Return

        txtIconPath.Text = droppedPaths(0)
    End Sub

    Private Sub BtnPickColor_Click(sender As Object, e As EventArgs)
        Using dlg As New ColorDialog()
            dlg.Color = pnlColorPreview.BackColor
            If dlg.ShowDialog() = DialogResult.OK Then
                pnlColorPreview.BackColor = dlg.Color
                UpdateCurrentModel(Nothing, Nothing)
            End If
        End Using
    End Sub

    Private Sub BtnBrowseExe_Click(sender As Object, e As EventArgs)
        Using dlg As New OpenFileDialog()
            dlg.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"
            If dlg.ShowDialog() = DialogResult.OK Then
                txtExePath.Text = dlg.FileName
                If String.IsNullOrEmpty(txtName.Text) Then
                    txtName.Text = Path.GetFileNameWithoutExtension(dlg.FileName)
                End If
            End If
        End Using
    End Sub

    Private Sub BtnBrowseIcon_Click(sender As Object, e As EventArgs)
        Using dlg As New OpenFileDialog()
            dlg.Filter = "Image & Icon Files (*.ico;*.png;*.jpg)|*.ico;*.png;*.jpg|All Files (*.*)|*.*"
            If dlg.ShowDialog() = DialogResult.OK Then txtIconPath.Text = dlg.FileName
        End Using
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Dim newApp As New AppConfig With {
            .Id = Guid.NewGuid().ToString(),
            .Name = "New Application",
            .Category = "General",
            .CardColor = "#FFFFFF"
        }
        currentApps.Add(newApp)

        isBinding = True
        lstApps.DataSource = Nothing
        lstApps.DataSource = currentApps
        lstApps.DisplayMember = "Name"
        isBinding = False

        lstApps.SelectedIndex = currentApps.Count - 1
        PopulateFields(newApp)
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs)
        If lstApps.SelectedItem IsNot Nothing Then
            Dim selectedApp As AppConfig = CType(lstApps.SelectedItem, AppConfig)
            currentApps.Remove(selectedApp)

            isBinding = True
            lstApps.DataSource = Nothing
            lstApps.DataSource = currentApps
            lstApps.DisplayMember = "Name"
            isBinding = False

            If currentApps.Count > 0 Then
                lstApps.SelectedIndex = 0
                PopulateFields(currentApps(0))
            Else
                ClearFields()
            End If
        End If
    End Sub

    Private Sub ClearFields()
        isBinding = True
        txtName.Clear()
        txtExePath.Clear()
        txtIconPath.Clear()
        txtFirmName.Clear()
        txtFinancialYear.Clear()
        txtCategory.Clear()
        txtDescription.Clear()
        pnlColorPreview.BackColor = Color.White
        isBinding = False
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        Try
            Dim options = New JsonSerializerOptions With {.WriteIndented = True}
            Dim jsonText As String = JsonSerializer.Serialize(currentApps, options)
            File.WriteAllText("apps.json", jsonText)
            MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Failed to save apps.json: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class