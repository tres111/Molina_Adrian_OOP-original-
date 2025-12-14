Imports System.Windows.Forms

''' <summary>
''' Admin Panel - Main admin interface for inventory and sales management
''' </summary>
Public Class AdminPanel
    Inherits Form

    Private currentAdmin As AdminUser
    Private dashboard As AdminDashboard
    Private alertManager As StockAlertManager
    Private auditManager As InventoryAuditManager

    Private WithEvents btnRefresh As Button
    Private dgvProducts As DataGridView
    Private rtbDashboard As RichTextBox

    Public Sub New(admin As AdminUser)
        MyBase.New()
        currentAdmin = admin
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Admin Panel - " & currentAdmin.FullName
        Me.Size = New Size(1200, 800)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Dashboard
        dashboard = New AdminDashboard()
        alertManager = New StockAlertManager()
        auditManager = New InventoryAuditManager()

        ' Toolbar
        Dim pnlToolbar As New Panel()
        pnlToolbar.Dock = DockStyle.Top
        pnlToolbar.Height = 50
        pnlToolbar.BackColor = Color.LightGray
        
        btnRefresh = New Button()
        btnRefresh.Text = "Refresh Metrics"
        btnRefresh.BackColor = Color.Blue
        btnRefresh.ForeColor = Color.White
        btnRefresh.Location = New Point(10, 10)
        btnRefresh.Size = New Size(150, 30)
        pnlToolbar.Controls.Add(btnRefresh)

        Me.Controls.Add(pnlToolbar)

        ' Dashboard display
        rtbDashboard = New RichTextBox()
        rtbDashboard.Dock = DockStyle.Top
        rtbDashboard.Height = 200
        rtbDashboard.Font = New Font("Courier New", 9)
        rtbDashboard.ReadOnly = True
        Me.Controls.Add(rtbDashboard)

        ' Products grid
        dgvProducts = New DataGridView()
        dgvProducts.Dock = DockStyle.Fill
        dgvProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvProducts.Columns.Add("ID", "ID")
        dgvProducts.Columns.Add("Name", "Product Name")
        dgvProducts.Columns.Add("Category", "Category")
        dgvProducts.Columns.Add("Price", "Price")
        dgvProducts.Columns.Add("Stock", "Stock")
        Me.Controls.Add(dgvProducts)

        LoadDashboard()
    End Sub

    Private Sub LoadDashboard()
        Try
            dashboard.UpdateMetrics()
            rtbDashboard.Text = dashboard.GetStatusSummary()

            ' Load products
            dgvProducts.Rows.Clear()
            Dim dt = DBmySql.GetAllProducts()

            For Each row In dt.Rows
                dgvProducts.Rows.Add(
                    row("id"),
                    row("product_name"),
                    row("category"),
                    row("price"),
                    row("stock")
                )
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading dashboard: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadDashboard()
        MessageBox.Show("Dashboard refreshed successfully", "Success")
    End Sub
End Class
