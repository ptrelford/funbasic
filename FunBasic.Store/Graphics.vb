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

    Private Sub PrepareColors()
        Dim ti = GetType(Colors)
        Dim colors = ti.GetRuntimeProperties()
        For Each pi In colors
            Dim color = pi.GetMethod().Invoke(Nothing, New Object() {})
            ColorLookup.Add(pi.Name.ToLower(), color)
        Next
    End Sub

    Public Sub DrawLine(penWidth As Double, _
                        penColor As String, _
                        x1 As Integer, y1 As Integer, _
                        x2 As Integer, y2 As Integer) _
        Implements Library.IGraphics.DrawLine

        Dim line = _
            New Line With {.StrokeThickness = penWidth,
                           .Stroke = New SolidColorBrush(ColorLookup(penColor)), _
                           .X1 = x1, .Y1 = y1, .X2 = x2, .Y2 = y2}
        Canvas.Children.Add(line)
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
End Class
