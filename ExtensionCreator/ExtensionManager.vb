Imports System.Reflection
Imports System.Reflection.Assembly
Imports System.Collections.Specialized
Imports System.Xml
Imports System.Xml.XPath
Imports System.Xml.Schema
Imports System.IO
Imports System.IO.Path
Imports <xmlns:xs="http://www.w3.org/2001/XMLSchema">

Public Class ExtensionManager

    Private _Extensions As New List(Of ExtensionItem)
    Private _ExtensionsView As New List(Of ExtensionItem)
    Private _CurrentView As String

    Public Property Extensions() As List(Of ExtensionItem)
        Get
            Return _Extensions
        End Get
        Set(ByVal value As List(Of ExtensionItem))
            _Extensions = value
        End Set
    End Property

    Public Property ExtensionsView() As List(Of ExtensionItem)
        Get
            Return _ExtensionsView
        End Get
        Set(ByVal value As List(Of ExtensionItem))
            _ExtensionsView = value
        End Set
    End Property

    Public Property CurrentView() As String
        Get
            Return _CurrentView
        End Get
        Set(ByVal value As String)
            _CurrentView = value
        End Set
    End Property

    Public ReadOnly Property ExtensionXmlFiles() As StringCollection
        Get
            Dim Result As New StringCollection
            For Each Item As ExtensionItem In Extensions
                Result.Add(Item.File)
            Next
            Return Result
        End Get
    End Property

    Public Sub Add(ByVal ExtensionXmlFile As String)
        'On start up the paths might be relative to the app directory, so make them absolute.
        'TODO: Think this code is now obsolete
        If Not Path.IsPathRooted(ExtensionXmlFile) Then
            ExtensionXmlFile = My.Application.Info.DirectoryPath & "\" & ExtensionXmlFile
        End If
        If Not ExtensionXmlFiles.Contains(ExtensionXmlFile) Then
            Dim AnExtensionItem As ExtensionItem = New ExtensionItem
            AnExtensionItem.File = ExtensionXmlFile
            AnExtensionItem.Label = GetExtensionLabel(ExtensionXmlFile)
            AnExtensionItem.ExtensionLoader = New XmlTemplateLoader(ExtensionXmlFile)
            Extensions.Add(AnExtensionItem)
            ExtensionsView.Add(AnExtensionItem)
        Else
            Throw New Exception("The template file '" & ExtensionXmlFile & "' is already in the list.")
        End If
    End Sub

    Public Sub Filter(ByVal Xpath As String)
        If Xpath = "" Then
            ExtensionsView.Clear()
            ExtensionsView.AddRange(Extensions)
        Else
            Dim Results As IEnumerable(Of ExtensionItem) = From item In Extensions _
                                                           Where item.ExtensionLoader.RenderedXmlDoc.XPathSelectElement(Xpath) IsNot Nothing _
                                                           Select item
            ExtensionsView.Clear()
            ExtensionsView.AddRange(Results)
        End If
    End Sub

    'Loads the extension's title attribute from the xml file.
    Public Function GetExtensionLabel(ByVal ExtensionXmlFile As String) As String
        Dim XDoc As XDocument
        Try
            XDoc = XDocument.Load(ExtensionXmlFile)
        Catch ex As XmlException
            Throw New Exception("Error loading xml file '" & ExtensionXmlFile & "' ." & ex.Message, ex)
        End Try

        Dim Extension As IEnumerable(Of XElement) = From elem In XDoc.XPathSelectElements("./extension")
                                                    Where elem.Attribute("title") IsNot Nothing
                                                    Select elem
        If Extension.Count() > 0 Then
            Return Extension.First.Attribute("title").Value
        Else
            Throw New Exception("This xml file '" & ExtensionXmlFile & "' is not a valid template file, the root element should have the name 'extension' and must have a title attribute.")
        End If
    End Function

    Public Function GetExtensionItem(ByVal ExtensionXmlFile As String) As ExtensionItem
        Dim Results As IEnumerable(Of ExtensionItem) = From items In Extensions
                                                       Where items.File = ExtensionXmlFile
                                                       Select items
        If Results.Count > 0 Then
            Return Results.First
        Else
            Throw New Exception(ExtensionXmlFile & " was not found in the list of Templates.")
        End If
    End Function

    Public Sub Remove(ByVal ExtensionItem As ExtensionItem)
        If Extensions.Contains(ExtensionItem) Then
            Extensions.Remove(ExtensionItem)
        End If
        If ExtensionsView.Contains(ExtensionItem) Then
            ExtensionsView.Remove(ExtensionItem)
        End If
    End Sub

    'CreateExtension creates an extension then returns it.
    'If the necessary data is found in the XElement, pass it to the class loader to load the extension.
    Public Function CreateExtension(ByVal ExtensionDocLoader As XmlTemplateLoader, ByVal Title As String) As Extension
        Dim ExtensionDoc As XElement = ExtensionDocLoader.RenderedXmlDoc
        Dim ExtensionXmlFile As String = ExtensionDocLoader.RenderedXmlFile
        Dim XElem As XElement
        Dim Result As Extension = Nothing
        Dim TemplateClass As String = ""
        Dim AssemblyFile As String = ""
        Dim schemalocation As XAttribute = ExtensionDoc.Attribute(GetXmlNamespace(xs) + "schemalocation")
        Dim schemafile As String = ""

        'TODO: schemalocation = nothing
        If schemalocation IsNot Nothing Then
            schemafile = schemalocation.Value
            If schemafile <> "" AndAlso Not Path.IsPathRooted(schemafile) Then
                schemafile = Path.GetDirectoryName(ExtensionDocLoader.TemplateXmlFile) & "\" & schemafile
            End If
            Dim schemaset As XmlSchemaSet = New XmlSchemaSet()
            Try
                schemaset.Add(Nothing, schemafile)
            Catch ex As XmlException
                Throw New Exception(String.Format("xml error: with schema file '{0}' : {1}", schemafile, ex.Message), ex)
            End Try

            schemaset.Compile()
            'todo: check that there were no errors
            If schemaset.IsCompiled Then
                'try to load the psvi into the xmldocument
                Dim exdoc As XDocument = New XDocument(ExtensionDoc)
                _SchemaErrors = 0
                'todo: error here
                exdoc.Validate(schemaset, AddressOf ValidationCallback, True)
                If Not _SchemaErrors = 0 Then
                    'todo: handle error here
                    MsgBox("schema validation errors.")
                Else
                    MsgBox("validated.")
                End If
            Else
                'todo: show the schema errors clearly...
                MsgBox("the schema is invalid. either read the errors and try to fix the problem or open the xml file without a schema.", MsgBoxStyle.Exclamation)
            End If
        End If

        XElem = ExtensionDoc.XPathSelectElement("./parameters/descendant::templateClass")
        If XElem IsNot Nothing Then
            TemplateClass = XElem.Value
        End If
        XElem = ExtensionDoc.XPathSelectElement("./parameters/descendant::assemblyFile")
        If XElem IsNot Nothing Then
            AssemblyFile = XElem.Value
        End If
        If TemplateClass = "" Or AssemblyFile = "" Then
            Throw New Exception("This xml file provided is not a valid template file, either the class or assembly file are missing.")
        End If
        If Not IsPathRooted(AssemblyFile) Then
            AssemblyFile = My.Application.Info.DirectoryPath & "\" & AssemblyFile
        End If

        Dim myExtension As Extension = Nothing
        If File.Exists(AssemblyFile) Then
            myExtension = classLoader("Extension", My.Application.Info.DirectoryPath & "\" & "ExtensionCreator.dll", _
                                                        TemplateClass, AssemblyFile, _
                                                        {ExtensionXmlFile, Title, ExtensionDocLoader})
        End If
        If Not IsNothing(myExtension) Then
            myExtension.UnRenderedExtensionXmlFile = ExtensionDocLoader.TemplateXmlFile
            Result = CType(myExtension, Extension)
        Else
            Dim sw As New StringWriter
            sw.Write("Problem loading template file {0} . Please make sure the class '{1}' and assemblyFile '{2}' are valid.", _
                     ExtensionDocLoader.TemplateXmlFile, TemplateClass, AssemblyFile)
            Throw New Exception(sw.ToString())
        End If
        Return Result
    End Function

    Private _SchemaErrors As Integer = 0
    Private Sub ValidationCallback(ByVal sender As Object, ByVal args As ValidationEventArgs)
        If args.Severity = XmlSeverityType.Warning Then
            Console.Write("WARNING: ")
            _SchemaErrors += 1
        Else
            If args.Severity = XmlSeverityType.Error Then
                Console.Write("ERROR: ")
                _SchemaErrors += 1
            End If
        End If
        Console.WriteLine(args.Message)
        MsgBox(args.Message)
    End Sub

    'Runs the extension chosen by the user then returns the output folder.
    'Make sure file exists before calling this.
    Public Function Run(ByVal ExtensionItem As ExtensionItem) As String
        Dim ExtensionDocLoader As XmlTemplateLoader = New XmlTemplateLoader(ExtensionItem.File)
        'ExtensionDocLoader.RenderedXmlDoc.Save("mytestloc.xml")
        Dim Extension As Extension = CreateExtension(ExtensionDocLoader, ExtensionItem.Label)

        Extension.Run()
        Return Extension.WorkingFolder
    End Function

    Public Sub Reset(ByVal ExtensionItem As ExtensionItem)
        Dim ExtensionDocLoader As XmlTemplateLoader = New XmlTemplateLoader(ExtensionItem.File)
        Dim Extension As Extension = CreateExtension(ExtensionDocLoader, ExtensionItem.Label)
        Extension.Reset()
    End Sub

    Public Sub Export(ByVal ExtensionItem As ExtensionItem)
        Dim ExtensionDocLoader As XmlTemplateLoader = New XmlTemplateLoader(ExtensionItem.File)
        Dim Extension As Extension = CreateExtension(ExtensionDocLoader, ExtensionItem.Label)
        Extension.Export()
    End Sub

    'Loads a child class ChildName of ParentName, with Params() being an array of parameters for the New
    'method of the class ChildName.
    Protected Function classLoader(ByVal ParentName As String, ByVal ParentAssemblyPath As String, _
                                ByVal ChildName As String, ByVal ChildAssemblyPath As String, ByVal Params() As Object) As Object
        Dim TempAssembly As [Assembly]
        Dim singleType As System.Type
        Dim ExtensionType As Type = Nothing
        Dim ChildInstance = Nothing
        TempAssembly = [Assembly].LoadFrom(ParentAssemblyPath)
        Dim TempTypes As System.Type() = TempAssembly.GetTypes
        For Each singleType In TempTypes
            If singleType.Name = ParentName Then
                ExtensionType = singleType
                Exit For
            End If
        Next
        If ExtensionType IsNot Nothing Then
            Dim myAssembly As [Assembly]
            myAssembly = [Assembly].LoadFrom(ChildAssemblyPath)
            Dim myTypes As System.Type() = myAssembly.GetTypes()
            For Each singleType In myTypes
                If ExtensionType.IsAssignableFrom(singleType) And singleType.Name = ChildName Then
                    ChildInstance = Activator.CreateInstance(singleType, Params)
                    Exit For
                End If
            Next
        End If
        Return ChildInstance
    End Function

End Class
