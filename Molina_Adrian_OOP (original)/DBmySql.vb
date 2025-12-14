Imports MySql.Data.MySqlClient

Module DBmySql
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Public Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' Create order and return order id
    Public Function CreateOrder(userId As Integer, fullName As String, mobile As String, email As String, address As String,
                                deliveryMethod As String, paymentMethod As String, notes As String,
                                subtotal As Decimal, deliveryFee As Decimal, total As Decimal) As Integer
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "INSERT INTO orders (user_id, full_name, mobile, email, address, delivery_method, payment_method, notes, subtotal, delivery_fee, total, status, created_at) VALUES (@user_id,@full_name,@mobile,@email,@address,@delivery_method,@payment_method,@notes,@subtotal,@delivery_fee,@total,'Placed',NOW())"
            Using cmd As New MySqlCommand(query, conn)
                ' If userId is not provided (0 or less) insert NULL so DB can accept guest orders
                If userId > 0 Then
                    cmd.Parameters.AddWithValue("@user_id", userId)
                Else
                    cmd.Parameters.AddWithValue("@user_id", DBNull.Value)
                End If
                cmd.Parameters.AddWithValue("@full_name", fullName)
                cmd.Parameters.AddWithValue("@mobile", mobile)
                cmd.Parameters.AddWithValue("@email", email)
                cmd.Parameters.AddWithValue("@address", address)
                cmd.Parameters.AddWithValue("@delivery_method", deliveryMethod)
                cmd.Parameters.AddWithValue("@payment_method", paymentMethod)
                cmd.Parameters.AddWithValue("@notes", notes)
                cmd.Parameters.AddWithValue("@subtotal", subtotal)
                cmd.Parameters.AddWithValue("@delivery_fee", deliveryFee)
                cmd.Parameters.AddWithValue("@total", total)
                cmd.ExecuteNonQuery()
                Return CType(cmd.LastInsertedId, Integer)
            End Using
        End Using
    End Function

    ' Create order item record
    Public Sub CreateOrderItem(orderId As Integer, productId As Integer, productName As String, unitPrice As Decimal, quantity As Integer)
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "INSERT INTO order_items (order_id, product_id, product_name, unit_price, quantity, total_price) VALUES (@order_id,@product_id,@product_name,@unit_price,@quantity,@total_price)"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@order_id", orderId)
                cmd.Parameters.AddWithValue("@product_id", If(productId > 0, productId, DBNull.Value))
                cmd.Parameters.AddWithValue("@product_name", productName)
                cmd.Parameters.AddWithValue("@unit_price", unitPrice)
                cmd.Parameters.AddWithValue("@quantity", quantity)
                cmd.Parameters.AddWithValue("@total_price", unitPrice * quantity)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub


    Public Function GetProductsByCategory(category As String) As DataTable
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id, product_name, category, price, stock FROM products WHERE category = @category ORDER BY id"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@category", category)
                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ' Get all products for inventory
    Public Function GetAllProducts() As DataTable
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id, product_name, category, price, stock FROM products ORDER BY category, id"
            Using cmd As New MySqlCommand(query, conn)
                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ' Get stock by product name
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

    ' Get stock by product id
    Public Function GetStockById(productId As Integer) As Integer
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT stock FROM products WHERE id = @id LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", productId)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return Convert.ToInt32(result)
                Else
                    Return 0
                End If
            End Using
        End Using
    End Function

    ' Get price by product name
    Public Function GetPrice(itemName As String) As Decimal
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT price FROM products WHERE product_name = @name LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@name", itemName)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return Convert.ToDecimal(result)
                Else
                    Return 0D
                End If
            End Using
        End Using
    End Function

    ' Get price by product id
    Public Function GetPriceById(productId As Integer) As Decimal
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT price FROM products WHERE id = @id LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", productId)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return Convert.ToDecimal(result)
                Else
                    Return 0D
                End If
            End Using
        End Using
    End Function

    ' Reduce stock
    Public Sub ReduceStock(itemName As String, qty As Integer)
        Using conn = GetConnection()
            conn.Open()
            ' Ensure stock never goes negative and clamp to zero
            Dim query As String = "UPDATE products SET stock = CASE WHEN stock - @qty < 0 THEN 0 ELSE stock - @qty END WHERE product_name = @name"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@qty", qty)
                cmd.Parameters.AddWithValue("@name", itemName)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' Reduce stock by product id (now uses StockManagementService for audit logging)
    Public Sub ReduceStockById(productId As Integer, qty As Integer)
        Try
            ' Use StockManagementService for proper logging
            Dim result = StockManagementService.ReduceStock(productId, qty, "Order checkout")
            If Not result.Item1 Then
                Debug.WriteLine("Stock reduction warning: " & result.Item2)
            End If
        Catch ex As Exception
            Debug.WriteLine("Error reducing stock: " & ex.Message)
            ' Fallback to direct update if service fails
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE products SET stock = CASE WHEN stock - @qty < 0 THEN 0 ELSE stock - @qty END WHERE id = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@qty", qty)
                    cmd.Parameters.AddWithValue("@id", productId)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End Try
    End Sub

    ' Check if product is out of stock
    Public Function IsOutOfStock(itemName As String) As Boolean
        Dim s = GetStock(itemName)
        Return s <= 0
    End Function

    ' Check if product is out of stock by id
    Public Function IsOutOfStockById(productId As Integer) As Boolean
        Dim s = GetStockById(productId)
        Return s <= 0
    End Function

    ' Update stock (for inventory management)
    Public Sub UpdateStock(productId As Integer, newStock As Integer)
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "UPDATE products SET stock = @stock WHERE id = @id"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@stock", newStock)
                cmd.Parameters.AddWithValue("@id", productId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    ' Save checkout record
    Public Sub SaveCheckoutRecord(productName As String, price As Decimal, quantity As Integer)
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "INSERT INTO checkout_records (product_name, price, quantity, total_price) VALUES (@productName, @price, @quantity, @totalPrice)"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@productName", productName)
                cmd.Parameters.AddWithValue("@price", price)
                cmd.Parameters.AddWithValue("@quantity", quantity)
                cmd.Parameters.AddWithValue("@totalPrice", price * quantity)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    ' Get product name by index (example logic)
    Public Function GetProductNameByIndex(index As Integer) As String
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT product_name FROM products WHERE id = @id LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", index + 1) ' Assuming index corresponds to product ID
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return result.ToString()
                Else
                    Return String.Empty
                End If
            End Using
        End Using
    End Function

    ' Get product id by name
    Public Function GetProductIdByName(productName As String) As Integer
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id FROM products WHERE product_name = @name LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@name", productName)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing Then
                    Return Convert.ToInt32(result)
                Else
                    Return 0
                End If
            End Using
        End Using
    End Function

    ' Get product details by ID
    Public Function GetProductById(productId As Integer) As DataRow
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id, product_name, category, price, stock, description FROM products WHERE id = @id LIMIT 1"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@id", productId)
                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                If dt.Rows.Count > 0 Then
                    Return dt.Rows(0)
                End If
            End Using
        End Using
        Return Nothing
    End Function

    ' Search products by name
    Public Function SearchProducts(searchTerm As String) As DataTable
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id, product_name, category, price, stock FROM products WHERE product_name LIKE @searchTerm OR category LIKE @searchTerm ORDER BY product_name"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@searchTerm", "%" & searchTerm & "%")
                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ' Get low stock products
    Public Function GetLowStockProducts(threshold As Integer) As DataTable
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id, product_name, category, price, stock FROM products WHERE stock <= @threshold ORDER BY stock ASC"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@threshold", threshold)
                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ' Get featured products
    Public Function GetFeaturedProducts() As DataTable
        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "SELECT id, product_name, category, price, stock FROM products WHERE is_featured = 1 ORDER BY id LIMIT 12"
            Using cmd As New MySqlCommand(query, conn)
                Dim adapter As New MySqlDataAdapter(cmd)
                Dim dt As New DataTable()
                adapter.Fill(dt)
                Return dt
            End Using
        End Using
    End Function

    ' Add new product
    Public Function AddProduct(productName As String, category As String, price As Decimal, stock As Integer, description As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "INSERT INTO products (product_name, category, price, stock, description, created_at) VALUES (@name, @category, @price, @stock, @description, NOW())"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", productName)
                    cmd.Parameters.AddWithValue("@category", category)
                    cmd.Parameters.AddWithValue("@price", price)
                    cmd.Parameters.AddWithValue("@stock", stock)
                    cmd.Parameters.AddWithValue("@description", If(String.IsNullOrEmpty(description), "", description))
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Product added successfully!")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adding product: " & ex.Message)
        End Try
    End Function

    ' Update product
    Public Function UpdateProduct(productId As Integer, productName As String, category As String, price As Decimal, stock As Integer, description As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE products SET product_name = @name, category = @category, price = @price, stock = @stock, description = @description WHERE id = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", productName)
                    cmd.Parameters.AddWithValue("@category", category)
                    cmd.Parameters.AddWithValue("@price", price)
                    cmd.Parameters.AddWithValue("@stock", stock)
                    cmd.Parameters.AddWithValue("@description", If(String.IsNullOrEmpty(description), "", description))
                    cmd.Parameters.AddWithValue("@id", productId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Product updated successfully!")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error updating product: " & ex.Message)
        End Try
    End Function

    ' Delete product
    Public Function DeleteProduct(productId As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "DELETE FROM products WHERE id = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", productId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Product deleted successfully!")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error deleting product: " & ex.Message)
        End Try
    End Function
    Public Function TestConnection() As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Return New Tuple(Of Boolean, String)(True, "OK")
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, ex.Message & " | " & ex.GetType().ToString())
        End Try
    End Function
End Module