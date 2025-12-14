''' <summary>
''' Product Integration - Load and integrate products with images
''' </summary>
Public Class ProductIntegration
    Public Structure ProductInfo
        Public Property ProductId As Integer
        Public Property ProductName As String
        Public Property Category As String
        Public Property Price As Decimal
        Public Property Stock As Integer
        Public Property ImagePath As String
    End Structure

    Public Shared Function LoadAllProductsWithImages() As List(Of ProductInfo)
        Dim products As New List(Of ProductInfo)()
        Try
            Dim dt = DBmySql.GetAllProducts()
            For Each row In dt.Rows
                products.Add(New ProductInfo With {
                    .ProductId = CInt(row("id")),
                    .ProductName = row("product_name").ToString(),
                    .Category = row("category").ToString(),
                    .Price = CDec(row("price")),
                    .Stock = CInt(row("stock")),
                    .ImagePath = GetProductImagePath(CInt(row("id")), row("category").ToString())
                })
            Next
        Catch ex As Exception
            Debug.WriteLine("Error loading products: " & ex.Message)
        End Try
        Return products
    End Function

    Private Shared Function GetProductImagePath(productId As Integer, category As String) As String
        Try
            Dim path = String.Format(".\images\{0}\product_{1}.jpg", category.ToLower(), productId)
            If System.IO.File.Exists(path) Then
                Return path
            End If
        Catch ex As Exception
            Debug.WriteLine("Error getting image path: " & ex.Message)
        End Try
        Return ""
    End Function

    Public Shared Function LoadProductsByCategory(category As String) As List(Of ProductInfo)
        Dim allProducts = LoadAllProductsWithImages()
        Return allProducts.Where(Function(p) p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList()
    End Function
End Class
