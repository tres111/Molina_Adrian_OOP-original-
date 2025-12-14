<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DeliveryDetailsForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.pContainer = New System.Windows.Forms.Panel()
        Me.pLeft = New System.Windows.Forms.Panel()
        Me.lblCustomerHeader = New System.Windows.Forms.Label()
        Me.txtFullName = New System.Windows.Forms.TextBox()
        Me.txtMobile = New System.Windows.Forms.TextBox()
        Me.txtEmail = New System.Windows.Forms.TextBox()
        Me.lblAddressHeader = New System.Windows.Forms.Label()
        Me.txtStreet = New System.Windows.Forms.TextBox()
        Me.txtBarangay = New System.Windows.Forms.TextBox()
        Me.txtCity = New System.Windows.Forms.TextBox()
        Me.txtProvince = New System.Windows.Forms.TextBox()
        Me.txtPostal = New System.Windows.Forms.TextBox()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.pRight = New System.Windows.Forms.Panel()
        Me.lblOrderSummary = New System.Windows.Forms.Label()
        Me.lvOrder = New System.Windows.Forms.ListView()
        Me.chProduct = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chQty = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chPrice = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblSubtotal = New System.Windows.Forms.Label()
        Me.lblDeliveryFee = New System.Windows.Forms.Label()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.cmbDeliveryMethod = New System.Windows.Forms.ComboBox()
        Me.cmbPaymentMethod = New System.Windows.Forms.ComboBox()
        Me.btnPlaceOrder = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        ' pContainer properties - fill the form so content is visible
        Me.pContainer.BackColor = System.Drawing.Color.WhiteSmoke
        Me.pContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pContainer.Name = "pContainer"
        Me.pContainer.TabIndex = 0

        ' pLeft properties - main input column
        Me.pLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.pLeft.Width = 620
        Me.pLeft.Name = "pLeft"
        Me.pLeft.Padding = New System.Windows.Forms.Padding(20)
        Me.pLeft.TabIndex = 1
        Me.pLeft.AutoScroll = True

        ' pRight properties - order summary column
        Me.pRight.Dock = System.Windows.Forms.DockStyle.Right
        Me.pRight.Width = 320
        Me.pRight.Name = "pRight"
        Me.pRight.Padding = New System.Windows.Forms.Padding(12)
        Me.pRight.TabIndex = 2
        Me.pRight.BackColor = System.Drawing.Color.White
        Me.pRight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        '
        ' txtFullName
        Me.txtFullName.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtFullName.Location = New System.Drawing.Point(12, 40)
        Me.txtFullName.Name = "txtFullName"
        Me.txtFullName.Size = New System.Drawing.Size(576, 29)
        Me.txtFullName.TabIndex = 0
        Me.txtFullName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtFullName.MaxLength = 100
        Me.txtFullName.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        
        ' lblFullName
        Me.lblFullName = New System.Windows.Forms.Label()
        Me.lblFullName.AutoSize = True
        Me.lblFullName.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblFullName.Location = New System.Drawing.Point(12, 18)
        Me.lblFullName.Name = "lblFullName"
        Me.lblFullName.Text = "Full Name"
        '
        ' Label: Mobile
        Me.lblMobile = New System.Windows.Forms.Label()
        Me.lblMobile.AutoSize = True
        Me.lblMobile.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblMobile.Location = New System.Drawing.Point(12, 78)
        Me.lblMobile.Name = "lblMobile"
        Me.lblMobile.Text = "Mobile Number"

        'txtMobile
        Me.txtMobile.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtMobile.Location = New System.Drawing.Point(12, 100)
        Me.txtMobile.Name = "txtMobile"
        Me.txtMobile.Size = New System.Drawing.Size(270, 29)
        Me.txtMobile.TabIndex = 1
        Me.txtMobile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtMobile.MaxLength = 15
        Me.txtMobile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        '
        ' Label: Email
        Me.lblEmail = New System.Windows.Forms.Label()
        Me.lblEmail.AutoSize = True
        Me.lblEmail.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblEmail.Location = New System.Drawing.Point(300, 78)
        Me.lblEmail.Name = "lblEmail"
        Me.lblEmail.Text = "Email Address"

        'txtEmail
        Me.txtEmail.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtEmail.Location = New System.Drawing.Point(300, 100)
        Me.txtEmail.Name = "txtEmail"
        Me.txtEmail.Size = New System.Drawing.Size(288, 29)
        Me.txtEmail.TabIndex = 2
        Me.txtEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtEmail.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)
        ' Address header
        Me.lblAddressHeader.AutoSize = True
        Me.lblAddressHeader.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblAddressHeader.Location = New System.Drawing.Point(12, 140)
        Me.lblAddressHeader.Name = "lblAddressHeader"
        Me.lblAddressHeader.Size = New System.Drawing.Size(140, 21)
        Me.lblAddressHeader.Text = "Delivery Address"

        ' Label: Street
        Me.lblStreet = New System.Windows.Forms.Label()
        Me.lblStreet.AutoSize = True
        Me.lblStreet.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblStreet.Location = New System.Drawing.Point(12, 168)
        Me.lblStreet.Name = "lblStreet"
        Me.lblStreet.Text = "Street Address"

        ' txtStreet
        Me.txtStreet.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtStreet.Location = New System.Drawing.Point(12, 188)
        Me.txtStreet.Name = "txtStreet"
        Me.txtStreet.Size = New System.Drawing.Size(576, 29)
        Me.txtStreet.TabIndex = 3
        Me.txtStreet.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtStreet.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right, System.Windows.Forms.AnchorStyles)

        ' Label: Barangay
        Me.lblBarangay = New System.Windows.Forms.Label()
        Me.lblBarangay.AutoSize = True
        Me.lblBarangay.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblBarangay.Location = New System.Drawing.Point(12, 228)
        Me.lblBarangay.Name = "lblBarangay"
        Me.lblBarangay.Text = "Barangay"

        ' txtBarangay
        Me.txtBarangay.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtBarangay.Location = New System.Drawing.Point(12, 248)
        Me.txtBarangay.Name = "txtBarangay"
        Me.txtBarangay.Size = New System.Drawing.Size(270, 29)
        Me.txtBarangay.TabIndex = 4
        Me.txtBarangay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle

        ' Label: City
        Me.lblCity = New System.Windows.Forms.Label()
        Me.lblCity.AutoSize = True
        Me.lblCity.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblCity.Location = New System.Drawing.Point(300, 228)
        Me.lblCity.Name = "lblCity"
        Me.lblCity.Text = "City"

        ' txtCity
        Me.txtCity.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtCity.Location = New System.Drawing.Point(300, 248)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(288, 29)
        Me.txtCity.TabIndex = 5
        Me.txtCity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle

        ' Label: Province
        Me.lblProvince = New System.Windows.Forms.Label()
        Me.lblProvince.AutoSize = True
        Me.lblProvince.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblProvince.Location = New System.Drawing.Point(12, 288)
        Me.lblProvince.Name = "lblProvince"
        Me.lblProvince.Text = "Province"

        ' txtProvince
        Me.txtProvince.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtProvince.Location = New System.Drawing.Point(12, 308)
        Me.txtProvince.Name = "txtProvince"
        Me.txtProvince.Size = New System.Drawing.Size(270, 29)
        Me.txtProvince.TabIndex = 6
        Me.txtProvince.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle

        ' Label: Postal
        Me.lblPostal = New System.Windows.Forms.Label()
        Me.lblPostal.AutoSize = True
        Me.lblPostal.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblPostal.Location = New System.Drawing.Point(300, 288)
        Me.lblPostal.Name = "lblPostal"
        Me.lblPostal.Text = "Postal Code"

        ' txtPostal
        Me.txtPostal.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtPostal.Location = New System.Drawing.Point(300, 308)
        Me.txtPostal.Name = "txtPostal"
        Me.txtPostal.Size = New System.Drawing.Size(120, 29)
        Me.txtPostal.TabIndex = 7
        Me.txtPostal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtPostal.MaxLength = 10

        ' Label: Notes
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblNotes.Location = New System.Drawing.Point(12, 350)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Text = "Notes / Landmark"
        '
        'cmbDeliveryMethod
        '
        ' cmbDeliveryMethod
        Me.cmbDeliveryMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbDeliveryMethod.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.cmbDeliveryMethod.Location = New System.Drawing.Point(12, 330)
        Me.cmbDeliveryMethod.Name = "cmbDeliveryMethod"
        Me.cmbDeliveryMethod.Size = New System.Drawing.Size(270, 29)
        Me.cmbDeliveryMethod.TabIndex = 8
        Me.cmbDeliveryMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat

        ' cmbPaymentMethod
        Me.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPaymentMethod.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.cmbPaymentMethod.Location = New System.Drawing.Point(300, 330)
        Me.cmbPaymentMethod.Name = "cmbPaymentMethod"
        Me.cmbPaymentMethod.Size = New System.Drawing.Size(288, 29)
        Me.cmbPaymentMethod.TabIndex = 9
        Me.cmbPaymentMethod.FlatStyle = System.Windows.Forms.FlatStyle.Flat

        ' txtNotes
        Me.txtNotes.Font = New System.Drawing.Font("Segoe UI", 12.0!)
        Me.txtNotes.Location = New System.Drawing.Point(12, 372)
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.Size = New System.Drawing.Size(576, 120)
        Me.txtNotes.TabIndex = 10
        Me.txtNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.txtNotes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)

        ' pRight
        Me.pRight.Location = New System.Drawing.Point(600, 20)
        Me.pRight.Name = "pRight"
        Me.pRight.Size = New System.Drawing.Size(300, 560)
        Me.pRight.TabIndex = 2

        ' lblOrderSummary
        Me.lblOrderSummary.AutoSize = True
        Me.lblOrderSummary.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblOrderSummary.Location = New System.Drawing.Point(8, 8)
        Me.lblOrderSummary.Name = "lblOrderSummary"
        Me.lblOrderSummary.Size = New System.Drawing.Size(140, 25)
        Me.lblOrderSummary.Text = "Order Summary"

        ' subtotal label title
        Me.lblSubtotalTitle = New System.Windows.Forms.Label()
        Me.lblSubtotalTitle.AutoSize = True
        Me.lblSubtotalTitle.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblSubtotalTitle.Location = New System.Drawing.Point(12, 400)
        Me.lblSubtotalTitle.Text = "Subtotal"

        ' delivery fee title
        Me.lblDeliveryFeeTitle = New System.Windows.Forms.Label()
        Me.lblDeliveryFeeTitle.AutoSize = True
        Me.lblDeliveryFeeTitle.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblDeliveryFeeTitle.Location = New System.Drawing.Point(12, 430)
        Me.lblDeliveryFeeTitle.Text = "Delivery Fee"

        ' grand total title
        Me.lblGrandTotalTitle = New System.Windows.Forms.Label()
        Me.lblGrandTotalTitle.AutoSize = True
        Me.lblGrandTotalTitle.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.lblGrandTotalTitle.Location = New System.Drawing.Point(12, 460)
        Me.lblGrandTotalTitle.Text = "Grand Total"

        ' lvOrder
        Me.lvOrder.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chProduct, Me.chQty, Me.chPrice})
        Me.lvOrder.GridLines = True
        Me.lvOrder.FullRowSelect = True
        Me.lvOrder.HideSelection = False
        Me.lvOrder.Location = New System.Drawing.Point(8, 40)
        Me.lvOrder.Name = "lvOrder"
        Me.lvOrder.Size = New System.Drawing.Size(288, 340)
        Me.lvOrder.TabIndex = 0
        Me.lvOrder.UseCompatibleStateImageBehavior = False
        Me.lvOrder.View = System.Windows.Forms.View.Details
        Me.lvOrder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)

        Me.chProduct.Text = "Product"
        Me.chProduct.Width = 120
        Me.chQty.Text = "Qty"
        Me.chQty.Width = 40
        Me.chPrice.Text = "Price"
        Me.chPrice.Width = 60

        ' lblSubtotal
        Me.lblSubtotal.AutoSize = True
        Me.lblSubtotal.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblSubtotal.Location = New System.Drawing.Point(220, 400)
        Me.lblSubtotal.Name = "lblSubtotal"
        Me.lblSubtotal.Size = New System.Drawing.Size(60, 19)
        Me.lblSubtotal.Text = "?0.00"
        Me.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.TopRight

        ' lblDeliveryFee
        Me.lblDeliveryFee.AutoSize = True
        Me.lblDeliveryFee.Font = New System.Drawing.Font("Segoe UI", 10.0!)
        Me.lblDeliveryFee.Location = New System.Drawing.Point(220, 430)
        Me.lblDeliveryFee.Name = "lblDeliveryFee"
        Me.lblDeliveryFee.Size = New System.Drawing.Size(60, 19)
        Me.lblDeliveryFee.Text = "?0.00"
        Me.lblDeliveryFee.TextAlign = System.Drawing.ContentAlignment.TopRight

        ' lblTotal
        Me.lblTotal.AutoSize = True
        Me.lblTotal.Font = New System.Drawing.Font("Segoe UI", 14.0!, System.Drawing.FontStyle.Bold)
        Me.lblTotal.Location = New System.Drawing.Point(180, 460)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(100, 25)
        Me.lblTotal.Text = "?0.00"
        Me.lblTotal.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'btnPlaceOrder
        '
        Me.btnPlaceOrder.Name = "btnPlaceOrder"
        Me.btnPlaceOrder.TabIndex = 10
        Me.btnPlaceOrder.Text = "Place Order"
        Me.btnPlaceOrder.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.TabIndex = 11
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'DeliveryDetailsForm
        '
        Me.ClientSize = New System.Drawing.Size(920, 700)
        Me.Controls.Add(Me.pContainer)
        Me.pContainer.Controls.Add(Me.pLeft)
        Me.pLeft.Controls.Add(Me.lblCustomerHeader)
        Me.pLeft.Controls.Add(Me.lblFullName)
        Me.pLeft.Controls.Add(Me.txtFullName)
        Me.pLeft.Controls.Add(Me.lblMobile)
        Me.pLeft.Controls.Add(Me.txtMobile)
        Me.pLeft.Controls.Add(Me.lblEmail)
        Me.pLeft.Controls.Add(Me.txtEmail)
        Me.pLeft.Controls.Add(Me.lblAddressHeader)
        Me.pLeft.Controls.Add(Me.lblStreet)
        Me.pLeft.Controls.Add(Me.txtStreet)
        Me.pLeft.Controls.Add(Me.lblBarangay)
        Me.pLeft.Controls.Add(Me.txtBarangay)
        Me.pLeft.Controls.Add(Me.lblCity)
        Me.pLeft.Controls.Add(Me.txtCity)
        Me.pLeft.Controls.Add(Me.lblProvince)
        Me.pLeft.Controls.Add(Me.txtProvince)
        Me.pLeft.Controls.Add(Me.lblPostal)
        Me.pLeft.Controls.Add(Me.txtPostal)
        Me.pLeft.Controls.Add(Me.cmbDeliveryMethod)
        Me.pLeft.Controls.Add(Me.cmbPaymentMethod)
        Me.pLeft.Controls.Add(Me.lblNotes)
        Me.pLeft.Controls.Add(Me.txtNotes)
        Me.pContainer.Controls.Add(Me.pRight)
        Me.pRight.Controls.Add(Me.lblOrderSummary)
        Me.pRight.Controls.Add(Me.lvOrder)
        Me.pRight.Controls.Add(Me.lblSubtotalTitle)
        Me.pRight.Controls.Add(Me.lblSubtotal)
        Me.pRight.Controls.Add(Me.lblDeliveryFeeTitle)
        Me.pRight.Controls.Add(Me.lblDeliveryFee)
        Me.pRight.Controls.Add(Me.lblGrandTotalTitle)
        Me.pRight.Controls.Add(Me.lblTotal)
        ' Buttons
        Me.btnPlaceOrder.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.btnPlaceOrder.BackColor = System.Drawing.Color.FromArgb(0, 120, 215)
        Me.btnPlaceOrder.ForeColor = System.Drawing.Color.White
        Me.btnPlaceOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPlaceOrder.FlatAppearance.BorderSize = 0
        Me.btnPlaceOrder.Size = New System.Drawing.Size(220, 44)
        Me.btnPlaceOrder.Location = New System.Drawing.Point(600, 520)

        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.btnCancel.BackColor = System.Drawing.Color.White
        Me.btnCancel.ForeColor = System.Drawing.Color.Black
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.FlatAppearance.BorderSize = 1
        Me.btnCancel.Size = New System.Drawing.Size(120, 36)
        Me.btnCancel.Location = New System.Drawing.Point(600, 570)

        ' place buttons inside right panel with modern sizing
        Me.btnPlaceOrder.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.btnPlaceOrder.BackColor = System.Drawing.Color.FromArgb(0, 120, 215)
        Me.btnPlaceOrder.ForeColor = System.Drawing.Color.White
        Me.btnPlaceOrder.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPlaceOrder.FlatAppearance.BorderSize = 0
        Me.btnPlaceOrder.Size = New System.Drawing.Size(268, 44)
        Me.btnPlaceOrder.Location = New System.Drawing.Point(16, 520)

        Me.btnCancel.Font = New System.Drawing.Font("Segoe UI", 11.0!)
        Me.btnCancel.BackColor = System.Drawing.Color.White
        Me.btnCancel.ForeColor = System.Drawing.Color.Black
        Me.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCancel.FlatAppearance.BorderSize = 1
        Me.btnCancel.Size = New System.Drawing.Size(128, 36)
        Me.btnCancel.Location = New System.Drawing.Point(156, 572)

        ' Confirm button (primary) placed on right panel
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.btnConfirm.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold)
        Me.btnConfirm.BackColor = System.Drawing.Color.FromArgb(0, 120, 215)
        Me.btnConfirm.ForeColor = System.Drawing.Color.White
        Me.btnConfirm.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnConfirm.FlatAppearance.BorderSize = 0
        Me.btnConfirm.Size = New System.Drawing.Size(268, 44)
        Me.btnConfirm.Location = New System.Drawing.Point(16, 520)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.TabIndex = 12
        Me.btnConfirm.Text = "Confirm & Place Order"
        Me.btnConfirm.UseVisualStyleBackColor = True

        Me.pRight.Controls.Add(Me.btnPlaceOrder)
        Me.pRight.Controls.Add(Me.btnConfirm)
        Me.pRight.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DeliveryDetailsForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Delivery & Payment"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    
    Friend WithEvents pContainer As Panel
    Friend WithEvents pLeft As Panel
    Friend WithEvents lblCustomerHeader As Label
    Friend WithEvents txtFullName As TextBox
    Friend WithEvents txtMobile As TextBox
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents lblAddressHeader As Label
    Friend WithEvents txtStreet As TextBox
    Friend WithEvents txtBarangay As TextBox
    Friend WithEvents txtCity As TextBox
    Friend WithEvents txtProvince As TextBox
    Friend WithEvents txtPostal As TextBox
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents pRight As Panel
    Friend WithEvents lblOrderSummary As Label
    Friend WithEvents lvOrder As ListView
    Friend WithEvents chProduct As ColumnHeader
    Friend WithEvents chQty As ColumnHeader
    Friend WithEvents chPrice As ColumnHeader
    Friend WithEvents lblSubtotal As Label
    Friend WithEvents lblDeliveryFee As Label
    Friend WithEvents lblTotal As Label
    Friend WithEvents cmbDeliveryMethod As ComboBox
    Friend WithEvents cmbPaymentMethod As ComboBox
    Friend WithEvents btnPlaceOrder As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnConfirm As Button
    Friend WithEvents lblFullName As Label
    Friend WithEvents lblMobile As Label
    Friend WithEvents lblEmail As Label
    Friend WithEvents lblStreet As Label
    Friend WithEvents lblBarangay As Label
    Friend WithEvents lblCity As Label
    Friend WithEvents lblProvince As Label
    Friend WithEvents lblPostal As Label
    Friend WithEvents lblNotes As Label
    Friend WithEvents lblSubtotalTitle As Label
    Friend WithEvents lblDeliveryFeeTitle As Label
    Friend WithEvents lblGrandTotalTitle As Label
End Class
