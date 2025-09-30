Imports System
Imports System.Windows.Forms

Public Class frmMain



    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "Access Databases (*.mdb;*.accdb)|*.mdb;*.accdb|All files (*.*)|*.*"
            openFileDialog.Title = "Select MS Access Database"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                textboxSearch.Text = openFileDialog.FileName
            End If
        End Using
    End Sub

    Private Sub tsProcess_Click(sender As Object, e As EventArgs) Handles tsProcess.Click
        Dim steps As String() = {
        "Creating Backup...",
        "Checking PostTrans...",
        "Checking TransPay...",
        "Validating ControlNo...",
        "Finalizing..."
    }

        For i As Integer = 0 To 100
            ProgressBarX1.Value = i

            ' Map i into steps (example: 20% per step)
            Dim stepIndex As Integer = Math.Min(i \ 20, steps.Length - 1)
            ProgressBarX1.CustomText = steps(stepIndex) & " " & i.ToString() & "%"

            Application.DoEvents()
            System.Threading.Thread.Sleep(50)
        Next

        ProgressBarX1.Value = 100
        ProgressBarX1.CustomText = "100%"
        Application.DoEvents()
        System.Threading.Thread.Sleep(50)

        ProgressBarX1.CustomText = "Done!"
        tsProcess.Text = "Run Auto Balance"
    End Sub


    Private Sub tsClose_Click(sender As Object, e As EventArgs) Handles tsClose.Click
        Me.Close()
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub



End Class