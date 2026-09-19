Imports System.IO

Public Class frmSettings

    Private currentApps As List(Of AppConfig)
    Private isBinding As Boolean = False
    Private selectedAppId As String = ""

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(appIdToSelect As String)
        Me.New()
        selectedAppId = appIdToSelect
    End Sub

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()
    End Sub

    Private Sub LoadData()
        Try
            currentApps = AppRepository.Load()
        Catch
            currentApps = New List(Of AppConfig)()
        End Try

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
        txtGroup.Text = selectedApp.Group
        txtSubGroup.Text = selectedApp.SubGroup
        txtCategory.Text = selectedApp.Category
        txtDescription.Text = selectedApp.Description

        Try
            pnlColorPreview.BackColor = ColorTranslator.FromHtml(selectedApp.CardColor)
        Catch
            pnlColorPreview.BackColor = Color.White
        End Try
        isBinding = False
    End Sub

    Private Sub LstApps_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstApps.SelectedIndexChanged
        If isBinding OrElse lstApps.SelectedItem Is Nothing Then Return
        PopulateFields(CType(lstApps.SelectedItem, AppConfig))
    End Sub

    Private Sub UpdateCurrentModel(sender As Object, e As EventArgs) Handles txtName.TextChanged, txtExePath.TextChanged, txtIconPath.TextChanged, txtGroup.TextChanged, txtSubGroup.TextChanged, txtCategory.TextChanged, txtDescription.TextChanged
        If isBinding OrElse lstApps.SelectedItem Is Nothing Then Return

        Dim selectedApp As AppConfig = CType(lstApps.SelectedItem, AppConfig)
        selectedApp.Name = txtName.Text
        selectedApp.ExecutablePath = txtExePath.Text
        selectedApp.IconPath = txtIconPath.Text
        selectedApp.Group = txtGroup.Text
        selectedApp.SubGroup = txtSubGroup.Text
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

    Private Sub DropTarget_DragEnter(sender As Object, e As DragEventArgs) Handles txtExePath.DragEnter, txtIconPath.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.Copy, DragDropEffects.None)
    End Sub

    Private Sub LstApps_DragEnter(sender As Object, e As DragEventArgs) Handles lstApps.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
            lstApps.BackColor = Color.FromArgb(230, 244, 255)
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub LstApps_DragLeave(sender As Object, e As EventArgs) Handles lstApps.DragLeave
        lstApps.BackColor = SystemColors.Window
    End Sub

    ''' <summary>
    ''' Adds one new application entry per dropped file (folders are ignored).
    ''' AppLauncher starts files with UseShellExecute, so any file type launches via its
    ''' shell association and the dropped path is used as-is (including .lnk shortcuts)
    ''' without needing to resolve shortcut targets.
    ''' </summary>
    Private Sub LstApps_DragDrop(sender As Object, e As DragEventArgs) Handles lstApps.DragDrop
        lstApps.BackColor = SystemColors.Window
        If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim lastAdded As AppConfig = Nothing

        For Each filePath As String In droppedPaths
            If Not File.Exists(filePath) Then Continue For

            lastAdded = AppConfig.FromFile(filePath)
            currentApps.Add(lastAdded)
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

    Private Sub TxtExePath_DragDrop(sender As Object, e As DragEventArgs) Handles txtExePath.DragDrop
        If lstApps.SelectedItem Is Nothing OrElse Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If droppedPaths.Length = 0 OrElse Not File.Exists(droppedPaths(0)) Then Return

        txtExePath.Text = droppedPaths(0)
        If String.IsNullOrEmpty(txtName.Text) Then
            txtName.Text = Path.GetFileNameWithoutExtension(droppedPaths(0))
        End If
    End Sub

    Private Sub TxtIconPath_DragDrop(sender As Object, e As DragEventArgs) Handles txtIconPath.DragDrop
        If lstApps.SelectedItem Is Nothing OrElse Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If droppedPaths.Length = 0 OrElse Not File.Exists(droppedPaths(0)) Then Return

        txtIconPath.Text = droppedPaths(0)
    End Sub

    Private Sub BtnPickColor_Click(sender As Object, e As EventArgs) Handles btnPickColor.Click
        Using dlg As New ColorDialog()
            dlg.Color = pnlColorPreview.BackColor
            If dlg.ShowDialog() = DialogResult.OK Then
                pnlColorPreview.BackColor = dlg.Color
                UpdateCurrentModel(Nothing, Nothing)
            End If
        End Using
    End Sub

    Private Sub BtnBrowseExe_Click(sender As Object, e As EventArgs) Handles btnBrowseExe.Click
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

    Private Sub BtnBrowseIcon_Click(sender As Object, e As EventArgs) Handles btnBrowseIcon.Click
        Using dlg As New OpenFileDialog()
            dlg.Filter = "Image & Icon Files (*.ico;*.png;*.jpg)|*.ico;*.png;*.jpg|All Files (*.*)|*.*"
            If dlg.ShowDialog() = DialogResult.OK Then txtIconPath.Text = dlg.FileName
        End Using
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim newApp As AppConfig = AppConfig.Create("New Application", "")
        currentApps.Add(newApp)

        isBinding = True
        lstApps.DataSource = Nothing
        lstApps.DataSource = currentApps
        lstApps.DisplayMember = "Name"
        isBinding = False

        lstApps.SelectedIndex = currentApps.Count - 1
        PopulateFields(newApp)
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
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
        txtGroup.Clear()
        txtSubGroup.Clear()
        txtCategory.Clear()
        txtDescription.Clear()
        pnlColorPreview.BackColor = Color.White
        isBinding = False
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            AppRepository.Save(currentApps)
            MessageBox.Show("Configuration saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Failed to save apps.json: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
