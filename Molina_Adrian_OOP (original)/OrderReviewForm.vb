Imports System.Linq

Public Class OrderReviewForm
    Private orderItems As List(Of OrderItem)
    Private parentForm As Form1

    Public Class OrderItem
        Public Property ProductName As String
        Public Property UnitPrice As Decimal
        Public Property Quantity As Integer
        Public ReadOnly Property TotalPrice As Decimal
            Get
                Return UnitPrice * Quantity
            End Get
        End Property
    End Class

    Public Sub New(parent As Form1, items As List(Of OrderItem))
        InitializeComponent()
        Me.parentForm = parent
        Me.orderItems = items
    End Sub

    Private Sub OrderReviewForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lvOrder.Items.Clear()
        For Each it In orderItems
            Dim lvi As New ListViewItem(it.ProductName)
            lvi.SubItems.Add(it.UnitPrice.ToString("C2"))
            lvi.SubItems.Add(it.Quantity.ToString())
            lvi.SubItems.Add(it.TotalPrice.ToString("C2"))
            lvOrder.Items.Add(lvi)
        Next

        Dim grand As Decimal = orderItems.Sum(Function(i) i.TotalPrice)
        lblGrandTotal.Text = grand.ToString("C2")

        ' Validate stock availability and disable Confirm if any item is out of stock
        Dim anyOutOfStock As Boolean = False
        For Each it In orderItems
            If DBmySql.IsOutOfStock(it.ProductName) Then
                anyOutOfStock = True
                ' add visual indicator
                For Each lvi As ListViewItem In lvOrder.Items
                    If lvi.Text = it.ProductName Then
                        lvi.BackColor = Drawing.Color.LightCoral
                        lvi.ToolTipText = "OUT OF STOCK"
                    End If
                Next
            End If
        Next

        btnConfirm.Enabled = Not anyOutOfStock
        If anyOutOfStock Then
            MessageBox.Show("One or more items are out of stock and cannot be purchased. Please remove them from your cart or contact an admin.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs) Handles btnConfirm.Click
        ' Proceed to finalize order: show delivery details popup and then create order once
        Dim dlg As New DeliveryDetailsForm(orderItems, orderItems.Sum(Function(i) i.TotalPrice))
        dlg.StartPosition = FormStartPosition.CenterParent
        dlg.FormBorderStyle = FormBorderStyle.FixedDialog
        dlg.MinimizeBox = False
        dlg.MaximizeBox = False
        dlg.ShowInTaskbar = False
        dlg.TopMost = True
        If dlg.ShowDialog(Me) <> DialogResult.OK Then
            Return
        End If

        Try
            ' Create order here (single place)
            Dim userId = SessionManager.CurrentUserId
            Dim orderId = DBmySql.CreateOrder(userId, dlg.FullName, dlg.Mobile, dlg.Email, dlg.Address, dlg.DeliveryMethod, dlg.PaymentMethod, dlg.Notes, orderItems.Sum(Function(i) i.TotalPrice), dlg.DeliveryFee, dlg.Total)

            ' Save order items and reduce stock
            For Each it In orderItems
                Dim pid As Integer = DBmySql.GetProductIdByName(it.ProductName)
                DBmySql.CreateOrderItem(orderId, pid, it.ProductName, it.UnitPrice, it.Quantity)
                If pid > 0 Then
                    DBmySql.ReduceStockById(pid, it.Quantity)
                Else
                    DBmySql.ReduceStock(it.ProductName, it.Quantity)
                End If
            Next

            ' Clear local cart and DB cart
            Try
                Cart.ResetCart()
            Catch
            End Try
            Try
                DBEcommerce.ClearCart(userId)
            Catch
            End Try

            ' Notify inventory and UI
            Try
                InventorySync.RaiseInventoryChanged(0, InventorySync.InventoryChangeType.OrderCompleted)
            Catch
            End Try

            ' Notify admin
            Try
                If parentForm IsNot Nothing Then
                    parentForm.Notifications.SendOrderStatusNotification(1, orderId, "Placed", parentForm.Username, orderItems.Sum(Function(i) i.TotalPrice))
                End If
            Catch ex As Exception
                Debug.WriteLine("Failed to send order notification: " & ex.Message)
            End Try

            MessageBox.Show("Order placed successfully! Order #" & orderId.ToString(), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            parentForm.UpdateTotal()
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Failed to place order: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
End Class
