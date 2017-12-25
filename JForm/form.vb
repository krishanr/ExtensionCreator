Imports System.Xml
Imports System.Xml.Serialization
Imports System.ComponentModel
Imports System.IO

Public Class form
    Private _Fieldsets As New List(Of JFieldset)
    Private _allowableFields As List(Of String) = Nothing
    Private _name As String = ""

    Public Enum SaveMethod
        Form = 1
        Fields = 2
        Context = 3
        Fieldsets = 4
    End Enum

    <XmlAttribute(AttributeName:="name")> _
    <DefaultValueAttribute("")> _
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    <XmlElement(ElementName:="fieldset")> _
    Public Property FieldSets() As List(Of JFieldset)
        Get
            Return _Fieldsets
        End Get
        Set(ByVal value As List(Of JFieldset))
            _Fieldsets = value
        End Set
    End Property

    Public Sub loadTypes(ByVal JFormDataset As JFormDataSet)
        Dim myDS As New JFormDataSet
        Using sr As New StringReader(My.Resources.type)
            myDS.type.ReadXml(sr)
        End Using
        For Each row As JFormDataSet.typeRow In myDS.type.Select(Nothing, "type")
            JFormDataset.type.ImportRow(row)
        Next
    End Sub

    Sub loadCustomTypes(ByVal JFormDataset As JFormDataSet)
        Dim myDS As New JFormDataSet
        For Each item As String In AllowableFields
            Dim ARow As JFormDataSet.CustomTypeRow
            ARow = myDS.CustomType.AddCustomTypeRow(item)
        Next

        For Each row As JFormDataSet.CustomTypeRow In myDS.CustomType.Select(Nothing, "CustomType")
            JFormDataset.CustomType.ImportRow(row)
        Next
    End Sub

    'Requires an xml file that exists on the file system.
    'If merge is set to true then fieldsets with the same name will be merged.
    'Note: field names are not unique in a fieldset, so by merging it's possible 
    'that you end up with 2 or more fields with the same name.
    Public Sub Load(ByVal InputFileName As String, Optional ByVal Merge As Boolean = False)
        Dim XmlDoc As New XmlDocument
        Dim myForm As form = Nothing
        Dim FormElem As XmlElement
        XmlDoc.Load(InputFileName)
        AddClassInfoToFields(XmlDoc)
        Dim XmlNodeList As XmlNodeList = XmlDoc.SelectNodes("//fieldset")
        If XmlNodeList.Count() > 0 Then
            FormElem = XmlDoc.CreateElement("form")
            For Each XmlElem As XmlElement In XmlNodeList
                FormElem.AppendChild(XmlElem)
            Next
            Using sr As StringReader = New StringReader(FormElem.OuterXml)
                Using XmlReader As XmlReader = XmlReader.Create(sr)
                    Dim x As New XmlSerializer(GetType(form), New XmlAttributeOverrides(), GetExtraTypes(), New XmlRootAttribute("form"), "")
                    If x.CanDeserialize(XmlReader) Then
                        myForm = x.Deserialize(XmlReader)
                        If Merge Then
                            For Each myFieldset As JFieldset In myForm.FieldSets
                                Dim myFieldsetName As String = myFieldset.Name
                                Dim Results As IEnumerable(Of JFieldset) = From item In FieldSets _
                                                                           Where item.Name = myFieldsetName
                                If Results.Count > 0 Then
                                    Dim AFieldset As JFieldset = Results.First
                                    AFieldset.Fields.AddRange(myFieldset.Fields)
                                Else
                                    FieldSets.Add(myFieldset)
                                End If
                            Next
                        Else
                            FieldSets.AddRange(myForm.FieldSets)
                        End If
                    Else
                        Throw New Exception(InputFileName & " is not a valid form file.")
                    End If
                End Using
            End Using
        Else
            'Throw New Exception("No fieldsets found to load from in file: " & InputFileName)
        End If
    End Sub

    Public Sub AddClassInfoToFields(ByRef XmlDoc As XmlDocument)
        Dim XmlNodeList As XmlNodeList = XmlDoc.SelectNodes("//field")
        For Each XmlElem As XmlElement In XmlNodeList
            If XmlElem.HasAttribute("type") Then
                If AllowableFields.Contains(XmlElem.GetAttribute("type")) And (Not XmlElem.HasAttribute("type", "http://www.w3.org/2001/XMLSchema-instance")) Then
                    XmlElem.SetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", XmlElem.GetAttribute("type"))
                End If
            End If
        Next
    End Sub

    Public Sub RemoveClassInfoFromFields(ByRef XmlDoc As XmlDocument)
        Dim XmlNodeList As XmlNodeList = XmlDoc.SelectNodes("//field")
        For Each XmlElem As XmlElement In XmlNodeList
            If XmlElem.HasAttribute("type") Then
                If XmlElem.HasAttribute("type", "http://www.w3.org/2001/XMLSchema-instance") Then
                    'TODO: needs work
                    XmlElem.SetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", "")
                End If
            End If
        Next
    End Sub

    'Requires a valid xml file name that exists as OutputFileName.
    'If <config> is found in the xml file its content is replaced with the form
    'content inside a <fields> element with name 'params'.
    'If <form> is found, it is replaced with the content of the form.
    'Note: Make sure multiple <config> or <form> elements don't exist.
    Public Sub SaveBasedOnContext(ByVal SaveMethod As SaveMethod, ByVal OutputFileName As String)
        Dim XDoc As XDocument
        Dim FormContents As XElement
        Dim XmlWriterSettings As New XmlWriterSettings
        Dim sw As New StringWriter
        Dim RootAttribute As String

        Try
            XDoc = XDocument.Load(OutputFileName)
        Catch ex As Exception
            'TODO: what to do here?
            Throw New Exception(ex.Message, ex)
        End Try

        'If we are dealing with a blank file just save it as a form.
        If XDoc.Elements.Count = 0 Then
            XDoc = New XDocument(<form></form>)
        End If

        Dim Results As IEnumerable(Of XElement)
        Results = From elem In XDoc.Descendants("form")
                  Select elem

        If Results.Count > 0 Then
            'This is a form.
            XmlWriterSettings.OmitXmlDeclaration = False
            RootAttribute = "form"
        Else
            Results = From elem In XDoc.Descendants("config")
                      Select elem
            If Results.Count > 0 Then
                XmlWriterSettings.OmitXmlDeclaration = True
                If Name = "" Then
                    Name = "params"
                End If
                RootAttribute = "fields"
            Else
                Throw New Exception("Can't determine how to save the form, no config or form element found in " & OutputFileName)
            End If
        End If
        Dim x As New XmlSerializer(Me.GetType, New XmlAttributeOverrides(), GetExtraTypes(), New XmlRootAttribute(RootAttribute), "")

        XmlWriterSettings.Indent = True
        XmlWriterSettings.IndentChars = (ControlChars.Tab)
        Dim XmlSerializerNameSpaces As New XmlSerializerNamespaces
        XmlSerializerNameSpaces.Add("", "")
        XmlWriterSettings.NewLineOnAttributes = True
        Using XmlWriter As XmlWriter = XmlWriter.Create(sw, XmlWriterSettings)
            x.Serialize(XmlWriter, Me, XmlSerializerNameSpaces)
        End Using

        Dim sr As TextReader = New StringReader(sw.ToString)
        FormContents = XElement.Load(sr)
        If RootAttribute = "form" Then
            Results.First.ReplaceWith(FormContents)
        ElseIf SaveMethod = SaveMethod.Context Then
            Results.First.ReplaceNodes(FormContents)
        Else
            Results.First.ReplaceNodes(FormContents.Elements)
        End If
        Using XmlWriter As XmlWriter = XmlWriter.Create(OutputFileName, XmlWriterSettings)
            XDoc.Save(XmlWriter)
        End Using
    End Sub

    'Requires a valid xml file name that exists as OutputFileName.
    Public Sub SaveUsingSaveMethod(ByVal SaveMethod As SaveMethod, ByVal OutputFileName As String)
        Dim myObject
        Dim XmlWriterSettings As New XmlWriterSettings
        Select Case SaveMethod
            Case form.SaveMethod.Form
                myObject = Me
                XmlWriterSettings.OmitXmlDeclaration = False
            Case form.SaveMethod.Fields
                XmlWriterSettings.OmitXmlDeclaration = True
                Dim myFields As New fields
                myFields.FieldSets = Me.FieldSets
                'TODO: keep this as a form param...
                myFields.Name = "params"
                myObject = myFields
            Case Else
                Throw New Exception("Form doesn't accept the given save method.")
        End Select
        Dim x As New XmlSerializer(myObject.GetType, New XmlAttributeOverrides(), GetExtraTypes(), New XmlRootAttribute(myObject.GetType().Name), "")

        XmlWriterSettings.Indent = True
        XmlWriterSettings.IndentChars = (ControlChars.Tab)
        Dim XmlSerializerNameSpaces As New XmlSerializerNamespaces
        XmlSerializerNameSpaces.Add("", "")
        XmlWriterSettings.NewLineOnAttributes = True
        Using XmlWriter As XmlWriter = XmlWriter.Create(OutputFileName, XmlWriterSettings)
            x.Serialize(XmlWriter, myObject, XmlSerializerNameSpaces)
        End Using

        Dim XmlDoc As New XmlDocument
        XmlDoc.Load(OutputFileName)
        Using XmlWriter As XmlWriter = XmlWriter.Create(OutputFileName, XmlWriterSettings)
            'RemoveClassInfoFromFields(XmlDoc)
            XmlDoc.Save(XmlWriter)
        End Using
    End Sub

    'Requires a valid xml file name that exists as OutputFileName.
    Public Sub Save(ByVal SaveMethod As SaveMethod, ByVal OutputFileName As String)
        Select Case SaveMethod
            Case SaveMethod.Context, SaveMethod.Fieldsets
                SaveBasedOnContext(SaveMethod, OutputFileName)
            Case Else
                SaveUsingSaveMethod(SaveMethod, OutputFileName)
        End Select
    End Sub

    Public Function GetExtraTypes() As Type()
        Dim ExtraTypes As New List(Of Type)
        For Each item As String In AllowableFields
            ExtraTypes.Add(System.Type.GetType("JForm." & item))
        Next
        Return ExtraTypes.ToArray()
    End Function

    'Returns XmlAttributeOverrides using AllowableFields
    Public Function GetXmlAttributeOverrides() As XmlAttributeOverrides
        Dim attrOverrides As New XmlAttributeOverrides()
        ' Create the XmlAttributes class.
        Dim attrs As New XmlAttributes()

        For Each item As String In AllowableFields
            Dim attr As New XmlElementAttribute()
            'TODO: Check that there is no conflict with JField, since it does that auto
            attr = New XmlElementAttribute(item, System.Type.GetType("JForm." & item))
            attrs.XmlElements.Add(attr)
        Next

        ' Add the XmlAttributes to the XmlAttributeOverrides. 
        attrOverrides.Add(GetType(JFieldset), "Fields", attrs)
        Return attrOverrides
    End Function

    <XmlIgnore()> _
    Public ReadOnly Property AllowableFields() As List(Of String)
        Get
            If IsNothing(_allowableFields) Then
                _allowableFields = New List(Of String)
                Dim thisAssembly As System.Reflection.Assembly
                thisAssembly = System.Reflection.Assembly.GetExecutingAssembly
                Dim myTypes As System.Type() = thisAssembly.GetTypes
                Dim FieldType As Type = GetType(JField)
                For Each singleType As Type In myTypes
                    If FieldType.IsAssignableFrom(singleType) Then
                        _allowableFields.Add(singleType.Name)
                    End If
                Next
            End If
            Return _allowableFields
        End Get
    End Property

End Class
