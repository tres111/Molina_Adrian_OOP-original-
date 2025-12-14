Public Class ProductReviewsForm
Inherits Form

Private productId As Integer
Private userId As Integer
Private productName As String

Public Sub New(productId As Integer, productName As String)
    MyBase.New()
    Me.productId = productId
    Me.productName = productName
    Me.userId = DBAuthentication.GetCurrentUserId()
End Sub

Private Sub ProductReviewsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Reviews - " & productName & " - Coziest Store"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Size = New Size(800, 700)
        Me.BackColor = Color.White

        CreateUI()
        LoadReviews()
        LoadRatingInfo()
    End Sub

    Private Sub CreateUI()
        ' Title Label
        Dim lblTitle As New Label()
        lblTitle.Text = "Reviews for " & productName
        lblTitle.Font = New Font("Arial", 16, FontStyle.Bold)
        lblTitle.ForeColor = Color.FromArgb(70, 130, 180)
        lblTitle.Location = New Point(20, 20)
        lblTitle.AutoSize = True
        Me.Controls.Add(lblTitle)

        ' Average Rating Label
        Dim lblRating As New Label()
        lblRating.Name = "lblRating"
        lblRating.Text = "Average Rating: 0.0 / 5.0"
        lblRating.Font = New Font("Arial", 11)
        lblRating.ForeColor = Color.FromArgb(70, 130, 180)
        lblRating.Location = New Point(20, 55)
        lblRating.AutoSize = True
        Me.Controls.Add(lblRating)

        ' Review Count Label
        Dim lblCount As New Label()
        lblCount.Name = "lblCount"
        lblCount.Text = "0 reviews"
        lblCount.Font = New Font("Arial", 10)
        lblCount.ForeColor = Color.Gray
        lblCount.Location = New Point(20, 80)
        lblCount.AutoSize = True
        Me.Controls.Add(lblCount)

        ' Your Rating Section
        Dim lblYourRating As New Label()
        lblYourRating.Text = "Your Rating:"
        lblYourRating.Font = New Font("Arial", 10, FontStyle.Bold)
        lblYourRating.Location = New Point(20, 120)
        lblYourRating.AutoSize = True
        Me.Controls.Add(lblYourRating)

        ' Rating ComboBox
        Dim cmbRating As New ComboBox()
        cmbRating.Name = "cmbRating"
        cmbRating.Location = New Point(130, 120)
        cmbRating.Size = New Size(100, 30)
        cmbRating.Font = New Font("Arial", 10)
        cmbRating.Items.AddRange(New String() {"5 - Excellent", "4 - Good", "3 - Average", "2 - Poor", "1 - Terrible"})
        cmbRating.SelectedIndex = 0
        Me.Controls.Add(cmbRating)

        ' Review Text Label
        Dim lblReviewText As New Label()
        lblReviewText.Text = "Your Review:"
        lblReviewText.Font = New Font("Arial", 10, FontStyle.Bold)
        lblReviewText.Location = New Point(20, 165)
        lblReviewText.AutoSize = True
        Me.Controls.Add(lblReviewText)

        ' Review TextBox
        Dim txtReview As New TextBox()
        txtReview.Name = "txtReview"
        txtReview.Location = New Point(20, 190)
        txtReview.Size = New Size(760, 80)
        txtReview.Font = New Font("Arial", 10)
        txtReview.Multiline = True
        Me.Controls.Add(txtReview)

        ' Submit Review Button
        Dim btnSubmit As New Button()
        btnSubmit.Name = "btnSubmit"
        btnSubmit.Text = "SUBMIT REVIEW"
        btnSubmit.Location = New Point(20, 280)
        btnSubmit.Size = New Size(200, 40)
        btnSubmit.Font = New Font("Arial", 10, FontStyle.Bold)
        btnSubmit.BackColor = Color.FromArgb(40, 167, 69)
        btnSubmit.ForeColor = Color.White
        btnSubmit.FlatStyle = FlatStyle.Flat
        btnSubmit.Cursor = Cursors.Hand
        AddHandler btnSubmit.Click, AddressOf BtnSubmit_Click
        Me.Controls.Add(btnSubmit)

        ' Reviews Label
        Dim lblReviews As New Label()
        lblReviews.Text = "Customer Reviews:"
        lblReviews.Font = New Font("Arial", 10, FontStyle.Bold)
        lblReviews.Location = New Point(20, 330)
        lblReviews.AutoSize = True
        Me.Controls.Add(lblReviews)

        ' DataGridView for Reviews
        Dim dgvReviews As New DataGridView()
        dgvReviews.Name = "dgvReviews"
        dgvReviews.Location = New Point(20, 360)
        dgvReviews.Size = New Size(760, 250)
        dgvReviews.AllowUserToAddRows = False
        dgvReviews.AllowUserToDeleteRows = False
        dgvReviews.ReadOnly = True
        dgvReviews.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        dgvReviews.ColumnCount = 5
        dgvReviews.Columns(0).Name = "Username"
        dgvReviews.Columns(1).Name = "Rating"
        dgvReviews.Columns(2).Name = "Review"
        dgvReviews.Columns(3).Name = "Date"
        dgvReviews.Columns(4).Name = "ID"
        dgvReviews.Columns(4).Visible = False
        dgvReviews.BackgroundColor = Color.White
        dgvReviews.GridColor = Color.LightGray
        Me.Controls.Add(dgvReviews)

        ' Close Button
        Dim btnClose As New Button()
        btnClose.Name = "btnClose"
        btnClose.Text = "CLOSE"
        btnClose.Location = New Point(600, 630)
        btnClose.Size = New Size(180, 40)
        btnClose.Font = New Font("Arial", 10, FontStyle.Bold)
        btnClose.BackColor = Color.FromArgb(108, 117, 125)
        btnClose.ForeColor = Color.White
        btnClose.FlatStyle = FlatStyle.Flat
        btnClose.Cursor = Cursors.Hand
        AddHandler btnClose.Click, AddressOf BtnClose_Click
        Me.Controls.Add(btnClose)
    End Sub

    Private Sub LoadRatingInfo()
        Dim averageRating = DBEcommerce.GetProductAverageRating(productId)
        Dim reviewCount = DBEcommerce.GetProductReviewCount(productId)

        Dim lblRating As Label = Me.Controls("lblRating")
        Dim lblCount As Label = Me.Controls("lblCount")

        lblRating.Text = "Average Rating: " & averageRating.ToString("N1") & " / 5.0"
        lblCount.Text = reviewCount.ToString() & " review" & If(reviewCount <> 1, "s", "")
    End Sub

    Private Sub LoadReviews()
        Dim dgvReviews As DataGridView = Me.Controls("dgvReviews")
        dgvReviews.Rows.Clear()

        Dim reviews = DBEcommerce.GetProductReviews(productId)

        For Each row As DataRow In reviews.Rows
            Dim createdDate As DateTime = CDate(row("created_at"))
            dgvReviews.Rows.Add(row("username"), row("rating") & "/5", row("review_text"), createdDate.ToString("yyyy-MM-dd"), row("id"))
        Next
    End Sub

    Private Sub BtnSubmit_Click(sender As Object, e As EventArgs)
        If userId = 0 Then
            MessageBox.Show("Please login first to submit a review.", "Not Logged In", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim cmbRating As ComboBox = Me.Controls("cmbRating")
        Dim txtReview As TextBox = Me.Controls("txtReview")

        Dim rating As Integer = 5 - cmbRating.SelectedIndex
        Dim reviewText As String = txtReview.Text.Trim()

        If String.IsNullOrEmpty(reviewText) Then
            MessageBox.Show("Please write a review.", "Empty Review", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result = DBEcommerce.AddReview(userId, productId, rating, reviewText)

        If result.Item1 Then
            MessageBox.Show(result.Item2, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtReview.Clear()
            LoadReviews()
            LoadRatingInfo()
        Else
            MessageBox.Show(result.Item2, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
