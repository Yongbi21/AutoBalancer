Namespace Models
    Public Class TRANSPAY
        Public Property TRANSPAY As Integer
        Public Property CONTROL_NO As String
        Public Property TRANS_NO As String
        Public Property OriginalControlNo As String

        ' ✅ Automatically extracts prefix (e.g. "G307" from "G307017654")
        Public ReadOnly Property CounterPrefix As String
            Get
                If String.IsNullOrEmpty(TRANS_NO) Then
                    Return String.Empty
                End If

                Dim match = System.Text.RegularExpressions.Regex.Match(TRANS_NO, "^[A-Za-z]+\d+")
                If match.Success Then
                    Dim full = match.Value
                    Return full.Substring(0, Math.Min(full.Length, 5))
                End If

                Return String.Empty
            End Get
        End Property
    End Class
End Namespace
