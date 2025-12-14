Imports System.Data

''' <summary>
''' Inventory Analytics - Advanced analytics and forecasting for inventory management
''' </summary>
Public Class InventoryAnalytics
    Public Class StockForecast
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property CurrentStock As Integer
        Public Property DailyVelocity As Decimal
        Public Property EstimatedRunoutDate As DateTime?
        Public Property RecommendedReorderDate As DateTime?
        Public Property ForecastedStockLevel As Integer
    End Class

    Public Class StockOptimization
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property CurrentStock As Integer
        Public Property OptimalStockLevel As Integer
        Public Property ExcessStock As Integer
        Public Property TurnsPerYear As Decimal
        Public Property Recommendation As String
    End Class

    Private trendData As New Dictionary(Of Integer, List(Of Tuple(Of DateTime, Integer)))

    Public Sub RecordTrendData(productId As Integer, stock As Integer, velocityPerDay As Decimal)
        If Not trendData.ContainsKey(productId) Then
            trendData.Add(productId, New List(Of Tuple(Of DateTime, Integer)))
        End If
        trendData(productId).Add(New Tuple(Of DateTime, Integer)(DateTime.Now, stock))
    End Sub

    Public Function ForecastStockLevels(productId As Integer, forecastDays As Integer) As StockForecast
        Try
            Dim dt = DBmySql.GetAllProducts()
            Dim product = dt.AsEnumerable().FirstOrDefault(Function(r) CInt(r("id")) = productId)

            If product Is Nothing Then Return Nothing

            Dim currentStock = CInt(product("stock"))
            Dim productName = product("product_name").ToString()

            Dim dailyVelocity As Decimal = 2 'Assume 2 units per day as default
            Dim projectedStock = currentStock - CInt(dailyVelocity * forecastDays)

            Dim forecast As New StockForecast With {
                .ProductId = productId,
                .ProductName = productName,
                .CurrentStock = currentStock,
                .DailyVelocity = dailyVelocity,
                .ForecastedStockLevel = Math.Max(0, projectedStock),
                .EstimatedRunoutDate = If(projectedStock <= 0, 
                    DateTime.Now.AddDays(currentStock / dailyVelocity), Nothing),
                .RecommendedReorderDate = DateTime.Now.AddDays(10)
            }

            Return forecast

        Catch ex As Exception
            Debug.WriteLine("Error forecasting stock: " & ex.Message)
            Return Nothing
        End Try
    End Function

    Public Function GetOptimizationSuggestions() As List(Of StockOptimization)
        Try
            Dim suggestions As New List(Of StockOptimization)
            Dim dt = DBmySql.GetAllProducts()

            For Each row In dt.Rows
                Dim productId = CInt(row("id"))
                Dim productName = row("product_name").ToString()
                Dim currentStock = CInt(row("stock"))

                Dim optimalStock = 50
                Dim recommendation = ""

                If currentStock > optimalStock * 2 Then
                    recommendation = "High excess stock - consider promotional pricing"
                ElseIf currentStock > optimalStock Then
                    recommendation = "Slightly above optimal - monitor sales velocity"
                ElseIf currentStock < 10 Then
                    recommendation = "URGENT: Critical stock level"
                ElseIf currentStock < optimalStock Then
                    recommendation = "Below optimal - recommend restock"
                Else
                    recommendation = "Optimal stock level"
                End If

                suggestions.Add(New StockOptimization With {
                    .ProductId = productId,
                    .ProductName = productName,
                    .CurrentStock = currentStock,
                    .OptimalStockLevel = optimalStock,
                    .ExcessStock = Math.Max(0, currentStock - optimalStock),
                    .TurnsPerYear = (currentStock * 5) / 365,
                    .Recommendation = recommendation
                })
            Next

            Return suggestions

        Catch ex As Exception
            Debug.WriteLine("Error getting optimization suggestions: " & ex.Message)
            Return New List(Of StockOptimization)
        End Try
    End Function
End Class
