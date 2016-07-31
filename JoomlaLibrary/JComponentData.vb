Imports MySql.Data
Imports System.IO
Imports JForm
Imports JoomlaGUI

Public Class JComponentData
    Inherits MarshalByRefObject

    Public objectName As String
    Public Table As DataTable
    Public JComponentDataTable As JComponentDataSet.JComponentDataTable

    'Database Variables
    Public tableName As String
    Protected conn As New MySql.Data.MySqlClient.MySqlConnection
    Protected connectionString As String
    Protected infoSqlQuery As String

    Public Sub New(ByVal name As String, ByVal tablePrefix As String, ByVal server As String, ByVal databaseName As String, _
                   ByVal userid As String, ByVal password As String)
        objectName = name
        tableName = tablePrefix & objectName
        Table = New DataTable(objectName)
        SetDatabaseVars(server, databaseName, userid, password)

        'Load data
        'TODO: Handle the exceptions that loadtable throws
        'TODO: determine primary key from table
        loadTable()
        'TODO: Handle empty rows
    End Sub

    Private Sub SetDatabaseVars(ByVal server As String, ByVal databaseName As String, _
                   ByVal userid As String, ByVal password As String)
        'create connection string
        Dim sw As New StringWriter
        sw.Write("server={0};uid={1};pwd={2};database={3};", server, userid, password, databaseName)
        connectionString = sw.ToString

        'create query
        sw = New StringWriter
        sw.Write("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{0}' AND TABLE_NAME = '{1}' ORDER BY 'COLUMN_COMMENT';", _
                 databaseName, tableName)
        infoSqlQuery = sw.ToString()
    End Sub

    Private Sub loadTable()
        Try
            'Open connection
            conn.ConnectionString = connectionString

            conn.Open()

            Dim cmd As New MySql.Data.MySqlClient.MySqlCommand
            cmd.Connection = conn
            cmd.CommandText = infoSqlQuery
            cmd.Prepare()
            Using myReader As MySql.Data.MySqlClient.MySqlDataReader = cmd.ExecuteReader()
                Table.Load(myReader)
            End Using
        Catch ex As MySql.Data.MySqlClient.MySqlException
            Dim msg As String = "MySql Error: "
            Select Case ex.Number
                Case 0
                    msg &= "Cannot connect to server. Contact administrator"
                Case 1045
                    msg &= "Invalid username/password, please try again"
                Case Else
                    msg &= ex.Number & " has occurred: " & ex.Message
            End Select
            Throw New Exception(msg, ex)
        Finally
            'Assuming close doesn't throw any exceptions
            conn.Close()
        End Try
    End Sub

    'Requires Table to have been filled by loadTable.
    'NOTE: assumes the table has only 1 primary key.
    Protected Friend Function getPrimaryKey() As String
        Dim Row As DataRow
        For Each Row In Table.Rows
            If StrConv(Row("COLUMN_KEY"), VbStrConv.Uppercase) = "PRI" Then
                Return Row("COLUMN_NAME")
            End If
        Next
        Return ""
    End Function

    'Requires Table to have been filled by loadTable.
    Protected Friend Sub FillForm(ByRef Form As JForm.form)
        Dim Row As DataRow
        Dim Fieldset As New JFieldset
        'TODO: make this a param
        Fieldset.Name = "basic"
        For Each Row In Table.Rows
            Dim Field As JField
            Select Case StrConv(Row("DATA_TYPE"), VbStrConv.Lowercase)
                Case "varchar", "char", "tinytext"
                    Dim tempField As New JForm.text
                    If Not IsDBNull(Row("CHARACTER_MAXIMUM_LENGTH")) Then
                        tempField.MaxLength = Row("CHARACTER_MAXIMUM_LENGTH")
                    End If
                    Field = tempField
                    'TODO: Doesn't handel floats and doubles
                Case "int", "smallint", "mediumint", "bigint"
                    Field = New JForm.int
                Case "tinyint"
                    If StrConv(Row("COLUMN_TYPE"), VbStrConv.Lowercase) = "tinyint(1)" Then
                        Field = New JForm.bool
                    Else
                        Dim tempField As New JForm.int
                        tempField.Size = 5
                        Field = tempField
                    End If
                Case "text", "mediumtext", "longtext", "blob", "mediumblob", "longblob"
                    Field = New JForm.textarea
                Case "date", "datetime", "timestamp", "time", "year"
                    Dim tempField As New JForm.calendar
                    Select Case StrConv(Row("DATA_TYPE"), VbStrConv.Lowercase)
                        Case "datetime", "timestamp"
                            tempField.Format = "%Y-%m-%d %H:%M:%S"
                        Case "date"
                            tempField.Format = "%Y-%m-%d"
                        Case "time"
                            tempField.Format = "%H:%M:%S"
                        Case "year"
                            tempField.Format = "%Y"
                    End Select
                    Field = tempField
                Case Else
                    'Note: If type can't be determined we give it JField
                    Field = New JForm.JField
                    Field.Description = "The field is of type " & Row("COLUMN_TYPE") & "."
            End Select
            If Row("COLUMN_NAME") = "published" Then
                Dim PField As New JForm.list
                PField.Label = "JSTATUS"
                PField.Description = "JFIELD_PUBLISHED_DESC"
                PField.CSSClass = "chzn-color-state"
                PField.Filter = "intval"
                PField.Options = New List(Of JOption)
                Dim myOpt = New JOption
                myOpt.OptValue = "1"
                myOpt.OptText = "JPUBLISHED"
                PField.Options.Add(myOpt)
                myOpt = New JOption
                myOpt.OptValue = "0"
                myOpt.OptText = "JUNPUBLISHED"
                PField.Options.Add(myOpt)
                myOpt = New JOption
                myOpt.OptValue = "-2"
                myOpt.OptText = "JTRASHED"
                PField.Options.Add(myOpt)
                Field = PField
            End If
            Field.Name = Row("COLUMN_NAME")
            Field.Description &= Row("COLUMN_COMMENT")
            If Not IsDBNull(Row("COLUMN_DEFAULT")) Then
                Field.DefaultValue = Row("COLUMN_DEFAULT")
            End If
            Fieldset.Fields.Add(Field)
        Next
        Form.FieldSets.Add(Fieldset)
    End Sub

    'Requires Table to have been filled by loadTable.
    Protected Friend Sub FillComponentForm(ByRef JComponentDataTable As JComponentDataSet.JComponentDataTable)
        For Each row In Table.Rows
            Dim NewRow As JComponentDataSet.JComponentRow = JComponentDataTable.NewJComponentRow
            With NewRow
                .Field = row("COLUMN_NAME")
                .ListLabel = StrConv(Replace(row("COLUMN_NAME"), "_", " "), VbStrConv.ProperCase)
            End With
            JComponentDataTable.AddJComponentRow(NewRow)
        Next
    End Sub

    'Requires Table to have been filled by loadTable and JComponentDataTable have been filled by the tasks start method.
    Protected Friend Sub FillListForm(ByRef Form As JForm.form, ByVal TableAlias As String)
        Dim Fieldset As New JFieldset
        Dim TempJComponentDataTable As DataTable = CType(JComponentDataTable, DataTable)
        Dim query = _
                    From FieldData In Table.AsEnumerable() _
                    Join JComponentData In TempJComponentDataTable.AsEnumerable() _
                    On FieldData.Field(Of String)("COLUMN_NAME") Equals _
                            JComponentData.Field(Of String)("Field") _
                    Where JComponentData.Field(Of Boolean)("Selectable") = True _
                    Or JComponentData.Field(Of Boolean)("Searchable") = True _
                    Or JComponentData.Field(Of Boolean)("ShowInList") = True
                    Select New With _
                    { _
                        .Field = JComponentData.Field(Of String)("Field"), _
                        .Default = FieldData.Field(Of String)("COLUMN_DEFAULT"), _
                        .Selectable = JComponentData.Field(Of Boolean)("Selectable"), _
                        .Searchable = JComponentData.Field(Of Boolean)("Searchable"), _
                        .ShowInList = JComponentData.Field(Of Boolean)("ShowInList"), _
                        .Linkable = JComponentData.Field(Of Boolean)("Linkable") _
                    }

        Fieldset.Name = "filters"
        For Each Row In query
            If Row.Selectable = True Then
                Dim Field As New JForm.sql
                Field.Name = Row.Field
                Field.KeyField = Row.Field
                Field.ValueField = Row.Field
                Field.CSSClass = "inputbox"
                Field.OnChange = "this.form.submit()"
                Dim sw As New StringWriter
                'TODO: get table alias...
                sw.Write("select distinct({0}) from #__{1} order by {0} ASC", Row.Field, objectName)
                Field.Query = sw.ToString()
                Field.Name = "filter_" & Row.Field
                Fieldset.Fields.Add(Field)
            End If
        Next
        Form.FieldSets.Add(Fieldset)

        Dim AFieldset As New JFieldset
        AFieldset.Name = "Rest"
        Dim AField As New JForm.list
        AField.Name = "search_filter"
        AField.CSSClass = "inputbox"
        Dim SearchOptions As New List(Of JOption)
        For Each row In query
            If row.Searchable = True Then
                Dim SearchOption As New JOption
                Dim myTempField As New JField
                myTempField.Name = row.Field
                SearchOption.OptText = myTempField.Label
                SearchOption.OptValue = TableAlias & "." & row.Field
                SearchOptions.Add(SearchOption)
            End If
        Next
        AField.Options = SearchOptions
        AFieldset.Fields.Add(AField)
        Form.FieldSets.Add(AFieldset)

        Dim ColumnFieldset As New JFieldset
        ColumnFieldset.Name = "columns"
        For Each Row In query
            If Row.ShowInList = True Then
                Dim Field As New JForm.column
                Field.Name = Row.Field
                Field.ReadOnlyField = True
                Field.CSSClass = "center"
                If Row.Linkable = True Then
                    Field.Link = True
                End If
                ColumnFieldset.Fields.Add(Field)
            End If
        Next
        Form.FieldSets.Add(ColumnFieldset)
    End Sub

End Class
