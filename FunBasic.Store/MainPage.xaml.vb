Imports System.Text
Imports System.Reflection
Imports FunBasic.Library

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim output = New StringBuilder()
        TextWindow.Console = New Console(output)
        Me.Graphics.Children.Clear()
        Dim graphics = New Graphics(Me.Graphics)
        GraphicsWindow.Graphics = graphics
        Turtle.Graphics = graphics

        Dim ffi = New FFI()
        Dim program = Code.Text
        Try
            Runtime.Run(program, ffi)
        Catch ex As Exception
            output.AppendLine(ex.Message)
        End Try
        Console.Text = output.ToString()
    End Sub

End Class
