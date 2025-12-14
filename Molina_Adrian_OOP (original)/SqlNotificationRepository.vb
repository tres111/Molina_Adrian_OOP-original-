Imports MySql.Data.MySqlClient

Public Class SqlNotificationRepository
    Implements AdminNotificationService.INotificationRepository

    Public Sub New()
    End Sub

    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    Public Sub Save(notification As AdminNotificationService.Notification) Implements AdminNotificationService.INotificationRepository.Save
        Using conn = GetConnection()
            conn.Open()
            Dim query = "INSERT INTO notifications (admin_id, title, message, severity, notification_type, is_read, created_at, related_entity_type, related_entity_id, channels, priority, expires_at) VALUES (@admin_id,@title,@message,@severity,@type,@is_read,NOW(),@related_type,@related_id,@channels,@priority,@expires)"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@admin_id", notification.AdminId)
                cmd.Parameters.AddWithValue("@title", notification.Title)
                cmd.Parameters.AddWithValue("@message", notification.Message)
                cmd.Parameters.AddWithValue("@severity", notification.Severity.ToString())
                cmd.Parameters.AddWithValue("@type", notification.NotificationType)
                cmd.Parameters.AddWithValue("@is_read", If(notification.IsRead, 1, 0))
                cmd.Parameters.AddWithValue("@related_type", If(notification.RelatedEntityType, DBNull.Value))
                cmd.Parameters.AddWithValue("@related_id", If(notification.RelatedEntityId, DBNull.Value))
                cmd.Parameters.AddWithValue("@channels", Convert.ToInt32(notification.Channels))
                cmd.Parameters.AddWithValue("@priority", notification.Priority)
                If notification.ExpiresAt.HasValue Then
                    cmd.Parameters.AddWithValue("@expires", notification.ExpiresAt.Value)
                Else
                    cmd.Parameters.AddWithValue("@expires", DBNull.Value)
                End If
                cmd.ExecuteNonQuery()
                notification.NotificationId = CType(cmd.LastInsertedId, Integer)
            End Using
        End Using
    End Sub

    Public Function GetAll() As List(Of AdminNotificationService.Notification) Implements AdminNotificationService.INotificationRepository.GetAll
        Dim list As New List(Of AdminNotificationService.Notification)()
        Using conn = GetConnection()
            conn.Open()
            Dim query = "SELECT id, admin_id, title, message, severity, notification_type, is_read, created_at, related_entity_type, related_entity_id, channels, priority, expires_at FROM notifications ORDER BY created_at DESC"
            Using cmd As New MySqlCommand(query, conn)
                Using rdr = cmd.ExecuteReader()
                    While rdr.Read()
                        Dim n As New AdminNotificationService.Notification With {
                            .NotificationId = Convert.ToInt32(rdr("id")),
                            .AdminId = Convert.ToInt32(rdr("admin_id")),
                            .Title = rdr("title").ToString(),
                            .Message = rdr("message").ToString(),
                            .Severity = [Enum].Parse(GetType(AdminNotificationService.NotificationSeverity), rdr("severity").ToString()),
                            .NotificationType = rdr("notification_type").ToString(),
                            .IsRead = Convert.ToInt32(rdr("is_read")) = 1,
                            .CreatedDate = Convert.ToDateTime(rdr("created_at")),
                            .RelatedEntityType = If(IsDBNull(rdr("related_entity_type")), Nothing, rdr("related_entity_type").ToString()),
                            .RelatedEntityId = If(IsDBNull(rdr("related_entity_id")), Nothing, CType(rdr("related_entity_id"), Integer)),
                            .Channels = CType(Convert.ToInt32(rdr("channels")), AdminNotificationService.NotificationChannel),
                            .Priority = Convert.ToInt32(rdr("priority"))
                        }
                        If Not IsDBNull(rdr("expires_at")) Then
                            n.ExpiresAt = Convert.ToDateTime(rdr("expires_at"))
                        End If
                        list.Add(n)
                    End While
                End Using
            End Using
        End Using
        Return list
    End Function

    Public Sub Update(notification As AdminNotificationService.Notification) Implements AdminNotificationService.INotificationRepository.Update
        Using conn = GetConnection()
            conn.Open()
            Dim query = "UPDATE notifications SET is_read=@is_read WHERE id=@id"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@is_read", If(notification.IsRead, 1, 0))
                cmd.Parameters.AddWithValue("@id", notification.NotificationId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
