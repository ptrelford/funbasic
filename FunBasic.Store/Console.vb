Imports System.Text

Public Class Console
    Implements FunBasic.Library.IConsole, IDisposable

    Dim MyTextBox As TextBox
    Dim Queue As Concurrent.ConcurrentQueue(Of String)

    Sub New(textBox As TextBox)
        textBox.Text = ""
        Me.MyTextBox = textBox
        Me.Queue = New Concurrent.ConcurrentQueue(Of String)()
        AddHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        RemoveHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Public Sub WriteLine(value As String) Implements Library.IConsole.WriteLine
        Queue.Enqueue(value + vbCrLf)
    End Sub

    Private Sub Rendering(sender As Object, e As Object)
        Dim text As String = Nothing
        Dim list = New List(Of String)
        Dim count = 10
        While Queue.TryDequeue(text)
            list.Add(text)
            count = count - 1
            If count = 0 Then Exit While
        End While
        If list.Count > 0 Then
            Dim appendText = String.Join("", list)
            Dim newText = MyTextBox.Text + appendText
            System.Diagnostics.Debug.WriteLine(newText.Count)
            MyTextBox.Text = newText
        End If
    End Sub

End Class
