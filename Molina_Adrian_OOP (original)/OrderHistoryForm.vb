Public Class OrderHistoryForm
Inherits Form

Private userId As Integer

Private Sub OrderHistoryForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Order History - Coziest Store"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(1000, 600)
        Me.BackColor = Color.White

        userId = DBAuthentication.GetCurrentUserId()
        CreateUI()
        LoadOrderHistory()
    End Sub

    Private Sub CreateUI()
        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "Order History"
        lblTitle.Font = New Font("Arial", 18, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        Me.Controls.Add(lblTitle)

        ' DataGridView for Orders
        Dim dgvOrders As New DataGridView()
        dgvOrders.Name = "dgvOrders"
        dgvOrders.Location = New Point(20, 60)
        dgvOrders.Size = New Size(960, 400)
        dgvOrders.AllowUserToAddRows = False
        dgvOrders.AllowUserToDeleteRows = False
        dgvOrders.ReadOnly = True
        dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvOrders.ColumnCount = 6
        dgvOrders.Columns(0).Name = "Order ID"
        dgvOrders.Columns(1).Name = "Order Date"
        dgvOrders.Columns(2).Name = "Total Amount"
        dgvOrders.Columns(3).Name = "Status"
        dgvOrders.Columns(4).Name = "Shipping Address"
        dgvOrders.Columns(5).Name = "Payment Method"
        dgvOrders.BackgroundColor = Color.White
        dgvOrders.GridColor = Color.LightGray
        AddHandler dgvOrders.CellDoubleClick, AddressOf DgvOrders_CellDoubleClick
        Me.Controls.Add(dgvOrders)

        ' View Details Button
        Dim btnDetails As New Button()
        btnDetails.Name = "btnDetails"
        btnDetails.Text = "VIEW DETAILS"
        btnDetails.Location = New Point(20, 480)
        btnDetails.Size = New Size(200, 40)
        btnDetails.Font = New Font("Arial", 10, FontStyle.Bold)
        btnDetails.BackColor = Color.FromArgb(70, 130, 180)
        btnDetails.ForeColor = Color.White
        btnDetails.FlatStyle = FlatStyle.Flat
        btnDetails.Cursor = Cursors.Hand
        AddHandler btnDetails.Click, AddressOf BtnDetails_Click
        Me.Controls.Add(btnDetails)

        ' Close Button
        Dim btnClose As New Button()
        btnClose.Name = "btnClose"
        btnClose.Text = "CLOSE"
        btnClose.Location = New Point(780, 480)
        btnClose.Size = New Size(200, 40)
        btnClose.Font = New Font("Arial", 10, FontStyle.Bold)
        btnClose.BackColor = Color.FromArgb(108, 117, 125)
        btnClose.ForeColor = Color.White
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Cursor = Cursors.Hand
        AddHandler btnClose.Click, AddressOf BtnClose_Click
        Me.Controls.Add(btnClose)
    End Sub

    Private Sub LoadOrderHistory()
        Dim dgvOrders As DataGridView = Me.Controls("dgvOrders")
        dgvOrders.Rows.Clear()

        Dim orders = DBEcommerce.GetOrderHistory(userId)

        If orders.Rows.Count = 0 Then
            MessageBox.Show("You have no orders yet.", "No Orders", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        For Each row As DataRow In orders.Rows
            Dim orderDate As DateTime = CDate(row("order_date"))
            dgvOrders.Rows.Add(row("id"), orderDate.ToString("yyyy-MM-dd HH:mm"), "?" & CDec(row("total_amount")).ToString("N2"), row("status"), row("shipping_address"), row("payment_method"))
        Next
    End Sub

    Private Sub BtnDetails_Click(sender As Object, e As EventArgs)
        Dim dgvOrders As DataGridView = Me.Controls("dgvOrders")

        If dgvOrders.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an order to view details.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim orderId As Long = CLng(dgvOrders.SelectedRows(0).Cells(0).Value)
        ShowOrderDetails(orderId)
    End Sub

    Private Sub DgvOrders_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim dgvOrders As DataGridView = Me.Controls("dgvOrders")
            Dim orderId As Long = CLng(dgvOrders.Rows(e.RowIndex).Cells(0).Value)
            ShowOrderDetails(orderId)
        End If
    End Sub

    Private Sub ShowOrderDetails(orderId As Long)
        Dim detailsForm As New Form()
        detailsForm.Text = "Order Details #" & orderId & " - Coziest Store"
        detailsForm.FormBorderStyle = FormBorderStyle.FixedSingle
        detailsForm.MaximizeBox = False
        detailsForm.StartPosition = FormStartPosition.CenterParent
        detailsForm.Size = New Size(800, 500)
        detailsForm.BackColor = Color.White

        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "Order #" & orderId
        lblTitle.Font = New Font("Arial", 16, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        detailsForm.Controls.Add(lblTitle)

        ' DataGridView for Order Items
        Dim dgvItems As New DataGridView()
        dgvItems.Location = New Point(20, 60)
        dgvItems.Size = New Size(760, 350)
        dgvItems.AllowUserToAddRows = False
        dgvItems.AllowUserToDeleteRows = False
        dgvItems.ReadOnly = True
        dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvItems.ColumnCount = 5
        dgvItems.Columns(0).Name = "Product Name"
        dgvItems.Columns(1).Name = "Category"
        dgvItems.Columns(2).Name = "Quantity"
        dgvItems.Columns(3).Name = "Price"
        dgvItems.Columns(4).Name = "Subtotal"
        dgvItems.BackgroundColor = Color.White
        dgvItems.GridColor = Color.LightGray
        detailsForm.Controls.Add(dgvItems)

        ' Load order items
        Dim orderItems = DBEcommerce.GetOrderDetails(orderId)

        For Each row As DataRow In orderItems.Rows
            Dim price As Decimal = CDec(row("price"))
            Dim subtotal As Decimal = CDec(row("subtotal"))
            dgvItems.Rows.Add(row("product_name"), row("category"), row("quantity"), "?" & price.ToString("N2"), "?" & subtotal.ToString("N2"))
        Next

        ' Close Button
        Dim btnClose As New Button()
        btnClose.Text = "CLOSE"
        btnClose.Location = New Point(620, 430)
        btnClose.Size = New Size(160, 40)
        btnClose.Font = New Font("Arial", 10, FontStyle.Bold)
        btnClose.BackColor = Color.FromArgb(108, 117, 125)
        btnClose.ForeColor = Color.White
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Cursor = Cursors.Hand
        AddHandler btnClose.Click, Sub() detailsForm.Close()
        detailsForm.Controls.Add(btnClose)

        detailsForm.ShowDialog(Me)
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
