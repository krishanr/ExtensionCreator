<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTaskProgress
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TaskProgressBar = New System.Windows.Forms.ProgressBar()
        Me.NotifyLabel = New System.Windows.Forms.Label()
        Me.TaskListBox = New System.Windows.Forms.ListBox()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TaskProgressBar
        '
        Me.TaskProgressBar.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TaskProgressBar.Location = New System.Drawing.Point(50, 25)
        Me.TaskProgressBar.Name = "TaskProgressBar"
        Me.TaskProgressBar.Size = New System.Drawing.Size(155, 23)
        Me.TaskProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.TaskProgressBar.TabIndex = 0
        '
        'NotifyLabel
        '
        Me.NotifyLabel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.NotifyLabel.AutoSize = True
        Me.NotifyLabel.Location = New System.Drawing.Point(47, 9)
        Me.NotifyLabel.Name = "NotifyLabel"
        Me.NotifyLabel.Size = New System.Drawing.Size(39, 13)
        Me.NotifyLabel.TabIndex = 1
        Me.NotifyLabel.Text = "Label1"
        '
        'TaskListBox
        '
        Me.TaskListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TaskListBox.FormattingEnabled = True
        Me.TaskListBox.Location = New System.Drawing.Point(0, 0)
        Me.TaskListBox.Name = "TaskListBox"
        Me.TaskListBox.Size = New System.Drawing.Size(237, 174)
        Me.TaskListBox.TabIndex = 2
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.NotifyLabel)
        Me.SplitContainer1.Panel1.Controls.Add(Me.TaskProgressBar)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TaskListBox)
        Me.SplitContainer1.Size = New System.Drawing.Size(237, 242)
        Me.SplitContainer1.SplitterDistance = 64
        Me.SplitContainer1.TabIndex = 3
        '
        'frmTaskProgress
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(237, 242)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "frmTaskProgress"
        Me.Text = "Task Progress"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.PerformLayout()
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TaskProgressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents NotifyLabel As System.Windows.Forms.Label
    Friend WithEvents TaskListBox As System.Windows.Forms.ListBox
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
End Class
