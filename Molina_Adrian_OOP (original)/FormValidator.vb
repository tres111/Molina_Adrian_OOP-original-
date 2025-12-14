''' <summary>
''' Form Validator - Comprehensive input validation for all forms
''' </summary>
Public Class FormValidator
    Public Structure ValidationResult
        Public Property IsValid As Boolean
        Public Property ErrorMessage As String
    End Structure

    Public Shared Function ValidateRequired(value As String, fieldName As String) As ValidationResult
        If String.IsNullOrWhiteSpace(value) Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " is required"
            }
        End If
        Return New ValidationResult With {.IsValid = True}
    End Function

    Public Shared Function ValidateQuantity(value As String, fieldName As String) As ValidationResult
        If Not Integer.TryParse(value, 0) Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " must be a valid number"
            }
        End If
        If CInt(value) < 0 Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " cannot be negative"
            }
        End If
        Return New ValidationResult With {.IsValid = True}
    End Function

    Public Shared Function ValidatePrice(value As String, fieldName As String) As ValidationResult
        If Not Decimal.TryParse(value, 0) Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " must be a valid decimal"
            }
        End If
        If CDec(value) < 0 Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " cannot be negative"
            }
        End If
        Return New ValidationResult With {.IsValid = True}
    End Function

    Public Shared Function ValidateEmail(value As String, fieldName As String) As ValidationResult
        If String.IsNullOrWhiteSpace(value) Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " is required"
            }
        End If
        If Not value.Contains("@") OrElse Not value.Contains(".") Then
            Return New ValidationResult With {
                .IsValid = False,
                .ErrorMessage = fieldName & " is not valid"
            }
        End If
        Return New ValidationResult With {.IsValid = True}
    End Function

    Public Shared Sub SetError(textBox As TextBox, errorMessage As String)
        If String.IsNullOrEmpty(errorMessage) Then
            textBox.BackColor = Color.White
        Else
            textBox.BackColor = Color.FromArgb(255, 200, 200)
        End If
    End Sub
End Class
