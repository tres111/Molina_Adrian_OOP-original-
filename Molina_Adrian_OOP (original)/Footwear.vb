Public Class Footwear

    Private mainForm As Form1

    Public Sub New(frm As Form1)
        InitializeComponent()
        mainForm = frm
    End Sub

    Private Sub Footwear_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        lblQty31.Text = Cart.qtyFootwear(0).ToString()
        lblQty32.Text = Cart.qtyFootwear(1).ToString()
        lblQty34.Text = Cart.qtyFootwear(2).ToString()
        lblQty35.Text = Cart.qtyFootwear(3).ToString()
        lblQty36.Text = Cart.qtyFootwear(4).ToString()
        lblQty37.Text = Cart.qtyFootwear(5).ToString()

        mainForm.UpdateTotal()

        ' Display OUT OF STOCK badges for footwear
        Try
            Dim panels = New Panel() {Panel1, Panel2, Panel3, Panel4, Panel5, Panel6}
            For i = 0 To Math.Min(panels.Length - 1, Cart.productIdFootwear.Length - 1)
                Dim pid = Cart.productIdFootwear(i)
                Dim name = Cart.footwearProducts(i)
                Dim outOfStock As Boolean = If(pid > 0, DBmySql.IsOutOfStockById(pid), DBmySql.IsOutOfStock(name))
                If outOfStock Then
                    Dim p = panels(i)
                    If p IsNot Nothing Then
                        Dim existing = p.Controls.Find("lblOutOfStock", False)
                        If existing.Length = 0 Then
                            Dim badge As New Label()
                            badge.Name = "lblOutOfStock"
                            badge.Text = "OUT OF STOCK"
                            badge.BackColor = Drawing.Color.FromArgb(180, Drawing.Color.DarkRed)
                            badge.ForeColor = Drawing.Color.White
                            badge.AutoSize = False
                            badge.TextAlign = ContentAlignment.MiddleCenter
                            badge.Font = New Drawing.Font("Segoe UI", 10.0!, Drawing.FontStyle.Bold)
                            badge.Size = New Drawing.Size(p.Width, 24)
                            badge.Location = New Drawing.Point(0, 0)
                            p.Controls.Add(badge)
                            badge.BringToFront()
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("Error showing footwear out of stock badges: " & ex.Message)
        End Try
    End Sub


    Private Sub btnPlus1_Click(sender As Object, e As EventArgs) Handles btnPlus1.Click
        Dim name = Cart.footwearProducts(0)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyFootwear(0) += 1
        lblQty31.Text = Cart.qtyFootwear(0)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus1_Click(sender As Object, e As EventArgs) Handles btnMinus1.Click
        If Cart.qtyFootwear(0) > 0 Then Cart.qtyFootwear(0) -= 1
        lblQty31.Text = Cart.qtyFootwear(0)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 2 (Index 1)
    ' ============================================================

    Private Sub btnPlus2_Click(sender As Object, e As EventArgs) Handles btnPlus2.Click
        Dim name = Cart.footwearProducts(1)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyFootwear(1) += 1
        lblQty32.Text = Cart.qtyFootwear(1)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus2_Click(sender As Object, e As EventArgs) Handles btnMinus2.Click
        If Cart.qtyFootwear(1) > 0 Then Cart.qtyFootwear(1) -= 1
        lblQty32.Text = Cart.qtyFootwear(1)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 3 (Index 2)
    ' ============================================================

    Private Sub btnPlus3_Click(sender As Object, e As EventArgs) Handles btnPlus3.Click
        Dim name = Cart.footwearProducts(2)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyFootwear(2) += 1
        lblQty34.Text = Cart.qtyFootwear(2)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus3_Click(sender As Object, e As EventArgs) Handles btnMinus3.Click
        If Cart.qtyFootwear(2) > 0 Then Cart.qtyFootwear(2) -= 1
        lblQty34.Text = Cart.qtyFootwear(2)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 4 (Index 3)
    ' ============================================================

    Private Sub btnPlus4_Click(sender As Object, e As EventArgs) Handles btnPlus4.Click
        Dim name = Cart.footwearProducts(3)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyFootwear(3) += 1
        lblQty35.Text = Cart.qtyFootwear(3)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus4_Click(sender As Object, e As EventArgs) Handles btnMinus4.Click
        If Cart.qtyFootwear(3) > 0 Then Cart.qtyFootwear(3) -= 1
        lblQty35.Text = Cart.qtyFootwear(3)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 5 (Index 4)
    ' ============================================================

    Private Sub btnPlus5_Click(sender As Object, e As EventArgs) Handles btnPlus5.Click
        Dim name = Cart.footwearProducts(4)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyFootwear(4) += 1
        lblQty36.Text = Cart.qtyFootwear(4)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus5_Click(sender As Object, e As EventArgs) Handles btnMinus5.Click
        If Cart.qtyFootwear(4) > 0 Then Cart.qtyFootwear(4) -= 1
        lblQty36.Text = Cart.qtyFootwear(4)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 6 (Index 5)
    ' ============================================================

    Private Sub btnPlus6_Click(sender As Object, e As EventArgs) Handles btnPlus6.Click
        Dim name = Cart.footwearProducts(5)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyFootwear(5) += 1
        lblQty37.Text = Cart.qtyFootwear(5)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus6_Click(sender As Object, e As EventArgs) Handles btnMinus6.Click
        If Cart.qtyFootwear(5) > 0 Then Cart.qtyFootwear(5) -= 1
        lblQty37.Text = Cart.qtyFootwear(5)
        mainForm.UpdateTotal()
    End Sub
End Class
