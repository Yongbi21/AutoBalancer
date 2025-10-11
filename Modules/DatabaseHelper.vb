Imports System.Data.OleDb
Imports AutoBalancer.Models
Imports System.Linq

Namespace Modules
    Public Class DatabaseHelper


        '-------------------------------------------
        ' Z - Counter cutting, Audit - Manager reading, X - End of store operation all audit.
        ' Audit = Y means transaction is done by the manager but not the end of store operation.
        ' Control No = If it skips just edit the control no to the last number in sequence.
        ' Transactin No = If it skips just edit the transaction no to the last number in sequence.
        '-------------------------------------------


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
            Dim controlList As New List(Of CONTROL)
            Dim query As String = "SELECT CONTROL_NO FROM CONTROL"
            Using command As New OleDbCommand(query, connection)
                Using reader As OleDbDataReader = command.ExecuteReader()
                    While reader.Read()
                        controlList.Add(New CONTROL() With {
                    .CONTROL_NO = reader("CONTROL_NO").ToString()
                })
                    End While
                End Using
            End Using
            Return controlList
        End Function


        '-------------------------------------------
        ' Read POSTRANS
        '-------------------------------------------
        Public Shared Function ReadPostrans(connection As OleDbConnection) As List(Of POSTRANS)
            Dim postTransList As New List(Of POSTRANS)

            Try
                ' --- diagnostic: show available tables
                Dim schema = connection.GetSchema("Tables")
                Dim tables = schema.AsEnumerable().Select(Function(r) r("TABLE_NAME").ToString()).ToList()
                If Not tables.Any(Function(n) n.ToUpper() = "POSTRANS") Then
                    Dim msg = "POSTRANS table not found. Available tables:" & vbCrLf & String.Join(vbCrLf, tables)
                    Throw New Exception(msg)
                End If

                Dim query As String = "SELECT [CONTROL_NO], [TRANS_NO], [AUDIT], [Z] FROM [POSTRANS]"
                Using command As New OleDbCommand(query, connection)
                    Using reader As OleDbDataReader = command.ExecuteReader()
                        Dim counter As Integer = 1
                        While reader.Read()
                            Dim auditValue As String = reader("AUDIT").ToString().Trim().ToUpperInvariant()
                            Dim zValue As String = reader("Z").ToString().Trim().ToUpperInvariant()
                            Dim controlNo = reader("CONTROL_NO").ToString()

                            'If auditValue <> "Y" AndAlso zValue = "" Then
                            postTransList.Add(New POSTRANS() With {
                            .POSTRANS = counter,
                            .CONTROL_NO = reader("CONTROL_NO").ToString(),
                            .TRANS_NO = reader("TRANS_NO").ToString()
                        })
                            counter += 1
                            'End If
                        End While
                    End Using
                End Using

            Catch ex As Exception
                Throw New Exception("Error reading POSTRANS table: " & ex.Message)
            End Try

            Return postTransList
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
                            Dim controlNo = reader("CONTROL_NO").ToString()

                            'If auditValue <> "Y" AndAlso zValue = "" Then
                            transpayList.Add(New TRANSPAY() With {
                            .TRANSPAY = counter,
                            .CONTROL_NO = reader("CONTROL_NO").ToString(),
                            .TRANS_NO = reader("TRANS_NO").ToString()
                            })
                            counter += 1
                            'End If
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
        '-------------------------------------------`
        Public Shared Function ScanForMismatches(postTransList As List(Of POSTRANS),
                                                 transpayList As List(Of TRANSPAY),
                                                 controlNoList As List(Of CONTROL)) As List(Of String)
            Dim mismatches As New List(Of String)

            For Each p In postTransList
                Dim hasMatch = transpayList.Any(Function(t) t.CONTROL_NO = p.CONTROL_NO AndAlso t.CounterPrefix = p.CounterPrefix)
                If Not hasMatch Then
                    mismatches.Add(String.Format("Missing in TransPay → Counter={0}, ControlNo={1}, TransNo={2}",
                                                 p.CounterPrefix, p.CONTROL_NO, p.TRANS_NO))
                End If
            Next

            For Each t In transpayList
                Dim hasMatch = postTransList.Any(Function(p) p.CONTROL_NO = t.CONTROL_NO AndAlso p.CounterPrefix = t.CounterPrefix)
                If Not hasMatch Then
                    mismatches.Add(String.Format("Missing in PostTrans → Counter={0}, ControlNo={1}, TransNo={2}",
                                                 t.CounterPrefix, t.CONTROL_NO, t.TRANS_NO))
                End If
            Next

            Return mismatches
        End Function

        '-------------------------------------------
        ' Write corrected POSTRANS and TRANSPAY to DB
        '-------------------------------------------

        Public Shared Sub ReconcileAndWriteChanges(connection As OleDbConnection,
                                           postTransList As List(Of POSTRANS),
                                           transpayList As List(Of TRANSPAY),
                                           controlList As List(Of CONTROL))

            Using tx = connection.BeginTransaction()
                Try
                    ' Track used CONTROL_NO to avoid duplicates
                    Dim usedControlNos As New List(Of String)
                    For Each c In controlList
                        usedControlNos.Add(c.CONTROL_NO)
                    Next

                    ' -----------------------------
                    ' 1️⃣ Handle POSTRANS not balanced
                    ' -----------------------------
                    For Each p In postTransList
                        Dim matchingTrans As TRANSPAY = Nothing
                        For Each t In transpayList
                            If t.CONTROL_NO = p.CONTROL_NO AndAlso t.TRANS_NO = p.TRANS_NO Then
                                matchingTrans = t
                                Exit For
                            End If
                        Next

                        If matchingTrans Is Nothing Then
                            ' Delete any TRANSPAY conflicting with this POSTRANS
                            Dim deleteTrans As String = "DELETE FROM [TRANSPAY] WHERE [CONTROL_NO] = ? OR [TRANS_NO] = ?"
                            Using cmdDel As New OleDbCommand(deleteTrans, connection, tx)
                                cmdDel.Parameters.AddWithValue("@controlNo", p.CONTROL_NO)
                                cmdDel.Parameters.AddWithValue("@transNo", p.TRANS_NO)
                                cmdDel.ExecuteNonQuery()
                            End Using

                            ' Ensure CONTROL_NO sequence is valid
                            Dim newControlNo As String = p.CONTROL_NO
                            While usedControlNos.Contains(newControlNo)
                                newControlNo = (CInt(newControlNo) + 1).ToString()
                            End While
                            usedControlNos.Add(newControlNo)

                            ' Update POSTRANS CONTROL_NO if needed
                            If newControlNo <> p.CONTROL_NO Then
                                Dim updatePost As String = "UPDATE [POSTRANS] SET [CONTROL_NO] = ? WHERE [CONTROL_NO] = ? AND [TRANS_NO] = ?"
                                Using cmdUpd As New OleDbCommand(updatePost, connection, tx)
                                    cmdUpd.Parameters.AddWithValue("@newControlNo", newControlNo)
                                    cmdUpd.Parameters.AddWithValue("@oldControlNo", p.CONTROL_NO)
                                    cmdUpd.Parameters.AddWithValue("@transNo", p.TRANS_NO)
                                    cmdUpd.ExecuteNonQuery()
                                End Using
                                p.CONTROL_NO = newControlNo
                            End If
                        End If
                    Next

                    ' -----------------------------
                    ' 2️⃣ Handle TRANSPAY not balanced
                    ' -----------------------------
                    For Each t In transpayList
                        Dim matchingPost As POSTRANS = Nothing
                        For Each p In postTransList
                            If p.CONTROL_NO = t.CONTROL_NO AndAlso p.TRANS_NO = t.TRANS_NO Then
                                matchingPost = p
                                Exit For
                            End If
                        Next

                        If matchingPost Is Nothing Then
                            ' Fix CONTROL_NO based on POSTRANS if available
                            Dim newControlNo As String = t.CONTROL_NO
                            Dim foundPost = postTransList.FirstOrDefault(Function(p) p.CounterPrefix = t.CounterPrefix)
                            If foundPost IsNot Nothing Then
                                newControlNo = foundPost.CONTROL_NO
                            Else
                                ' ensure sequence
                                While usedControlNos.Contains(newControlNo)
                                    newControlNo = (CInt(newControlNo) + 1).ToString()
                                End While
                            End If
                            usedControlNos.Add(newControlNo)

                            Dim updateTrans As String = "UPDATE [TRANSPAY] SET [CONTROL_NO] = ?, [TRANS_NO] = ? WHERE [CONTROL_NO] = ? AND [TRANS_NO] = ?"
                            Using cmdUpd As New OleDbCommand(updateTrans, connection, tx)
                                cmdUpd.Parameters.AddWithValue("@newControlNo", newControlNo)
                                cmdUpd.Parameters.AddWithValue("@newTransNo", t.TRANS_NO)
                                cmdUpd.Parameters.AddWithValue("@oldControlNo", t.CONTROL_NO)
                                cmdUpd.Parameters.AddWithValue("@oldTransNo", t.TRANS_NO)
                                cmdUpd.ExecuteNonQuery()
                            End Using

                            ' Delete POSTRANS for manual POS re-entry
                            Dim deletePost As String = "DELETE FROM [POSTRANS] WHERE [CONTROL_NO] = ? AND [TRANS_NO] = ?"
                            Using cmdDel As New OleDbCommand(deletePost, connection, tx)
                                cmdDel.Parameters.AddWithValue("@controlNo", t.CONTROL_NO)
                                cmdDel.Parameters.AddWithValue("@transNo", t.TRANS_NO)
                                cmdDel.ExecuteNonQuery()
                            End Using
                        End If
                    Next

                    ' -----------------------------
                    ' 3️⃣ Both tables balanced but sequences differ
                    ' -----------------------------
                    For Each t In transpayList
                        Dim pMatch = postTransList.FirstOrDefault(Function(p) p.TRANS_NO = t.TRANS_NO)
                        If pMatch IsNot Nothing AndAlso pMatch.CONTROL_NO <> t.CONTROL_NO Then
                            ' Adjust TRANSPAY to match POSTRANS
                            Dim updateTrans As String = "UPDATE [TRANSPAY] SET [CONTROL_NO] = ? WHERE [CONTROL_NO] = ? AND [TRANS_NO] = ?"
                            Using cmdUpd As New OleDbCommand(updateTrans, connection, tx)
                                cmdUpd.Parameters.AddWithValue("@newControlNo", pMatch.CONTROL_NO)
                                cmdUpd.Parameters.AddWithValue("@oldControlNo", t.CONTROL_NO)
                                cmdUpd.Parameters.AddWithValue("@transNo", t.TRANS_NO)
                                cmdUpd.ExecuteNonQuery()
                            End Using
                        End If
                    Next

                    ' -----------------------------
                    ' 4️⃣ Check TRANS_NO if all CONTROL_NO balanced
                    ' -----------------------------
                    For Each p In postTransList
                        Dim t = transpayList.FirstOrDefault(Function(x) x.CONTROL_NO = p.CONTROL_NO)
                        If t IsNot Nothing AndAlso t.TRANS_NO <> p.TRANS_NO Then
                            Dim updateTransNo As String = "UPDATE [TRANSPAY] SET [TRANS_NO] = ? WHERE [CONTROL_NO] = ?"
                            Using cmdUpd As New OleDbCommand(updateTransNo, connection, tx)
                                cmdUpd.Parameters.AddWithValue("@newTransNo", p.TRANS_NO)
                                cmdUpd.Parameters.AddWithValue("@controlNo", p.CONTROL_NO)
                                cmdUpd.ExecuteNonQuery()
                            End Using
                        End If
                    Next

                    tx.Commit()
                    Console.WriteLine("✅ Reconciliation complete, all changes committed.")

                Catch ex As Exception
                    tx.Rollback()
                    Throw New Exception("❌ Error reconciling POSTRANS and TRANSPAY: " & ex.Message)
                End Try
            End Using
        End Sub




    End Class


End Namespace
