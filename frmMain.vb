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
        If String.IsNullOrEmpty(dbPath) OrElse Not File.Exists(dbPath) Then
            MessageBox.Show("Please select a valid MS Access database file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            tsProcess.Text = "Run Auto Balance"
            ProgressBarX1.CustomText = "Error!"
            Return
        End If

        Dim connection As OleDb.OleDbConnection = Nothing

        Try
            ' ✅ Step 1: Backup
            Logger.LogProgress("Creating Backup...")
            Dim backupFilePath As String = BackupService.CreateBackup(dbPath)
            Logger.LogProgress("Backup created at: " & backupFilePath)
            UpdateProgressBar(10, "Creating Backup...")

            ' ✅ Step 2: Open Database
            Logger.LogProgress("Opening database connection...")
            connection = DatabaseHelper.OpenDatabase(dbPath)
            Logger.LogProgress("Database connection opened.")
            labelConnected.Text = "Connected"
            UpdateProgressBar(20, "Reading Data...")

            ' ✅ Step 3: Read Data
            Logger.LogProgress("Reading CONTROL table...")
            Dim controlNoList As List(Of Models.CONTROL) = DatabaseHelper.ReadControlNo(connection)

            Logger.LogProgress("Reading POSTRANS table...")
            Dim postTransList As List(Of Models.POSTRANS) = DatabaseHelper.ReadPostrans(connection)

            Logger.LogProgress("Reading TRANSPAY table...")
            Dim transpayList As List(Of Models.TRANSPAY) = DatabaseHelper.ReadTranspay(connection)
            UpdateProgressBar(40, "Scanning for Mismatches...")

            ' ✅ Step 4: Scan for mismatches
            Logger.LogProgress("Scanning for mismatches...")
            Dim mismatches As List(Of String) = DatabaseHelper.ScanForMismatches(postTransList, transpayList, controlNoList)

            If mismatches.Any() Then
                Logger.LogProgress($"Detected {mismatches.Count} mismatches:")
                For Each mismatch As String In mismatches
                    Logger.LogProgress("- " & mismatch)
                Next
            Else
                Logger.LogProgress("No mismatches detected.")
            End If
            UpdateProgressBar(60, "Handling Sequences...")

            ' ✅ Step 5: Sequence Handling
            If postTransList.Count = 0 OrElse transpayList.Count = 0 OrElse controlNoList.Count = 0 Then
                Logger.LogProgress("⚠️ One or more tables are empty. Skipping sequence handling.")
                Logger.LogProgress($"DEBUG: POSTRANS count = {postTransList.Count}")
                Logger.LogProgress($"DEBUG: TRANSPAY count = {transpayList.Count}")
                Logger.LogProgress($"DEBUG: CONTROL count = {controlNoList.Count}")
            Else
                Logger.LogProgress("Collecting sequences...")
                Dim detectedGaps As List(Of String) = SequenceHelper.DetectGaps(postTransList, transpayList, controlNoList)

                If detectedGaps.Any() Then
                    Logger.LogProgress($"Detected {detectedGaps.Count} gaps:")
                    For Each gap As String In detectedGaps
                        Logger.LogProgress("- " & gap)
                    Next
                    Logger.LogProgress("Correcting gaps...")
                    SequenceHelper.CorrectGaps(postTransList, transpayList, controlNoList, detectedGaps)
                Else
                    Logger.LogProgress("No gaps detected.")
                End If

                Logger.LogProgress("Renumbering for continuity...")
                SequenceHelper.RenumberForContinuity(postTransList, transpayList, controlNoList, detectedGaps)

                Logger.LogProgress("Verifying sequences...")
                If SequenceHelper.VerifySequences(postTransList, transpayList) Then
                    Logger.LogProgress("Sequence verification passed.")
                Else
                    Logger.LogProgress("⚠️ Sequence continuity check failed.")
                End If
            End If

            ' ✅ Step 6: Balancing check
            Logger.LogProgress("Applying balancing rules...")
            If postTransList.Any() Then
                Dim firstPOSTRANS As Models.POSTRANS = postTransList.First()
                If Not ValidationHelper.CheckPostTransBalance(firstPOSTRANS, transpayList) Then
                    Logger.LogProgress($"POSTRANS {firstPOSTRANS.POSTRANS} is not balanced.")
                End If
            End If
            UpdateProgressBar(90, "Reconciling tables...")

            ' ✅ Step 6b: Reconcile POSTRANS and TRANSPAY according to use cases
            Logger.LogProgress("Reconciling POSTRANS and TRANSPAY...")
            DatabaseHelper.ReconcileAndWriteChanges(connection, postTransList, transpayList, controlNoList)
            Logger.LogProgress("Reconciliation applied.")
            UpdateProgressBar(95, "Finalizing...")

            ' ✅ Step 7: Finalization
            Logger.LogProgress("Saving fixed MDB...")
            Threading.Thread.Sleep(1000)
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
        ProgressBarX1.CustomText = $"{text} {value}%"
        Application.DoEvents()
        Threading.Thread.Sleep(50)
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
            Dim backupFolderPath As String = Path.Combine(Path.GetDirectoryName(dbPath), "Backup")
            Process.Start(backupFolderPath)
        Catch ex As Exception
            MessageBox.Show("Could not open backup folder: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Logger.LogError("Failed to open backup folder: " & ex.Message, ex)
        End Try
    End Sub

End Class
