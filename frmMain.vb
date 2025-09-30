Imports System.Diagnostics
Imports System.IO
Imports System
Imports System.Windows.Forms
Imports AutoBalancer.Backup
Imports AutoBalancer.Logs
Imports AutoBalancer.Modules

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
        tsProcess.Text = "Processing..."
        ProgressBarX1.Value = 0
        ProgressBarX1.CustomText = "Starting..."
        Application.DoEvents()

        Dim dbPath As String = textboxSearch.Text
        If String.IsNullOrEmpty(dbPath) OrElse Not System.IO.File.Exists(dbPath) Then
            MessageBox.Show("Please select a valid MS Access database file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            tsProcess.Text = "Run Auto Balance"
            ProgressBarX1.CustomText = "Error!"
            Return
        End If

        Dim connection As System.Data.OleDb.OleDbConnection = Nothing

        Try
            ' Step 1: Creating Backup...
            Logger.LogProgress("Creating Backup...")
            Dim backupFilePath As String = BackupService.CreateBackup(dbPath)
            Logger.LogProgress("Backup created at: " & backupFilePath)
            UpdateProgressBar(10, "Creating Backup...")

            ' Step 2: Open Database
            Logger.LogProgress("Opening database connection...")
            connection = DatabaseHelper.OpenDatabase(dbPath)
            Logger.LogProgress("Database connection opened.")
            labelConnected.Text = "Connected"
            UpdateProgressBar(20, "Reading Data...")

            ' Step 3: Read Data
            Logger.LogProgress("Reading ControlNo data...")
            Dim controlNoList As List(Of Models.ControlNo) = DatabaseHelper.ReadControlNo(connection)
            Logger.LogProgress("Reading PostTrans data...")
            Dim postTransList As List(Of Models.PostTrans) = DatabaseHelper.ReadPostTrans(connection)
            Logger.LogProgress("Reading TransPay data...")
            Dim transpayList As List(Of Models.Transpay) = DatabaseHelper.ReadTranspay(connection)
            UpdateProgressBar(40, "Scanning for Mismatches...")

            ' Step 4: Scan for Mismatches
            Logger.LogProgress("Scanning for mismatches...")
            Dim mismatches As List(Of String) = DatabaseHelper.ScanForMismatches(postTransList, transpayList, controlNoList)
            If mismatches.Any() Then
                Logger.LogProgress("Detected " & mismatches.Count.ToString() & " mismatches.")
                For Each mismatch As String In mismatches
                    Logger.LogProgress("- " & mismatch)
                Next
            Else
                Logger.LogProgress("No mismatches detected.")
            End If
            UpdateProgressBar(60, "Handling Sequences...")

            ' Step 5: Sequence Handling
            Logger.LogProgress("Collecting sequences...")
            Dim sequences = SequenceHelper.CollectSequences(connection)
            Logger.LogProgress("Detecting gaps...")
            Dim detectedGaps As List(Of String) = SequenceHelper.DetectGaps(postTransList, transpayList, controlNoList)
            If detectedGaps.Any() Then
                Logger.LogProgress("Detected " & detectedGaps.Count.ToString() & " gaps.")
                For Each gap As String In detectedGaps
                    Logger.LogProgress("- " & gap)
                Next
                Logger.LogProgress("Correcting gaps...")
                SequenceHelper.CorrectGaps(connection, detectedGaps)
            Else
                Logger.LogProgress("No gaps detected.")
            End If
            Logger.LogProgress("Renumbering for continuity...")
            SequenceHelper.RenumberForContinuity(connection, False) ' Assuming auditAware is false for now
            Logger.LogProgress("Verifying sequences...")
            Dim verificationMismatches As List(Of String) = SequenceHelper.VerifySequences(connection)
            If verificationMismatches.Any() Then
                Logger.LogProgress("Sequence verification found " & verificationMismatches.Count.ToString() & " issues.")
                For Each vm As String In verificationMismatches
                    Logger.LogProgress("- " & vm)
                Next
            Else
                Logger.LogProgress("Sequence verification passed.")
            End If
            UpdateProgressBar(80, "Applying Balancing Rules...")

            ' Step 6: Apply Balancing Rules (Placeholder calls)
            Logger.LogProgress("Applying balancing rules...")
            ' Example: Check a PostTrans balance
            If postTransList.Any() Then
                Dim firstPostTrans As Models.PostTrans = postTransList.First()
                If Not ValidationHelper.CheckPostTransBalance(firstPostTrans, transpayList) Then
                    Logger.LogProgress("PostTrans " & firstPostTrans.posttrans_id.ToString() & " is not balanced.")
                    ' Implement correction logic here
                End If
            End If
            UpdateProgressBar(90, "Finalizing...")

            ' Step 7: Save Results (Placeholder)
            Logger.LogProgress("Saving fixed MDB...")
            ' This would involve writing updated data back to the database
            ' For now, just simulate
            System.Threading.Thread.Sleep(1000)
            UpdateProgressBar(100, "Completed!")

            ProgressBarX1.CustomText = "Done!"
            Logger.LogProgress("Auto Balance process completed successfully.")
            MessageBox.Show("Auto Balance process completed successfully! Check logs for details.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            ProgressBarX1.CustomText = "Error!"
            Logger.LogError("Auto Balance process failed: " & ex.Message, ex)
            MessageBox.Show("An error occurred during the Auto Balance process: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            labelConnected.Text = "Not Connected"
        Finally
            If connection IsNot Nothing AndAlso connection.State = ConnectionState.Open Then
                connection.Close()
                Logger.LogProgress("Database connection closed.")
            End If
            labelConnected.Text = "Not Connected"
            tsProcess.Text = "Run Auto Balance"
        End Try
    End Sub

    Private Sub UpdateProgressBar(value As Integer, text As String)
        ProgressBarX1.Value = value
        ProgressBarX1.CustomText = text & " " & value.ToString() & "%"
        Application.DoEvents()
        System.Threading.Thread.Sleep(50)
    End Sub


    Private Sub tsClose_Click(sender As Object, e As EventArgs) Handles tsClose.Click
        Me.Close()
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        labelConnected.Text = "Not Connected"
    End Sub

    Private Sub btn_Click(sender As Object, e As EventArgs) Handles btn.Click
        Try
            Process.Start(Logger.LogFolderPath)
        Catch ex As Exception
            MessageBox.Show("Could not open log folder: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logger.LogError("Failed to open log folder: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub btnViewLogs_Click(sender As Object, e As EventArgs) Handles btnViewLogs.Click
        Try
            Dim dbPath As String = textboxSearch.Text
            If String.IsNullOrEmpty(dbPath) Then
                MessageBox.Show("Please select a database first to determine backup folder location.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If
            Dim backupFolderPath As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(dbPath), "Backup")
            Process.Start(backupFolderPath)
        Catch ex As Exception
            MessageBox.Show("Could not open backup folder: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logger.LogError("Failed to open backup folder: " & ex.Message, ex)
        End Try
    End Sub

End Class