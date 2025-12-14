Public Class WishlistForm
Inherits Form

Private userId As Integer

Private Sub WishlistForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "My Wishlist - Coziest Store"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(900, 600)
        Me.BackColor = Color.White

        userId = DBAuthentication.GetCurrentUserId()
        CreateUI()
        LoadWishlist()
    End Sub

    Private Sub CreateUI()
        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "My Wishlist"
        lblTitle.Font = New Font("Arial", 18, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        Me.Controls.Add(lblTitle)

        ' DataGridView for Wishlist Items
        Dim dgvWishlist As New DataGridView()
        dgvWishlist.Name = "dgvWishlist"
        dgvWishlist.Location = New Point(20, 60)
        dgvWishlist.Size = New Size(860, 400)
        dgvWishlist.AllowUserToAddRows = False
        dgvWishlist.AllowUserToDeleteRows = False
        dgvWishlist.ReadOnly = True
        dgvWishlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvWishlist.ColumnCount = 6
        dgvWishlist.Columns(0).Name = "Wishlist ID"
        dgvWishlist.Columns(1).Name = "Product Name"
        dgvWishlist.Columns(2).Name = "Category"
        dgvWishlist.Columns(3).Name = "Price"
        dgvWishlist.Columns(4).Name = "Stock"
        dgvWishlist.Columns(5).Name = "Added Date"
        dgvWishlist.BackgroundColor = Color.White
        dgvWishlist.GridColor = Color.LightGray
        Me.Controls.Add(dgvWishlist)

        ' Add to Cart Button
        Dim btnAddToCart As New Button()
        btnAddToCart.Name = "btnAddToCart"
        btnAddToCart.Text = "ADD TO CART"
        btnAddToCart.Location = New Point(20, 480)
        btnAddToCart.Size = New Size(180, 40)
        btnAddToCart.Font = New Font("Arial", 10, FontStyle.Bold)
        btnAddToCart.BackColor = Color.FromArgb(40, 167, 69)
        btnAddToCart.ForeColor = Color.White
        btnAddToCart.FlatStyle = FlatStyle.Flat
        btnAddToCart.Cursor = Cursors.Hand
        AddHandler btnAddToCart.Click, AddressOf BtnAddToCart_Click
        Me.Controls.Add(btnAddToCart)

        ' Remove Button
        Dim btnRemove As New Button()
        btnRemove.Name = "btnRemove"
        btnRemove.Text = "REMOVE"
        btnRemove.Location = New Point(210, 480)
        btnRemove.Size = New Size(180, 40)
        btnRemove.Font = New Font("Arial", 10, FontStyle.Bold)
        btnRemove.BackColor = Color.FromArgb(220, 53, 69)
        btnRemove.ForeColor = Color.White
        btnRemove.FlatStyle = FlatStyle.Flat
        btnRemove.Cursor = Cursors.Hand
        AddHandler btnRemove.Click, AddressOf BtnRemove_Click
        Me.Controls.Add(btnRemove)

        ' Close Button
        Dim btnClose As New Button()
        btnClose.Name = "btnClose"
        btnClose.Text = "CLOSE"
        btnClose.Location = New Point(700, 480)
        btnClose.Size = New Size(180, 40)
        btnClose.Font = New Font("Arial", 10, FontStyle.Bold)
        btnClose.BackColor = Color.FromArgb(108, 117, 125)
        btnClose.ForeColor = Color.White
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Cursor = Cursors.Hand
        AddHandler btnClose.Click, AddressOf BtnClose_Click
        Me.Controls.Add(btnClose)
    End Sub

    Private Sub LoadWishlist()
        Dim dgvWishlist As DataGridView = Me.Controls("dgvWishlist")
        dgvWishlist.Rows.Clear()

        Dim wishlistItems = DBEcommerce.GetWishlist(userId)

        If wishlistItems.Rows.Count = 0 Then
            MessageBox.Show("Your wishlist is empty.", "Empty Wishlist", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        For Each row As DataRow In wishlistItems.Rows
            Dim addedDate As DateTime = CDate(row("added_at"))
            Dim stock As Integer = CInt(row("stock"))
            Dim stockStatus As String = If(stock > 0, stock.ToString(), "Out of Stock")
            dgvWishlist.Rows.Add(row("id"), row("product_name"), row("category"), "?" & CDec(row("price")).ToString("N2"), stockStatus, addedDate.ToString("yyyy-MM-dd"))
        Next
    End Sub

    Private Sub BtnAddToCart_Click(sender As Object, e As EventArgs)
        Dim dgvWishlist As DataGridView = Me.Controls("dgvWishlist")

        If dgvWishlist.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a product to add to cart.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim productId As Integer = CInt(dgvWishlist.SelectedRows(0).Cells(1).Value)
        Dim stock As String = dgvWishlist.SelectedRows(0).Cells(4).Value.ToString()

        If stock = "Out of Stock" Then
            MessageBox.Show("This product is out of stock.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result = DBEcommerce.AddToCart(userId, productId, 1)

        If result.Item1 Then
            MessageBox.Show(result.Item2, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnRemove_Click(sender As Object, e As EventArgs)
        Dim dgvWishlist As DataGridView = Me.Controls("dgvWishlist")

        If dgvWishlist.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim wishlistId As Integer = CInt(dgvWishlist.SelectedRows(0).Cells(0).Value)
        Dim result = DBEcommerce.RemoveFromWishlist(wishlistId)

        If result.Item1 Then
            MessageBox.Show(result.Item2, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadWishlist()
        Else
            MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
