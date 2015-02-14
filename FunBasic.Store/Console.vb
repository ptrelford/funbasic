Imports System.Text

Public Class Console
    Implements FunBasic.Library.IConsole

    Dim MyTextBox As TextBox

    Sub New(textBox As TextBox)
        textBox.Text = ""
        Me.MyTextBox = textBox    

        AddHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Dim queue As New Concurrent.ConcurrentQueue(Of String)

    Public Sub WriteLine(value As Object) Implements Library.IConsole.WriteLine
        queue.Enqueue(value.ToString() + vbCrLf)
    End Sub

    Private Sub Rendering(sender As Object, e As Object)
        Dim text As String = Nothing
        Dim list = New List(Of String)
        While queue.TryDequeue(text)
            list.Add(text)
        End While
        MyTextBox.Text = MyTextBox.Text + String.Join("", list)
    End Sub

End Class
