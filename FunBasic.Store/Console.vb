Imports System.Text

Public Class Console
    Implements FunBasic.Library.IConsole

    Dim MyTextBox As TextBox

    Sub New(textBox As TextBox)
        textBox.Text = ""
        Me.MyTextBox = textBox
    End Sub

    Public Sub WriteLine(value As Object) Implements Library.IConsole.WriteLine
        Dim ignore =
            MyTextBox.Dispatcher.RunAsync( _
            Windows.UI.Core.CoreDispatcherPriority.Normal, _
                Sub()
                    MyTextBox.Text = MyTextBox.Text + value.ToString() + vbCrLf
                End Sub)
    End Sub

End Class
