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
        Me.components = New System.ComponentModel.Container()
        Me.CustomTypeBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.JFormDataSet = New JoomlaLibrary.JFormDataSet()
        Me.TypeBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.FieldsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.FormFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.SaveFormFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.purchaseLink = New System.Windows.Forms.LinkLabel()
        CType(Me.CustomTypeBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.JFormDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.TypeBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FieldsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CustomTypeBindingSource
        '
        Me.CustomTypeBindingSource.DataMember = "CustomType"
        Me.CustomTypeBindingSource.DataSource = Me.JFormDataSet
        '
        'JFormDataSet
        '
        Me.JFormDataSet.DataSetName = "JFormDataSet"
        Me.JFormDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'TypeBindingSource
        '
        Me.TypeBindingSource.DataMember = "type"
        Me.TypeBindingSource.DataSource = Me.JFormDataSet
        '
        'FormFileDialog
        '
        Me.FormFileDialog.FileName = "OpenFormFileDialog"
        Me.FormFileDialog.Filter = "Xml files (*.xml)|*.xml"
        Me.FormFileDialog.RestoreDirectory = True
        Me.FormFileDialog.Title = "Please Choose the Xml File you would like to load the form from."
        '
        'SaveFormFileDialog
        '
        Me.SaveFormFileDialog.FileName = "SaveFormFileDialog"
        Me.SaveFormFileDialog.Filter = "Xml files (*.xml)|*.xml"
        Me.SaveFormFileDialog.RestoreDirectory = True
        Me.SaveFormFileDialog.Title = "Please Choose the Xml File you would like to save the form to."
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
        'frmJForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(349, 37)
        Me.Controls.Add(Me.purchaseLink)
        Me.Name = "frmJForm"
        Me.Text = "Joomla Form"
        CType(Me.CustomTypeBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.JFormDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.TypeBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FieldsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents FieldsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents JFormDataSet As JoomlaLibrary.JFormDataSet
    Friend WithEvents TypeBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents SaveFormFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents CustomTypeBindingSource As System.Windows.Forms.BindingSource
    Public WithEvents FormFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents purchaseLink As System.Windows.Forms.LinkLabel
End Class
