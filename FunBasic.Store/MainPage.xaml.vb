Imports System.Text
Imports System.Reflection
Imports FunBasic.Library

Public NotInheritable Class MainPage
    Inherits Page

    Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
        Dim output = New StringBuilder()
        TextWindow.Console = New Console(Me.MyConsole)
        Dim theTurtle = Me.MyTurtle
        Me.MyGraphics.Children.Clear()
        Me.MyGraphics.Children.Add(theTurtle)
        Dim graphics = New Graphics(Me.MyGraphics, Me.MyTurtle)
        GraphicsWindow.Graphics = graphics
        Turtle.Graphics = graphics
        FunBasic.Library.Timer.SetTimer(New Timer())

        Dim ffi = New FFI()
        Dim program = Code.Text
        Task.Run(Sub()
                     Try
                         Runtime.Run(program, ffi)
                     Catch ex As Exception
                         TextWindow.Console.WriteLine(ex.Message)                        
                     End Try
                 End Sub)        
    End Sub

End Class
