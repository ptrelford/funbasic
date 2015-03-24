Imports FunBasic.Library
Imports Windows.UI.Core, Windows.UI.Text
Imports Windows.UI.Xaml.Shapes, Windows.UI.Xaml.Media.Animation

Public Class Shapes
    Implements IShapes

    Class ShapeInfo
        Public Property Element As UIElement
        Public Property Left As Double
        Public Property Top As Double
        Public Property Opacity As Integer

        Public Sub New()
            Me.Element = Nothing
            Me.Left = 0.0
            Me.Top = 0.0
            Me.Opacity = 0
        End Sub
    End Class

    Dim MyStyle As IStyle
    Dim MyShapesCanvas As Canvas
    Dim MyImages As Images
    Dim MyRenderer As Renderer
    Dim ShapeLookup As New Dictionary(Of String, ShapeInfo)()
    Dim ZIndex As Integer
    Dim ShapesCount As Integer

    Sub New(style As IStyle, canvas As Canvas, imageList As Images, turtle As UIElement, renderer As Renderer)
        Me.MyStyle = style
        Me.MyShapesCanvas = canvas
        Me.MyImages = imageList
        Me.MyRenderer = renderer
        Dim myTurtle = New ShapeInfo With {.Element = turtle}
        ShapeLookup.Add("Turtle", myTurtle)
    End Sub

    Sub Dispatch(action As DispatchedHandler)
        Me.MyRenderer.Dispatch(action)
    End Sub

    Private Sub AddElement(element As UIElement)        
        MyShapesCanvas.Children.Add(element)
        ShapesCount = MyShapesCanvas.Children.Count
    End Sub

#Region "IShapes"

    Public Function AddImage(url As String) As String _
        Implements IShapes.AddImage
        Dim name = "Image" + Guid.NewGuid().ToString()
        Dim shape = New ShapeInfo()
        ShapeLookup.Add(name, shape)
        Dispatch(
            Sub()
                Dim image = MyImages.CreateImage(url)
                image.Name = name
                shape.Element = image
                AddElement(image)
            End Sub)
        Return name
    End Function

    Public Function AddEllipse(width As Double, height As Double) As String _
        Implements IShapes.AddEllipse
        Dim name = "Ellipse" + Guid.NewGuid().ToString()
        Dim shape = New ShapeInfo()
        ShapeLookup.Add(name, shape)
        Dim thickness = MyStyle.PenWidth
        Dim stroke = GetColor(MyStyle.PenColor)
        Dim fill = GetColor(MyStyle.BrushColor)
        Dispatch(
            Sub()
                Dim ellipse = New Ellipse With {.Width = width, .Height = height}
                ellipse.StrokeThickness = thickness
                ellipse.Stroke = New SolidColorBrush(stroke)
                ellipse.Fill = New SolidColorBrush(fill)
                ellipse.Name = name
                shape.Element = ellipse
                AddElement(ellipse)
            End Sub)
        Return name
    End Function

    Public Function AddLine(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double) As String _
        Implements IShapes.AddLine
        Dim name = "Line" + Guid.NewGuid().ToString()
        Dim shape = New ShapeInfo()
        ShapeLookup.Add(name, shape)
        Dim color = GetColor(MyStyle.PenColor)
        Dim thickness = MyStyle.PenWidth
        Dispatch(
            Sub()
                Dim line = New Line With {.X1 = x1, .Y1 = y1, .X2 = x2, .Y2 = y2}
                line.StrokeThickness = thickness
                line.Stroke = New SolidColorBrush(color)
                line.Name = name
                shape.Element = line
                AddElement(line)
            End Sub)
        Return name
    End Function

    Public Function AddTriangle(x1 As Double, y1 As Double, _
                                x2 As Double, y2 As Double, _
                                x3 As Double, y3 As Double) As String _
        Implements IShapes.AddTriangle
        Dim name = "Triangle" + Guid.NewGuid().ToString()
        Dim shape = New ShapeInfo()
        ShapeLookup.Add(name, shape)
        Dim thickness = MyStyle.PenWidth
        Dim stroke = GetColor(MyStyle.PenColor)
        Dim fill = GetColor(MyStyle.BrushColor)
        Dispatch(
            Sub()
                Dim poly = Drawings.CreateTriangle(x1, y1, x2, y2, x3, y3)
                poly.StrokeThickness = thickness
                poly.Stroke = New SolidColorBrush(stroke)
                poly.Fill = New SolidColorBrush(fill)
                poly.Name = name
                shape.Element = poly
                AddElement(poly)
            End Sub)
        Return name
    End Function

    Public Function AddRectangle(width As Double, height As Double) As String _
        Implements IShapes.AddRectangle
        Dim name = "Rectangle" + Guid.NewGuid().ToString()
        Dim shape = New ShapeInfo()
        ShapeLookup.Add(name, shape)
        Dim thickness = MyStyle.PenWidth
        Dim stroke = GetColor(MyStyle.PenColor)
        Dim fill = GetColor(MyStyle.BrushColor)
        Dispatch(
            Sub()
                Dim rectangle = New Rectangle With {.Width = width, .Height = height}
                rectangle.StrokeThickness = thickness
                rectangle.Stroke = New SolidColorBrush(stroke)
                rectangle.Fill = New SolidColorBrush(fill)
                rectangle.Name = name
                shape.Element = rectangle
                AddElement(rectangle)
            End Sub)
        Return name
    End Function

    Public Function AddText(text As String) As String _
        Implements IShapes.AddText
        Dim name = "Text" + Guid.NewGuid().ToString()
        Dim shape = New ShapeInfo()
        ShapeLookup.Add(name, shape)
        Dim foreground = GetColor(MyStyle.BrushColor)
        Dim size = MyStyle.FontSize
        Dim family = MyStyle.FontName
        Dim italic = MyStyle.FontItalic
        Dim bold = MyStyle.FontBold
        Dispatch(
            Sub()
                Dim textBlock = New TextBlock With {.Text = text}
                textBlock.Foreground = New SolidColorBrush(foreground)
                textBlock.FontSize = size
                textBlock.FontFamily = New FontFamily(family)
                textBlock.FontStyle = If(italic, FontStyle.Italic, FontStyle.Normal)
                textBlock.FontWeight = If(bold, FontWeights.Bold, FontWeights.Normal)
                textBlock.Name = name
                shape.Element = textBlock
                AddElement(textBlock)
            End Sub)
        Return name
    End Function

    Public Sub SetText(name As String, text As String) _
        Implements IShapes.SetText
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            Dispatch(
                Sub()
                    Dim textBlock = CType(shape.Element, TextBlock)
                    textBlock.Text = text
                End Sub)
        End If
    End Sub

    Public Function GetLeft(name As String) As Double _
        Implements IShapes.GetLeft
        Dim left = 0
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            left = shape.Left
        End If
        Return left
    End Function

    Public Function GetTop(name As String) As Double _
        Implements IShapes.GetTop
        Dim top = 0
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            top = shape.Top
        End If
        Return top
    End Function

    Public Sub HideShape(name As String) _
        Implements IShapes.HideShape
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            Dispatch(Sub() shape.Element.Visibility = Visibility.Collapsed)
        End If
    End Sub

    Public Sub ShowShape(name As String) _
        Implements IShapes.ShowShape
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            Dispatch(Sub() shape.Element.Visibility = Visibility.Visible)
        End If
    End Sub

    Public Sub Remove(name As String) _
        Implements IShapes.Remove
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            ShapeLookup.Remove(name)
            Dispatch(Sub() MyShapesCanvas.Children.Remove(shape.Element))
        End If
    End Sub

    Public Sub Move(name As String, x As Double, y As Double) _
        Implements IShapes.Move
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            shape.Left = x
            shape.Top = y
            Dispatch(
                Sub()
                    Canvas.SetLeft(shape.Element, x)
                    Canvas.SetTop(shape.Element, y)
                    ' Handle ZIndex when elements added via Drawing
                    If MyShapesCanvas.Children.Count > ShapesCount Then
                        ShapesCount = MyShapesCanvas.Children.Count
                        ZIndex += 1
                        Canvas.SetZIndex(shape.Element, ZIndex)
                    End If
                End Sub)
        End If
    End Sub

    Public Sub Animate(name As String, x As Double, y As Double, duration As Integer) _
        Implements IShapes.Animate
        If name = "" Then Return
        Dispatch(
            Sub()
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

    Public Sub Rotate(name As String, angle As Double) _
        Implements IShapes.Rotate
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            Dispatch(
                Sub()
                    Dim transform As New RotateTransform()
                    Dim el = CType(shape.Element, FrameworkElement)
                    transform.CenterX = el.ActualWidth / 2.0
                    transform.CenterY = el.ActualHeight / 2.0
                    transform.Angle = angle
                    shape.Element.RenderTransform = transform
                End Sub)
        End If
    End Sub

    Public Sub Zoom(name As String, scaleX As Double, scaleY As Double) _
        Implements IShapes.Zoom
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            Dispatch(
                Sub()
                    Dim transform As New ScaleTransform()
                    shape.Element.RenderTransform = transform
                    Dim el = CType(shape.Element, FrameworkElement)
                    Dim centerX = CreateDivideBy2Binding(name, "ActualWidth")
                    BindingOperations.SetBinding(transform, ScaleTransform.CenterXProperty, centerX)
                    Dim centerY = CreateDivideBy2Binding(name, "ActualHeight")
                    BindingOperations.SetBinding(transform, ScaleTransform.CenterYProperty, centerY)
                    transform.ScaleX = scaleX
                    transform.ScaleY = scaleY
                End Sub)
        End If
    End Sub

    Private Function CreateDivideBy2Binding(elementName As String, propertyName As String) As Binding
        Dim binding = New Binding()
        binding.ElementName = elementName
        binding.Path = New PropertyPath(propertyName)
        binding.Converter = New DivideByTwoConverter()
        Return binding
    End Function

    Public Function GetOpacity(name As String) As Integer _
        Implements IShapes.GetOpacity
        Dim opacity = 0
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then
            opacity = shape.Opacity
        End If
        Return opacity
    End Function

    Public Sub SetOpacity(name As String, value As Integer) _
        Implements IShapes.SetOpacity
        Dim shape As ShapeInfo = Nothing
        If ShapeLookup.TryGetValue(name, shape) Then            
            shape.Opacity = value
            Dispatch(Sub() shape.Element.Opacity = CType(value, Double) / 100.0)
        End If
    End Sub

#End Region

End Class
