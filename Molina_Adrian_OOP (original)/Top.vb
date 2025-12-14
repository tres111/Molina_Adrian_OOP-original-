Public Class Top

    Private mainForm As Form1

    Public Sub New(frm As Form1)
        InitializeComponent()
        mainForm = frm
    End Sub

    Private Sub Top_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        lblQty1.Text = Cart.qtyTop(0)
        lblQty2.Text = Cart.qtyTop(1)
        lblQty3.Text = Cart.qtyTop(2)
        lblQty4.Text = Cart.qtyTop(3)
        lblQty5.Text = Cart.qtyTop(4)
        lblQty6.Text = Cart.qtyTop(5)
        lblQty7.Text = Cart.qtyTop(6)
        lblQty8.Text = Cart.qtyTop(7)
        lblQty9.Text = Cart.qtyTop(8)

        mainForm.UpdateTotal()

        ' Show OUT OF STOCK badge on product panels
        Try
            Dim panels = New Panel() {Panel1, Panel2, Panel3, Panel4, Panel5, Panel6, Panel7, Panel8, Panel9}
            For i = 0 To Math.Min(panels.Length - 1, Cart.productIdTop.Length - 1)
                Dim pid = Cart.productIdTop(i)
                Dim name = Cart.topProducts(i)
                Dim outOfStock As Boolean = False
                If pid > 0 Then
                    outOfStock = DBmySql.IsOutOfStockById(pid)
                Else
                    outOfStock = DBmySql.IsOutOfStock(name)
                End If
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
            Debug.WriteLine("Error showing out of stock badges: " & ex.Message)
        End Try
    End Sub


    ' ============================================================
    ' ITEM 1 (Index 0)
    ' ============================================================

    Private Sub btnPlus1_Click(sender As Object, e As EventArgs) Handles btnPlus1.Click
        Dim name = Cart.topProducts(0)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(0) += 1
        lblQty1.Text = Cart.qtyTop(0)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus1_Click(sender As Object, e As EventArgs) Handles btnMinus1.Click
        If Cart.qtyTop(0) > 0 Then Cart.qtyTop(0) -= 1
        lblQty1.Text = Cart.qtyTop(0)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 2 (Index 1)
    ' ============================================================

    Private Sub btnPlus2_Click(sender As Object, e As EventArgs) Handles btnPlus2.Click
        Dim name = Cart.topProducts(1)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(1) += 1
        lblQty2.Text = Cart.qtyTop(1)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus2_Click(sender As Object, e As EventArgs) Handles btnMinus2.Click
        If Cart.qtyTop(1) > 0 Then Cart.qtyTop(1) -= 1
        lblQty2.Text = Cart.qtyTop(1)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 3 (Index 2)
    ' ============================================================

    Private Sub btnPlus3_Click(sender As Object, e As EventArgs) Handles btnPlus3.Click
        Dim name = Cart.topProducts(2)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(2) += 1
        lblQty3.Text = Cart.qtyTop(2)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus3_Click(sender As Object, e As EventArgs) Handles btnMinus3.Click
        If Cart.qtyTop(2) > 0 Then Cart.qtyTop(2) -= 1
        lblQty3.Text = Cart.qtyTop(2)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 4 (Index 3)
    ' ============================================================

    Private Sub btnPlus4_Click(sender As Object, e As EventArgs) Handles btnPlus4.Click
        Dim name = Cart.topProducts(3)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(3) += 1
        lblQty4.Text = Cart.qtyTop(3)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus4_Click(sender As Object, e As EventArgs) Handles btnMinus4.Click
        If Cart.qtyTop(3) > 0 Then Cart.qtyTop(3) -= 1
        lblQty4.Text = Cart.qtyTop(3)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 5 (Index 4)
    ' ============================================================

    Private Sub btnPlus5_Click(sender As Object, e As EventArgs) Handles btnPlus5.Click
        Dim name = Cart.topProducts(4)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(4) += 1
        lblQty5.Text = Cart.qtyTop(4)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus5_Click(sender As Object, e As EventArgs) Handles btnMinus5.Click
        If Cart.qtyTop(4) > 0 Then Cart.qtyTop(4) -= 1
        lblQty5.Text = Cart.qtyTop(4)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 6 (Index 5)
    ' ============================================================

    Private Sub btnPlus6_Click(sender As Object, e As EventArgs) Handles btnPlus6.Click
        Dim name = Cart.topProducts(5)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(5) += 1
        lblQty6.Text = Cart.qtyTop(5)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus6_Click(sender As Object, e As EventArgs) Handles btnMinus6.Click
        If Cart.qtyTop(5) > 0 Then Cart.qtyTop(5) -= 1
        lblQty6.Text = Cart.qtyTop(5)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 7 (Index 6)
    ' ============================================================

    Private Sub btnPlus7_Click(sender As Object, e As EventArgs) Handles btnPlus7.Click
        Dim name = Cart.topProducts(6)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(6) += 1
        lblQty7.Text = Cart.qtyTop(6)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus7_Click(sender As Object, e As EventArgs) Handles btnMinus7.Click
        If Cart.qtyTop(6) > 0 Then Cart.qtyTop(6) -= 1
        lblQty7.Text = Cart.qtyTop(6)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 8 (Index 7)
    ' ============================================================

    Private Sub btnPlus8_Click(sender As Object, e As EventArgs) Handles btnPlus8.Click
        Dim name = Cart.topProducts(7)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(7) += 1
        lblQty8.Text = Cart.qtyTop(7)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus8_Click(sender As Object, e As EventArgs) Handles btnMinus8.Click
        If Cart.qtyTop(7) > 0 Then Cart.qtyTop(7) -= 1
        lblQty8.Text = Cart.qtyTop(7)
        mainForm.UpdateTotal()
    End Sub


    ' ============================================================
    ' ITEM 9 (Index 8)
    ' ============================================================

    Private Sub btnPlus9_Click(sender As Object, e As EventArgs) Handles btnPlus9.Click
        Dim name = Cart.topProducts(8)
        If DBmySql.IsOutOfStock(name) Then
            MessageBox.Show(name & " is OUT OF STOCK.", "Out of Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Cart.qtyTop(8) += 1
        lblQty9.Text = Cart.qtyTop(8)
        mainForm.UpdateTotal()
    End Sub

    Private Sub btnMinus9_Click(sender As Object, e As EventArgs) Handles btnMinus9.Click
        If Cart.qtyTop(8) > 0 Then Cart.qtyTop(8) -= 1
        lblQty9.Text = Cart.qtyTop(8)
        mainForm.UpdateTotal()
    End Sub

End Class
