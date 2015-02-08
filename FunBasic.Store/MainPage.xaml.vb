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
    Dim done As ManualResetEvent = New ManualResetEvent(False)

    Private Async Sub StartButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles StartButton.Click
        StartButton.IsEnabled = False
        StopButton.IsEnabled = True

        cancelToken = New CancelToken()

        Dim program = Code.Text
        Await Task.Run(Sub() Start(program))
    End Sub

    Private Async Sub StopButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles StopButton.Click
        StopButton.IsEnabled = False

        ffi.Unhook()
        timer.Pause()
        cancelToken.Cancel()

        Await Task.Run(Sub() [Stop]())
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

    Private Async Sub Start(program As String)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, _
                            Sub() InitLibrary())

        Try
            Runtime.Run(program, ffi, cancelToken)
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine(ex)
            TextWindow.Console.WriteLine(ex.Message)
        Finally        
            done.Set()
        End Try
        If Not ffi.IsHooked Then
            timer.Pause()
            Await EnableStartButton()
        End If
    End Sub

    Private Async Sub [Stop]()
        If done IsNot Nothing Then
            Dim success = done.WaitOne()
            done.Reset()
        End If
        Await EnableStartButton()
    End Sub

    Private Async Function EnableStartButton() As Task
        Await Dispatcher.RunAsync(Core.CoreDispatcherPriority.Normal,
                    Sub()
                        StartButton.IsEnabled = True
                        StopButton.IsEnabled = False
                    End Sub)
    End Function

End Class
