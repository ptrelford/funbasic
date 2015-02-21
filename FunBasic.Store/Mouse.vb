Public Class Mouse
    Implements FunBasic.Library.IMouse, IDisposable

    Dim MyDrawingCanvas As Canvas
    Dim PointerX As Double
    Dim PointerY As Double

    Public Sub New(drawingCanvas As Canvas)
        MyDrawingCanvas = drawingCanvas
        AddHandler drawingCanvas.PointerPressed, AddressOf PointerPressed
        AddHandler drawingCanvas.PointerReleased, AddressOf PointerReleased
        AddHandler drawingCanvas.PointerMoved, AddressOf PointerMoved
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        RemoveHandler MyDrawingCanvas.PointerPressed, AddressOf PointerPressed
        RemoveHandler MyDrawingCanvas.PointerReleased, AddressOf PointerReleased
        RemoveHandler MyDrawingCanvas.PointerMoved, AddressOf PointerMoved
    End Sub


#Region "Mouse"

    Public Event MouseDown(sender As Object, e As EventArgs) Implements Library.IMouse.MouseDown
    Public Event MouseMove(sender As Object, e As EventArgs) Implements Library.IMouse.MouseMove
    Public Event MouseUp(sender As Object, e As EventArgs) Implements Library.IMouse.MouseUp

    Public ReadOnly Property MouseX As Double _
        Implements Library.IMouse.MouseX
        Get
            Return PointerX
        End Get
    End Property

    Public ReadOnly Property MouseY As Double _
        Implements Library.IMouse.MouseY
        Get
            Return PointerY
        End Get
    End Property

    Private Sub PointerPressed(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyDrawingCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        FunBasic.Library.Mouse.MouseX = position.X
        FunBasic.Library.Mouse.MouseY = position.Y
        FunBasic.Library.Mouse.IsLeftButtonDown = True
        RaiseEvent MouseDown(Me, New EventArgs())
    End Sub

    Private Sub PointerReleased(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyDrawingCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        FunBasic.Library.Mouse.IsLeftButtonDown = False
        RaiseEvent MouseUp(Me, New EventArgs())
    End Sub

    Private Sub PointerMoved(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyDrawingCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        RaiseEvent MouseMove(Me, New EventArgs())
    End Sub
#End Region

End Class
