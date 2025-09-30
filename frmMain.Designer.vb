Imports AutoBalancer
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.tsContainer = New System.Windows.Forms.ToolStrip()
        Me.tsProcess = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.tsClose = New System.Windows.Forms.ToolStripButton()
        Me.textboxSearch = New System.Windows.Forms.TextBox()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.ProgressBarX1 = New CustomProgressBarX()
        Me.labelConnected = New System.Windows.Forms.Label()
        Me.labelStatus = New System.Windows.Forms.Label()
        Me.btnViewLogs = New System.Windows.Forms.Button()
        Me.btn = New System.Windows.Forms.Button()
        Me.tsContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'tsContainer
        '
        Me.tsContainer.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.tsContainer.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsProcess, Me.ToolStripSeparator1, Me.tsClose})
        Me.tsContainer.Location = New System.Drawing.Point(0, 167)
        Me.tsContainer.Name = "tsContainer"
        Me.tsContainer.Size = New System.Drawing.Size(290, 25)
        Me.tsContainer.TabIndex = 0
        Me.tsContainer.Text = "tsContainer"
        '
        'tsProcess
        '
        Me.tsProcess.Image = CType(resources.GetObject("tsProcess.Image"), System.Drawing.Image)
        Me.tsProcess.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsProcess.Name = "tsProcess"
        Me.tsProcess.Size = New System.Drawing.Size(121, 22)
        Me.tsProcess.Text = "Run Auto Balance"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'tsClose
        '
        Me.tsClose.Image = CType(resources.GetObject("tsClose.Image"), System.Drawing.Image)
        Me.tsClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsClose.Name = "tsClose"
        Me.tsClose.Size = New System.Drawing.Size(56, 22)
        Me.tsClose.Text = "Close"
        '
        'textboxSearch
        '
        Me.textboxSearch.Location = New System.Drawing.Point(12, 24)
        Me.textboxSearch.Name = "textboxSearch"
        Me.textboxSearch.ReadOnly = True
        Me.textboxSearch.Size = New System.Drawing.Size(231, 20)
        Me.textboxSearch.TabIndex = 1
        '
        'btnSearch
        '
        Me.btnSearch.AccessibleRole = System.Windows.Forms.AccessibleRole.ToolTip
        Me.btnSearch.BackColor = System.Drawing.SystemColors.Window
        Me.btnSearch.ForeColor = System.Drawing.SystemColors.InfoText
        Me.btnSearch.Location = New System.Drawing.Point(249, 22)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(30, 23)
        Me.btnSearch.TabIndex = 2
        Me.btnSearch.Text = "...."
        Me.btnSearch.UseVisualStyleBackColor = False
        '
        'ProgressBarX1
        '
        '
        '
        '
        Me.ProgressBarX1.BackgroundStyle.Class = ""
        Me.ProgressBarX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ProgressBarX1.Location = New System.Drawing.Point(11, 82)
        Me.ProgressBarX1.Name = "ProgressBarX1"
        Me.ProgressBarX1.Size = New System.Drawing.Size(267, 23)
        Me.ProgressBarX1.TabIndex = 3
        '
        'labelConnected
        '
        Me.labelConnected.AutoSize = True
        Me.labelConnected.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelConnected.Location = New System.Drawing.Point(69, 56)
        Me.labelConnected.Name = "labelConnected"
        Me.labelConnected.Size = New System.Drawing.Size(108, 18)
        Me.labelConnected.TabIndex = 5
        Me.labelConnected.Text = "Not Connected"
        '
        'labelStatus
        '
        Me.labelStatus.AutoSize = True
        Me.labelStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 11.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.labelStatus.Location = New System.Drawing.Point(12, 56)
        Me.labelStatus.Name = "labelStatus"
        Me.labelStatus.Size = New System.Drawing.Size(61, 18)
        Me.labelStatus.TabIndex = 4
        Me.labelStatus.Text = "Status:"
        '
        'btnViewLogs
        '
        Me.btnViewLogs.Location = New System.Drawing.Point(144, 124)
        Me.btnViewLogs.Name = "btnViewLogs"
        Me.btnViewLogs.Size = New System.Drawing.Size(134, 23)
        Me.btnViewLogs.TabIndex = 11
        Me.btnViewLogs.Text = "Open Backup Folder"
        '
        'btn
        '
        Me.btn.Location = New System.Drawing.Point(11, 124)
        Me.btn.Name = "btn"
        Me.btn.Size = New System.Drawing.Size(126, 23)
        Me.btn.TabIndex = 10
        Me.btn.Text = "View Logs"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(290, 192)
        Me.Controls.Add(Me.btn)
        Me.Controls.Add(Me.btnViewLogs)
        Me.Controls.Add(Me.labelConnected)
        Me.Controls.Add(Me.labelStatus)
        Me.Controls.Add(Me.ProgressBarX1)
        Me.Controls.Add(Me.btnSearch)
        Me.Controls.Add(Me.textboxSearch)
        Me.Controls.Add(Me.tsContainer)
        Me.Name = "frmMain"
        Me.Text = "Auto Balancer"
        Me.tsContainer.ResumeLayout(False)
        Me.tsContainer.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tsContainer As ToolStrip
    Friend WithEvents tsProcess As ToolStripButton
    Friend WithEvents tsClose As ToolStripButton
    Friend WithEvents textboxSearch As TextBox
    Friend WithEvents btnSearch As Button
    Friend WithEvents ProgressBarX1 As CustomProgressBarX
    Friend WithEvents labelConnected As Label
    Friend WithEvents labelStatus As Label
    Friend WithEvents btnViewLogs As Button
    Friend WithEvents btn As Button
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
End Class
