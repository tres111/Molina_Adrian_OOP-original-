Imports MySql.Data.MySqlClient

''' <summary>
''' Stock Management Service - Handles all stock operations with audit logging
''' Implements the Stock Validation Algorithm and admin controls
''' </summary>
Public Module StockManagementService

    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    ''' <summary>
    ''' Get database connection
    ''' </summary>
    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ''' <summary>
    ''' Add stock (Admin operation) - Logs to audit trail
    ''' </summary>
    Public Function AddStock(productId As Integer, quantityToAdd As Integer, adminId As Integer, adminName As String, reason As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Get current stock and product details
                Dim currentStock = GetStockById(productId)
                Dim productRow = DBmySql.GetProductById(productId)
                If productRow Is Nothing Then
                    Return New Tuple(Of Boolean, String)(False, "Product not found")
                End If

                Dim productName = productRow("product_name").ToString()
                Dim newStock = currentStock + quantityToAdd

                ' Begin transaction for atomicity
                Dim transaction = conn.BeginTransaction()
                Try
                    ' Update product stock
                    Dim updateQuery = "UPDATE products SET stock = @newStock, last_stock_update = NOW() WHERE id = @productId"
                    Using updateCmd = New MySqlCommand(updateQuery, conn, transaction)
                        updateCmd.Parameters.AddWithValue("@newStock", newStock)
                        updateCmd.Parameters.AddWithValue("@productId", productId)
                        updateCmd.ExecuteNonQuery()
                    End Using

                    ' Log the stock update
                    LogStockUpdate(productId, productName, adminId, adminName, currentStock, newStock, quantityToAdd, "ADD", reason, conn, transaction)

                    ' Check if low stock alert should be triggered
                    CheckAndCreateLowStockAlert(productId, productName, newStock, conn, transaction)

                    transaction.Commit()
                    Return New Tuple(Of Boolean, String)(True, $"Stock updated successfully. {productName}: {currentStock} ? {newStock}")

                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adding stock: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Reduce stock (During order processing) - Logs to audit trail
    ''' </summary>
    Public Function ReduceStock(productId As Integer, quantityToReduce As Integer, reason As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Get current stock
                Dim currentStock = GetStockById(productId)
                Dim productRow = DBmySql.GetProductById(productId)
                If productRow Is Nothing Then
                    Return New Tuple(Of Boolean, String)(False, "Product not found")
                End If

                Dim productName = productRow("product_name").ToString()

                ' Stock Validation Algorithm: Check availability
                If currentStock < quantityToReduce Then
                    Return New Tuple(Of Boolean, String)(False, $"Insufficient stock. Available: {currentStock}, Requested: {quantityToReduce}")
                End If

                Dim newStock = currentStock - quantityToReduce

                ' Begin transaction
                Dim transaction = conn.BeginTransaction()
                Try
                    ' Update product stock (ensures stock never goes negative)
                    Dim updateQuery = "UPDATE products SET stock = CASE WHEN stock - @qty < 0 THEN 0 ELSE stock - @qty END, last_stock_update = NOW() WHERE id = @productId"
                    Using updateCmd = New MySqlCommand(updateQuery, conn, transaction)
                        updateCmd.Parameters.AddWithValue("@qty", quantityToReduce)
                        updateCmd.Parameters.AddWithValue("@productId", productId)
                        updateCmd.ExecuteNonQuery()
                    End Using

                    ' Log the stock reduction
                    LogStockUpdate(productId, productName, 0, "SYSTEM", currentStock, newStock, quantityToReduce, "REDUCE", reason, conn, transaction)

                    ' Check if low stock alert should be triggered
                    CheckAndCreateLowStockAlert(productId, productName, newStock, conn, transaction)

                    transaction.Commit()
                    Return New Tuple(Of Boolean, String)(True, $"Stock reduced successfully. {productName}: {currentStock} ? {newStock}")

                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error reducing stock: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Adjust stock (Inventory correction) - Logs to audit trail
    ''' </summary>
    Public Function AdjustStock(productId As Integer, newStockValue As Integer, adminId As Integer, adminName As String, reason As String) As Tuple(Of Boolean, String)
        Try
            If newStockValue < 0 Then
                Return New Tuple(Of Boolean, String)(False, "Stock cannot be negative")
            End If

            Using conn = GetConnection()
                conn.Open()

                Dim currentStock = GetStockById(productId)
                Dim productRow = DBmySql.GetProductById(productId)
                If productRow Is Nothing Then
                    Return New Tuple(Of Boolean, String)(False, "Product not found")
                End If

                Dim productName = productRow("product_name").ToString()
                Dim quantityChange = newStockValue - currentStock

                ' Begin transaction
                Dim transaction = conn.BeginTransaction()
                Try
                    ' Update product stock
                    Dim updateQuery = "UPDATE products SET stock = @newStock, last_stock_update = NOW() WHERE id = @productId"
                    Using updateCmd = New MySqlCommand(updateQuery, conn, transaction)
                        updateCmd.Parameters.AddWithValue("@newStock", newStockValue)
                        updateCmd.Parameters.AddWithValue("@productId", productId)
                        updateCmd.ExecuteNonQuery()
                    End Using

                    ' Log the adjustment
                    LogStockUpdate(productId, productName, adminId, adminName, currentStock, newStockValue, quantityChange, "CORRECTION", reason, conn, transaction)

                    ' Check if low stock alert should be triggered
                    CheckAndCreateLowStockAlert(productId, productName, newStockValue, conn, transaction)

                    transaction.Commit()
                    Return New Tuple(Of Boolean, String)(True, $"Stock adjusted successfully. {productName}: {currentStock} ? {newStockValue}")

                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adjusting stock: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Get current stock for a product by ID
    ''' </summary>
    Public Function GetStockById(productId As Integer) As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query = "SELECT stock FROM products WHERE id = @id LIMIT 1"
                Using cmd = New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", productId)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        Return CInt(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error getting stock: " & ex.Message)
        End Try
        Return 0
    End Function

    ''' <summary>
    ''' Log stock update to audit trail
    ''' </summary>
    Private Sub LogStockUpdate(productId As Integer, productName As String, adminId As Integer, adminName As String,
                               previousStock As Integer, newStock As Integer, quantityChange As Integer,
                               changeType As String, reason As String, conn As MySqlConnection, transaction As MySqlTransaction)
        Try
            Dim logQuery = "INSERT INTO stock_update_logs (product_id, product_name, admin_id, admin_name, previous_stock, new_stock, quantity_change, change_type, reason, timestamp) " &
                          "VALUES (@productId, @productName, @adminId, @adminName, @prevStock, @newStock, @qtyChange, @changeType, @reason, NOW())"
            Using logCmd = New MySqlCommand(logQuery, conn, transaction)
                logCmd.Parameters.AddWithValue("@productId", productId)
                logCmd.Parameters.AddWithValue("@productName", productName)
                logCmd.Parameters.AddWithValue("@adminId", If(adminId > 0, adminId, DBNull.Value))
                logCmd.Parameters.AddWithValue("@adminName", If(String.IsNullOrEmpty(adminName), "SYSTEM", adminName))
                logCmd.Parameters.AddWithValue("@prevStock", previousStock)
                logCmd.Parameters.AddWithValue("@newStock", newStock)
                logCmd.Parameters.AddWithValue("@qtyChange", quantityChange)
                logCmd.Parameters.AddWithValue("@changeType", changeType)
                logCmd.Parameters.AddWithValue("@reason", If(String.IsNullOrEmpty(reason), "", reason))
                logCmd.ExecuteNonQuery()
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error logging stock update: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Check and create low stock alert if needed
    ''' </summary>
    Private Sub CheckAndCreateLowStockAlert(productId As Integer, productName As String, currentStock As Integer,
                                            conn As MySqlConnection, transaction As MySqlTransaction)
        Try
            ' Get stock thresholds
            Dim lowThreshold = GetStockSetting("LOW_STOCK_THRESHOLD", 10)
            Dim criticalThreshold = GetStockSetting("CRITICAL_STOCK_THRESHOLD", 5)

            Dim alertType = ""
            If currentStock = 0 Then
                alertType = "OUT_OF_STOCK"
            ElseIf currentStock <= criticalThreshold Then
                alertType = "CRITICAL"
            ElseIf currentStock <= lowThreshold Then
                alertType = "WARNING"
            End If

            If Not String.IsNullOrEmpty(alertType) Then
                ' Check if alert already exists
                Dim checkQuery = "SELECT id FROM low_stock_alerts WHERE product_id = @productId AND status IN ('PENDING', 'ACKNOWLEDGED') LIMIT 1"
                Using checkCmd = New MySqlCommand(checkQuery, conn, transaction)
                    checkCmd.Parameters.AddWithValue("@productId", productId)
                    Dim existingAlert = checkCmd.ExecuteScalar()

                    If existingAlert Is Nothing Then
                        ' Create new alert
                        Dim alertQuery = "INSERT INTO low_stock_alerts (product_id, product_name, current_stock, alert_threshold, alert_type, status, created_at) " &
                                        "VALUES (@productId, @productName, @currentStock, @threshold, @alertType, 'PENDING', NOW())"
                        Using alertCmd = New MySqlCommand(alertQuery, conn, transaction)
                            alertCmd.Parameters.AddWithValue("@productId", productId)
                            alertCmd.Parameters.AddWithValue("@productName", productName)
                            alertCmd.Parameters.AddWithValue("@currentStock", currentStock)
                            alertCmd.Parameters.AddWithValue("@threshold", If(alertType = "CRITICAL", criticalThreshold, lowThreshold))
                            alertCmd.Parameters.AddWithValue("@alertType", alertType)
                            alertCmd.ExecuteNonQuery()
                        End Using
                    End If
                End Using
            End If
        Catch ex As Exception
            Debug.WriteLine("Error creating low stock alert: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Get stock setting value
    ''' </summary>
    Public Function GetStockSetting(settingName As String, defaultValue As Integer) As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query = "SELECT setting_value FROM stock_settings WHERE setting_name = @name LIMIT 1"
                Using cmd = New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", settingName)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        If Integer.TryParse(result.ToString(), 0) Then
                            Return CInt(result)
                        End If
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error getting stock setting: " & ex.Message)
        End Try
        Return defaultValue
    End Function

    ''' <summary>
    ''' Get stock update logs for a product
    ''' </summary>
    Public Function GetStockUpdateLogs(productId As Integer, Optional limit As Integer = 100) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query = "SELECT id, product_id, product_name, admin_name, previous_stock, new_stock, quantity_change, change_type, reason, timestamp " &
                           "FROM stock_update_logs WHERE product_id = @productId ORDER BY timestamp DESC LIMIT @limit"
                Using cmd = New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    cmd.Parameters.AddWithValue("@limit", limit)
                    Dim adapter = New MySqlDataAdapter(cmd)
                    Dim dt = New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error getting stock update logs: " & ex.Message)
            Return New DataTable()
        End Try
    End Function

    ''' <summary>
    ''' Get all low stock alerts
    ''' </summary>
    Public Function GetLowStockAlerts(Optional status As String = "PENDING") As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query = "SELECT id, product_id, product_name, current_stock, alert_threshold, alert_type, status, created_at " &
                           "FROM low_stock_alerts WHERE status = @status ORDER BY alert_type DESC, created_at DESC"
                Using cmd = New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@status", status)
                    Dim adapter = New MySqlDataAdapter(cmd)
                    Dim dt = New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error getting low stock alerts: " & ex.Message)
            Return New DataTable()
        End Try
    End Function

    ''' <summary>
    ''' Acknowledge low stock alert
    ''' </summary>
    Public Function AcknowledgeLowStockAlert(alertId As Integer, adminId As Integer) As Boolean
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query = "UPDATE low_stock_alerts SET status = 'ACKNOWLEDGED', acknowledged_by = @adminId, acknowledged_at = NOW() WHERE id = @alertId"
                Using cmd = New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@alertId", alertId)
                    cmd.Parameters.AddWithValue("@adminId", adminId)
                    cmd.ExecuteNonQuery()
                    Return True
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error acknowledging alert: " & ex.Message)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Get low stock products view
    ''' </summary>
    Public Function GetLowStockProducts() As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query = "SELECT * FROM v_low_stock_products"
                Using cmd = New MySqlCommand(query, conn)
                    Dim adapter = New MySqlDataAdapter(cmd)
                    Dim dt = New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error getting low stock products: " & ex.Message)
            Return New DataTable()
        End Try
    End Function

End Module
