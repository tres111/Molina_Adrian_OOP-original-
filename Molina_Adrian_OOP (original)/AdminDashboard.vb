Imports System.Data

''' <summary>
''' Admin Dashboard - Real-time KPI dashboard with inventory and sales metrics
''' </summary>
Public Class AdminDashboard
    Private lastUpdated As DateTime

    Public Property InventoryValue As Decimal
    Public Property TotalProducts As Integer
    Public Property LowStockProducts As Integer
    Public Property OutOfStockProducts As Integer
    Public Property TodayRevenue As Decimal
    Public Property ThisWeekRevenue As Decimal
    Public Property ThisMonthRevenue As Decimal
    Public Property TodayOrders As Integer
    Public Property ThisWeekOrders As Integer
    Public Property AverageOrderValue As Decimal

    Public Sub New()
        InventoryValue = 0
        TotalProducts = 0
        LowStockProducts = 0
        OutOfStockProducts = 0
        TodayRevenue = 0
        ThisWeekRevenue = 0
        ThisMonthRevenue = 0
        TodayOrders = 0
        ThisWeekOrders = 0
        AverageOrderValue = 0
        lastUpdated = DateTime.Now
    End Sub

    Public Sub UpdateMetrics()
        Try
            Dim dt = DBmySql.GetAllProducts()

            TotalProducts = dt.Rows.Count
            InventoryValue = 0
            LowStockProducts = 0
            OutOfStockProducts = 0

            For Each row In dt.Rows
                Dim stock = CInt(row("stock"))
                Dim price = CDec(row("price"))

                InventoryValue += stock * price

                If stock = 0 Then
                    OutOfStockProducts += 1
                ElseIf stock < 10 Then
                    LowStockProducts += 1
                End If
            Next

            lastUpdated = DateTime.Now

        Catch ex As Exception
            Debug.WriteLine("Error updating dashboard metrics: " & ex.Message)
        End Try
    End Sub

    Public Function GetInventoryHealthScore() As Integer
        Dim healthScore = 100

        If OutOfStockProducts > 0 Then
            healthScore -= OutOfStockProducts * 5
        End If

        If LowStockProducts > 0 Then
            healthScore -= LowStockProducts * 2
        End If

        If healthScore < 0 Then healthScore = 0
        If healthScore > 100 Then healthScore = 100

        Return healthScore
    End Function

    Public Function GetSalesPerformance() As Integer
        If ThisMonthRevenue = 0 Then Return 50

        Dim performanceScore = CInt((ThisMonthRevenue / 100000) * 100)
        If performanceScore > 100 Then performanceScore = 100

        Return performanceScore
    End Function

    Public Function GetStatusSummary() As String
        Dim summary As New System.Text.StringBuilder()

        summary.AppendLine("=== INVENTORY STATUS ===")
        summary.AppendLine("Total Products: " & TotalProducts)
        summary.AppendLine("Low Stock: " & LowStockProducts)
        summary.AppendLine("Out of Stock: " & OutOfStockProducts)
        summary.AppendLine("Inventory Value: ?" & InventoryValue.ToString("N2"))
        summary.AppendLine()
        summary.AppendLine("=== SALES STATUS ===")
        summary.AppendLine("Today's Orders: " & TodayOrders)
        summary.AppendLine("This Week's Orders: " & ThisWeekOrders)
        summary.AppendLine("Today's Revenue: ?" & TodayRevenue.ToString("N2"))
        summary.AppendLine("This Month's Revenue: ?" & ThisMonthRevenue.ToString("N2"))
        summary.AppendLine("Average Order Value: ?" & AverageOrderValue.ToString("N2"))
        summary.AppendLine()
        summary.AppendLine("Last Updated: " & lastUpdated.ToString("yyyy-MM-dd HH:mm:ss"))

        Return summary.ToString()
    End Function
End Class
