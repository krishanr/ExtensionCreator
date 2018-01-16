Imports System.Xml
Imports System.IO
Imports JoomlaForm
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

#Region "Utility Functions"

    'Requires JDataTable to be in the active task collection, and have been filled by the site form or database to admin connector,
    'if FillMethod <> ""
    'OR
    'if FillMethod = "" then just creates an empty form with title FormName and saves using SaveMethod.
    'Public Overloads Function createform(ByVal formname As String, ByVal savemethod As JForm.form.SaveMethod, Optional ByVal fillmethod As String = "") As String
    '    Dim myform As New frmJForm
    '    Dim activetask As Microsoft.VisualBasic.Collection
    '    myform.SaveMethod = savemethod
    '    myform.Text = formname
    '    If fillmethod <> "" Then
    '        activetask = GetActiveTaskCollection()
    '        If Not activetask.Contains("jdatatable") Then
    '            'todo: change title
    '            Throw New Exception("unable to fill form, data to fill the form is not available.")
    '            'todo: add text to the top of the form saying that jdatatable was not loaded so form, 
    '            'couldn't be filled.
    '        End If
    '    End If
    '    Select Case fillmethod
    '        Case "editform"
    '            Dim myjcomponentdata As JComponentData = activetask.Item("jdatatable")
    '            myjcomponentdata.FillForm(myform.Form)
    '        Case "listform"
    '            Dim myjcomponentdata As JComponentData = activetask.Item("jdatatable")
    '            'note: only for task database to admin connector
    '            'todo: check if taskparameters has tablealias
    '            myjcomponentdata.FillListForm(myform.Form, activetask.Item("taskparameters").item("tablealias"))
    '    End Select
    '    Return MyBase.createform(myform)
    'End Function

    Protected Overrides Sub FillJform(ByRef myForm As frmJForm, ByVal FileNode As System.Xml.Linq.XElement)
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
                                         ExtensionParameters.MyItem("mySqlFile"))
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