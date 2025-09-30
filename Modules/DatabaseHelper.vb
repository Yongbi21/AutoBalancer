Imports System.Data.OleDb
Imports AutoBalancer.Models

Namespace Modules
    Public Class DatabaseHelper

        Public Shared Function OpenDatabase(dbPath As String) As OleDbConnection
            Dim connectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & dbPath & ";Persist Security Info=False;"
            Dim connection As New OleDbConnection(connectionString)
            connection.Open()
            Return connection
        End Function

        Public Shared Function ReadControlNo(connection As OleDbConnection) As List(Of ControlNo)
            Dim controlNos As New List(Of ControlNo)
            Dim query As String = "SELECT Control_No, Audit FROM ControlNo"
            Using command As New OleDbCommand(query, connection)
                Using reader As OleDbDataReader = command.ExecuteReader()
                    While reader.Read()
                        controlNos.Add(New ControlNo() With {
                            .Control_No = reader("Control_No").ToString(),
                            .Audit = If(reader("Audit") Is DBNull.Value, Nothing, reader("Audit").ToString())
                        })
                    End While
                End Using
            End Using
            Return controlNos
        End Function

        Public Shared Function ReadPostTrans(connection As OleDbConnection) As List(Of PostTrans)
            Dim postTransList As New List(Of PostTrans)
            Dim query As String = "SELECT posttrans_id, Control_No, TransactionNo, InvoiceNo, Amount, Audit FROM PostTrans"
            Using command As New OleDbCommand(query, connection)
                Using reader As OleDbDataReader = command.ExecuteReader()
                    While reader.Read()
                        postTransList.Add(New PostTrans() With {
                            .posttrans_id = CInt(reader("posttrans_id")),
                            .Control_No = reader("Control_No").ToString(),
                            .TransactionNo = reader("TransactionNo").ToString(),
                            .InvoiceNo = reader("InvoiceNo").ToString(),
                            .Amount = CDec(reader("Amount")),
                            .Audit = If(reader("Audit") Is DBNull.Value, Nothing, reader("Audit").ToString())
                        })
                    End While
                End Using
            End Using
            Return postTransList
        End Function

        Public Shared Function ReadTranspay(connection As OleDbConnection) As List(Of Transpay)
            Dim transpayList As New List(Of Transpay)
            Dim query As String = "SELECT transpay_id, Control_No, TransactionNo, Amount, Audit FROM TransPay"
            Using command As New OleDbCommand(query, connection)
                Using reader As OleDbDataReader = command.ExecuteReader()
                    While reader.Read()
                        transpayList.Add(New Transpay() With {
                            .transpay_id = CInt(reader("transpay_id")),
                            .Control_No = reader("Control_No").ToString(),
                            .TransactionNo = reader("TransactionNo").ToString(),
                            .Amount = CDec(reader("Amount")),
                            .Audit = If(reader("Audit") Is DBNull.Value, Nothing, reader("Audit").ToString())
                        })
                    End While
                End Using
            End Using
            Return transpayList
        End Function

        Public Shared Function ScanForMismatches(postTransList As List(Of PostTrans), transpayList As List(Of Transpay), controlNoList As List(Of ControlNo)) As List(Of String)
            Dim mismatches As New List(Of String)
            ' Placeholder for actual mismatch scanning logic based on rules
            mismatches.Add("Placeholder Mismatch: PostTrans ID 123 not found in TransPay")
            mismatches.Add("Placeholder Mismatch: TransPay ID 456 not found in PostTrans")
            Return mismatches
        End Function

        ' Placeholder for methods to write data back to the database
        Public Shared Sub UpdateControlNo(connection As OleDbConnection, controlNo As ControlNo)
            ' Implement update logic
            System.Threading.Thread.Sleep(100) ' Simulate work
        End Sub

        Public Shared Sub DeleteTranspay(connection As OleDbConnection, transpayId As Integer)
            ' Implement delete logic
            System.Threading.Thread.Sleep(100) ' Simulate work
        End Sub

        Public Shared Sub InsertPostTrans(connection As OleDbConnection, postTrans As PostTrans)
            ' Implement insert logic
            System.Threading.Thread.Sleep(100) ' Simulate work
        End Sub

        Public Shared Sub UpdatePostTrans(connection As OleDbConnection, postTrans As PostTrans)
            ' Implement update logic
            System.Threading.Thread.Sleep(100) ' Simulate work
        End Sub

    End Class
End Namespace