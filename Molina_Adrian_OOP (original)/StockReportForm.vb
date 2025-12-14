Imports System.Data
Imports MySql.Data.MySqlClient

''' <summary>
''' Stock Report Form - View stock history and analysis
''' </summary>
Public Class StockReportForm
    Inherits Form

    Private adminId As Integer

    Public Sub New()
        ' Initialize form properties
        Me.Text = "Stock Management Reports"
        Me.Width = 1200
        Me.Height = 700
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.White
    End Sub

    Private Sub StockReportForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        adminId = SessionManager.CurrentUserId
        CreateUI()
        LoadReports()
    End Sub

    Private Sub CreateUI()
        ' Create tab control
        Dim tabControl As New TabControl()
        tabControl.Dock = DockStyle.Fill
        Me.Controls.Add(tabControl)

        ' Tab 1: Stock Update History
        Dim tabHistory As New TabPage("Stock Update History")
        CreateHistoryTab(tabHistory)
        tabControl.TabPages.Add(tabHistory)

        ' Tab 2: Low Stock Alerts
        Dim tabAlerts As New TabPage("Low Stock Alerts")
        CreateAlertsTab(tabAlerts)
        tabControl.TabPages.Add(tabAlerts)

        ' Tab 3: Stock Summary
        Dim tabSummary As New TabPage("Stock Summary")
        CreateSummaryTab(tabSummary)
        tabControl.TabPages.Add(tabSummary)

        ' Tab 4: Stock Statistics
        Dim tabStats As New TabPage("Stock Statistics")
        CreateStatsTab(tabStats)
        tabControl.TabPages.Add(tabStats)
    End Sub

    Private Sub CreateHistoryTab(tab As TabPage)
        ' DataGridView for stock update logs
        Dim dgv As New DataGridView()
        dgv.Dock = DockStyle.Fill
        dgv.ReadOnly = True
        dgv.AllowUserToAddRows = False
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv.Name = "dgvHistory"
        tab.Controls.Add(dgv)

        ' Filter panel
        Dim pnlFilter As New Panel()
        pnlFilter.Dock = DockStyle.Top
        pnlFilter.Height = 50
        pnlFilter.BorderStyle = BorderStyle.FixedSingle

        Dim lblProduct As New Label()
        lblProduct.Text = "Product ID:"
        lblProduct.Location = New Point(10, 15)
        lblProduct.AutoSize = True
        pnlFilter.Controls.Add(lblProduct)

        Dim txtProductId As New TextBox()
        txtProductId.Name = "txtProductId"
        txtProductId.Location = New Point(100, 12)
        txtProductId.Width = 100
        pnlFilter.Controls.Add(txtProductId)

        Dim btnFilter As New Button()
        btnFilter.Text = "Filter"
        btnFilter.Location = New Point(210, 12)
        btnFilter.Width = 80
        btnFilter.Height = 25
        AddHandler btnFilter.Click, Sub()
                                        Try
                                            Dim productId As Integer = 0
                                            If Integer.TryParse(txtProductId.Text, productId) Then
                                                Dim dt = StockManagementService.GetStockUpdateLogs(productId, 1000)
                                                dgv.DataSource = dt
                                            Else
                                                LoadAllStockHistory()
                                            End If
                                        Catch ex As Exception
                                            MessageBox.Show("Error: " & ex.Message)
                                        End Try
                                    End Sub
        pnlFilter.Controls.Add(btnFilter)

        Dim btnExport As New Button()
        btnExport.Text = "Export to CSV"
        btnExport.Location = New Point(300, 12)
        btnExport.Width = 100
        btnExport.Height = 25
        AddHandler btnExport.Click, AddressOf ExportToCSV
        pnlFilter.Controls.Add(btnExport)

        tab.Controls.Add(pnlFilter)
        tab.Controls.SetChildIndex(pnlFilter, 0)

        LoadAllStockHistory()
    End Sub

    Private Sub CreateAlertsTab(tab As TabPage)
        ' DataGridView for alerts
        Dim dgv As New DataGridView()
        dgv.Dock = DockStyle.Fill
        dgv.ReadOnly = False
        dgv.AllowUserToAddRows = False
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv.Name = "dgvAlerts"
        tab.Controls.Add(dgv)

        ' Status panel
        Dim pnlStatus As New Panel()
        pnlStatus.Dock = DockStyle.Top
        pnlStatus.Height = 50
        pnlStatus.BorderStyle = BorderStyle.FixedSingle

        Dim lblStatus As New Label()
        lblStatus.Text = "Show:"
        lblStatus.Location = New Point(10, 15)
        lblStatus.AutoSize = True
        pnlStatus.Controls.Add(lblStatus)

        Dim cmbStatus As New ComboBox()
        cmbStatus.Name = "cmbStatus"
        cmbStatus.Location = New Point(60, 12)
        cmbStatus.Width = 150
        cmbStatus.Items.AddRange(New String() {"PENDING", "ACKNOWLEDGED", "RESOLVED", "ALL"})
        cmbStatus.SelectedIndex = 0
        AddHandler cmbStatus.SelectedIndexChanged, Sub()
                                                       LoadAlerts(cmbStatus.SelectedItem.ToString())
                                                   End Sub
        pnlStatus.Controls.Add(cmbStatus)

        Dim btnAcknowledge As New Button()
        btnAcknowledge.Text = "Acknowledge Selected"
        btnAcknowledge.Location = New Point(220, 12)
        btnAcknowledge.Width = 150
        btnAcknowledge.Height = 25
        AddHandler btnAcknowledge.Click, Sub()
                                             Try
                                                 If dgv.SelectedRows.Count > 0 Then
                                                     Dim alertId = CInt(dgv.SelectedRows(0).Cells(0).Value)
                                                     StockManagementService.AcknowledgeLowStockAlert(alertId, adminId)
                                                     MessageBox.Show("Alert acknowledged!", "Success")
                                                     LoadAlerts(cmbStatus.SelectedItem.ToString())
                                                 End If
                                             Catch ex As Exception
                                                 MessageBox.Show("Error: " & ex.Message)
                                             End Try
                                         End Sub
        pnlStatus.Controls.Add(btnAcknowledge)

        tab.Controls.Add(pnlStatus)
        tab.Controls.SetChildIndex(pnlStatus, 0)

        LoadAlerts("PENDING")
    End Sub

    Private Sub CreateSummaryTab(tab As TabPage)
        ' DataGridView for summary
        Dim dgv As New DataGridView()
        dgv.Dock = DockStyle.Fill
        dgv.ReadOnly = True
        dgv.AllowUserToAddRows = False
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        dgv.Name = "dgvSummary"
        tab.Controls.Add(dgv)

        LoadSummary()
    End Sub

    Private Sub CreateStatsTab(tab As TabPage)
        ' Create labels for statistics
        Dim lblStats As New Label()
        lblStats.Dock = DockStyle.Top
        lblStats.Height = 300
        lblStats.Font = New Font("Arial", 12, FontStyle.Regular)
        lblStats.Padding = New Padding(20)
        lblStats.Name = "lblStats"
        tab.Controls.Add(lblStats)

        LoadStats(lblStats)
    End Sub

    Private Sub LoadReports()
        ' Load all data
        LoadAllStockHistory()
        LoadAlerts("PENDING")
        LoadSummary()
    End Sub

    Private Sub LoadAllStockHistory()
        Try
            Using conn = New MySqlConnection("server=localhost; userid=root; password=; database=coziest; port=3306;")
                conn.Open()
                Dim query = "SELECT id, product_id, product_name, admin_name, previous_stock, new_stock, quantity_change, change_type, reason, timestamp FROM stock_update_logs ORDER BY timestamp DESC LIMIT 500"
                Using cmd = New MySqlCommand(query, conn)
                    Dim adapter = New MySqlDataAdapter(cmd)
                    Dim dt = New DataTable()
                    adapter.Fill(dt)

                    Dim dgv = Me.Controls.OfType(Of TabControl)().FirstOrDefault()?.TabPages(0).Controls("dgvHistory")
                    If dgv IsNot Nothing Then
                        CType(dgv, DataGridView).DataSource = dt
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error loading stock history: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadAlerts(status As String)
        Try
            Using conn = New MySqlConnection("server=localhost; userid=root; password=; database=coziest; port=3306;")
                conn.Open()
                Dim query = "SELECT id, product_id, product_name, current_stock, alert_threshold, alert_type, status, created_at FROM low_stock_alerts"
                If status <> "ALL" Then
                    query &= " WHERE status = '" & status & "'"
                End If
                query &= " ORDER BY alert_type DESC, created_at DESC"

                Using cmd = New MySqlCommand(query, conn)
                    Dim adapter = New MySqlDataAdapter(cmd)
                    Dim dt = New DataTable()
                    adapter.Fill(dt)

                    Dim dgv = Me.Controls.OfType(Of TabControl)().FirstOrDefault()?.TabPages(1).Controls("dgvAlerts")
                    If dgv IsNot Nothing Then
                        CType(dgv, DataGridView).DataSource = dt
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error loading alerts: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadSummary()
        Try
            Using conn = New MySqlConnection("server=localhost; userid=root; password=; database=coziest; port=3306;")
                conn.Open()
                Dim query = "SELECT * FROM v_stock_update_summary ORDER BY product_name"
                Using cmd = New MySqlCommand(query, conn)
                    Dim adapter = New MySqlDataAdapter(cmd)
                    Dim dt = New DataTable()
                    adapter.Fill(dt)

                    Dim dgv = Me.Controls.OfType(Of TabControl)().FirstOrDefault()?.TabPages(2).Controls("dgvSummary")
                    If dgv IsNot Nothing Then
                        CType(dgv, DataGridView).DataSource = dt
                    End If
                End Using
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error loading summary: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadStats(lblStats As Label)
        Try
            Using conn = New MySqlConnection("server=localhost; userid=root; password=; database=coziest; port=3306;")
                conn.Open()

                ' Get various statistics
                Dim totalUpdates As Integer = 0
                Dim pendingAlerts As Integer = 0
                Dim criticalAlerts As Integer = 0
                Dim outOfStockProducts As Integer = 0

                ' Total stock updates
                Using cmd = New MySqlCommand("SELECT COUNT(*) FROM stock_update_logs", conn)
                    totalUpdates = CInt(cmd.ExecuteScalar())
                End Using

                ' Pending alerts
                Using cmd = New MySqlCommand("SELECT COUNT(*) FROM low_stock_alerts WHERE status = 'PENDING'", conn)
                    pendingAlerts = CInt(cmd.ExecuteScalar())
                End Using

                ' Critical alerts
                Using cmd = New MySqlCommand("SELECT COUNT(*) FROM low_stock_alerts WHERE alert_type = 'CRITICAL'", conn)
                    criticalAlerts = CInt(cmd.ExecuteScalar())
                End Using

                ' Out of stock products
                Using cmd = New MySqlCommand("SELECT COUNT(*) FROM products WHERE stock = 0", conn)
                    outOfStockProducts = CInt(cmd.ExecuteScalar())
                End Using

                Dim stats = String.Format("
Stock Management Statistics
?????????????????????????????????????????????

Total Stock Updates:        {0}
Pending Alerts:             {1}
Critical Alerts:            {2}
Out of Stock Products:      {3}

Last Updated:               {4}", totalUpdates, pendingAlerts, criticalAlerts, outOfStockProducts, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))

                lblStats.Text = stats
            End Using
        Catch ex As Exception
            Debug.WriteLine("Error loading stats: " & ex.Message)
        End Try
    End Sub

    Private Sub ExportToCSV()
        Try
            Dim saveDialog As New SaveFileDialog()
            saveDialog.Filter = "CSV Files|*.csv"
            saveDialog.FileName = "stock_history_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".csv"

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Dim dgv = Me.Controls.OfType(Of TabControl)().FirstOrDefault()?.TabPages(0).Controls("dgvHistory")
                If dgv IsNot Nothing Then
                    ExportDataGridViewToCSV(CType(dgv, DataGridView), saveDialog.FileName)
                    MessageBox.Show("Exported successfully!", "Success")
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub ExportDataGridViewToCSV(dgv As DataGridView, filePath As String)
        Using sw = New System.IO.StreamWriter(filePath)
            ' Write headers
            For i = 0 To dgv.Columns.Count - 1
                If i > 0 Then sw.Write(",")
                sw.Write("""" & dgv.Columns(i).HeaderText & """")
            Next
            sw.WriteLine()

            ' Write rows
            For Each row As DataGridViewRow In dgv.Rows
                For i = 0 To dgv.Columns.Count - 1
                    If i > 0 Then sw.Write(",")
                    sw.Write("""" & row.Cells(i).Value.ToString() & """")
                Next
                sw.WriteLine()
            Next
        End Using
    End Sub
End Class
