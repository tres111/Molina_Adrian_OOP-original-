''' <summary>
''' Represents an admin user in the system
''' </summary>
Public Class AdminUser
    Public Property UserId As Integer
    Public Property Username As String
    Public Property Email As String
    Public Property FullName As String
    Public Property Role As String
    Public Property IsActive As Boolean
    Public Property LastLogin As DateTime?
    Public Property CreatedDate As DateTime

    Public Sub New()
        UserId = 0
        Username = ""
        Email = ""
        FullName = ""
        Role = "Admin"
        IsActive = True
        CreatedDate = DateTime.Now
    End Sub

    Public Overrides Function ToString() As String
        Return String.Format("{0} ({1})", FullName, Username)
    End Function
End Class
