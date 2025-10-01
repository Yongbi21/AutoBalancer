Namespace Logs
    Public Class Logger
        Public Shared ReadOnly LogFolderPath As String = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs")

        Private Shared Sub EnsureLogFolderExists()
            If Not System.IO.Directory.Exists(LogFolderPath) Then
                System.IO.Directory.CreateDirectory(LogFolderPath)
            End If
        End Sub

        Private Shared Function GetLogFilePath() As String
            EnsureLogFolderExists()
            Dim logFileName As String = "AutoBalancer_Log_" & DateTime.Now.ToString("yyyyMMdd") & ".txt"
            Return System.IO.Path.Combine(LogFolderPath, logFileName)
        End Function

        Public Shared Sub LogProgress(message As String)
            Try
                Dim logMessage As String = "[" & DateTime.Now.ToString("HH:mm:ss") & "] [INFO] " & message
                System.IO.File.AppendAllText(GetLogFilePath(), logMessage & Environment.NewLine)
                System.Diagnostics.Debug.WriteLine(logMessage)
            Catch ex As Exception
                ' Fallback: if logging to file fails, just write to debug output
                System.Diagnostics.Debug.WriteLine("[" & DateTime.Now.ToString("HH:mm:ss") & "] [ERROR] Failed to write to log file: " & ex.Message)
            End Try
        End Sub

        Public Shared Sub LogError(message As String, Optional ex As Exception = Nothing)
            Try
                Dim logMessage As String = "[" & DateTime.Now.ToString("HH:mm:ss") & "] [ERROR] " & message
                If ex IsNot Nothing Then
                    logMessage &= Environment.NewLine & "Exception: " & ex.Message & Environment.NewLine & "Stack Trace: " & ex.StackTrace
                End If
                System.IO.File.AppendAllText(GetLogFilePath(), logMessage & Environment.NewLine)
                System.Diagnostics.Debug.WriteLine(logMessage)
            Catch logEx As Exception
                ' Fallback: if logging to file fails, just write to debug output
                System.Diagnostics.Debug.WriteLine("[" & DateTime.Now.ToString("HH:mm:ss") & "] [ERROR] Failed to write to log file: " & logEx.Message)
            End Try
        End Sub
    End Class
End Namespace