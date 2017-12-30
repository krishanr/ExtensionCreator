Imports System.ComponentModel
Imports System.Xml.Serialization

<DefaultPropertyAttribute("Name")> _
Public Class JField

    Private _name As String = ""
    Private _label As String = ""
    Private _type As String = "text"
    Private _Description As String = ""
    Private _hidden As Boolean
    Private _filter As String
    Private _validate As String
    Private _required As Boolean
    Private _default As String
    Private _onchange As String
    Private _disabled As Boolean = False

    Private _className As String = Me.GetType().Name
    <XmlIgnore()> _
    Public Property CustomType() As String
        Get
            Return _className
        End Get
        Set(ByVal value As String)
            '_className = value
        End Set
    End Property

    <CategoryAttribute(" Standard"), _
     DefaultValueAttribute("")> _
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

    <CategoryAttribute(" Standard"), _
     DefaultValueAttribute("")> _
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

    <CategoryAttribute(" Standard")> _
    <XmlAttribute(AttributeName:="type")> _
    Public Overridable Property Type() As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    <CategoryAttribute(" Standard"), _
     DefaultValueAttribute("")> _
    <XmlAttribute(AttributeName:="description")> _
    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property

    <CategoryAttribute(" Standard"), _
     DefaultValueAttribute("")> _
    <XmlAttribute(AttributeName:="default")> _
    Public Property DefaultValue() As String
        Get
            Return _default
        End Get
        Set(ByVal value As String)
            _default = value
        End Set
    End Property

    <DescriptionAttribute("Don't yet know for which fields this works."), _
     CategoryAttribute("Display"), _
     DefaultValueAttribute(False)>
    <XmlAttribute(AttributeName:="hidden")> _
    Public Property Hidden() As Boolean
        Get
            Return _hidden
        End Get
        Set(ByVal value As Boolean)
            _hidden = value
        End Set
    End Property

    Private _width As String
    <DescriptionAttribute("This is only used if this field is part of a data grid."), _
     CategoryAttribute("Display"), _
     DefaultValueAttribute("")>
    <XmlAttribute(AttributeName:="width")> _
    Public Property Width() As String
        Get
            Return _width
        End Get
        Set(ByVal value As String)
            _width = value
        End Set
    End Property

    Private _cssclass As String
    <CategoryAttribute("Display")> _
    <XmlAttribute(AttributeName:="class")> _
    Public Property CSSClass() As String
        Get
            Return _cssclass
        End Get
        Set(ByVal value As String)
            _cssclass = value
        End Set
    End Property

    <CategoryAttribute("Validation"), _
     DefaultValueAttribute(False)> _
    <XmlAttribute(AttributeName:="required")> _
    Public Property Required() As Boolean
        Get
            Return _required
        End Get
        Set(ByVal value As Boolean)
            _required = value
        End Set
    End Property

    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="filter")> _
    Public Overridable Property Filter() As String
        Get
            Return _filter
        End Get
        Set(ByVal value As String)
            _filter = value
        End Set
    End Property

    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="validate")> _
    Public Overridable Property Validate() As String
        Get
            Return _validate
        End Get
        Set(ByVal value As String)
            _validate = value
        End Set
    End Property

    <CategoryAttribute("Validation")> _
    <XmlAttribute(AttributeName:="onchange")> _
    Public Property OnChange() As String
        Get
            Return _onchange
        End Get
        Set(ByVal value As String)
            _onchange = value
        End Set
    End Property

    <CategoryAttribute("Behavior"), _
     DefaultValueAttribute(False)> _
    <XmlAttribute(AttributeName:="disabled")> _
    Public Property Disabled() As Boolean
        Get
            Return _disabled
        End Get
        Set(ByVal value As Boolean)
            _disabled = value
        End Set
    End Property

    Private _readOnly As Boolean
    <CategoryAttribute("Behavior"), _
     DefaultValueAttribute(False)> _
    <XmlAttribute(AttributeName:="readonly")> _
    Public Overridable Property ReadOnlyField() As Boolean
        Get
            Return _readOnly
        End Get
        Set(ByVal value As Boolean)
            _readOnly = value
        End Set
    End Property

    Private _select As String
    <DescriptionAttribute("This will be used to filter the view for DatabaseToAdminConnector. If you use an alias, make sure it is unique (different from the alias in the task parameters for the parent table)."), _
     CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="select")> _
    Public Property SelectStatement() As String
        Get
            Return _select
        End Get
        Set(ByVal value As String)
            _select = value
        End Set
    End Property

    Private _join As String
    <DescriptionAttribute("This will be used to filter the view for DatabaseToAdminConnector. This will default to a left join if joinType is not specified. Use the same alias as in the select for the joined table, and the same alias as specified in the task parameters for the parent table."), _
     CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="join")> _
    Public Property JoinStatement() As String
        Get
            Return _join
        End Get
        Set(ByVal value As String)
            _join = value
        End Set
    End Property

    Private _joinType As String = "LEFT"
    <TypeConverter(GetType(JoinTypeConverter)), _
     DescriptionAttribute("The type of join used for the join statement if specified."), _
     DefaultValue("LEFT"), _
     CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="joinType")> _
    Public Property JoinType() As String
        Get
            Return _joinType
        End Get
        Set(ByVal value As String)
            _joinType = value
        End Set
    End Property

    Public Class JoinTypeConverter
        Inherits StringConverter

        Public Overloads Overrides Function GetStandardValuesSupported( _
                            ByVal context As ITypeDescriptorContext) As Boolean
            Return True
        End Function

        Public Overloads Overrides Function GetStandardValues( _
                             ByVal context As ITypeDescriptorContext) _
                          As StandardValuesCollection

            Return New StandardValuesCollection(New String() {"LEFT", _
                                                              "RIGHT", _
                                                              "OUTER", _
                                                              "INNER"})
        End Function
    End Class

    Private _where As String
    <DescriptionAttribute("This will be used to filter the view for DatabaseToAdminConnector. Use the same alias as in the select for the joined table, and use {0} to denote the value of the field. Also use the same alias as specified in the task parameters for the parent table"), _
     CategoryAttribute("Sql")> _
    <XmlAttribute(AttributeName:="where")> _
    Public Property WhereStatement() As String
        Get
            Return _where
        End Get
        Set(ByVal value As String)
            _where = value
        End Set
    End Property

    Private _jtip_url As String
    <DescriptionAttribute("The url relative to site root of the tip html file."), _
     CategoryAttribute("Jtip")> _
    <XmlAttribute(AttributeName:="jtipUrl")> _
    Public Property JTipUrl() As String
        Get
            Return _jtip_url
        End Get
        Set(ByVal value As String)
            _jtip_url = value
            Dim Classes As List(Of String) = Split(_cssclass, " ").ToList
            Classes.Remove("")
            If value <> "" AndAlso (Not Classes.Contains("jInput")) Then
                Classes.Add("jInput")
            ElseIf value = "" AndAlso Classes.Contains("jInput") Then
                Classes.Remove("jInput")
            End If
            _cssclass = Join(Classes.ToArray, " ")
        End Set
    End Property

    Private _jtip_title As String
    <CategoryAttribute("Jtip")> _
    <XmlAttribute(AttributeName:="jtipTitle")> _
    Public Property JtipTitle() As String
        Get
            Return _jtip_title
        End Get
        Set(ByVal value As String)
            _jtip_title = value
        End Set
    End Property

    Private _fields As String = ""
    'This is put here so that other scripts using this class can use it. Not used by frmJForm.
    <Browsable(False)> _
    <XmlIgnore()> _
    Public Property Fields() As String
        Get
            Return _fields
        End Get
        Set(ByVal value As String)
            _fields = value
        End Set
    End Property

    Private _fieldset As String = ""
    'This is put here so that other scripts using this class can use it. Not used by frmJForm.
    <Browsable(False)> _
    <XmlIgnore()> _
    Public Property Fieldset() As String
        Get
            Return _fieldset
        End Get
        Set(ByVal value As String)
            _fieldset = value
        End Set
    End Property
End Class

Public Class FieldHolder
    Private _field As JField
    Public Property Field() As JField
        Get
            Return _field
        End Get
        Set(ByVal value As JField)
            _field = value
        End Set
    End Property
End Class