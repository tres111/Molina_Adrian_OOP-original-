Public Class Form1
Public Property Username As String
Public Property IsAdmin As Boolean
    Private WithEvents adminNotificationService As AdminNotificationService

    Public ReadOnly Property Notifications As AdminNotificationService
        Get
            Return adminNotificationService
        End Get
    End Property

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        lblTotal.Text = "₱0.00"
        ' initialize product id mappingss
        Cart.InitializeProductIds()

        ' use SQL-backed notification repository so notifications persist
        Dim repo As New SqlNotificationRepository()
        adminNotificationService = New AdminNotificationService(repo)
        AddHandler adminNotificationService.NotificationCreated, AddressOf OnNotificationCreated
        InitializeNotificationPanel()

        ' subscribe to inventory changes so main total updates when cart/order changes
        Try
            AddHandler InventorySync.InventoryChanged, AddressOf OnInventoryChanged
        Catch ex As Exception
            Debug.WriteLine("Failed to subscribe to InventorySync in Form1: " & ex.Message)
        End Try
    End Sub

    Private Sub OnInventoryChanged(sender As Object, e As InventorySync.InventoryChangeEventArgs)
        Try
            If Me.InvokeRequired Then
                Me.BeginInvoke(New Action(Sub() OnInventoryChanged(sender, e)))
                Return
            End If

            ' Update main total label to reflect cleared cart or changes
            UpdateTotal()
        Catch ex As Exception
            Debug.WriteLine("Form1 OnInventoryChanged error: " & ex.Message)
        End Try
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        Try
            RemoveHandler InventorySync.InventoryChanged, AddressOf OnInventoryChanged
        Catch
        End Try
    End Sub

    Private Sub InitializeNotificationPanel()
        Try
            ' create panel if not present
            Dim found = Me.Controls.Find("pNotifications", True)
            If found.Length = 0 Then
                Return
            End If
            Dim p As Panel = DirectCast(found(0), Panel)
            p.BorderStyle = BorderStyle.FixedSingle
            p.Width = 320
            p.Height = 200
            p.Left = Me.Width - p.Width - 40
            p.Top = 80

            Dim lv As New ListView()
            lv.Name = "lvNotifications"
            lv.View = View.Details
            lv.Columns.Add("Title", 180)
            lv.Columns.Add("When", 120)
            lv.FullRowSelect = True
            lv.Dock = DockStyle.Fill
            p.Controls.Add(lv)

            Dim btn As New Button()
            btn.Name = "btnMarkRead"
            btn.Text = "Mark Read"
            btn.Dock = DockStyle.Bottom
            AddHandler btn.Click, AddressOf BtnMarkRead_Click
            p.Controls.Add(btn)

            RefreshNotificationsList()
        Catch ex As Exception
            Debug.WriteLine("Init notifications failed: " & ex.Message)
        End Try
    End Sub

    Private Sub RefreshNotificationsList()
        Try
            Dim found = Me.Controls.Find("lvNotifications", True)
            If found.Length = 0 Then Return
            Dim lv As ListView = DirectCast(found(0), ListView)
            lv.Items.Clear()
            Dim repoNotifs = adminNotificationService.GetAllNotifications(1)
            For Each n In repoNotifs
                Dim lvi As New ListViewItem(n.Title)
                lvi.SubItems.Add(n.CreatedDate.ToString("g"))
                lvi.Tag = n.NotificationId
                If Not n.IsRead Then lvi.Font = New Drawing.Font(lvi.Font, Drawing.FontStyle.Bold)
                lv.Items.Add(lvi)
            Next
        Catch ex As Exception
            Debug.WriteLine("Refresh notifications failed: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnMarkRead_Click(sender As Object, e As EventArgs)
        Try
            Dim found = Me.Controls.Find("lvNotifications", True)
            If found.Length = 0 Then Return
            Dim lv As ListView = DirectCast(found(0), ListView)
            If lv.SelectedItems.Count = 0 Then Return
            Dim id = Convert.ToInt32(lv.SelectedItems(0).Tag)
            adminNotificationService.MarkAsRead(id)
            RefreshNotificationsList()
        Catch ex As Exception
            Debug.WriteLine("Mark read failed: " & ex.Message)
        End Try
    End Sub

    Public Sub UpdateTotal()
        lblTotal.Text = "₱" & Cart.GetTotal().ToString("N2")
    End Sub

    Public Sub LoadFormInPanel(childForm As Form)
        Panel2.Controls.Clear()
        childForm.TopLevel = False
        childForm.FormBorderStyle = FormBorderStyle.None
        childForm.Dock = DockStyle.Fill
        Panel2.Controls.Add(childForm)
        childForm.Show()
    End Sub

    Private Sub btnTop_Click(sender As Object, e As EventArgs) Handles btnTop.Click
        LoadFormInPanel(New Top(Me))
    End Sub

    Private Sub btnBottoms_Click(sender As Object, e As EventArgs) Handles btnBottoms.Click
        LoadFormInPanel(New Bottoms(Me))
    End Sub

    Private Sub btnFootwear_Click(sender As Object, e As EventArgs) Handles btnFootwear.Click
        LoadFormInPanel(New Footwear(Me))
    End Sub

    Private Sub btnAccessories_Click(sender As Object, e As EventArgs) Handles btnAccessories.Click
        LoadFormInPanel(New Accessories(Me))
    End Sub

    Private Sub btnInventory_Click(sender As Object, e As EventArgs) Handles btnInventory.Click
        LoadFormInPanel(New Inventory(Me))
    End Sub
    Private Sub btnCheckout_Click(sender As Object, e As EventArgs) Handles btnCheckout.Click
        Dim total As Decimal = Cart.GetTotal()
        If total > 0 Then
            ' Build order item list for review
            Dim items As New List(Of OrderReviewForm.OrderItem)()

            ' Tops
            For i As Integer = 0 To Cart.qtyTop.Length - 1
                If Cart.qtyTop(i) > 0 Then
                    Dim productName As String = DBmySql.GetProductNameByIndex(i)
                    Dim price As Decimal = DBmySql.GetPrice(productName)
                    items.Add(New OrderReviewForm.OrderItem With {.ProductName = productName, .UnitPrice = price, .Quantity = Cart.qtyTop(i)})
                End If
            Next

            ' Bottoms
            For i As Integer = 0 To Cart.qtyBottoms.Length - 1
                If Cart.qtyBottoms(i) > 0 Then
                    Dim productName As String = DBmySql.GetProductNameByIndex(i + 9) ' offset if necessary
                    Dim price As Decimal = DBmySql.GetPrice(productName)
                    items.Add(New OrderReviewForm.OrderItem With {.ProductName = productName, .UnitPrice = price, .Quantity = Cart.qtyBottoms(i)})
                End If
            Next

            ' Footwear
            For i As Integer = 0 To Cart.qtyFootwear.Length - 1
                If Cart.qtyFootwear(i) > 0 Then
                    Dim productName As String = DBmySql.GetProductNameByIndex(i + 18) ' offset if necessary
                    Dim price As Decimal = DBmySql.GetPrice(productName)
                    items.Add(New OrderReviewForm.OrderItem With {.ProductName = productName, .UnitPrice = price, .Quantity = Cart.qtyFootwear(i)})
                End If
            Next

            ' Accessories
            For i As Integer = 0 To Cart.qtyAccessories.Length - 1
                If Cart.qtyAccessories(i) > 0 Then
                    Dim productName As String = DBmySql.GetProductNameByIndex(i + 24) ' offset if necessary
                    Dim price As Decimal = DBmySql.GetPrice(productName)
                    items.Add(New OrderReviewForm.OrderItem With {.ProductName = productName, .UnitPrice = price, .Quantity = Cart.qtyAccessories(i)})
                End If
            Next

            ' Show order review dialog
            Dim review As New OrderReviewForm(Me, items)
            review.ShowDialog()
            ' After review closes, check if any items were removed or stock changed
            ' If the review placed order, the cart is reset inside the review confirm flow
            UpdateTotal()
        Else
            MessageBox.Show("Your cart is empty!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub OnNotificationCreated(sender As Object, e As AdminNotificationService.NotificationEventArgs)
        ' Simple in-app alert for critical notifications
        If e.Notification.Severity = AdminNotificationService.NotificationSeverity.Critical Then
            MessageBox.Show(e.Notification.Title & vbCrLf & e.Notification.Message, "Admin Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
        ' Optionally update a notification badge on the UI (only if control exists)
        Try
            Dim found = Me.Controls.Find("lblNotificationsCount", True)
            If found.Length > 0 AndAlso TypeOf found(0) Is Label Then
                DirectCast(found(0), Label).Text = adminNotificationService.GetNotificationCount(1).ToString()
            End If
        Catch
            ' ignore if UI element not present
        End Try
    End Sub
End Class
