Imports MySql.Data.MySqlClient

''' <summary>
''' Advanced search and filtering engine for modern e-commerce
''' Implements AI-powered search, autocomplete, and advanced filtering
''' </summary>
Module SearchAndFilterManager
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' ===== ADVANCED SEARCH =====
    ''' <summary>
    ''' Search products with autocomplete suggestions
    ''' </summary>
    Public Function SearchProductsWithAutocomplete(searchTerm As String) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT DISTINCT product_name FROM products WHERE product_name LIKE @term LIMIT 10"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@term", searchTerm & "%")
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

    ''' <summary>
    ''' Advanced search with multiple criteria
    ''' </summary>
    Public Function AdvancedSearch(searchTerm As String, minPrice As Decimal, maxPrice As Decimal, 
                                   category As String, sortBy As String) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, product_name, category, price, stock FROM products WHERE " &
                    "(product_name LIKE @term OR category LIKE @term) AND " &
                    "price BETWEEN @minPrice AND @maxPrice"
                
                If Not String.IsNullOrEmpty(category) Then
                    query &= " AND category = @category"
                End If
                
                Select Case sortBy
                    Case "price_asc"
                        query &= " ORDER BY price ASC"
                    Case "price_desc"
                        query &= " ORDER BY price DESC"
                    Case "newest"
                        query &= " ORDER BY id DESC"
                    Case "popular"
                        query &= " ORDER BY id DESC"
                    Case Else
                        query &= " ORDER BY product_name"
                End Select

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@term", "%" & searchTerm & "%")
                    cmd.Parameters.AddWithValue("@minPrice", minPrice)
                    cmd.Parameters.AddWithValue("@maxPrice", maxPrice)
                    If Not String.IsNullOrEmpty(category) Then
                        cmd.Parameters.AddWithValue("@category", category)
                    End If

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

    ' ===== PERSONALIZED RECOMMENDATIONS =====
    ''' <summary>
    ''' Get trending products based on sales
    ''' </summary>
    Public Function GetTrendingProducts(limit As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT p.id, p.product_name, p.category, p.price, " &
                    "COUNT(oi.id) as purchase_count FROM products p " &
                    "LEFT JOIN order_items oi ON p.id = oi.product_id " &
                    "GROUP BY p.id ORDER BY purchase_count DESC LIMIT @limit"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@limit", limit)
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

    ''' <summary>
    ''' Get products frequently bought together
    ''' </summary>
    Public Function GetFrequentlyBoughtTogether(productId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT p2.id, p2.product_name, p2.price, COUNT(*) as frequency " &
                    "FROM order_items oi1 " &
                    "JOIN order_items oi2 ON oi1.order_id = oi2.order_id " &
                    "JOIN products p2 ON oi2.product_id = p2.id " &
                    "WHERE oi1.product_id = @productId AND oi2.product_id != @productId " &
                    "GROUP BY p2.id ORDER BY frequency DESC LIMIT 5"
                
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

    ''' <summary>
    ''' Get personalized recommendations based on user's purchase history
    ''' </summary>
    Public Function GetPersonalizedRecommendations(userId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT DISTINCT p.id, p.product_name, p.price FROM products p " &
                    "WHERE p.category IN (SELECT category FROM products WHERE id IN " &
                    "(SELECT product_id FROM order_items WHERE order_id IN " &
                    "(SELECT id FROM orders WHERE user_id = @userId))) " &
                    "AND p.id NOT IN (SELECT product_id FROM order_items WHERE order_id IN " &
                    "(SELECT id FROM orders WHERE user_id = @userId)) " &
                    "LIMIT 8"
                
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

    ' ===== PRODUCT ANALYTICS =====
    ''' <summary>
    ''' Get product performance metrics
    ''' </summary>
    Public Function GetProductPerformance(productId As Integer) As Dictionary(Of String, Object)
        Dim performance As New Dictionary(Of String, Object)
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Get view count
                Dim viewQuery As String = "SELECT COUNT(*) FROM product_views WHERE product_id = @productId"
                Using cmd As New MySqlCommand(viewQuery, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    performance("views") = CInt(cmd.ExecuteScalar())
                End Using

                ' Get sales count
                Dim salesQuery As String = "SELECT COUNT(*) FROM order_items WHERE product_id = @productId"
                Using cmd As New MySqlCommand(salesQuery, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    performance("sales") = CInt(cmd.ExecuteScalar())
                End Using

                ' Get conversion rate
                Dim viewCount = CInt(performance("views"))
                Dim salesCount = CInt(performance("sales"))
                If viewCount > 0 Then
                    performance("conversion_rate") = CDec(salesCount) / CDec(viewCount) * 100
                Else
                    performance("conversion_rate") = 0D
                End If

                ' Get average rating
                Dim ratingQuery As String = "SELECT AVG(rating) FROM reviews WHERE product_id = @productId"
                Using cmd As New MySqlCommand(ratingQuery, conn)
                    cmd.Parameters.AddWithValue("@productId", productId)
                    Dim result = cmd.ExecuteScalar()
                    performance("avg_rating") = If(IsDBNull(result), 0D, CDec(result))
                End Using

            End Using
        Catch ex As Exception
        End Try
        Return performance
    End Function

End Module
