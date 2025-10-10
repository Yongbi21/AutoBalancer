Namespace Models
    Public Class POSTRANS
        Public Property POSTRANS As Integer
        Public Property CONTROL_NO As String
        Public Property TRANS_NO As String

        ' ✅ Automatically extracts the prefix (e.g. "G307" from "G307017987")
        Public ReadOnly Property CounterPrefix As String
            Get
                If String.IsNullOrEmpty(TRANS_NO) Then
                    Return String.Empty
                End If

                ' Match prefix part before numeric sequence changes
                Dim match = System.Text.RegularExpressions.Regex.Match(TRANS_NO, "^[A-Za-z]+\d+")
                If match.Success Then
                    ' Extract letters + first number group (like G307)
                    Dim full = match.Value
                    ' If longer, keep only first 4–5 chars for safety
                    Return full.Substring(0, Math.Min(full.Length, 5))
                End If

                Return String.Empty
            End Get
        End Property
    End Class
End Namespace
