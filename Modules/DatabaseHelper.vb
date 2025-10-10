Imports System.Data.OleDb
Imports AutoBalancer.Models
Imports System.Linq

Namespace Modules
    Public Class DatabaseHelper

        Public Shared Function OpenDatabase(dbPath As String) As OleDbConnection
            Dim connectionString As String =
                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dbPath & ";Persist Security Info=False;"
            Dim connection As New OleDbConnection(connectionString)
            connection.Open()
            Return connection
        End Function

        '-------------------------------------------
        ' Read CONTROL table
        '-------------------------------------------
        Public Shared Function ReadControlNo(connection As OleDbConnection) As List(Of CONTROL)
            Dim controlNos As New List(Of CONTROL)
            Dim query As String = "SELECT CONTROL_NO FROM CONTROL"
            Using command As New OleDbCommand(query, connection)
                Using reader As OleDbDataReader = command.ExecuteReader()
                    While reader.Read()
                        controlNos.Add(New CONTROL() With {
                            .Control_No = reader("CONTROL_NO").ToString()
                        })
                    End While
                End Using
            End Using
            Return controlNos
        End Function

        '-------------------------------------------
        ' Read POSTRANS
        '-------------------------------------------
        Public Shared Function ReadPostTrans(connection As OleDbConnection) As List(Of POSTRANS)
            Dim POSTRANS As New List(Of POSTRANS)
            Dim query As String = "SELECT [CONTROL_NO], [TRANS_NO], [AUDIT], [Z] FROM [POSTRANS]"

            Try
                Using command As New OleDbCommand(query, connection)
                    Using reader As OleDbDataReader = command.ExecuteReader()
                        Dim counter As Integer = 1

                        While reader.Read()
                            Dim auditValue As String = reader("AUDIT").ToString().Trim().ToUpperInvariant()
                            Dim zValue As String = reader("Z").ToString().Trim().ToUpperInvariant()

                            ' ✅ Ignore rows where AUDIT = "Y" or Z is not empty
                            If auditValue <> "Y" AndAlso zValue = "" Then
                                POSTRANS.Add(New POSTRANS() With {
                            .POSTRANS = counter,
                            .CONTROL_NO = reader("CONTROL_NO").ToString(),
                            .TRANS_NO = reader("TRANS_NO").ToString()
                        })
                                counter += 1
                            End If
                        End While
                    End Using
                End Using

            Catch ex As Exception
                Throw New Exception("Error reading POSTRANS table: " & ex.Message)
            End Try

            Return POSTRANS
        End Function



        '-------------------------------------------
        ' Read TRANSPAY
        '-------------------------------------------
        Public Shared Function ReadTranspay(connection As OleDbConnection) As List(Of TRANSPAY)
            Dim transpayList As New List(Of TRANSPAY)
            Dim query As String = "SELECT [CONTROL_NO], [TRANS_NO], [AUDIT], [Z] FROM [TRANSPAY]"

            Try
                Using command As New OleDbCommand(query, connection)
                    Using reader As OleDbDataReader = command.ExecuteReader()
                        Dim counter As Integer = 1

                        While reader.Read()
                            Dim auditValue As String = reader("AUDIT").ToString().Trim().ToUpperInvariant()
                            Dim zValue As String = reader("Z").ToString().Trim().ToUpperInvariant()

                            If auditValue <> "Y" AndAlso zValue = "" Then
                                transpayList.Add(New TRANSPAY() With {
                            .TRANSPAY = counter,
                            .CONTROL_NO = reader("CONTROL_NO").ToString(),
                            .TRANS_NO = reader("TRANS_NO").ToString()
                        })
                                counter += 1
                            End If
                        End While
                    End Using
                End Using

            Catch ex As Exception
                Throw New Exception("Error reading TRANSPAY table: " & ex.Message)
            End Try

            Return transpayList
        End Function

        '-------------------------------------------
        ' Mismatch Scanner (prefix + transaction check)
        '-------------------------------------------
        Public Shared Function ScanForMismatches(postTransList As List(Of POSTRANS),
                                                 transpayList As List(Of TRANSPAY),
                                                 controlNoList As List(Of CONTROL)) As List(Of String)

            Dim mismatches As New List(Of String)

            ' Compare by prefix + Control_No
            For Each p In postTransList
                Dim hasMatch = transpayList.Any(Function(t) t.Control_No = p.Control_No AndAlso t.CounterPrefix = p.CounterPrefix)
                If Not hasMatch Then
                    mismatches.Add(String.Format("Missing in TransPay → Counter={0}, ControlNo={1}, TransNo={2}",
                                                 p.CounterPrefix, p.CONTROL_NO, p.TRANS_NO))
                End If
            Next

            For Each t In transpayList
                Dim hasMatch = postTransList.Any(Function(p) p.Control_No = t.Control_No AndAlso p.CounterPrefix = t.CounterPrefix)
                If Not hasMatch Then
                    mismatches.Add(String.Format("Missing in PostTrans → Counter={0}, ControlNo={1}, TransNo={2}",
                                                 t.CounterPrefix, t.CONTROL_NO, t.TRANS_NO))
                End If
            Next

            Return mismatches
        End Function

    End Class
End Namespace
