Imports MySql.Data.MySqlClient

''' <summary>
''' Modern analytics engine for real-time business intelligence
''' Tracks KPIs, provides insights, and generates reports
''' </summary>
Module AnalyticsEngine
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' ===== SALES METRICS =====
    ''' <summary>
    ''' Get total sales for date range
    ''' </summary>
    Public Function GetTotalSales(startDate As DateTime, endDate As DateTime) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COALESCE(SUM(total_amount), 0) FROM orders " &
                    "WHERE order_date BETWEEN @startDate AND @endDate AND status != 'Cancelled'"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate)
                    cmd.Parameters.AddWithValue("@endDate", endDate)
                    Dim result = cmd.ExecuteScalar()
                    Return CDec(result)
                End Using
            End Using
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    ''' <summary>
    ''' Get average order value
    ''' </summary>
    Public Function GetAverageOrderValue(startDate As DateTime, endDate As DateTime) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COALESCE(AVG(total_amount), 0) FROM orders " &
                    "WHERE order_date BETWEEN @startDate AND @endDate AND status != 'Cancelled'"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate)
                    cmd.Parameters.AddWithValue("@endDate", endDate)
                    Dim result = cmd.ExecuteScalar()
                    Return CDec(result)
                End Using
            End Using
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    ''' <summary>
    ''' Get daily sales breakdown
    ''' </summary>
    Public Function GetDailySalesBreakdown(startDate As DateTime, endDate As DateTime) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT DATE(order_date) as date, COUNT(*) as order_count, " &
                    "SUM(total_amount) as daily_revenue FROM orders " &
                    "WHERE order_date BETWEEN @startDate AND @endDate AND status != 'Cancelled' " &
                    "GROUP BY DATE(order_date) ORDER BY date DESC"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate)
                    cmd.Parameters.AddWithValue("@endDate", endDate)
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

    ' ===== CUSTOMER METRICS =====
    ''' <summary>
    ''' Get total customers (active users)
    ''' </summary>
    Public Function GetTotalCustomers() As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COUNT(DISTINCT user_id) FROM orders"
                
                Using cmd As New MySqlCommand(query, conn)
                    Dim result = cmd.ExecuteScalar()
                    Return CInt(result)
                End Using
            End Using
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Get new customers in date range
    ''' </summary>
    Public Function GetNewCustomers(startDate As DateTime, endDate As DateTime) As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COUNT(DISTINCT user_id) FROM orders " &
                    "WHERE order_date BETWEEN @startDate AND @endDate"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate)
                    cmd.Parameters.AddWithValue("@endDate", endDate)
                    Dim result = cmd.ExecuteScalar()
                    Return CInt(result)
                End Using
            End Using
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Get customer lifetime value
    ''' </summary>
    Public Function GetCustomerLifetimeValue(userId As Integer) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COALESCE(SUM(total_amount), 0) FROM orders WHERE user_id = @userId"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()
                    Return CDec(result)
                End Using
            End Using
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    ''' <summary>
    ''' Get customer retention rate
    ''' </summary>
    Public Function GetRetentionRate(monthsBack As Integer) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim previousMonth As DateTime = DateTime.Now.AddMonths(-monthsBack)
                Dim currentMonth As DateTime = DateTime.Now

                ' Count customers from previous month who made purchases in current month
                Dim query As String = "SELECT COUNT(DISTINCT u.user_id) FROM " &
                    "(SELECT DISTINCT user_id FROM orders WHERE MONTH(order_date) = @prevMonth) u " &
                    "JOIN orders o ON u.user_id = o.user_id WHERE MONTH(o.order_date) = @currMonth"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@prevMonth", previousMonth.Month)
                    cmd.Parameters.AddWithValue("@currMonth", currentMonth.Month)
                    Dim retained = CInt(cmd.ExecuteScalar())

                    ' Count total customers from previous month
                    Dim totalQuery As String = "SELECT COUNT(DISTINCT user_id) FROM orders WHERE MONTH(order_date) = @prevMonth"
                    Using cmd2 As New MySqlCommand(totalQuery, conn)
                        cmd2.Parameters.AddWithValue("@prevMonth", previousMonth.Month)
                        Dim total = CInt(cmd2.ExecuteScalar())
                        
                        If total > 0 Then
                            Return CDec(retained) / CDec(total) * 100
                        Else
                            Return 0D
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    ' ===== PRODUCT METRICS =====
    ''' <summary>
    ''' Get top selling products
    ''' </summary>
    Public Function GetTopSellingProducts(limit As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT p.id, p.product_name, p.category, " &
                    "COUNT(oi.id) as quantity_sold, SUM(oi.price * oi.quantity) as revenue " &
                    "FROM products p JOIN order_items oi ON p.id = oi.product_id " &
                    "GROUP BY p.id ORDER BY quantity_sold DESC LIMIT @limit"
                
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
    ''' Get category performance
    ''' </summary>
    Public Function GetCategoryPerformance() As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT p.category, COUNT(oi.id) as sales, " &
                    "SUM(oi.price * oi.quantity) as revenue, AVG(r.rating) as avg_rating " &
                    "FROM products p JOIN order_items oi ON p.id = oi.product_id " &
                    "LEFT JOIN reviews r ON p.id = r.product_id " &
                    "GROUP BY p.category ORDER BY revenue DESC"
                
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

    ' ===== CONVERSION METRICS =====
    ''' <summary>
    ''' Get conversion rate
    ''' </summary>
    Public Function GetConversionRate(startDate As DateTime, endDate As DateTime) As Decimal
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Get total visits (from product views)
                Dim visitsQuery As String = "SELECT COUNT(*) FROM product_views WHERE view_date BETWEEN @startDate AND @endDate"
                Dim visits As Integer
                Using cmd As New MySqlCommand(visitsQuery, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate)
                    cmd.Parameters.AddWithValue("@endDate", endDate)
                    visits = CInt(cmd.ExecuteScalar())
                End Using

                ' Get total orders
                Dim ordersQuery As String = "SELECT COUNT(*) FROM orders WHERE order_date BETWEEN @startDate AND @endDate"
                Dim orders As Integer
                Using cmd As New MySqlCommand(ordersQuery, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate)
                    cmd.Parameters.AddWithValue("@endDate", endDate)
                    orders = CInt(cmd.ExecuteScalar())
                End Using

                If visits > 0 Then
                    Return CDec(orders) / CDec(visits) * 100
                Else
                    Return 0D
                End If
            End Using
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    ' ===== INVENTORY METRICS =====
    ''' <summary>
    ''' Get inventory health
    ''' </summary>
    Public Function GetInventoryHealth() As Dictionary(Of String, Object)
        Dim health As New Dictionary(Of String, Object)
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Low stock items
                Dim lowStockQuery As String = "SELECT COUNT(*) FROM products WHERE stock < 10"
                Using cmd As New MySqlCommand(lowStockQuery, conn)
                    health("low_stock_items") = CInt(cmd.ExecuteScalar())
                End Using

                ' Out of stock items
                Dim outOfStockQuery As String = "SELECT COUNT(*) FROM products WHERE stock = 0"
                Using cmd As New MySqlCommand(outOfStockQuery, conn)
                    health("out_of_stock_items") = CInt(cmd.ExecuteScalar())
                End Using

                ' Total inventory value
                Dim inventoryValueQuery As String = "SELECT COALESCE(SUM(price * stock), 0) FROM products"
                Using cmd As New MySqlCommand(inventoryValueQuery, conn)
                    health("total_inventory_value") = CDec(cmd.ExecuteScalar())
                End Using

                ' Average stock level
                Dim avgStockQuery As String = "SELECT COALESCE(AVG(stock), 0) FROM products"
                Using cmd As New MySqlCommand(avgStockQuery, conn)
                    health("avg_stock_level") = CInt(cmd.ExecuteScalar())
                End Using

            End Using
        Catch ex As Exception
        End Try
        Return health
    End Function

    ' ===== PERFORMANCE METRICS =====
    ''' <summary>
    ''' Get overall KPIs dashboard
    ''' </summary>
    Public Function GetKPIDashboard(startDate As DateTime, endDate As DateTime) As Dictionary(Of String, Object)
        Dim kpis As New Dictionary(Of String, Object)
        
        kpis("total_sales") = GetTotalSales(startDate, endDate)
        kpis("average_order_value") = GetAverageOrderValue(startDate, endDate)
        kpis("total_customers") = GetTotalCustomers()
        kpis("new_customers") = GetNewCustomers(startDate, endDate)
        kpis("conversion_rate") = GetConversionRate(startDate, endDate)
        kpis("inventory_health") = GetInventoryHealth()
        
        Return kpis
    End Function

End Module
