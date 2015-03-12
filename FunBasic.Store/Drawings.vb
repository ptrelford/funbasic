Imports System.Globalization
Imports Windows.System
Imports Windows.UI, Windows.UI.Core, Windows.UI.Text
Imports Windows.UI.Xaml.Shapes, Windows.UI.Xaml.Media.Animation

Public Class Drawings
    Implements FunBasic.Library.IDrawings

    Dim MyRenderer As New Renderer()
    Dim MyStyle As FunBasic.Library.IStyle
    Dim MyDrawingCanvas As Canvas
    Dim MyShapesCanvas As Canvas
    Dim MyImages As Images

    Public Sub New(style As FunBasic.Library.IStyle, _
                   drawingCanvas As Canvas, _
                   shapesCanvas As Canvas, _
                   imageList As Images, _
                   renderer As Renderer)
        Me.MyStyle = style
        Me.MyDrawingCanvas = drawingCanvas
        Me.MyShapesCanvas = shapesCanvas
        Me.MyImages = imageList
        Me.MyRenderer = renderer
    End Sub

    Private Function GetColor(name As String) As Color
        Return ColorLookup.GetColor(name)
    End Function

    Private Sub Dispatch(handler As DispatchedHandler)
        MyRenderer.Dispatch(handler)
    End Sub

    Private Sub AddElement(element As UIElement)
        If MyShapesCanvas.Children.Count <= 1 Then
            MyDrawingCanvas.Children.Add(element)
        Else
            MyShapesCanvas.Children.Add(element)
        End If
    End Sub

#Region "IDrawing"

    Public Function GetPixel(x As Double, y As Double) As String _
        Implements Library.IDrawings.GetPixel
        Dim color As String = "#FFFFFF"
        Dim handle = New System.Threading.ManualResetEvent(False)
        Dim ignore = _
            MyDrawingCanvas.Dispatcher.RunAsync( _
                CoreDispatcherPriority.Normal, _
                Async Sub()
                    Dim bitmap = New RenderTargetBitmap()
                    Await bitmap.RenderAsync(MyDrawingCanvas)
                    x = x * bitmap.PixelWidth / MyDrawingCanvas.ActualWidth
                    y = y * bitmap.PixelHeight / MyDrawingCanvas.ActualHeight
                    If x >= 0 And x < bitmap.PixelWidth And _
                       y >= 0 And y < bitmap.PixelHeight Then
                        Dim pixels = Await bitmap.GetPixelsAsync()
                        Dim stream = pixels.AsStream()
                        Dim offset = CType(x, Integer) + (bitmap.PixelWidth * CType(y, Integer))
                        stream.Seek(offset * 4, SeekOrigin.Begin)
                        color = String.Format("#{0:X2}{1:X2}{2:X2}", _
                            stream.ReadByte(), _
                            stream.ReadByte(), _
                            stream.ReadByte())
                    End If
                    handle.Set()
                End Sub)
        handle.WaitOne()
        Return color
    End Function

    Public Sub SetPixel(x As Double, y As Double, color As String) _
        Implements Library.IDrawings.SetPixel
        Dim fillColor = GetColor(color)
        Dim thickness = MyStyle.PenWidth
        Dispatch(Sub()
                     Dim rectangle = CreateRectangle(1.0, 1.0)
                     rectangle.Fill = New SolidColorBrush(fillColor)
                     rectangle.Margin = New Thickness(x, y, 0, 0)
                     AddElement(rectangle)
                 End Sub)
    End Sub

    Public Sub DrawEllipse(x As Double, y As Double, _
                           width As Double, height As Double) _
        Implements Library.IDrawings.DrawEllipse
        Dim color = GetColor(MyStyle.PenColor)
        Dim thickness = MyStyle.PenWidth
        Dispatch(Sub()
                     Dim ellipse = CreateEllipse(x, y)
                     ellipse.StrokeThickness = thickness
                     ellipse.Stroke = New SolidColorBrush(color)
                     ellipse.Margin = New Thickness(x, y, 0, 0)
                     AddElement(ellipse)
                 End Sub)
    End Sub

    Public Sub DrawLine(x1 As Double, y1 As Double, _
                        x2 As Double, y2 As Double) _
        Implements Library.IDrawings.DrawLine
        Dim color = GetColor(MyStyle.PenColor)
        Dim thickness = MyStyle.PenWidth
        Dispatch(Sub()
                     Dim line = CreateLine(x1, y1, x2, y2)
                     line.Stroke = New SolidColorBrush(color)
                     line.StrokeThickness = thickness
                     AddElement(line)
                 End Sub)
    End Sub

    Private Function CreateLine(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double)
        Return New Line With {.X1 = x1, .Y1 = y1, .X2 = x2, .Y2 = y2}
    End Function

    Public Sub DrawTriangle(x1 As Double, y1 As Double, _
                            x2 As Double, y2 As Double, _
                            x3 As Double, y3 As Double) _
        Implements Library.IDrawings.DrawTriangle
        Dim color = GetColor(MyStyle.PenColor)
        Dim thickness = MyStyle.PenWidth
        Dispatch(Sub()
                     Dim poly = CreateTriangle(x1, y1, x2, y2, x3, y3)
                     poly.StrokeThickness = thickness
                     poly.Stroke = New SolidColorBrush(color)
                     AddElement(poly)
                 End Sub)
    End Sub

    Shared Function CreateTriangle(x1 As Double, y1 As Double, _
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
        Implements Library.IDrawings.DrawRectangle
        Dim color = GetColor(MyStyle.PenColor)
        Dim thickness = MyStyle.PenWidth
        Dispatch(Sub()
                     Dim rectangle = CreateRectangle(width, height)
                     rectangle.StrokeThickness = thickness
                     rectangle.Stroke = New SolidColorBrush(color)
                     rectangle.Margin = New Thickness(x, y, 0, 0)
                     AddElement(rectangle)
                 End Sub)
    End Sub

    Private Function CreateRectangle(width As Double, height As Double) As Rectangle
        Return New Rectangle With {.Width = width, .Height = height}
    End Function

    Public Sub DrawImage(url As String, x As Double, y As Double) _
        Implements Library.IDrawings.DrawImage
        Dispatch(Sub()
                     Dim image = MyImages.CreateImage(url)
                     Canvas.SetLeft(image, x)
                     Canvas.SetTop(image, y)
                     AddElement(image)
                 End Sub)
    End Sub

    Public Sub DrawResizedImage(url As String, x As Double, y As Double, width As Double, height As Double) _
        Implements Library.IDrawings.DrawResizedImage
        Dispatch(Sub()
                     Dim image = MyImages.CreateImage(url)
                     Canvas.SetLeft(image, x)
                     Canvas.SetTop(image, y)
                     image.Width = width
                     image.Height = height
                     AddElement(image)
                 End Sub)
    End Sub

    Public Sub DrawText(x As Double, y As Double, _
                        text As String) _
    Implements Library.IDrawings.DrawText
        Dim foreground = GetColor(MyStyle.BrushColor)
        Dim size = MyStyle.FontSize
        Dim family = MyStyle.FontName
        Dispatch(Sub()
                     Dim textBlock = CreateTextBlock(text)
                     textBlock.Foreground = New SolidColorBrush(foreground)
                     textBlock.FontSize = size
                     textBlock.FontFamily = New FontFamily(family)
                     textBlock.FontStyle = If(MyStyle.FontItalic, FontStyle.Italic, FontStyle.Normal)
                     textBlock.FontWeight = If(MyStyle.FontBold, FontWeights.Bold, FontWeights.Normal)
                     textBlock.Margin = New Thickness(x, y, 0, 0)
                     AddElement(textBlock)
                 End Sub)
    End Sub

    Public Sub DrawBoundText(x As Double, y As Double, width As Double, text As String) _
        Implements Library.IDrawings.DrawBoundText
        Dim foreground = GetColor(MyStyle.BrushColor)
        Dim size = MyStyle.FontSize
        Dim family = MyStyle.FontName
        Dispatch(Sub()
                     Dim textBlock = CreateTextBlock(text)
                     textBlock.Foreground = New SolidColorBrush(foreground)
                     textBlock.FontSize = size
                     textBlock.FontFamily = New FontFamily(family)
                     textBlock.FontStyle = If(MyStyle.FontItalic, FontStyle.Italic, FontStyle.Normal)
                     textBlock.FontWeight = If(MyStyle.FontBold, FontWeights.Bold, FontWeights.Normal)
                     textBlock.Margin = New Thickness(x, y, 0, 0)
                     textBlock.MaxWidth = width
                     AddElement(textBlock)
                 End Sub)
    End Sub

    Private Function CreateTextBlock(text As String) As TextBlock
        Return New TextBlock With {.Text = text}
    End Function

    Public Sub FillTriangle(x1 As Double, y1 As Double, _
                        x2 As Double, y2 As Double, _
                        x3 As Double, y3 As Double) _
    Implements Library.IDrawings.FillTriangle
        Dim color = GetColor(MyStyle.BrushColor)
        Dispatch(Sub()
                     Dim poly = CreateTriangle(x1, y1, x2, y2, x3, y3)
                     poly.Fill = New SolidColorBrush(color)
                     AddElement(poly)
                 End Sub)
    End Sub

    Public Sub FillRectangle(x As Double, y As Double, _
                             width As Double, height As Double) _
        Implements Library.IDrawings.FillRectangle
        Dim color = GetColor(MyStyle.BrushColor)
        Dispatch(Sub()
                     Dim rectangle = CreateRectangle(width, height)
                     rectangle.Fill = New SolidColorBrush(color)
                     rectangle.Margin = New Thickness(x, y, 0, 0)
                     AddElement(rectangle)
                 End Sub)
    End Sub

    Public Sub FillEllipse(x As Double, y As Double, _
                           width As Double, height As Double) _
        Implements Library.IDrawings.FillEllipse
        Dim color = GetColor(MyStyle.BrushColor)
        Dispatch(Sub()
                     Dim ellipse = CreateEllipse(width, height)
                     ellipse.Fill = New SolidColorBrush(color)
                     ellipse.Margin = New Thickness(x, y, 0, 0)
                     AddElement(ellipse)
                 End Sub)
    End Sub

    Function CreateEllipse(width As Double, height As Double) As Ellipse
        Return New Ellipse With {.Width = width, .Height = height}
    End Function
#End Region

End Class
