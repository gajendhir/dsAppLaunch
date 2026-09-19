Imports System.ComponentModel
Imports System.IO

Public Class frmSettings

    Private currentApps As BindingList(Of AppConfig)
    Private isBinding As Boolean = False
    Private selectedAppId As String = ""

    Public Sub New()
        InitializeComponent()
        Icon = Theme.AppIcon()

        ' Interactive button looks live in code (see Theme): the designer can't express hover/disabled states.
        Theme.StylePrimary(btnSave)
        For Each b As Button In {btnAdd, btnRemove, btnBrowseExe, btnBrowseIcon, btnPickColor}
            Theme.StyleGhost(b)
        Next
        btnRemove.ForeColor = Color.FromArgb(185, 28, 28)
        lstApps.BackColor = Theme.Surface
    End Sub

    Private Sub pnlHeader_Paint(sender As Object, e As PaintEventArgs) Handles pnlHeader.Paint
        Dim logoSize As Integer = Theme.Scale(pnlHeader, 38)
        Dim left As Integer = Theme.Scale(pnlHeader, 18)
        Theme.DrawLogo(e.Graphics, New Rectangle(left, Theme.Scale(pnlHeader, 12), logoSize, logoSize))
        Using pen As New Pen(Theme.Border)
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1)
        End Using
        Using brush As New SolidBrush(Theme.AccentYellow)
            e.Graphics.FillRectangle(brush, left, pnlHeader.Height - Theme.Scale(pnlHeader, 3), logoSize, Theme.Scale(pnlHeader, 3))
        End Using
    End Sub

    Private Sub lstApps_DrawItem(sender As Object, e As DrawItemEventArgs) Handles lstApps.DrawItem
        If e.Index < 0 Then Return
        Dim isSelected As Boolean = (e.State And DrawItemState.Selected) = DrawItemState.Selected
        Using back As New SolidBrush(If(isSelected, Theme.Blend(Theme.Surface, Theme.Accent, 0.1), lstApps.BackColor))
            e.Graphics.FillRectangle(back, e.Bounds)
        End Using
        If isSelected Then
            Using bar As New SolidBrush(Theme.Accent)
                e.Graphics.FillRectangle(bar, e.Bounds.X, e.Bounds.Y, Theme.Scale(lstApps, 3), e.Bounds.Height)
            End Using
        End If
        Dim pad As Integer = Theme.Scale(lstApps, 12)
        Dim textBounds As New Rectangle(e.Bounds.X + pad, e.Bounds.Y, e.Bounds.Width - pad * 2, e.Bounds.Height)
        TextRenderer.DrawText(e.Graphics, lstApps.GetItemText(lstApps.Items(e.Index)), e.Font, textBounds,
                              Theme.TextPrimary,
                              TextFormatFlags.VerticalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)
    End Sub

    Public Sub New(appIdToSelect As String)
        Me.New()
        selectedAppId = appIdToSelect
    End Sub

    Private Sub frmSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not LoadData() Then Close()
    End Sub

    ''' <summary>Returns False (after telling the user) if apps.json exists but can't be read, so a later Save can't overwrite it.</summary>
    Private Function LoadData() As Boolean
        Dim loaded As List(Of AppConfig)
        Try
            loaded = AppRepository.Load()
        Catch ex As Exception
            MessageBox.Show("Settings can't be opened because apps.json could not be read:" & vbCrLf & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                            "Fix or delete the file and try again. It has not been changed.",
                            "apps.json unreadable", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End Try
        currentApps = New BindingList(Of AppConfig)(loaded)

        ' Bind ListBox once; edits below refresh only the changed item
        isBinding = True
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
        Return True
    End Function

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
        nudSortOrder.Value = Math.Clamp(CDec(selectedApp.SortOrder), nudSortOrder.Minimum, nudSortOrder.Maximum)
        chkSelected.Checked = selectedApp.Selected

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

    Private Sub UpdateCurrentModel(sender As Object, e As EventArgs) Handles txtName.TextChanged, txtExePath.TextChanged, txtIconPath.TextChanged, txtGroup.TextChanged, txtSubGroup.TextChanged, txtCategory.TextChanged, txtDescription.TextChanged, nudSortOrder.ValueChanged, chkSelected.CheckedChanged
        If isBinding OrElse lstApps.SelectedItem Is Nothing Then Return

        Dim selectedApp As AppConfig = CType(lstApps.SelectedItem, AppConfig)
        selectedApp.Name = txtName.Text
        selectedApp.ExecutablePath = txtExePath.Text
        selectedApp.IconPath = txtIconPath.Text
        selectedApp.Group = txtGroup.Text
        selectedApp.SubGroup = txtSubGroup.Text
        selectedApp.Category = txtCategory.Text
        selectedApp.Description = txtDescription.Text
        selectedApp.SortOrder = CInt(nudSortOrder.Value)
        selectedApp.Selected = chkSelected.Checked
        selectedApp.CardColor = ColorTranslator.ToHtml(pnlColorPreview.BackColor)

        isBinding = True
        currentApps.ResetItem(lstApps.SelectedIndex)
        isBinding = False
    End Sub

    Private Sub DropTarget_DragEnter(sender As Object, e As DragEventArgs) Handles txtExePath.DragEnter, txtIconPath.DragEnter
        e.Effect = If(e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.Copy, DragDropEffects.None)
    End Sub

    Private Sub LstApps_DragEnter(sender As Object, e As DragEventArgs) Handles lstApps.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
            lstApps.BackColor = Theme.BusyTint
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub LstApps_DragLeave(sender As Object, e As EventArgs) Handles lstApps.DragLeave
        lstApps.BackColor = Theme.Surface
    End Sub

    ''' <summary>
    ''' Adds one new application entry per dropped file (folders are ignored).
    ''' AppLauncher starts files with UseShellExecute, so any file type launches via its
    ''' shell association and the dropped path is used as-is (including .lnk shortcuts)
    ''' without needing to resolve shortcut targets.
    ''' </summary>
    Private Sub LstApps_DragDrop(sender As Object, e As DragEventArgs) Handles lstApps.DragDrop
        lstApps.BackColor = Theme.Surface
        If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then Return

        Dim droppedPaths() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        Dim lastAdded As AppConfig = Nothing

        For Each filePath As String In droppedPaths
            If Not File.Exists(filePath) Then Continue For

            lastAdded = AppConfig.FromFile(filePath)
            currentApps.Add(lastAdded)
        Next

        If lastAdded Is Nothing Then Return

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

        lstApps.SelectedIndex = currentApps.Count - 1
        PopulateFields(newApp)
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        If lstApps.SelectedItem IsNot Nothing Then
            Dim selectedApp As AppConfig = CType(lstApps.SelectedItem, AppConfig)
            currentApps.Remove(selectedApp)

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
        nudSortOrder.Value = 0
        chkSelected.Checked = False
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
