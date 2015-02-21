Imports Windows.UI.Core, Windows.UI.Popups

Public Class Surface
    Implements Library.ISurface, IDisposable

    Dim MyDrawingCanvas As Canvas
    Dim MyShapesCanvas As Canvas
    Dim MyRenderer As Renderer
    Dim MyTurtle As UIElement
    Dim MyBackgroundColor As String
    Dim MyWidth As Double
    Dim MyHeight As Double

    Sub New(drawingCanvas As Canvas, shapesCanvas As Canvas, renderer As Renderer, turtle As UIElement)
        Me.MyDrawingCanvas = drawingCanvas
        Me.MyShapesCanvas = shapesCanvas
        Me.MyRenderer = renderer
        Me.MyTurtle = turtle
        Me.MyBackgroundColor = "White"
        Me.MyWidth = drawingCanvas.ActualWidth
        Me.MyHeight = drawingCanvas.ActualHeight
        AddHandler MyDrawingCanvas.SizeChanged, AddressOf SizeChanged
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        RemoveHandler MyDrawingCanvas.SizeChanged, AddressOf SizeChanged
    End Sub

    Private Sub SizeChanged(sender As Object, e As SizeChangedEventArgs)
        MyWidth = e.NewSize.Width
        MyHeight = e.NewSize.Height
    End Sub

    Private Sub Dispatch(action As DispatchedHandler)
        MyRenderer.Dispatch(action)
    End Sub

#Region "ISurface"
    Public Property Width As Double _
        Implements Library.ISurface.Width
        Get
            Return MyWidth
        End Get
        Set(value As Double)
            MyWidth = value
        End Set
    End Property

    Public Property Height As Double _
        Implements Library.ISurface.Height
        Get
            Return MyHeight
        End Get
        Set(value As Double)
            MyHeight = value
        End Set
    End Property

    Public Property BackgroundColor As String _
        Implements Library.ISurface.BackgroundColor
        Get
            Return MyBackgroundColor
        End Get
        Set(value As String)
            Dispatch(
                Sub()
                    MyBackgroundColor = value
                    MyDrawingCanvas.Background = New SolidColorBrush(GetColor(value))
                End Sub)
        End Set
    End Property

    Public Sub Clear() Implements Library.ISurface.Clear
        Dispatch(
            Sub()
                MyDrawingCanvas.Children.Clear()
                MyShapesCanvas.Children.Clear()
                MyShapesCanvas.Children.Add(MyTurtle)
                MyShapesCanvas.Resources.Clear()
            End Sub)
    End Sub

    Dim IsDialogShowing As Boolean

    Public Sub ShowMessage(content As String, title As String) _
        Implements Library.ISurface.ShowMessage
        Dispatch(
            Async Sub()
                If Not IsDialogShowing Then
                    Try
                        IsDialogShowing = True
                        Dim message = New MessageDialog(content, title)
                        Dim result = Await message.ShowAsync()
                    Finally
                        IsDialogShowing = False
                    End Try
                End If
            End Sub)
    End Sub

#End Region

End Class
