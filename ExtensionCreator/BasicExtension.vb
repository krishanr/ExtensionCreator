'The simplest possible extension.
Public Class BasicExtension
    Inherits Extension

    Public Sub New(ByVal xmlfile As String, ByVal title As String, ByVal extensionDocLoader As XmlTemplateLoader)
        MyBase.New(xmlfile, title, extensionDocLoader)
    End Sub

    Public Overrides Sub Run()
        MyBase.Run()
        CreateArchive()
    End Sub

    Public Overrides Sub Finish()
        'Add custom code here.
    End Sub
End Class
