Imports Windows.UI.Core

Public Class Controls
    Implements FunBasic.Library.IControls

    Public Class Control
        Public Property Element As UIElement
        Public Property Caption As String
    End Class

    Dim MyCanvas As Canvas
    Dim Clicked As String
    Dim ControlLookup = New Dictionary(Of String, Control)()

    Public Sub New(canvas As Canvas)
        Me.MyCanvas = canvas
    End Sub

    Private Async Sub Dispatch(action As DispatchedHandler)
        Await Me.MyCanvas.Dispatcher.RunAsync( _
                Windows.UI.Core.CoreDispatcherPriority.Normal, action)
    End Sub

    Public Function AddButton(text As String, x As Integer, y As Integer) As String _
        Implements Library.IControls.AddButton
        Dim name = "Button" + Guid.NewGuid().ToString()
        Dim control = New Control With {.Caption = text}
        ControlLookup.Add(name, control)
        Dispatch(Sub()
                     Dim button = New Button With {.Content = text}
                     button.Name = name
                     AddHandler button.Click, AddressOf ButtonClick
                     'TODO: unhook
                     Canvas.SetLeft(button, x)
                     Canvas.SetTop(button, y)
                     MyCanvas.Children.Add(button)
                     control.Element = button
                 End Sub)
        Return name
    End Function

    Public Function AddTextBox(x As Integer, y As Integer) As String _
        Implements Library.IControls.AddTextBox
        Dim name = "TextBox" + Guid.NewGuid().ToString()
        Dim control = New Control()
        ControlLookup.Add(name, control)
        Dispatch(Sub()
                     Dim textBox = New TextBox()
                     textBox.Name = name
                     Canvas.SetLeft(textBox, x)
                     Canvas.SetTop(textBox, y)
                     MyCanvas.Children.Add(textBox)
                     control.Element = textBox
                 End Sub)
        Return name
    End Function

    Public Function AddMultiLineTextBox(x As Integer, y As Integer) As String _
        Implements Library.IControls.AddMultiLineTextBox
        Dim name = "TextBox" + Guid.NewGuid().ToString()
        Dim control = New Control()
        ControlLookup.Add(name, control)
        Dispatch(Sub()
                     Dim textBox = New TextBox With {.AcceptsReturn = True}
                     textBox.Name = name
                     Canvas.SetLeft(textBox, x)
                     Canvas.SetTop(textBox, y)
                     MyCanvas.Children.Add(textBox)
                     control.Element = textBox
                 End Sub)
        Return name
    End Function

    Public Function GetButtonCaption(name As String) As String _
        Implements Library.IControls.GetButtonCaption
        Dim control = ControlLookup(name)
        Return control.Caption
    End Function

    Public Function GetTextBoxText(name As String) As String _
        Implements Library.IControls.GetTextBoxText
        Return ""
    End Function

    Public ReadOnly Property LastClickedButton As String _
        Implements Library.IControls.LastClickedButton
        Get
            Return Clicked
        End Get
    End Property

    Public Sub SetSize(name As String, width As Integer, height As Integer) _
        Implements Library.IControls.SetSize
        Dim control = ControlLookup(name)
        Dispatch(Sub()
                     Dim element = CType(control.Element, FrameworkElement)
                     element.Width = width
                     element.Height = height
                 End Sub)
    End Sub

    Public Sub SetTextBoxText(name As String, text As String) _
        Implements Library.IControls.SetTextBoxText
        Dim control = ControlLookup(name)
        Dispatch(Sub()
                     Dim textBox = CType(control.Element, TextBox)
                     textBox.Text = text
                 End Sub)
    End Sub

    Private Sub ButtonClick(sender As Object, e As RoutedEventArgs)
        Dim button = CType(sender, Button)
        Clicked = button.Name
        RaiseEvent ButtonClicked(Me, New EventArgs())
    End Sub

    Public Event ButtonClicked(sender As Object, e As EventArgs) _
        Implements Library.IControls.ButtonClicked

End Class
