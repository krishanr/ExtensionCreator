Imports ExtensionCreator
Imports System.IO
Imports System.Xml
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports JoomlaForm
Imports System.Windows.Forms
Imports System.Xml.XPath
Imports System.Text.RegularExpressions
Imports System.Reflection
Imports System.Reflection.Assembly

'TTODO: Make this an abstract class
Public Class JExtension
    Inherits Extension
    Implements IFileProcessor

    Public LangFiles As New Dictionary(Of String, String)
    Public FormFiles As New ListDictionary
    Protected Forms As New ListDictionary
    Public LangKeyPrefix As String = ""

    Protected Const FormTitleAttribute As String = "formTitle"
    Protected Const OpenJformAttribute As String = "openJform"
    Protected Const FormSaveAttribute As String = "formSaveMethod"
    Protected Const FormFileAttribute As String = "formFile"
    Protected Const ContainsFormAttribute As String = "hasForm"
    Protected Const FormFieldsetAttribute As String = "formFieldset"

    Public Sub New(ByVal xmlfile As String, ByVal title As String, ByVal extensionDocLoader As XmlTemplateLoader)
        MyBase.New(xmlfile, title, extensionDocLoader)

        DerivedAssembly = Me.GetType().Assembly.Location
        DerivedNamespace = "JoomlaLibrary"
    End Sub

    Protected Overrides Sub loadFolders()
        ExtensionFilesXmlFile = ExtensionParameters.MyItem("templateFiles")
        If ExtensionFilesXmlFile = "" Then
            Throw New Exception("Extension parameters should contain ""templateFiles"".")
            ' --------- Commented code below is obsolete code, since templateFiles are public. --------
            'Dim ExtPrefix As String = ExtensionParameters.GetValue("extensionPrefix")
            'Dim assembly As Assembly = Reflection.Assembly.GetExecutingAssembly()
            'Dim ExtFilesName As String = ""
            'Select Case ExtPrefix
            '    Case "com"
            '        ExtFilesName = "JoomlaLibrary.component_files.xml"
            '    Case "mod"
            '        ExtFilesName = "JoomlaLibrary.module_files.xml"
            '    Case "plg"
            '        ExtFilesName = "JoomlaLibrary.plugin_files.xml"
            '    Case "lib"
            '        ExtFilesName = "JoomlaLibrary.library_files.xml"
            'End Select

            'ExtensionFilesXmlFile = Path.GetTempFileName()
            'Using sr As New StreamReader(assembly.GetManifestResourceStream(ExtFilesName))
            '    Using sw As New StreamWriter(ExtensionFilesXmlFile)
            '        sw.Write(sr.ReadToEnd)
            '    End Using
            'End Using
        End If
        MyBase.loadFolders()
    End Sub

    Protected Overrides Sub AddToExtensionParameters()
        MyBase.AddToExtensionParameters()

        If Not ExtensionParameters.ContainsKey("extensionPrefix") Then
            Throw New Exception("Extension parameters should contain ""extensionPrefix"".")
        End If
        Dim NamePrefix As String = ExtensionParameters.GetValue("extensionPrefix")

        'Generate the default name
        If Not ExtensionParameters.ContainsKey("label") Then
            Throw New Exception("Extension parameters should contain ""label"".")
        End If
        Dim Label As String = ExtensionParameters.GetValue("label")
        Dim Pattern As String = ""
        Select Case StrConv(ExtensionParameters.GetValue("nameType", "plain"), VbStrConv.Lowercase)
            Case "underscores"
                Pattern = "[^A-Z0-9\s]"
                Dim rgx As New Regex(Pattern, RegexOptions.IgnoreCase)
                Call ExtensionParameters.SetDefault("name", StrConv(rgx.Replace(Label, "").Replace(" ", "_"), VbStrConv.Lowercase))
            Case "camelcase"
                Pattern = "[^A-Z0-9]"
                Dim rgx As New Regex(Pattern, RegexOptions.IgnoreCase)
                Call ExtensionParameters.SetDefault("name", rgx.Replace(Label, ""))
                'plain falls into the else clause
            Case Else
                Pattern = "[^A-Z0-9]"
                Dim rgx As New Regex(Pattern, RegexOptions.IgnoreCase)
                Call ExtensionParameters.SetDefault("name", StrConv(rgx.Replace(Label, ""), VbStrConv.Lowercase))
        End Select

        If NamePrefix <> "" Then
            Dim FullName As String = NamePrefix & "_" & StrConv(ExtensionParameters.MyItem("name"), VbStrConv.Lowercase)
            ExtensionParameters.SetDefault("fullName", FullName)
        End If
        If ExtensionParameters.GetValue("group") <> "" Then
            LangKeyPrefix = StrConv(NamePrefix & "_" & ExtensionParameters.MyItem("group") & "_" & ExtensionParameters.MyItem("name") & "_", VbStrConv.Uppercase)
        Else
            LangKeyPrefix = StrConv(ExtensionParameters.MyItem("fullName") & "_", VbStrConv.Uppercase)
        End If
    End Sub

    Public Overrides Sub Run()
        MyBase.Run()
        CreateArchive()
    End Sub

#Region "Task Start and End functions"
    Public Overridable Function StartLanguage() As Exception
        Return Nothing
    End Function

    Public Overridable Function EndLanguage() As Exception
        Return Nothing
    End Function
#End Region

#Region "Functions called from files"

    Public Function CreateIndexHtmlFile(ByVal TempTemplateFile As String, ByVal FileNode As XElement)
        Try
            TempTemplateFile = My.Computer.FileSystem.GetTempFileName()
            'TODO: TempTemplateFile never gets deleted...
            'TODO: FILE double check that new code works. See commented code below for old one.
            'Dim myTempFile = File.CreateText(TempTemplateFile) 'myTempFileSystem.CreateTextFile(TempTemplateFile, True)
            Using myTempFile As StreamWriter = File.CreateText(TempTemplateFile)
                myTempFile.WriteLine("<html><body bgcolor='#FFFFFF'></body></html>")
                'myTempFile.Close()
            End Using

            Return TempTemplateFile
        Catch ex As Exception
            TempTemplateFile = ""
            Return ex
        End Try
    End Function

#End Region

#Region "Utility Functions"

    'Takes the language key, the default text value for that key and the
    'language file to append the key, value pair to (relative to WorkingFolder).
    'Checks to see if the key already occurs in the file, if it does the text is NOT added.
    'Adds the key,text pair to the language file, then returns the key.
    'Note: Can be called weather or not File exists, aslong as it can be created (valid file).
    Public Function GenText(ByVal key As String, ByVal text As String, ByVal File As String) As String
        key = Replace(StrConv(key, vbUpperCase), " ", "_")
        Dim fileExists As Boolean
        fileExists = My.Computer.FileSystem.FileExists(WorkingFolder & File)
        If fileExists Then
            Using sr As New StreamReader(WorkingFolder & File)
                Do While sr.Peek() >= 0
                    Dim CurrentKey As String = StrConv(sr.ReadLine(), vbUpperCase)
                    If CurrentKey.StartsWith(key & "=") Then
                        Return key
                    End If
                Loop
            End Using
        End If
        Using sw As New StreamWriter(WorkingFolder & File, True)
            sw.WriteLine("{0}=""{1}""", key, text)
        End Using
        Return key
    End Function

    'Takes the language key and the default text value for that key.
    'Deduces the language file based on the context.
    'Adds the key,text pair to the language file, then returns the key.
    'Note: prepends LangKeyPrefix to the given key
    'Note: Must create language files using _files.xml in the correct way for this to work.
    Public Function GenText(ByVal key As String, ByVal text As String) As String
        Dim CurrentLangFile As String = ""
        Dim ActiveFileElem As XElement = DirMgr.ActiveXmlFileElem
        key = LangKeyPrefix & key
        'TODO: language files not added? can we handle exceptions from template files?
        'TODO:throw an exception if ActiveFileElem doesn't have a correct langFile tag...
        'TODO: need to complete admin.sys, not sure which files send lang tags to this
        Return GenText(key, text, LangFiles.Item(ActiveFileElem.Attribute("langFile").Value))
    End Function

    Public Function GenText(ByVal key As String) As String
        Return GenText(key, "")
    End Function

    Private _myForm As frmJForm
    'Should be called after the file has been rendered. i.e. requires a valid xml file.
    Public Sub OpenJForm(ByVal FileNode As XElement, ByVal AbsFilePath As String)
        _myForm = GetJForm()
        If FileNode.Attribute(FormTitleAttribute) IsNot Nothing Then _myForm.Text = FileNode.Attribute(FormTitleAttribute).Value
        If FileNode.Attribute(FormSaveAttribute) IsNot Nothing Then _myForm.SaveMethod = Val(FileNode.Attribute(FormSaveAttribute).Value)

        Try
            _myForm.LoadForm(AbsFilePath)
            'Auto save to this file
            _myForm.OutputFileName = AbsFilePath
        Catch ex As Exception
            MsgBox("Unable to load '" & AbsFilePath & "' as a JForm object. Error: " & ex.Message, MsgBoxStyle.Exclamation, "Error opening file")
        End Try
        FillJform(_myForm, FileNode)

        'Open the JForm from the GUI thread
        OnInteractUser(AddressOf JFormShowDialog)
    End Sub

    Private _frmJFormType As Type = Nothing
    'This function loads the frmJFormEditor editor from JForm.exe if JForm.exe
    'is included in the currenty executing path.
    Protected Function GetJForm() As frmJForm

        If _frmJFormType IsNot Nothing Then
            Return CType(Activator.CreateInstance(_frmJFormType), frmJForm)
        End If
        Dim JFormAssemblyPath As String = My.Application.Info.DirectoryPath & "\JForm.exe"
        Dim myAssembly As [Assembly]

        If File.Exists(JFormAssemblyPath) Then
            myAssembly = [Assembly].LoadFrom(JFormAssemblyPath)

            Dim myTypes As System.Type() = myAssembly.GetTypes()
            For Each singleType In myTypes
                If GetType(frmJForm).IsAssignableFrom(singleType) And singleType.Name = "frmJFormEditor" Then
                    'Save this result for the next use
                    _frmJFormType = singleType
                    Return CType(Activator.CreateInstance(singleType), frmJForm)
                    Exit For
                End If
            Next
        End If

        'If no extensions were found, return the default form.
        Return New frmJForm
    End Function

    'Requires private myForm variable to be initialized
    Protected Function JFormShowDialog()
        Dim result As Boolean = False
        Try
            If _myForm.showDialogEnabled AndAlso _myForm.ShowDialog() = DialogResult.OK Then
                result = True
            ElseIf _myForm.showDialogEnabled Then
                'TODO: (optional) maybe put this in an event log..., this is unusual
            Else
                'Show dialog is not enabled.
            End If
        Catch ex As Exception
            'Very unusual, show a message, then move on.
            MsgBox(ex.Message, MsgBoxStyle.Exclamation, "Jform Error")
        Finally
            'Save loaded form, manually since show dialog is disabled.
            If Not _myForm.showDialogEnabled Then
                _myForm.Save()
                result = True
            End If
            _myForm.Dispose()
        End Try
        _myForm = Nothing
        Return result
    End Function

    Protected Overridable Sub FillJform(ByRef myForm As frmJForm, ByVal FileNode As XElement)
        'More sepcialized extensions like components will override this.
    End Sub

    'Important: The form xml file must have been loaded into memory before this can be called.
    'If FieldSetName is not specified the form file element should have the FormFieldsetAttribute set to the default fieldset.
    'Pass just the FieldName to retrieve the default value (FieldValue) for that field from the form.
    'ContainsFormAttribute should be unique
    Public Function AddParam(ByVal FieldName As String, Optional ByVal FieldValue As String = "", Optional ByVal FieldType As String = "text", _
                             Optional ByVal Description As String = "", Optional ByVal FieldSetName As String = "", Optional ByVal FormKey As String = "")
        Try
            Dim FileName As String = ""
            If FormKey = "" Then
                FormKey = DirMgr.ActiveXmlFileElem.Attribute(FormFileAttribute).Value
            End If
            If FieldType = "" Then
                FieldType = "text"
            End If

            'TODO: this might cause poor preformance...
            Dim FormElem As XElement = (From item In DirMgr.DirectoryXmlDoc.Descendants _
                                        Where item.Attribute(ContainsFormAttribute) IsNot Nothing _
                                        AndAlso item.Attribute(ContainsFormAttribute).Value = FormKey _
                                        Select item).FirstOrDefault

            If FormElem Is Nothing Then
                Throw New Exception("Form element with the specified key not found in the directory xml document.")
            Else
                FileName = FormElem.Attribute(DirectoryManager.FilePathAttribute).Value & "\" & FormElem.Value
            End If

            If FieldSetName = "" Then
                FieldSetName = FormElem.Attribute(FormFieldsetAttribute).Value
            End If

            If Not Forms.Contains(FileName) Then
                'Forms.Add(FileName, New form)
                Throw New Exception("The form was not loaded, please make sure AddParam is called after the appropriate form was processed.")
            End If

            Dim Form As JoomlaForm.form = Forms.Item(FileName)

            Dim Fieldsets As IEnumerable(Of JFieldset) = From item In Form.FieldSets
                                                         Where item.Name = FieldSetName
                                                         Select item
            Dim FieldSet As JFieldset
            If Fieldsets.Count = 0 Then
                FieldSet = New JFieldset With {
                    .Name = FieldSetName
                }
                Form.FieldSets.Add(FieldSet)
            Else
                FieldSet = Fieldsets.First
            End If

            'TODO: optional. should store the fields in the fieldsets as a better ordered list...
            Dim AField As JField
            If FieldSet.Fields.Count > 0 Then
                AField = (From item In FieldSet.Fields _
                          Where item.Name = FieldName
                          Select item).FirstOrDefault
            Else
                AField = Nothing
            End If

            If AField Is Nothing Then
                Dim myField As JField = Activator.CreateInstance("JoomlaForm", "JoomlaForm." & FieldType).Unwrap
                myField.Name = FieldName
                myField.DefaultValue = FieldValue
                myField.Description = Description
                FieldSet.Fields.Add(myField)
            Else
                'Get the old default value for return method
                FieldValue = AField.DefaultValue
                'TODO: do something about this: the default value that they may provide gets ignored...
                'TODO: optional. For now lets ignore this, but can update the field.
            End If

            Dim sw As New StringWriter
            sw.Write("'{0}' , '{1}'", FieldName, FieldValue)
            Return sw.ToString()
        Catch ex As Exception
            Dim sw As New StringWriter
            sw.WriteLine("Error adding field '{0}' using key '{1}': {2}", FieldName, FormKey, ex.Message)
            Throw New Exception(sw.ToString(), ex)
        End Try
    End Function

#End Region

#Region "File Processing"
    Public Overloads Sub ProcessFileNode(ByVal FileNode As XElement, ByVal TemplateFolder As String, ByVal OutputFolder As String, ByVal FilePathAttribute As String, Optional ByVal templateFileLoc As String = "") Implements IFileProcessor.ProcessFileNode
        Dim RelFilePath As String = FileNode.Attribute(DirectoryManager.FilePathAttribute).Value & "\" & FileNode.Value
        Dim AbsFilePath As String = OutputFolder & RelFilePath

        If GetActiveTaskName() = "Language" AndAlso (FileNode.Attribute("type") IsNot Nothing) Then
            'Handle language files
            LangFiles.Add(FileNode.Attribute("type").Value, RelFilePath)
            MyBase.ProcessFileNode(FileNode, TemplateFolder, OutputFolder, FilePathAttribute, templateFileLoc)
        ElseIf (FileNode.Attribute("install") IsNot Nothing) Then
            Dim InstallXmlDoc As XDocument = Nothing
            'Handle install file
            If My.Computer.FileSystem.FileExists(AbsFilePath) Then
                'This must be an update, don't process the install file.
                NewExtension = False
            Else
                'New extension, process the install file.
                NewExtension = True
                MyBase.ProcessFileNode(FileNode, TemplateFolder, OutputFolder, FilePathAttribute, templateFileLoc)
            End If
            Try
                InstallXmlDoc = XDocument.Load(AbsFilePath)
            Catch ex As Exception
                InstallXmlDoc = Nothing
                MsgBox("Problem loading install file: " & ex.Message)
            End Try

            If (InstallXmlDoc IsNot Nothing) AndAlso (Not NewExtension) Then
                Try
                    'Update old versions of the install file
                    If StrConv(ExtensionParameters.GetValue("upgrade", "false"), VbStrConv.Lowercase) = "true" AndAlso _
                        InstallXmlDoc.XPathSelectElement("./install") IsNot Nothing Then
                        RefactorOldInstallFiles(InstallXmlDoc)
                    End If

                    If InstallXmlDoc.XPathSelectElement("./install") IsNot Nothing Then
                        'TODO: better error message
                        MsgBox("Error the install file is of an older version")
                    End If

                    Dim InstallVersionElem As XElement = InstallXmlDoc.XPathSelectElement("./extension/version")
                    Dim InstallFileVersion As String() = Split(InstallVersionElem.Value, ".")
                    Dim InstallSubVersion As Integer = CInt(Val(InstallFileVersion(InstallFileVersion.Length - 1))) + 1
                    InstallFileVersion(InstallFileVersion.Length - 1) = CStr(InstallSubVersion)

                    If ExtensionParameters.ContainsKey("version") Then
                        ExtensionParameters.Item("version") = Join(InstallFileVersion, ".")
                    Else
                        ExtensionParameters.Add("version", Join(InstallFileVersion, "."))
                    End If
                    InstallVersionElem.Value = ExtensionParameters.Item("version")
                Catch ex As Exception
                    MsgBox("Error trying to update version number: " & ex.Message)
                End Try
            End If
            Try
                InstallXmlDoc.Save(AbsFilePath)
            Catch ex As Exception
                MsgBox("Problem saving install file: " & ex.Message)
            End Try
        Else
            MyBase.ProcessFileNode(FileNode, TemplateFolder, OutputFolder, FilePathAttribute, templateFileLoc)
        End If

        'Open Jform if specified.
        If ParseBooleanAttribute(FileNode, OpenJformAttribute) Then
            'Handle xml files that have a JForm form
            If (TaskRan AndAlso _
                StrConv(ExtensionParameters.GetValue("showFormForExistingExtension", "true"), VbStrConv.Lowercase) = "true") Or _
                Not TaskRan Then
                OpenJForm(FileNode, AbsFilePath)
            End If
        End If

        'Load in any forms so fields can be added on demand.
        If FileNode.Attribute(ContainsFormAttribute) IsNot Nothing Then
            Dim myForm As New JoomlaForm.form
            myForm.Load(AbsFilePath)
            Forms.Add(RelFilePath, myForm)
        End If
    End Sub
#End Region

#Region "Helper Functions"

    Protected Sub RefactorOldInstallFiles(ByRef InstallXmlDoc As XDocument)
        Dim Root As XElement = InstallXmlDoc.XPathSelectElement("./install")
        If Root IsNot Nothing Then
            Root.Name = "extension"
            Root.SetAttributeValue("version", ExtensionParameters.MyItem("joomlaVersion"))
            Root.SetAttributeValue("method", ExtensionParameters.MyItem("intallMethod"))

            If ExtensionParameters.GetValue("extensionType") = "module" Then
                Root.SetAttributeValue("client", ExtensionParameters.MyItem("client"))
            End If
        End If
        JParametersToJForms(InstallXmlDoc)
    End Sub

    'TODO: optional. maybe put this as a method in the JForms library?
    Protected Sub JParametersToJForms(ByRef InstallXmlDoc As XDocument)
        'Dim Params As XElement = InstallXmlDoc.XPathSelectElement("//params")
        Dim FirstParamSet As Boolean = True
        Dim ParamSets As IEnumerable(Of XElement) = InstallXmlDoc.XPathSelectElements("//params")
        Dim Config As XElement = Nothing

        If ParamSets IsNot Nothing Then
            For Each Params As XElement In ParamSets
                If FirstParamSet = True Then
                    Config = New XElement(<config><fields name="params"></fields></config>)
                    Params.AddBeforeSelf(Config)
                    FirstParamSet = False
                End If
                'Replace all occurences of the attribute addpath with addfieldpath
                Dim NodesWithAddPathAttribute As IEnumerable(Of XElement) = Params.XPathSelectElements("//*[@addpath]")
                For Each NodeWithAddPathAttribute As XElement In NodesWithAddPathAttribute
                    'Add the addfieldpath attribute
                    NodeWithAddPathAttribute.SetAttributeValue("addfieldpath", NodeWithAddPathAttribute.Attribute("addpath").Value)
                    'Remove the addpath attribute from the node
                    NodeWithAddPathAttribute.SetAttributeValue("addpath", Nothing)
                Next

                'Ading the Params to fields creates a copy of the element
                Config.Element("fields").Add(Params)

                'Now lets work with the copy of the Params element in fields, so that
                'we can delete the old params element later.
                Dim item As XElement = Config.Element("fields").Element("params")

                item.Name = "fieldset"
                Dim FieldsetName As String = "basic"
                If item.Attribute("group") IsNot Nothing Then
                    FieldsetName = item.Attribute("group").Value
                    item.SetAttributeValue("group", Nothing)
                End If

                item.SetAttributeValue("name", FieldsetName)

                Dim ParamNodes As IEnumerable(Of XElement) = item.XPathSelectElements("//param")
                For Each ParamNode As XElement In ParamNodes
                    ParamNode.Name = "field"
                Next
            Next

            'Remove the old params elements
            InstallXmlDoc.XPathSelectElements("//params").Remove()
        End If
    End Sub

    Public Sub ListDirectoryInfo(ByVal DirectoryName As String, ByVal XmlDoc As XmlDocument, ByVal XPath As String, Optional ByVal IgnoreList As List(Of String) = Nothing)
        Dim XmlNode As XmlNode
        Dim XmlElem As XmlElement

        If IgnoreList Is Nothing Then
            IgnoreList = New List(Of String)
        End If

        XmlNode = XmlDoc.SelectSingleNode(XPath)
        If XmlNode IsNot Nothing Then
            If TypeOf XmlNode Is XmlElement Then
                XmlElem = CType(XmlNode, XmlElement)
                'TODO: FILE double check
                If Directory.Exists(DirectoryName) Then
                    Dim Directories As DirectoryInfo() = New DirectoryInfo(DirectoryName).GetDirectories()
                    For Each Dir As DirectoryInfo In Directories
                        If Not IgnoreList.Contains(Dir.Name) Then
                            Call InsertTextNode(XmlDoc, XmlElem, "folder", Dir.Name)
                        End If
                    Next
                    Dim Files As FileInfo() = New DirectoryInfo(DirectoryName).GetFiles()
                    For Each File As FileInfo In Files
                        If Not IgnoreList.Contains(File.Name) Then
                            Call InsertTextNode(XmlDoc, XmlElem, "filename", File.Name)
                        End If
                    Next
                End If
            End If
        End If
    End Sub

    'Should only be called by the finish method
    'Lists all files and folders in the directory to the xpath location in xDoc.
    Public Sub ListDirectoryInfo(ByVal DirectoryToList As XElement, ByVal xDoc As XDocument, ByVal XPath As String, Optional ByVal IgnoreList As List(Of String) = Nothing, Optional ByVal DeletePrev As Boolean = True)
        If IgnoreList Is Nothing Then
            IgnoreList = New List(Of String)
        End If

        Dim xLocation As XElement = xDoc.XPathSelectElement(XPath)

        If xLocation IsNot Nothing Then
            If DeletePrev Then
                xLocation.RemoveNodes()
            End If
            For Each elem As XElement In DirectoryToList.Elements("folder")
                If Not IgnoreList.Contains(elem.Attribute("name").Value) Then
                    xLocation.Add(<folder><%= elem.Attribute("name").Value %></folder>)
                End If
            Next
            For Each elem As XElement In DirectoryToList.Elements("file")
                If Not IgnoreList.Contains(elem.Value) Then
                    Select Case ExtensionParameters.GetValue("extensionType")
                        Case "plugin"
                            If elem.Value = ExtensionParameters.Item("name") & ".php" Then
                                xLocation.Add(<filename plugin=<%= ExtensionParameters.MyItem("name") %>><%= elem.Value %></filename>)
                            Else
                                xLocation.Add(<filename><%= elem.Value %></filename>)
                            End If
                        Case "module"
                            If elem.Value = ExtensionParameters.MyItem("fullName") & ".php" Then
                                xLocation.Add(<filename module=<%= ExtensionParameters.MyItem("fullName") %>><%= elem.Value %></filename>)
                            Else
                                xLocation.Add(<filename><%= elem.Value %></filename>)
                            End If
                        Case Else
                            xLocation.Add(<filename><%= elem.Value %></filename>)
                    End Select
                End If
            Next
        End If
    End Sub

    Public Function InsertTextNode(ByVal xDoc As XmlDocument, ByVal xNode As XmlNode, ByVal strTag As String, ByVal strText As String) As XmlElement
        ' Insert a text node as a child of xNode.
        ' Set the tag to be strTag, and the
        ' text to be strText. Return a pointer
        ' to the new node.
        Dim xNodeTemp As XmlNode

        xNodeTemp = xDoc.CreateElement(strTag)
        xNodeTemp.AppendChild(xDoc.CreateTextNode(strText))
        xNode.AppendChild(xNodeTemp)

        Return CType(xNodeTemp, XmlElement)
    End Function

    'Overwrites OutputFileName with a formatted version of XmlDoc, make sure OutputFileName 
    'is a valid file.
    Public Sub SaveXmlDoc(ByVal XmlDoc As XmlDocument, ByVal OutputFileName As String)
        Dim XmlWriterSettings As New XmlWriterSettings With {
            .Indent = True,
            .IndentChars = (ControlChars.Tab),
            .NewLineOnAttributes = True
        }

        Using XmlWriter As XmlWriter = XmlWriter.Create(OutputFileName, XmlWriterSettings)
            XmlDoc.Save(XmlWriter)
        End Using
    End Sub

#End Region

    'Should only be called by the finish method.
    Protected Sub StoreForms()
        Dim FormNodes As IEnumerable(Of XElement) = From item In DirMgr.DirectoryXmlDoc.Descendants _
                                                    Where item.Attribute(ContainsFormAttribute) IsNot Nothing
        For Each FormEntry As DictionaryEntry In Forms
            Dim RelFormFilePath As String = ""
            Dim AbsFormFilePath As String = ""

            Try
                RelFormFilePath = CStr(FormEntry.Key)
                AbsFormFilePath = WorkingFolder & RelFormFilePath

                Dim myForm As JoomlaForm.form = FormEntry.Value
                Dim SaveMethod As Integer

                'Should always get a result since Forms is populated using these XElements.
                Dim FormElem As XElement = (From item In FormNodes _
                                            Where item.Attribute(DirectoryManager.FilePathAttribute).Value & "\" & item.Value = RelFormFilePath _
                                            Select item).FirstOrDefault

                If FormElem.Attribute(FormSaveAttribute) IsNot Nothing Then
                    SaveMethod = Val(FormElem.Attribute(FormSaveAttribute).Value)
                Else
                    SaveMethod = JoomlaForm.form.SaveMethod.Context
                End If
                'myForm.Load(AbsFormFilePath, True)
                myForm.Save(SaveMethod, AbsFormFilePath)
            Catch ex As Exception
                Dim sw As New StringWriter
                sw.WriteLine("Error: Unable to save form '{0}': {1}", RelFormFilePath, ex.Message)
                MsgBox(sw.ToString())
            End Try
        Next
    End Sub
    'Adds folders/files to the install file, deleting any that exist already.
    'Requires the InstallXmlDoc to have been loaded by process file node.
    Public Overrides Sub Finish()

        'Save any forms that were loaded previously.
        StoreForms()

        Dim InstallXmlDoc As XDocument = Nothing
        Dim XmlDoc As New XmlDocument
        Dim IgnoreList As New List(Of String)

        Dim InstallFiles As IEnumerable(Of XElement) = From elem In DirMgr.DirectoryXmlDoc.Descendants _
                                                       Where elem.Attribute("install") IsNot Nothing _
                                                       Select elem

        Dim DirectoriesToList As IEnumerable(Of XElement) = From elem In DirMgr.DirectoryXmlDoc.Descendants _
                                                            Where elem.Attribute("listContentsTo") IsNot Nothing _
                                                            AndAlso String.Compare(elem.Attribute("listContentsTo").Value, "false", True) <> 0 _
                                                            Select elem

        If InstallFiles.Count() > 0 Then
            Dim InstallFileElem As XElement = InstallFiles.First
            Dim InstallFile As String = DirMgr.OutputFolder & InstallFileElem.Attribute(DirectoryManager.FilePathAttribute).Value & "\" & InstallFileElem.Value

            If My.Computer.FileSystem.FileExists(InstallFile) Then

                Try
                    InstallXmlDoc = XDocument.Load(InstallFile)
                Catch ex As Exception
                    MsgBox("Problem loading install file: " & ex.Message)
                    InstallXmlDoc = Nothing
                End Try

                If InstallXmlDoc IsNot Nothing Then
                    'TODO: optional. this will ignore the file name for all Descendants...
                    'IgnoreList.Add(InstallFileElem.Value)

                    For Each DirectoryToList As XElement In DirectoriesToList
                        'Currently haven't tested when files are listed recursively.
                        'TODO: just get ListDirectoryInfo to check if each node has listContentsTo set to false.
                        Dim IgnoreableItems As IEnumerable(Of String) = From elem In DirectoryToList.Descendants _
                                                                        Where (elem.Attribute("listContentsTo") IsNot Nothing _
                                                                        AndAlso String.Compare(elem.Attribute("listContentsTo").Value, "false", True) = 0) _
                                                                        Or elem.Attribute("install") IsNot Nothing
                                                                        Select elem.Value

                        IgnoreList = IgnoreableItems.ToList()
                        'Read in any new files/folders that might not have been added during ReadFileDirectory.
                        'Note: this won't work properly if you tried to list all sub-folders/files since these new nodes
                        '      will not have filepaths...
                        DirMgr.ReadInNodes(DirectoryToList)
                        ListDirectoryInfo(DirectoryToList, InstallXmlDoc, DirectoryToList.Attribute("listContentsTo").Value, IgnoreList, True)
                    Next

                    InstallXmlDoc.Save(InstallFile)

                    'Load the InstallFile as an XmlDocument
                    XmlDoc.Load(InstallFile)
                    SaveXmlDoc(XmlDoc, InstallFile)

                End If
            End If
        End If
    End Sub

End Class
