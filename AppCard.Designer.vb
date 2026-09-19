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
        toolTip = New ToolTip(components)
        CType(picIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        ' picIcon
        '
        picIcon.Location = New Point(8, 8)
        picIcon.Name = "picIcon"
        picIcon.Size = New Size(32, 32)
        picIcon.SizeMode = PictureBoxSizeMode.Zoom
        picIcon.TabIndex = 0
        picIcon.TabStop = False
        '
        ' lblName
        '
        lblName.AutoEllipsis = True
        lblName.Font = New Font("Segoe UI", 11.25F, FontStyle.Bold)
        lblName.ForeColor = Color.FromArgb(CByte(44), CByte(62), CByte(80))
        lblName.Location = New Point(48, 6)
        lblName.Name = "lblName"
        lblName.Size = New Size(150, 18)
        lblName.TabIndex = 1
        '
        ' lblSubGroup
        '
        lblSubGroup.AutoEllipsis = True
        lblSubGroup.Font = New Font("Segoe UI", 9.9F, FontStyle.Italic)
        lblSubGroup.ForeColor = Color.FromArgb(CByte(127), CByte(140), CByte(141))
        lblSubGroup.Location = New Point(48, 25)
        lblSubGroup.Name = "lblSubGroup"
        lblSubGroup.Size = New Size(150, 15)
        lblSubGroup.TabIndex = 2
        '
        ' lblDate
        '
        lblDate.AutoEllipsis = True
        lblDate.Cursor = Cursors.Hand
        lblDate.ForeColor = Color.FromArgb(CByte(39), CByte(174), CByte(96))
        lblDate.Location = New Point(48, 41)
        lblDate.Name = "lblDate"
        lblDate.Size = New Size(150, 15)
        lblDate.TabIndex = 3
        '
        ' lblDesc
        '
        lblDesc.AutoEllipsis = True
        lblDesc.ForeColor = Color.DimGray
        lblDesc.Location = New Point(48, 57)
        lblDesc.Name = "lblDesc"
        lblDesc.Size = New Size(150, 23)
        lblDesc.TabIndex = 4
        '
        ' btnSettings
        '
        btnSettings.BackColor = Color.Transparent
        btnSettings.Cursor = Cursors.Hand
        btnSettings.FlatAppearance.BorderSize = 0
        btnSettings.FlatStyle = FlatStyle.Flat
        btnSettings.Font = New Font("Segoe UI", 9.9F)
        btnSettings.ForeColor = Color.FromArgb(CByte(50), CByte(50), CByte(50))
        btnSettings.Location = New Point(8, 85)
        btnSettings.Name = "btnSettings"
        btnSettings.Size = New Size(32, 30)
        btnSettings.TabIndex = 6
        btnSettings.Text = "⚙"
        btnSettings.UseVisualStyleBackColor = False
        '
        ' btnLaunch
        '
        btnLaunch.BackColor = Color.FromArgb(CByte(46), CByte(204), CByte(113))
        btnLaunch.Cursor = Cursors.Hand
        btnLaunch.FlatAppearance.BorderSize = 0
        btnLaunch.FlatStyle = FlatStyle.Flat
        btnLaunch.Font = New Font("Segoe UI", 9.9F)
        btnLaunch.ForeColor = Color.White
        btnLaunch.Location = New Point(118, 85)
        btnLaunch.Name = "btnLaunch"
        btnLaunch.Size = New Size(95, 30)
        btnLaunch.TabIndex = 5
        btnLaunch.Text = "▶ Launch"
        btnLaunch.UseVisualStyleBackColor = False
        '
        ' toolTip
        '
        toolTip.AutoPopDelay = 5000
        toolTip.InitialDelay = 400
        toolTip.ReshowDelay = 200
        '
        ' AppCard
        '
        AutoScaleMode = AutoScaleMode.None
        BorderStyle = BorderStyle.FixedSingle
        Controls.Add(picIcon)
        Controls.Add(lblName)
        Controls.Add(lblSubGroup)
        Controls.Add(lblDate)
        Controls.Add(lblDesc)
        Controls.Add(btnLaunch)
        Controls.Add(btnSettings)
        Margin = New Padding(8)
        Name = "AppCard"
        Size = New Size(225, 125)
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
    Friend WithEvents toolTip As ToolTip
End Class
