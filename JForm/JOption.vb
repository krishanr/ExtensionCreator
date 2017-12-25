Imports System.Xml.Serialization
Imports System.ComponentModel

<TypeConverter(GetType(ExpandableObjectConverter)), _
DefaultPropertyAttribute("OptValue")> _
Public Class JOption

    Private _text As String
    <XmlText()> _
    Public Property OptText() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            _text = value
        End Set
    End Property

    Private _value As String
    <XmlAttribute(AttributeName:="value")> _
    Public Property OptValue() As String
        Get
            Return _value
        End Get
        Set(ByVal value As String)
            _value = value
        End Set
    End Property

End Class
