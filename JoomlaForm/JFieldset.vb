Imports System.Xml.Serialization
Imports System.ComponentModel

Public Class JFieldset
    Private _name As String
    Private _label As String
    Private _Description As String = ""
    Private _Fields As New List(Of JField)

    <XmlElement(ElementName:="field")> _
    Public Property Fields() As List(Of JField)
        Get
            Return _Fields
        End Get
        Set(ByVal value As List(Of JField))
            _Fields = value
        End Set
    End Property

    <XmlAttribute(AttributeName:="name")> _
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
            If _label = "" Then
                Dim myNameArray() As String = Split(value, "_")
                For i = 0 To (myNameArray.Length - 1)
                    myNameArray(i) = StrConv(myNameArray(i), VbStrConv.ProperCase)
                Next
                _label = Join(myNameArray, " ")
            End If
        End Set
    End Property

    <XmlAttribute(AttributeName:="label")> _
    Public Property Label() As String
        Get
            Return _label
        End Get
        Set(ByVal value As String)
            _label = StrConv(value, VbStrConv.ProperCase)
            If _name = "" Then
                _name = Replace(StrConv(value, VbStrConv.Lowercase), " ", "_")
            End If
        End Set
    End Property

    <DefaultValueAttribute("")> _
    <XmlAttribute(AttributeName:="description")> _
    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property

    Private _addfieldpath As String
    <DefaultValueAttribute("")> _
    <XmlAttribute(AttributeName:="addfieldpath")> _
    Public Property Addfieldpath() As String
        Get
            Return _addfieldpath
        End Get
        Set(ByVal value As String)
            _addfieldpath = value
        End Set
    End Property
End Class
