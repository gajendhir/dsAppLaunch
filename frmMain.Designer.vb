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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        topPanel = New Panel()
        picLogo = New PictureBox()
        btnReload = New Button()
        btnSettings = New Button()
        lblHeaderSubtitle = New Label()
        lblHeaderTitle = New Label()
        toolbarPanel = New Panel()
        btnSelectAll = New Button()
        btnSelectNone = New Button()
        btnLaunchSelected = New Button()
        bottomPanel = New Panel()
        picCompanyLogo = New PictureBox()
        lblSupportFooter = New LinkLabel()
        flowLayout = New FlowLayoutPanel()
        toolTip = New ToolTip(components)
        topPanel.SuspendLayout()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        toolbarPanel.SuspendLayout()
        bottomPanel.SuspendLayout()
        CType(picCompanyLogo, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' topPanel
        ' 
        topPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        topPanel.BackColor = Color.White
        topPanel.Controls.Add(picLogo)
        topPanel.Controls.Add(btnReload)
        topPanel.Controls.Add(btnSettings)
        topPanel.Controls.Add(lblHeaderSubtitle)
        topPanel.Controls.Add(lblHeaderTitle)
        topPanel.Location = New Point(0, 0)
        topPanel.Name = "topPanel"
        topPanel.Size = New Size(934, 56)
        topPanel.TabIndex = 1
        ' 
        ' picLogo
        ' 
        picLogo.BackColor = Color.Transparent
        picLogo.Image = CType(resources.GetObject("picLogo.Image"), Image)
        picLogo.Location = New Point(0, 0)
        picLogo.Name = "picLogo"
        picLogo.Size = New Size(56, 56)
        picLogo.SizeMode = PictureBoxSizeMode.Zoom
        picLogo.TabIndex = 4
        picLogo.TabStop = False
        ' 
        ' btnReload
        ' 
        btnReload.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnReload.Font = New Font("Segoe MDL2 Assets", 12.0F)
        btnReload.Location = New Point(843, 8)
        btnReload.Name = "btnReload"
        btnReload.Size = New Size(38, 38)
        btnReload.TabIndex = 0
        toolTip.SetToolTip(btnReload, "Reload application list")
        btnReload.UseVisualStyleBackColor = False
        ' 
        ' btnSettings
        ' 
        btnSettings.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        btnSettings.Font = New Font("Segoe MDL2 Assets", 12.0F)
        btnSettings.Location = New Point(887, 8)
        btnSettings.Name = "btnSettings"
        btnSettings.Size = New Size(38, 38)
        btnSettings.TabIndex = 1
        toolTip.SetToolTip(btnSettings, "Open settings to manage applications")
        btnSettings.UseVisualStyleBackColor = False
        ' 
        ' lblHeaderSubtitle
        ' 
        lblHeaderSubtitle.AutoEllipsis = True
        lblHeaderSubtitle.BackColor = Color.Transparent
        lblHeaderSubtitle.Font = New Font("Segoe UI", 9.0F)
        lblHeaderSubtitle.ForeColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
        lblHeaderSubtitle.Location = New Point(58, 34)
        lblHeaderSubtitle.Name = "lblHeaderSubtitle"
        lblHeaderSubtitle.Size = New Size(500, 20)
        lblHeaderSubtitle.TabIndex = 3
        lblHeaderSubtitle.Text = "Launch your applications from one place"
        ' 
        ' lblHeaderTitle
        ' 
        lblHeaderTitle.AutoEllipsis = True
        lblHeaderTitle.BackColor = Color.Transparent
        lblHeaderTitle.Font = New Font("Segoe UI Semibold", 14.25F)
        lblHeaderTitle.ForeColor = Color.FromArgb(CByte(31), CByte(41), CByte(55))
        lblHeaderTitle.Location = New Point(57, 5)
        lblHeaderTitle.Name = "lblHeaderTitle"
        lblHeaderTitle.Size = New Size(500, 30)
        lblHeaderTitle.TabIndex = 2
        lblHeaderTitle.Text = "Data Spec Application Launcher"
        ' 
        ' toolbarPanel
        ' 
        toolbarPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        toolbarPanel.BackColor = Color.FromArgb(CByte(246), CByte(247), CByte(249))
        toolbarPanel.Controls.Add(btnSelectAll)
        toolbarPanel.Controls.Add(btnSelectNone)
        toolbarPanel.Controls.Add(btnLaunchSelected)
        toolbarPanel.Location = New Point(0, 56)
        toolbarPanel.Name = "toolbarPanel"
        toolbarPanel.Size = New Size(934, 43)
        toolbarPanel.TabIndex = 3
        ' 
        ' btnSelectAll
        ' 
        btnSelectAll.Location = New Point(205, 6)
        btnSelectAll.Name = "btnSelectAll"
        btnSelectAll.Size = New Size(56, 32)
        btnSelectAll.TabIndex = 3
        btnSelectAll.Text = "All"
        toolTip.SetToolTip(btnSelectAll, "Tick every application for Launch Selected")
        btnSelectAll.UseVisualStyleBackColor = False
        ' 
        ' btnSelectNone
        ' 
        btnSelectNone.Location = New Point(267, 6)
        btnSelectNone.Name = "btnSelectNone"
        btnSelectNone.Size = New Size(64, 32)
        btnSelectNone.TabIndex = 4
        btnSelectNone.Text = "None"
        toolTip.SetToolTip(btnSelectNone, "Untick every application")
        btnSelectNone.UseVisualStyleBackColor = False
        ' 
        ' btnLaunchSelected
        ' 
        btnLaunchSelected.Enabled = False
        btnLaunchSelected.Font = New Font("Segoe UI Semibold", 9.0F)
        btnLaunchSelected.Location = New Point(7, 6)
        btnLaunchSelected.Name = "btnLaunchSelected"
        btnLaunchSelected.Size = New Size(190, 32)
        btnLaunchSelected.TabIndex = 5
        btnLaunchSelected.Text = "▶ Launch Selected (0)"
        toolTip.SetToolTip(btnLaunchSelected, "Launch all ticked applications, one after another, in on-screen order")
        btnLaunchSelected.UseVisualStyleBackColor = False
        ' 
        ' bottomPanel
        ' 
        bottomPanel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        bottomPanel.BackColor = Color.FromArgb(CByte(246), CByte(247), CByte(249))
        bottomPanel.Controls.Add(picCompanyLogo)
        bottomPanel.Controls.Add(lblSupportFooter)
        bottomPanel.Location = New Point(0, 629)
        bottomPanel.Name = "bottomPanel"
        bottomPanel.Size = New Size(934, 32)
        bottomPanel.TabIndex = 2
        ' 
        ' picCompanyLogo
        ' 
        picCompanyLogo.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        picCompanyLogo.BackColor = Color.Transparent
        picCompanyLogo.Image = CType(resources.GetObject("picCompanyLogo.Image"), Image)
        picCompanyLogo.Location = New Point(905, 4)
        picCompanyLogo.Name = "picCompanyLogo"
        picCompanyLogo.Size = New Size(24, 24)
        picCompanyLogo.SizeMode = PictureBoxSizeMode.Zoom
        picCompanyLogo.TabIndex = 1
        picCompanyLogo.TabStop = False
        ' 
        ' lblSupportFooter
        ' 
        lblSupportFooter.ActiveLinkColor = Color.FromArgb(CByte(154), CByte(38), CByte(0))
        lblSupportFooter.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        lblSupportFooter.Font = New Font("Segoe UI", 8.25F)
        lblSupportFooter.LinkBehavior = LinkBehavior.HoverUnderline
        lblSupportFooter.LinkColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
        lblSupportFooter.Location = New Point(701, 0)
        lblSupportFooter.Name = "lblSupportFooter"
        lblSupportFooter.Size = New Size(200, 32)
        lblSupportFooter.TabIndex = 0
        lblSupportFooter.TabStop = True
        lblSupportFooter.Text = "S/W Support By Data Spec"
        lblSupportFooter.TextAlign = ContentAlignment.MiddleRight
        toolTip.SetToolTip(lblSupportFooter, "Visit https://www.dataspec.info")
        lblSupportFooter.VisitedLinkColor = Color.FromArgb(CByte(107), CByte(114), CByte(128))
        ' 
        ' flowLayout
        ' 
        flowLayout.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        flowLayout.AutoScroll = True
        flowLayout.BackColor = Color.FromArgb(CByte(246), CByte(247), CByte(249))
        flowLayout.Location = New Point(0, 99)
        flowLayout.Name = "flowLayout"
        flowLayout.Padding = New Padding(16, 4, 16, 16)
        flowLayout.Size = New Size(934, 530)
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
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.FromArgb(CByte(246), CByte(247), CByte(249))
        ClientSize = New Size(934, 661)
        Controls.Add(flowLayout)
        Controls.Add(toolbarPanel)
        Controls.Add(topPanel)
        Controls.Add(bottomPanel)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MinimumSize = New Size(560, 400)
        Name = "frmMain"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Data Spec Application Launcher"
        topPanel.ResumeLayout(False)
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        toolbarPanel.ResumeLayout(False)
        bottomPanel.ResumeLayout(False)
        CType(picCompanyLogo, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents topPanel As Panel
    Friend WithEvents picLogo As PictureBox
    Friend WithEvents picCompanyLogo As PictureBox
    Friend WithEvents btnReload As Button
    Friend WithEvents btnSettings As Button
    Friend WithEvents lblHeaderSubtitle As Label
    Friend WithEvents lblHeaderTitle As Label
    Friend WithEvents toolbarPanel As Panel
    Friend WithEvents btnSelectAll As Button
    Friend WithEvents btnSelectNone As Button
    Friend WithEvents btnLaunchSelected As Button
    Friend WithEvents bottomPanel As Panel
    Friend WithEvents lblSupportFooter As LinkLabel
    Friend WithEvents flowLayout As FlowLayoutPanel
    Friend WithEvents toolTip As ToolTip
End Class
