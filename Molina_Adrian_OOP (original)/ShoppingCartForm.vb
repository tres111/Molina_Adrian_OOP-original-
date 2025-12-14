Public Class ShoppingCartForm
Inherits Form

Private userId As Integer

Private Sub ShoppingCartForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Shopping Cart - Coziest Store"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(900, 600)
        Me.BackColor = Color.White

        userId = DBAuthentication.GetCurrentUserId()

        If userId = 0 Then
            MessageBox.Show("Please login first.", "Not Logged In", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        CreateUI()
        LoadCartItems()
    End Sub

    Private Sub CreateUI()
        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "Shopping Cart"
        lblTitle.Font = New Font("Arial", 18, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        Me.Controls.Add(lblTitle)

        ' DataGridView for Cart Items
        Dim dgvCart As New DataGridView()
        dgvCart.Name = "dgvCart"
        dgvCart.Location = New Point(20, 60)
        dgvCart.Size = New Size(860, 350)
        dgvCart.AllowUserToAddRows = False
        dgvCart.AllowUserToDeleteRows = False
        dgvCart.ReadOnly = False
        dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvCart.ColumnCount = 6
        dgvCart.Columns(0).Name = "Cart ID"
        dgvCart.Columns(1).Name = "Product Name"
        dgvCart.Columns(2).Name = "Category"
        dgvCart.Columns(3).Name = "Price"
        dgvCart.Columns(4).Name = "Quantity"
        dgvCart.Columns(5).Name = "Subtotal"
        dgvCart.BackgroundColor = Color.White
        dgvCart.GridColor = Color.LightGray
        AddHandler dgvCart.CellEndEdit, AddressOf DgvCart_CellEndEdit
        Me.Controls.Add(dgvCart)

        ' Total Label
        Dim lblTotal As New Label()
        lblTotal.Name = "lblTotal"
        lblTotal.Text = "Total: ?0.00"
        lblTotal.Font = New Font("Arial", 14, FontStyle.Bold)
        lblTotal.ForeColor = Color.FromArgb(70, 130, 180)
        lblTotal.Location = New Point(20, 430)
        lblTotal.AutoSize = True
        Me.Controls.Add(lblTotal)

        ' Remove Button
        Dim btnRemove As New Button()
        btnRemove.Name = "btnRemove"
        btnRemove.Text = "REMOVE SELECTED"
        btnRemove.Location = New Point(20, 480)
        btnRemove.Size = New Size(150, 40)
        btnRemove.Font = New Font("Arial", 10, FontStyle.Bold)
        btnRemove.BackColor = Color.FromArgb(220, 53, 69)
        btnRemove.ForeColor = Color.White
        btnRemove.FlatStyle = FlatStyle.Flat
        btnRemove.Cursor = Cursors.Hand
        AddHandler btnRemove.Click, AddressOf BtnRemove_Click
        Me.Controls.Add(btnRemove)

        ' Clear Cart Button
        Dim btnClear As New Button()
        btnClear.Name = "btnClear"
        btnClear.Text = "CLEAR CART"
        btnClear.Location = New Point(180, 480)
        btnClear.Size = New Size(150, 40)
        btnClear.Font = New Font("Arial", 10, FontStyle.Bold)
        btnClear.BackColor = Color.FromArgb(108, 117, 125)
        btnClear.ForeColor = Color.White
        btnClear.FlatStyle = FlatStyle.Flat
        btnClear.Cursor = Cursors.Hand
        AddHandler btnClear.Click, AddressOf BtnClear_Click
        Me.Controls.Add(btnClear)

        ' Checkout Button
        Dim btnCheckout As New Button()
        btnCheckout.Name = "btnCheckout"
        btnCheckout.Text = "PROCEED TO CHECKOUT"
        btnCheckout.Location = New Point(730, 480)
        btnCheckout.Size = New Size(150, 40)
        btnCheckout.Font = New Font("Arial", 10, FontStyle.Bold)
        btnCheckout.BackColor = Color.FromArgb(40, 167, 69)
        btnCheckout.ForeColor = Color.White
        btnCheckout.FlatStyle = FlatStyle.Flat
        btnCheckout.Cursor = Cursors.Hand
        AddHandler btnCheckout.Click, AddressOf BtnCheckout_Click
        Me.Controls.Add(btnCheckout)

        ' Promo Code Label
        Dim lblPromo As New Label()
        lblPromo.Text = "Promo Code:"
        lblPromo.Font = New Font("Arial", 10)
        lblPromo.Location = New Point(20, 540)
        lblPromo.AutoSize = True
        Me.Controls.Add(lblPromo)

        ' Promo Code TextBox
        Dim txtPromo As New TextBox()
        txtPromo.Name = "txtPromo"
        txtPromo.Location = New Point(120, 535)
        txtPromo.Size = New Size(150, 30)
        txtPromo.Font = New Font("Arial", 10)
        Me.Controls.Add(txtPromo)

        ' Apply Promo Button
        Dim btnApplyPromo As New Button()
        btnApplyPromo.Name = "btnApplyPromo"
        btnApplyPromo.Text = "APPLY"
        btnApplyPromo.Location = New Point(280, 535)
        btnApplyPromo.Size = New Size(80, 30)
        btnApplyPromo.Font = New Font("Arial", 9, FontStyle.Bold)
        btnApplyPromo.BackColor = Color.FromArgb(70, 130, 180)
        btnApplyPromo.ForeColor = Color.White
        btnApplyPromo.FlatStyle = FlatStyle.Flat
        btnApplyPromo.Cursor = Cursors.Hand
        AddHandler btnApplyPromo.Click, AddressOf BtnApplyPromo_Click
        Me.Controls.Add(btnApplyPromo)
    End Sub

    Private Sub LoadCartItems()
        Dim dgvCart As DataGridView = Me.Controls("dgvCart")
        Dim lblTotal As Label = Me.Controls("lblTotal")
        dgvCart.Rows.Clear()

        Dim cartItems = DBEcommerce.GetCartItems(userId)

        If cartItems.Rows.Count = 0 Then
            ' Cart is empty - show zero total
            lblTotal.Text = "Total: P0.00"
            lblTotal.ForeColor = Color.FromArgb(70, 130, 180)
            Return
        End If

        For Each row As DataRow In cartItems.Rows
            dgvCart.Rows.Add(row("id"), row("product_name"), row("category"), row("price"), row("quantity"), row("subtotal"))
        Next

        UpdateTotal()
    End Sub

    Private Sub UpdateTotal()
        Dim dgvCart As DataGridView = Me.Controls("dgvCart")
        Dim lblTotal As Label = Me.Controls("lblTotal")
        Dim total As Decimal = 0

        For Each row As DataGridViewRow In dgvCart.Rows
            If row.Cells(5).Value IsNot Nothing Then
                total += CDec(row.Cells(5).Value)
            End If
        Next

        lblTotal.Text = "Total: ?" & total.ToString("N2")
    End Sub

    Private Sub DgvCart_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs)
        If e.ColumnIndex = 4 Then ' Quantity column
            Dim dgvCart As DataGridView = Me.Controls("dgvCart")
            Dim cartId As Integer = CInt(dgvCart.Rows(e.RowIndex).Cells(0).Value)
            Dim newQuantity As Integer

            If Integer.TryParse(dgvCart.Rows(e.RowIndex).Cells(4).Value, newQuantity) Then
                Dim result = DBEcommerce.UpdateCartQuantity(cartId, newQuantity)
                If result.Item1 Then
                    LoadCartItems()
                Else
                    MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    LoadCartItems()
                End If
            End If
        End If
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs)
        Dim dgvCart As DataGridView = Me.Controls("dgvCart")

        If dgvCart.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim cartId As Integer = CInt(dgvCart.SelectedRows(0).Cells(0).Value)
        Dim result = DBEcommerce.RemoveFromCart(cartId)

        If result.Item1 Then
            MessageBox.Show(result.Item2, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadCartItems()
        Else
            MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnClear_Click(sender As Object, e As EventArgs)
        If MessageBox.Show("Are you sure you want to clear the cart?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Dim result = DBEcommerce.ClearCart(userId)
            If result.Item1 Then
                MessageBox.Show(result.Item2, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadCartItems()
            Else
                MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Private Sub BtnCheckout_Click(sender As Object, e As EventArgs)
        Dim checkoutForm As New CheckoutForm()
        ' Show checkout as dialog and wait for completion
        Dim result = checkoutForm.ShowDialog(Me)
        
        ' After checkout completes, refresh the cart display
        ' This will show empty cart (P=0.00) and all quantities reset to 0
        LoadCartItems()
        RefreshCartDisplay()
    End Sub

    Private Sub RefreshCartDisplay()
        Dim dgvCart As DataGridView = Me.Controls("dgvCart")
        Dim lblTotal As Label = Me.Controls("lblTotal")
        
        ' Check if cart is empty
        If dgvCart.Rows.Count = 0 Then
            lblTotal.Text = "Total: P0.00"
            lblTotal.ForeColor = Color.FromArgb(70, 130, 180)
        Else
            UpdateTotal()
        End If
    End Sub

    Private Sub BtnApplyPromo_Click(sender As Object, e As EventArgs)
        Dim txtPromo As TextBox = Me.Controls("txtPromo")
        Dim code As String = txtPromo.Text.Trim()

        If String.IsNullOrEmpty(code) Then
            MessageBox.Show("Please enter a promo code.", "Empty Code", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result = DBEcommerce.ValidatePromoCode(code)

        If result.Item1 Then
            MessageBox.Show(result.Item3, "Promo Applied", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' You can implement discount application logic here
        Else
            MessageBox.Show(result.Item3, "Invalid Code", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
End Class
