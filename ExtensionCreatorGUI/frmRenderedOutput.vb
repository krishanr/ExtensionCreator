Imports ExtensionCreator
Imports System.IO

Public Class frmRenderedOutput

    Private host As TemplateEngineHost

    Public Sub LoadData(ByVal m_host As TemplateEngineHost, ByVal ex As Exception)
        host = m_host
        LabelNotice.Text = "Template compile error: Compiled code shown below:"
        LabelRetry.Text = "Please click retry if you have changed the source code, or cancel to stop the program."
        FillTextBoxRendered(ex)
    End Sub

    Public Sub FillTextBoxRendered(ByVal ex As Exception)
        If host.Errors.Count = 0 Then
            'We must have had a run-time exception
            LabelNotice.Text = "Run Exception: " & ex.Message
            TextBoxRendered.Text = "Error run: " & ex.Message & vbCrLf & vbCrLf & TextBoxRendered.Text
            Exit Sub
        Else
            SetRenderedText()
            'TODO: (Optional) put compiled code at top
            TextBoxRendered.Text &= "-----------------------------------------" & vbCrLf
            For i As Integer = 0 To host.Errors.Count - 1
                TextBoxRendered.Text &= "Line " & host.Errors(i).Line _
                    & ", column " & host.Errors(i).Column & ": " _
                    & host.Errors(i).ErrorNumber & " " _
                    & host.Errors(i).ErrorText _
                    & " (File: " & host.Errors(i).FileName _
                    & ")" & vbCrLf
            Next
        End If
    End Sub

    Private Sub SetRenderedText()
        TextBoxRendered.Text = ""
        Dim codeFile As String = ""
        If host.Errors.Count > 0 AndAlso host.Errors(0).FileName <> "" Then
            codeFile = host.Errors(0).FileName
        Else
            codeFile = host.TemplateFile
        End If

        If Not File.Exists(codeFile) Then
            Exit Sub
        End If

        Dim compiledCode As String = ""
        Using sr As New StreamReader(codeFile)
            compiledCode = sr.ReadToEnd()
        End Using

        Dim lines() As String
        lines = Split(compiledCode, vbCrLf)
        Dim i As Integer = 0
        For Each line As String In lines
            i += 1
            TextBoxRendered.Text &= Format(i, "00000") & ": " & line & vbCrLf
        Next
    End Sub
End Class