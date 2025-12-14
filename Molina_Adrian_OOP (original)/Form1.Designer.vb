<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.btnInventory = New System.Windows.Forms.Button()
        Me.pNotifications = New System.Windows.Forms.Panel()
        Me.lvNotifications = New System.Windows.Forms.ListView()
        Me.btnMarkRead = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnCheckout = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnAccessories = New System.Windows.Forms.Button()
        Me.btnFootwear = New System.Windows.Forms.Button()
        Me.btnBottoms = New System.Windows.Forms.Button()
        Me.btnTop = New System.Windows.Forms.Button()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackgroundImage = Global.Molina_Adrian_OOP__original_.My.Resources.Resources.Green_Blue_and_White_Minimalist_Landscape_Desktop_Wallpaper__1_
        Me.Panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Panel1.Controls.Add(Me.lblTotal)
        Me.Panel1.Controls.Add(Me.btnInventory)
        ' pNotifications panel (notification center)
        Me.pNotifications.Name = "pNotifications"
        Me.pNotifications.Location = New System.Drawing.Point(1100, 20)
        Me.pNotifications.Size = New System.Drawing.Size(320, 200)
        Me.pNotifications.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        ' lvNotifications
        Me.lvNotifications.Name = "lvNotifications"
        Me.lvNotifications.View = System.Windows.Forms.View.Details
        Me.lvNotifications.FullRowSelect = True
        Me.lvNotifications.Dock = System.Windows.Forms.DockStyle.Fill
        ' btnMarkRead
        Me.btnMarkRead.Name = "btnMarkRead"
        Me.btnMarkRead.Text = "Mark Read"
        Me.btnMarkRead.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pNotifications.Controls.Add(Me.lvNotifications)
        Me.pNotifications.Controls.Add(Me.btnMarkRead)
        Me.Panel1.Controls.Add(Me.pNotifications)
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.btnCheckout)
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Controls.Add(Me.btnAccessories)
        Me.Panel1.Controls.Add(Me.btnFootwear)
        Me.Panel1.Controls.Add(Me.btnBottoms)
        Me.Panel1.Controls.Add(Me.btnTop)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1443, 857)
        Me.Panel1.TabIndex = 0
        '
        'lblTotal
        '
        Me.lblTotal.AutoSize = True
        Me.lblTotal.BackColor = System.Drawing.Color.Transparent
        Me.lblTotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 22.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotal.ForeColor = System.Drawing.SystemColors.ActiveBorder
        Me.lblTotal.Location = New System.Drawing.Point(433, 36)
        Me.lblTotal.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(81, 36)
        Me.lblTotal.TabIndex = 10
        Me.lblTotal.Text = "Total"
        '
        'btnInventory
        '
        Me.btnInventory.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnInventory.Font = New System.Drawing.Font("Mongolian Baiti", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnInventory.Location = New System.Drawing.Point(68, 422)
        Me.btnInventory.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnInventory.Name = "btnInventory"
        Me.btnInventory.Size = New System.Drawing.Size(115, 37)
        Me.btnInventory.TabIndex = 9
        Me.btnInventory.Text = "Inventory"
        Me.btnInventory.UseVisualStyleBackColor = False
        '
        'Panel2
        '
        Me.Panel2.BackgroundImage = Global.Molina_Adrian_OOP__original_.My.Resources.Resources.jutjtujt
        Me.Panel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Panel2.Location = New System.Drawing.Point(495, 202)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(1217, 873)
        Me.Panel2.TabIndex = 8
        '
        'btnCheckout
        '
        Me.btnCheckout.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnCheckout.Font = New System.Drawing.Font("Mongolian Baiti", 10.2!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCheckout.Location = New System.Drawing.Point(443, 87)
        Me.btnCheckout.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnCheckout.Name = "btnCheckout"
        Me.btnCheckout.Size = New System.Drawing.Size(89, 26)
        Me.btnCheckout.TabIndex = 7
        Me.btnCheckout.Text = "Check out"
        Me.btnCheckout.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 28.2!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(378, 32)
        Me.Label1.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(66, 44)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "₱="
        '
        'btnAccessories
        '
        Me.btnAccessories.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnAccessories.Font = New System.Drawing.Font("Mongolian Baiti", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnAccessories.Location = New System.Drawing.Point(68, 363)
        Me.btnAccessories.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnAccessories.Name = "btnAccessories"
        Me.btnAccessories.Size = New System.Drawing.Size(115, 37)
        Me.btnAccessories.TabIndex = 3
        Me.btnAccessories.Text = "Accessories"
        Me.btnAccessories.UseVisualStyleBackColor = False
        '
        'btnFootwear
        '
        Me.btnFootwear.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnFootwear.Font = New System.Drawing.Font("Mongolian Baiti", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnFootwear.Location = New System.Drawing.Point(68, 306)
        Me.btnFootwear.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnFootwear.Name = "btnFootwear"
        Me.btnFootwear.Size = New System.Drawing.Size(115, 37)
        Me.btnFootwear.TabIndex = 2
        Me.btnFootwear.Text = "Footwear"
        Me.btnFootwear.UseVisualStyleBackColor = False
        '
        'btnBottoms
        '
        Me.btnBottoms.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnBottoms.Font = New System.Drawing.Font("Mongolian Baiti", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBottoms.Location = New System.Drawing.Point(68, 243)
        Me.btnBottoms.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnBottoms.Name = "btnBottoms"
        Me.btnBottoms.Size = New System.Drawing.Size(115, 37)
        Me.btnBottoms.TabIndex = 1
        Me.btnBottoms.Text = "Bottoms"
        Me.btnBottoms.UseVisualStyleBackColor = False
        '
        'btnTop
        '
        Me.btnTop.BackColor = System.Drawing.SystemColors.ActiveCaption
        Me.btnTop.Font = New System.Drawing.Font("Mongolian Baiti", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTop.Location = New System.Drawing.Point(68, 186)
        Me.btnTop.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.btnTop.Name = "btnTop"
        Me.btnTop.Size = New System.Drawing.Size(115, 37)
        Me.btnTop.TabIndex = 0
        Me.btnTop.Text = "Top"
        Me.btnTop.UseVisualStyleBackColor = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1443, 857)
        Me.Controls.Add(Me.Panel1)
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.Name = "Form1"
        Me.Text = "Home"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnAccessories As Button
    Friend WithEvents btnFootwear As Button
    Friend WithEvents btnBottoms As Button
    Friend WithEvents btnTop As Button
    Friend WithEvents btnCheckout As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents btnInventory As Button
    Friend WithEvents lblTotal As Label
    Friend WithEvents pNotifications As Panel
    Friend WithEvents lvNotifications As ListView
    Friend WithEvents btnMarkRead As Button
End Class
