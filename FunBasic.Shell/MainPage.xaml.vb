Imports System.Text, System.Reflection, System.Threading
Imports FunBasic.Interpreter, FunBasic.Library, FunBasic.Store
Imports Windows.UI

Public NotInheritable Class MainPage
    Inherits Page

    Dim ffi As New FFI()
    Dim timer As New FunBasic.Store.Timer()
    Dim cancelToken As New CancelToken()
    Dim done As ManualResetEvent = New ManualResetEvent(False)

    Private Async Sub StartButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles StartButton.Click
        StartButton.IsEnabled = False
        StopButton.IsEnabled = True
        Code.IsEnabled = False

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
        Dim style = New Style()
        Dim console = New Console(Me.MyConsole)
        Dim theTurtle = Me.MyTurtle
        Me.MyGraphics.Children.Clear()
        Me.MyGraphics.Background = New SolidColorBrush(Colors.White)
        Me.MyShapes.Children.Clear()
        Me.MyShapes.Children.Add(theTurtle)
        Dim images = New FunBasic.Store.Images(Me.MyGraphics.Dispatcher)
        Dim renderer = New Renderer()
        Dim surface = New Surface(Me.MyGraphics, Me.MyShapes, renderer, theTurtle)
        Dim drawings = New Drawings(style, Me.MyGraphics, images, renderer)
        Dim controls = New FunBasic.Store.Controls(style, Me.MyShapes)
        Dim shapes = New FunBasic.Store.Shapes(style, Me.MyShapes, images, theTurtle, renderer)
        Dim sounds = New Sounds(Nothing, Nothing, Nothing, Nothing, Nothing)
        Dim keyboard = New Keyboard()
        Dim mouse = New FunBasic.Store.Mouse(Me.MyGraphics)
        Dim flickr = New FunBasic.Store.Flickr()
        Dim speech = New FunBasic.Store.Speech(Me.MyGraphics.Dispatcher)
        Dim token = New CancellationTokenSource()
        FunBasic.Library._Library.Initialize( _
            console,
            surface,
            style,
            drawings,
            shapes,
            images,
            controls,
            sounds,
            keyboard,
            mouse,
            timer,
            flickr,
            speech,
            token.Token)
    End Sub

    Private Async Sub Start(program As String)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, _
                            Sub() InitLibrary())

        Try
            Runtime.Run(program, ffi, cancelToken)
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine(ex)
            TextWindow.WriteLine(ex.Message)
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
                        Me.Code.IsEnabled = True
                    End Sub)
    End Function

End Class
