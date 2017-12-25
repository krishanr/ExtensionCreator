Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports JoomlaLibrary
Imports System.IO

<TestClass()> Public Class JComponentDataTest

    Private Sub TestLoadTableHelper(ByVal myJComponentData As JComponentData)
        Console.WriteLine(myJComponentData.tableName & " table in " & Path.GetFileName(myJComponentData.mySqlFileName))
        Console.WriteLine("COLUMN_NAME--DATA_TYPE--COLUMN_TYPE--CHARACTER_MAXIMUM_LENGTH--COLUMN_KEY--COLUMN_DEFAULT--COLUMN_COMMENT")
        For Each row As Data.DataRow In myJComponentData.Table.Rows
            For Each col As Data.DataColumn In myJComponentData.Table.Columns
                If IsDBNull(row(col)) Then
                    'Use "-+" as the column separator to distinguish NULL values from strings.
                    Console.Write("NULL" & "-+")
                Else
                    Console.Write(row(col) & "--")
                End If
            Next
            Console.WriteLine("")
        Next
    End Sub

    <TestMethod()> Public Sub TestLoadTable()
        Dim myJComponentData As New JComponentData("customers", "", _
                                         My.Application.Info.DirectoryPath & "\..\..\MySql\classicautos_database.sql")

        TestLoadTableHelper(myJComponentData)

        Dim tables As String() = New String(12) {"assets", "banners", "banner_clients", "categories", _
                                                "contact_details", "content", "extensions", "menu", "newsfeeds", _
                                                "updates", "update_sites_extensions", "template_styles", "usergroups"}
        For Each table In tables
            Console.WriteLine("-----------------------------------------------------------")
            myJComponentData = New JComponentData(table, "#__", _
                                             My.Application.Info.DirectoryPath & "\..\..\MySql\joomla1.6.sql")
            TestLoadTableHelper(myJComponentData)
        Next

        Dim tables1 As String() = New String(8) {"vendors", "divisions", "projects", "departments", _
                                                "purchases", "line_items", "activities", "purchase_items", "purchase_comments"}

        For Each table In tables1
            Console.WriteLine("-----------------------------------------------------------")
            myJComponentData = New JComponentData(table, "#__po_", _
                                             My.Application.Info.DirectoryPath & "\..\..\MySql\purchase_order_app_test_database.sql")
            TestLoadTableHelper(myJComponentData)
        Next
    End Sub

End Class