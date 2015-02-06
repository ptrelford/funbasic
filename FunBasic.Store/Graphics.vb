Imports System.Globalization
Imports System.Reflection
Imports Windows.UI, Windows.UI.Xaml.Shapes
Imports Windows.UI.Core
Imports Windows.System
Imports Windows.UI.Xaml.Media.Animation
Imports Windows.UI.Popups
Imports Windows.UI.Text

Public Class Graphics
    Implements FunBasic.Library.IGraphics

    Dim MyCanvas As Canvas
    Dim MyShapesCanvas As Canvas
    Dim MyBackgroundColor As String
    Dim MyWidth As Double
    Dim MyHeight As Double
    Dim ColorLookup As New Dictionary(Of String, Color)()
    Dim ShapeLookup As New Dictionary(Of String, UIElement)()
    Dim MyLastKey As VirtualKey
    Dim PointerX As Double
    Dim PointerY As Double

    Public Sub New(canvas As Canvas, shapesCanvas As Canvas, turtle As UIElement)
        Me.MyCanvas = canvas
        Me.MyShapesCanvas = shapesCanvas
        Me.MyBackgroundColor = "White"
        Me.MyWidth = canvas.Width
        Me.MyHeight = canvas.Height
        PrepareColors()

        AddHandler canvas.PointerPressed, AddressOf PointerPressed
        AddHandler canvas.PointerReleased, AddressOf PointerReleased
        AddHandler canvas.PointerMoved, AddressOf PointerMoved

        Dim window = CoreWindow.GetForCurrentThread()
        AddHandler window.KeyDown, AddressOf OnKeyDown

        ShapeLookup.Add("Turtle", turtle)
    End Sub

    Sub Dispatch(action As DispatchedHandler)
        Dim ignored = _
            Me.MyCanvas.Dispatcher.RunAsync(Core.CoreDispatcherPriority.Normal, action)
    End Sub

    Function GetColor(name As String) As Color
        If name.StartsWith("#") Then
            Dim n = Integer.Parse(name.Substring(1), NumberStyles.HexNumber)
            Dim r = (n And &HFF0000) >> 16
            Dim g = (n And &HFF00) >> 8
            Dim b = (n And &HFF)
            Return Color.FromArgb(255, r, g, b)
        Else
            Return ColorLookup(name.ToLower())
        End If
    End Function

    Sub PrepareColors()
        Dim ti = GetType(Colors)
        Dim colors = ti.GetRuntimeProperties()
        For Each pi In colors
            Dim color = pi.GetMethod().Invoke(Nothing, New Object() {})
            ColorLookup.Add(pi.Name.ToLower(), color)
        Next
    End Sub

#Region "Properties"
    Public Property Width As Double _
        Implements Library.IGraphics.Width
        Get
            Return MyWidth
        End Get
        Set(value As Double)
            Dispatch(Sub()
                         MyWidth = value
                         'MyCanvas.Width = value
                     End Sub)
        End Set
    End Property

    Public ReadOnly Property Height As Double _
        Implements Library.IGraphics.Height
        Get
            Return MyHeight
        End Get
    End Property

    Public Property BackgroundColor As String _
        Implements Library.IGraphics.BackgroundColor
        Get
            Return MyBackgroundColor
        End Get
        Set(value As String)
            Dispatch(Sub()
                         MyBackgroundColor = value
                         MyCanvas.Background = New SolidColorBrush(GetColor(value))
                     End Sub)
        End Set
    End Property

    Public Property BrushColor As String Implements Library.IGraphics.BrushColor
    Public Property FontName As String Implements Library.IGraphics.FontName
    Public Property FontSize As Double Implements Library.IGraphics.FontSize
    Public Property FontItalic As Boolean Implements Library.IGraphics.FontItalic
    Public Property FontBold As Boolean Implements Library.IGraphics.FontBold
    Public Property PenColor As String Implements Library.IGraphics.PenColor
    Public Property PenWidth As Double Implements Library.IGraphics.PenWidth
#End Region

#Region "Graphics"
    Public Sub Clear() Implements Library.IGraphics.Clear
        Dispatch(Sub()
                     Dim myTurtle = ShapeLookup("Turtle")
                     MyCanvas.Children.Clear()
                     MyShapesCanvas.Children.Clear()
                     MyShapesCanvas.Children.Add(myTurtle)
                     MyShapesCanvas.Resources.Clear()
                 End Sub)
    End Sub

    Public Sub ShowMessage(content As String, title As String) Implements Library.IGraphics.ShowMessage
        Dispatch(Sub()
                     Dim message = New MessageDialog(content, title)
                     Dim task = message.ShowAsync()
                 End Sub)
    End Sub

    Public Sub DrawEllipse(x As Double, y As Double, _
                           width As Double, height As Double) _
        Implements Library.IGraphics.DrawEllipse
        Dim color = GetColor(PenColor)
        Dim thickness = PenWidth
        Dispatch(Sub()
                     Dim ellipse = CreateEllipse(x, y)
                     ellipse.StrokeThickness = thickness
                     ellipse.Stroke = New SolidColorBrush(color)
                     ellipse.Margin = New Thickness(x, y, 0, 0)
                     MyCanvas.Children.Add(ellipse)
                 End Sub)
    End Sub

    Public Sub DrawLine(x1 As Double, y1 As Double, _
                        x2 As Double, y2 As Double) _
        Implements Library.IGraphics.DrawLine
        Dim color = GetColor(PenColor)
        Dim thickness = PenWidth
        Dispatch(Sub()
                     Dim line = CreateLine(x1, y1, x2, y2)
                     line.Stroke = New SolidColorBrush(color)
                     line.StrokeThickness = thickness
                     MyCanvas.Children.Add(line)
                 End Sub)
    End Sub

    Private Function CreateLine(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double)
        Return New Line With {.X1 = x1, .Y1 = y1, .X2 = x2, .Y2 = y2}
    End Function

    Public Sub DrawTriangle(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double, _
                            x3 As Double, y3 As Double) _
        Implements Library.IGraphics.DrawTriangle
        Dim color = GetColor(PenColor)
        Dim thickness = PenWidth
        Dispatch(Sub()
                     Dim poly = CreateTriangle(x1, y1, x2, y2, x3, y3)
                     poly.StrokeThickness = thickness
                     poly.Stroke = New SolidColorBrush(color)
                     MyCanvas.Children.Add(poly)
                 End Sub)
    End Sub

    Function CreateTriangle(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double, _
                            x3 As Double, y3 As Double) As Polygon
        Dim poly = New Polygon()
        poly.Points.Add(New Point(x1, y1))
        poly.Points.Add(New Point(x2, y2))
        poly.Points.Add(New Point(x3, y3))
        Return poly
    End Function

    Public Sub DrawRectangle(x As Double, y As Double, _
                             width As Double, height As Double) _
        Implements Library.IGraphics.DrawRectangle
        Dim color = GetColor(PenColor)
        Dim thickness = PenWidth
        Dispatch(Sub()
                     Dim rectangle = CreateRectangle(width, height)
                     rectangle.StrokeThickness = thickness
                     rectangle.Stroke = New SolidColorBrush(color)
                     rectangle.Margin = New Thickness(x, y, 0, 0)
                     MyCanvas.Children.Add(rectangle)
                 End Sub)
    End Sub

    Public Sub DrawImage(url As String, x As Double, y As Double) _
        Implements Library.IGraphics.DrawImage
        Dispatch(Sub()
                     Dim image = CreateImage(url)
                     Canvas.SetLeft(image, x)
                     Canvas.SetTop(image, y)
                 End Sub)
    End Sub

    Private Function CreateImage(url As String) As Image
        Dim image As Image = Nothing
        If ImageList.TryGetValue(url, image) Then
            Return New Image With {.Source = image.Source}
        Else
            Dim bitmap = New BitmapImage With {.UriSource = New Uri(url)}
            Return New Image With {.Source = bitmap}
        End If
    End Function

    Public Sub DrawText(x As Double, y As Double, _
                        text As String) _
    Implements Library.IGraphics.DrawText
        Dim foreground = GetColor(BrushColor)
        Dim size = FontSize
        Dim family = FontName
        Dispatch(Sub()
                     Dim textBlock = CreateTextBlock(text)
                     textBlock.Foreground = New SolidColorBrush(foreground)
                     textBlock.FontSize = size
                     textBlock.FontFamily = New FontFamily(family)
                     textBlock.FontStyle = If(Me.FontItalic, FontStyle.Italic, FontStyle.Normal)
                     textBlock.FontWeight = If(Me.FontBold, FontWeights.Bold, FontWeights.Normal)
                     textBlock.Margin = New Thickness(x, y, 0, 0)
                     MyCanvas.Children.Add(textBlock)
                 End Sub)
    End Sub

    Public Sub DrawBoundText(x As Double, y As Double, width As Double, text As String) _
        Implements Library.IGraphics.DrawBoundText
        Dim foreground = GetColor(BrushColor)
        Dim size = FontSize
        Dim family = FontName
        Dispatch(Sub()
                     Dim textBlock = CreateTextBlock(text)
                     textBlock.Foreground = New SolidColorBrush(foreground)
                     textBlock.FontSize = size
                     textBlock.FontFamily = New FontFamily(family)
                     textBlock.FontStyle = If(Me.FontItalic, FontStyle.Italic, FontStyle.Normal)
                     textBlock.FontWeight = If(Me.FontBold, FontWeights.Bold, FontWeights.Normal)
                     textBlock.Margin = New Thickness(x, y, 0, 0)
                     textBlock.MaxWidth = width
                     MyCanvas.Children.Add(textBlock)
                 End Sub)
    End Sub

    Private Function CreateTextBlock(text As String) As TextBlock
        Return New TextBlock With {.Text = text}
    End Function

    Public Sub FillTriangle(x1 As Double, y1 As Double, _
                        x2 As Double, y2 As Double, _
                        x3 As Double, y3 As Double) _
    Implements Library.IGraphics.FillTriangle
        Dim color = GetColor(BrushColor)
        Dispatch(Sub()
                     Dim poly = CreateTriangle(x1, y1, x2, y2, x3, y3)
                     poly.Fill = New SolidColorBrush(color)
                     MyCanvas.Children.Add(poly)
                 End Sub)
    End Sub

    Public Sub FillRectangle(x As Double, y As Double, _
                             width As Double, height As Double) _
        Implements Library.IGraphics.FillRectangle
        Dim color = GetColor(BrushColor)
        Dispatch(Sub()
                     Dim rectangle = CreateRectangle(width, height)
                     rectangle.Fill = New SolidColorBrush(color)
                     rectangle.Margin = New Thickness(x, y, 0, 0)
                     MyCanvas.Children.Add(rectangle)
                 End Sub)
    End Sub

    Public Sub FillEllipse(x As Double, y As Double, _
                           width As Double, height As Double) _
        Implements Library.IGraphics.FillEllipse
        Dim color = GetColor(BrushColor)
        Dispatch(Sub()
                     Dim ellipse = CreateEllipse(width, height)
                     ellipse.Fill = New SolidColorBrush(color)
                     ellipse.Margin = New Thickness(x, y, 0, 0)
                     MyCanvas.Children.Add(ellipse)
                 End Sub)
    End Sub

    Function CreateEllipse(width As Double, height As Double) As Ellipse
        Return New Ellipse With {.Width = width, .Height = height}
    End Function
#End Region

#Region "Shapes"

    Public Function AddImage(url As String) As String _
        Implements Library.IGraphics.AddImage
        Dim name = "Image" + Guid.NewGuid().ToString()
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                Dim image = CreateImage(url)
                image.Name = name
                MyShapesCanvas.Children.Add(image)
                ShapeLookup.Add(name, image)
            End Sub).AsTask().Wait()
        Return name
    End Function

    Public Function AddEllipse(width As Double, height As Double) As String _
        Implements Library.IGraphics.AddEllipse
        Dim name = "Ellipse" + Guid.NewGuid().ToString()
        Dim thickness = PenWidth
        Dim stroke = GetColor(PenColor)
        Dim fill = GetColor(BrushColor)
        Dispatch(
            Sub()
                Dim ellipse = CreateEllipse(width, height)
                ellipse.StrokeThickness = thickness
                ellipse.Stroke = New SolidColorBrush(stroke)
                ellipse.Fill = New SolidColorBrush(fill)
                ellipse.Name = name
                MyShapesCanvas.Children.Add(ellipse)
                ShapeLookup.Add(name, ellipse)
            End Sub)
        Return name
    End Function

    Public Function AddLine(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double) As String _
        Implements Library.IGraphics.AddLine
        Dim name = "Line" + Guid.NewGuid().ToString()
        Dim color = GetColor(PenColor)
        Dim thickness = PenWidth
        Dim line As Line = Nothing
        Dispatch(
            Sub()
                line = CreateLine(x1, y1, x2, y2)
                line.StrokeThickness = thickness
                line.Stroke = New SolidColorBrush(color)
                line.Name = name
                MyShapesCanvas.Children.Add(line)
                ShapeLookup.Add(name, line)
            End Sub)
        Return name
    End Function

    Public Function AddTriangle(x1 As Double, y1 As Double, _
                                x2 As Double, y2 As Double, _
                                x3 As Double, y3 As Double) As String _
        Implements Library.IGraphics.AddTriangle
        Dim name = "Triangle" + Guid.NewGuid().ToString()
        Dim thickness = PenWidth
        Dim stroke = GetColor(PenColor)
        Dim fill = GetColor(BrushColor)
        Dispatch(
            Sub()
                Dim poly = CreateTriangle(x1, y1, x2, y2, x3, y3)
                poly.StrokeThickness = thickness
                poly.Stroke = New SolidColorBrush(stroke)
                poly.Fill = New SolidColorBrush(fill)
                poly.Name = name
                MyShapesCanvas.Children.Add(poly)
                ShapeLookup.Add(name, poly)
            End Sub)
        Return name
    End Function

    Public Function AddRectangle(width As Double, height As Double) As String _
        Implements Library.IGraphics.AddRectangle
        Dim name = "Rectangle" + Guid.NewGuid().ToString()
        Dim thickness = PenWidth
        Dim stroke = GetColor(PenColor)
        Dim fill = GetColor(BrushColor)
        Dispatch(
            Sub()
                Dim rectangle = CreateRectangle(width, height)
                rectangle.StrokeThickness = thickness
                rectangle.Stroke = New SolidColorBrush(stroke)
                rectangle.Fill = New SolidColorBrush(fill)
                rectangle.Name = name
                MyShapesCanvas.Children.Add(rectangle)
                ShapeLookup.Add(name, rectangle)
            End Sub)
        Return name
    End Function

    Private Function CreateRectangle(width As Double, height As Double) As Rectangle
        Return New Rectangle With {.Width = width, .Height = height}
    End Function

    Public Function AddText(text As String) As String _
        Implements Library.IGraphics.AddText
        Dim name = "Text" + Guid.NewGuid().ToString()
        Dim foreground = GetColor(BrushColor)
        Dim size = FontSize
        Dim family = FontName
        Dispatch(
            Sub()
                Dim textBlock = CreateTextBlock(text)
                textBlock.Foreground = New SolidColorBrush(foreground)
                textBlock.FontSize = size
                textBlock.FontFamily = New FontFamily(family)
                textBlock.FontStyle = If(Me.FontItalic, FontStyle.Italic, FontStyle.Normal)
                textBlock.FontWeight = If(Me.FontBold, FontWeights.Bold, FontWeights.Normal)
                textBlock.Name = name
                MyShapesCanvas.Children.Add(textBlock)
                ShapeLookup.Add(name, textBlock)
            End Sub)
        Return name
    End Function

    Public Sub SetText(name As String, text As String) _
        Implements Library.IGraphics.SetText
        Dispatch(Sub()
                     Dim textBlock = CType(ShapeLookup(name), TextBlock)
                     textBlock.Text = text
                 End Sub)
    End Sub

    Public Function GetLeft(name As String) As Double _
        Implements Library.IGraphics.GetLeft
        Dim left = 0
        Dim shape As UIElement = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            MyCanvas.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, _
                                         Sub() left = Canvas.GetLeft(shape)).AsTask().Wait()
        End If
        Return left
    End Function

    Public Function GetTop(name As String) As Double _
        Implements Library.IGraphics.GetTop
        Dim top = 0
        Dim shape As UIElement = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            MyCanvas.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, _
                                         Sub() top = Canvas.GetTop(shape)).AsTask().Wait()
        End If
        Return top
    End Function

    Public Sub HideShape(name As String) _
        Implements Library.IGraphics.HideShape
        Dispatch(Sub()
                     Dim shape = ShapeLookup(name)
                     shape.Visibility = Visibility.Collapsed
                 End Sub)
    End Sub

    Public Sub ShowShape(name As String) _
        Implements Library.IGraphics.ShowShape
        Dispatch(Sub()
                     Dim shape = ShapeLookup(name)
                     shape.Visibility = Visibility.Visible
                 End Sub)
    End Sub

    Public Sub Remove(name As String) _
        Implements Library.IGraphics.Remove
        Dispatch(Sub()
                     Dim shape As UIElement = Nothing
                     If ShapeLookup.TryGetValue(name, shape) Then
                         MyShapesCanvas.Children.Remove(shape)
                         ShapeLookup.Remove(name)
                     End If
                 End Sub)
    End Sub

    Public Sub Move(name As String, x As Double, y As Double) _
        Implements Library.IGraphics.Move
        Dispatch(Sub()
                     Dim shape As UIElement = Nothing
                     If ShapeLookup.TryGetValue(name, shape) Then
                         Canvas.SetLeft(shape, x)
                         Canvas.SetTop(shape, y)
                     End If
                 End Sub)
    End Sub

    Public Sub Animate(name As String, x As Double, y As Double, duration As Integer) _
        Implements Library.IGraphics.Animate
        Dispatch(Sub()
                     Dim shape = ShapeLookup(name)
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

                     MyShapesCanvas.Resources.Add(CType(Guid.NewGuid().ToString(), Object), story)
                     story.Begin()
                 End Sub)
    End Sub

    Public Sub Rotate(name As String, angle As Integer) _
        Implements Library.IGraphics.Rotate
        Dispatch(Sub()
                     Dim shape = ShapeLookup(name)
                     Dim transform As New RotateTransform()
                     Dim el = CType(shape, FrameworkElement)
                     transform.CenterX = el.ActualWidth / 2.0
                     transform.CenterY = el.ActualHeight / 2.0
                     transform.Angle = angle
                     shape.RenderTransform = transform
                 End Sub)
    End Sub

    Public Sub Zoom(name As String, scaleX As Double, scaleY As Double) _
        Implements Library.IGraphics.Zoom
        Dispatch(Sub()
                     Dim shape = ShapeLookup(name)
                     Dim transform As New ScaleTransform()
                     Dim el = CType(shape, FrameworkElement)
                     transform.CenterX = el.ActualWidth / 2.0
                     transform.CenterY = el.ActualHeight / 2.0
                     transform.ScaleX = scaleX
                     transform.ScaleY = scaleY
                     shape.RenderTransform = transform
                 End Sub)
    End Sub

    Public Function GetOpacity(name As String) As Integer _
        Implements Library.IGraphics.GetOpacity
        Dim opacity = 0
        MyCanvas.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, _
            Sub()
                Dim shape = ShapeLookup(name)
                opacity = shape.Opacity * 100.0
            End Sub).AsTask().Wait()
        Return opacity
    End Function

    Public Sub SetOpacity(name As String, value As Integer) _
        Implements Library.IGraphics.SetOpacity
        Dispatch(Sub()
                     Dim shape = ShapeLookup(name)
                     shape.Opacity = CType(value, Double) / 100.0
                 End Sub)
    End Sub

#End Region

#Region "Image List"

    Dim ImageList As New Dictionary(Of String, Image)()

    Public Function GetImageWidth(name As String) As Integer Implements Library.IGraphics.GetImageWidth
        Dim image As Image = Nothing
        If ImageList.TryGetValue(name, image) Then
            Return image.Width
        Else
            Return 0
        End If
    End Function

    Public Function GetImageHeight(name As String) As Integer Implements Library.IGraphics.GetImageHeight
        Dim image As Image = Nothing
        If ImageList.TryGetValue(name, image) Then
            Return image.Height
        Else
            Return 0
        End If
    End Function

    Public Function LoadImage(url As String) As String Implements Library.IGraphics.LoadImage
        Dim name = "ImageList" + Guid.NewGuid.ToString()
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                Dim image = CreateImage(url)
                image.Name = name
                ImageList.Add(name, image)
            End Sub).AsTask().Wait()
        Return name
    End Function

#End Region

#Region "Keyboard"
    Public Event KeyDown(sender As Object, e As EventArgs) _
        Implements Library.IGraphics.KeyDown

    Public ReadOnly Property LastKey As String Implements Library.IGraphics.LastKey
        Get
            Select Case MyLastKey
                Case VirtualKey.Number0 : Return "D0"
                Case VirtualKey.Number1 : Return "D1"
                Case VirtualKey.Number2 : Return "D2"
                Case VirtualKey.Number3 : Return "D3"
                Case VirtualKey.Number4 : Return "D4"
                Case VirtualKey.Number5 : Return "D5"
                Case VirtualKey.Number6 : Return "D6"
                Case VirtualKey.Number7 : Return "D7"
                Case VirtualKey.Number8 : Return "D8"
                Case VirtualKey.Number9 : Return "D9"
            End Select
            Return [Enum].GetName(GetType(VirtualKey), MyLastKey)
        End Get
    End Property

    Private Sub OnKeyDown(sender As CoreWindow, args As KeyEventArgs)
        MyLastKey = args.VirtualKey
        RaiseEvent KeyDown(Me, New EventArgs)
    End Sub
#End Region

#Region "Mouse"
    Public Event MouseDown(sender As Object, e As EventArgs) Implements Library.IGraphics.MouseDown
    Public Event MouseMove(sender As Object, e As EventArgs) Implements Library.IGraphics.MouseMove
    Public Event MouseUp(sender As Object, e As EventArgs) Implements Library.IGraphics.MouseUp

    Public ReadOnly Property MouseX As Double _
        Implements Library.IGraphics.MouseX
        Get
            Return PointerX
        End Get
    End Property

    Public ReadOnly Property MouseY As Double _
        Implements Library.IGraphics.MouseY
        Get
            Return PointerY
        End Get
    End Property

    Private Sub PointerPressed(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        FunBasic.Library.Mouse.IsLeftButtonDown = True
        RaiseEvent MouseDown(Me, New EventArgs())
    End Sub

    Private Sub PointerReleased(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        FunBasic.Library.Mouse.IsLeftButtonDown = False
        RaiseEvent MouseUp(Me, New EventArgs())
    End Sub

    Private Sub PointerMoved(sender As Object, e As PointerRoutedEventArgs)
        Dim position = e.GetCurrentPoint(MyCanvas).Position
        PointerX = position.X
        PointerY = position.Y
        RaiseEvent MouseMove(Me, New EventArgs())
    End Sub
#End Region

End Class
