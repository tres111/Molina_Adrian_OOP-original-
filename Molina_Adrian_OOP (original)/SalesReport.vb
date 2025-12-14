''' <summary>
''' Class for sales reporting and analytics
''' </summary>
Public Class SalesReport
    ''' <summary>
    ''' Report period enumeration
    ''' </summary>
    Public Enum ReportPeriod
        Daily = 0
        Weekly = 1
        Monthly = 2
        Quarterly = 3
        Yearly = 4
        Custom = 5
    End Enum

    ''' <summary>
    ''' Report period
    ''' </summary>
    Public Property Period As ReportPeriod = ReportPeriod.Monthly

    ''' <summary>
    ''' Start date for custom reports
    ''' </summary>
    Public Property StartDate As DateTime

    ''' <summary>
    ''' End date for custom reports
    ''' </summary>
    Public Property EndDate As DateTime

    ''' <summary>
    ''' Total orders in period
    ''' </summary>
    Public Property TotalOrders As Integer = 0

    ''' <summary>
    ''' Total revenue
    ''' </summary>
    Public Property TotalRevenue As Decimal = 0

    ''' <summary>
    ''' Total discount given
    ''' </summary>
    Public Property TotalDiscount As Decimal = 0

    ''' <summary>
    ''' Total tax collected
    ''' </summary>
    Public Property TotalTax As Decimal = 0

    ''' <summary>
    ''' Total shipping cost
    ''' </summary>
    Public Property TotalShipping As Decimal = 0

    ''' <summary>
    ''' Average order value
    ''' </summary>
    Public Property AverageOrderValue As Decimal = 0

    ''' <summary>
    ''' Total items sold
    ''' </summary>
    Public Property TotalItemsSold As Integer = 0

    ''' <summary>
    ''' Best selling products
    ''' </summary>
    Public Property BestSellingProducts As List(Of ProductSales) = New List(Of ProductSales)

    ''' <summary>
    ''' Low stock items
    ''' </summary>
    Public Property LowStockItems As List(Of String) = New List(Of String)

    ''' <summary>
    ''' Daily sales data
    ''' </summary>
    Public Property DailySalesData As Dictionary(Of String, SalesDataPoint) = New Dictionary(Of String, SalesDataPoint)

    ''' <summary>
    ''' Product sales information
    ''' </summary>
    Public Class ProductSales
        Public Property ProductName As String = ""
        Public Property QuantitySold As Integer = 0
        Public Property TotalSales As Decimal = 0
        Public Property AveragePrice As Decimal = 0
        Public Property Rank As Integer = 0

        Public Overrides Function ToString() As String
            Return String.Format("#{0} {1}: {2} units, ?{3:F2}", Rank, ProductName, QuantitySold, TotalSales)
        End Function
    End Class

    ''' <summary>
    ''' Daily sales data point
    ''' </summary>
    Public Class SalesDataPoint
        Public Property Date As String = ""
        Public Property OrderCount As Integer = 0
        Public Property Revenue As Decimal = 0
        Public Property ItemsSold As Integer = 0
    End Class

    ''' <summary>
    ''' Calculate average order value
    ''' </summary>
    Public Sub CalculateAverageOrderValue()
        If TotalOrders > 0 Then
            AverageOrderValue = TotalRevenue / TotalOrders
        Else
            AverageOrderValue = 0
        End If
    End Sub

    ''' <summary>
    ''' Get period display name
    ''' </summary>
    Public Function GetPeriodName() As String
        Select Case Period
            Case ReportPeriod.Daily
                Return "Daily"
            Case ReportPeriod.Weekly
                Return "Weekly"
            Case ReportPeriod.Monthly
                Return "Monthly"
            Case ReportPeriod.Quarterly
                Return "Quarterly"
            Case ReportPeriod.Yearly
                Return "Yearly"
            Case ReportPeriod.Custom
                Return String.Format("Custom ({0:yyyy-MM-dd} to {1:yyyy-MM-dd})", StartDate, EndDate)
            Case Else
                Return "Unknown"
        End Select
    End Function

    ''' <summary>
    ''' Generate report summary
    ''' </summary>
    Public Function GenerateReportSummary() As String
        Dim sb As New System.Text.StringBuilder()

        sb.AppendLine("=" & New String("="c, 50))
        sb.AppendLine("SALES REPORT - " & GetPeriodName().ToUpper())
        sb.AppendLine("=" & New String("="c, 50))
        sb.AppendLine()

        sb.AppendLine("KEY METRICS")
        sb.AppendLine("-" & New String("-"c, 49))
        sb.AppendLine(String.Format("Total Orders: {0}", TotalOrders))
        sb.AppendLine(String.Format("Total Revenue: ?{0:F2}", TotalRevenue))
        sb.AppendLine(String.Format("Average Order Value: ?{0:F2}", AverageOrderValue))
        sb.AppendLine(String.Format("Total Items Sold: {0}", TotalItemsSold))
        sb.AppendLine()

        sb.AppendLine("FINANCIAL BREAKDOWN")
        sb.AppendLine("-" & New String("-"c, 49))
        sb.AppendLine(String.Format("Subtotal: ?{0:F2}", TotalRevenue - TotalTax))
        sb.AppendLine(String.Format("Tax (12%): ?{0:F2}", TotalTax))
        sb.AppendLine(String.Format("Discounts: -?{0:F2}", TotalDiscount))
        sb.AppendLine(String.Format("Shipping: ?{0:F2}", TotalShipping))
        sb.AppendLine()

        sb.AppendLine("TOP SELLING PRODUCTS")
        sb.AppendLine("-" & New String("-"c, 49))
        If BestSellingProducts.Count > 0 Then
            For Each product In BestSellingProducts.Take(5)
                sb.AppendLine(String.Format("#{0} {1}: {2} units, ?{3:F2}",
                    product.Rank, product.ProductName, product.QuantitySold, product.TotalSales))
            Next
        Else
            sb.AppendLine("No sales data available")
        End If
        sb.AppendLine()

        sb.AppendLine("LOW STOCK ITEMS")
        sb.AppendLine("-" & New String("-"c, 49))
        If LowStockItems.Count > 0 Then
            For Each item In LowStockItems.Take(5)
                sb.AppendLine("• " & item)
            Next
        Else
            sb.AppendLine("All items have adequate stock")
        End If
        sb.AppendLine()

        sb.AppendLine("=" & New String("="c, 50))
        sb.AppendLine("Report Generated: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        sb.AppendLine("=" & New String("="c, 50))

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Clone report
    ''' </summary>
    Public Function Clone() As SalesReport
        Dim clonedProducts As New List(Of ProductSales)
        For Each p In BestSellingProducts
            clonedProducts.Add(New ProductSales With {
                .ProductName = p.ProductName,
                .QuantitySold = p.QuantitySold,
                .TotalSales = p.TotalSales,
                .AveragePrice = p.AveragePrice,
                .Rank = p.Rank
            })
        Next

        Return New SalesReport With {
            .Period = Me.Period,
            .StartDate = Me.StartDate,
            .EndDate = Me.EndDate,
            .TotalOrders = Me.TotalOrders,
            .TotalRevenue = Me.TotalRevenue,
            .TotalDiscount = Me.TotalDiscount,
            .TotalTax = Me.TotalTax,
            .TotalShipping = Me.TotalShipping,
            .AverageOrderValue = Me.AverageOrderValue,
            .TotalItemsSold = Me.TotalItemsSold,
            .BestSellingProducts = clonedProducts,
            .LowStockItems = New List(Of String)(Me.LowStockItems),
            .DailySalesData = New Dictionary(Of String, SalesDataPoint)(Me.DailySalesData)
        }
    End Function
End Class
