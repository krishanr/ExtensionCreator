Imports Microsoft.VisualStudio.TextTemplating

Public MustInherit Class ETemplate
    Inherits TextTransformation

    Protected Ext As Extension

    Public Overrides Sub Initialize()
        MyBase.Initialize()

        If (Not Me.Errors.HasErrors) AndAlso Me.Session.ContainsKey("Ext") Then
            Ext = Me.Session("Ext")
        End If
    End Sub

    Public Function Value(ByVal param As String)
        Dim results As String() = Split(param, ".")
        'TODO: handle exception here. Give better error message when parameter isn't found.
        If results.Length = 1 Then
            Return CType(Ext.ReplacementDictionaries("Extension"), MyDictionary).MyItem(param)
        Else
            Return CType(Ext.ReplacementDictionaries(results(0)), MyDictionary).MyItem(results(1))
        End If
    End Function
End Class