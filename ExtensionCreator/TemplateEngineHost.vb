Imports Microsoft.VisualStudio.TextTemplating
Imports System.Text
Imports System.CodeDom.Compiler
Imports System.IO

'The text template transformation engine is responsible for running 
'the transformation process.
'The host is responsible for all input and output, locating files, 
'and anything else related to the external environment.
'-------------------------------------------------------------------------
Public Class TemplateEngineHost
    Inherits MarshalByRefObject
    Implements ITextTemplatingEngineHost
    Implements ITextTemplatingSessionHost

    Protected Ext As Extension
    Public ErrorResolved As Boolean = False

    Public Sub New(ByVal m_ext As Extension)
        Ext = m_ext
        errorsValue = New CompilerErrorCollection()
    End Sub

    'the path and file name of the text template that is being processed
    '---------------------------------------------------------------------
    Friend TemplateFileValue As String
    Public ReadOnly Property TemplateFile() As String Implements ITextTemplatingEngineHost.TemplateFile
        Get
            Return TemplateFileValue
        End Get
    End Property
    'This will be the extension of the generated text output file.
    'The host can provide a default by setting the value of the field here.
    'The engine can change this based on the optional output directive
    'if the user specifies it in the text template.
    '---------------------------------------------------------------------
    Private fileExtensionValue As String = ".txt"
    Public ReadOnly Property FileExtension() As String
        Get
            Return fileExtensionValue
        End Get
    End Property
    'This will be the encoding of the generated text output file.
    'The host can provide a default by setting the value of the field here.
    'The engine can change this value based on the optional output directive
    'if the user specifies it in the text template.
    '---------------------------------------------------------------------
    Private fileEncodingValue As Encoding = Encoding.UTF8
    Public ReadOnly Property fileEncoding() As Encoding
        Get
            Return fileEncodingValue
        End Get
    End Property
    'These are the errors that occur when the engine processes a template.
    'The engine passes the errors to the host when it is done processing,
    'and the host can decide how to display them. For example, the host 
    'can display the errors in the UI or write them to a file.
    '---------------------------------------------------------------------
    Private errorsValue As CompilerErrorCollection
    Public ReadOnly Property Errors() As CompilerErrorCollection
        Get
            Return errorsValue
        End Get
    End Property
    'The host can provide standard assembly references.
    'The engine will use these references when compiling and
    'executing the generated transformation class.
    '--------------------------------------------------------------
    Public ReadOnly Property StandardAssemblyReferences() As IList(Of String) Implements ITextTemplatingEngineHost.StandardAssemblyReferences
        Get
            'If this host searches standard paths and the GAC,
            'we can specify the assembly name like this.
            '---------------------------------------------------------
            'Return New String() {"System"}
            'Because this host only resolves assemblies from the 
            'fully qualified path and name of the assembly,
            'this is a quick way to get the code to give us the
            'fully qualified path and name of the System assembly.
            '---------------------------------------------------------
            'TODO: quick fix. add method to load assembies from the template
            Dim assemblies As List(Of String) = New List(Of String)({(New System.UriBuilder()).GetType().Assembly.Location, (New Microsoft.VisualStudio.TextTemplating.Engine()).GetType.Assembly.Location, _
                                                                      (New System.Dynamic.ExpandoObject()).GetType().Assembly.Location, Me.GetType().Assembly.Location})
            If Ext.DerivedAssembly <> "" Then
                assemblies.Add(Ext.DerivedAssembly)
            End If
            Return assemblies.ToArray()
        End Get
    End Property
    'The host can provide standard imports or imports statements.
    'The engine will add these statements to the generated 
    'transformation class.
    '--------------------------------------------------------------
    Public ReadOnly Property StandardImports() As IList(Of String) Implements ITextTemplatingEngineHost.StandardImports
        Get
            Dim stdImports As List(Of String) = New List(Of String)({"System", "Microsoft.VisualBasic", "Microsoft.VisualStudio.TextTemplating", "System.Collections.Generic", "ExtensionCreator"})
            If Ext.DerivedNamespace <> "" Then
                stdImports.Add(Ext.DerivedNamespace)
            End If
            Return stdImports.ToArray()
        End Get
    End Property
    ' Called by the Engine to enquire about 
    ' the processing options you require. 
    ' If you recognize that option, return an 
    ' appropriate value. 
    ' Otherwise, pass back NULL.
    '--------------------------------------------------------------------
    Public Function GetHostOption(ByVal optionName As String) As Object Implements ITextTemplatingEngineHost.GetHostOption
        Dim returnObject As Object
        Select Case optionName
            Case "CacheAssemblies"
                returnObject = True
            Case Else
                returnObject = False
        End Select
        Return returnObject
    End Function
    'The engine calls this method based on the optional include directive
    'if the user has specified it in the text template.
    'This method can be called 0, 1, or more times.
    '---------------------------------------------------------------------
    'The included text is returned in the context parameter.
    'If the host searches the registry for the location of include files
    'or if the host searches multiple locations by default, the host can
    'return the final path of the include file in the location parameter.
    '---------------------------------------------------------------------
    Public Function LoadIncludeText(ByVal requestFileName As String, ByRef content As String, ByRef location As String) As Boolean Implements ITextTemplatingEngineHost.LoadIncludeText
        content = System.String.Empty
        location = System.String.Empty
        'If the argument is the fully qualified path of an existing file,
        'then we are done.
        '----------------------------------------------------------------
        If File.Exists(requestFileName) Then
            content = File.ReadAllText(requestFileName)
            Return True
            'This can be customized to search specific paths for the file.
            'This can be customized to accept paths to search as command line
            'arguments.
            '----------------------------------------------------------------
        Else
            Return False
        End If
    End Function
    'The engine calls this method to resolve assembly references used in
    'the generated transformation class project and for the optional 
    'assembly directive if the user has specified it in the text template.
    'This method can be called 0, 1, or more times.
    '---------------------------------------------------------------------
    Public Function ResolveAssemblyReference(ByVal assemblyReference As String) As String Implements ITextTemplatingEngineHost.ResolveAssemblyReference
        'If the argument is the fully qualified path of an existing file,
        'then we are done. (This does not do any work.)
        '----------------------------------------------------------------
        If File.Exists(assemblyReference) Then
            Return assemblyReference
        End If
        'Maybe the assembly is in the same folder as the text template that 
        'called the directive.
        '----------------------------------------------------------------
        Dim candidate As String = Path.Combine(Path.GetDirectoryName(Me.TemplateFile), assemblyReference)
        If File.Exists(candidate) Then
            Return candidate
        End If
        'This can be customized to search specific paths for the file,
        'or to search the GAC.
        '----------------------------------------------------------------
        candidate = Path.Combine(My.Application.Info.DirectoryPath, assemblyReference)
        If File.Exists(candidate) Then
            Return candidate
        End If
        'This can be customized to accept paths to search as command line
        'arguments.
        '----------------------------------------------------------------
        'If we cannot do better, return the original file name.
        Return ""
    End Function
    'The engine calls this method based on the directives the user has 
    'specified in the text template.
    'This method can be called 0, 1, or more times.
    '---------------------------------------------------------------------
    Public Function ResolveDirectiveProcessor(ByVal processorName As String) As System.Type Implements ITextTemplatingEngineHost.ResolveDirectiveProcessor
        'This host will not resolve any specific processors.
        'Check the processor name, and if it is the name of a processor the 
        'host wants to support, return the type of the processor.
        '---------------------------------------------------------------------
        If String.Compare(processorName, "XYZ", StringComparison.InvariantCultureIgnoreCase) = 0 Then
            'return typeof()
        End If
        'This can be customized to search specific paths for the file,
        'or to search the GAC.
        'If the directive processor cannot be found, throw an error.
        Throw New Exception("Directive Processor not found")
    End Function
    'A directive processor can call this method if a file name does not 
    'have a path.
    'The host can attempt to provide path information by searching 
    'specific paths for the file and returning the file and path if found.
    'This method can be called 0, 1, or more times.
    '---------------------------------------------------------------------
    Public Function ResolvePath(ByVal fileName As String) As String Implements ITextTemplatingEngineHost.ResolvePath
        If fileName Is Nothing Then
            Throw New ArgumentNullException("the file name cannot be null")
        End If
        'If the argument is the fully qualified path of an existing file,
        'then we are done.
        '----------------------------------------------------------------
        If File.Exists(fileName) Then
            Return fileName
        End If
        'Maybe the file is in the same folder as the text template that 
        'called the directive.
        '----------------------------------------------------------------
        Dim candidate As String = Path.Combine(Path.GetDirectoryName(Me.TemplateFile), fileName)
        If File.Exists(candidate) Then
            Return candidate
        End If
        'Look more places.
        '----------------------------------------------------------------
        'More code can go here...
        'If we cannot do better, return the original file name
        Return fileName
    End Function
    'If a call to a directive in a text template does not provide a value
    'for a required parameter, the directive processor can try to get it
    'from the host by calling this method.
    'This method can be called 0, 1, or more times.
    '---------------------------------------------------------------------
    Public Function ResolveParameterValue(ByVal directiveId As String, ByVal processorName As String, ByVal parameterName As String) As String Implements ITextTemplatingEngineHost.ResolveParameterValue
        If directiveId Is Nothing Then
            Throw New ArgumentNullException("the directiveId cannot be null")
        End If
        If processorName Is Nothing Then
            Throw New ArgumentNullException("the processorName cannot be null")
        End If
        If parameterName Is Nothing Then
            Throw New ArgumentNullException("the parameterName cannot be null")
        End If
        'Code to provide "hard-coded" parameter values goes here.
        'This code depends on the directive processors this host will interact with.
        'If we cannot do better, return the empty string.
        Return String.Empty
    End Function
    'The engine calls this method to change the extension of the 
    'generated text output file based on the optional output directive 
    'if the user specifies it in the text template.
    '---------------------------------------------------------------------
    Public Sub SetFileExtension(ByVal extension As String) Implements ITextTemplatingEngineHost.SetFileExtension
        'The parameter extension has a '.' in front of it already.
        '--------------------------------------------------------
        fileExtensionValue = extension
    End Sub
    'The engine calls this method to change the encoding of the 
    'generated text output file based on the optional output directive 
    'if the user specifies it in the text template.
    '---------------------------------------------------------------------
    Public Sub SetOutputEncoding(ByVal encoding As System.Text.Encoding, ByVal fromOutputDirective As Boolean) Implements ITextTemplatingEngineHost.SetOutputEncoding
        fileEncodingValue = encoding
    End Sub
    'The engine calls this method when it is done processing a text
    'template to pass any errors that occurred to the host.
    'The host can decide how to display them.
    '---------------------------------------------------------------------
    Public Sub LogErrors(ByVal errors As System.CodeDom.Compiler.CompilerErrorCollection) Implements ITextTemplatingEngineHost.LogErrors
        errorsValue = errors
    End Sub
    'This is the application domain that is used to compile and run
    'the generated transformation class to create the generated text output.
    '----------------------------------------------------------------------
    Public Function ProvideTemplatingAppDomain(ByVal content As String) As System.AppDomain Implements ITextTemplatingEngineHost.ProvideTemplatingAppDomain
        'This host will provide a new application domain each time the 
        'engine processes a text template.
        '-------------------------------------------------------------
        Return AppDomain.CreateDomain("Generation App Domain")
        'This could be changed to return the current appdomain, but new 
        'assemblies are loaded into this AppDomain on a regular basis.
        'If the AppDomain lasts too long, it will grow indefintely, 
        'which might be regarded as a leak.
        'This could be customized to cache the application domain for 
        'a certain number of text template generations (for example, 10).
        'This could be customized based on the contents of the text 
        'template, which are provided as a parameter for that purpose.
    End Function

    Public Function CreateSession() As ITextTemplatingSession Implements ITextTemplatingSessionHost.CreateSession
        Return New TextTemplatingSession()
    End Function

    Public Property Session As ITextTemplatingSession Implements ITextTemplatingSessionHost.Session
End Class
