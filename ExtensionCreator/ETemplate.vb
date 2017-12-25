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

    Public Function KeyExists(ByVal param As String) As Boolean
        Dim results As String() = Split(param, ".")
        If results.Length = 1 Then
            Return CType(Ext.ReplacementDictionaries("Extension"), MyDictionary).ContainsKey(param)
        Else
            Return CType(Ext.ReplacementDictionaries(results(0)), MyDictionary).ContainsKey(results(1))
        End If
    End Function

    Public Function Value(ByVal param As String) As String
        If Not KeyExists(param) Then
            Dim msgResult As MsgBoxResult = MsgBox("Parameter error: " & param & " not found. Continue running extension?", MsgBoxStyle.YesNo)
            If msgResult = MsgBoxResult.No Then
                Throw New ExParameterNotFoundException("Error running extension: Parameter " & param & " not found.")
            End If
            Return ""
        Else
            Dim results As String() = Split(param, ".")
            If results.Length = 1 Then
                Return CType(Ext.ReplacementDictionaries("Extension"), MyDictionary).MyItem(param)
            Else
                Return CType(Ext.ReplacementDictionaries(results(0)), MyDictionary).MyItem(results(1))
            End If
        End If
    End Function
End Class