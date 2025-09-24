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
        Me.tsClose = New System.Windows.Forms.ToolStripButton()
        Me.tsContainer.SuspendLayout()
        Me.SuspendLayout()
        '
        'tsContainer
        '
        Me.tsContainer.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.tsContainer.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsProcess, Me.tsClose})
        Me.tsContainer.Location = New System.Drawing.Point(0, 425)
        Me.tsContainer.Name = "tsContainer"
        Me.tsContainer.Size = New System.Drawing.Size(800, 25)
        Me.tsContainer.TabIndex = 0
        '
        'tsProcess
        '
        Me.tsProcess.Image = CType(resources.GetObject("tsProcess.Image"), System.Drawing.Image)
        Me.tsProcess.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsProcess.Name = "tsProcess"
        Me.tsProcess.Size = New System.Drawing.Size(67, 22)
        Me.tsProcess.Text = "Process"
        '
        'tsClose
        '
        Me.tsClose.Image = CType(resources.GetObject("tsClose.Image"), System.Drawing.Image)
        Me.tsClose.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.tsClose.Name = "tsClose"
        Me.tsClose.Size = New System.Drawing.Size(56, 22)
        Me.tsClose.Text = "Close"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.tsContainer)
        Me.Name = "frmMain"
        Me.Text = "Auto Balancer"
        Me.UseWaitCursor = True
        Me.tsContainer.ResumeLayout(False)
        Me.tsContainer.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents tsContainer As ToolStrip
    Friend WithEvents tsProcess As ToolStripButton
    Friend WithEvents tsClose As ToolStripButton
End Class
