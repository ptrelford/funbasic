Imports System.Reflection
Imports Windows.UI, Windows.UI.Xaml.Shapes

Public Class Graphics
    Implements FunBasic.Library.IGraphics

    Dim Canvas As Canvas
    Dim ColorLookup As New Dictionary(Of String, Color)()

    Public Sub New(canvas As Canvas)
        Me.Canvas = canvas
        PrepareColors()
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
            Return Canvas.Width
        End Get
    End Property

    Public ReadOnly Property Width As Double _
        Implements Library.IGraphics.Width
        Get
            Return Canvas.Height
        End Get
    End Property

    Public Property BackgroundColor As String _
        Implements Library.IGraphics.BackgroundColor
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As String)
            Canvas.Background = New SolidColorBrush(GetColor(value))
        End Set
    End Property

    Public Sub DrawLine(penWidth As Double, _
                        penColor As String, _
                        x1 As Integer, y1 As Integer, _
                        x2 As Integer, y2 As Integer) _
        Implements Library.IGraphics.DrawLine

        Dim line = _
            New Line With {.StrokeThickness = penWidth,
                           .Stroke = New SolidColorBrush(GetColor(penColor)), _
                           .X1 = x1, .Y1 = y1, .X2 = x2, .Y2 = y2}
        Canvas.Children.Add(line)
    End Sub

    Public Sub FillEllipse(brushColor As String, x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer) _
        Implements Library.IGraphics.FillEllipse
        Dim brush = New SolidColorBrush(GetColor(brushColor))
        Dim ellipse = _
            New Ellipse With {.Fill = brush,
                              .Margin = New Thickness(x1, y1, 0, 0),
                              .Width = Math.Abs(x2 - x1), .Height = Math.Abs(y2 - y1)}
        Canvas.Children.Add(ellipse)
    End Sub

End Class
