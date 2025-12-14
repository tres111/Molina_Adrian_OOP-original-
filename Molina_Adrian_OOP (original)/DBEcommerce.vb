Imports MySql.Data.MySqlClient

Module DBEcommerce
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' ===== CART MANAGEMENT =====
    Public Function AddToCart(userId As Integer, productId As Integer, quantity As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim checkQuery As String = "SELECT id, quantity FROM cart WHERE user_id = @userId AND product_id = @productId"

                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.AddWithValue("@userId", userId)
                    checkCmd.Parameters.AddWithValue("@productId", productId)
                    Using reader = checkCmd.ExecuteReader()
                        If reader.Read() Then
                            Dim cartId = CInt(reader("id"))
                            reader.Close()

                            Dim updateQuery As String = "UPDATE cart SET quantity = quantity + @quantity WHERE id = @cartId"
                            Using updateCmd As New MySqlCommand(updateQuery, conn)
                                updateCmd.Parameters.AddWithValue("@quantity", quantity)
                                updateCmd.Parameters.AddWithValue("@cartId", cartId)
                                updateCmd.ExecuteNonQuery()
                            End Using
                            Return New Tuple(Of Boolean, String)(True, "Product quantity updated in cart.")
                        Else
                            reader.Close()
                            Dim insertQuery As String = "INSERT INTO cart (user_id, product_id, quantity, added_at) VALUES (@userId, @productId, @quantity, NOW())"
                            Using insertCmd As New MySqlCommand(insertQuery, conn)
                                insertCmd.Parameters.AddWithValue("@userId", userId)
                                insertCmd.Parameters.AddWithValue("@productId", productId)
                                insertCmd.Parameters.AddWithValue("@quantity", quantity)
                                insertCmd.ExecuteNonQuery()
                            End Using
                            Return New Tuple(Of Boolean, String)(True, "Product added to cart.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adding to cart: " & ex.Message)
        End Try
    End Function

    Public Function GetCartItems(userId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT c.id, p.id as product_id, p.product_name, p.category, p.price, c.quantity, (p.price * c.quantity) as subtotal FROM cart c JOIN products p ON c.product_id = p.id WHERE c.user_id = @userId ORDER BY c.added_at DESC"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    Public Function UpdateCartQuantity(cartId As Integer, quantity As Integer) As Tuple(Of Boolean, String)
        Try
            If quantity <= 0 Then
                Return RemoveFromCart(cartId)
            End If

            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE cart SET quantity = @quantity WHERE id = @cartId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@quantity", quantity)
                    cmd.Parameters.AddWithValue("@cartId", cartId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Cart updated.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error updating cart: " & ex.Message)
        End Try
    End Function

    Public Function RemoveFromCart(cartId As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "DELETE FROM cart WHERE id = @cartId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@cartId", cartId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Item removed from cart.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error removing item: " & ex.Message)
        End Try
    End Function

    Public Function ClearCart(userId As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "DELETE FROM cart WHERE user_id = @userId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Cart cleared.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error clearing cart: " & ex.Message)
        End Try
    End Function

    Public Function GetCartTotal(userId As Integer) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COALESCE(SUM(p.price * c.quantity), 0) FROM cart c JOIN products p ON c.product_id = p.id WHERE c.user_id = @userId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return CDec(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return 0D
    End Function

    Public Function GetCartItemCount(userId As Integer) As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COUNT(*) FROM cart WHERE user_id = @userId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        Return CInt(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return 0
    End Function

    ' ===== ORDERS =====
    Public Function CreateOrder(userId As Integer, totalAmount As Decimal, shippingAddress As String, paymentMethod As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim orderId As Long = 0

                ' Insert order
                Dim orderQuery As String = "INSERT INTO orders (user_id, order_date, total_amount, status, shipping_address, payment_method) VALUES (@userId, NOW(), @totalAmount, 'Pending', @shippingAddress, @paymentMethod)"
                Using cmd As New MySqlCommand(orderQuery, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@totalAmount", totalAmount)
                    cmd.Parameters.AddWithValue("@shippingAddress", shippingAddress)
                    cmd.Parameters.AddWithValue("@paymentMethod", paymentMethod)
                    cmd.ExecuteNonQuery()
                    orderId = cmd.LastInsertedId
                End Using

                ' Get cart items
                Dim cartItems = GetCartItems(userId)

                ' Insert order items
                For Each row As DataRow In cartItems.Rows
                    Dim itemQuery As String = "INSERT INTO order_items (order_id, product_id, quantity, price) VALUES (@orderId, @productId, @quantity, @price)"
                    Using cmd As New MySqlCommand(itemQuery, conn)
                        cmd.Parameters.AddWithValue("@orderId", orderId)
                        cmd.Parameters.AddWithValue("@productId", row("product_id"))
                        cmd.Parameters.AddWithValue("@quantity", row("quantity"))
                        cmd.Parameters.AddWithValue("@price", row("price"))
                        cmd.ExecuteNonQuery()
                    End Using

                    ' Reduce stock
                    DBmySql.ReduceStock(row("product_name").ToString(), CInt(row("quantity")))
                Next

                ' Clear cart
                ClearCart(userId)

                Return New Tuple(Of Boolean, String)(True, "Order created successfully. Order ID: " & orderId)
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error creating order: " & ex.Message)
        End Try
    End Function

    Public Function GetOrderHistory(userId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, order_date, total_amount, status, shipping_address, payment_method FROM orders WHERE user_id = @userId ORDER BY order_date DESC"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    Public Function GetOrderDetails(orderId As Long) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT oi.id, p.product_name, p.category, oi.quantity, oi.price, (oi.quantity * oi.price) as subtotal FROM order_items oi JOIN products p ON oi.product_id = p.id WHERE oi.order_id = @orderId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@orderId", orderId)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    Public Function UpdateOrderStatus(orderId As Long, status As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE orders SET status = @status WHERE id = @orderId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@status", status)
                    cmd.Parameters.AddWithValue("@orderId", orderId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Order status updated.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error updating order: " & ex.Message)
        End Try
    End Function

    ' ===== WISHLIST =====
    Public Function AddToWishlist(userId As Integer, productId As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim checkQuery As String = "SELECT COUNT(*) FROM wishlist WHERE user_id = @userId AND product_id = @productId"

                Using checkCmd As New MySqlCommand(checkQuery, conn)
                    checkCmd.Parameters.AddWithValue("@userId", userId)
                    checkCmd.Parameters.AddWithValue("@productId", productId)
                    Dim exists = CInt(checkCmd.ExecuteScalar())

                    If exists > 0 Then
                        Return New Tuple(Of Boolean, String)(False, "Product already in wishlist.")
                    End If
                End Using

                Dim insertQuery As String = "INSERT INTO wishlist (user_id, product_id, added_at) VALUES (@userId, @productId, NOW())"
                Using cmd As New MySqlCommand(insertQuery, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Added to wishlist.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adding to wishlist: " & ex.Message)
        End Try
    End Function

    Public Function GetWishlist(userId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT w.id, p.id as product_id, p.product_name, p.category, p.price, p.stock, w.added_at FROM wishlist w JOIN products p ON w.product_id = p.id WHERE w.user_id = @userId ORDER BY w.added_at DESC"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    Public Function RemoveFromWishlist(wishlistId As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "DELETE FROM wishlist WHERE id = @wishlistId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@wishlistId", wishlistId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Removed from wishlist.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error removing from wishlist: " & ex.Message)
        End Try
    End Function

    Public Function IsProductInWishlist(userId As Integer, productId As Integer) As Boolean
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COUNT(*) FROM wishlist WHERE user_id = @userId AND product_id = @productId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    Dim count = CInt(cmd.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    ' ===== REVIEWS & RATINGS =====
    Public Function AddReview(userId As Integer, productId As Integer, rating As Integer, reviewText As String) As Tuple(Of Boolean, String)
        Try
            If rating < 1 OrElse rating > 5 Then
                Return New Tuple(Of Boolean, String)(False, "Rating must be between 1 and 5.")
            End If

            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "INSERT INTO reviews (user_id, product_id, rating, review_text, created_at) VALUES (@userId, @productId, @rating, @reviewText, NOW())"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    cmd.Parameters.AddWithValue("@rating", rating)
                    cmd.Parameters.AddWithValue("@reviewText", If(String.IsNullOrEmpty(reviewText), "", reviewText))
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Review added successfully!")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adding review: " & ex.Message)
        End Try
    End Function

    Public Function GetProductReviews(productId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT r.id, u.username, r.rating, r.review_text, r.created_at FROM reviews r JOIN users u ON r.user_id = u.id WHERE r.product_id = @productId ORDER BY r.created_at DESC"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    Public Function GetProductAverageRating(productId As Integer) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COALESCE(AVG(rating), 0) FROM reviews WHERE product_id = @productId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                        Return CDec(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return 0D
    End Function

    Public Function GetProductReviewCount(productId As Integer) As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COUNT(*) FROM reviews WHERE product_id = @productId"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing Then
                        Return CInt(result)
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return 0
    End Function

    ' ===== PROMOTIONS & DISCOUNTS =====
    Public Function GetActivePromotions() As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, code, description, discount_percentage, discount_amount, min_purchase, valid_from, valid_until FROM promotions WHERE valid_from <= NOW() AND valid_until >= NOW() AND is_active = 1"
                Using cmd As New MySqlCommand(query, conn)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    Public Function ValidatePromoCode(code As String) As Tuple(Of Boolean, Decimal, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT discount_percentage, discount_amount FROM promotions WHERE code = @code AND valid_from <= NOW() AND valid_until >= NOW() AND is_active = 1"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@code", code)
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim discountPct = CDec(reader("discount_percentage"))
                            Dim discountAmt = CDec(reader("discount_amount"))
                            If discountPct > 0 Then
                                Return New Tuple(Of Boolean, Decimal, String)(True, discountPct, "Percentage discount applied.")
                            Else
                                Return New Tuple(Of Boolean, Decimal, String)(True, discountAmt, "Fixed discount applied.")
                            End If
                        Else
                            Return New Tuple(Of Boolean, Decimal, String)(False, 0, "Invalid or expired promo code.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, Decimal, String)(False, 0, "Error validating promo code.")
        End Try
    End Function
End Module
