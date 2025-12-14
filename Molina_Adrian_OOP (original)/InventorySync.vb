Imports System
Imports System.Diagnostics

''' <summary>
''' Central inventory synchronization notifier.
''' Other UI modules can subscribe to InventoryChanged to refresh their displays in real-time.
''' </summary>
Public Module InventorySync
    Public Enum InventoryChangeType
        Add = 0
        Update = 1
        Delete = 2
        Save = 3
        StockAdjusted = 4
        OrderCompleted = 5
    End Enum

    Public Class InventoryChangeEventArgs
        Inherits EventArgs
        Public Property ProductId As Integer
        Public Property ChangeType As InventoryChangeType

        Public Sub New(productId As Integer, changeType As InventoryChangeType)
            Me.ProductId = productId
            Me.ChangeType = changeType
        End Sub
    End Class

    Public Event InventoryChanged As EventHandler(Of InventoryChangeEventArgs)

    Public Sub RaiseInventoryChanged(productId As Integer, changeType As InventoryChangeType)
        Try
            RaiseEvent InventoryChanged(Nothing, New InventoryChangeEventArgs(productId, changeType))
        Catch ex As Exception
            Debug.WriteLine("InventorySync Raise error: " & ex.Message)
        End Try
    End Sub
End Module