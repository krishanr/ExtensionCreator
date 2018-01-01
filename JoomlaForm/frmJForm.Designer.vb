<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmJForm
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
        Me.purchaseLink = New System.Windows.Forms.LinkLabel()
        Me.JFormDataSet = New JoomlaForm.JFormDataSet()
        CType(Me.JFormDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'purchaseLink
        '
        Me.purchaseLink.AutoSize = True
        Me.purchaseLink.Location = New System.Drawing.Point(12, 9)
        Me.purchaseLink.Name = "purchaseLink"
        Me.purchaseLink.Size = New System.Drawing.Size(328, 13)
        Me.purchaseLink.TabIndex = 0
        Me.purchaseLink.TabStop = True
        Me.purchaseLink.Text = "To edit Joomla xml forms please click this link to purchase the editor."
        '
        'JFormDataSet
        '
        Me.JFormDataSet.DataSetName = "JFormDataSet"
        Me.JFormDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'frmJForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(349, 37)
        Me.Controls.Add(Me.purchaseLink)
        Me.Name = "frmJForm"
        Me.Text = "Joomla Form"
        CType(Me.JFormDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents JFormDataSet As JFormDataSet
    Protected Friend WithEvents purchaseLink As Windows.Forms.LinkLabel
End Class
