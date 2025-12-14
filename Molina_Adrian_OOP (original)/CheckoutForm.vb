Public Class CheckoutForm
Inherits Form

Private userId As Integer

Private Sub CheckoutForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Checkout - Coziest Store"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(600, 700)
        Me.BackColor = Color.White

        userId = DBAuthentication.GetCurrentUserId()
        CreateUI()
        LoadUserProfile()
        LoadOrderSummary()
    End Sub

    Private Sub CreateUI()
        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "Checkout"
        lblTitle.Font = New Font("Arial", 18, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        Me.Controls.Add(lblTitle)

        ' Shipping Address Section
        Dim lblShipping As New Label()
        lblShipping.Text = "Shipping Address"
        lblShipping.Font = New Font("Arial", 12, FontStyle.Bold)
        lblShipping.Location = New Point(20, 60)
        lblShipping.AutoSize = True
        Me.Controls.Add(lblShipping)

        Dim lblFullName As New Label()
        lblFullName.Text = "Full Name:"
        lblFullName.Font = New Font("Arial", 10)
        lblFullName.Location = New Point(20, 90)
        lblFullName.AutoSize = True
        Me.Controls.Add(lblFullName)

        Dim txtFullName As New TextBox()
        txtFullName.Name = "txtFullName"
        txtFullName.Location = New Point(20, 110)
        txtFullName.Size = New Size(560, 30)
        txtFullName.Font = New Font("Arial", 10)
        Me.Controls.Add(txtFullName)

        Dim lblPhone As New Label()
        lblPhone.Text = "Phone:"
        lblPhone.Font = New Font("Arial", 10)
        lblPhone.Location = New Point(20, 150)
        lblPhone.AutoSize = True
        Me.Controls.Add(lblPhone)

        Dim txtPhone As New TextBox()
        txtPhone.Name = "txtPhone"
        txtPhone.Location = New Point(20, 170)
        txtPhone.Size = New Size(560, 30)
        txtPhone.Font = New Font("Arial", 10)
        Me.Controls.Add(txtPhone)

        Dim lblAddress As New Label()
        lblAddress.Text = "Address:"
        lblAddress.Font = New Font("Arial", 10)
        lblAddress.Location = New Point(20, 210)
        lblAddress.AutoSize = True
        Me.Controls.Add(lblAddress)

        Dim txtAddress As New TextBox()
        txtAddress.Name = "txtAddress"
        txtAddress.Location = New Point(20, 230)
        txtAddress.Size = New Size(560, 80)
        txtAddress.Font = New Font("Arial", 10)
        txtAddress.Multiline = True
        Me.Controls.Add(txtAddress)

        ' Payment Method Section
        Dim lblPayment As New Label()
        lblPayment.Text = "Payment Method"
        lblPayment.Font = New Font("Arial", 12, FontStyle.Bold)
        lblPayment.Location = New Point(20, 330)
        lblPayment.AutoSize = True
        Me.Controls.Add(lblPayment)

        Dim cmbPayment As New ComboBox()
        cmbPayment.Name = "cmbPayment"
        cmbPayment.Location = New Point(20, 360)
        cmbPayment.Size = New Size(560, 30)
        cmbPayment.Font = New Font("Arial", 10)
        cmbPayment.Items.AddRange(New String() {"Credit Card", "Debit Card", "Online Banking", "Cash on Delivery"})
        cmbPayment.SelectedIndex = 0
        Me.Controls.Add(cmbPayment)

        ' Order Summary Section
        Dim lblSummary As New Label()
        lblSummary.Text = "Order Summary"
        lblSummary.Font = New Font("Arial", 12, FontStyle.Bold)
        lblSummary.Location = New Point(20, 410)
        lblSummary.AutoSize = True
        Me.Controls.Add(lblSummary)

        Dim lblSubtotal As New Label()
        lblSubtotal.Name = "lblSubtotal"
        lblSubtotal.Text = "Subtotal: ?0.00"
        lblSubtotal.Font = New Font("Arial", 10)
        lblSubtotal.Location = New Point(20, 440)
        lblSubtotal.AutoSize = True
        Me.Controls.Add(lblSubtotal)

        Dim lblShippingFee As New Label()
        lblShippingFee.Text = "Shipping Fee: ?150.00"
        lblShippingFee.Font = New Font("Arial", 10)
        lblShippingFee.Location = New Point(20, 465)
        lblShippingFee.AutoSize = True
        Me.Controls.Add(lblShippingFee)

        Dim lblTotal As New Label()
        lblTotal.Name = "lblTotal"
        lblTotal.Text = "Total: ?0.00"
        lblTotal.Font = New Font("Arial", 12, FontStyle.Bold)
        lblTotal.ForeColor = Color.FromArgb(70, 130, 180)
        lblTotal.Location = New Point(20, 495)
        lblTotal.AutoSize = True
        Me.Controls.Add(lblTotal)

        ' Place Order Button
        Dim btnPlaceOrder As New Button()
        btnPlaceOrder.Name = "btnPlaceOrder"
        btnPlaceOrder.Text = "PLACE ORDER"
        btnPlaceOrder.Location = New Point(20, 540)
        btnPlaceOrder.Size = New Size(560, 50)
        btnPlaceOrder.Font = New Font("Arial", 12, FontStyle.Bold)
        btnPlaceOrder.BackColor = Color.FromArgb(40, 167, 69)
        btnPlaceOrder.ForeColor = Color.White
        btnPlaceOrder.FlatStyle = FlatStyle.Flat
        btnPlaceOrder.Cursor = Cursors.Hand
        AddHandler btnPlaceOrder.Click, AddressOf BtnPlaceOrder_Click
        Me.Controls.Add(btnPlaceOrder)

        ' Cancel Button
        Dim btnCancel As New Button()
        btnCancel.Name = "btnCancel"
        btnCancel.Text = "CANCEL"
        btnCancel.Location = New Point(20, 600)
        btnCancel.Size = New Size(560, 40)
        btnCancel.Font = New Font("Arial", 10, FontStyle.Bold)
        btnCancel.BackColor = Color.FromArgb(108, 117, 125)
        btnCancel.ForeColor = Color.White
        btnCancel.FlatStyle = FlatStyle.Flat
        btnCancel.Cursor = Cursors.Hand
        AddHandler btnCancel.Click, AddressOf BtnCancel_Click
        Me.Controls.Add(btnCancel)
    End Sub

    Private Sub LoadUserProfile()
        Dim userProfile = DBAuthentication.GetUserProfile(userId)

        If userProfile IsNot Nothing Then
            Dim txtFullName As TextBox = Me.Controls("txtFullName")
            Dim txtPhone As TextBox = Me.Controls("txtPhone")
            Dim txtAddress As TextBox = Me.Controls("txtAddress")

            txtFullName.Text = If(IsDBNull(userProfile("full_name")), "", userProfile("full_name").ToString())
            txtPhone.Text = If(IsDBNull(userProfile("phone")), "", userProfile("phone").ToString())
            txtAddress.Text = If(IsDBNull(userProfile("address")), "", userProfile("address").ToString())
        End If
    End Sub

    Private Sub LoadOrderSummary()
        Dim cartTotal = DBEcommerce.GetCartTotal(userId)
        Dim shippingFee As Decimal = 150.0
        Dim grandTotal = cartTotal + shippingFee

        Dim lblSubtotal As Label = Me.Controls("lblSubtotal")
        Dim lblTotal As Label = Me.Controls("lblTotal")

        lblSubtotal.Text = "Subtotal: ?" & cartTotal.ToString("N2")
        lblTotal.Text = "Total: ?" & grandTotal.ToString("N2")
    End Sub

    Private Sub BtnPlaceOrder_Click(sender As Object, e As EventArgs)
        Dim txtFullName As TextBox = Me.Controls("txtFullName")
        Dim txtPhone As TextBox = Me.Controls("txtPhone")
        Dim txtAddress As TextBox = Me.Controls("txtAddress")
        Dim cmbPayment As ComboBox = Me.Controls("cmbPayment")

        If String.IsNullOrEmpty(txtFullName.Text) OrElse String.IsNullOrEmpty(txtAddress.Text) Then
            MessageBox.Show("Please fill in all shipping details.", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Update user profile
        DBAuthentication.UpdateUserProfile(userId, txtFullName.Text, txtPhone.Text, txtAddress.Text)

        ' Calculate total with shipping
        Dim cartTotal = DBEcommerce.GetCartTotal(userId)
        Dim shippingFee As Decimal = 150.0
        Dim totalAmount = cartTotal + shippingFee

        ' Create order (this also clears the database cart)
        Dim result = DBEcommerce.CreateOrder(userId, totalAmount, txtAddress.Text, cmbPayment.SelectedItem.ToString())

        If result.Item1 Then
            ' Success! Show confirmation
            MessageBox.Show(result.Item2 & vbCrLf & "Thank you for your purchase!" & vbCrLf & vbCrLf & "Your cart has been cleared.", "Order Placed Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information)

            ' Reset local Cart arrays for top-level forms using Cart.vb and clear DB cart (defensive)
            Try
                Cart.ResetCart()
            Catch ex As Exception
                Debug.WriteLine("Cart.ResetCart() - Note: Using database cart instead - " & ex.Message)
            End Try
            Try
                DBEcommerce.ClearCart(userId)
            Catch ex As Exception
                Debug.WriteLine("DBEcommerce.ClearCart() failed: " & ex.Message)
            End Try

            ' Reset UI: subtotal/total and input fields to defaults to be ready for next transaction
            Try
                Dim lblSubtotal As Label = Me.Controls("lblSubtotal")
                Dim lblTotal As Label = Me.Controls("lblTotal")
                If lblSubtotal IsNot Nothing Then lblSubtotal.Text = "Subtotal: ?0.00"
                If lblTotal IsNot Nothing Then lblTotal.Text = "Total: ?0.00"

                txtFullName.Text = String.Empty
                txtPhone.Text = String.Empty
                txtAddress.Text = String.Empty
                If cmbPayment IsNot Nothing Then cmbPayment.SelectedIndex = 0

                ' Notify other modules that inventory/cart changed so they can refresh
                Try
                    InventorySync.RaiseInventoryChanged(0, InventorySync.InventoryChangeType.OrderCompleted)
                Catch ex As Exception
                    Debug.WriteLine("InventorySync notify failed: " & ex.Message)
                End Try
            Catch ex As Exception
                Debug.WriteLine("Post-order reset failed: " & ex.Message)
            End Try

            ' Close checkout and return to shopping cart (which will refresh)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Else
            MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
