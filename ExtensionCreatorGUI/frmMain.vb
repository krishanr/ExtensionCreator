Imports ExtensionCreator
Imports System.IO
Imports System.Collections.Specialized
Imports System.Threading
Imports System.ComponentModel

Public Class frmMain

    Private _ExtensionMgr As New ExtensionManager
    Private progressForm As frmTaskProgress
    Private workBw As BackgroundWorker

    Public Property ExtensionMgr() As ExtensionManager
        Get
            Return _ExtensionMgr
        End Get
        Set(ByVal value As ExtensionManager)
            _ExtensionMgr = value
        End Set
    End Property

    'Get the ExtensionXmlFiles from My.Settings. Then add the extension xml file to the list ExtensionMgr
    'maintains, then set the datasource for ExtensionFilesBindingSource.
    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim CommandLineArgs As String() = Environment.GetCommandLineArgs
        If CommandLineArgs.Length = 2 AndAlso CommandLineArgs(1) = "-i" Then
            'Program is being installed. Load initial settings. 
            Install()
            For Each ExtensionXmlFile As String In My.Settings.ExtensionXmlFiles
                Dim AnExtensionItem As ExtensionItem = New ExtensionItem
                AnExtensionItem.File = ExtensionXmlFile
                ExtensionMgr.Extensions.Add(AnExtensionItem)
            Next
            'Lets exit after ExtensionMgr was initalized.
            'FormClosing event will save My.Settings
            Me.Close()
            Exit Sub
        End If

        For Each ExtensionXmlFile As String In My.Settings.ExtensionXmlFiles
            Try
                ExtensionMgr.Add(ExtensionXmlFile)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error loading template")
            End Try
        Next
        ProcessComandLineArgs()

        ExtensionFilesBindingSource.DataSource = ExtensionMgr
        FiltersBindingSource.DataSource = My.Settings.Filters
        ExtensionMgr.CurrentView = My.Settings.SelectedFilter
        ExtensionManagerBindingSource.DataSource = ExtensionMgr
        Try
            If My.Settings.ExtensionFileDialogInitialDirectory = "" Then
                OpenExtensionFileDialog.InitialDirectory = My.Application.Info.DirectoryPath
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error loading template")
        End Try
    End Sub

    'FILENAME -a -r -c
    '-a will add the file
    '-r will add it then run
    '-c closes the program at the end
    Private Sub ProcessComandLineArgs()
        Dim CommandLineArgs As String() = Environment.GetCommandLineArgs
        Dim FileName As String = ""
        Dim Add As Boolean = False
        Dim Run As Boolean = False
        Dim Close As Boolean = False
        Dim ErrorOccured As Boolean = False

        'Need the file and actions
        If CommandLineArgs.Length <= 2 Then
            Exit Sub
        End If

        For i = 1 To CommandLineArgs.Length - 1
            Select Case CommandLineArgs(i)
                Case "-a"
                    Add = True
                Case "-r"
                    'For run, add it if we don't have it already then run it.
                    Add = True
                    Run = True
                Case "-c"
                    Close = True
            End Select
        Next

        FileName = CommandLineArgs(1)
        If Not My.Computer.FileSystem.FileExists(FileName) Then
            MsgBox("The file provided does not exist. " & FileName)
            Add = False
            Run = False
        End If

        If Add AndAlso (Not ExtensionMgr.ExtensionXmlFiles.Contains(FileName)) Then
            Try
                AddExtension(FileName)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error loading template")
                ErrorOccured = True
            End Try
        End If

        If Run And (Not ErrorOccured) Then
            'TODO: optional. GetExtensionItem not the best way to do this...
            'TODO: test Backgroundworker with command line Run. RunExtension below uses backgroundworker...
            RunExtension(ExtensionMgr.GetExtensionItem(FileName))
        End If

        If Close Then
            Me.Close()
        End If
    End Sub

    'This code is run when the ExtensionCreatorGui.exe is first installed. The ExtensionXmlFile urls
    'are made absolute.
    Private Sub Install()
        Dim newFiles As Collections.Specialized.StringCollection = New Collections.Specialized.StringCollection()
        For Each ExtensionXmlFile As String In My.Settings.ExtensionXmlFiles
            If Not Path.IsPathRooted(ExtensionXmlFile) Then
                ExtensionXmlFile = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Extension Creator\" & ExtensionXmlFile 'My.Application.Info.DirectoryPath & "\" & ExtensionXmlFile
            End If
            newFiles.Add(ExtensionXmlFile)
        Next
        My.Settings.ExtensionFileDialogInitialDirectory = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Extension Creator\TemplateManifests"
        My.Settings.ExtensionXmlFiles = newFiles
    End Sub

    Private Sub AddExtension(ByVal FileName As String)
        'Save the directory of the file
        'TODO: maybe use restore directories...
        My.Settings.ExtensionFileDialogInitialDirectory = Path.GetDirectoryName(FileName)
        OpenExtensionFileDialog.InitialDirectory = Path.GetDirectoryName(FileName)
        'add the extension
        ExtensionMgr.Add(FileName)
    End Sub

    Private Sub RunExtension(ByVal ExtensionItem As ExtensionItem)

        'Check that the file still exists
        If Not My.Computer.FileSystem.FileExists(ExtensionItem.File) Then
            MsgBox("File doesn't exist: " & ExtensionItem.File, MsgBoxStyle.Exclamation, "Error running template")
            Exit Sub
        End If

        'TODO: create a new progress form for each extension or reuse one instance?
        progressForm = New frmTaskProgress()
        'Run the extension.
        progressForm.NotifyLabel.Text = "Running " & ExtensionItem.Label
        progressForm.Show()
        ' Refresh causes an instant (non-posted) display of the label.
        progressForm.Refresh()
        'Disable the menu strip untill this operation is complete.
        TemplatesMenuStrip.Enabled = False

        BackgroundWorker1.RunWorkerAsync(ExtensionItem)
    End Sub

    Private Sub ResetExtension(ByVal ExtensionItem As ExtensionItem)
        'Check that the file still exists
        If Not My.Computer.FileSystem.FileExists(ExtensionItem.File) Then
            MsgBox("File doesn't exist: " & ExtensionItem.File, MsgBoxStyle.Exclamation, "Error reseting template")
            Exit Sub
        End If

        Try
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
            ExtensionMgr.Reset(ExtensionItem)
            System.Windows.Forms.Cursor.Current = Cursors.Default
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error reseting Template")
        End Try
    End Sub

    Private Sub ExportExtension(ByVal ExtensionItem As ExtensionItem)
        'Check that the file still exists
        If Not My.Computer.FileSystem.FileExists(ExtensionItem.File) Then
            MsgBox("File doesn't exist: " & ExtensionItem.File, MsgBoxStyle.Exclamation, "Error exporting template")
            Exit Sub
        End If

        Try
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor
            ExtensionMgr.Export(ExtensionItem)
            System.Windows.Forms.Cursor.Current = Cursors.Default
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error exporting Template")
        End Try
    End Sub

    Private Function FilterExtension(ByVal Xpath As String) As Boolean
        Try
            ExtensionMgr.Filter(Xpath)
            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error Filtering Templates")
            Return False
        End Try
    End Function

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Try
            My.Settings.ExtensionXmlFiles = ExtensionMgr.ExtensionXmlFiles
            My.Settings.SelectedFilter = ExtensionMgr.CurrentView
            My.Settings.Save()
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error saving settings")
        End Try
    End Sub

    Private Sub BindingNavigatorAddNewItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            If OpenExtensionFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
                AddExtension(OpenExtensionFileDialog.FileName)
                ExtensionFilesBindingSource.ResetBindings(False)
            End If
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error adding new template")
        End Try
    End Sub

    Private Sub BindingNavigatorDeleteItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Try
            ExtensionMgr.Remove(ExtensionsListBox.SelectedItem)
            ExtensionFilesBindingSource.ResetBindings(False)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error removing template")
        End Try
    End Sub

    Private Sub FilterButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        FilterExtension(FilterComboBox.Text)
        Dim Filters As StringCollection = CType(FiltersBindingSource.DataSource, StringCollection)
        If FilterComboBox.Text <> "" AndAlso (Not Filters.Contains(FilterComboBox.Text)) Then
            'TODO: check that FilterExtension didn't get an exception before adding
            Filters.Add(FilterComboBox.Text)
        End If

        FiltersBindingSource.ResetBindings(False)
        ExtensionFilesBindingSource.ResetBindings(False)
    End Sub

    Private Sub FilterMethod()
        FilterExtension(FilterComboBox.Text)
        Dim Filters As StringCollection = CType(FiltersBindingSource.DataSource, StringCollection)
        If FilterComboBox.Text <> "" AndAlso (Not Filters.Contains(FilterComboBox.Text)) Then
            'TODO: check that FilterExtension didn't get an exception before adding
            Filters.Add(FilterComboBox.Text)
        End If

        FiltersBindingSource.ResetBindings(False)
        ExtensionFilesBindingSource.ResetBindings(False)
    End Sub

    Private Sub ResetMethod()
        FilterComboBox.Text = ""
        FilterExtension("")
        ExtensionFilesBindingSource.ResetBindings(False)
    End Sub

    Private Sub DeleteMethod()
        Dim Filters As StringCollection = CType(FiltersBindingSource.DataSource, StringCollection)
        If Filters.Contains(FilterComboBox.Text) Then
            Filters.Remove(FilterComboBox.Text)
        End If
        FiltersBindingSource.ResetBindings(False)
        ExtensionFilesBindingSource.ResetBindings(False)
    End Sub

    Private Sub RunMethod()
        If ExtensionsListBox.SelectedItems.Count > 0 Then
            For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
                RunExtension(item)
            Next
        Else
            MsgBox("Please select an item.")
        End If
        Try
            'Dim MyProcess As New Process()
            'MyProcess.StartInfo.FileName = "explorer.exe"
            'MyProcess.StartInfo.Arguments = OutputFolder
            'MyProcess.Start()
        Catch ex As Exception
            'Ignore errors
        End Try
    End Sub

    'OBSOLETE: no longer used
    'Private Sub RunButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    If ExtensionsListBox.SelectedItems.Count > 0 Then
    '        RunToolStripMenuItem.Enabled = False
    '        RunToolStripMenuItem2.Enabled = False
    '        For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
    '            RunExtension(item)
    '        Next
    '        RunToolStripMenuItem.Enabled = True
    '        RunToolStripMenuItem2.Enabled = True
    '    Else
    '        MsgBox("Please select an item.")
    '    End If
    '    Try
    '        'Dim MyProcess As New Process()
    '        'MyProcess.StartInfo.FileName = "explorer.exe"
    '        'MyProcess.StartInfo.Arguments = OutputFolder
    '        'MyProcess.Start()
    '    Catch ex As Exception
    '        'Ignore errors
    '    End Try
    'End Sub

    'Handles extension reset
    Private Sub ResetToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ResetToolStripMenuItem1.Click
        TemplatesMenuStrip.Enabled = False
        Try
            If ExtensionsListBox.SelectedItems.Count > 0 Then
                For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
                    ResetExtension(item)
                Next
            Else
                MsgBox("Please select an item.")
            End If
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub ExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If ExtensionsListBox.SelectedItems.Count > 0 Then
            For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
                ExportExtension(item)
            Next
        Else
            MsgBox("Please select an item.")
        End If
    End Sub

    Private Sub FilterComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilterComboBox.SelectedIndexChanged
        FilterExtension(FilterComboBox.Text)
        ExtensionFilesBindingSource.ResetBindings(False)
    End Sub

    Private Sub ExportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportToolStripMenuItem.Click
        TemplatesMenuStrip.Enabled = False
        Try
            If ExtensionsListBox.SelectedItems.Count > 0 Then
                For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
                    ExportExtension(item)
                Next
            Else
                MsgBox("Please select an item.")
            End If
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub AddToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddToolStripMenuItem.Click
        TemplatesMenuStrip.Enabled = False
        Try
            If OpenExtensionFileDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
                For Each FileName In OpenExtensionFileDialog.FileNames
                    Try
                        AddExtension(FileName)
                        ExtensionFilesBindingSource.ResetBindings(False)
                    Catch ex As Exception
                        MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error adding new template")
                    End Try
                Next
            End If
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub RemoveToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveToolStripMenuItem1.Click
        TemplatesMenuStrip.Enabled = False
        Try
            If ExtensionsListBox.SelectedItems.Count > 0 Then
                Dim ExtensionsToDelete As New List(Of ExtensionItem)
                For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
                    ExtensionsToDelete.Add(item)
                Next
                Dim ExtensionsToDeleteArray As ExtensionItem() = ExtensionsToDelete.ToArray()
                For i = 0 To ExtensionsToDeleteArray.Length - 1
                    Try
                        ExtensionMgr.Remove(ExtensionsToDeleteArray(i))
                        ExtensionFilesBindingSource.ResetBindings(False)
                    Catch ex As Exception
                        MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Error removing template")
                    End Try
                Next
            Else
                MsgBox("Please select an item.")
            End If
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        TemplatesMenuStrip.Enabled = False
        Try
            If ExtensionsListBox.SelectedItems.Count > 0 Then
                For Each item As ExtensionItem In ExtensionsListBox.SelectedItems
                    'Check that the file still exists
                    If Not My.Computer.FileSystem.FileExists(item.File) Then
                        MsgBox("File doesn't exist: " & item.File, MsgBoxStyle.Exclamation, "Error opening File")
                        'TODO: Could optionally remove this from the list.
                    Else
                        Try
                            Process.Start(item.File)
                        Catch ex As Exception
                            MsgBox(ex.Message, MsgBoxStyle.Critical, "Error opening File")
                        End Try
                    End If
                Next
            Else
                MsgBox("Please select an item.")
            End If
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub FilterToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FilterToolStripMenuItem1.Click
        TemplatesMenuStrip.Enabled = False
        Try
            Dim Result As Boolean = FilterExtension(FilterComboBox.Text)
            Dim Filters As StringCollection = CType(FiltersBindingSource.DataSource, StringCollection)
            Dim Selected As String = FilterComboBox.Text
            If Result AndAlso (Not Filters.Contains(FilterComboBox.Text)) Then
                'TODO: check that FilterExtension didn't get an exception before adding
                Filters.Add(FilterComboBox.Text)

                FiltersBindingSource.ResetBindings(False)
                FilterComboBox.SelectedItem = Selected
                ExtensionFilesBindingSource.ResetBindings(False)
            Else
                FiltersBindingSource.ResetBindings(False)
                ExtensionFilesBindingSource.ResetBindings(False)
            End If
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub ResetToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetToolStripMenuItem.Click
        TemplatesMenuStrip.Enabled = False
        Try
            FilterComboBox.Text = ""
            Call FilterExtension("")
            ExtensionFilesBindingSource.ResetBindings(False)
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub RemoveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RemoveToolStripMenuItem.Click
        TemplatesMenuStrip.Enabled = False
        Try
            Dim Filters As StringCollection = CType(FiltersBindingSource.DataSource, StringCollection)
            If Filters.Contains(FilterComboBox.Text) Then
                Filters.Remove(FilterComboBox.Text)
            End If
            FiltersBindingSource.ResetBindings(False)
            ExtensionFilesBindingSource.ResetBindings(False)
        Finally
            TemplatesMenuStrip.Enabled = True
        End Try
    End Sub

    Private Sub RunToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunToolStripMenuItem2.Click, RunToolStripMenuItem.Click
        RunMethod()
    End Sub

    'Backgroundworker code was taken from here:
    'https://msdn.microsoft.com/en-us/library/vstudio/ms233672(v=vs.100).aspx 
    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        ' Do not access the form's BackgroundWorker reference directly.
        ' Instead, use the reference provided by the sender parameter.
        workBw = CType(sender, BackgroundWorker)

        ' Extract the argument. The commented code below causes an exception.
        'Dim arg As Integer = Fix(e.Argument)
        'This handler is removed in RunWorkerCompleted
        AddHandler ExtensionCreator.Extension.ShowMessage, AddressOf ReportProgress
        AddHandler ExtensionCreator.Extension.InteractUser, AddressOf RunGUI
        AddHandler ExtensionCreator.Extension.TemplateError, AddressOf OnTemplateError
        AddHandler progressForm.FormClosed, AddressOf ProgressForm_Closed
        ' Start the time-consuming operation. Note that e.Argument is an ExtensionItem.
        'Note that if there is an exception, e.error in the RunWorkerCompleted sub will be true.
        e.Result = ExtensionMgr.Run(e.Argument)

        ' If the operation was canceled by the user, 
        ' set the DoWorkEventArgs.Cancel property to true.
        If workBw.CancellationPending Then
            e.Cancel = True 'TODO: (optional) handle BackgroundWorker cancel event.
        End If
    End Sub

    'Simple method to show windows forms on main GUI thread, which is STA (single-threaded-apartment).
    'Code from: http://stackoverflow.com/questions/10498555/calling-showdialog-in-backgroundworker
    Public Sub RunGUI(ByVal Func As [Delegate])
        Me.Invoke(Func)
    End Sub

    Private Sub OnTemplateError(ByVal host As TemplateEngineHost, ByVal ex As Exception)
        Dim tplRenderedForm As New frmRenderedOutput()
        'TODO: remove LoadData exception argument
        tplRenderedForm.LoadData(host, ex)

        If tplRenderedForm.ShowDialog() = Windows.Forms.DialogResult.Retry Then
            'try again
            host.ErrorResolved = True
        Else
            host.ErrorResolved = False
        End If
        tplRenderedForm.Dispose()
    End Sub

    Public Sub ReportProgress(ByVal Message As String)
        'TODO: (optional) show percent progress as well
        workBw.ReportProgress(0, Message)
        'TaskListBox.Items.Add(Message)
    End Sub

    ' This event handler demonstrates how to interpret 
    ' the outcome of the asynchronous operation implemented
    ' in the DoWork event handler.
    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Dim OutputFolder As String = ""

        RemoveHandler ExtensionCreator.Extension.ShowMessage, AddressOf ReportProgress
        RemoveHandler ExtensionCreator.Extension.InteractUser, AddressOf RunGUI
        RemoveHandler ExtensionCreator.Extension.TemplateError, AddressOf OnTemplateError
        RemoveHandler progressForm.FormClosed, AddressOf ProgressForm_Closed
        If e.Cancelled Then
            ' The user canceled the operation.
            MessageBox.Show("Operation was canceled")
        ElseIf (e.Error IsNot Nothing) Then
            ' There was an error during the operation.
            MsgBox(e.Error.Message, MsgBoxStyle.Exclamation, "Error running template")
            progressForm.NotifyLabel.Text = "Error occurred"
        Else
            OutputFolder = e.Result

            progressForm.NotifyLabel.Text = "Finished running"
            ' The operation completed normally.
            'Dim msg As String = String.Format("Result = {0}", e.Result)
            'MessageBox.Show(msg)
        End If
        progressForm.TaskProgressBar.Hide()
        TemplatesMenuStrip.Enabled = True
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        progressForm.MessageDisplayer(e.UserState)
    End Sub

    Private Sub ProgressForm_Closed(sender As Object, e As FormClosedEventArgs)
        BackgroundWorker1.CancelAsync()
        'TODO: implement cancel feature. Get Extension class to stop execution of the run function.
    End Sub

End Class