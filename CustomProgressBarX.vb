Imports System.Drawing
Imports System.Windows.Forms
Imports DevComponents.DotNetBar.Controls

Public Class CustomProgressBarX
    Inherits ProgressBarX

    Public Property CustomText As String = ""

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        If Not String.IsNullOrEmpty(CustomText) Then
            Dim g As Graphics = e.Graphics
            Dim rect As Rectangle = ClientRectangle

            Using sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
                Using textBrush As New SolidBrush(ForeColor)
                    g.DrawString(CustomText, Font, textBrush, rect, sf)
                End Using
            End Using
        End If
    End Sub
End Class