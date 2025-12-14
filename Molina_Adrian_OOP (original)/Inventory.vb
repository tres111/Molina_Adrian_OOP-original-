Imports System.Data
Imports MySql.Data.MySqlClient

Public Class Inventory
    Private mainForm As Form1

    Public Sub New(frm As Form1)
        InitializeComponent()
        mainForm = frm
    End Sub

    Private Sub Inventory_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadInventoryData()
        ClearTextBoxes()
    End Sub

    Private Sub LoadInventoryData()
        Try
            Dim dt As DataTable = DBmySql.GetAllProducts()
            DataGridView1.DataSource = dt

            ' Format the DataGridView
            DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill

            If DataGridView1.Columns.Contains("id") Then
                DataGridView1.Columns("id").HeaderText = "ID"
                DataGridView1.Columns("id").ReadOnly = True
            End If

            If DataGridView1.Columns.Contains("product_name") Then
                DataGridView1.Columns("product_name").HeaderText = "Product Name"
            End If

            If DataGridView1.Columns.Contains("category") Then
                DataGridView1.Columns("category").HeaderText = "Category"
            End If

            If DataGridView1.Columns.Contains("price") Then
                DataGridView1.Columns("price").HeaderText = "Price (₱)"
                DataGridView1.Columns("price").DefaultCellStyle.Format = "N2"
            End If

            If DataGridView1.Columns.Contains("stock") Then
                DataGridView1.Columns("stock").HeaderText = "Stock"
            End If

            ' Allow selection
            DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect
            DataGridView1.MultiSelect = False

        Catch ex As Exception
            MessageBox.Show("Error loading inventory data: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Populate textboxes when clicking a row in DataGridView
    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        If e.RowIndex >= 0 Then
            Dim row As DataGridViewRow = DataGridView1.Rows(e.RowIndex)

            txtID.Text = row.Cells("id").Value.ToString()
            txtName.Text = row.Cells("product_name").Value.ToString()
            txtCategory.Text = row.Cells("category").Value.ToString()
            txtPrice.Text = row.Cells("price").Value.ToString()
            txtStock.Text = row.Cells("stock").Value.ToString()
        End If
    End Sub

    ' Clear all textboxes
    Private Sub ClearTextBoxes()
        txtID.Clear()
        txtName.Clear()
        txtCategory.Clear()
        txtPrice.Clear()
        txtStock.Clear()
    End Sub

    ' Add new product
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If String.IsNullOrWhiteSpace(txtName.Text) OrElse
           String.IsNullOrWhiteSpace(txtCategory.Text) OrElse
           String.IsNullOrWhiteSpace(txtPrice.Text) OrElse
           String.IsNullOrWhiteSpace(txtStock.Text) Then
            MessageBox.Show("Please fill in all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim price As Decimal = Convert.ToDecimal(txtPrice.Text)
            Dim stock As Integer = Convert.ToInt32(txtStock.Text)

            Using conn = DBmySql.GetConnection()
                conn.Open()
                Dim query As String = "INSERT INTO products (product_name, category, price, stock) VALUES (@name, @category, @price, @stock)"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", txtName.Text)
                    cmd.Parameters.AddWithValue("@category", txtCategory.Text)
                    cmd.Parameters.AddWithValue("@price", price)
                    cmd.Parameters.AddWithValue("@stock", stock)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadInventoryData()
            ClearTextBoxes()

        Catch ex As Exception
            MessageBox.Show("Error adding product: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Update existing product
    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        If String.IsNullOrWhiteSpace(txtID.Text) Then
            MessageBox.Show("Please select a product to update!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtName.Text) OrElse
           String.IsNullOrWhiteSpace(txtCategory.Text) OrElse
           String.IsNullOrWhiteSpace(txtPrice.Text) OrElse
           String.IsNullOrWhiteSpace(txtStock.Text) Then
            MessageBox.Show("Please fill in all fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim id As Integer = Convert.ToInt32(txtID.Text)
            Dim price As Decimal = Convert.ToDecimal(txtPrice.Text)
            Dim stock As Integer = Convert.ToInt32(txtStock.Text)

            Using conn = DBmySql.GetConnection()
                conn.Open()
                Dim query As String = "UPDATE products SET product_name = @name, category = @category, price = @price, stock = @stock WHERE id = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@id", id)
                    cmd.Parameters.AddWithValue("@name", txtName.Text)
                    cmd.Parameters.AddWithValue("@category", txtCategory.Text)
                    cmd.Parameters.AddWithValue("@price", price)
                    cmd.Parameters.AddWithValue("@stock", stock)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadInventoryData()
            ClearTextBoxes()

        Catch ex As Exception
            MessageBox.Show("Error updating product: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Delete product
    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If String.IsNullOrWhiteSpace(txtID.Text) Then
            MessageBox.Show("Please select a product to delete!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                Dim id As Integer = Convert.ToInt32(txtID.Text)

                Using conn = DBmySql.GetConnection()
                    conn.Open()
                    Dim query As String = "DELETE FROM products WHERE id = @id"
                    Using cmd As New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@id", id)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadInventoryData()
                ClearTextBoxes()

            Catch ex As Exception
                MessageBox.Show("Error deleting product: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    ' Save changes (batch update stocks from DataGridView)
    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            For Each row As DataGridViewRow In DataGridView1.Rows
                If Not row.IsNewRow Then
                    Dim id As Integer = Convert.ToInt32(row.Cells("id").Value)
                    Dim newStock As Integer = Convert.ToInt32(row.Cells("stock").Value)
                    DBmySql.UpdateStock(id, newStock)
                End If
            Next
            MessageBox.Show("Inventory updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadInventoryData()
        Catch ex As Exception
            MessageBox.Show("Error saving inventory: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Refresh data
    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadInventoryData()
        ClearTextBoxes()
        MessageBox.Show("Data refreshed!", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' Search functionality (if you have a search textbox)
    ' Search functionality - searches by ID, Product Name, or Category
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        If String.IsNullOrWhiteSpace(txtSearch.Text) Then
            LoadInventoryData()
        Else
            Try
                Using conn = DBmySql.GetConnection()
                    conn.Open()

                    ' Try to parse as integer for ID search
                    Dim searchId As Integer
                    Dim isNumeric As Boolean = Integer.TryParse(txtSearch.Text, searchId)

                    Dim query As String
                    Dim cmd As MySqlCommand

                    If isNumeric Then
                        ' If search text is a number, search by ID, product name, or category
                        query = "SELECT id, product_name, category, price, stock FROM products WHERE id = @id OR product_name LIKE @search OR category LIKE @search ORDER BY category, id"
                        cmd = New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@id", searchId)
                        cmd.Parameters.AddWithValue("@search", "%" & txtSearch.Text & "%")
                    Else
                        ' If search text is not a number, only search by product name or category
                        query = "SELECT id, product_name, category, price, stock FROM products WHERE product_name LIKE @search OR category LIKE @search ORDER BY category, id"
                        cmd = New MySqlCommand(query, conn)
                        cmd.Parameters.AddWithValue("@search", "%" & txtSearch.Text & "%")
                    End If

                    Using cmd
                        Dim adapter As New MySqlDataAdapter(cmd)
                        Dim dt As New DataTable()
                        adapter.Fill(dt)
                        DataGridView1.DataSource = dt

                        ' If only one result found, automatically populate the textboxes
                        If dt.Rows.Count = 1 Then
                            txtID.Text = dt.Rows(0)("id").ToString()
                            txtName.Text = dt.Rows(0)("product_name").ToString()
                            txtCategory.Text = dt.Rows(0)("category").ToString()
                            txtPrice.Text = dt.Rows(0)("price").ToString()
                            txtStock.Text = dt.Rows(0)("stock").ToString()
                        End If
                    End Using
                End Using
            Catch ex As Exception
                MessageBox.Show("Error searching: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

End Class