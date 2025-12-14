Public Class Cart
    ' Store product ids for items in cart to avoid index-offset issues
    Public Shared productIdTop(8) As Integer
    Public Shared productIdBottoms(8) As Integer
    Public Shared productIdFootwear(5) As Integer
    Public Shared productIdAccessories(5) As Integer

    Public Shared qtyTop(8) As Integer
    Public Shared qtyBottoms(8) As Integer
    Public Shared qtyFootwear(5) As Integer  ' Changed to 5 since you have 6 footwear items (0-5)
    Public Shared qtyAccessories(5) As Integer  ' Changed to 5 since you have 6 accessories (0-5)

    ' Product names for database lookup
    Public Shared topProducts As String() = {
        "Signature Script Tee",
        "Monochrome Statement Tee",
        "Sunshine Smile Tee",
        "Racing Stripe Tee",
        "Sky Blue Heritage Tee",
        "Flame Logo Tee",
        "Neon Flash Tee",
        "Midnight Satin Bomber",
        "Carolina Blue Varsity Hoodie"
    }

    Public Shared bottomProducts As String() = {
        "Wave Rider Boardshorts",
        "Wild Side Leopard Shorts",
        "Electric Storm Shorts",
        "Urban Essential Shorts",
        "Powder Blue 303 Shorts",
        "Gothic Edge Shorts",
        "Renaissance Art Shorts",
        "Paisley Dreams Shorts",
        "Shadow Knit Shorts"
    }

    Public Shared footwearProducts As String() = {
        "Classic Court Sneakers",
        "Midnight Slide Sandals",
        "Expedition Olive Sandals",
        "Desert Dune Sandals",
        "Earth Tone Sandals",
        "Stealth Black Sandals"
    }

    Public Shared accessoryProducts As String() = {
        "Performance Sweatband",
        "Elite Crossbody Bag",
        "Face Shield Balaclava",
        "Heritage Explorer Backpack",
        "Coziest Sticker Pack",
        "Coziest Lanyard"
    }

    Public Shared Function GetTotal() As Decimal
        Dim total As Decimal = 0

        ' Calculate Tops
        For i = 0 To 8
            If qtyTop(i) > 0 Then
                Dim pid = productIdTop(i)
                If pid > 0 Then
                    total += qtyTop(i) * DBmySql.GetPriceById(pid)
                Else
                    total += qtyTop(i) * DBmySql.GetPrice(topProducts(i))
                End If
            End If
        Next

        ' Calculate Bottoms
        For i = 0 To 8
            If qtyBottoms(i) > 0 Then
                Dim pid = productIdBottoms(i)
                If pid > 0 Then
                    total += qtyBottoms(i) * DBmySql.GetPriceById(pid)
                Else
                    total += qtyBottoms(i) * DBmySql.GetPrice(bottomProducts(i))
                End If
            End If
        Next

        ' Calculate Footwear
        For i = 0 To 5
            If qtyFootwear(i) > 0 Then
                Dim pid = productIdFootwear(i)
                If pid > 0 Then
                    total += qtyFootwear(i) * DBmySql.GetPriceById(pid)
                Else
                    total += qtyFootwear(i) * DBmySql.GetPrice(footwearProducts(i))
                End If
            End If
        Next

        ' Calculate Accessories
        For i = 0 To 5
            If qtyAccessories(i) > 0 Then
                Dim pid = productIdAccessories(i)
                If pid > 0 Then
                    total += qtyAccessories(i) * DBmySql.GetPriceById(pid)
                Else
                    total += qtyAccessories(i) * DBmySql.GetPrice(accessoryProducts(i))
                End If
            End If
        Next

        Return total
    End Function

    ' Initialize product id arrays by looking up products table by name
    Public Shared Sub InitializeProductIds()
        For i = 0 To topProducts.Length - 1
            Dim name = topProducts(i)
            productIdTop(i) = DBmySql.GetProductIdByName(name)
        Next
        For i = 0 To bottomProducts.Length - 1
            Dim name = bottomProducts(i)
            productIdBottoms(i) = DBmySql.GetProductIdByName(name)
        Next
        For i = 0 To footwearProducts.Length - 1
            Dim name = footwearProducts(i)
            productIdFootwear(i) = DBmySql.GetProductIdByName(name)
        Next
        For i = 0 To accessoryProducts.Length - 1
            Dim name = accessoryProducts(i)
            productIdAccessories(i) = DBmySql.GetProductIdByName(name)
        Next
    End Sub

    Public Shared Sub ResetCart()
        ' Reset quantities for Accessories
        For i As Integer = 0 To qtyAccessories.Length - 1
            qtyAccessories(i) = 0
        Next

        ' Reset quantities for Top
        For i As Integer = 0 To qtyTop.Length - 1
            qtyTop(i) = 0
            qtyBottoms(i) = 0
            productIdTop(i) = 0
            productIdBottoms(i) = 0
        Next

        For i = 0 To 5
            qtyFootwear(i) = 0
            qtyAccessories(i) = 0
            productIdFootwear(i) = 0
            productIdAccessories(i) = 0
        Next
    End Sub

    ' Process checkout - reduce stock for all items in cart
    Public Shared Sub ProcessCheckout()
        For i = 0 To 8
            If qtyTop(i) > 0 Then
                If productIdTop(i) > 0 Then
                    DBmySql.ReduceStockById(productIdTop(i), qtyTop(i))
                Else
                    DBmySql.ReduceStock(topProducts(i), qtyTop(i))
                End If
            End If
            If qtyBottoms(i) > 0 Then
                If productIdBottoms(i) > 0 Then
                    DBmySql.ReduceStockById(productIdBottoms(i), qtyBottoms(i))
                Else
                    DBmySql.ReduceStock(bottomProducts(i), qtyBottoms(i))
                End If
            End If
        Next

        For i = 0 To 5
            If qtyFootwear(i) > 0 Then
                If productIdFootwear(i) > 0 Then
                    DBmySql.ReduceStockById(productIdFootwear(i), qtyFootwear(i))
                Else
                    DBmySql.ReduceStock(footwearProducts(i), qtyFootwear(i))
                End If
            End If
            If qtyAccessories(i) > 0 Then
                If productIdAccessories(i) > 0 Then
                    DBmySql.ReduceStockById(productIdAccessories(i), qtyAccessories(i))
                Else
                    DBmySql.ReduceStock(accessoryProducts(i), qtyAccessories(i))
                End If
            End If
        Next

        ' After reducing stock, notify admin(s) about low stock
        Try
            Dim adminService As New AdminNotificationService()
            ' check low stock for each product and send alert when below threshold
            For i = 0 To 8
                If qtyTop(i) > 0 Then
                    Dim pid = productIdTop(i)
                    Dim current As Integer
                    Dim name As String
                    If pid > 0 Then
                        current = DBmySql.GetStockById(pid)
                        name = DBmySql.GetProductById(pid)?("product_name").ToString()
                    Else
                        current = DBmySql.GetStock(topProducts(i))
                        name = topProducts(i)
                        pid = i + 1
                    End If
                    If current <= 0 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 0)
                    ElseIf current <= 5 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 5)
                    End If
                End If
                If qtyBottoms(i) > 0 Then
                    Dim pid = productIdBottoms(i)
                    Dim current As Integer
                    Dim name As String
                    If pid > 0 Then
                        current = DBmySql.GetStockById(pid)
                        name = DBmySql.GetProductById(pid)?("product_name").ToString()
                    Else
                        current = DBmySql.GetStock(bottomProducts(i))
                        name = bottomProducts(i)
                        pid = i + 1
                    End If
                    If current <= 0 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 0)
                    ElseIf current <= 5 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 5)
                    End If
                End If
            Next

            For i = 0 To 5
                If qtyFootwear(i) > 0 Then
                    Dim pid = productIdFootwear(i)
                    Dim current As Integer
                    Dim name As String
                    If pid > 0 Then
                        current = DBmySql.GetStockById(pid)
                        name = DBmySql.GetProductById(pid)?("product_name").ToString()
                    Else
                        current = DBmySql.GetStock(footwearProducts(i))
                        name = footwearProducts(i)
                        pid = i + 1
                    End If
                    If current <= 0 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 0)
                    ElseIf current <= 5 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 5)
                    End If
                End If
                If qtyAccessories(i) > 0 Then
                    Dim pid = productIdAccessories(i)
                    Dim current As Integer
                    Dim name As String
                    If pid > 0 Then
                        current = DBmySql.GetStockById(pid)
                        name = DBmySql.GetProductById(pid)?("product_name").ToString()
                    Else
                        current = DBmySql.GetStock(accessoryProducts(i))
                        name = accessoryProducts(i)
                        pid = i + 1
                    End If
                    If current <= 0 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 0)
                    ElseIf current <= 5 Then
                        adminService.SendStockAlertNotification(1, pid, name, "", current, 5)
                    End If
                End If
            Next
        Catch ex As Exception
            Debug.WriteLine("Error notifying admin on checkout: " & ex.Message)
        End Try

        ResetCart()
    End Sub
End Class