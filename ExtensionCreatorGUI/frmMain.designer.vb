<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.OpenExtensionFileDialog = New System.Windows.Forms.OpenFileDialog()
        Me.ExtensionsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ExtensionFilesBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ExtensionsListBox = New System.Windows.Forms.ListBox()
        Me.FilterLabel = New System.Windows.Forms.Label()
        Me.FilterComboBox = New System.Windows.Forms.ComboBox()
        Me.ExtensionManagerBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.FiltersBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.FilterMethodsBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.TemplatesMenuStrip = New System.Windows.Forms.MenuStrip()
        Me.TemplateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtensionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RunToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ResetToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.RunToolStripMenuItem2 = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FilterToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        CType(Me.ExtensionsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ExtensionFilesBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ExtensionManagerBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FiltersBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.FilterMethodsBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TemplatesMenuStrip.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SuspendLayout()
        '
        'OpenExtensionFileDialog
        '
        Me.OpenExtensionFileDialog.FileName = "Open the Xml File for the template you wish to add."
        Me.OpenExtensionFileDialog.Filter = "Xml files (*.xml)|*.xml"
        Me.OpenExtensionFileDialog.InitialDirectory = Global.ExtensionCreatorGUI.My.MySettings.Default.ExtensionFileDialogInitialDirectory
        Me.OpenExtensionFileDialog.Multiselect = True
        Me.OpenExtensionFileDialog.Title = "Open the Xml File for the template you wish to add."
        '
        'ExtensionFilesBindingSource
        '
        Me.ExtensionFilesBindingSource.DataMember = "ExtensionsView"
        Me.ExtensionFilesBindingSource.DataSource = GetType(ExtensionCreator.ExtensionManager)
        '
        'ExtensionsListBox
        '
        Me.ExtensionsListBox.DataSource = Me.ExtensionFilesBindingSource
        Me.ExtensionsListBox.DisplayMember = "Label"
        Me.ExtensionsListBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ExtensionsListBox.FormattingEnabled = True
        Me.ExtensionsListBox.Location = New System.Drawing.Point(0, 0)
        Me.ExtensionsListBox.Name = "ExtensionsListBox"
        Me.ExtensionsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.ExtensionsListBox.Size = New System.Drawing.Size(311, 249)
        Me.ExtensionsListBox.Sorted = True
        Me.ExtensionsListBox.TabIndex = 1
        Me.ExtensionsListBox.ValueMember = "File"
        '
        'FilterLabel
        '
        Me.FilterLabel.AutoSize = True
        Me.FilterLabel.Location = New System.Drawing.Point(12, 3)
        Me.FilterLabel.Name = "FilterLabel"
        Me.FilterLabel.Size = New System.Drawing.Size(63, 13)
        Me.FilterLabel.TabIndex = 4
        Me.FilterLabel.Text = "Xpath Filter:"
        '
        'FilterComboBox
        '
        Me.FilterComboBox.DataBindings.Add(New System.Windows.Forms.Binding("SelectedItem", Me.ExtensionManagerBindingSource, "CurrentView", True))
        Me.FilterComboBox.DataSource = Me.FiltersBindingSource
        Me.FilterComboBox.Dock = System.Windows.Forms.DockStyle.Fill
        Me.FilterComboBox.FormattingEnabled = True
        Me.FilterComboBox.Location = New System.Drawing.Point(0, 0)
        Me.FilterComboBox.Name = "FilterComboBox"
        Me.FilterComboBox.Size = New System.Drawing.Size(222, 21)
        Me.FilterComboBox.TabIndex = 5
        '
        'ExtensionManagerBindingSource
        '
        Me.ExtensionManagerBindingSource.DataSource = GetType(ExtensionCreator.ExtensionManager)
        '
        'TemplatesMenuStrip
        '
        Me.TemplatesMenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TemplateToolStripMenuItem, Me.ExtensionToolStripMenuItem, Me.RunToolStripMenuItem2, Me.FilterToolStripMenuItem})
        Me.TemplatesMenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.TemplatesMenuStrip.Name = "TemplatesMenuStrip"
        Me.TemplatesMenuStrip.Size = New System.Drawing.Size(311, 24)
        Me.TemplatesMenuStrip.TabIndex = 12
        Me.TemplatesMenuStrip.Text = "Templates Menu Strip"
        '
        'TemplateToolStripMenuItem
        '
        Me.TemplateToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddToolStripMenuItem, Me.RemoveToolStripMenuItem1, Me.OpenToolStripMenuItem, Me.ToolStripSeparator2, Me.ExitToolStripMenuItem})
        Me.TemplateToolStripMenuItem.Name = "TemplateToolStripMenuItem"
        Me.TemplateToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.TemplateToolStripMenuItem.Text = "File"
        '
        'AddToolStripMenuItem
        '
        Me.AddToolStripMenuItem.Name = "AddToolStripMenuItem"
        Me.AddToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.AddToolStripMenuItem.Text = "Add"
        '
        'RemoveToolStripMenuItem1
        '
        Me.RemoveToolStripMenuItem1.Name = "RemoveToolStripMenuItem1"
        Me.RemoveToolStripMenuItem1.Size = New System.Drawing.Size(117, 22)
        Me.RemoveToolStripMenuItem1.Text = "Remove"
        '
        'OpenToolStripMenuItem
        '
        Me.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem"
        Me.OpenToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.OpenToolStripMenuItem.Text = "Open"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(114, 6)
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'ExtensionToolStripMenuItem
        '
        Me.ExtensionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RunToolStripMenuItem, Me.ExportToolStripMenuItem, Me.ResetToolStripMenuItem1})
        Me.ExtensionToolStripMenuItem.Name = "ExtensionToolStripMenuItem"
        Me.ExtensionToolStripMenuItem.Size = New System.Drawing.Size(69, 20)
        Me.ExtensionToolStripMenuItem.Text = "Extension"
        '
        'RunToolStripMenuItem
        '
        Me.RunToolStripMenuItem.Name = "RunToolStripMenuItem"
        Me.RunToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.RunToolStripMenuItem.Text = "Run"
        '
        'ExportToolStripMenuItem
        '
        Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
        Me.ExportToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.ExportToolStripMenuItem.Text = "Export"
        '
        'ResetToolStripMenuItem1
        '
        Me.ResetToolStripMenuItem1.Name = "ResetToolStripMenuItem1"
        Me.ResetToolStripMenuItem1.Size = New System.Drawing.Size(107, 22)
        Me.ResetToolStripMenuItem1.Text = "Reset"
        '
        'RunToolStripMenuItem2
        '
        Me.RunToolStripMenuItem2.Name = "RunToolStripMenuItem2"
        Me.RunToolStripMenuItem2.Size = New System.Drawing.Size(40, 20)
        Me.RunToolStripMenuItem2.Text = "Run"
        '
        'FilterToolStripMenuItem
        '
        Me.FilterToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FilterToolStripMenuItem1, Me.ResetToolStripMenuItem, Me.RemoveToolStripMenuItem})
        Me.FilterToolStripMenuItem.Name = "FilterToolStripMenuItem"
        Me.FilterToolStripMenuItem.Size = New System.Drawing.Size(45, 20)
        Me.FilterToolStripMenuItem.Text = "Filter"
        '
        'FilterToolStripMenuItem1
        '
        Me.FilterToolStripMenuItem1.Name = "FilterToolStripMenuItem1"
        Me.FilterToolStripMenuItem1.Size = New System.Drawing.Size(117, 22)
        Me.FilterToolStripMenuItem1.Text = "Run"
        '
        'ResetToolStripMenuItem
        '
        Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
        Me.ResetToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.ResetToolStripMenuItem.Text = "Reset"
        '
        'RemoveToolStripMenuItem
        '
        Me.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem"
        Me.RemoveToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.RemoveToolStripMenuItem.Text = "Remove"
        '
        'BackgroundWorker1
        '
        Me.BackgroundWorker1.WorkerReportsProgress = True
        Me.BackgroundWorker1.WorkerSupportsCancellation = True
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.SplitContainer2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.ExtensionsListBox)
        Me.SplitContainer1.Size = New System.Drawing.Size(311, 278)
        Me.SplitContainer1.SplitterDistance = 25
        Me.SplitContainer1.TabIndex = 13
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.SplitContainer2.IsSplitterFixed = True
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.FilterLabel)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.FilterComboBox)
        Me.SplitContainer2.Size = New System.Drawing.Size(311, 25)
        Me.SplitContainer2.SplitterDistance = 85
        Me.SplitContainer2.TabIndex = 6
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(311, 302)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.TemplatesMenuStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.TemplatesMenuStrip
        Me.Name = "frmMain"
        Me.Text = "Extension Manager"
        CType(Me.ExtensionsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ExtensionFilesBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ExtensionManagerBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FiltersBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.FilterMethodsBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TemplatesMenuStrip.ResumeLayout(False)
        Me.TemplatesMenuStrip.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.PerformLayout()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenExtensionFileDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ExtensionsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents ExtensionsListBox As System.Windows.Forms.ListBox
    Friend WithEvents ExtensionFilesBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents FilterLabel As System.Windows.Forms.Label
    Friend WithEvents FilterComboBox As System.Windows.Forms.ComboBox
    Friend WithEvents FiltersBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents FilterMethodsBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents TemplatesMenuStrip As System.Windows.Forms.MenuStrip
    Friend WithEvents TemplateToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AddToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ExitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FilterToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FilterToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResetToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RemoveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RunToolStripMenuItem2 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExtensionManagerBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents OpenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BackgroundWorker1 As System.ComponentModel.BackgroundWorker
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
    Friend WithEvents ExtensionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents RunToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ResetToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ExportToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
