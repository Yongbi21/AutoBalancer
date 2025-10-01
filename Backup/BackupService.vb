Namespace Backup
    Public Class BackupService
        Public Shared Function CreateBackup(sourceDbPath As String) As String
            Try
                Dim backupFolderPath As String = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(sourceDbPath), "Backup")
                If Not System.IO.Directory.Exists(backupFolderPath) Then
                    System.IO.Directory.CreateDirectory(backupFolderPath)
                End If

                Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
                Dim backupFileName As String = System.IO.Path.GetFileNameWithoutExtension(sourceDbPath) & "_backup_" & timestamp & System.IO.Path.GetExtension(sourceDbPath)
                Dim backupFilePath As String = System.IO.Path.Combine(backupFolderPath, backupFileName)

                System.IO.File.Copy(sourceDbPath, backupFilePath, True)
                Return backupFilePath
            Catch ex As Exception
                ' In a real application, you would log this error
                Throw New Exception("Failed to create backup: " & ex.Message, ex)
            End Try
        End Function
    End Class
End Namespace