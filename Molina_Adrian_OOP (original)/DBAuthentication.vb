Imports MySql.Data.MySqlClient

Module DBAuthentication
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    Public Function RegisterUser(username As String, email As String, password As String, fullName As String, phone As String, address As String) As Tuple(Of Boolean, String)
        Try
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(email) OrElse String.IsNullOrWhiteSpace(password) Then
                Return New Tuple(Of Boolean, String)(False, "All fields are required.")
            End If

            If username.Length < 3 Then
                Return New Tuple(Of Boolean, String)(False, "Username must be at least 3 characters.")
            End If

            If password.Length < 6 Then
                Return New Tuple(Of Boolean, String)(False, "Password must be at least 6 characters.")
            End If

            If UserExists(username) Then
                Return New Tuple(Of Boolean, String)(False, "Username already exists.")
            End If

            Using conn = GetConnection()
                conn.Open()
                Dim hashedPassword As String = HashPassword(password)
                Dim query As String = "INSERT INTO users (username, email, password, full_name, phone, address, created_at) VALUES (@username, @email, @password, @fullName, @phone, @address, NOW())"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    cmd.Parameters.AddWithValue("@email", email)
                    cmd.Parameters.AddWithValue("@password", hashedPassword)
                    cmd.Parameters.AddWithValue("@fullName", If(String.IsNullOrEmpty(fullName), "", fullName))
                    cmd.Parameters.AddWithValue("@phone", If(String.IsNullOrEmpty(phone), "", phone))
                    cmd.Parameters.AddWithValue("@address", If(String.IsNullOrEmpty(address), "", address))
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Registration successful!")
                End Using
            End Using
        Catch ex As MySqlException
            If ex.Message.Contains("Duplicate entry") Then
                Return New Tuple(Of Boolean, String)(False, "Username or email already exists.")
            End If
            Return New Tuple(Of Boolean, String)(False, "Database error: " & ex.Message)
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "An error occurred: " & ex.Message)
        End Try
    End Function

    Public Function LoginUser(username As String, password As String) As Tuple(Of Boolean, String)
        Try
            If String.IsNullOrWhiteSpace(username) OrElse String.IsNullOrWhiteSpace(password) Then
                Return New Tuple(Of Boolean, String)(False, "Username and password are required.")
            End If

            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, password, full_name FROM users WHERE username = @username LIMIT 1"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            Dim storedHash As String = reader("password").ToString()
                            Dim userId As Integer = CInt(reader("id"))
                            Dim fullName As String = reader("full_name").ToString()

                            If VerifyPassword(password, storedHash) Then
                                SessionManager.SetUserSession(userId, username, fullName)
                                Return New Tuple(Of Boolean, String)(True, "Login successful!")
                            Else
                                Return New Tuple(Of Boolean, String)(False, "Invalid password.")
                            End If
                        Else
                            Return New Tuple(Of Boolean, String)(False, "User not found.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As MySqlException
            Return New Tuple(Of Boolean, String)(False, "Database connection error.")
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "An error occurred: " & ex.Message)
        End Try
    End Function

    Public Sub LogoutUser()
        SessionManager.ClearUserSession()
    End Sub

    Public Function GetCurrentUsername() As String
        Return SessionManager.CurrentUsername
    End Function

    Public Function GetCurrentUserFullName() As String
        Return SessionManager.CurrentUserFullName
    End Function

    Public Function GetCurrentUserId() As Integer
        Return SessionManager.CurrentUserId
    End Function

    Public Function IsUserLoggedIn() As Boolean
        Return SessionManager.IsLoggedIn()
    End Function

    Public Function GetUserProfile(userId As Integer) As DataRow
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, username, email, full_name, phone, address, created_at FROM users WHERE id = @userId"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    
                    If dt.Rows.Count > 0 Then
                        Return dt.Rows(0)
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
        Return Nothing
    End Function

    Public Function UpdateUserProfile(userId As Integer, fullName As String, phone As String, address As String) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "UPDATE users SET full_name = @fullName, phone = @phone, address = @address WHERE id = @userId"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@fullName", fullName)
                    cmd.Parameters.AddWithValue("@phone", phone)
                    cmd.Parameters.AddWithValue("@address", address)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.ExecuteNonQuery()
                    Return New Tuple(Of Boolean, String)(True, "Profile updated successfully!")
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error updating profile: " & ex.Message)
        End Try
    End Function

    Private Function UserExists(username As String) As Boolean
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COUNT(*) FROM users WHERE username = @username"

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@username", username)
                    Dim count = CInt(cmd.ExecuteScalar())
                    Return count > 0
                End Using
            End Using
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function HashPassword(password As String) As String
        Using sha As New System.Security.Cryptography.SHA256Managed()
            Dim hashedBytes As Byte() = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))
            Return Convert.ToBase64String(hashedBytes)
        End Using
    End Function

    Private Function VerifyPassword(password As String, hash As String) As Boolean
        Dim hashOfInput As String = HashPassword(password)
        Return hashOfInput.Equals(hash)
    End Function
End Module
