Imports System.IO
Imports System.IO.Path
Imports System.Xml.Serialization
Imports System.Xml

Public Class frmJForm
    Private _frm As New form
    Private _outputFileName As String
    Private _saveMethod As Integer = Form.SaveMethod.Context
    Private _firstSelection As Boolean = True
    'Tells users of this form that it doesn't have a show dialog option.
    Protected _showDialogEnabled As Boolean = False

#Region "Properties"

    Public ReadOnly Property showDialogEnabled As Boolean
        Get
            Return _showDialogEnabled
        End Get
    End Property


    Public Property SaveMethod() As Integer
        Get
            Return _saveMethod
        End Get
        Set(ByVal value As Integer)
            _saveMethod = value
        End Set
    End Property

    Public Property OutputFileName() As String
        Get
            Return _outputFileName
        End Get
        Set(ByVal value As String)
            _outputFileName = value
        End Set
    End Property

    Public Property Form() As form
        Get
            Return _frm
        End Get
        Set(ByVal value As form)
            _frm = value
        End Set
    End Property

#End Region

    Public Overridable Sub LoadForm(ByVal FileName As String)
        Form.Load(FileName)
    End Sub

    Public Overridable Sub Save()
        If OutputFileName = "" Then
            'TODO: add ex handling
        End If
        'TODO: add a NULL save method enum to form..
        If SaveMethod = 0 Then
            'If the save method is not set, just save it as a form.
            SaveMethod = form.SaveMethod.Form
        End If
        Form.Save(SaveMethod, OutputFileName)
    End Sub

    Private Sub frmJForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Form.loadTypes(Me.JFormDataSet)
            Form.loadCustomTypes(Me.JFormDataSet)
        Catch ex As Exception
            MsgBox(ex.Message)
            Form = New form
        End Try
    End Sub

End Class
