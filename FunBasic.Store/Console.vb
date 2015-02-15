Imports System.Text

Public Class Console
    Implements FunBasic.Library.IConsole

    Dim MyTextBox As TextBox

    Sub New(textBox As TextBox)
        textBox.Text = ""
        Me.MyTextBox = textBox
        Me.queue = New Concurrent.ConcurrentQueue(Of String)()
        AddHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Sub [Stop]()
        RemoveHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Dim Queue As Concurrent.ConcurrentQueue(Of String)

    Public Sub WriteLine(value As Object) Implements Library.IConsole.WriteLine
        queue.Enqueue(value.ToString() + vbCrLf)
    End Sub

    Private Sub Rendering(sender As Object, e As Object)
        Dim text As String = Nothing
        Dim list = New List(Of String)
        Dim count = 10
        While queue.TryDequeue(text) And count > 0
            list.Add(text)
            count = count - 1
        End While
        If list.Count > 0 Then
            Dim appendText = String.Join("", list)
            Dim newText = MyTextBox.Text + appendText
            System.Diagnostics.Debug.WriteLine(newText.Count)
            MyTextBox.Text = newText
        End If
    End Sub

End Class
