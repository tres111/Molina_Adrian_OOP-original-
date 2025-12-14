Imports System.Data

''' <summary>
''' Inventory Audit Manager - Comprehensive audit trail logging and tracking
''' </summary>
Public Class InventoryAuditManager
    Public Class AuditLog
        Public Property LogId As Integer
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Action As String
        Public Property PreviousValue As String
        Public Property NewValue As String
        Public Property AdminId As Integer
        Public Property AdminUsername As String
        Public Property Timestamp As DateTime
        Public Property Notes As String
    End Class

    Private auditLogs As New List(Of AuditLog)
    Private logCounter As Integer = 0

    Public Sub LogAction(productId As Integer, productName As String, action As String, 
                        previousValue As String, newValue As String, 
                        adminId As Integer, adminUsername As String, notes As String)
        Try
            logCounter += 1

            auditLogs.Add(New AuditLog With {
                .LogId = logCounter,
                .ProductId = productId,
                .ProductName = productName,
                .Action = action,
                .PreviousValue = previousValue,
                .NewValue = newValue,
                .AdminId = adminId,
                .AdminUsername = adminUsername,
                .Timestamp = DateTime.Now,
                .Notes = notes
            })

            Debug.WriteLine(String.Format("Audit: {0} - {1} changed from {2} to {3}", 
                productName, action, previousValue, newValue))

        Catch ex As Exception
            Debug.WriteLine("Error logging action: " & ex.Message)
        End Try
    End Sub

    Public Function GetTodayLogs() As List(Of AuditLog)
        Dim result As New List(Of AuditLog)
        For Each log In auditLogs
            If log.Timestamp.Date = DateTime.Now.Date Then
                result.Add(log)
            End If
        Next
        Return result
    End Function

    Public Function SearchLogs(keyword As String) As List(Of AuditLog)
        Dim result As New List(Of AuditLog)
        For Each log In auditLogs
            If log.ProductName.Contains(keyword) OrElse _
               log.Action.Contains(keyword) OrElse _
               log.Notes.Contains(keyword) Then
                result.Add(log)
            End If
        Next
        Return result
    End Function

    Public Function GetAuditSummary(Optional days As Integer = 7) As String
        Dim summary As New System.Text.StringBuilder()
        Dim startDate = DateTime.Now.AddDays(-days)
        Dim relevantLogs As New List(Of AuditLog)
        
        For Each log In auditLogs
            If log.Timestamp >= startDate Then
                relevantLogs.Add(log)
            End If
        Next

        summary.AppendLine("=== AUDIT LOG SUMMARY (Last " & days & " days) ===")
        summary.AppendLine("Total Actions: " & relevantLogs.Count)
        summary.AppendLine()

        Dim actionGroups As New Dictionary(Of String, Integer)
        For Each log In relevantLogs
            If Not actionGroups.ContainsKey(log.Action) Then
                actionGroups.Add(log.Action, 0)
            End If
            actionGroups(log.Action) += 1
        Next

        For Each kvp In actionGroups
            summary.AppendLine(kvp.Key & ": " & kvp.Value)
        Next

        summary.AppendLine()
        summary.AppendLine("Recent Actions:")
        
        Dim sortedLogs As New List(Of AuditLog)(relevantLogs)
        Dim comparer = New DateTimeComparer()
        sortedLogs.Sort(comparer)
        
        Dim count = 0
        For i = sortedLogs.Count - 1 To 0 Step -1
            If count >= 10 Then Exit For
            Dim log = sortedLogs(i)
            summary.AppendLine(String.Format("[{0}] {1} - {2}: {3} ? {4} (by {5})", 
                log.Timestamp.ToString("yyyy-MM-dd HH:mm"), 
                log.ProductName, log.Action, 
                log.PreviousValue, log.NewValue, log.AdminUsername))
            count += 1
        Next

        Return summary.ToString()
    End Function

    Public Function GetLogsByDateRange(startDate As DateTime, endDate As DateTime) As List(Of AuditLog)
        Dim result As New List(Of AuditLog)
        For Each log In auditLogs
            If log.Timestamp >= startDate AndAlso log.Timestamp <= endDate Then
                result.Add(log)
            End If
        Next
        Return result
    End Function

    Private Class DateTimeComparer
        Implements IComparer(Of AuditLog)
        Public Function Compare(x As AuditLog, y As AuditLog) As Integer Implements IComparer(Of AuditLog).Compare
            Return x.Timestamp.CompareTo(y.Timestamp)
        End Function
    End Class
End Class
