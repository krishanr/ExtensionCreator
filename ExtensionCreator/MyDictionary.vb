Imports System.IO
Imports System.Runtime.Serialization

<Serializable>
Public Class MyDictionary
    Inherits Dictionary(Of String, String)

    Public Sub New()
    End Sub

    Protected Sub New(info As SerializationInfo, context As StreamingContext)
        MyBase.New(info, context)
    End Sub

    Public Sub SetValue(ByVal key As String, ByVal value As String)
        If Not Me.ContainsKey(key) Then
            Me.Add(key, value)
        Else
            Me.Item(key) = value
        End If
    End Sub

    Public Function GetValue(ByVal key As String, Optional ByVal defaultValue As String = "") As String
        If Not Me.ContainsKey(key) Then
            Return defaultValue
        Else
            Return Me.Item(key)
        End If
    End Function

    Public Function MyItem(ByVal key As String) As String
        If Not Me.ContainsKey(key) Then
            Dim sw As New StringWriter
            sw.Write("Key '{0}' not found in dictionary.", key)
            Throw New KeyNotFoundException(sw.ToString())
        Else
            Return Me.Item(key)
        End If
    End Function

    Public Function SetDefault(ByVal key As String, ByVal value As String) As String
        If Not Me.ContainsKey(key) Then
            Me.Add(key, value)
            Return value
        Else
            Return Me.Item(key)
        End If
    End Function

End Class
