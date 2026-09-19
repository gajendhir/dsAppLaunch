<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
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
        topPanel = New Panel()
        btnReload = New Button()
        btnSettings = New Button()
        lblHeaderTitle = New Label()
        bottomPanel = New Panel()
        lblSupportFooter = New LinkLabel()
        flowLayout = New FlowLayoutPanel()
        toolTip = New ToolTip(components)
        topPanel.SuspendLayout()
        bottomPanel.SuspendLayout()
        SuspendLayout()
        '
        ' topPanel
        '
        topPanel.BackColor = Color.FromArgb(CByte(28), CByte(40), CByte(51))
        topPanel.Controls.Add(btnReload)
        topPanel.Controls.Add(btnSettings)
        topPanel.Controls.Add(lblHeaderTitle)
        topPanel.Dock = DockStyle.Top
        topPanel.Location = New Point(0, 0)
        topPanel.Name = "topPanel"
        topPanel.Padding = New Padding(10)
        topPanel.Size = New Size(934, 75)
        topPanel.TabIndex = 1
        '
        ' btnReload
        '
        btnReload.Anchor = CType(AnchorStyles.Top Or AnchorStyles.Right, AnchorStyles)
        btnReload.BackColor = Color.Transparent
        btnReload.Cursor = Cursors.Hand
        btnReload.FlatStyle = FlatStyle.Flat
        btnReload.Font = New Font("Segoe UI", 11.25F)
        btnReload.ForeColor = Color.White
        btnReload.Location = New Point(896, 37)
        btnReload.Name = "btnReload"
        btnReload.Size = New Size(30, 30)
        btnReload.TabIndex = 0
        btnReload.Text = "🔄"
        toolTip.SetToolTip(btnReload, "Reload application list")
        btnReload.UseVisualStyleBackColor = False
        '
        ' btnSettings
        '
        btnSettings.Anchor = CType(AnchorStyles.Top Or AnchorStyles.Right, AnchorStyles)
        btnSettings.BackColor = Color.Transparent
        btnSettings.Cursor = Cursors.Hand
        btnSettings.FlatStyle = FlatStyle.Flat
        btnSettings.Font = New Font("Segoe UI", 11.25F)
        btnSettings.ForeColor = Color.White
        btnSettings.Location = New Point(858, 37)
        btnSettings.Name = "btnSettings"
        btnSettings.Size = New Size(30, 30)
        btnSettings.TabIndex = 1
        btnSettings.Text = "⚙️"
        toolTip.SetToolTip(btnSettings, "Open settings to manage applications")
        btnSettings.UseVisualStyleBackColor = False
        '
        ' lblHeaderTitle
        '
        lblHeaderTitle.BackColor = Color.Transparent
        lblHeaderTitle.Dock = DockStyle.Top
        lblHeaderTitle.Font = New Font("Segoe UI", 13.5F, FontStyle.Bold)
        lblHeaderTitle.ForeColor = Color.White
        lblHeaderTitle.Location = New Point(10, 10)
        lblHeaderTitle.Name = "lblHeaderTitle"
        lblHeaderTitle.Size = New Size(914, 30)
        lblHeaderTitle.TabIndex = 2
        lblHeaderTitle.Text = "Data Spec Application Launcher"
        lblHeaderTitle.TextAlign = ContentAlignment.MiddleCenter
        '
        ' bottomPanel
        '
        bottomPanel.BackColor = Color.FromArgb(CByte(235), CByte(238), CByte(242))
        bottomPanel.Controls.Add(lblSupportFooter)
        bottomPanel.Dock = DockStyle.Bottom
        bottomPanel.Location = New Point(0, 631)
        bottomPanel.Name = "bottomPanel"
        bottomPanel.Padding = New Padding(0, 0, 20, 0)
        bottomPanel.Size = New Size(934, 30)
        bottomPanel.TabIndex = 2
        '
        ' lblSupportFooter
        '
        lblSupportFooter.ActiveLinkColor = Color.Red
        lblSupportFooter.Dock = DockStyle.Fill
        lblSupportFooter.LinkBehavior = LinkBehavior.HoverUnderline
        lblSupportFooter.LinkColor = Color.Red
        lblSupportFooter.Location = New Point(0, 0)
        lblSupportFooter.Name = "lblSupportFooter"
        lblSupportFooter.Size = New Size(914, 30)
        lblSupportFooter.TabIndex = 0
        lblSupportFooter.TabStop = True
        lblSupportFooter.Text = "S/W Support By Data Spec"
        lblSupportFooter.TextAlign = ContentAlignment.MiddleRight
        toolTip.SetToolTip(lblSupportFooter, "Visit https://www.dataspec.info")
        lblSupportFooter.VisitedLinkColor = Color.Red
        '
        ' flowLayout
        '
        flowLayout.AutoScroll = True
        flowLayout.Dock = DockStyle.Fill
        flowLayout.Location = New Point(0, 75)
        flowLayout.Name = "flowLayout"
        flowLayout.Padding = New Padding(15, 10, 15, 15)
        flowLayout.Size = New Size(934, 556)
        flowLayout.TabIndex = 0
        '
        ' toolTip
        '
        toolTip.AutoPopDelay = 5000
        toolTip.InitialDelay = 400
        toolTip.ReshowDelay = 200
        '
        ' frmMain
        '
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(245), CByte(246), CByte(250))
        ClientSize = New Size(934, 661)
        Controls.Add(flowLayout)
        Controls.Add(topPanel)
        Controls.Add(bottomPanel)
        Name = "frmMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Data Spec Application Launcher"
        topPanel.ResumeLayout(False)
        bottomPanel.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents topPanel As Panel
    Friend WithEvents btnReload As Button
    Friend WithEvents btnSettings As Button
    Friend WithEvents lblHeaderTitle As Label
    Friend WithEvents bottomPanel As Panel
    Friend WithEvents lblSupportFooter As LinkLabel
    Friend WithEvents flowLayout As FlowLayoutPanel
    Friend WithEvents toolTip As ToolTip
End Class
