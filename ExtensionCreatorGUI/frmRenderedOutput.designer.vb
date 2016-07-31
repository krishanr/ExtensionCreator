<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmRenderedOutput
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
        Me.LabelNotice = New System.Windows.Forms.Label()
        Me.TextBoxRendered = New System.Windows.Forms.TextBox()
        Me.ButtonRetry = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.LabelRetry = New System.Windows.Forms.Label()
        Me.ButtonCancel = New System.Windows.Forms.Button()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'LabelNotice
        '
        Me.LabelNotice.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.LabelNotice, 3)
        Me.LabelNotice.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelNotice.ForeColor = System.Drawing.Color.Black
        Me.LabelNotice.Location = New System.Drawing.Point(3, 0)
        Me.LabelNotice.Name = "LabelNotice"
        Me.LabelNotice.Size = New System.Drawing.Size(63, 20)
        Me.LabelNotice.TabIndex = 0
        Me.LabelNotice.Text = "Label1"
        '
        'TextBoxRendered
        '
        Me.TableLayoutPanel1.SetColumnSpan(Me.TextBoxRendered, 3)
        Me.TextBoxRendered.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TextBoxRendered.Location = New System.Drawing.Point(3, 23)
        Me.TextBoxRendered.Multiline = True
        Me.TextBoxRendered.Name = "TextBoxRendered"
        Me.TextBoxRendered.ReadOnly = True
        Me.TextBoxRendered.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBoxRendered.Size = New System.Drawing.Size(656, 400)
        Me.TextBoxRendered.TabIndex = 1
        Me.TextBoxRendered.WordWrap = False
        '
        'ButtonRetry
        '
        Me.ButtonRetry.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.ButtonRetry.DialogResult = System.Windows.Forms.DialogResult.Retry
        Me.ButtonRetry.Location = New System.Drawing.Point(292, 429)
        Me.ButtonRetry.Name = "ButtonRetry"
        Me.ButtonRetry.Size = New System.Drawing.Size(75, 23)
        Me.ButtonRetry.TabIndex = 3
        Me.ButtonRetry.Text = "Retry"
        Me.ButtonRetry.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.03571!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.48214!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334!))
        Me.TableLayoutPanel1.Controls.Add(Me.LabelNotice, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBoxRendered, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonRetry, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.LabelRetry, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.ButtonCancel, 2, 2)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(662, 456)
        Me.TableLayoutPanel1.TabIndex = 5
        '
        'LabelRetry
        '
        Me.LabelRetry.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.LabelRetry.AutoSize = True
        Me.LabelRetry.Location = New System.Drawing.Point(90, 434)
        Me.LabelRetry.Name = "LabelRetry"
        Me.LabelRetry.Size = New System.Drawing.Size(39, 13)
        Me.LabelRetry.TabIndex = 2
        Me.LabelRetry.Text = "Label1"
        '
        'ButtonCancel
        '
        Me.ButtonCancel.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButtonCancel.Location = New System.Drawing.Point(513, 429)
        Me.ButtonCancel.Name = "ButtonCancel"
        Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButtonCancel.TabIndex = 4
        Me.ButtonCancel.Text = "Cancel"
        Me.ButtonCancel.UseVisualStyleBackColor = True
        '
        'frmRenderedOutput
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(662, 456)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Name = "frmRenderedOutput"
        Me.Text = "T4 Template"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ButtonRetry As System.Windows.Forms.Button
    Public WithEvents LabelNotice As System.Windows.Forms.Label
    Public WithEvents TextBoxRendered As System.Windows.Forms.TextBox
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents LabelRetry As System.Windows.Forms.Label
    Friend WithEvents ButtonCancel As System.Windows.Forms.Button

End Class
