<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class OrderReviewForm
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lvOrder = New System.Windows.Forms.ListView()
        Me.chProduct = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chUnitPrice = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chQuantity = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chTotal = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.lblTotalText = New System.Windows.Forms.Label()
        Me.lblGrandTotal = New System.Windows.Forms.Label()
        Me.btnConfirm = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lvOrder
        '
        Me.lvOrder.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chProduct, Me.chUnitPrice, Me.chQuantity, Me.chTotal})
        Me.lvOrder.GridLines = True
        Me.lvOrder.HideSelection = False
        Me.lvOrder.Location = New System.Drawing.Point(12, 12)
        Me.lvOrder.Name = "lvOrder"
        Me.lvOrder.Size = New System.Drawing.Size(560, 300)
        Me.lvOrder.TabIndex = 0
        Me.lvOrder.UseCompatibleStateImageBehavior = False
        Me.lvOrder.View = System.Windows.Forms.View.Details
        '
        'chProduct
        '
        Me.chProduct.Text = "Product"
        Me.chProduct.Width = 280
        '
        'chUnitPrice
        '
        Me.chUnitPrice.Text = "Unit Price"
        Me.chUnitPrice.Width = 100
        '
        'chQuantity
        '
        Me.chQuantity.Text = "Quantity"
        Me.chQuantity.Width = 80
        '
        'chTotal
        '
        Me.chTotal.Text = "Total"
        Me.chTotal.Width = 100
        '
        'lblTotalText
        '
        Me.lblTotalText.AutoSize = True
        Me.lblTotalText.Location = New System.Drawing.Point(12, 325)
        Me.lblTotalText.Name = "lblTotalText"
        Me.lblTotalText.Size = New System.Drawing.Size(67, 13)
        Me.lblTotalText.TabIndex = 1
        Me.lblTotalText.Text = "Grand Total:"
        '
        'lblGrandTotal
        '
        Me.lblGrandTotal.AutoSize = True
        Me.lblGrandTotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Bold)
        Me.lblGrandTotal.Location = New System.Drawing.Point(85, 322)
        Me.lblGrandTotal.Name = "lblGrandTotal"
        Me.lblGrandTotal.Size = New System.Drawing.Size(46, 17)
        Me.lblGrandTotal.TabIndex = 2
        Me.lblGrandTotal.Text = "?0.00"
        '
        'btnConfirm
        '
        Me.btnConfirm.Location = New System.Drawing.Point(416, 318)
        Me.btnConfirm.Name = "btnConfirm"
        Me.btnConfirm.Size = New System.Drawing.Size(75, 23)
        Me.btnConfirm.TabIndex = 3
        Me.btnConfirm.Text = "Confirm"
        Me.btnConfirm.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(497, 318)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 4
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'OrderReviewForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(584, 351)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnConfirm)
        Me.Controls.Add(Me.lblGrandTotal)
        Me.Controls.Add(Me.lblTotalText)
        Me.Controls.Add(Me.lvOrder)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "OrderReviewForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Review Order"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lvOrder As ListView
    Friend WithEvents chProduct As ColumnHeader
    Friend WithEvents chUnitPrice As ColumnHeader
    Friend WithEvents chQuantity As ColumnHeader
    Friend WithEvents chTotal As ColumnHeader
    Friend WithEvents lblTotalText As Label
    Friend WithEvents lblGrandTotal As Label
    Friend WithEvents btnConfirm As Button
    Friend WithEvents btnCancel As Button
End Class
