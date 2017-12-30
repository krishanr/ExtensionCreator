Imports System.ComponentModel
Imports System.Xml.Serialization

Public Class media
    Inherits JField

    Private _type As String = "media"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "media"
        End Set
    End Property

    Private _hide_none As Boolean
    <CategoryAttribute("Behavior")> _
    <XmlAttribute(AttributeName:="hide_none")> _
    Public Property HideNone() As Boolean
        Get
            Return _hide_none
        End Get
        Set(ByVal value As Boolean)
            _hide_none = value
        End Set
    End Property

End Class

Public Class spacer
    Inherits JField

    Private _type As String = "spacer"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "spacer"
        End Set
    End Property

    Private _hr As Boolean = False
    <CategoryAttribute("Behavior"), _
     DefaultValueAttribute(False)> _
    <XmlAttribute(AttributeName:="hr")> _
    Public Property hr() As Boolean
        Get
            Return _hr
        End Get
        Set(ByVal value As Boolean)
            _hr = value
        End Set
    End Property
End Class

Public Class rules
    Inherits JField

    Private _type As String = "rules"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "rules"
        End Set
    End Property

    Private _component As String
    <CategoryAttribute("Behavior")> _
    <XmlAttribute(AttributeName:="component")> _
    Public Property Component() As String
        Get
            Return _component
        End Get
        Set(ByVal value As String)
            _component = value
        End Set
    End Property

    Private _section As String = "component"
    <CategoryAttribute("Behavior")> _
    <XmlAttribute(AttributeName:="section")> _
    Public Property Section() As String
        Get
            Return _section
        End Get
        Set(ByVal value As String)
            _section = value
        End Set
    End Property
End Class

Public Class modulelayout
    Inherits JField

    Private _type As String = "modulelayout"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "modulelayout"
        End Set
    End Property
End Class

Public Class column
    Inherits JField

    Private _type As String = "column"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "column"
        End Set
    End Property

    Private _readOnly As Boolean = True
    <CategoryAttribute("Behavior"), _
     DefaultValueAttribute(False)> _
    <XmlAttribute(AttributeName:="readonly")> _
    Public Overrides Property ReadOnlyField() As Boolean
        Get
            Return _readOnly
        End Get
        Set(ByVal value As Boolean)
            _readOnly = value
        End Set
    End Property

    Private _link As Boolean
    <CategoryAttribute(" Standard"), _
     DefaultValue(False)> _
    <XmlAttribute(AttributeName:="showLink")> _
    Public Property Link() As Boolean
        Get
            Return _link
        End Get
        Set(ByVal value As Boolean)
            _link = value
        End Set
    End Property
End Class

Public Class hidden
    Inherits JField

    Private _type As String = "hidden"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "hidden"
        End Set
    End Property
End Class

Public Class category
    Inherits list

    Private _type As String = "category"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "category"
        End Set
    End Property

    Private _extension As String = "com_content"
    <CategoryAttribute("Behavior")> _
    <XmlAttribute(AttributeName:="extension")> _
    Public Property Extension() As String
        Get
            Return _extension
        End Get
        Set(ByVal value As String)
            _extension = value
        End Set
    End Property

    Public Enum State
        Published = 1
        Unpublished = 0
        Trash = -2
        Archived = 2
        Standard = 3
    End Enum

    Private _published As Integer = State.Standard
    <CategoryAttribute("Behavior"), _
     DefaultValueAttribute(State.Standard), _
     DescriptionAttribute("Determines the state(s) of the categories that fill the list. Leave as 3 for the default which is published and unpublished.")> _
    <XmlAttribute(AttributeName:="published")> _
    Public Property Published() As Integer
        Get
            Return _published
        End Get
        Set(ByVal value As Integer)
            _published = value
        End Set
    End Property

    Private _validate As String
    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="validate")> _
    Public Overrides Property Validate() As String
        Get
            Return _validate
        End Get
        Set(ByVal value As String)
            _validate = value
        End Set
    End Property
End Class

Public Class email
    Inherits text

    Private _type As String = "email"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "email"
        End Set
    End Property

    Private _validate As String = "email"
    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="validate")> _
    Public Overrides Property Validate() As String
        Get
            Return _validate
        End Get
        Set(ByVal value As String)
            _validate = "email"
        End Set
    End Property
End Class

Public Class int
    Inherits text

    Private _size As Integer = 20
    <CategoryAttribute("Display"), _
       DefaultValueAttribute(0)> _
      <XmlAttribute(AttributeName:="size")> _
    Public Overrides Property Size() As Integer
        Get
            Return _size
        End Get
        Set(ByVal value As Integer)
            _size = value
        End Set
    End Property

    Private _filter As String = "intval"
    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="filter")> _
    Public Overrides Property Filter() As String
        Get
            Return _filter
        End Get
        Set(ByVal value As String)
            _filter = "intval"
        End Set
    End Property
End Class

Public Class calendar
    Inherits text

    Private _type As String = "calendar"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "calendar"
        End Set
    End Property

    Private _format As String = "%d-%m-%Y"
    <CategoryAttribute("Display")> _
    <XmlAttribute(AttributeName:="format")> _
    Public Property Format() As String
        Get
            Return _format
        End Get
        Set(ByVal value As String)
            _format = value
        End Set
    End Property

    Private _size As Integer = 22
    <CategoryAttribute("Display"), _
     DefaultValueAttribute(0)> _
    <XmlAttribute(AttributeName:="size")> _
    Public Overrides Property Size() As Integer
        Get
            Return _size
        End Get
        Set(ByVal value As Integer)
            _size = value
        End Set
    End Property
End Class

Public Class text
    Inherits JField

    Private _type As String = "text"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "text"
        End Set
    End Property

    Private _size As Integer = 30
    <CategoryAttribute("Display"), _
     DefaultValueAttribute(0)> _
    <XmlAttribute(AttributeName:="size")> _
    Public Overridable Property Size() As Integer
        Get
            Return _size
        End Get
        Set(ByVal value As Integer)
            _size = value
        End Set
    End Property

    Private _maxLength As Integer
    <CategoryAttribute("Validation"), _
     DefaultValueAttribute(0)> _
    <XmlAttribute(AttributeName:="maxLength")> _
    Public Property MaxLength() As Integer
        Get
            Return _maxLength
        End Get
        Set(ByVal value As Integer)
            _maxLength = value
        End Set
    End Property

End Class

Public Class textarea
    Inherits JField

    Private _type As String = "textarea"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "textarea"
        End Set
    End Property

    Private _rows As Integer = 3
    <CategoryAttribute("Display"), _
     DefaultValueAttribute(0)> _
    <XmlAttribute(AttributeName:="rows")> _
    Public Property Rows() As Integer
        Get
            Return _rows
        End Get
        Set(ByVal value As Integer)
            _rows = value
        End Set
    End Property

    Private _cols As Integer = 30
    <CategoryAttribute("Display"), _
     DefaultValueAttribute(0)> _
    <XmlAttribute(AttributeName:="cols")> _
    Public Property Cols() As Integer
        Get
            Return _cols
        End Get
        Set(ByVal value As Integer)
            _cols = value
        End Set
    End Property

    Private _filter As String = "safehtml"
    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="filter")> _
    Public Overrides Property Filter() As String
        Get
            Return _filter
        End Get
        Set(ByVal value As String)
            _filter = value
        End Set
    End Property
End Class

Public Class radio
    Inherits JField

    Private _type As String = "radio"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "radio"
        End Set
    End Property

    Private _options As New List(Of JOption)
    <CategoryAttribute(" Standard")> _
    <XmlElement(ElementName:="option")> _
    Public Overridable Property Options() As List(Of JOption)
        Get
            Return _options
        End Get
        Set(ByVal value As List(Of JOption))
            _options = value
        End Set
    End Property

End Class

Public Class list
    Inherits JField

    Private _type As String = "list"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "list"
        End Set
    End Property

    Private _options As New List(Of JOption)
    <CategoryAttribute(" Standard")> _
    <XmlElement(ElementName:="option")> _
    Public Overridable Property Options() As List(Of JOption)
        Get
            Return _options
        End Get
        Set(ByVal value As List(Of JOption))
            _options = value
        End Set
    End Property

    Private _multiple As Boolean = False
    <CategoryAttribute(" Standard"),
     DefaultValue(False)> _
    <XmlAttribute(AttributeName:="multiple")> _
    Public Property Multiple() As Boolean
        Get
            Return _multiple
        End Get
        Set(ByVal value As Boolean)
            _multiple = value
        End Set
    End Property

    Private _size As Integer = 1
    <CategoryAttribute("Display"), _
     DefaultValueAttribute(0)> _
    <XmlAttribute(AttributeName:="size")> _
    Public Property Size() As Integer
        Get
            Return _size
        End Get
        Set(ByVal value As Integer)
            _size = value
        End Set
    End Property

    Private _validate As String
    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="validate")> _
    Public Overrides Property Validate() As String
        Get
            Return "options"
        End Get
        Set(ByVal value As String)
            _validate = value
        End Set
    End Property
End Class

Public Class bool
    Inherits radio

    Private _options As New List(Of JOption)
    <CategoryAttribute(" Standard")> _
    <XmlElement(ElementName:="option")> _
    Public Overrides Property Options() As List(Of JOption)
        Get
            Dim myListOptions As New List(Of JOption)
            Dim myOption1 As New JOption
            myOption1.OptText = "JYES"
            myOption1.OptValue = "1"
            myListOptions.Add(myOption1)
            Dim myOption2 As New JOption
            myOption2.OptText = "JNO"
            myOption2.OptValue = "0"
            myListOptions.Add(myOption2)
            Return myListOptions
        End Get
        Set(ByVal value As List(Of JOption))
        End Set
    End Property

    Private _validate As String
    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="validate")> _
    Public Overrides Property Validate() As String
        Get
            Return "boolean"
        End Get
        Set(ByVal value As String)
            _validate = value
        End Set
    End Property
End Class

Public Class sql
    Inherits list

    Private _type As String = "sql"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "sql"
        End Set
    End Property

    Private _options As New List(Of JOption)
    <CategoryAttribute(" Standard")> _
    <XmlElement(ElementName:="option")> _
    Public Overrides Property Options() As List(Of JOption)
        Get
            'TODO: can be confusing, if you don't know what it does...
            Dim myOptions As New List(Of JOption)
            Dim myOption As New JOption
            myOption.OptValue = ""
            Dim sw As New IO.StringWriter
            sw = New IO.StringWriter
            sw.Write("- Select {0} -", Label)
            myOption.OptText = sw.ToString()
            myOptions.Add(myOption)
            Return myOptions
        End Get
        Set(ByVal value As List(Of JOption))
        End Set
    End Property

    Private _query As String
    <DescriptionAttribute("This is the query that will be used to fill the list with values."), _
     CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="query")> _
    Public Property Query() As String
        Get
            Return _query
        End Get
        Set(ByVal value As String)
            _query = value
        End Set
    End Property

    Private _key_field As String
    <CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="key_field")> _
    Public Property KeyField() As String
        Get
            Return _key_field
        End Get
        Set(ByVal value As String)
            _key_field = value
        End Set
    End Property

    Private _value_field As String
    <CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="value_field")> _
    Public Property ValueField() As String
        Get
            Return _value_field
        End Get
        Set(ByVal value As String)
            _value_field = value
        End Set
    End Property
End Class

Public Class editor
    Inherits JField

    Private _type As String = "editor"
    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overrides Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = "editor"
        End Set
    End Property

    Private _buttons As Boolean = True
    <CategoryAttribute("Behavior")> _
    <XmlAttribute(AttributeName:="buttons")> _
    Public Property Buttons() As Boolean
        Get
            Return _buttons
        End Get
        Set(ByVal value As Boolean)
            _buttons = value
        End Set
    End Property

End Class
