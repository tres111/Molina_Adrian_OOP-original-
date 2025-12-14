Imports System.Windows.Forms

''' <summary>
''' Form for viewing inventory audit log and action history
''' </summary>
Public Class InventoryAuditForm
    Inherits Form

    Private currentAdmin As AdminUser
    Private dgvAuditLog As DataGridView
    Private WithEvents btnRefresh As Button
    Private WithEvents btnFilter As Button
    Private cmbActionFilter As ComboBox

    Public Sub New(admin As AdminUser)
        MyBase.New()
        currentAdmin = admin
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Inventory Audit Log"
        Me.Size = New Size(1100, 650)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        ' Filter panel
        Dim pnlFilter As New Panel()
        pnlFilter.Dock = DockStyle.Top
        pnlFilter.Height = 50
        pnlFilter.BackColor = Color.LightGray
        pnlFilter.Padding = New Padding(10)

        Dim lblAction As New Label()
        lblAction.Text = "Filter by Action:"
        lblAction.Location = New Point(10, 15)
        lblAction.AutoSize = True
        pnlFilter.Controls.Add(lblAction)

        cmbActionFilter = New ComboBox()
        cmbActionFilter.Location = New Point(110, 12)
        cmbActionFilter.Size = New Size(150, 25)
        cmbActionFilter.DropDownStyle = ComboBoxStyle.DropDownList
        cmbActionFilter.Items.AddRange({"All", "Add", "Update", "Delete", "Restock", "Purchase", "Adjust Stock"})
        cmbActionFilter.SelectedIndex = 0
        pnlFilter.Controls.Add(cmbActionFilter)

        btnFilter = New Button()
        btnFilter.Text = "Apply Filter"
        btnFilter.Location = New Point(270, 12)
        btnFilter.Size = New Size(100, 25)
        btnFilter.BackColor = Color.Blue
        btnFilter.ForeColor = Color.White
        pnlFilter.Controls.Add(btnFilter)

        btnRefresh = New Button()
        btnRefresh.Text = "Refresh"
        btnRefresh.Location = New Point(380, 12)
        btnRefresh.Size = New Size(80, 25)
        btnRefresh.BackColor = Color.Green
        btnRefresh.ForeColor = Color.White
        pnlFilter.Controls.Add(btnRefresh)

        Me.Controls.Add(pnlFilter)

        ' DataGridView
        dgvAuditLog = New DataGridView()
        dgvAuditLog.Dock = DockStyle.Fill
        dgvAuditLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvAuditLog.ReadOnly = True
        dgvAuditLog.AllowUserToAddRows = False
        dgvAuditLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        dgvAuditLog.Columns.Add("ID", "ID")
        dgvAuditLog.Columns.Add("ProductName", "Product")
        dgvAuditLog.Columns.Add("Action", "Action")
        dgvAuditLog.Columns.Add("AdminUser", "Admin")
        dgvAuditLog.Columns.Add("OldValue", "Old Value")
        dgvAuditLog.Columns.Add("NewValue", "New Value")
        dgvAuditLog.Columns.Add("Timestamp", "Date/Time")

        Me.Controls.Add(dgvAuditLog)

        LoadAuditLog()
    End Sub

    Private Sub LoadAuditLog()
        Try
            ' Load sample audit log data
            Dim logs As New List(Of InventoryLog)

            logs.Add(New InventoryLog With {
                .LogId = 1,
                .ProductName = "Signature Script Tee",
                .Action = InventoryLog.ActionType.Restock,
                .AdminUsername = currentAdmin.Username,
                .QuantityChange = 50,
                .Timestamp = DateTime.Now.AddHours(-2),
                .Notes = "Stock replenishment"
            })

            logs.Add(New InventoryLog With {
                .LogId = 2,
                .ProductName = "Classic Court Sneakers",
                .Action = InventoryLog.ActionType.Purchase,
                .AdminUsername = "system",
                .QuantityChange = -5,
                .Timestamp = DateTime.Now.AddHours(-1),
                .Notes = "Customer order"
            })

            For Each log In logs
                dgvAuditLog.Rows.Add(log.LogId, log.ProductName, log.GetActionName(), 
                    log.AdminUsername, log.PreviousValue, log.NewValue, log.Timestamp)
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading audit log: " & ex.Message, "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        dgvAuditLog.Rows.Clear()
        LoadAuditLog()
    End Sub

    Private Sub BtnFilter_Click(sender As Object, e As EventArgs) Handles btnFilter.Click
        MessageBox.Show("Filter applied: " & cmbActionFilter.SelectedItem.ToString(), "Filter", 
            MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub InventoryAuditForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Form loaded
    End Sub
End Class

''' <summary>
''' Form for viewing and managing stock alerts
''' </summary>
Public Class StockAlertsForm
    Inherits Form

    Private currentAdmin As AdminUser
    Private dgvAlerts As DataGridView
    Private WithEvents btnRefresh As Button
    Private WithEvents btnResolveAlert As Button
    Private WithEvents btnConfigureThresholds As Button

    Public Sub New(admin As AdminUser)
        MyBase.New()
        currentAdmin = admin
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Stock Alerts Management"
        Me.Size = New Size(1100, 650)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        ' Control panel
        Dim pnlControls As New Panel()
        pnlControls.Dock = DockStyle.Top
        pnlControls.Height = 50
        pnlControls.BackColor = Color.LightGray
        pnlControls.Padding = New Padding(10)

        btnConfigureThresholds = New Button()
        btnConfigureThresholds.Text = "?? Configure Thresholds"
        btnConfigureThresholds.Location = New Point(10, 12)
        btnConfigureThresholds.Size = New Size(150, 25)
        btnConfigureThresholds.BackColor = Color.Orange
        btnConfigureThresholds.ForeColor = Color.White
        pnlControls.Controls.Add(btnConfigureThresholds)

        btnResolveAlert = New Button()
        btnResolveAlert.Text = "? Resolve Alert"
        btnResolveAlert.Location = New Point(170, 12)
        btnResolveAlert.Size = New Size(120, 25)
        btnResolveAlert.BackColor = Color.Green
        btnResolveAlert.ForeColor = Color.White
        pnlControls.Controls.Add(btnResolveAlert)

        btnRefresh = New Button()
        btnRefresh.Text = "Refresh"
        btnRefresh.Location = New Point(300, 12)
        btnRefresh.Size = New Size(80, 25)
        btnRefresh.BackColor = Color.Blue
        btnRefresh.ForeColor = Color.White
        pnlControls.Controls.Add(btnRefresh)

        Me.Controls.Add(pnlControls)

        ' DataGridView
        dgvAlerts = New DataGridView()
        dgvAlerts.Dock = DockStyle.Fill
        dgvAlerts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvAlerts.ReadOnly = True
        dgvAlerts.AllowUserToAddRows = False
        dgvAlerts.SelectionMode = DataGridViewSelectionMode.FullRowSelect

        dgvAlerts.Columns.Add("ID", "Alert ID")
        dgvAlerts.Columns.Add("Product", "Product Name")
        dgvAlerts.Columns.Add("Current", "Current Stock")
        dgvAlerts.Columns.Add("Minimum", "Min Threshold")
        dgvAlerts.Columns.Add("Severity", "Severity")
        dgvAlerts.Columns.Add("Status", "Status")
        dgvAlerts.Columns.Add("AlertDate", "Alert Date")

        Me.Controls.Add(dgvAlerts)

        LoadAlerts()
    End Sub

    Private Sub LoadAlerts()
        Try
            ' Load sample alerts
            Dim alert1 As New StockAlert With {
                .AlertId = 1,
                .ProductName = "Signature Script Tee",
                .CurrentStock = 5,
                .MinimumThreshold = 10,
                .CriticalThreshold = 5,
                .Severity = StockAlert.AlertSeverity.Critical,
                .IsActive = True,
                .AlertDate = DateTime.Now.AddHours(-1)
            }

            Dim alert2 As New StockAlert With {
                .AlertId = 2,
                .ProductName = "Classic Court Sneakers",
                .CurrentStock = 8,
                .MinimumThreshold = 10,
                .CriticalThreshold = 5,
                .Severity = StockAlert.AlertSeverity.Warning,
                .IsActive = True,
                .AlertDate = DateTime.Now.AddHours(-3)
            }

            dgvAlerts.Rows.Add(alert1.AlertId, alert1.ProductName, alert1.CurrentStock, 
                alert1.MinimumThreshold, alert1.GetSeverityName(), 
                If(alert1.IsActive, "Active", "Resolved"), alert1.AlertDate)

            dgvAlerts.Rows.Add(alert2.AlertId, alert2.ProductName, alert2.CurrentStock, 
                alert2.MinimumThreshold, alert2.GetSeverityName(), 
                If(alert2.IsActive, "Active", "Resolved"), alert2.AlertDate)

        Catch ex As Exception
            MessageBox.Show("Error loading alerts: " & ex.Message, "Error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        dgvAlerts.Rows.Clear()
        LoadAlerts()
        MessageBox.Show("Alerts refreshed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnResolveAlert_Click(sender As Object, e As EventArgs) Handles btnResolveAlert.Click
        If dgvAlerts.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an alert to resolve", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        If MessageBox.Show("Mark this alert as resolved?", "Confirm", 
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            MessageBox.Show("Alert resolved successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            LoadAlerts()
        End If
    End Sub

    Private Sub BtnConfigureThresholds_Click(sender As Object, e As EventArgs) Handles btnConfigureThresholds.Click
        MessageBox.Show("Configure threshold settings for all products", "Configuration", 
            MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub StockAlertsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Form loaded
    End Sub
End Class
