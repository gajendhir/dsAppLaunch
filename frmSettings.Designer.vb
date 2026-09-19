<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSettings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        lblApps = New Label()
        lstApps = New ListBox()
        btnAdd = New Button()
        btnRemove = New Button()
        lblName = New Label()
        txtName = New TextBox()
        lblExePath = New Label()
        txtExePath = New TextBox()
        btnBrowseExe = New Button()
        lblIconPath = New Label()
        txtIconPath = New TextBox()
        btnBrowseIcon = New Button()
        lblGroup = New Label()
        txtGroup = New TextBox()
        lblSubGroup = New Label()
        txtSubGroup = New TextBox()
        lblCategory = New Label()
        txtCategory = New TextBox()
        lblCardColor = New Label()
        pnlColorPreview = New Panel()
        btnPickColor = New Button()
        lblDescription = New Label()
        txtDescription = New TextBox()
        lblSortOrder = New Label()
        nudSortOrder = New NumericUpDown()
        chkSelected = New CheckBox()
        btnSave = New Button()
        lblFooterSupport = New Label()
        toolTip = New ToolTip(components)
        CType(nudSortOrder, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        ' lblApps
        '
        lblApps.AutoSize = True
        lblApps.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        lblApps.Location = New Point(20, 15)
        lblApps.Name = "lblApps"
        lblApps.Size = New Size(140, 15)
        lblApps.TabIndex = 0
        lblApps.Text = "Configured Applications:"
        '
        ' lstApps
        '
        lstApps.AllowDrop = True
        lstApps.DisplayMember = "Name"
        lstApps.Font = New Font("Segoe UI", 9.5F)
        lstApps.ItemHeight = 17
        lstApps.Location = New Point(20, 40)
        lstApps.Name = "lstApps"
        lstApps.Size = New Size(200, 391)
        lstApps.TabIndex = 1
        toolTip.SetToolTip(lstApps, "Drag & drop .exe files (or any file/shortcut) here to add them as new applications")
        '
        ' btnAdd
        '
        btnAdd.BackColor = Color.White
        btnAdd.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        btnAdd.Location = New Point(20, 440)
        btnAdd.Name = "btnAdd"
        btnAdd.Size = New Size(95, 32)
        btnAdd.TabIndex = 2
        btnAdd.Text = "+ Add New"
        toolTip.SetToolTip(btnAdd, "Add a new application entry")
        btnAdd.UseVisualStyleBackColor = False
        '
        ' btnRemove
        '
        btnRemove.BackColor = Color.White
        btnRemove.Font = New Font("Segoe UI", 9.0F, FontStyle.Bold)
        btnRemove.ForeColor = Color.DarkRed
        btnRemove.Location = New Point(125, 440)
        btnRemove.Name = "btnRemove"
        btnRemove.Size = New Size(95, 32)
        btnRemove.TabIndex = 3
        btnRemove.Text = "- Remove"
        toolTip.SetToolTip(btnRemove, "Remove the selected application")
        btnRemove.UseVisualStyleBackColor = False
        '
        ' lblName
        '
        lblName.AutoSize = True
        lblName.Location = New Point(240, 15)
        lblName.Name = "lblName"
        lblName.Size = New Size(106, 15)
        lblName.TabIndex = 4
        lblName.Text = "Application Name:"
        '
        ' txtName
        '
        txtName.Location = New Point(240, 35)
        txtName.Name = "txtName"
        txtName.Size = New Size(400, 23)
        txtName.TabIndex = 5
        '
        ' lblExePath
        '
        lblExePath.AutoSize = True
        lblExePath.Location = New Point(240, 65)
        lblExePath.Name = "lblExePath"
        lblExePath.Size = New Size(130, 15)
        lblExePath.TabIndex = 6
        lblExePath.Text = "Executable (.exe) Path:"
        '
        ' txtExePath
        '
        txtExePath.AllowDrop = True
        txtExePath.Location = New Point(240, 85)
        txtExePath.Name = "txtExePath"
        txtExePath.Size = New Size(320, 23)
        txtExePath.TabIndex = 7
        toolTip.SetToolTip(txtExePath, "Browse, or drag & drop a file here")
        '
        ' btnBrowseExe
        '
        btnBrowseExe.Location = New Point(565, 84)
        btnBrowseExe.Name = "btnBrowseExe"
        btnBrowseExe.Size = New Size(75, 27)
        btnBrowseExe.TabIndex = 8
        btnBrowseExe.Text = "Browse..."
        toolTip.SetToolTip(btnBrowseExe, "Browse for the application's executable (.exe) file")
        btnBrowseExe.UseVisualStyleBackColor = True
        '
        ' lblIconPath
        '
        lblIconPath.AutoSize = True
        lblIconPath.Location = New Point(240, 115)
        lblIconPath.Name = "lblIconPath"
        lblIconPath.Size = New Size(160, 15)
        lblIconPath.TabIndex = 9
        lblIconPath.Text = "Custom Icon Path (Optional):"
        '
        ' txtIconPath
        '
        txtIconPath.AllowDrop = True
        txtIconPath.Location = New Point(240, 135)
        txtIconPath.Name = "txtIconPath"
        txtIconPath.Size = New Size(320, 23)
        txtIconPath.TabIndex = 10
        toolTip.SetToolTip(txtIconPath, "Browse, or drag & drop an icon/image file here")
        '
        ' btnBrowseIcon
        '
        btnBrowseIcon.Location = New Point(565, 134)
        btnBrowseIcon.Name = "btnBrowseIcon"
        btnBrowseIcon.Size = New Size(75, 27)
        btnBrowseIcon.TabIndex = 11
        btnBrowseIcon.Text = "Browse..."
        toolTip.SetToolTip(btnBrowseIcon, "Browse for a custom icon image")
        btnBrowseIcon.UseVisualStyleBackColor = True
        '
        ' lblGroup
        '
        lblGroup.AutoSize = True
        lblGroup.Location = New Point(240, 165)
        lblGroup.Name = "lblGroup"
        lblGroup.Size = New Size(102, 15)
        lblGroup.TabIndex = 12
        lblGroup.Text = "Group (Optional):"
        '
        ' txtGroup
        '
        txtGroup.Location = New Point(240, 185)
        txtGroup.Name = "txtGroup"
        txtGroup.Size = New Size(200, 23)
        txtGroup.TabIndex = 13
        '
        ' lblSubGroup
        '
        lblSubGroup.AutoSize = True
        lblSubGroup.Location = New Point(450, 165)
        lblSubGroup.Name = "lblSubGroup"
        lblSubGroup.Size = New Size(125, 15)
        lblSubGroup.TabIndex = 14
        lblSubGroup.Text = "Sub Group (Optional):"
        '
        ' txtSubGroup
        '
        txtSubGroup.Location = New Point(450, 185)
        txtSubGroup.Name = "txtSubGroup"
        txtSubGroup.Size = New Size(190, 23)
        txtSubGroup.TabIndex = 15
        '
        ' lblCategory
        '
        lblCategory.AutoSize = True
        lblCategory.Location = New Point(240, 215)
        lblCategory.Name = "lblCategory"
        lblCategory.Size = New Size(58, 15)
        lblCategory.TabIndex = 16
        lblCategory.Text = "Category:"
        '
        ' txtCategory
        '
        txtCategory.Location = New Point(240, 235)
        txtCategory.Name = "txtCategory"
        txtCategory.Size = New Size(200, 23)
        txtCategory.TabIndex = 17
        '
        ' lblCardColor
        '
        lblCardColor.AutoSize = True
        lblCardColor.Location = New Point(450, 215)
        lblCardColor.Name = "lblCardColor"
        lblCardColor.Size = New Size(132, 15)
        lblCardColor.TabIndex = 18
        lblCardColor.Text = "Card Background Color:"
        '
        ' pnlColorPreview
        '
        pnlColorPreview.BackColor = Color.White
        pnlColorPreview.BorderStyle = BorderStyle.FixedSingle
        pnlColorPreview.Location = New Point(450, 235)
        pnlColorPreview.Name = "pnlColorPreview"
        pnlColorPreview.Size = New Size(30, 25)
        pnlColorPreview.TabIndex = 19
        '
        ' btnPickColor
        '
        btnPickColor.Location = New Point(485, 234)
        btnPickColor.Name = "btnPickColor"
        btnPickColor.Size = New Size(155, 27)
        btnPickColor.TabIndex = 20
        btnPickColor.Text = "Pick Color..."
        toolTip.SetToolTip(btnPickColor, "Choose the card's background color")
        btnPickColor.UseVisualStyleBackColor = True
        '
        ' lblDescription
        '
        lblDescription.AutoSize = True
        lblDescription.Location = New Point(240, 265)
        lblDescription.Name = "lblDescription"
        lblDescription.Size = New Size(70, 15)
        lblDescription.TabIndex = 21
        lblDescription.Text = "Description:"
        '
        ' txtDescription
        '
        txtDescription.Location = New Point(240, 285)
        txtDescription.Multiline = True
        txtDescription.Name = "txtDescription"
        txtDescription.Size = New Size(400, 60)
        txtDescription.TabIndex = 22
        '
        ' lblSortOrder
        '
        lblSortOrder.AutoSize = True
        lblSortOrder.Location = New Point(240, 355)
        lblSortOrder.Name = "lblSortOrder"
        lblSortOrder.Size = New Size(130, 15)
        lblSortOrder.TabIndex = 23
        lblSortOrder.Text = "Sort Order (lower first):"
        '
        ' nudSortOrder
        '
        nudSortOrder.Location = New Point(240, 375)
        nudSortOrder.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        nudSortOrder.Minimum = New Decimal(New Integer() {99999, 0, 0, Integer.MinValue})
        nudSortOrder.Name = "nudSortOrder"
        nudSortOrder.Size = New Size(80, 23)
        nudSortOrder.TabIndex = 24
        toolTip.SetToolTip(nudSortOrder, "Position of this card within its group on the main screen. Equal values fall back to Sub Group order.")
        '
        ' chkSelected
        '
        chkSelected.AutoSize = True
        chkSelected.Location = New Point(340, 377)
        chkSelected.Name = "chkSelected"
        chkSelected.Size = New Size(252, 19)
        chkSelected.TabIndex = 25
        chkSelected.Text = "Include in 'Launch Selected' by default"
        toolTip.SetToolTip(chkSelected, "Saved default. On the main screen the tick can be changed for the current session without saving.")
        chkSelected.UseVisualStyleBackColor = True
        '
        ' btnSave
        '
        btnSave.BackColor = Color.FromArgb(CByte(46), CByte(204), CByte(113))
        btnSave.FlatStyle = FlatStyle.Flat
        btnSave.Font = New Font("Segoe UI", 10.0F, FontStyle.Bold)
        btnSave.ForeColor = Color.White
        btnSave.Location = New Point(420, 440)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(220, 35)
        btnSave.TabIndex = 26
        btnSave.Text = "💾 Save & Apply Changes"
        toolTip.SetToolTip(btnSave, "Save changes to apps.json")
        btnSave.UseVisualStyleBackColor = False
        '
        ' lblFooterSupport
        '
        lblFooterSupport.Font = New Font("Segoe UI", 8.5F, FontStyle.Bold)
        lblFooterSupport.ForeColor = Color.Red
        lblFooterSupport.Location = New Point(0, 495)
        lblFooterSupport.Name = "lblFooterSupport"
        lblFooterSupport.Size = New Size(705, 20)
        lblFooterSupport.TabIndex = 27
        lblFooterSupport.Text = "S/W Support By Data Spec"
        lblFooterSupport.TextAlign = ContentAlignment.MiddleRight
        '
        ' toolTip
        '
        toolTip.AutoPopDelay = 5000
        toolTip.InitialDelay = 400
        toolTip.ReshowDelay = 200
        '
        ' frmSettings
        '
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(245), CByte(246), CByte(250))
        ClientSize = New Size(704, 521)
        Controls.Add(lblApps)
        Controls.Add(lstApps)
        Controls.Add(btnAdd)
        Controls.Add(btnRemove)
        Controls.Add(lblName)
        Controls.Add(txtName)
        Controls.Add(lblExePath)
        Controls.Add(txtExePath)
        Controls.Add(btnBrowseExe)
        Controls.Add(lblIconPath)
        Controls.Add(txtIconPath)
        Controls.Add(btnBrowseIcon)
        Controls.Add(lblGroup)
        Controls.Add(txtGroup)
        Controls.Add(lblSubGroup)
        Controls.Add(txtSubGroup)
        Controls.Add(lblCategory)
        Controls.Add(txtCategory)
        Controls.Add(lblCardColor)
        Controls.Add(pnlColorPreview)
        Controls.Add(btnPickColor)
        Controls.Add(lblDescription)
        Controls.Add(txtDescription)
        Controls.Add(lblSortOrder)
        Controls.Add(nudSortOrder)
        Controls.Add(chkSelected)
        Controls.Add(btnSave)
        Controls.Add(lblFooterSupport)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmSettings"
        StartPosition = FormStartPosition.CenterParent
        Text = "Data Spec Application Launcher - Manager"
        CType(nudSortOrder, System.ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents lblApps As Label
    Friend WithEvents lstApps As ListBox
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnRemove As Button
    Friend WithEvents lblName As Label
    Friend WithEvents txtName As TextBox
    Friend WithEvents lblExePath As Label
    Friend WithEvents txtExePath As TextBox
    Friend WithEvents btnBrowseExe As Button
    Friend WithEvents lblIconPath As Label
    Friend WithEvents txtIconPath As TextBox
    Friend WithEvents btnBrowseIcon As Button
    Friend WithEvents lblGroup As Label
    Friend WithEvents txtGroup As TextBox
    Friend WithEvents lblSubGroup As Label
    Friend WithEvents txtSubGroup As TextBox
    Friend WithEvents lblCategory As Label
    Friend WithEvents txtCategory As TextBox
    Friend WithEvents lblCardColor As Label
    Friend WithEvents pnlColorPreview As Panel
    Friend WithEvents btnPickColor As Button
    Friend WithEvents lblDescription As Label
    Friend WithEvents txtDescription As TextBox
    Friend WithEvents lblSortOrder As Label
    Friend WithEvents nudSortOrder As NumericUpDown
    Friend WithEvents chkSelected As CheckBox
    Friend WithEvents btnSave As Button
    Friend WithEvents lblFooterSupport As Label
    Friend WithEvents toolTip As ToolTip
End Class
