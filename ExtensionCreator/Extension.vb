Imports System.Xml
Imports System.Collections.Specialized
Imports System.IO
Imports System.IO.Path
Imports System.Windows.Forms
Imports System.Xml.XPath
Imports System.Dynamic
Imports System.Text.RegularExpressions
Imports Microsoft.VisualStudio.TextTemplating
Imports System.CodeDom.Compiler

Public MustInherit Class Extension
    Inherits MarshalByRefObject 'Used so that this object can be passed as a parameter to T4 templates.
    Implements IFileProcessor

    'TODO: make members protected/private
    'TODO: make functions shared
    'Extension
    Public ExtensionTitle As String
    'If ExtensionXmlFile is loaded using XmlTemplateLoader
    'UnRenderedExtensionXmlFile should be specified.
    Public UnRenderedExtensionXmlFile As String = ""
    Public ExtensionXmlFile As String
    Public ExtensionFilesXmlFile As String
    'ActiveExtensionFilesXmlFile holds the rendered (ready to use) version
    'of ExtensionFilesXmlFile.
    Public ActiveExtensionFilesXmlFile As String = ""
    Public ExtensionParameters As New MyDictionary
    Public TemplateFolder As String
    Public WorkingFolder As String
    Public TempFolder As String
    Protected DirMgr As DirectoryManager

    Protected Friend DerivedAssembly As String = ""
    Protected Friend DerivedNamespace As String = ""

    Private m_UnRenderedExtensionXmlDoc As XElement = Nothing
    Public ExtensionXmlDoc As XmlDocument = Nothing
    Protected ExtensionTasks As IEnumerable(Of XElement) = Nothing
    Protected ExtensionTasksIndex As Integer = -1
    Public ReplacementDictionaries As New ListDictionary()
    'Having this variable ensures that the temporary file XmlTemplateLoader creates
    'isn't deleted untill this class is disposed of.
    Private ExtensionDocLoader As XmlTemplateLoader

    'list dictionaries are used for very short lists
    Public TaskDictionary As New ListDictionary()
    'Manages Task execution
    Public TaskQueue As New Queue()
    Public FirstTask As Boolean
    Public NewExtension As Boolean = True

    'Constants
    Protected Const TaskDefinitionXpath As String = "/extension/taskDefinitions/taskDefinition"
    'The location of TaskDefinition parameters relative to the TaskDefinition element.
    Protected Const TaskDefinitionParametersXpath As String = "/taskDefinition/parameters"
    Protected Const ExtensionParamatersXpath As String = "/extension/parameters"

    Public Enum FileStatus
        Created = 1
        Updated = 2
        Unknown = 0
    End Enum

    Public Shared Event ShowMessage(ByVal Message As String)
    Public Delegate Function GUIinteract() As Boolean
    'Raised when the extension needs to open up a GUI. The GUI should handle these events appropriately.
    Public Shared Event InteractUser(ByVal Func As GUIinteract)
    Public Shared Event TemplateError(ByVal host As TemplateEngineHost, ByVal ex As Exception)

    Protected Overridable Sub OnInteractUser(ByVal Func As GUIinteract)
        RaiseEvent InteractUser(Func)
    End Sub

    ' This document is used to store program data.
    Protected Property UnRenderedExtensionXmlDoc() As XElement
        Get
            If IsNothing(m_UnRenderedExtensionXmlDoc) And UnRenderedExtensionXmlFile <> "" Then
                Try
                    ' If the XML file (or string for that matter)
                    ' is invalid, an exception of the type
                    ' XmlException will be thrown
                    'Note: Changes made to this file will be saved during Run() after Finish() is called.
                    m_UnRenderedExtensionXmlDoc = XElement.Load(UnRenderedExtensionXmlFile)
                Catch ex As Exception
                    'TODO: add xml file name
                    Throw New Exception(ex.Message, ex)
                End Try
            End If
            Return m_UnRenderedExtensionXmlDoc
        End Get
        Set(ByVal value As XElement)
            m_UnRenderedExtensionXmlDoc = value
        End Set
    End Property

    Private _taskRan As Boolean = False
    Public Property TaskRan() As Boolean
        Get
            Return _taskRan
        End Get
        Protected Set(ByVal value As Boolean)
            _taskRan = value
        End Set
    End Property


#Region "Methods"

    'Only called on already created templates
    Public Overridable Sub Export()
        NewExtension = False
        Start()
        DirMgr.FileProcessor = Me
        DirMgr.ArchiveAttribute = "exportAs"
        While GetNextTask()
            DirMgr.LoadDirectoryModel(ActiveExtensionFilesXmlFile)
            DirMgr.ReadFileDirectory(GetActiveTaskName(), TaskQueue.Peek)
        End While

        CreateArchive(ExtensionParameters.MyItem("exportFileName"), False)
        Try
            DirMgr.DirectoryXmlDoc.Save(My.Application.Info.DirectoryPath & "\DirectoryXmlTree.xml")
        Catch ex As Exception
            'Ignore errors
        End Try
    End Sub

    'TODO: should also remove files from the output directory
    Public Overridable Sub Reset()
        NewExtension = False
        Start()
        Dim xElem As XElement
        For Each xElem In ExtensionTasks
            If xElem.Elements("parameters").Count() > 0 AndAlso _
                xElem.Elements("parameters").First.Elements("ran").Count() > 0 Then
                xElem.Elements("parameters").First.Elements("ran").First.Remove()
            End If
        Next

        'Save program data.
        If Not IsNothing(m_UnRenderedExtensionXmlDoc) Then
            m_UnRenderedExtensionXmlDoc.Save(UnRenderedExtensionXmlFile)
        End If
    End Sub

    'Runs the extension.
    'Note requires DirMaker to have been initalized.
    Public Overridable Sub Run()
        Start()
        DirMgr.FileProcessor = Me
        While GetNextTask()
            DirMgr.LoadDirectoryModel(ActiveExtensionFilesXmlFile)
            DirMgr.ReadFileDirectory(GetActiveTaskName(), TaskQueue.Peek)
            DirMgr.CreateFileDirectory(TaskQueue.Peek, DirMgr.TemplateFolder, DirMgr.OutputFolder, DirectoryManager.FilePathAttribute)
        End While
        Finish()
        'Save program data.
        If Not IsNothing(m_UnRenderedExtensionXmlDoc) Then
            m_UnRenderedExtensionXmlDoc.Save(UnRenderedExtensionXmlFile)
        End If
        Try
            'TODO: might not be able to save to directory path... Check this in exe file
            DirMgr.DirectoryXmlDoc.Save(My.Application.Info.DirectoryPath & "\DirectoryXmlTree.xml")
        Catch ex As Exception
            'Ignore errors
        End Try
    End Sub
#End Region

#Region "Startup, and Finish"

    Public Sub New(ByVal m_ExtensionXmlFile As String, ByVal m_ExtensionTitle As String, ByVal m_ExtensionDocLoader As XmlTemplateLoader)
        ExtensionXmlFile = m_ExtensionXmlFile
        ExtensionTitle = m_ExtensionTitle
        ExtensionDocLoader = m_ExtensionDocLoader
    End Sub

    'Obsolete
    Public Sub New(ByVal m_ExtensioXmlFile As String, ByVal m_TemplateFolder As String, ByVal m_TempFolder As String)
        ExtensionXmlFile = m_ExtensioXmlFile
        TemplateFolder = m_TemplateFolder
        TempFolder = m_TempFolder

        loadExtension()
        loadTasks()

        'These should be changed by child classes
        WorkingFolder = "OutputFile"
        'DEBUG
        'Shows how to access the info stored in TaskDictionary
        'For Each de As DictionaryEntry In TaskDictionary
        'Console.WriteLine("TaskID: " & de.Key)
        ' Dim thiscoll As Microsoft.VisualBasic.Collection = de.Value
        ' Console.WriteLine(vbTab & "TaskName: " & thiscoll.Item("TaskName"))
        ' Dim Dictionary As Dictionary(Of String, String) = thiscoll.Item("TaskParameters")
        ' For Each Key In Dictionary.Keys
        'Console.WriteLine(vbTab & vbTab & Key & " : " & Dictionary.Item(Key))
        ' Next
        'Next
        'DEBUG

        'Set FirstTask to true so getNextTask method 
        'only calls StartNextTask method
        FirstTask = True

        'Reload the xml doc, so selectSingleNodes don't interfere
        loadExtensionXmlDoc(True)
    End Sub

    'make sure it stores the template and output folders
    'get the extension to use templateEngine to parse the _files.xml file so it will be readable
    'NOTE have to reparse the _files.xml every task, since they are task specific
    'get it to store the output folder and working folder
    'TODO: write summary
    Public Overridable Sub Start()
        'Extension is loaded before the tasks so that it is added first 
        'into the task queue.
        loadExtension()
        loadTasks()
        'Initalize the DirectoryMaker
        loadFolders()
        SetupFolders()
        'Set FirstTask to true so getNextTask method 
        'only calls StartNextTask method
        FirstTask = True
        'TODO: update this, make more general to just take xml file name
        'Reload the xml doc, so selectSingleNodes don't interfere
        loadExtensionXmlDoc(True)

    End Sub

    'Loads the working and output folder, and sets ExtensionFilesXmlFile (also checking ExtensionFilesXmlFile exists).
    Protected Sub loadFolders()
        Dim RootDir As String
        If UnRenderedExtensionXmlFile <> "" Then
            RootDir = GetDirectoryName(UnRenderedExtensionXmlFile)
        Else
            RootDir = GetDirectoryName(ExtensionXmlFile)
        End If
        ExtensionFilesXmlFile = ExtensionParameters.MyItem("templateFiles")
        If Not IsPathRooted(ExtensionFilesXmlFile) Then
            ExtensionFilesXmlFile = RootDir & "\" & ExtensionFilesXmlFile
        End If
        If Not My.Computer.FileSystem.FileExists(ExtensionFilesXmlFile) Then
            Dim sw As New StringWriter
            sw.Write("The xml file containing the file information for the {0} template was not found. The file name should be {1}", _
                     ExtensionTitle, ExtensionFilesXmlFile)
            Throw New FileNotFoundException(sw.ToString())
        End If

        DirMgr = New DirectoryManager(ExtensionFilesXmlFile, ExtensionParameters.MyItem("TemplateFolder"), _
                                      ExtensionParameters.MyItem("OutputFolder"), ExtensionParameters.MyItem("ArchiveDirectory"))
        DirMgr.FileProcessor = Me

        TemplateFolder = ExtensionParameters.MyItem("TemplateFolder")
        WorkingFolder = ExtensionParameters.MyItem("OutputFolder")
    End Sub

    'Requires WorkingFolder and TemplateFolder to be set, before it's called.
    'Checks that WorkingFolder doesn't exist then creates it, 
    'and also checks if TemplateFolder exists.
    Protected Sub SetupFolders()
        Dim folderExists As Boolean
        folderExists = My.Computer.FileSystem.DirectoryExists(WorkingFolder)
        If folderExists Then
            'Work with existing directory.
        Else
            My.Computer.FileSystem.CreateDirectory(WorkingFolder)
        End If
        folderExists = My.Computer.FileSystem.DirectoryExists(TemplateFolder)
        If Not folderExists Then
            Dim sw As New StringWriter
            sw.Write("The template folder {0} doesn't exist. Please make sure it is correct or add it before running.", TemplateFolder)
            Throw New Exception(sw.ToString())
        End If
    End Sub

    Protected Overridable Sub AddToExtensionParameters()
        ExtensionParameters.Add("RunningExtensionAssemblyPath", System.Reflection.Assembly.GetAssembly(Me.GetType).Location)
    End Sub

    'Loads the parameters for the extension
    Public Sub loadExtension()
        'Load extension parameters
        loadExtensionXmlDoc(True)
        loadParams(ExtensionParameters, ExtensionXmlDoc.SelectNodes(ExtensionParamatersXpath), ExtensionXmlFile)
        'Place to add additional extension wide parameters for use in templates
        AddToExtensionParameters()
        ReplacementDictionaries.Add("Extension", ExtensionParameters)
        'TODO: Handel empty extension parameters...

        'Load tasks from UnRenderedExtensionXmlDoc so that task run info can be set.
        Dim TasksElems As IEnumerable(Of XElement) = UnRenderedExtensionXmlDoc.XPathSelectElements("/tasks/task")
        ExtensionTasks = TasksElems.ToList()
    End Sub

    'Loads task parameters into TaskDictionary for requested tasks, and fills TaskQueue
    'with the requested tasks.
    Public Sub loadTasks()
        Dim xmlElem As XmlElement
        loadExtensionXmlDoc(False)

        Dim TaskDefinitionDictionary As New ListDictionary()
        Dim xmlNodeList As XmlNodeList
        Dim xmlNode As XmlNode
        'LOAD the task definitions ------------------------------------------------------------------
        xmlNodeList = ExtensionXmlDoc.SelectNodes(TaskDefinitionXpath)
        If xmlNodeList IsNot Nothing Then
            For Each xmlNode In xmlNodeList
                If TypeOf xmlNode Is XmlElement Then
                    xmlElem = CType(xmlNode, XmlElement)
                    If Not xmlElem.HasAttribute("name") Then
                        Throw New Exception("No name for task definition in " & ExtensionXmlFile)
                    End If
                    'Get parameters for task definition
                    Dim xmlTempDoc As New XmlDocument
                    xmlTempDoc.LoadXml(xmlElem.OuterXml)
                    Dim TaskParameters As New Dictionary(Of String, String)
                    loadParams(TaskParameters, xmlTempDoc.SelectNodes(TaskDefinitionParametersXpath), ExtensionXmlFile)
                    'Add task definition parameters to list of task definitions
                    Dim ACollection As New Microsoft.VisualBasic.Collection()
                    ACollection.Add(TaskParameters, "TaskParameters")
                    ACollection.Add(0, "TasksCount")
                    If TaskDefinitionDictionary.Contains(xmlElem.GetAttribute("name")) Then
                        'Task definition names must be unique
                        Throw New Exception("Multiple task definitions with the name " & xmlElem.GetAttribute("name"))
                        Continue For
                    End If
                    TaskDefinitionDictionary.Add(xmlElem.GetAttribute("name"), ACollection)
                    'DEBUG
                    'Console.WriteLine(xmlElem.GetAttribute("name") & "--------------------------")
                    'Dim list As New List(Of String)(TaskParameters.Keys)
                    'For Each item As String In list
                    'Console.WriteLine(TaskParameters.Item(item))
                    'Next
                    'DEBUG
                End If
            Next xmlNode
        End If
        If TaskDefinitionDictionary.Count = 0 Then
            'TODO: No task definitions set or found, so tell user and exit. 
            '      Since task definitions need to be set before tasks are set.
        End If
        'LOAD the tasks --------------------------------------------------------------------------------
        xmlNodeList = ExtensionXmlDoc.SelectNodes("/extension/tasks/task")
        If xmlNodeList IsNot Nothing Then
            For Each xmlNode In xmlNodeList
                'Must be an XmlElement

                If TypeOf xmlNode Is XmlElement Then
                    xmlElem = CType(xmlNode, XmlElement)
                    If Not xmlElem.HasAttribute("name") Then
                        Throw New Exception("No name for task in " & ExtensionXmlFile)
                    End If
                    Dim TaskParameters As New MyDictionary()
                    If TaskDefinitionDictionary.Contains(xmlElem.GetAttribute("name")) Then
                        'Merge task definition params and task params, task params overwritting on param intersection
                        Dim myTempDict As Dictionary(Of String, String) = TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Item("TaskParameters")
                        Dim myTempList As New List(Of String)(myTempDict.Keys)
                        For Each myTempKey In myTempList
                            TaskParameters.Add(myTempKey, myTempDict.Item(myTempKey))
                        Next
                    Else
                        'Lets add an entry to TaskDefinitionDictionary for this task, so we can keep track of TaskIds
                        Dim TCollection As New Microsoft.VisualBasic.Collection()
                        TCollection.Add(New Dictionary(Of String, String), "TaskParameters")
                        TCollection.Add(0, "TasksCount")
                        TaskDefinitionDictionary.Add(xmlElem.GetAttribute("name"), TCollection)
                    End If
                    Dim xmlTempDoc As New XmlDocument
                    xmlTempDoc.LoadXml(xmlElem.OuterXml)
                    loadParams(TaskParameters, xmlTempDoc.SelectNodes("/task/parameters"), ExtensionXmlFile)
                    Dim ACollection As New Microsoft.VisualBasic.Collection()
                    'Update the counter for the task definition, to have unique ids for tasks
                    Dim tempInt As Integer = TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Item("TasksCount")
                    tempInt += 1
                    TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Remove("TasksCount")
                    TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Add(tempInt, "TasksCount")
                    ACollection.Add(TaskParameters, "TaskParameters")
                    ACollection.Add(xmlElem.GetAttribute("name"), "TaskName")
                    TaskDictionary.Add(xmlElem.GetAttribute("name") & _
                                       CInt(TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Item("TasksCount")), ACollection)
                    'Add the task to the queue so it can be run
                    TaskQueue.Enqueue(xmlElem.GetAttribute("name") & CInt(TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Item("TasksCount")))
                    'DEBUG
                    'Console.WriteLine(xmlElem.GetAttribute("name") & _
                    '                   CInt(TaskDefinitionDictionary.Item(xmlElem.GetAttribute("name")).Item("TasksCount")) & "--------------------------")
                    'Dim list As New List(Of String)(TaskParameters.Keys)
                    'For Each item As String In list
                    'Console.WriteLine(vbTab & item & " : " & TaskParameters.Item(item))
                    'Next
                    'DEBUG
                End If
            Next xmlNode
        End If
        If TaskDictionary.Count = 0 Then
            'TODO: No tasks set or found, so tell user and exit.
        End If
    End Sub

    'This subroutine is called after all tasks
    'have been executed
    Public MustOverride Sub Finish()

#End Region

#Region "Task Management"

    'TODO: (critical) neeed to test this
    Public Function GetTasksWithAllParams(ByVal TaskName As String) As List(Of Task)
        Dim Tasks = From Task In TaskDictionary
                    Where Task.Value.item("TaskName") Like TaskName
                    Select New Task(Task.Value.item("TaskParameters"), Task.Value.item("TaskParameters").Keys.ToArray())
        Return Tasks.ToList()
    End Function

    Public Function GetTasksWithNoParams(ByVal TaskName As String) As List(Of Task)
        Dim Tasks = From Task In TaskDictionary
                    Where Task.Value.item("TaskName") Like TaskName
                    Select New Task(Task.Value.item("TaskParameters"))
        Return Tasks.ToList()
    End Function

    Public Function GetTasks(ByVal TaskName As String, ByVal ParamArray Params() As String) As List(Of Task)
        If Params.Length <= 0 Then Throw New Exception("No parameters were passed to get tasks.")
        Dim Tasks = From Task In TaskDictionary
                    Where Task.Value.item("TaskName") Like TaskName
                    Select New Task(Task.Value.item("TaskParameters"), Params)
        Return Tasks.ToList()
    End Function

    'Gets the next task to run in the TaskQueue
    'Assuming a task finished running.
    'Also manages switching between tasks
    'Returns False when no task to run, otherwise true.
    Public Function GetNextTask() As Boolean
        If TaskQueue.Count = 0 Then
            Return False
        End If
        If FirstTask = True Then
            FirstTask = False
            StartNextTask()
            Return True
        ElseIf TaskQueue.Count = 1 Then
            EndActiveTask()
            Return False
        Else
            EndActiveTask()
            StartNextTask()
            Return True
        End If
    End Function

    Public Function GetActiveTaskName() As String
        If TaskQueue.Count = 0 Then
            Return ""
        End If
        Dim ActiveTaskId As String = TaskQueue.Peek()
        Dim ActiveTask As Microsoft.VisualBasic.Collection = TaskDictionary.Item(ActiveTaskId)
        Return ActiveTask.Item("TaskName")
    End Function

    Public Function GetActiveTaskCollection() As Microsoft.VisualBasic.Collection
        If TaskQueue.Count = 0 Then
            Return Nothing
        End If

        Dim ActiveTaskId As String = TaskQueue.Peek()
        Dim ActiveTask As Microsoft.VisualBasic.Collection = TaskDictionary.Item(ActiveTaskId)
        Return ActiveTask
    End Function

    Public Sub EndActiveTask()
        Dim ActiveTaskId As String = TaskQueue.Peek()
        Dim ActiveTask As Microsoft.VisualBasic.Collection = TaskDictionary.Item(ActiveTaskId)
        Dim sw As New StringWriter

        'Add a "ran" xml tag into xml task so we know this task was ran in the next execution.
        'TODO: (optional) put in a namespace to distinguish it from other parameters.
        ExtensionTasksIndex += 1
        Dim xElem As XElement = ExtensionTasks(ExtensionTasksIndex)
        If Not ActiveTask.Item("TaskParameters").ContainsKey("ran") Then
            If xElem.Elements("parameters").Count = 0 Then
                xElem.Add(<parameters></parameters>)
            End If
            xElem.Elements("parameters").First.Add(<ran>true</ran>)
        End If

        'Write the error message for the case where the function we are calling doesn't exist
        sw.Write("Problem running task: Member function '{0}' doesn't exist. It was called while running task '{1}'", _
                 "End" & ActiveTask.Item("TaskName"), ActiveTask.Item("TaskName"))
        MyCallByName(Me, "End" & ActiveTask.Item("TaskName"), CallType.Method, sw.ToString())
        'Allow the end method to use the replace dictionaries
        'before removing the active task parameters from it.
        If ReplacementDictionaries.Contains("Task") Then
            ReplacementDictionaries.Remove("Task")
        End If
        My.Computer.FileSystem.DeleteFile(ActiveExtensionFilesXmlFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        ActiveExtensionFilesXmlFile = ""
        'TODO: change the console action below to event of some sort...
        'Console.WriteLine("Executed an instance of task: " & ActiveTask.Item("TaskName"))
        TaskQueue.Dequeue()
        TaskRan = False
        'Reload the xml doc, so selectSingleNodes don't interfere
        loadExtensionXmlDoc(True)
        'DEBUG
        'Console.WriteLine("Ended : " & ActiveTaskId & " -----------------------------------")
        'DEBUG
        RaiseEvent ShowMessage("Finished running task " & ActiveTask.Item("TaskName") & " --------------------")
    End Sub

    'Assumes there is another task to run.
    Public Sub StartNextTask()
        Dim NextTaskId As String = TaskQueue.Peek()
        Dim NextTask As Microsoft.VisualBasic.Collection = TaskDictionary.Item(NextTaskId)

        RaiseEvent ShowMessage("Starting task " & NextTask.Item("TaskName") & " --------------------")

        'Set the task ran property so the startup functions know.
        If (NextTask.Item("TaskParameters").ContainsKey("ran") AndAlso NextTask.Item("TaskParameters").Item("ran") = "true") _
            OrElse DirMgr.ArchiveAttribute = "exportAs" Then
            'Set task ran as true if this is part of an export.
            TaskRan = True
        End If

        Dim sw As New StringWriter
        'Write the error message for the case where the function we are calling doesn't exist
        sw.Write("Problem running task: Member function '{0}' doesn't exist. It was called while running task '{1}'", _
                    "Start" & NextTask.Item("TaskName"), NextTask.Item("TaskName"))
        MyCallByName(Me, "Start" & NextTask.Item("TaskName"), CallType.Method, sw.ToString())
        'Modify the replace dictionaries after the start method, 
        'incase the start method wants to modify the parameters
        If NextTask.Contains("TaskParameters") Then
            ReplacementDictionaries.Add("Task", NextTask.Item("TaskParameters"))
        End If
        ActiveExtensionFilesXmlFile = GetCurrentExtensionFilesXmlFile()
        'DEBUG
        'Console.WriteLine("Started : " & NextTaskId & " -----------------------------------")
        'DEBUG
    End Sub

    'Standard task. To add files the files must belong to a task,
    'so for simple extensions just use the Main task for adding files
    Public Overridable Function StartMain() As Exception
        Return Nothing
    End Function

    Public Overridable Function EndMain() As Exception
        Return Nothing
    End Function
#End Region

#Region "Parameter Processing"

    'Note: Needs valid xml files
    Sub loadParams(ByRef ACollection As Dictionary(Of String, String), ByVal xmlNodes As XmlNodeList, ByVal XmlFile As String)
        'TODO Get xmlNode then type cast it to xmlElement
        If xmlNodes.Count > 0 Then
            For Each xmlElem As XmlElement In xmlNodes
                TraverseParamTree(xmlElem, ACollection)
            Next
        End If
    End Sub

    Sub TraverseParamTree(ByVal xElem As XmlElement, ByRef ACollection As Dictionary(Of String, String))
        Dim xNodeLoop As XmlNode

        If xElem.HasChildNodes Then
            For Each xNodeLoop In xElem.ChildNodes
                'Skip any nodes that aren't elements
                If xNodeLoop.NodeType = XmlNodeType.Element Then
                    'Note: For parameter structures either:
                    '   the element is a parameter in which case it has only 1 child which is a text node
                    '   or it is a grouping element which we can TraverseParamTree with
                    If xNodeLoop.HasChildNodes AndAlso (xNodeLoop.ChildNodes.Count > 1 OrElse xNodeLoop.FirstChild.NodeType = XmlNodeType.Element) Then
                        Call TraverseParamTree(CType(xNodeLoop, XmlElement), ACollection)
                    Else
                        Call ProcessParam(CType(xNodeLoop, XmlElement), ACollection)
                    End If
                End If
            Next xNodeLoop
        End If
    End Sub

    Sub ProcessParam(ByVal xElem As XmlElement, ByRef ACollection As Dictionary(Of String, String))
        'Always override previous instances of the variable
        If ACollection.ContainsKey(xElem.Name) Then
            ACollection.Remove(xElem.Name)
        End If
        ACollection.Add(xElem.Name, xElem.InnerText)
    End Sub

#End Region

#Region "File Processing"

    'Determines the template file location.
    'If the output file already exists it will be used as the template file.
    Public Sub ProcessFileNode(ByVal FileNode As XElement, ByVal TemplateFolder As String, ByVal OutputFolder As String, ByVal FilePathAttribute As String, Optional ByVal templateFileLoc As String = "") Implements IFileProcessor.ProcessFileNode
        Dim myFileLoc As String = FileNode.Attribute(DirectoryManager.FilePathAttribute).Value & "\" & FileNode.Value
        Dim outputFileLoc As String = OutputFolder & myFileLoc

        If My.Computer.FileSystem.FileExists(outputFileLoc) Then
            templateFileLoc = outputFileLoc
        ElseIf templateFileLoc <> "" Then
            'Child class genereated the template file location
        ElseIf FileNode.Attribute("template") IsNot Nothing Then
            If Path.IsPathRooted(FileNode.Attribute("template").Value) Then
                'The template file was supplied from a non-standard location.
                templateFileLoc = FileNode.Attribute("template").Value
            Else
                templateFileLoc = TemplateFolder & "\" & FileNode.Attribute("template").Value
            End If
        ElseIf FileNode.Attribute("function") IsNot Nothing Then
            Try
                Dim CallException
                CallException = CallByName(Me, FileNode.Attribute("function").Value, CallType.Method, templateFileLoc, FileNode)
                If CallException.GetType.ToString() = GetType(Exception).ToString() Then
                    Throw New Exception(CallException.Message, CallException)
                ElseIf CallException.GetType.ToString() = GetType(String).ToString() Then
                    templateFileLoc = CallException
                End If
            Catch ex As MissingMemberException
                templateFileLoc = ""
                Dim sw As New StringWriter
                'Write the error message for the case where the function we are calling doesn't exist
                sw.Write("Problem reading directory model: Member function '{0}' doesn't exist. It was called in file node '{1}'", FileNode.Attribute("function").Value, myFileLoc)
                Throw New Exception(sw.ToString(), ex)
            Catch ex As Exception
                templateFileLoc = ""
                Dim sw As New StringWriter
                sw.Write("Problem reading directory model: Function '{0}' did not run properly. It was called in file node '{1}'. Error: " _
                          & ex.Message, FileNode.Attribute("function").Value, myFileLoc)
                Throw New Exception(sw.ToString(), ex)
            End Try
        Else
            'TODO: better info in error messages, i.e task name, xml file name
            Throw New Exception("Problem reading directory model: Can't determine what to do with " & myFileLoc & _
                              ". It has no template or function attribute.")
        End If

        If (templateFileLoc <> "") Then
            Select Case ProcessFile(templateFileLoc, outputFileLoc)
                Case FileStatus.Created
                    RaiseEvent ShowMessage("Wrote " & myFileLoc)
                Case FileStatus.Updated
                    RaiseEvent ShowMessage("Updated " & myFileLoc)
                Case Else
                    RaiseEvent ShowMessage("Error " & myFileLoc)
            End Select
        End If
    End Sub

    'If the template file is found:
    'RenderFile is called to compile the template using the template engine, then copy it 
    'to the output file location.
    'Return: FileStatus
    Protected Function ProcessFile(ByVal templateFileLoc As String, ByVal outputFileLoc As String) As FileStatus
        Dim Result As FileStatus
        'TODO: FILE double check. Noteice that myTemplateFile var is not used.
        If File.Exists(templateFileLoc) Then
            'Dim myTemplateFile = myFileSystem.GetFile(templateFileLoc)
            If Not File.Exists(outputFileLoc) Then
                Result = FileStatus.Created
            Else
                Result = FileStatus.Updated
                'If the output file was already created, it will be overwritten.
            End If
            RenderFile(templateFileLoc, outputFileLoc)
        Else
            Result = FileStatus.Unknown
            Throw New Exception("Problem reading directory model: Template file doesn't exist : " & templateFileLoc)
        End If
        Return Result
    End Function

#End Region

#Region "Template"
    'Assumes templateFileLoc points to an existing text file, and tpl is a global Template object
    'with appropriate parameters (and Args if needed) set. Renders the file by 
    'running the VB compiler on the files contents, then writes the output to outputFileLoc 
    '(which should also be a valid file location).
    Protected Sub RenderFile(ByVal templateFileLoc As String, ByVal outputFileLoc As String, Optional ByVal overwrite As Boolean = False)
        Dim status As String = "Not Done"
        Dim host As New TemplateEngineHost(Me)
        Dim engine As New Engine()
        Dim myOutput As String = ""
        Dim runEx As Exception = Nothing

        Do While status = "Not Done"
            Try
                Dim input As String = ""
                host.TemplateFileValue = templateFileLoc
                Using sr As StreamReader = New StreamReader(templateFileLoc)
                    input = sr.ReadToEnd()
                End Using

                'Pass in the extension as a parameter to the template
                host.Session = host.CreateSession()
                host.Session("Ext") = Me

                myOutput = engine.ProcessTemplate(input, host)
            Catch ex As Exception
                runEx = ex
            Finally
                If host.Errors.Count > 0 OrElse runEx IsNot Nothing Then
                    RaiseEvent TemplateError(host, runEx)
                    If Not host.ErrorResolved Then
                        status = "Done With Error"
                    Else
                        'Reinitalize host and engine for another try.
                        host = New TemplateEngineHost(Me)
                        engine = New Engine()
                    End If
                Else
                    'If there are no errors change the status to Done
                    status = "Done"
                End If
            End Try
        Loop

        Select Case status
            Case "Done With Error", "Not Done"
                Dim e As CompilerError
                Dim sw As New StringWriter

                For Each e In host.Errors
                    Console.WriteLine(e.ToString())
                Next
                sw.Write("Error trying to render the template file '{0}' to the output file '{1}'", templateFileLoc, outputFileLoc)
                Throw New Exception(sw.ToString())
            Case Else
                'Only use the template file if the output file doesn't already exist.
                If Not File.Exists(outputFileLoc) OrElse overwrite Then
                    Using sw As New StreamWriter(outputFileLoc, True, host.fileEncoding)
                        sw.Write(myOutput)
                    End Using
                End If
        End Select
    End Sub
#End Region

#Region "Helpers"

    'Returns true if the element has the attribute and the attribute has the value true (case-insensitive match)
    'or the attribute has the value 1.
    Public Function ParseBooleanAttribute(ByVal XElem As XElement, ByVal Attribute As String)
        Dim TempBool As Boolean = False
        If XElem.Attribute(Attribute) IsNot Nothing AndAlso _
            ((Boolean.TryParse(XElem.Attribute(Attribute).Value, TempBool) AndAlso TempBool) Or (XElem.Attribute(Attribute).Value = "1")) Then
            Return True
        Else
            Return False
        End If
    End Function

    'Should be called at the end (after run) when DirMgr has the full directory list in memory. DirMgr.ArchiveName
    'must have been loaded.
    Protected Sub CreateArchive(Optional ByVal ArchiveName As String = "", Optional ByVal DeletePrev As Boolean = True)
        'Will assume that DirMgr.LoadDirectoryModel has been called at least once before this sub is called.
        ArchiveName = IIf(ArchiveName = "", DirMgr.ArchiveName, ArchiveName)
        DirMgr.CreateArchive(ArchiveName, DeletePrev)
    End Sub

    'Wrapper for CallByName function that gets exceptions returned by the functions.
    'Requires the member being called to be a function that returns an exception on error and nothing on success.
    'Notes: Checks if ObjectRef has ProcName as a method before calling.
    Public Sub MyCallByName(ByVal ObjectRef As Object, ByVal ProcName As String, ByVal UserCallType As Microsoft.VisualBasic.CallType, _
                            ByVal MissingMemberExceptionMessage As String, ByVal ParamArray Args() As Object)
        Dim CallException As Exception
        'Only call the method if it exists
        If ObjectRef.GetType().GetMethod(ProcName) IsNot Nothing Then
            CallException = CallByName(ObjectRef, ProcName, UserCallType, Args)
        Else
            CallException = Nothing
        End If
        If CallException IsNot Nothing Then
            Throw New Exception(CallException.Message, CallException)
        End If
    End Sub

    Public Sub loadExtensionXmlDoc(ByVal ForceReload As Boolean)
        ' intialize XML Document
        ' Only re-load if ForceReload is true or
        ' the document is uninitialized.
        ' OrElse performs short-circut evaluation
        ' unlike Or which will execute both tests
        ' even if ForceReload is True
        If ForceReload OrElse (ExtensionXmlDoc Is Nothing) Then
            ExtensionXmlDoc = New XmlDocument
            Try
                ' If the XML file (or string for that matter)
                ' is invalid, an exception of the type
                ' XmlException will be thrown
                ExtensionXmlDoc.Load(ExtensionXmlFile)
            Catch ex As Exception
                'TODO: add xml file name
                Throw New Exception(ex.Message, ex)
            End Try
        End If
    End Sub

    'Notes: This should only be called after an ActiveTask is set.
    'Make sure the file that is returned is deleted after use.
    'Return: The location of the rendered (TemplateEngine executed) extension XML file with only the files for
    'the active task inside.
    Protected Function GetCurrentExtensionFilesXmlFile()
        Dim myTempFilesXmlFile As String = Path.GetTempFileName
        'Temporary location where the unrendered extension xml file will be saved.

        Dim ExtXmlFileLoc As String = My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\Extension Creator\" & Path.GetFileName(ExtensionFilesXmlFile) 'My.Application.Info.DirectoryPath & "\" & Path.GetFileName(ExtensionFilesXmlFile)

        Dim ExtFilesXml As String
        'Earlier code checked that ExtensionFilesXmlFile exists in the file system.
        Using sr As StreamReader = New StreamReader(ExtensionFilesXmlFile)
            ExtFilesXml = sr.ReadToEnd()
        End Using

        Using sw As StreamWriter = New StreamWriter(ExtXmlFileLoc)
            Dim rx As New Regex("<files\b[^>]*for=""(.*?)""[^>]*>(.*?)</files>", RegexOptions.Singleline)
            sw.Write(rx.Replace(ExtFilesXml, AddressOf ExtFilesMatcher))
        End Using

        RenderFile(ExtXmlFileLoc, myTempFilesXmlFile, True)

        'Delete the temporary unrendered extension xml file.
        My.Computer.FileSystem.DeleteFile(ExtXmlFileLoc, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
        Return myTempFilesXmlFile
    End Function

    'Helper function for Regex.Replace in the above function, GetCurrentExtensionFilesXmlFile.
    Private Function ExtFilesMatcher(ByVal m As Match) As String
        If m.Groups(1).Value = GetActiveTaskName() Then
            Return m.Value
        Else
            Return ""
        End If
    End Function

    Public Overrides Function ToString() As String
        Return ExtensionTitle
    End Function

#End Region

End Class

Public Class ExtensionItem
    Private _file As String
    Private _label As String
    Private _ExtensionLoader As XmlTemplateLoader

    Public Property File() As String
        Get
            Return _file
        End Get
        Set(ByVal value As String)
            _file = value
        End Set
    End Property

    Public Property Label() As String
        Get
            Return _label
        End Get
        Set(ByVal value As String)
            _label = value
        End Set
    End Property

    Public Property ExtensionLoader() As XmlTemplateLoader
        Get
            Return _ExtensionLoader
        End Get
        Set(ByVal value As XmlTemplateLoader)
            _ExtensionLoader = value
        End Set
    End Property

End Class