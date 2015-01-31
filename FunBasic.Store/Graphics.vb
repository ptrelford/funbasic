Imports System.Reflection
Imports Windows.UI, Windows.UI.Xaml.Shapes
Imports Windows.UI.Core
Imports Windows.System
Imports Windows.UI.Xaml.Media.Animation

Public Class Graphics
    Implements FunBasic.Library.IGraphics

    Dim MyCanvas As Canvas
    Dim ColorLookup As New Dictionary(Of String, Color)()
    Dim ShapeLookup As New Dictionary(Of String, UIElement)()
    Dim MyLastKey As VirtualKey
    Dim PointerX As Double
    Dim PointerY As Double

    Public Sub New(canvas As Canvas)
        Me.MyCanvas = canvas
        PrepareColors()

        AddHandler canvas.PointerPressed, AddressOf PointerPressed

        Dim window = CoreWindow.GetForCurrentThread()
        AddHandler window.KeyDown, AddressOf OnKeyDown
    End Sub

    Sub Dispatch(action As DispatchedHandler)
        Dim ignored = _
            Me.MyCanvas.Dispatcher.RunAsync(Core.CoreDispatcherPriority.Normal, action)
    End Sub

    Function GetColor(name As String) As Color
        Return ColorLookup(name.ToLower())
    End Function

    Private Sub PrepareColors()
        Dim ti = GetType(Colors)
        Dim colors = ti.GetRuntimeProperties()
        For Each pi In colors
            Dim color = pi.GetMethod().Invoke(Nothing, New Object() {})
            ColorLookup.Add(pi.Name.ToLower(), color)
        Next
    End Sub

    Public ReadOnly Property Height As Double _
    Implements Library.IGraphics.Height
        Get
            Return MyCanvas.Width
        End Get
    End Property

    Public ReadOnly Property Width As Double _
        Implements Library.IGraphics.Width
        Get
            Return MyCanvas.Height
        End Get
    End Property

    Public Property BackgroundColor As String _
        Implements Library.IGraphics.BackgroundColor
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As String)
            Dispatch(Sub() MyCanvas.Background = New SolidColorBrush(GetColor(value)))
        End Set
    End Property

    Public Property BrushColor As String Implements Library.IGraphics.BrushColor
    Public Property FontName As String Implements Library.IGraphics.FontName
    Public Property FontSize As Double Implements Library.IGraphics.FontSize
    Public Property PenColor As String Implements Library.IGraphics.PenColor
    Public Property PenWidth As Double Implements Library.IGraphics.PenWidth

    Public Sub Clear() Implements Library.IGraphics.Clear
        Dispatch(Sub() MyCanvas.Children.Clear())
    End Sub

    Public Sub DrawLine(x1 As Integer, y1 As Integer, _
                        x2 As Integer, y2 As Integer) _
        Implements Library.IGraphics.DrawLine
        Dispatch(Sub()
                     Dim line = CreateLine(x1, y1, x2, y2)
                     MyCanvas.Children.Add(line)
                 End Sub)
    End Sub

    Private Function CreateLine(x1 As Integer, y1 As Integer, _
                               x2 As Integer, y2 As Integer)
        Return _
            New Line With {.StrokeThickness = PenWidth, _
                    .Stroke = New SolidColorBrush(GetColor(PenColor)), _
                    .X1 = x1, .Y1 = y1, .X2 = x2, .Y2 = y2}
    End Function

    Public Sub DrawImage(url As String, x As Integer, y As Integer) _
        Implements Library.IGraphics.DrawImage
        Dispatch(Sub()
                     Dim image = CreateImage(url)
                     Canvas.SetLeft(image, x)
                     Canvas.SetTop(image, y)
                 End Sub)
    End Sub

    Private Function CreateImage(url As String) As Image
        Dim bitmap = New BitmapImage With {.UriSource = New Uri(url)}
        Return New Image With {.Source = bitmap}
    End Function

    Public Sub DrawText(x As Integer, y As Integer, _
                        text As String) _
    Implements Library.IGraphics.DrawText
        Dispatch(Sub()
                     Dim textBlock = CreateTextBlock(text)
                     textBlock.Margin = New Thickness(x, y, 0, 0)
                     MyCanvas.Children.Add(textBlock)
                 End Sub)
    End Sub

    Private Function CreateTextBlock(text As String) As TextBlock
        Return _
            New TextBlock With { _
                .Foreground = New SolidColorBrush(GetColor(BrushColor)), _
                .Text = text, _
                .FontSize = FontSize, .FontFamily = New FontFamily(FontName) _
                }
    End Function

    Public Sub FillEllipse(x As Integer, y As Integer, _
                           width As Integer, height As Integer) _
        Implements Library.IGraphics.FillEllipse
        Dispatch(Sub()
                     Dim brush = New SolidColorBrush(GetColor(BrushColor))
                     Dim ellipse = _
                         New Ellipse With {.Fill = brush,
                                           .Margin = New Thickness(x, y, 0, 0),
                                           .Width = width, .Height = height}
                     MyCanvas.Children.Add(ellipse)
                 End Sub)
    End Sub

    Public Function AddImage(url As String) As String _
        Implements Library.IGraphics.AddImage
        Dim name = "Image" + Guid.NewGuid().ToString()
        Dim image As Image = Nothing
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                image = CreateImage(url)
                image.Name = name
                MyCanvas.Children.Add(image)
                ShapeLookup.Add(name, image)
            End Sub).AsTask().Wait()
        Return name
    End Function

    Public Function AddLine(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As String _
        Implements Library.IGraphics.AddLine
        Dim name = "Line" + Guid.NewGuid().ToString()
        Dim line As Line = Nothing
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                line = CreateLine(x1, y1, x2, y2)
                line.Name = name
                MyCanvas.Children.Add(line)
                ShapeLookup.Add(name, line)
            End Sub).AsTask().Wait()
        Return name
    End Function

    Public Function AddRectangle(width As Integer, height As Integer) As String _
        Implements Library.IGraphics.AddRectangle
        Dim name = "Rectangle" + Guid.NewGuid().ToString()
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                Dim rectangle = CreateRectangle(width, height)
                rectangle.Name = name
                MyCanvas.Children.Add(rectangle)
                ShapeLookup.Add(name, rectangle)
            End Sub).AsTask().Wait()
        Return name
    End Function

    Private Function CreateRectangle(width As Integer, height As Integer) As Rectangle
        Return New Rectangle _
            With {.StrokeThickness = PenWidth, _
                    .Stroke = New SolidColorBrush(GetColor(PenColor)), _
                    .Fill = New SolidColorBrush(GetColor(BrushColor)), _
                    .Width = width, .Height = height}
    End Function

    Public Function AddText(text As String) As String _
        Implements Library.IGraphics.AddText
        Dim name = "Text" + Guid.NewGuid().ToString()
        Dim textBlock As TextBlock = Nothing
        MyCanvas.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, _
            Sub()
                textBlock = CreateTextBlock(text)
                textBlock.Name = name
                MyCanvas.Children.Add(textBlock)
                ShapeLookup.Add(name, textBlock)
            End Sub).AsTask().Wait()
        Return name
    End Function

    Public Sub SetText(name As String, text As String) Implements Library.IGraphics.SetText
        Dim textBlock = CType(ShapeLookup(name), TextBlock)
        Dispatch(Sub() textBlock.Text = text)
    End Sub

    Public Sub Remove(name As String) _
        Implements Library.IGraphics.Remove
        Dim shape = ShapeLookup(name)
        Dispatch(Sub() MyCanvas.Children.Remove(shape))
        ShapeLookup.Remove(name)
    End Sub

    Public Sub Move(name As String, x As Integer, y As Integer) _
        Implements Library.IGraphics.Move
        Dim shape = ShapeLookup(name)
        Dispatch(Sub()
                     Canvas.SetLeft(shape, x)
                     Canvas.SetTop(shape, y)
                 End Sub)
    End Sub

    Public Sub Animate(name As String, x As Integer, y As Integer, duration As Integer) _
        Implements Library.IGraphics.Animate
        Dim shape = ShapeLookup(name)
        Dispatch(Sub()
                     Dim story = New Storyboard()
                     story.Duration = TimeSpan.FromMilliseconds(duration)

                     Dim left = New DoubleAnimation()
                     left.To = x
                     left.SetValue(Storyboard.TargetPropertyProperty, "(Canvas.Left)")
                     story.Children.Add(left)

                     Dim top = New DoubleAnimation()
                     top.To = y
                     top.SetValue(Storyboard.TargetPropertyProperty, "(Canvas.Top)")
                     story.SetValue(Storyboard.TargetNameProperty, name)
                     story.Children.Add(top)

                     MyCanvas.Resources.Add(CType(Guid.NewGuid().ToString(), Object), story)
                     story.Begin()
                 End Sub)
    End Sub

    Public Sub Rotate(name As String, angle As Integer) _
        Implements Library.IGraphics.Rotate
        Dim shape = ShapeLookup(name)
        Dispatch(Sub()
                     Dim transform As New RotateTransform()
                     Dim el = CType(shape, FrameworkElement)
                     transform.CenterX = el.ActualWidth / 2
                     transform.CenterY = el.ActualHeight / 2
                     transform.Angle = angle
                     shape.RenderTransform = transform
                 End Sub)
    End Sub

    Public Sub SetOpacity(name As String, value As Integer) _
        Implements Library.IGraphics.SetOpacity
        Dim shape = ShapeLookup(name)
        Dispatch(Sub() shape.Opacity = CType(value, Double) / 100.0)
    End Sub

    Public Event KeyDown(sender As Object, e As EventArgs) _
        Implements Library.IGraphics.KeyDown

    Public ReadOnly Property LastKey As String Implements Library.IGraphics.LastKey
        Get
            Return [Enum].GetName(GetType(VirtualKey), MyLastKey)
        End Get
    End Property

    Private Sub OnKeyDown(sender As CoreWindow, args As KeyEventArgs)
        MyLastKey = args.VirtualKey
        RaiseEvent KeyDown(Me, New EventArgs)
    End Sub

    Public Event MouseDown(sender As Object, e As EventArgs) Implements Library.IGraphics.MouseDown

    Public ReadOnly Property MouseX As Integer Implements Library.IGraphics.MouseX
        Get
            Return PointerX
        End Get
    End Property

    Public ReadOnly Property MouseY As Integer Implements Library.IGraphics.MouseY
        Get
            Return PointerY
        End Get
    End Property

    Private Sub PointerPressed(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        RaiseEvent MouseDown(Me, New EventArgs())
    End Sub

End Class
