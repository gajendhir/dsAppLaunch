<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AppCard
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        picIcon = New PictureBox()
        lblName = New Label()
        lblSubGroup = New Label()
        lblDate = New Label()
        lblDesc = New Label()
        btnSettings = New Button()
        btnLaunch = New Button()
        chkSelect = New CheckBox()
        toolTip = New ToolTip(components)
        CType(picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        ' picIcon
        '
        picIcon.BackColor = Color.Transparent
        picIcon.Location = New Point(14, 13)
        picIcon.Name = "picIcon"
        picIcon.Size = New Size(34, 34)
        picIcon.SizeMode = PictureBoxSizeMode.Zoom
        picIcon.TabIndex = 0
        picIcon.TabStop = False
        '
        ' lblName
        '
        lblName.AutoEllipsis = True
        lblName.BackColor = Color.Transparent
        lblName.Font = New Font("Segoe UI Semibold", 10.0F)
        lblName.ForeColor = Color.FromArgb(CByte(31), CByte(41), CByte(55))
        lblName.Location = New Point(56, 9)
        lblName.Name = "lblName"
        lblName.Size = New Size(130, 21)
        lblName.TabIndex = 1
        '
        ' lblSubGroup
        '
        lblSubGroup.AutoEllipsis = True
        lblSubGroup.BackColor = Color.Transparent
        lblSubGroup.Font = New Font("Segoe UI", 8.25F)
        lblSubGroup.ForeColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
        lblSubGroup.Location = New Point(56, 31)
        lblSubGroup.Name = "lblSubGroup"
        lblSubGroup.Size = New Size(152, 16)
        lblSubGroup.TabIndex = 2
        '
        ' lblDate
        '
        lblDate.AutoEllipsis = True
        lblDate.BackColor = Color.Transparent
        lblDate.Cursor = Cursors.Hand
        lblDate.Font = New Font("Segoe UI", 7.5F)
        lblDate.ForeColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
        lblDate.Location = New Point(46, 71)
        lblDate.Name = "lblDate"
        lblDate.Size = New Size(84, 16)
        lblDate.TabIndex = 3
        lblDate.TextAlign = ContentAlignment.MiddleCenter
        '
        ' lblDesc
        '
        lblDesc.AutoEllipsis = True
        lblDesc.BackColor = Color.Transparent
        lblDesc.Font = New Font("Segoe UI", 8.25F)
        lblDesc.ForeColor = Color.FromArgb(CByte(75), CByte(85), CByte(99))
        lblDesc.Location = New Point(56, 48)
        lblDesc.Name = "lblDesc"
        lblDesc.Size = New Size(152, 18)
        lblDesc.TabIndex = 4
        '
        ' btnSettings
        '
        btnSettings.Font = New Font("Segoe MDL2 Assets", 10.0F)
        btnSettings.Location = New Point(12, 68)
        btnSettings.Name = "btnSettings"
        btnSettings.Size = New Size(30, 24)
        btnSettings.TabIndex = 6
        btnSettings.UseVisualStyleBackColor = False
        '
        ' btnLaunch
        '
        btnLaunch.Font = New Font("Segoe UI Semibold", 8.25F)
        btnLaunch.Location = New Point(132, 68)
        btnLaunch.Name = "btnLaunch"
        btnLaunch.Size = New Size(76, 24)
        btnLaunch.TabIndex = 5
        btnLaunch.UseVisualStyleBackColor = False
        '
        ' chkSelect
        '
        chkSelect.BackColor = Color.Transparent
        chkSelect.Cursor = Cursors.Hand
        chkSelect.Location = New Point(190, 10)
        chkSelect.Name = "chkSelect"
        chkSelect.Size = New Size(20, 20)
        chkSelect.TabIndex = 7
        toolTip.SetToolTip(chkSelect, "Include in Launch Selected (this session only)")
        chkSelect.UseVisualStyleBackColor = False
        '
        ' toolTip
        '
        toolTip.AutoPopDelay = 5000
        toolTip.InitialDelay = 400
        toolTip.ReshowDelay = 200
        '
        ' AppCard
        '
        AutoScaleDimensions = New SizeF(96.0F, 96.0F)
        AutoScaleMode = AutoScaleMode.Dpi
        BackColor = Color.FromArgb(CByte(246), CByte(247), CByte(249))
        Controls.Add(picIcon)
        Controls.Add(lblName)
        Controls.Add(lblSubGroup)
        Controls.Add(lblDate)
        Controls.Add(lblDesc)
        Controls.Add(btnLaunch)
        Controls.Add(btnSettings)
        Controls.Add(chkSelect)
        Margin = New Padding(4)
        Name = "AppCard"
        Size = New Size(220, 100)
        CType(picIcon, System.ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents picIcon As PictureBox
    Friend WithEvents lblName As Label
    Friend WithEvents lblSubGroup As Label
    Friend WithEvents lblDate As Label
    Friend WithEvents lblDesc As Label
    Friend WithEvents btnSettings As Button
    Friend WithEvents btnLaunch As Button
    Friend WithEvents chkSelect As CheckBox
    Friend WithEvents toolTip As ToolTip
End Class
