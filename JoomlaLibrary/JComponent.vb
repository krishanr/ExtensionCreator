Imports System.Xml
Imports System.IO
Imports JForm
Imports ExtensionCreator
Imports JoomlaGUI
Imports System.Windows.Forms

Public Class JComponent
    Inherits JExtension

    Public Sub New(ByVal xmlfile As String, ByVal title As String, ByVal extensionDocLoader As XmlTemplateLoader)
        MyBase.New(xmlfile, title, extensionDocLoader)
    End Sub

#Region "Task Start and End functions"

    Public Function StartSiteForm() As Exception
        Try
            If Not TaskRan Then
                LoadData()
            End If
            Return Nothing
        Catch ex As Exception
            Dim sw As New StringWriter
            sw.Write("Error starting task SiteForm: {0}", ex.Message)
            Return New Exception(sw.ToString, ex)
        End Try
    End Function

    Public Function EndSiteForm() As Exception
        Try
            If Not TaskRan Then
                GetActiveTaskCollection().Remove("JDataTable")
            End If
            Return Nothing
        Catch ex As Exception
            'TODO: better messages
            Return ex
        End Try
    End Function

    Private _myComponentForm As frmJComponent = Nothing
    Public Function StartDatabaseToAdminConnector() As Exception
        If Not TaskRan Then
            _myComponentForm = New frmJComponent
            Try
                LoadData()
                Dim ActiveTask As Microsoft.VisualBasic.Collection = GetActiveTaskCollection()
                Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                'TODO: check if TaskParameters has table
                _myComponentForm.Text = "Database To Admin Connector Parameters for Table " & ActiveTask.Item("TaskParameters").Item("table")
                myJComponentData.FillComponentForm(_myComponentForm.JComponent)
                OnInteractUser(AddressOf ComponentFormShowDialog)
                If Not _ComponentFormResult Then
                    'TODO: (Optional) disable form close...
                    Throw New Exception("Unable to run task 'DatabaseToAdminConnector' for table '" _
                                        & ActiveTask.Item("TaskParameters").Item("table") & "'. Parameters form was not submitted. ")
                End If
            Catch ex As Exception
                Dim sw As New StringWriter
                sw.Write("Error starting task DatabaseToAdminConnector: {0}", ex.Message)
                Return New Exception(sw.ToString, ex)
            Finally
                _myComponentForm.Dispose()
                _myComponentForm = Nothing
            End Try
            Return Nothing
        Else
            Return Nothing
        End If
    End Function

    Public Function EndDatabaseToAdminConnector() As Exception
        Try
            If Not TaskRan Then
                GetActiveTaskCollection().Remove("JDataTable")
            End If
            Return Nothing
        Catch ex As Exception
            Return ex
        End Try
    End Function

    'Note that the code for this function is copied from the task DatabaseToAdminConnector
    Public Function StartBasicBackend() As Exception
        If Not TaskRan Then
            _myComponentForm = New frmJComponent
            Try
                LoadData()
                Dim ActiveTask As Microsoft.VisualBasic.Collection = GetActiveTaskCollection()
                Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                'TODO: check if TaskParameters has table
                _myComponentForm.Text = "Basic Backend Parameters for Table " & ActiveTask.Item("TaskParameters").Item("table")
                myJComponentData.FillComponentForm(_myComponentForm.JComponent)
                OnInteractUser(AddressOf ComponentFormShowDialog)
                If Not _ComponentFormResult Then
                    'TODO: (Optional) disable form close...
                    Throw New Exception("Unable to run task 'BasicBackend' for table '" _
                                        & ActiveTask.Item("TaskParameters").Item("table") & "'. Parameters form was not submitted. ")
                End If
            Catch ex As Exception
                Dim sw As New StringWriter
                sw.Write("Error starting task BasicBackend: {0}", ex.Message)
                Return New Exception(sw.ToString, ex)
            Finally
                _myComponentForm.Dispose()
                _myComponentForm = Nothing
            End Try
            Return Nothing
        Else
            Return Nothing
        End If
    End Function

    Private _ComponentFormResult As Boolean = False
    Protected Function ComponentFormShowDialog() As Boolean
        _ComponentFormResult = False
        If (_myComponentForm.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            _ComponentFormResult = True
            Dim myJComponentData As JComponentData = GetActiveTaskCollection().Item("JDataTable")
            myJComponentData.JComponentDataTable = _myComponentForm.JComponent
        End If
        Return _ComponentFormResult
    End Function

    Public Function EndBasicBackend() As Exception
        Try
            If Not TaskRan Then
                GetActiveTaskCollection().Remove("JDataTable")
            End If
            Return Nothing
        Catch ex As Exception
            Return ex
        End Try
    End Function

    Public Function StartHelpers() As Exception
        Return Nothing
    End Function

    Public Function EndHelpers() As Exception
        Return Nothing
    End Function

    Public Function StartField() As Exception
        Return Nothing
    End Function

    Public Function EndField() As Exception
        Return Nothing
    End Function

#End Region

#Region "Functions called from files"

    'DO not use anymore, use the CreateForm in Utility Functions
    Public Overloads Function CreateForm(ByVal myTempFileLoc As String, ByVal FileNode As XElement)
        Dim myForm As New frmJForm
        Try
            myTempFileLoc = Path.GetTempFileName()
            myForm.OutputFileName = myTempFileLoc
            myForm.SaveMethod = JForm.form.SaveMethod.Form
            myForm.Text = GetActiveTaskName() & " Form"
            Try
                Dim ActiveTask As Microsoft.VisualBasic.Collection = GetActiveTaskCollection()
                If ActiveTask.Contains("JDataTable") Then
                    Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                    myJComponentData.FillForm(myForm.Form)
                Else
                    'TODO: change title
                    Throw New Exception("Unable to fill form")
                    'TODO: add text to the top of the form saying that jdatatable was not loaded so form, 
                    'couldn't be filled.
                End If
            Catch ex As Exception
                Throw New Exception(ex.Message)
                'TODO: add text to the top of the form saying there was an error trying to load the form.
            End Try
            If myForm.ShowDialog() = DialogResult.OK Then
                myForm.Close()
                Return myTempFileLoc
            Else
                'TODO: Tell them no form was created?
                myForm.Close()
                Return ""
            End If
        Catch ex As Exception
            myTempFileLoc = ""
            myForm.Close()
            Return ex
        End Try
    End Function

#End Region

#Region "Utility Functions"

    'Requires JDataTable to be in the active task collection, and have been filled by the site form or database to admin connector,
    'if FillMethod <> ""
    'OR
    'if FillMethod = "" then just creates an empty form with title FormName and saves using SaveMethod.
    Public Overloads Function CreateForm(ByVal FormName As String, ByVal SaveMethod As JForm.form.SaveMethod, Optional ByVal FillMethod As String = "") As String
        Dim myForm As New frmJForm
        Dim ActiveTask As Microsoft.VisualBasic.Collection
        myForm.SaveMethod = SaveMethod
        myForm.Text = FormName
        If FillMethod <> "" Then
            ActiveTask = GetActiveTaskCollection()
            If Not ActiveTask.Contains("JDataTable") Then
                'TODO: change title
                Throw New Exception("Unable to fill form, data to fill the form is not available.")
                'TODO: add text to the top of the form saying that jdatatable was not loaded so form, 
                'couldn't be filled.
            End If
        End If
        Select Case FillMethod
            Case "EditForm"
                Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                myJComponentData.FillForm(myForm.Form)
            Case "ListForm"
                Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                'Note: only for task database to admin connector
                'TODO: check if TaskParameters has tableAlias
                myJComponentData.FillListForm(myForm.Form, ActiveTask.Item("TaskParameters").Item("tableAlias"))
        End Select
        Return MyBase.CreateForm(myForm)
    End Function

    Protected Overrides Sub FillJform(ByRef myForm As JForm.frmJForm, ByVal FileNode As System.Xml.Linq.XElement)
        If FileNode.Attribute("formFillMethod") IsNot Nothing And Not TaskRan Then
            Dim ActiveTask As Microsoft.VisualBasic.Collection = GetActiveTaskCollection()
            'TODO: not robust since they can use these 'fillMethod's on any file node, even when JDataTable was not loaded.
            Select Case FileNode.Attribute("formFillMethod").Value
                Case "EditForm"
                    Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                    myJComponentData.FillForm(myForm.Form)
                Case "ListForm"
                    Dim myJComponentData As JComponentData = ActiveTask.Item("JDataTable")
                    'Note: only for task database to admin connector
                    'TODO: check if TaskParameters has tableAlias
                    myJComponentData.FillListForm(myForm.Form, ActiveTask.Item("TaskParameters").Item("tableAlias"))
            End Select
        End If
    End Sub
#End Region

#Region "Helper Functions"

    Private Sub LoadData()
        Dim ActiveTask As Microsoft.VisualBasic.Collection = GetActiveTaskCollection()
        Try
            Dim myJComponentData As New JComponentData(ActiveTask.Item("TaskParameters").Item("table"), ExtensionParameters.MyItem("tablePrefix"), _
                                         ExtensionParameters.MyItem("server"), ExtensionParameters.MyItem("databaseName"), _
                                         ExtensionParameters.MyItem("userid"), ExtensionParameters.MyItem("password"))
            ActiveTask.Add(myJComponentData, "JDataTable")
            Dim myPrimaryKey As String = myJComponentData.getPrimaryKey()
            Dim myTaskParameters As Dictionary(Of String, String) = ActiveTask.Item("TaskParameters")
            'NOTE: assumes the table has only 1 primary key.
            If myPrimaryKey <> "" Then
                If myTaskParameters.ContainsKey("primaryKey") Then
                    myTaskParameters.Remove("primaryKey")
                End If
                myTaskParameters.Add("primaryKey", myPrimaryKey)
            Else
                Throw New Exception("Couldn't find the primary key for " & myJComponentData.tableName)
            End If
            'Check if the table has a published field and the result to the ActiveTask Parameters.
            For Each row In myJComponentData.Table.Rows
                If row("COLUMN_NAME") = "published" Then
                    If myTaskParameters.ContainsKey("hasPublishedField") Then
                        myTaskParameters.Remove("hasPublishedField")
                    End If
                    myTaskParameters.Add("hasPublishedField", "True")
                End If
            Next
        Catch ex As Exception
            Throw New Exception("Error loading MySQL data: " & ex.Message, ex)
        End Try
    End Sub

#End Region

End Class