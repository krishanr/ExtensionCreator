Imports Irony
Imports Irony.Samples
Imports Irony.Parsing
Imports System.IO
Imports JForm
Imports JoomlaGUI

Public Class JComponentData
    Inherits MarshalByRefObject

    Public objectName As String
    Public Table As DataTable
    Public JComponentDataTable As JComponentDataSet.JComponentDataTable
    Public mySqlFileName As String

    'Database Variables
    Public tableName As String

    Public Sub New(ByVal name As String, ByVal tablePrefix As String, ByVal _mySqlFileName As String)
        objectName = name
        tableName = tablePrefix & objectName
        mySqlFileName = _mySqlFileName
        Table = New DataTable(objectName)

        'Load Irony Parser and parse file referenced by mySqlFileName
        Dim grammar As New MySQL.MySql57Grammar()
        Dim parser As New Parsing.Parser(grammar)
        Dim parserInput As String = ""
        Dim parseTree As ParseTree
        Dim tableNode As ParseTreeNode = Nothing

        'Load the parser input from a file.
        Using sr As New StreamReader(mySqlFileName)
            parserInput = sr.ReadToEnd()
        End Using
        
        'Any exceptions thrown by the parser should be handled by calling functions
        parseTree = parser.Parse(parserInput)

        If parseTree Is Nothing OrElse parseTree.Root Is Nothing OrElse parseTree.ParserMessages.Count > 0 Then
            Dim sw As New StringWriter
            'Error occured
            sw.WriteLine("Error parsing MySQL file '{0}'", mySqlFileName)
            If parseTree.ParserMessages.Count = 0 Then
                'Unknown error
                Throw New Exception(sw.ToString())
            End If

            For Each Err As LogMessage In parseTree.ParserMessages
                sw.WriteLine("Parser Error: L {0}, C {1} : {2}", Err.Location.Line, Err.Location.Column, Err.Message)
            Next
            Throw New Exception(sw.ToString())
        End If

        'Locate table to load
        For Each child In parseTree.Root.ChildNodes
            If child.ToString() = "CREATEStmt" AndAlso child.ChildNodes(4).ChildNodes(0).Token.Value = tableName Then
                tableNode = child
                Exit For
            End If
        Next

        If tableNode Is Nothing Then
            Dim sw As New StringWriter
            sw.Write("Could not find '{0}' table in '{1}'", tableName, mySqlFileName)
            Throw New Exception(sw.ToString())
        End If

        'Load data into Table
        loadTable(tableNode.ChildNodes(5))
    End Sub

    'Paramater node: should be create_definitionDefList1 from a CREATEStmt ParseTreeNode.
    'Filling datatable: http://www.dotnetperls.com/datatable-vbnet
    Protected Sub loadTable(ByVal node As ParseTreeNode)
        'Initalize DataTable
        'Columns for DataTable are: COLUMN_NAME, DATA_TYPE, COLUMN_TYPE, CHARACTER_MAXIMUM_LENGTH, COLUMN_KEY
        'COLUMN_DEFAULT, COLUMN_COMMENT
        Table.Columns.Add("COLUMN_NAME", GetType(String))
        Table.Columns.Add("DATA_TYPE", GetType(String))
        Table.Columns.Add("COLUMN_TYPE", GetType(String))
        Table.Columns.Add("CHARACTER_MAXIMUM_LENGTH", GetType(Integer))
        Table.Columns.Add("COLUMN_KEY", GetType(String))
        Table.Columns.Add("COLUMN_DEFAULT", GetType(String))
        Table.Columns.Add("COLUMN_COMMENT", GetType(String))

        Dim dataType, colType, priKeyCol As String
        Dim colDef As ParseTreeNode
        Dim rowVals As Object()
        priKeyCol = Nothing
        'Make sure to search for primary key
        For Each col In node.ChildNodes
            'Reset row values
            rowVals = New Object(6) {DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value}
            colDef = Nothing
            dataType = Nothing
            colType = Nothing
            If col.ChildNodes.First.ToString() = "Id" Then
                rowVals(0) = col.ChildNodes(0).ChildNodes.First.Token.Value
                colDef = col.ChildNodes(1)
                For Each item In colDef.ChildNodes
                    If item.ChildNodes.Count > 0 Then
                        Select Case item.ToString()
                            Case "data_typeDef"
                                dataType = StrConv(item.ChildNodes.First.Token.Value, VbStrConv.Lowercase)
                                rowVals(1) = dataType
                                'Calculate colType depending on dataTpe
                                If item.ChildNodes.Count > 1 AndAlso dataType <> "enum" AndAlso dataType <> "set" Then
                                    Dim lengthList As ParseTreeNode = item.ChildNodes(1)
                                    Do While ((lengthList.Term Is Nothing OrElse lengthList.Term.Flags <> TermFlags.IsList) _
                                              AndAlso lengthList.ChildNodes.Count > 0)
                                        lengthList = lengthList.ChildNodes(0)
                                    Loop
                                    'If there are no child nodes then this node isn't a list
                                    If Not lengthList.ChildNodes.Count = 0 Then
                                        colType = dataType & "(" & lengthList.ChildNodes(0).Token.Value
                                        If lengthList.ChildNodes.Count > 1 Then
                                            colType = colType & "," & lengthList.ChildNodes(1).Token.Value
                                        End If
                                        colType = colType & ")"
                                        'Set CHARACTER_MAXIMUM_LENGTH to length if specified
                                        rowVals(3) = lengthList.ChildNodes(0).Token.Value
                                    Else
                                        'Set character max length from http://dev.mysql.com/doc/refman/5.7/en/string-type-overview.html
                                        Select Case dataType
                                            Case "tinytext"
                                                rowVals(3) = 255
                                            Case "text"
                                                rowVals(3) = 65535
                                            Case "mediumtext"
                                                rowVals(3) = 16777215
                                            Case "longtext"
                                                rowVals(3) = 4294967295
                                        End Select
                                        If Not IsDBNull(rowVals(3)) Then
                                            colType = dataType & "(" & rowVals(3) & ")"
                                        Else
                                            colType = dataType
                                        End If
                                    End If
                                Else
                                    colType = dataType
                                End If
                                rowVals(2) = colType
                            Case "DEFAULTOpt"
                                Dim default_val As ParseTreeNode = item.ChildNodes(1).ChildNodes(0)
                                If Not (default_val.Token.Terminal.Name = "NULL") Then
                                    rowVals(5) = default_val.Token.Value
                                End If
                            Case "COMMENTOpt"
                                rowVals(6) = item.ChildNodes(1).Token.Value
                            Case "UNIQUEOpt"
                                If item.ChildNodes.Count = 2 AndAlso item.ChildNodes(0).ToString() = "PRIMARYOpt" _
                                    AndAlso item.ChildNodes(0).ChildNodes.Count > 0 Then
                                    'PRIMARY key option
                                    rowVals(4) = "PRI"
                                End If
                        End Select
                    End If
                Next
                'Add row to table
                Table.Rows.Add(rowVals)
            ElseIf col.ChildNodes.First.ToString() = "CONSTRAINTOpt" AndAlso col.ChildNodes(1).Token.Value = "PRIMARY" Then
                'Specifying PRIMARY key
                priKeyCol = col.ChildNodes(4).ChildNodes.First.ChildNodes.First.ChildNodes.First.Token.Value
            End If
        Next

        'Set primary key
        'TODO: what to do with multiple primary key errors? update_sites_extensions from joomla1.6.sql has a primary key based on two columns..
        If priKeyCol IsNot Nothing Then
            For Each row In Table.Rows
                If row("COLUMN_NAME") = priKeyCol Then
                    row("COLUMN_KEY") = "PRI"
                End If
            Next
        End If
    End Sub

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
            End Select
            'Custom field for the 'published' column
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

    'Requires JComponentDataTable have been filled by the tasks start method.
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
