Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class fields
    Private _Fieldsets As New List(Of JFieldset)
    Private _name As String = ""

    <XmlElement(ElementName:="fieldset")> _
    Public Property FieldSets() As List(Of JFieldset)
        Get
            Return _Fieldsets
        End Get
        Set(ByVal value As List(Of JFieldset))
            _Fieldsets = value
        End Set
    End Property

    <XmlAttribute(AttributeName:="name")> _
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property
End Class
