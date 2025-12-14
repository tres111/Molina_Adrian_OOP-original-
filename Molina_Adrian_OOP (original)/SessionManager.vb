Public Module SessionManager
    Public CurrentUserId As Integer = 0
    Public CurrentUsername As String = ""
    Public CurrentUserFullName As String = ""

    Public Sub SetUserSession(userId As Integer, username As String, fullName As String)
        CurrentUserId = userId
        CurrentUsername = username
        CurrentUserFullName = fullName
    End Sub

    Public Sub ClearUserSession()
        CurrentUserId = 0
        CurrentUsername = ""
        CurrentUserFullName = ""
    End Sub

    Public Function IsLoggedIn() As Boolean
        Return CurrentUserId > 0
    End Function
End Module
