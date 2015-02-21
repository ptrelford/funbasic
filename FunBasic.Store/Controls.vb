Imports Windows.UI.Core
Imports FunBasic.Library

Public Class Controls
    Implements IControls, IDisposable

    Public Class Control
        Public Property Element As UIElement
        Public Property Caption As String
        Public Property Unsubscribe As Action
    End Class

    Dim MyCanvas As Canvas
    Dim MyStyle As IStyle
    Dim Clicked As String
    Dim ControlLookup = New Dictionary(Of String, Control)()

    Public Sub New(style As IStyle, canvas As Canvas)
        Me.MyCanvas = canvas
        Me.MyStyle = style
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        For Each control As Control In ControlLookup.Values
            If control.Unsubscribe IsNot Nothing Then
                control.Unsubscribe.Invoke()
                control.Unsubscribe = Nothing
            End If
        Next
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
                     button.Foreground = New SolidColorBrush(ColorLookup.GetColor(MyStyle.BrushColor))
                     AddHandler button.Click, AddressOf ButtonClick
                     control.Unsubscribe = Sub() RemoveHandler button.Click, AddressOf ButtonClick
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
                     textBox.Foreground = New SolidColorBrush(ColorLookup.GetColor(MyStyle.BrushColor))
                     AddHandler textBox.TextChanged, AddressOf TextChanged
                     control.Unsubscribe = Sub() RemoveHandler textBox.TextChanged, AddressOf TextChanged
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
                     textBox.Foreground = New SolidColorBrush(ColorLookup.GetColor(MyStyle.BrushColor))
                     AddHandler textBox.TextChanged, AddressOf TextChanged
                     control.Unsubscribe = Sub() RemoveHandler textBox.TextChanged, AddressOf TextChanged
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
        Dim control = ControlLookup(name)
        Return control.Caption
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
        control.Caption = text
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

    Private Sub TextChanged(sender As Object, e As TextChangedEventArgs)
        Dim textBox = CType(sender, TextBox)
        Dim control = ControlLookup(textBox.Name)
        control.Caption = textBox.Text
    End Sub

    Public Event ButtonClicked(sender As Object, e As EventArgs) _
        Implements Library.IControls.ButtonClicked

End Class
