Imports System.Text
Imports System.Reflection
Imports FunBasic.Library
Imports System.Threading
Imports FunBasic.Interpreter
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Dim ffi As New FFI()
    Dim timer As New Timer()
    Dim cancelToken As New CancelToken()
    Dim done As ManualResetEvent = Nothing

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        ffi.Unhook()
        timer.Pause()

        ' Stop interpreter
        cancelToken.Cancel()
        cancelToken = New CancelToken()

        Dim program = Code.Text

        Task.Run(Sub() Run(program))
    End Sub

    Private Sub InitLibrary()
        Dim c = New Console(Me.MyConsole)
        TextWindow.Console = c
        Dim theTurtle = Me.MyTurtle
        Me.MyGraphics.Children.Clear()
        Me.MyGraphics.Background = New SolidColorBrush(Colors.White)
        Me.MyShapes.Children.Clear()
        Me.MyShapes.Children.Add(theTurtle)
        Dim graphics = New Graphics(Me.MyGraphics, Me.MyShapes, Me.MyTurtle)
        GraphicsWindow.Graphics = graphics
        Turtle.Graphics = graphics
        timer.Interval = -1
        FunBasic.Library.Timer.SetTimer(timer)
    End Sub

    Private Sub Run(program As String)
        If done IsNot Nothing Then
            Dim success = done.WaitOne()
            done.Reset()
        Else
            done = New ManualResetEvent(False)
        End If

        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, _
                            Sub() InitLibrary()).AsTask().Wait()

        Try
            Runtime.Run(Program, ffi, cancelToken)
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine(ex)
            TextWindow.Console.WriteLine(ex.Message)
        Finally
            done.Set()
        End Try

    End Sub

End Class
