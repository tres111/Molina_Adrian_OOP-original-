Imports MySql.Data.MySqlClient

''' <summary>
''' Modern loyalty program to increase customer retention and lifetime value
''' Supports points, tiers, and exclusive rewards
''' </summary>
Module LoyaltyProgramManager
    Private ReadOnly connectionString As String = "server=localhost; userid=root; password=; database=coziest; port=3306;"

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    ' ===== LOYALTY TIERS =====
    Public Enum LoyaltyTier
        Bronze = 1
        Silver = 2
        Gold = 3
        Platinum = 4
    End Enum

    ' ===== POINTS MANAGEMENT =====
    ''' <summary>
    ''' Add loyalty points for purchase
    ''' 1 point per peso spent
    ''' </summary>
    Public Function AddPointsForPurchase(userId As Integer, orderAmount As Decimal) As Tuple(Of Boolean, String)
        Try
            Dim points As Integer = CInt(orderAmount) ' 1 point per peso

            Using conn = GetConnection()
                conn.Open()

                ' Check if user has loyalty account
                Dim checkQuery As String = "SELECT id FROM loyalty_accounts WHERE user_id = @userId"
                Using cmd As New MySqlCommand(checkQuery, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()

                    If result Is Nothing Then
                        ' Create loyalty account
                        Dim createQuery As String = "INSERT INTO loyalty_accounts (user_id, points, tier, created_at) " &
                            "VALUES (@userId, @points, 1, NOW())"
                        Using createCmd As New MySqlCommand(createQuery, conn)
                            createCmd.Parameters.AddWithValue("@userId", userId)
                            createCmd.Parameters.AddWithValue("@points", points)
                            createCmd.ExecuteNonQuery()
                        End Using
                    Else
                        ' Add points
                        Dim addQuery As String = "UPDATE loyalty_accounts SET points = points + @points WHERE user_id = @userId"
                        Using addCmd As New MySqlCommand(addQuery, conn)
                            addCmd.Parameters.AddWithValue("@points", points)
                            addCmd.Parameters.AddWithValue("@userId", userId)
                            addCmd.ExecuteNonQuery()
                        End Using
                    End If

                    ' Log transaction
                    Dim logQuery As String = "INSERT INTO loyalty_transactions (user_id, points, type, order_id, created_at) " &
                        "VALUES (@userId, @points, 'earned', NULL, NOW())"
                    Using logCmd As New MySqlCommand(logQuery, conn)
                        logCmd.Parameters.AddWithValue("@userId", userId)
                        logCmd.Parameters.AddWithValue("@points", points)
                        logCmd.ExecuteNonQuery()
                    End Using

                    ' Check and update tier
                    UpdateLoyaltyTier(userId, conn)

                    Return New Tuple(Of Boolean, String)(True, String.Format("Added {0} loyalty points!", points))
                End Using
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error adding points: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Redeem loyalty points for discount
    ''' 100 points = ?100 discount
    ''' </summary>
    Public Function RedeemPoints(userId As Integer, pointsToRedeem As Integer) As Tuple(Of Boolean, String)
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Check available points
                Dim checkQuery As String = "SELECT points FROM loyalty_accounts WHERE user_id = @userId"
                Using cmd As New MySqlCommand(checkQuery, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()
                    
                    If result Is Nothing Then
                        Return New Tuple(Of Boolean, String)(False, "No loyalty account found.")
                    End If

                    Dim availablePoints = CInt(result)
                    If availablePoints < pointsToRedeem Then
                        Return New Tuple(Of Boolean, String)(False, String.Format("Insufficient points. Available: {0}", availablePoints))
                    End If
                End Using

                ' Deduct points
                Dim redeemQuery As String = "UPDATE loyalty_accounts SET points = points - @points WHERE user_id = @userId"
                Using cmd As New MySqlCommand(redeemQuery, conn)
                    cmd.Parameters.AddWithValue("@points", pointsToRedeem)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.ExecuteNonQuery()
                End Using

                ' Log transaction
                Dim logQuery As String = "INSERT INTO loyalty_transactions (user_id, points, type, created_at) " &
                    "VALUES (@userId, @points, 'redeemed', NOW())"
                Using cmd As New MySqlCommand(logQuery, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    cmd.Parameters.AddWithValue("@points", pointsToRedeem)
                    cmd.ExecuteNonQuery()
                End Using

                Dim discountAmount As Decimal = pointsToRedeem ' 100 points = ?100
                Return New Tuple(Of Boolean, String)(True, String.Format("Redeemed {0} points for ?{1} discount!", pointsToRedeem, discountAmount))
            End Using
        Catch ex As Exception
            Return New Tuple(Of Boolean, String)(False, "Error redeeming points: " & ex.Message)
        End Try
    End Function

    ''' <summary>
    ''' Get user's loyalty points balance
    ''' </summary>
    Public Function GetLoyaltyPoints(userId As Integer) As Integer
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT COALESCE(points, 0) FROM loyalty_accounts WHERE user_id = @userId"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()
                    Return CInt(result)
                End Using
            End Using
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ' ===== TIER MANAGEMENT =====
    ''' <summary>
    ''' Update user loyalty tier based on spending
    ''' Bronze: 0-5000 points
    ''' Silver: 5001-10000 points
    ''' Gold: 10001-25000 points
    ''' Platinum: 25001+ points
    ''' </summary>
    Private Sub UpdateLoyaltyTier(userId As Integer, conn As MySqlConnection)
        Try
            Dim query As String = "SELECT points FROM loyalty_accounts WHERE user_id = @userId"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@userId", userId)
                Dim points = CInt(cmd.ExecuteScalar())

                Dim newTier As Integer
                If points >= 25001 Then
                    newTier = 4 ' Platinum
                ElseIf points >= 10001 Then
                    newTier = 3 ' Gold
                ElseIf points >= 5001 Then
                    newTier = 2 ' Silver
                Else
                    newTier = 1 ' Bronze
                End If

                Dim updateQuery As String = "UPDATE loyalty_accounts SET tier = @tier WHERE user_id = @userId"
                Using updateCmd As New MySqlCommand(updateQuery, conn)
                    updateCmd.Parameters.AddWithValue("@tier", newTier)
                    updateCmd.Parameters.AddWithValue("@userId", userId)
                    updateCmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Get user's loyalty tier
    ''' </summary>
    Public Function GetLoyaltyTier(userId As Integer) As String
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT tier FROM loyalty_accounts WHERE user_id = @userId"
                
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@userId", userId)
                    Dim result = cmd.ExecuteScalar()
                    
                    If result Is Nothing Then
                        Return "Bronze"
                    End If

                    Select Case CInt(result)
                        Case 1
                            Return "Bronze"
                        Case 2
                            Return "Silver"
                        Case 3
                            Return "Gold"
                        Case 4
                            Return "Platinum"
                        Case Else
                            Return "Bronze"
                    End Select
                End Using
            End Using
        Catch ex As Exception
            Return "Bronze"
        End Try
    End Function

    ' ===== TIER BENEFITS =====
    ''' <summary>
    ''' Get discount percentage based on tier
    ''' </summary>
    Public Function GetTierDiscount(userId As Integer) As Decimal
        Try
            Dim tier As String = GetLoyaltyTier(userId)
            Select Case tier
                Case "Bronze"
                    Return 0D
                Case "Silver"
                    Return 2D ' 2% discount
                Case "Gold"
                    Return 5D ' 5% discount
                Case "Platinum"
                    Return 10D ' 10% discount
                Case Else
                    Return 0D
            End Select
        Catch ex As Exception
            Return 0D
        End Try
    End Function

    ''' <summary>
    ''' Get bonus points multiplier based on tier
    ''' </summary>
    Public Function GetBonusPointsMultiplier(userId As Integer) As Decimal
        Try
            Dim tier As String = GetLoyaltyTier(userId)
            Select Case tier
                Case "Bronze"
                    Return 1.0D
                Case "Silver"
                    Return 1.1D ' 10% extra
                Case "Gold"
                    Return 1.2D ' 20% extra
                Case "Platinum"
                    Return 1.5D ' 50% extra
                Case Else
                    Return 1.0D
            End Select
        Catch ex As Exception
            Return 1.0D
        End Try
    End Function

    ' ===== REWARDS CATALOG =====
    ''' <summary>
    ''' Get available rewards for redemption
    ''' </summary>
    Public Function GetAvailableRewards() As DataTable
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT id, reward_name, points_required, discount_amount, description FROM loyalty_rewards WHERE is_active = 1"
                
                Using cmd As New MySqlCommand(query, conn)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    Return dt
                End Using
            End Using
        Catch ex As Exception
            Return New DataTable()
        End Try
    End Function

    ' ===== LOYALTY ANALYTICS =====
    ''' <summary>
    ''' Get loyalty program statistics
    ''' </summary>
    Public Function GetLoyaltyStats() As Dictionary(Of String, Object)
        Dim stats As New Dictionary(Of String, Object)
        Try
            Using conn = GetConnection()
                conn.Open()

                ' Total members
                Dim membersQuery As String = "SELECT COUNT(*) FROM loyalty_accounts"
                Using cmd As New MySqlCommand(membersQuery, conn)
                    stats("total_members") = CInt(cmd.ExecuteScalar())
                End Using

                ' Total points issued
                Dim pointsQuery As String = "SELECT COALESCE(SUM(points), 0) FROM loyalty_accounts"
                Using cmd As New MySqlCommand(pointsQuery, conn)
                    stats("total_points_issued") = CInt(cmd.ExecuteScalar())
                End Using

                ' Average points per member
                Dim avgQuery As String = "SELECT COALESCE(AVG(points), 0) FROM loyalty_accounts"
                Using cmd As New MySqlCommand(avgQuery, conn)
                    stats("avg_points_per_member") = CInt(cmd.ExecuteScalar())
                End Using

                ' Members by tier
                Dim tierQuery As String = "SELECT tier, COUNT(*) as count FROM loyalty_accounts GROUP BY tier"
                Using cmd As New MySqlCommand(tierQuery, conn)
                    Dim adapter As New MySqlDataAdapter(cmd)
                    Dim dt As New DataTable()
                    adapter.Fill(dt)
                    stats("members_by_tier") = dt
                End Using

            End Using
        Catch ex As Exception
        End Try
        Return stats
    End Function

End Module
