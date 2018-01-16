Imports System.Xml
Imports System.Xml.XPath
Imports System.Collections.Specialized
Imports System.IO
Imports System.IO.Path
'Imports System.IO.Compression
Imports Ionic.Zip

Public Class DirectoryManager

    Public TemplateFolder As String
    Public OutputFolder As String
    Public FileProcessor As IFileProcessor
    Public Const ProcessFileAttribute As String = "processFile"

    Public Const DirectoryXmlDocRootElement As String = "templateFiles"

    'Archive vars
    Public ArchiveAttribute As String = "archiveAs"
    Public Const FilePathAttribute As String = "FilePath"
    Public Const ArchivePathAttribute As String = "ArchivePath"
    Public ArchiveName As String
    Public ArchiveDirectory As String
    'Directory vars
    Public DirectoryXmlDoc As New XElement("templateFiles")
    'The _files.xml file before it has been processed into a valid xml file.
    'User for error messages.
    Public DirectoryModelFile As String
    Public DirectoryModelXmlFile As String
    Public DirectoryModelXmlDoc As XElement
    Public ActiveFilePath As String = ""
    Public ActiveXmlFileElem As XElement = Nothing
    Protected inheritableAttributes As New List(Of String)

    Public Enum ProcessingMode
        UseFileProcessor = 2
        ArchiveFiles = 3
    End Enum

    'Initalizes the local myFileSystem variable.
    Public Sub New(ByVal m_DirectoryModelFile As String, ByVal m_TemplateFolder As String, _
                   ByVal m_OutputFolder As String, ByVal m_ArchiveDirectory As String)
        DirectoryModelFile = m_DirectoryModelFile
        TemplateFolder = m_TemplateFolder
        OutputFolder = m_OutputFolder
        ArchiveDirectory = m_ArchiveDirectory
    End Sub

    'Requires a valid xml template file (i.e. all placeholders replaced).
    'Loads the xmlFile into the protected xmlDocument, then selects the templateDirectory and
    'outputDirectory from the xmlFile
    Public Sub LoadDirectoryModel(ByVal m_DirectoryModelXmlFile As String)
        DirectoryModelXmlFile = m_DirectoryModelXmlFile
        Try
            DirectoryModelXmlDoc = XElement.Load(DirectoryModelXmlFile)
        Catch ex As Exception
            Throw New Exception("Xml Parse error in file " & DirectoryModelXmlFile & " ." & ex.Message, ex)
        End Try

        SetValueFromXml("ArchiveName")
        Dim Results As IEnumerable(Of XName) = From elem In DirectoryModelXmlDoc.Descendants("inheritableAttributes").Descendants _
                                                Select elem.Name
        For Each Result As XName In Results
            inheritableAttributes.Add(Result.ToString)
        Next
    End Sub

    Public Sub SetValueFromXml(ByVal var As String)
        Dim xmlElem As XElement
        xmlElem = DirectoryModelXmlDoc.Descendants(var).FirstOrDefault
        If Not IsNothing(xmlElem) Then
            CallByName(Me, var, CallType.Set, xmlElem.Value)
        Else
            Throw New Exception(var & " not found in file " & DirectoryModelFile)
        End If
    End Sub

    'Requires ReadFileDirectory to have been called atleast once, otherwise an exception is thrown.
    'Requires the task Identifier to have been read in using ReadFileDirectory before this can work.
    'Requires the file processer to have been set.
    Public Sub CreateFileDirectory(ByVal Identifier As String, ByVal TemplateFolder As String, ByVal OutputFolder As String, _
                                   ByVal FilePathAttribute As String, Optional ByVal Mode As ProcessingMode = ProcessingMode.UseFileProcessor)

        Dim Results As IEnumerable(Of XElement) = From Elem In DirectoryXmlDoc.Elements _
                                                  Where Elem.Attribute("id") = Identifier _
                                                  Select Elem
        Dim Files As XElement = Results.First

        If (Files IsNot Nothing) Then
            TraverseDirectory(Files, TemplateFolder, OutputFolder, FilePathAttribute, Mode)
        Else
            Dim sw As New StringWriter
            sw.Write("Could not find files for task identifier {0} in file {1}", Identifier, DirectoryModelFile)
            Throw New Exception(sw.ToString())
        End If
    End Sub

    Protected Sub TraverseDirectory(ByVal ParentElem As XElement, ByVal TemplateFolder As String, ByVal OutputFolder As String, _
                                    ByVal FilePathAttribute As String, ByVal Mode As ProcessingMode)
        'Process the file nodes
        For Each FileNode As XElement In ParentElem.Elements("file")
            If Mode = ProcessingMode.UseFileProcessor Then
                If ((FileNode.Attribute(ProcessFileAttribute) IsNot Nothing) AndAlso _
                   (String.Compare(FileNode.Attribute(ProcessFileAttribute).Value, "false", True) <> 0)) Or _
                    (FileNode.Attribute(ProcessFileAttribute) Is Nothing) Then
                    ActiveXmlFileElem = FileNode
                    Call FileProcessor.ProcessFileNode(FileNode, TemplateFolder, OutputFolder, FilePathAttribute)
                    ActiveXmlFileElem = Nothing
                End If
            ElseIf Mode = ProcessingMode.ArchiveFiles Then
                ArchiveFile(FileNode, TemplateFolder, OutputFolder)
            Else
                Throw New Exception("Unknown mode passed to DirectoryManager:TraverseDirectory")
            End If
        Next

        'Process the folder nodes
        For Each FolderNode As XElement In ParentElem.Elements("folder")
            If FolderNode.Attribute(FilePathAttribute) IsNot Nothing Then
                Dim AbsFilePath As String = OutputFolder & FolderNode.Attribute(FilePathAttribute).Value
                If Not Directory.Exists(AbsFilePath) Then
                    'TODO: FILE double check code
                    Directory.CreateDirectory(AbsFilePath)
                End If
            End If
            Call TraverseDirectory(FolderNode, TemplateFolder, OutputFolder, FilePathAttribute, Mode)
        Next
    End Sub

    'Requires LoadDirectoryModel to have been called with a valid xml file before this can work.
    'Given TaskName and the identifier for the task instance
    'this subroutine will process the (single) xml element returned by XpathLocation. Then it will import 
    'the processed element from DirectoryModelXmlDoc into DirectoryXmlDoc where the xml element
    'will now have the identifer tag set to the one given as a parameter.
    Public Sub ReadFileDirectory(ByVal TaskName As String, ByVal Identifier As String)

        Dim Results As IEnumerable(Of XElement) = From Elem In DirectoryModelXmlDoc.Elements _
                                                  Where Elem.Attribute("for") = TaskName _
                                                  Select Elem

        If Results.Count > 0 Then
            Dim Files As XElement = Results.First
            'InheritAttributes(Files)
            TraverseDirectoryModel(Files)
            Files.SetAttributeValue("id", Identifier)
            DirectoryXmlDoc.Add(Files)
        Else
            Dim sw As New StringWriter
            sw.Write("Could not find files for task {0} in file {1}", TaskName, DirectoryModelFile)
            Throw New Exception(sw.ToString())
        End If
    End Sub

    'Recursive function that will inherit attributes from xElem into its child nodes, 
    'create a file path recursively, an archive path recursively, and adds any files
    'specified by the folder automatically.
    Protected Sub TraverseDirectoryModel(ByVal ParentElem As XElement)
        'Set the parent elements file path (is not a folder on the first call).
        Dim ParentElemFilePath As String = ""
        If ParentElem.Attribute("FilePath") IsNot Nothing Then
            ParentElemFilePath = ParentElem.Attribute("FilePath").Value
        End If

        'Add any files to this folder defined in the node.
        'TODO: Optional. weird exception will be thrown if the attributes don't have names
        If ParentElem.Attribute("addFile") IsNot Nothing Then
            Dim FileName As String = ParentElem.Attribute("addFile").Value
            Dim Results As IEnumerable(Of XElement) = From Elem In DirectoryModelXmlDoc.Elements("fileDefinitions").Descendants _
                                                      Where Elem.Attribute("name").Value = FileName _
                                                      Select Elem
            If Results.Count > 0 Then
                ParentElem.Add(Results)
            Else
                Dim sw As New StringWriter
                sw.Write("File name {0} not found in file {1}", FileName, DirectoryModelFile)
                Throw New Exception(sw.ToString())
            End If

        End If

        'Read in any child nodes from the file system that don't already exist.
        If (ParentElem.Attribute("readInNodes") IsNot Nothing) AndAlso _
            (String.Compare(ParentElem.Attribute("readInNodes").Value, "false", True) <> 0) Then
            ReadInNodes(ParentElem)
        End If

        'Process the file nodes
        For Each FileNode As XElement In ParentElem.Elements("file")
            FileNode.SetAttributeValue("FilePath", ParentElemFilePath)
            If FileNode.Attribute(ArchiveAttribute) IsNot Nothing Then
                If FileNode.Attribute(ArchiveAttribute).Value <> "" Then
                    FileNode.SetAttributeValue(ArchivePathAttribute, "\" & FileNode.Attribute(ArchiveAttribute).Value)
                Else
                    FileNode.SetAttributeValue(ArchivePathAttribute, "")
                End If
            ElseIf ParentElem.Attribute(ArchivePathAttribute) IsNot Nothing Then
                FileNode.SetAttributeValue(ArchivePathAttribute, ParentElem.Attribute(ArchivePathAttribute).Value)
            End If
            InheritAttributes(FileNode)
        Next

        'Process the folder nodes
        For Each FolderNode As XElement In ParentElem.Elements("folder")
            If FolderNode.Attribute("name") IsNot Nothing Then
                FolderNode.SetAttributeValue("FilePath", ParentElemFilePath & "\" & FolderNode.Attribute("name").Value)
                If FolderNode.Attribute(ArchiveAttribute) IsNot Nothing Then
                    'Note: The path specified must not begin with a slash
                    FolderNode.SetAttributeValue("ArchivePath", "\" & FolderNode.Attribute(ArchiveAttribute).Value)
                ElseIf ParentElem.Attribute("ArchivePath") IsNot Nothing Then
                    FolderNode.SetAttributeValue("ArchivePath", ParentElem.Attribute("ArchivePath").Value & "\" & FolderNode.Attribute("name").Value)
                End If
                InheritAttributes(FolderNode)
                TraverseDirectoryModel(FolderNode)
            Else
                Dim sw As New StringWriter
                sw.Write("Found folder without a name in file {1}", DirectoryModelFile)
                Throw New Exception(sw.ToString())
            End If
        Next
    End Sub

    'Requires an xelement who has an xelement as its parent node.
    Protected Sub InheritAttributes(ByRef ChildElem As XElement)
        'In all normal circumstances each child will have a parent.
        Dim ParentElem As XElement = ChildElem.Parent
        For Each Attribute As XAttribute In ParentElem.Attributes
            If (ChildElem.Attribute(Attribute.Name) Is Nothing) AndAlso inheritableAttributes.Contains(Attribute.Name.ToString) Then
                ChildElem.SetAttributeValue(Attribute.Name, Attribute.Value)
            End If
        Next
    End Sub

    'Requires DirectoryXmlDoc to have been loaded with some files/directories for tasks.
    'So make sure CreateFileDirectory was called atleast once.
    Public Sub CreateArchive(ByVal ArchiveName As String, Optional ByVal DeletePrev As Boolean = True)
        Dim TempFolder As String = GetTempPath() & "ImposibleToClone." & GetRandomFileName()
        If Directory.Exists(TempFolder) Then
            MsgBox("Unable to create a zip file, since a temp folder couldn't be generated.")
            Exit Sub
        Else
            Directory.CreateDirectory(TempFolder)
        End If

        Dim TasksToArchive As IEnumerable(Of String) = From elem In DirectoryXmlDoc.Descendants("files")
                                                       Select elem.Attribute("id").Value
        For Each item In TasksToArchive
            CreateFileDirectory(item, OutputFolder, TempFolder, DirectoryManager.ArchivePathAttribute, ProcessingMode.ArchiveFiles)
        Next

        If Not Directory.Exists(ArchiveDirectory) Then
            Directory.CreateDirectory(ArchiveDirectory)
        Else
            If DeletePrev AndAlso File.Exists(ArchiveDirectory & "\" & ArchiveName) Then
                My.Computer.FileSystem.DeleteFile(ArchiveDirectory & "\" & ArchiveName, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            End If
        End If

        Try
            Using zip As ZipFile = New ZipFile()
                zip.AddDirectory(TempFolder)
                zip.Save(ArchiveDirectory & "\" & ArchiveName)
            End Using
            'ZipFile.CreateFromDirectory(TempFolder, ArchiveDirectory & "\" & ArchiveName, CompressionLevel.Optimal, False)
        Catch ex As Exception
            MsgBox("Error trying to archive output: " & ex.Message)
        End Try
        My.Computer.FileSystem.DeleteDirectory(TempFolder, FileIO.DeleteDirectoryOption.DeleteAllContents)
    End Sub

    Public Sub ArchiveFile(ByVal FileNode As XElement, ByVal InputRootFolder As String, ByVal OutputRootFolder As String)
        If (FileNode.Attribute(ArchivePathAttribute) IsNot Nothing) Then
            Dim InputFile As String = InputRootFolder & FileNode.Attribute(FilePathAttribute).Value & "\" & FileNode.Value
            Dim OutPutFile As String = OutputRootFolder & FileNode.Attribute(ArchivePathAttribute).Value & "\" & FileNode.Value
            If My.Computer.FileSystem.FileExists(InputFile) Then
                If Not My.Computer.FileSystem.FileExists(OutPutFile) Then
                    My.Computer.FileSystem.CopyFile(InputFile, OutPutFile, False)
                End If
            End If
        End If
    End Sub

    'Uses OutputFolder as the base path and FilePath to see if there are any files/folders
    'that aren't already child nodes of ParentElem, then adds them to ParentElem.
    'Only done if the folder exists.
    'This can also be called from outside this class to up-date a folder nodes files/folders.
    Public Sub ReadInNodes(ByRef ParentElem As XElement)
        'Determine if the nodes should be processed by the file processor later.
        'Default is not to process files that have been read in this way.
        Dim ProcessReadInNodes As String
        If (ParentElem.Attribute(ProcessFileAttribute) IsNot Nothing) AndAlso _
            (String.Compare(ParentElem.Attribute(ProcessFileAttribute).Value, "false", True) <> 0) Then
            ProcessReadInNodes = "true"
        Else
            ProcessReadInNodes = "false"
        End If

        Dim FolderLocation As String = OutputFolder
        Dim Results As IEnumerable(Of XElement)
        If ParentElem.Attribute(FilePathAttribute) IsNot Nothing Then
            FolderLocation &= ParentElem.Attribute(FilePathAttribute).Value
        End If

        'TODO: FILE double check
        If Not Directory.Exists(FolderLocation) Then
            Exit Sub
        End If

        Dim Directories As DirectoryInfo() = New DirectoryInfo(FolderLocation).GetDirectories()
        For Each Dir As DirectoryInfo In Directories
            Dim DirName As String = Dir.Name
            Results = From elem In ParentElem.Elements("folder") _
                        Where (elem.Attribute("name") IsNot Nothing) _
                        AndAlso elem.Attribute("name").Value = DirName _
                        Select elem
            If Results.Count = 0 Then
                ParentElem.Add(<folder <%= ProcessFileAttribute %>=<%= ProcessReadInNodes %> name=<%= Dir.Name %>></folder>)
            End If
        Next

        Dim Files As FileInfo() = New DirectoryInfo(FolderLocation).GetFiles()
        For Each File As FileInfo In Files
            Dim FileName As String = File.Name
            Results = From elem In ParentElem.Elements("file") _
                        Where elem.Value = FileName _
                        Select elem
            If Results.Count = 0 Then
                ParentElem.Add(<file <%= ProcessFileAttribute %>=<%= ProcessReadInNodes %>><%= FileName %></file>)
            End If
        Next
    End Sub

#Region "Utility Methods"    
    ''' <summary>
    ''' If its a folder the path should be of the form \Directory1\Directory2\...\Directoryn,
    ''' if its a file the path should be of the form \Directory1\Directory2\...\Directoryn\FileName.
    ''' </summary>
    ''' <param name="TaskId">The id of the task to retrieve the file or folder from.</param>
    ''' <param name="FilePath">Uses the active file path if the file path is not specified.</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' TODO: optional. should use the FilePathAttribute to build the xpath query.
    ''' TODO: not rigorously tested.
    Public Function GetXpath(ByVal TaskId As String, Optional ByVal FilePath As String = Nothing, Optional ByVal IsFile As Boolean = False) As String
        If (IsNothing(FilePath)) Then
            FilePath = ActiveFilePath
            IsFile = False
        End If
        Dim PathFolders As String() = Split(FilePath, "\")
        For i = 0 To (PathFolders.Length - 1)
            If PathFolders(i) <> "" Then
                If i = (PathFolders.Length - 1) AndAlso IsFile Then
                    PathFolders(i) = "file[.='" & PathFolders(i) & "']"
                Else
                    PathFolders(i) = "folder[@name='" & PathFolders(i) & "']"
                End If
            End If
        Next
        Return "/templateFiles/files[@id='" & TaskId & "']" & Join(PathFolders, "/")
    End Function

#End Region

End Class

Public Interface IFileProcessor

    Sub ProcessFileNode(ByVal FileNode As XElement, ByVal TemplateFolder As String, ByVal OutputFolder As String, ByVal FilePathAttribute As String, Optional ByVal templateFileLoc As String = "")

End Interface
