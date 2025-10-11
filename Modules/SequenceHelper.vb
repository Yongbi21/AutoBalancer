Imports System.Text.RegularExpressions
Imports AutoBalancer.Models
Imports Models

Public Module SequenceHelper

    ' 🔹 Detect gaps and mismatches between PostTrans, TransPay, and Control
    Public Function DetectGaps(postTrans As List(Of POSTRANS), transPay As List(Of TRANSPAY), control As List(Of CONTROL)) As List(Of String)
        Dim logs As New List(Of String)

        If postTrans.Count = 0 OrElse transPay.Count = 0 OrElse control.Count = 0 Then
            logs.Add("⚠️ One or more tables are empty — cannot perform gap detection.")
            Return logs
        End If

        Dim postIds = postTrans.Select(Function(x) x.POSTRANS).OrderBy(Function(x) x).ToList()
        Dim payIds = transPay.Select(Function(x) x.TRANSPAY).OrderBy(Function(x) x).ToList()
        Dim controlNos = control.Select(Function(x) x.CONTROL_NO).OrderBy(Function(x) x).ToList()

        logs.AddRange(CheckMissing(postIds, payIds, "POSTRANS", "TRANSPAY"))
        logs.AddRange(CheckMissing(payIds, postIds, "TRANSPAY", "POSTRANS"))

        ' Missing ControlNos
        For Each pt In postTrans
            If Not controlNos.Contains(pt.CONTROL_NO) Then
                logs.Add($"⚠️ Missing CONTROL_NO {pt.CONTROL_NO} (from POSTRANS)")
            End If
        Next

        ' Duplicates check
        Dim duplicateCtrl = controlNos.GroupBy(Function(x) x).Where(Function(g) g.Count() > 1).Select(Function(g) g.Key).ToList()
        For Each dup In duplicateCtrl
            logs.Add($"⚠️ Duplicate CONTROL_NO detected: {dup}")
        Next

        Return logs
    End Function


    ' Helper for missing ID detection
    Private Function CheckMissing(source As List(Of Integer), target As List(Of Integer), srcName As String, tgtName As String) As List(Of String)
        Dim list As New List(Of String)
        For Each id In source
            If Not target.Contains(id) Then
                list.Add($"⚠️ {srcName} ID {id} missing in {tgtName}")
            End If
        Next
        Return list
    End Function


    ' 🔹 Correct detected gaps by syncing IDs and ControlNos
    Public Sub CorrectGaps(postTrans As List(Of POSTRANS), transPay As List(Of TRANSPAY), control As List(Of CONTROL), logs As List(Of String))
        If postTrans.Count = 0 OrElse transPay.Count = 0 Then
            logs.Add("⚠️ Cannot correct gaps because one or more lists are empty.")
            Exit Sub
        End If

        Dim postIds = postTrans.Select(Function(x) x.POSTRANS).ToList()
        Dim payIds = transPay.Select(Function(x) x.TRANSPAY).ToList()

        ' ✅ Add missing TransPay entries
        For Each pt In postTrans
            If Not payIds.Contains(pt.POSTRANS) Then
                Dim tp As New TRANSPAY With {
                    .TRANSPAY = pt.POSTRANS,
                    .CONTROL_NO = pt.CONTROL_NO,
                    .TRANS_NO = pt.TRANS_NO
                }
                transPay.Add(tp)
                logs.Add($"🧩 Added missing TRANSPAY for ID {pt.POSTRANS}")
            End If
        Next

        ' ✅ Add missing PostTrans entries
        For Each tp In transPay
            If Not postIds.Contains(tp.TRANSPAY) Then
                Dim pt As New POSTRANS With {
                    .POSTRANS = tp.TRANSPAY,
                    .CONTROL_NO = tp.CONTROL_NO,
                    .TRANS_NO = tp.TRANS_NO
                }
                postTrans.Add(pt)
                logs.Add($"🧩 Added missing POSTRANS for ID {tp.TRANSPAY}")
            End If
        Next

        ' ✅ Add missing ControlNos
        Dim controlNos = control.Select(Function(x) x.CONTROL_NO).ToList()
        For Each pt In postTrans
            If Not controlNos.Contains(pt.CONTROL_NO) Then
                control.Add(New CONTROL With {.CONTROL_NO = pt.CONTROL_NO})
                logs.Add($"🧩 Added missing CONTROL_NO {pt.CONTROL_NO}")
            End If
        Next

        ' ✅ Remove duplicates in Control
        Dim duplicates = control.GroupBy(Function(x) x.CONTROL_NO).
                                 Where(Function(g) g.Count() > 1).
                                 Select(Function(g) g.Key).ToList()
        For Each dup In duplicates
            Dim first = control.First(Function(x) x.CONTROL_NO = dup)
            control.RemoveAll(Function(x) x.CONTROL_NO = dup AndAlso x IsNot first)
            logs.Add($"🧹 Removed duplicate CONTROL_NO {dup}")
        Next
    End Sub


    ' 🔹 Renumber sequences to remove gaps (keeps IDs continuous)
    Public Sub RenumberForContinuity(postTrans As List(Of POSTRANS), transPay As List(Of TRANSPAY), control As List(Of CONTROL), logs As List(Of String))
        If postTrans.Count = 0 Then
            logs.Add("⚠️ No records in POSTRANS to renumber.")
            Exit Sub
        End If

        Dim sorted = postTrans.OrderBy(Function(x) x.POSTRANS).ToList()
        Dim newId As Integer = sorted.First().POSTRANS

        For Each pt In sorted
            Dim oldId = pt.POSTRANS
            If oldId <> newId Then
                pt.POSTRANS = newId

                ' Update corresponding TRANSPAY
                Dim tp = transPay.FirstOrDefault(Function(x) x.TRANSPAY = oldId)
                If tp IsNot Nothing Then tp.TRANSPAY = newId

                logs.Add($"🔢 Renumbered {oldId} → {newId}")
            End If
            newId += 1
        Next
    End Sub


    ' 🔹 Verify final sequences are now continuous
    Public Function VerifySequences(postTrans As List(Of POSTRANS), transPay As List(Of TRANSPAY)) As Boolean
        If postTrans.Count = 0 OrElse transPay.Count = 0 Then Return False

        Dim postIds = postTrans.Select(Function(x) x.POSTRANS).OrderBy(Function(x) x).ToList()
        Dim payIds = transPay.Select(Function(x) x.TRANSPAY).OrderBy(Function(x) x).ToList()

        Return IsContinuous(postIds) AndAlso IsContinuous(payIds)
    End Function

    Private Function IsContinuous(ids As List(Of Integer)) As Boolean
        If ids.Count = 0 Then Return False
        For i = 1 To ids.Count - 1
            If ids(i) - ids(i - 1) <> 1 Then Return False
        Next
        Return True
    End Function

End Module
