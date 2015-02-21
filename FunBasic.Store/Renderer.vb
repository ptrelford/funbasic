Imports Windows.UI.Core

Public Class Renderer
    Implements System.IDisposable

    Dim MyQueue As New Concurrent.ConcurrentQueue(Of DispatchedHandler)

    Public Sub New()
        AddHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        RemoveHandler CompositionTarget.Rendering, AddressOf Rendering
    End Sub

    Private Sub Rendering(sender As Object, e As Object)
        Dim action As DispatchedHandler = Nothing
        Dim timer = Stopwatch.StartNew()
        While MyQueue.TryDequeue(action)
            Try
                action.Invoke()
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine(ex)
            End Try
            If timer.ElapsedMilliseconds > 50 Then
                Exit While
            End If

        End While
    End Sub

    Public Sub Dispatch(action As DispatchedHandler)
        MyQueue.Enqueue(action)
    End Sub

End Class
