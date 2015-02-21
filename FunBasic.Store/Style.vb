Public Class Style
    Implements FunBasic.Library.IStyle

    Public Property BrushColor As String Implements Library.IStyle.BrushColor
    Public Property FontBold As Boolean Implements Library.IStyle.FontBold
    Public Property FontItalic As Boolean Implements Library.IStyle.FontItalic
    Public Property FontName As String Implements Library.IStyle.FontName
    Public Property FontSize As Double Implements Library.IStyle.FontSize
    Public Property PenColor As String Implements Library.IStyle.PenColor
    Public Property PenWidth As Double Implements Library.IStyle.PenWidth

End Class
