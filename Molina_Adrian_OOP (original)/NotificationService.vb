Imports MySql.Data.MySqlClient

''' <summary>
''' Modern notification service for real-time customer engagement
''' Supports email, SMS, push notifications, and in-app notifications
''' </summary>
Module NotificationService
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' ===== NOTIFICATION TYPES =====
    Public Enum NotificationType
        OrderConfirmation = 1
        ShippingUpdate = 2
        DeliveryNotification = 3
        ReviewReminder = 4
        FlashSaleAlert = 5
        RestockNotification = 6
        OrderCancelled = 7
        PaymentFailed = 8
        WishlistPriceDropped = 9
        NewProductInCategory = 10
    End Enum

    ' ===== IN-APP NOTIFICATIONS =====
    ''' <summary>
    ''' Create in-app notification
    ''' </summary>
    Public Function CreateNotification(userId As Integer, title As String, message As String, 
                                      notificationType As NotificationType) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "INSERT INTO notifications (user_id, title, message, type, created_at, is_read) " &
                    "VALUES (@userId, @title, @message, @type, NOW(), 0)"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@title", title)
                    cmd.Parameters.AddWithValue("@message", message)
                    cmd.Parameters.AddWithValue("@type", CInt(notificationType))
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Notification created.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error creating notification: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Get unread notifications for user
    ''' </summary>
    Public Function GetUnreadNotifications(userId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, title, message, type, created_at FROM notifications " &
                    "WHERE user_id = @userId AND is_read = 0 ORDER BY created_at DESC"
                
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

    ''' <summary>
    ''' Mark notification as read
    ''' </summary>
    Public Function MarkNotificationAsRead(notificationId As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE notifications SET is_read = 1, read_at = NOW() WHERE id = @notificationId"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@notificationId", notificationId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Notification marked as read.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error marking notification: " & ex.Message)
        End Try
    End Function

    ' ===== ORDER NOTIFICATIONS =====
    ''' <summary>
    ''' Send order confirmation notification
    ''' </summary>
    Public Function SendOrderConfirmation(userId As Integer, orderId As Long) As Tuple(Of Boolean, String)
        Try
            Dim title As String = "Order Confirmed"
            Dim message As String = String.Format("Your order #{0} has been confirmed and is being prepared.", orderId)
            Return CreateNotification(userId, title, message, NotificationType.OrderConfirmation)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Send shipping update notification
    ''' </summary>
    Public Function SendShippingUpdate(userId As Integer, orderId As Long, trackingNumber As String) As Tuple(Of Boolean, String)
        Try
            Dim title As String = "Order Shipped"
            Dim message As String = String.Format("Your order #{0} has been shipped. Tracking: {1}", orderId, trackingNumber)
            Return CreateNotification(userId, title, message, NotificationType.ShippingUpdate)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Send delivery notification
    ''' </summary>
    Public Function SendDeliveryNotification(userId As Integer, orderId As Long) As Tuple(Of Boolean, String)
        Try
            Dim title As String = "Delivery Complete"
            Dim message As String = String.Format("Your order #{0} has been delivered. Thank you for shopping with us!", orderId)
            Return CreateNotification(userId, title, message, NotificationType.DeliveryNotification)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ' ===== PRODUCT NOTIFICATIONS =====
    ''' <summary>
    ''' Send restock notification
    ''' </summary>
    Public Function SendRestockNotification(userId As Integer, productName As String) As Tuple(Of Boolean, String)
        Try
            Dim title As String = "Product Back in Stock"
            Dim message As String = String.Format("{0} is now available. Add it to your cart before it sells out!", productName)
            Return CreateNotification(userId, title, message, NotificationType.RestockNotification)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Send flash sale notification
    ''' </summary>
    Public Function SendFlashSaleNotification(userId As Integer, productName As String, discount As Decimal) As Tuple(Of Boolean, String)
        Try
            Dim title As String = "Flash Sale Alert!"
            Dim message As String = String.Format("{0} is now {1}% off! Limited time offer.", productName, discount)
            Return CreateNotification(userId, title, message, NotificationType.FlashSaleAlert)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Send price drop notification for wishlist items
    ''' </summary>
    Public Function SendWishlistPriceDropNotification(userId As Integer, productName As String, 
                                                      oldPrice As Decimal, newPrice As Decimal) As Tuple(Of Boolean, String)
        Try
            Dim savingAmount As Decimal = oldPrice - newPrice
            Dim title As String = "Price Drop on Wishlist Item"
            Dim message As String = String.Format("{0} price dropped! Save ?{1}", productName, savingAmount.ToString("N2"))
            Return CreateNotification(userId, title, message, NotificationType.WishlistPriceDropped)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ' ===== REVIEW NOTIFICATIONS =====
    ''' <summary>
    ''' Send review reminder notification
    ''' </summary>
    Public Function SendReviewReminder(userId As Integer, productName As String, orderId As Long) As Tuple(Of Boolean, String)
        Try
            Dim title As String = "Share Your Feedback"
            Dim message As String = String.Format("How did you like {0}? Share your review and earn points!", productName)
            Return CreateNotification(userId, title, message, NotificationType.ReviewReminder)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ' ===== EMAIL NOTIFICATIONS (Template) =====
    ''' <summary>
    ''' Send email notification (Integration required)
    ''' </summary>
    Public Function SendEmailNotification(email As String, subject As String, body As String) As Tuple(Of Boolean, String)
        Try
            ' TODO: Implement email service integration
            ' Options: SendGrid, Mailgun, SMTP
            
            ' Placeholder for now
            Return New Tuple(Of Boolean, String)(True, "Email queued for sending.")
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ' ===== SMS NOTIFICATIONS (Template) =====
    ''' <summary>
    ''' Send SMS notification (Integration required)
    ''' </summary>
    Public Function SendSMSNotification(phoneNumber As String, message As String) As Tuple(Of Boolean, String)
        Try
            ' TODO: Implement SMS service integration
            ' Options: Twilio, Nexmo, Smart Communications (for PH)
            
            ' Placeholder for now
            Return New Tuple(Of Boolean, String)(True, "SMS queued for sending.")
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ' ===== PUSH NOTIFICATIONS (Template) =====
    ''' <summary>
    ''' Send push notification (Integration required)
    ''' </summary>
    Public Function SendPushNotification(deviceToken As String, title As String, message As String) As Tuple(Of Boolean, String)
        Try
            ' TODO: Implement push notification service
            ' Options: Firebase Cloud Messaging, OneSignal, Azure Notification Hub
            
            ' Placeholder for now
            Return New Tuple(Of Boolean, String)(True, "Push notification queued.")
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

    ' ===== NOTIFICATION PREFERENCES =====
    ''' <summary>
    ''' Get user notification preferences
    ''' </summary>
    Public Function GetNotificationPreferences(userId As Integer) As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT notification_type, email_enabled, sms_enabled, push_enabled " &
                    "FROM notification_preferences WHERE user_id = @userId"
                
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

    ''' <summary>
    ''' Update notification preferences
    ''' </summary>
    Public Function UpdateNotificationPreferences(userId As Integer, notificationType As NotificationType,
                                                  emailEnabled As Boolean, smsEnabled As Boolean, 
                                                  pushEnabled As Boolean) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE notification_preferences SET email_enabled = @email, " &
                    "sms_enabled = @sms, push_enabled = @push WHERE user_id = @userId AND notification_type = @type"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@type", CInt(notificationType))
                    cmd.Parameters.AddWithValue("@email", If(emailEnabled, 1, 0))
                    cmd.Parameters.AddWithValue("@sms", If(smsEnabled, 1, 0))
                    cmd.Parameters.AddWithValue("@push", If(pushEnabled, 1, 0))
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Preferences updated.")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error: " & ex.Message)
        End Try
    End Function

End Module
