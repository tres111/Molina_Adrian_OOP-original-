''' <summary>
''' Concurrent Stock Manager - Thread-safe stock operations
''' </summary>
Public Class ConcurrentStockManager
    Private Shared lockObj As New Object()

    Public Shared Function CheckStockAvailability(productId As Integer, quantity As Integer) As Boolean
        SyncLock lockObj
            Try
                Dim dt = DBmySql.GetAllProducts()
                Dim product = dt.AsEnumerable().FirstOrDefault(Function(r) CInt(r("id")) = productId)
                If product Is Nothing Then Return False
                Return CInt(product("stock")) >= quantity
            Catch ex As Exception
                Debug.WriteLine("Error checking stock: " & ex.Message)
                Return False
            End Try
        End SyncLock
    End Function

    Public Shared Function ProcessPurchase(productId As Integer, quantity As Integer) As Boolean
        SyncLock lockObj
            Try
                Dim dt = DBmySql.GetAllProducts()
                Dim product = dt.AsEnumerable().FirstOrDefault(Function(r) CInt(r("id")) = productId)
                If product Is Nothing Then Return False

                Dim currentStock = CInt(product("stock"))
                If currentStock < quantity Then Return False

                Dim newStock = currentStock - quantity
                DBmySql.UpdateStock(productId, newStock)
                Return True

            Catch ex As Exception
                Debug.WriteLine("Error processing purchase: " & ex.Message)
                Return False
            End Try
        End SyncLock
    End Function

    Public Shared Function AdjustStock(productId As Integer, adjustment As Integer) As Boolean
        SyncLock lockObj
            Try
                Dim dt = DBmySql.GetAllProducts()
                Dim product = dt.AsEnumerable().FirstOrDefault(Function(r) CInt(r("id")) = productId)
                If product Is Nothing Then Return False

                Dim currentStock = CInt(product("stock"))
                Dim newStock = currentStock + adjustment
                If newStock < 0 Then Return False

                DBmySql.UpdateStock(productId, newStock)
                Return True

            Catch ex As Exception
                Debug.WriteLine("Error adjusting stock: " & ex.Message)
                Return False
            End Try
        End SyncLock
    End Function
End Class

