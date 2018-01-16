Public Class frmTaskProgress

    Private Sub frmTaskProgress_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'AddHandler TemplateMaker.Extension.ShowMessage, AddressOf MessageDisplayer
    End Sub

    Public Sub MessageDisplayer(ByVal Message As String)
        TaskListBox.Items.Add(Message)
        TaskListBox.TopIndex = TaskListBox.Items.Count - 1
    End Sub

End Class