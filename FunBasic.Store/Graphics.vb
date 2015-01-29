Imports System.Reflection
Imports Windows.UI, Windows.UI.Xaml.Shapes
Imports Windows.UI.Core

Public Class Graphics
    Implements FunBasic.Library.IGraphics

    Dim MyCanvas As Canvas
    Dim ColorLookup As New Dictionary(Of String, Color)()
    Dim ShapeLookup As New Dictionary(Of String, UIElement)()

    Public Sub New(canvas As Canvas)
        Me.MyCanvas = canvas
        PrepareColors()
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

    Public Function CreateLine(x1 As Integer, y1 As Integer, _
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

    Public Function CreateImage(url As String) As Image
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

    Public Function CreateTextBlock(text As String) As TextBlock
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
        Dim name = "Image"
        Dim image As Image = Nothing
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                image = CreateImage(url)
                MyCanvas.Children.Add(image)
            End Sub).AsTask().Wait()
        ShapeLookup.Add(name, Image)
        Return name
    End Function

    Public Function AddLine(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) As String _
        Implements Library.IGraphics.AddLine
        Dim name = "Line"
        Dim line As Line = Nothing
        MyCanvas.Dispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                line = CreateLine(x1, y1, x2, y2)
                ShapeLookup.Add(name, line)
            End Sub).AsTask().Wait()
        MyCanvas.Children.Add(line)
        Return name
    End Function

    Public Function AddText(text As String) As String _
        Implements Library.IGraphics.AddText
        Dim name = "Text"
        Dim textBlock As TextBlock = Nothing
        MyCanvas.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, _
            Sub()
                textBlock = CreateTextBlock(text)
                MyCanvas.Children.Add(textBlock)
            End Sub).AsTask().Wait()
        ShapeLookup.Add(name, textBlock)
        Return name
    End Function

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

    Public Sub SetOpacity(name As String, value As Integer) _
        Implements Library.IGraphics.SetOpacity
        Dim shape = ShapeLookup(name)
        Dispatch(Sub() shape.Opacity = CType(value, Double) / 100.0)
    End Sub

End Class
