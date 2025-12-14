Imports System.Data

''' <summary>
''' Stock Alert Manager - Automated alert generation for inventory management
''' </summary>
Public Class StockAlertManager
    Public Class Alert
        Public Property AlertId As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property CurrentStock As Integer
        Public Property MinimumThreshold As Integer
        Public Property AlertLevel As String 'Critical, Warning, Info
        Public Property RecommendedReorder As Integer
        Public Property CreatedDate As DateTime
        Public Property IsResolved As Boolean
    End Class

    Private minimumThreshold As Integer = 10
    Private criticalThreshold As Integer = 5
    Private reorderQuantity As Integer = 50
    Private alerts As New List(Of Alert)

    Public Sub GenerateAlerts()
        Try
            alerts.Clear()
            Dim dt = DBmySql.GetAllProducts()

            For Each row In dt.Rows
                Dim productId = CInt(row("id"))
                Dim productName = row("product_name").ToString()
                Dim stock = CInt(row("stock"))

                Dim alertLevel = ""
                If stock = 0 Then
                    alertLevel = "Critical"
                ElseIf stock <= criticalThreshold Then
                    alertLevel = "Critical"
                ElseIf stock <= minimumThreshold Then
                    alertLevel = "Warning"
                End If

                If alertLevel <> "" Then
                    alerts.Add(New Alert With {
                        .AlertId = alerts.Count + 1,
                        .ProductId = productId,
                        .ProductName = productName,
                        .CurrentStock = stock,
                        .MinimumThreshold = minimumThreshold,
                        .AlertLevel = alertLevel,
                        .RecommendedReorder = reorderQuantity,
                        .CreatedDate = DateTime.Now,
                        .IsResolved = False
                    })
                End If
            Next

        Catch ex As Exception
            Debug.WriteLine("Error generating alerts: " & ex.Message)
        End Try
    End Sub

    Public Function GetCriticalAlerts() As List(Of Alert)
        Return alerts.Where(Function(a) a.AlertLevel = "Critical" AndAlso Not a.IsResolved).ToList()
    End Function

    Public Function GetAllAlerts() As List(Of Alert)
        Return alerts
    End Function

    Public Function GetAlertStatistics() As String
        Dim critical = alerts.Where(Function(a) a.AlertLevel = "Critical" AndAlso Not a.IsResolved).Count()
        Dim warnings = alerts.Where(Function(a) a.AlertLevel = "Warning" AndAlso Not a.IsResolved).Count()

        Return String.Format("Critical: {0} | Warnings: {1} | Total: {2}", critical, warnings, alerts.Count)
    End Function

    Public Sub UpdateThresholds(minimumThreshold As Integer, criticalThreshold As Integer, reorderQuantity As Integer)
        Me.minimumThreshold = minimumThreshold
        Me.criticalThreshold = criticalThreshold
        Me.reorderQuantity = reorderQuantity
    End Sub

    Public Sub ResolveAlert(alertId As Integer)
        Dim alert = alerts.FirstOrDefault(Function(a) a.AlertId = alertId)
        If alert IsNot Nothing Then
            alert.IsResolved = True
        End If
    End Sub
End Class
