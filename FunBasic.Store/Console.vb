Imports System.Text

Public Class Console
    Implements FunBasic.Library.IConsole

    Dim Output As StringBuilder

    Sub New(output As StringBuilder)
        Me.Output = output
    End Sub

    Public Sub WriteLine(value As Object) Implements Library.IConsole.WriteLine
        Output.AppendLine(value.ToString())
    End Sub

End Class
