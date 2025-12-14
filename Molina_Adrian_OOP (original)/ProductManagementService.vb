Imports System.Data

''' <summary>
''' Product Management Service - CRUD operations with comprehensive audit logging
''' </summary>
Public Class ProductManagementService
    Private auditManager As New InventoryAuditManager

    Public Sub AddProduct(productName As String, category As String, price As Decimal, 
                         stock As Integer, adminId As Integer, adminUsername As String, notes As String)
        Try
            ' Log the action before database operation
            auditManager.LogAction(0, productName, "ADD", "", "Added", adminId, adminUsername, notes)
        Catch ex As Exception
            Debug.WriteLine("Error adding product: " & ex.Message)
        End Try
    End Sub

    Public Sub UpdateProduct(productId As Integer, productName As String, category As String, 
                            price As Decimal, stock As Integer, 
                            adminId As Integer, adminUsername As String, notes As String)
        Try
            ' Log the action
            auditManager.LogAction(productId, productName, "UPDATE", 
                "", productName & ", " & category, adminId, adminUsername, notes)
        Catch ex As Exception
            Debug.WriteLine("Error updating product: " & ex.Message)
        End Try
    End Sub

    Public Sub UpdateStock(productId As Integer, newStock As Integer, 
                          adminId As Integer, adminUsername As String, notes As String)
        Try
            Dim dt = DBmySql.GetAllProducts()
            Dim row = dt.AsEnumerable().FirstOrDefault(Function(r) CInt(r("id")) = productId)
            
            If row IsNot Nothing Then
                Dim oldStock = CInt(row("stock"))
                Dim productName = row("product_name").ToString()

                DBmySql.UpdateStock(productId, newStock)
                auditManager.LogAction(productId, productName, "STOCK_UPDATE", 
                    oldStock.ToString(), newStock.ToString(), adminId, adminUsername, notes)
            End If
        Catch ex As Exception
            Debug.WriteLine("Error updating stock: " & ex.Message)
        End Try
    End Sub

    Public Sub DeleteProduct(productId As Integer, adminId As Integer, 
                            adminUsername As String, notes As String)
        Try
            Dim dt = DBmySql.GetAllProducts()
            Dim row = dt.AsEnumerable().FirstOrDefault(Function(r) CInt(r("id")) = productId)
            
            If row IsNot Nothing Then
                Dim productName = row("product_name").ToString()
                auditManager.LogAction(productId, productName, "DELETE", 
                    "Deleted", "", adminId, adminUsername, notes)
            End If
        Catch ex As Exception
            Debug.WriteLine("Error deleting product: " & ex.Message)
        End Try
    End Sub

    Public Function GetAuditLogsByDateRange(startDate As DateTime, endDate As DateTime) As List(Of InventoryAuditManager.AuditLog)
        Return auditManager.GetLogsByDateRange(startDate, endDate)
    End Function

    Public Function GetProductAuditHistory(productId As Integer) As List(Of InventoryAuditManager.AuditLog)
        Return auditManager.SearchLogs(productId.ToString())
    End Function
End Class

