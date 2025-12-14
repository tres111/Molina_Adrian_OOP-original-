Imports System.Windows.Forms

''' <summary>
''' Form for displaying sales analytics and reports
''' </summary>
Public Class SalesAnalyticsForm
    Inherits Form

    Private currentAdmin As AdminUser
    Private dgvSalesData As DataGridView
    Private dgvBestSellers As DataGridView
    Private rtbReport As RichTextBox
    Private WithEvents cmbPeriod As ComboBox
    Private WithEvents btnGenerateReport As Button
    Private WithEvents btnExport As Button

    Public Sub New(admin As AdminUser)
        MyBase.New()
        currentAdmin = admin
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Sales Analytics & Reports"
        Me.Size = New Size(1100, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        ' Control panel
        Dim pnlControls As New Panel()
        pnlControls.Dock = DockStyle.Top
        pnlControls.Height = 60
        pnlControls.BackColor = Color.LightGray
        pnlControls.Padding = New Padding(10)

        Dim lblPeriod As New Label()
        lblPeriod.Text = "Report Period:"
        lblPeriod.Location = New Point(10, 15)
        lblPeriod.AutoSize = True
        pnlControls.Controls.Add(lblPeriod)

        cmbPeriod = New ComboBox()
        cmbPeriod.Location = New Point(110, 12)
        cmbPeriod.Size = New Size(150, 25)
        cmbPeriod.DropDownStyle = ComboBoxStyle.DropDownList
        cmbPeriod.Items.AddRange({"Daily", "Weekly", "Monthly", "Quarterly", "Yearly"})
        cmbPeriod.SelectedIndex = 2
        pnlControls.Controls.Add(cmbPeriod)

        btnGenerateReport = New Button()
        btnGenerateReport.Text = "Generate Report"
        btnGenerateReport.Location = New Point(280, 12)
        btnGenerateReport.Size = New Size(120, 25)
        btnGenerateReport.BackColor = Color.Blue
        btnGenerateReport.ForeColor = Color.White
        pnlControls.Controls.Add(btnGenerateReport)

        btnExport = New Button()
        btnExport.Text = "Export to Excel"
        btnExport.Location = New Point(410, 12)
        btnExport.Size = New Size(120, 25)
        btnExport.BackColor = Color.Green
        btnExport.ForeColor = Color.White
        pnlControls.Controls.Add(btnExport)

        Me.Controls.Add(pnlControls)

        ' Tab control for different views
        Dim tabControl As New TabControl()
        tabControl.Dock = DockStyle.Fill
        tabControl.Padding = New Point(10, 10)

        ' Summary tab
        Dim tabSummary As New TabPage("Summary Report")
        rtbReport = New RichTextBox()
        rtbReport.Dock = DockStyle.Fill
        rtbReport.Font = New Font("Courier New", 9)
        rtbReport.ReadOnly = True
        tabSummary.Controls.Add(rtbReport)
        tabControl.TabPages.Add(tabSummary)

        ' Best Sellers tab
        Dim tabBestSellers As New TabPage("Best Sellers")
        dgvBestSellers = New DataGridView()
        dgvBestSellers.Dock = DockStyle.Fill
        dgvBestSellers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvBestSellers.ReadOnly = True
        dgvBestSellers.Columns.Add("Rank", "Rank")
        dgvBestSellers.Columns.Add("ProductName", "Product Name")
        dgvBestSellers.Columns.Add("QuantitySold", "Quantity Sold")
        dgvBestSellers.Columns.Add("TotalSales", "Total Sales")
        tabBestSellers.Controls.Add(dgvBestSellers)
        tabControl.TabPages.Add(tabBestSellers)

        ' Daily Sales tab
        Dim tabDailySales As New TabPage("Daily Sales")
        dgvSalesData = New DataGridView()
        dgvSalesData.Dock = DockStyle.Fill
        dgvSalesData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvSalesData.ReadOnly = True
        dgvSalesData.Columns.Add("Date", "Date")
        dgvSalesData.Columns.Add("Orders", "Orders")
        dgvSalesData.Columns.Add("Revenue", "Revenue")
        dgvSalesData.Columns.Add("Items", "Items Sold")
        tabDailySales.Controls.Add(dgvSalesData)
        tabControl.TabPages.Add(tabDailySales)

        Me.Controls.Add(tabControl)

        LoadSalesReport()
    End Sub

    Private Sub LoadSalesReport()
        Try
            ' Create sample sales report
            Dim report As New SalesReport()
            report.Period = SalesReport.ReportPeriod.Monthly
            report.TotalOrders = 156
            report.TotalRevenue = 125000
            report.TotalDiscount = 5000
            report.TotalTax = 15000
            report.TotalShipping = 2000
            report.TotalItemsSold = 342
            report.CalculateAverageOrderValue()

            ' Add sample best sellers
            report.BestSellingProducts.Add(New SalesReport.ProductSales With {
                .Rank = 1,
                .ProductName = "Signature Script Tee",
                .QuantitySold = 45,
                .TotalSales = 11250
            })

            report.BestSellingProducts.Add(New SalesReport.ProductSales With {
                .Rank = 2,
                .ProductName = "Classic Court Sneakers",
                .QuantitySold = 38,
                .TotalSales = 57000
            })

            ' Display report
            rtbReport.Text = report.GenerateReportSummary()

            ' Populate best sellers
            For Each product In report.BestSellingProducts
                dgvBestSellers.Rows.Add(product.Rank, product.ProductName, product.QuantitySold, product.TotalSales)
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading sales report: " & ex.Message, "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnGenerateReport_Click(sender As Object, e As EventArgs) Handles btnGenerateReport.Click
        LoadSalesReport()
        MessageBox.Show("Report generated successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        MessageBox.Show("Export to Excel feature coming soon!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub SalesAnalyticsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Form loaded
    End Sub
End Class
