Public Class UserProfileForm
Inherits Form

Private userId As Integer

Private Sub UserProfileForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "User Profile - Coziest Store"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(700, 600)
        Me.BackColor = Color.White

        userId = DBAuthentication.GetCurrentUserId()

        If userId = 0 Then
            MessageBox.Show("Please login first.", "Not Logged In", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
            Return
        End If

        CreateUI()
        LoadUserProfile()
    End Sub

    Private Sub CreateUI()
        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "User Profile"
        lblTitle.Font = New Font("Arial", 18, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        Me.Controls.Add(lblTitle)

        ' Username Label
        Dim lblUsername As New Label()
        lblUsername.Text = "Username:"
        lblUsername.Font = New Font("Arial", 10)
        lblUsername.Location = New Point(20, 70)
        lblUsername.AutoSize = True
        Me.Controls.Add(lblUsername)

        Dim txtUsername As New TextBox()
        txtUsername.Name = "txtUsername"
        txtUsername.Location = New Point(150, 70)
        txtUsername.Size = New Size(500, 30)
        txtUsername.Font = New Font("Arial", 10)
        txtUsername.ReadOnly = True
        txtUsername.BackColor = Color.LightGray
        Me.Controls.Add(txtUsername)

        ' Email Label
        Dim lblEmail As New Label()
        lblEmail.Text = "Email:"
        lblEmail.Font = New Font("Arial", 10)
        lblEmail.Location = New Point(20, 115)
        lblEmail.AutoSize = True
        Me.Controls.Add(lblEmail)

        Dim txtEmail As New TextBox()
        txtEmail.Name = "txtEmail"
        txtEmail.Location = New Point(150, 115)
        txtEmail.Size = New Size(500, 30)
        txtEmail.Font = New Font("Arial", 10)
        txtEmail.ReadOnly = True
        txtEmail.BackColor = Color.LightGray
        Me.Controls.Add(txtEmail)

        ' Full Name Label
        Dim lblFullName As New Label()
        lblFullName.Text = "Full Name:"
        lblFullName.Font = New Font("Arial", 10)
        lblFullName.Location = New Point(20, 160)
        lblFullName.AutoSize = True
        Me.Controls.Add(lblFullName)

        Dim txtFullName As New TextBox()
        txtFullName.Name = "txtFullName"
        txtFullName.Location = New Point(150, 160)
        txtFullName.Size = New Size(500, 30)
        txtFullName.Font = New Font("Arial", 10)
        Me.Controls.Add(txtFullName)

        ' Phone Label
        Dim lblPhone As New Label()
        lblPhone.Text = "Phone:"
        lblPhone.Font = New Font("Arial", 10)
        lblPhone.Location = New Point(20, 205)
        lblPhone.AutoSize = True
        Me.Controls.Add(lblPhone)

        Dim txtPhone As New TextBox()
        txtPhone.Name = "txtPhone"
        txtPhone.Location = New Point(150, 205)
        txtPhone.Size = New Size(500, 30)
        txtPhone.Font = New Font("Arial", 10)
        Me.Controls.Add(txtPhone)

        ' Address Label
        Dim lblAddress As New Label()
        lblAddress.Text = "Address:"
        lblAddress.Font = New Font("Arial", 10)
        lblAddress.Location = New Point(20, 250)
        lblAddress.AutoSize = True
        Me.Controls.Add(lblAddress)

        Dim txtAddress As New TextBox()
        txtAddress.Name = "txtAddress"
        txtAddress.Location = New Point(20, 270)
        txtAddress.Size = New Size(630, 80)
        txtAddress.Font = New Font("Arial", 10)
        txtAddress.Multiline = True
        Me.Controls.Add(txtAddress)

        ' Member Since Label
        Dim lblMemberSince As New Label()
        lblMemberSince.Name = "lblMemberSince"
        lblMemberSince.Text = "Member Since: N/A"
        lblMemberSince.Font = New Font("Arial", 9)
        lblMemberSince.ForeColor = Color.Gray
        lblMemberSince.Location = New Point(20, 365)
        lblMemberSince.AutoSize = True
        Me.Controls.Add(lblMemberSince)

        ' Update Button
        Dim btnUpdate As New Button()
        btnUpdate.Name = "btnUpdate"
        btnUpdate.Text = "UPDATE PROFILE"
        btnUpdate.Location = New Point(20, 410)
        btnUpdate.Size = New Size(200, 40)
        btnUpdate.Font = New Font("Arial", 10, FontStyle.Bold)
        btnUpdate.BackColor = Color.FromArgb(70, 130, 180)
        btnUpdate.ForeColor = Color.White
        btnUpdate.FlatStyle = FlatStyle.Flat
        btnUpdate.Cursor = Cursors.Hand
        AddHandler btnUpdate.Click, AddressOf BtnUpdate_Click
        Me.Controls.Add(btnUpdate)

        ' View Orders Button
        Dim btnOrders As New Button()
        btnOrders.Name = "btnOrders"
        btnOrders.Text = "VIEW ORDERS"
        btnOrders.Location = New Point(230, 410)
        btnOrders.Size = New Size(200, 40)
        btnOrders.Font = New Font("Arial", 10, FontStyle.Bold)
        btnOrders.BackColor = Color.FromArgb(100, 160, 200)
        btnOrders.ForeColor = Color.White
        btnOrders.FlatStyle = FlatStyle.Flat
        btnOrders.Cursor = Cursors.Hand
        AddHandler btnOrders.Click, AddressOf BtnOrders_Click
        Me.Controls.Add(btnOrders)

        ' View Wishlist Button
        Dim btnWishlist As New Button()
        btnWishlist.Name = "btnWishlist"
        btnWishlist.Text = "MY WISHLIST"
        btnWishlist.Location = New Point(440, 410)
        btnWishlist.Size = New Size(210, 40)
        btnWishlist.Font = New Font("Arial", 10, FontStyle.Bold)
        btnWishlist.BackColor = Color.FromArgb(255, 193, 7)
        btnWishlist.ForeColor = Color.Black
        btnWishlist.FlatStyle = FlatStyle.Flat
        btnWishlist.Cursor = Cursors.Hand
        AddHandler btnWishlist.Click, AddressOf BtnWishlist_Click
        Me.Controls.Add(btnWishlist)

        ' Logout Button
        Dim btnLogout As New Button()
        btnLogout.Name = "btnLogout"
        btnLogout.Text = "LOGOUT"
        btnLogout.Location = New Point(20, 470)
        btnLogout.Size = New Size(630, 40)
        btnLogout.Font = New Font("Arial", 10, FontStyle.Bold)
        btnLogout.BackColor = Color.FromArgb(220, 53, 69)
        btnLogout.ForeColor = Color.White
        btnLogout.FlatStyle = FlatStyle.Flat
        btnLogout.Cursor = Cursors.Hand
        AddHandler btnLogout.Click, AddressOf BtnLogout_Click
        Me.Controls.Add(btnLogout)

        ' Close Button
        Dim btnClose As New Button()
        btnClose.Name = "btnClose"
        btnClose.Text = "CLOSE"
        btnClose.Location = New Point(20, 520)
        btnClose.Size = New Size(630, 40)
        btnClose.Font = New Font("Arial", 10, FontStyle.Bold)
        btnClose.BackColor = Color.FromArgb(108, 117, 125)
        btnClose.ForeColor = Color.White
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Cursor = Cursors.Hand
        AddHandler btnClose.Click, AddressOf BtnClose_Click
        Me.Controls.Add(btnClose)
    End Sub

    Private Sub LoadUserProfile()
        Dim userProfile = DBAuthentication.GetUserProfile(userId)

        If userProfile IsNot Nothing Then
            Dim txtUsername As TextBox = Me.Controls("txtUsername")
            Dim txtEmail As TextBox = Me.Controls("txtEmail")
            Dim txtFullName As TextBox = Me.Controls("txtFullName")
            Dim txtPhone As TextBox = Me.Controls("txtPhone")
            Dim txtAddress As TextBox = Me.Controls("txtAddress")
            Dim lblMemberSince As Label = Me.Controls("lblMemberSince")

            txtUsername.Text = userProfile("username").ToString()
            txtEmail.Text = userProfile("email").ToString()
            txtFullName.Text = If(IsDBNull(userProfile("full_name")), "", userProfile("full_name").ToString())
            txtPhone.Text = If(IsDBNull(userProfile("phone")), "", userProfile("phone").ToString())
            txtAddress.Text = If(IsDBNull(userProfile("address")), "", userProfile("address").ToString())

            Dim createdAt As DateTime = CDate(userProfile("created_at"))
            lblMemberSince.Text = "Member Since: " & createdAt.ToString("MMMM dd, yyyy")
        End If
    End Sub

    Private Sub BtnUpdate_Click(sender As Object, e As EventArgs)
        Dim txtFullName As TextBox = Me.Controls("txtFullName")
        Dim txtPhone As TextBox = Me.Controls("txtPhone")
        Dim txtAddress As TextBox = Me.Controls("txtAddress")

        Dim result = DBAuthentication.UpdateUserProfile(userId, txtFullName.Text, txtPhone.Text, txtAddress.Text)

        If result.Item1 Then
            MessageBox.Show(result.Item2, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Else
            MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnOrders_Click(sender As Object, e As EventArgs)
        Dim orderForm As New OrderHistoryForm()
        orderForm.ShowDialog(Me)
    End Sub

    Private Sub BtnWishlist_Click(sender As Object, e As EventArgs)
        Dim wishlistForm As New WishlistForm()
        wishlistForm.ShowDialog(Me)
    End Sub

    Private Sub BtnLogout_Click(sender As Object, e As EventArgs)
        If MessageBox.Show("Are you sure you want to logout?", "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            DBAuthentication.LogoutUser()
            MessageBox.Show("Logged out successfully.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Dim loginForm As New LoginForm()
            loginForm.Show()
            Me.Close()
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
