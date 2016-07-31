Imports System.Dynamic

Public Class Task
    Inherits DynamicObject

    Private Parameters As New Dictionary(Of String, String)

    Public Sub New(ByVal TaskParameters As Dictionary(Of String, String), ByVal ParamArray Params() As String)
        For Each Param As String In Params
            If TaskParameters.ContainsKey(Param) Then
                Parameters.Add(Param, TaskParameters.Item(Param))
            Else
                Parameters.Add(Param, "")
            End If
        Next
    End Sub

    ' Implement the TryGetMember method of the DynamicObject class for dynamic member calls.
    Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder,
                                           ByRef result As Object) As Boolean
        If Parameters.ContainsKey(binder.Name) Then
            result = Parameters.Item(binder.Name)
            Return True
        Else
            result = Nothing
            Return False
        End If
    End Function

    Public Overrides Function GetDynamicMemberNames() As IEnumerable(Of String)
        Return Parameters.Keys
    End Function
End Class