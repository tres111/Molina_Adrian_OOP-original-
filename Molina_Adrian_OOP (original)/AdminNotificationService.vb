''' <summary>
''' Admin Notification Service - Manage admin alerts and notifications
''' </summary>
Public Class AdminNotificationService
    Public Enum NotificationSeverity
        Info = 0
        Warning = 1
        Critical = 2
    End Enum

    <Flags()>
    Public Enum NotificationChannel
        InApp = 1
        Email = 2
        SMS = 4
        Webhook = 8
    End Enum

    Public Class Notification
        Public Property NotificationId As Integer
        Public Property AdminId As Integer
        Public Property Title As String
        Public Property Message As String
        Public Property Severity As NotificationSeverity
        Public Property NotificationType As String 'StockAlert, SalesMilestone, SystemAlert, OrderUpdate, PaymentFailure, Promotion
        Public Property IsRead As Boolean
        Public Property CreatedDate As DateTime
        Public Property RelatedEntityType As String 'Order, Product, User
        Public Property RelatedEntityId As Integer?
        Public Property Channels As NotificationChannel
        Public Property Priority As Integer
        Public Property ExpiresAt As DateTime?
    End Class

    Public Class AdminPreference
        Public Property AdminId As Integer
        Public Property PreferredChannels As NotificationChannel = NotificationChannel.InApp Or NotificationChannel.Email
        Public Property LowStockThresholdOverride As Integer? = Nothing
    End Class

    Public Interface INotificationRepository
        Sub Save(notification As Notification)
        Function GetAll() As List(Of Notification)
        Sub Update(notification As Notification)
    End Interface

    Private Class InMemoryNotificationRepository
        Implements INotificationRepository

        Private notifications As New List(Of Notification)
        Public Sub Save(notification As Notification) Implements INotificationRepository.Save
            notifications.Add(notification)
        End Sub

        Public Function GetAll() As List(Of Notification) Implements INotificationRepository.GetAll
            Return notifications
        End Function

        Public Sub Update(notification As Notification) Implements INotificationRepository.Update
            Dim idx = notifications.FindIndex(Function(n) n.NotificationId = notification.NotificationId)
            If idx >= 0 Then
                notifications(idx) = notification
            End If
        End Sub
    End Class

    Public Class NotificationEventArgs
        Inherits System.EventArgs
        Public Property Notification As Notification
        Public Sub New(n As Notification)
            Me.Notification = n
        End Sub
    End Class

    Public Event NotificationCreated As EventHandler(Of NotificationEventArgs)

    Private repo As INotificationRepository
    Private notificationCounter As Integer = 0
    Private adminPreferences As New Dictionary(Of Integer, AdminPreference)

    Public Sub New()
        Me.repo = New InMemoryNotificationRepository()
    End Sub

    Public Sub New(repository As INotificationRepository)
        Me.repo = If(repository, New InMemoryNotificationRepository())
    End Sub

    Public Sub UpsertAdminPreference(pref As AdminPreference)
        adminPreferences(pref.AdminId) = pref
    End Sub

    Public Function GetAdminPreference(adminId As Integer) As AdminPreference
        Dim p As AdminPreference = Nothing
        If adminPreferences.TryGetValue(adminId, p) Then
            Return p
        End If
        Return New AdminPreference With {.AdminId = adminId}
    End Function

    Private Sub CreateAndDispatchNotification(n As Notification)
        notificationCounter += 1
        n.NotificationId = notificationCounter
        If n.CreatedDate = Nothing Then n.CreatedDate = DateTime.Now

        repo.Save(n)
        RaiseEvent NotificationCreated(Me, New NotificationEventArgs(n))
    End Sub

    Public Sub SendStockAlertNotification(adminId As Integer, productId As Integer, productName As String,
                                         sku As String, currentStock As Integer, minimumStock As Integer)
        Try
            Dim severity = If(currentStock = 0, NotificationSeverity.Critical, NotificationSeverity.Warning)
            Dim pref = GetAdminPreference(adminId)

            Dim n As New Notification With {
                .AdminId = adminId,
                .Title = "Stock Alert: " & productName & " (" & sku & ")",
                .Message = String.Format("{0} (SKU:{1}) stock is at {2} (minimum: {3})", productName, sku, currentStock, minimumStock),
                .Severity = severity,
                .NotificationType = "StockAlert",
                .IsRead = False,
                .CreatedDate = DateTime.Now,
                .RelatedEntityType = "Product",
                .RelatedEntityId = productId,
                .Channels = pref.PreferredChannels,
                .Priority = If(currentStock = 0, 10, 5)
            }

            CreateAndDispatchNotification(n)
        Catch ex As Exception
            Debug.WriteLine("Error sending stock alert: " & ex.Message)
        End Try
    End Sub

    Public Sub SendOrderStatusNotification(adminId As Integer, orderId As Integer, orderStatus As String,
                                           customerName As String, totalAmount As Decimal)
        Try
            Dim pref = GetAdminPreference(adminId)
            Dim severity = NotificationSeverity.Info
            If orderStatus = "PaymentFailed" OrElse orderStatus = "FraudSuspected" Then severity = NotificationSeverity.Critical

            Dim n As New Notification With {
                .AdminId = adminId,
                .Title = String.Format("Order #{0} status: {1}", orderId, orderStatus),
                .Message = String.Format("Order #{0} for {1} ({2:C2}) is now {3}.", orderId, customerName, totalAmount, orderStatus),
                .Severity = severity,
                .NotificationType = "OrderUpdate",
                .IsRead = False,
                .CreatedDate = DateTime.Now,
                .RelatedEntityType = "Order",
                .RelatedEntityId = orderId,
                .Channels = pref.PreferredChannels,
                .Priority = If(severity = NotificationSeverity.Critical, 20, 5)
            }

            CreateAndDispatchNotification(n)
        Catch ex As Exception
            Debug.WriteLine("Error sending order status notification: " & ex.Message)
        End Try
    End Sub

    Public Sub SendPaymentFailureNotification(adminId As Integer, orderId As Integer, amount As Decimal, failureReason As String)
        Try
            SendOrderStatusNotification(adminId, orderId, "PaymentFailed", "Customer", amount)
            Dim pref = GetAdminPreference(adminId)
            Dim n As New Notification With {
                .AdminId = adminId,
                .Title = String.Format("Payment Failure for Order #{0}", orderId),
                .Message = String.Format("A payment of {0:C2} for order {1} failed: {2}", amount, orderId, failureReason),
                .Severity = NotificationSeverity.Critical,
                .NotificationType = "PaymentFailure",
                .IsRead = False,
                .CreatedDate = DateTime.Now,
                .RelatedEntityType = "Order",
                .RelatedEntityId = orderId,
                .Channels = pref.PreferredChannels,
                .Priority = 25
            }

            CreateAndDispatchNotification(n)
        Catch ex As Exception
            Debug.WriteLine("Error sending payment failure notification: " & ex.Message)
        End Try
    End Sub

    Public Sub SendAbandonedCartNotification(adminId As Integer, userId As Integer, cartValue As Decimal, cartItemCount As Integer)
        Try
            Dim pref = GetAdminPreference(adminId)
            Dim n As New Notification With {
                .AdminId = adminId,
                .Title = String.Format("Abandoned Cart (User #{0})", userId),
                .Message = String.Format("User #{0} abandoned a cart with {1} items worth {2:C2}. Consider recovery actions.", userId, cartItemCount, cartValue),
                .Severity = NotificationSeverity.Info,
                .NotificationType = "AbandonedCart",
                .IsRead = False,
                .CreatedDate = DateTime.Now,
                .RelatedEntityType = "User",
                .RelatedEntityId = userId,
                .Channels = pref.PreferredChannels,
                .Priority = 3
            }

            CreateAndDispatchNotification(n)
        Catch ex As Exception
            Debug.WriteLine("Error sending abandoned cart notification: " & ex.Message)
        End Try
    End Sub

    Public Sub SendPromotionNotification(adminId As Integer, title As String, message As String, expiresAt As DateTime?)
        Try
            Dim pref = GetAdminPreference(adminId)
            Dim n As New Notification With {
                .AdminId = adminId,
                .Title = title,
                .Message = message,
                .Severity = NotificationSeverity.Info,
                .NotificationType = "Promotion",
                .IsRead = False,
                .CreatedDate = DateTime.Now,
                .RelatedEntityType = Nothing,
                .RelatedEntityId = Nothing,
                .Channels = pref.PreferredChannels,
                .Priority = 1,
                .ExpiresAt = expiresAt
            }

            CreateAndDispatchNotification(n)
        Catch ex As Exception
            Debug.WriteLine("Error sending promotion notification: " & ex.Message)
        End Try
    End Sub

    Public Function GetPendingNotifications(adminId As Integer, Optional notificationType As String = Nothing) As List(Of Notification)
        Dim all = repo.GetAll().Where(Function(n) n.AdminId = adminId AndAlso Not n.IsRead)
        If Not String.IsNullOrEmpty(notificationType) Then
            all = all.Where(Function(n) n.NotificationType = notificationType)
        End If
        Return all.OrderByDescending(Function(n) n.CreatedDate).ToList()
    End Function

    Public Function GetAllNotifications(adminId As Integer) As List(Of Notification)
        Return repo.GetAll().Where(Function(n) n.AdminId = adminId).OrderByDescending(Function(n) n.CreatedDate).ToList()
    End Function

    Public Function GetNotificationsByRelatedEntity(entityType As String, entityId As Integer) As List(Of Notification)
        Return repo.GetAll().Where(Function(n) n.RelatedEntityType = entityType AndAlso n.RelatedEntityId = entityId).OrderByDescending(Function(n) n.CreatedDate).ToList()
    End Function

    Public Sub MarkAsRead(notificationId As Integer)
        Dim notification = repo.GetAll().FirstOrDefault(Function(n) n.NotificationId = notificationId)
        If notification IsNot Nothing Then
            notification.IsRead = True
            repo.Update(notification)
        End If
    End Sub

    Public Sub BulkMarkAsRead(adminId As Integer, Optional notificationType As String = Nothing)
        Dim toMark = repo.GetAll().Where(Function(n) n.AdminId = adminId AndAlso Not n.IsRead)
        If Not String.IsNullOrEmpty(notificationType) Then
            toMark = toMark.Where(Function(n) n.NotificationType = notificationType)
        End If
        For Each n In toMark
            n.IsRead = True
            repo.Update(n)
        Next
    End Sub

    Public Function GetNotificationCount(adminId As Integer) As Integer
        Return repo.GetAll().Where(Function(n) n.AdminId = adminId AndAlso Not n.IsRead).Count()
    End Function
End Class
