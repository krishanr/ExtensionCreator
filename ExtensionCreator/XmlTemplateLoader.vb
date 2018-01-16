Imports System.IO
Imports System.IO.Path
Imports System.Xml.XPath

Public Class XmlTemplateLoader
    Implements IDisposable

    Public TemplateXmlFile As String = ""
    Public RenderedXmlFile As String = ""
    Public RenderedXmlDoc As XElement

    Protected Const LoadTemplateElemName As String = "loadTemplate"
    Protected Const TemplateFileAttribute As String = "from"
    Protected Const XpathAttribute As String = "select"
    Protected Const UriAttribute As String = "relUri"

    'Renders the xml template document located at XmlFile location.
    'Sets RenderedXmlDoc, and creates a temporary file that that will
    'contain its contents. The file will be disposed of when the object is
    'disposed of.
    Public Sub New(ByVal XmlFile As String)
        TemplateXmlFile = XmlFile
        RenderedXmlDoc = ProcessTemplate(XmlFile)
        RenderedXmlFile = GetTempFileName()
        RenderedXmlDoc.Save(RenderedXmlFile)
    End Sub

    'Given an XElement with name LoadTemplateElemName, attributes TemplateFileAttribute and [XpathAttribute].
    'First this will attempt to build the file system file path to load the template xml document from.
    'This will then attempt to load the template xml document as an XElement, then execute the xpath query if one exists.
    'If no xpath query exists it will simply load the xml document as an XElement.
    'In either case the results will be encapsulated by another XElement, then passed to ProcessTemplate.
    'After calling ProcessTemplate this will get the children of the resulting XElement and return them.
    'Note: RootDirectory must be the Root Directory of the xml file where TemplateElem was found.
    'Returns IEnumerable(Of XElement)
    Protected Function ProcessTemplateElem(ByVal TemplateElem As XElement, ByVal RootDirectory As String) As IEnumerable(Of XElement)
        Dim TemplateDoc As XElement
        Dim ResultContainer As New XElement("ResultContainer")


        Dim FilePath As String = TemplateElem.Attribute(TemplateFileAttribute).Value
        If Not IsPathRooted(FilePath) Then
            FilePath = RootDirectory & "\" & FilePath
        End If
        If Not My.Computer.FileSystem.FileExists(FilePath) Then
            Dim sw As New StringWriter
            sw.Write("Could not find template xml file '{0}'", FilePath)
            Throw New Exception(sw.ToString())
        End If
        Try
            TemplateDoc = XElement.Load(FilePath)
        Catch ex As Exception
            Dim sw As New StringWriter
            sw.Write("Error reading template xml file '{0}': Error Msg: {1}", FilePath, ex.Message)
            Throw New Exception(sw.ToString(), ex)
        End Try
        If TemplateElem.Attribute(XpathAttribute) Is Nothing Then
            ResultContainer.Add(TemplateDoc)
        Else
            Dim Results As IEnumerable(Of XElement)
            Dim TempDoc As New XElement("Temp")
            Try
                'Execute the query with respect to a container, since this is what normal xpath statements are written for.
                TempDoc.Add(TemplateDoc)
                Results = TempDoc.XPathSelectElements(TemplateElem.Attribute(XpathAttribute).Value)
            Catch ex As Exception
                Dim sw As New StringWriter
                sw.Write("Error with Xpath in template xml file '{0}', Xpath: '{1}': Error Msg: {2}", FilePath, TemplateElem.Attribute(XpathAttribute).Value, ex.Message)
                Throw New Exception(sw.ToString(), ex)
            End Try
            ResultContainer.Add(Results)
        End If
        Return ProcessTemplate(ResultContainer, GetDirectoryName(FilePath)).Elements
    End Function

    'Given a container of XElement with zero or more LoadTemplateElems Descendants this will select them all using the name
    'LoadTemplateElemName and TemplateFileAttribute as a filter, then replace each one with the
    'result of calling ProcessTemplateElem on it.
    'Note: Ignores stray elements with name LoadTemplateElem, and no TemplateFileAttribute.
    'Note: RootDirectory must be the Root Directory where the contents of XElem was loaded from.
    'Returns the XElem container with all valid LoadTemplateElems replaced with the content they generate.
    Protected Function ProcessTemplate(ByRef XElem As XElement, ByVal RootDirectory As String) As XElement

        'This first block of code makes any relative file/folder uris absolute using RootDirectory.
        'TODO: Must make sure that the "uri" attribute is reserved for this.
        Dim Uris As IEnumerable(Of XElement) = From elem In XElem.Descendants _
                                               Where elem.Attribute(UriAttribute) IsNot Nothing _
                                               AndAlso String.Compare(elem.Attribute(UriAttribute).Value, "false", True) <> 0 _
                                               Select elem
        Dim ElemUri As String
        For Each Uri In Uris
            ElemUri = Uri.Value
            'Make relative paths absolute
            If Not IsPathRooted(ElemUri) Then
                ElemUri = RootDirectory & "\" & ElemUri
                Uri.SetValue(ElemUri)
            End If
        Next

        Dim Results As IEnumerable(Of XElement) = From elem In XElem.Descendants _
                                                  Where elem.Name = LoadTemplateElemName _
                                                  AndAlso elem.Attribute(TemplateFileAttribute) IsNot Nothing
        Dim TemplateElems As XElement() = Results.ToArray
        For i = 0 To (TemplateElems.Count - 1)
            TemplateElems(i).ReplaceWith(ProcessTemplateElem(TemplateElems(i), RootDirectory))
        Next
        Return XElem
    End Function

    'Requires a valid xml file that exists.
    'Note: The root node of the xml document cannot be a load template element.
    Protected Function ProcessTemplate(ByVal XmlFile As String)
        Dim TemplateDoc As XElement
        Try
            TemplateDoc = XElement.Load(XmlFile)
        Catch ex As Exception
            Dim sw As New StringWriter
            sw.Write("Error reading template xml file '{0}': Error Msg: {1}", XmlFile, ex.Message)
            Throw New Exception(sw.ToString(), ex)
        End Try

        Return ProcessTemplate(TemplateDoc, GetDirectoryName(XmlFile))
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean = False ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            If My.Computer.FileSystem.FileExists(RenderedXmlFile) Then
                My.Computer.FileSystem.DeleteFile(RenderedXmlFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            End If

            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    Protected Overrides Sub Finalize()
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(False)
        MyBase.Finalize()
    End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
