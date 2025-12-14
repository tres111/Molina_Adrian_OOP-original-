Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Class DBConnection
    Private ReadOnly connectionString As String =
        "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private mainForm As Form1

    ' ----------------------------
    ' GET CONNECTION
    ' ----------------------------
    Public Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' ----------------------------
    ' GET STOCK BY ITEM NAME
    ' ----------------------------
    Public Function GetStock(itemName As String) As Integer
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT stock FROM products WHERE product_name = @name LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@name", itemName)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return Convert.ToInt32(result)
                Else
                    Return 0
                End If
            End Using
        End Using
    End Function

    ' ----------------------------
    ' REDUCE STOCK AFTER CHECKOUT
    ' ----------------------------
    Public Sub ReduceStock(itemName As String, qty As Integer)
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "UPDATE products SET stock = stock - @qty WHERE product_name = @name"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@qty", qty)
                cmd.Parameters.AddWithValue("@name", itemName)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

End Class