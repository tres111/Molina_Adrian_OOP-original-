Partial Public Class DeliveryDetailsForm
    Public Property FullName As String
    Public Property Mobile As String
    Public Property Address As String
    Public Property Email As String
    Public Property DeliveryMethod As String
    Public Property PaymentMethod As String
    Public Property Notes As String
    Public Property DeliveryFee As Decimal
    Public Property Total As Decimal

    Private orderSubtotal As Decimal
    Private orderItems As List(Of OrderReviewForm.OrderItem)

    Public Sub New(items As List(Of OrderReviewForm.OrderItem), subtotal As Decimal)
        InitializeComponent()
        orderItems = items
        orderSubtotal = subtotal
        ' initialize controls (combo boxes, handlers)
        If cmbDeliveryMethod IsNot Nothing Then
            cmbDeliveryMethod.Items.Clear()
            cmbDeliveryMethod.Items.AddRange(New String() {"Courier", "Pickup"})
            If cmbDeliveryMethod.Items.Count > 0 Then cmbDeliveryMethod.SelectedIndex = 0
            AddHandler cmbDeliveryMethod.SelectedIndexChanged, AddressOf cmbDeliveryMethod_SelectedIndexChanged
        End If
        If cmbPaymentMethod IsNot Nothing Then
            cmbPaymentMethod.Items.Clear()
            cmbPaymentMethod.Items.AddRange(New String() {"COD", "GCash", "Bank Transfer"})
            If cmbPaymentMethod.Items.Count > 0 Then cmbPaymentMethod.SelectedIndex = 0
        End If
        If btnPlaceOrder IsNot Nothing Then AddHandler btnPlaceOrder.Click, AddressOf btnPlaceOrder_Click
        If btnConfirm IsNot Nothing Then AddHandler btnConfirm.Click, AddressOf btnConfirm_Click
        If btnCancel IsNot Nothing Then AddHandler btnCancel.Click, AddressOf btnCancel_Click
        UpdateTotals()
        ' Populate order summary
        Try
            If orderItems IsNot Nothing AndAlso lvOrder IsNot Nothing Then
                lvOrder.Items.Clear()
                For Each it In orderItems
                    Dim lvi = New ListViewItem(it.ProductName)
                    lvi.SubItems.Add(it.Quantity.ToString())
                    lvi.SubItems.Add(it.UnitPrice.ToString("C2"))
                    lvOrder.Items.Add(lvi)
                Next
            End If
        Catch ex As Exception
            Debug.WriteLine("Populate order summary failed: " & ex.Message)
        End Try

        ' compute subtotal from items (prefer authoritative items list)
        Try
            If orderItems IsNot Nothing Then
                orderSubtotal = orderItems.Sum(Function(i) i.TotalPrice)
            End If
            If lblSubtotal IsNot Nothing Then lblSubtotal.Text = orderSubtotal.ToString("C2")
            UpdateTotals()
        Catch ex As Exception
            Debug.WriteLine("Compute subtotal failed: " & ex.Message)
        End Try

        ' Wire realtime validation
        If txtEmail IsNot Nothing Then AddHandler txtEmail.TextChanged, AddressOf ValidateEmail
        If txtMobile IsNot Nothing Then AddHandler txtMobile.TextChanged, AddressOf ValidateMobile
        If txtFullName IsNot Nothing Then AddHandler txtFullName.TextChanged, AddressOf ValidateName
        If txtMobile IsNot Nothing Then AddHandler txtMobile.KeyPress, AddressOf TxtNumericOnly_KeyPress
        If txtPostal IsNot Nothing Then AddHandler txtPostal.KeyPress, AddressOf TxtNumericOnly_KeyPress
        If txtFullName IsNot Nothing Then AddHandler txtFullName.KeyPress, AddressOf TxtAlphaOnly_KeyPress

        ' Improve ComboBox visual (dropdown arrow visibility)
        Try
            If cmbDeliveryMethod IsNot Nothing Then
                cmbDeliveryMethod.FlatStyle = Windows.Forms.FlatStyle.Standard
            End If
            If cmbPaymentMethod IsNot Nothing Then
                cmbPaymentMethod.FlatStyle = Windows.Forms.FlatStyle.Standard
            End If
        Catch ex As Exception
        End Try

        ' layout buttons full-width on right panel
        Try
            If pRight IsNot Nothing AndAlso btnPlaceOrder IsNot Nothing Then
                btnPlaceOrder.Width = pRight.Width - 12
                btnPlaceOrder.Left = pRight.Left + 6
                btnPlaceOrder.Top = pRight.Bottom - 80
                btnCancel.Left = btnPlaceOrder.Left + btnPlaceOrder.Width - btnCancel.Width
                btnCancel.Top = btnPlaceOrder.Top + btnPlaceOrder.Height + 8
            End If
        Catch ex As Exception
            Debug.WriteLine("Button layout error: " & ex.Message)
        End Try
    End Sub

    Private Sub DeliveryDetailsForm_Load(sender As Object, e As EventArgs)
        ' Left intentionally empty; initialization is done in constructor to avoid designer load ordering issues
    End Sub

    Private animationTimer As Timer
    Private startLeft As Integer
    Private targetLeft As Integer

    Private Sub DeliveryDetailsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        ' Slide-in animation for the container panel
        Try
            If Me.pContainer Is Nothing Then Return
            ' center horizontally
            Me.pContainer.Left = (Me.ClientSize.Width - Me.pContainer.Width) \ 2
            Dim targetTop = (Me.ClientSize.Height - Me.pContainer.Height) \ 2
            Dim startTop = -Me.pContainer.Height
            Me.pContainer.Top = startTop
            Me.pContainer.Visible = True

            animationTimer = New Timer()
            animationTimer.Interval = 12
            Dim duration = 18
            Dim t = 0
            AddHandler animationTimer.Tick, Sub(s, ev)
                                               t += 1
                                               ' ease-out cubic
                                               Dim p = t / duration
                                               If p > 1 Then p = 1
                                               Dim ease = 1 - Math.Pow(1 - p, 3)
                                               Me.pContainer.Top = CInt(startTop + (targetTop - startTop) * ease)
                                               Me.Opacity = Math.Min(1, Me.Opacity + 0.06)
                                               If p >= 1 Then
                                                   animationTimer.Stop()
                                                   animationTimer.Dispose()
                                               End If
                                           End Sub
            Me.Opacity = 0
            animationTimer.Start()
        Catch ex As Exception
            Debug.WriteLine("Animation error: " & ex.Message)
        End Try
    End Sub

    Private Sub UpdateTotals()
        Dim dm As String = If(cmbDeliveryMethod.SelectedItem IsNot Nothing, cmbDeliveryMethod.SelectedItem.ToString(), "Courier")
        DeliveryFee = If(dm = "Courier", 50D, 0D)
        Total = orderSubtotal + DeliveryFee
        If lblDeliveryFee IsNot Nothing Then lblDeliveryFee.Text = DeliveryFee.ToString("C2")
        If lblTotal IsNot Nothing Then lblTotal.Text = Total.ToString("C2")
    End Sub

    Private Sub ValidateEmail(sender As Object, e As EventArgs)
        Try
            If lblEmail Is Nothing Then Return
            Dim txt = txtEmail.Text
            If IsValidEmail(txt) Then
                lblEmail.ForeColor = Drawing.Color.Green
                lblEmail.Text = "Email Address ?"
            Else
                lblEmail.ForeColor = Drawing.Color.Black
                lblEmail.Text = "Email Address"
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ValidateMobile(sender As Object, e As EventArgs)
        Try
            If lblMobile Is Nothing Then Return
            Dim txt = txtMobile.Text
            If IsValidPhone(txt) Then
                lblMobile.ForeColor = Drawing.Color.Green
                lblMobile.Text = "Mobile Number ?"
            Else
                lblMobile.ForeColor = Drawing.Color.Black
                lblMobile.Text = "Mobile Number"
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ValidateName(sender As Object, e As EventArgs)
        Try
            If lblFullName Is Nothing Then Return
            If Not String.IsNullOrWhiteSpace(txtFullName.Text) Then
                lblFullName.ForeColor = Drawing.Color.Green
                lblFullName.Text = "Full Name ?"
            Else
                lblFullName.ForeColor = Drawing.Color.Black
                lblFullName.Text = "Full Name"
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function IsValidEmail(email As String) As Boolean
        Try
            Dim addr = New System.Net.Mail.MailAddress(email)
            Return addr.Address = email
        Catch
            Return False
        End Try
    End Function

    Private Function IsValidPhone(phone As String) As Boolean
        If String.IsNullOrWhiteSpace(phone) Then Return False
        Dim digits = New String(phone.Where(AddressOf Char.IsDigit).ToArray())
        Return digits.Length >= 7 AndAlso digits.Length <= 15
    End Function

    Private Function ValidateInput() As Boolean
        If String.IsNullOrWhiteSpace(txtFullName.Text) Then
            MessageBox.Show("Full name is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If String.IsNullOrWhiteSpace(txtMobile.Text) OrElse txtMobile.Text.Length < 7 Then
            MessageBox.Show("Valid mobile is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If String.IsNullOrWhiteSpace(txtStreet.Text) Then
            MessageBox.Show("Address is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If String.IsNullOrWhiteSpace(txtEmail.Text) OrElse Not txtEmail.Text.Contains("@") Then
            MessageBox.Show("Valid email is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        Return True
    End Function

    Private Sub TxtNumericOnly_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Allow control keys, digits only
        If Char.IsControl(e.KeyChar) Then Return
        If Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True
        End If
    End Sub

    Private Sub TxtAlphaOnly_KeyPress(sender As Object, e As KeyPressEventArgs)
        ' Allow letters, spaces, and basic punctuation for names
        If Char.IsControl(e.KeyChar) Then Return
        If Char.IsLetter(e.KeyChar) OrElse Char.IsWhiteSpace(e.KeyChar) OrElse e.KeyChar = "-"c OrElse e.KeyChar = "'"c Then
            Return
        Else
            e.Handled = True
        End If
    End Sub

    Private Sub cmbDeliveryMethod_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateTotals()
    End Sub

    Private Sub btnPlaceOrder_Click(sender As Object, e As EventArgs)
        If Not ValidateInput() Then Return
        FullName = txtFullName.Text.Trim()
        Mobile = txtMobile.Text.Trim()
        ' compose address from fields
        Dim parts As New System.Text.StringBuilder()
        parts.Append(txtStreet.Text.Trim())
        If Not String.IsNullOrWhiteSpace(txtBarangay.Text) Then parts.Append(", " & txtBarangay.Text.Trim())
        If Not String.IsNullOrWhiteSpace(txtCity.Text) Then parts.Append(", " & txtCity.Text.Trim())
        If Not String.IsNullOrWhiteSpace(txtProvince.Text) Then parts.Append(", " & txtProvince.Text.Trim())
        If Not String.IsNullOrWhiteSpace(txtPostal.Text) Then parts.Append(" " & txtPostal.Text.Trim())
        Address = parts.ToString()
        Email = txtEmail.Text.Trim()
        DeliveryMethod = If(cmbDeliveryMethod.SelectedItem IsNot Nothing, cmbDeliveryMethod.SelectedItem.ToString(), "Courier")
        PaymentMethod = If(cmbPaymentMethod.SelectedItem IsNot Nothing, cmbPaymentMethod.SelectedItem.ToString(), "COD")
        Notes = txtNotes.Text.Trim()
        DeliveryFee = If(DeliveryMethod = "Courier", 50D, 0D)
        Total = orderSubtotal + DeliveryFee
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnConfirm_Click(sender As Object, e As EventArgs)
        ' Confirm: validate input and return collected data to caller (OrderReviewForm)
        Try
            If Not ValidateInput() Then Return
            FullName = txtFullName.Text.Trim()
            Mobile = txtMobile.Text.Trim()
            Dim parts As New System.Text.StringBuilder()
            parts.Append(txtStreet.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtBarangay.Text) Then parts.Append(", " & txtBarangay.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtCity.Text) Then parts.Append(", " & txtCity.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtProvince.Text) Then parts.Append(", " & txtProvince.Text.Trim())
            If Not String.IsNullOrWhiteSpace(txtPostal.Text) Then parts.Append(" " & txtPostal.Text.Trim())
            Address = parts.ToString()
            Email = txtEmail.Text.Trim()
            DeliveryMethod = If(cmbDeliveryMethod.SelectedItem IsNot Nothing, cmbDeliveryMethod.SelectedItem.ToString(), "Courier")
            PaymentMethod = If(cmbPaymentMethod.SelectedItem IsNot Nothing, cmbPaymentMethod.SelectedItem.ToString(), "COD")
            Notes = txtNotes.Text.Trim()
            DeliveryFee = If(DeliveryMethod = "Courier", 50D, 0D)
            Total = orderSubtotal + DeliveryFee

            ' Set dialog result only; actual order creation is handled by the calling form (OrderReviewForm)
            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Failed to confirm delivery details: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs)
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
