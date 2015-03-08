Imports Windows.System, Windows.UI.Core

Public Class Keyboard
    Implements Library.IKeyboard, IDisposable

    Dim MyLastKey As VirtualKey

    Public Sub New()
        Dim window = CoreWindow.GetForCurrentThread()
        AddHandler window.KeyDown, AddressOf OnKeyDown
        AddHandler window.KeyUp, AddressOf OnKeyUp
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dim window = CoreWindow.GetForCurrentThread()
        RemoveHandler window.KeyDown, AddressOf OnKeyDown
        RemoveHandler window.KeyUp, AddressOf OnKeyUp
    End Sub

#Region "Keyboard"
    Public Event KeyDown(sender As Object, e As EventArgs) _
        Implements Library.IKeyboard.KeyDown
    Public Event KeyUp(sender As Object, e As EventArgs) _
        Implements Library.IKeyboard.KeyUp

    Public ReadOnly Property LastKey As String Implements Library.IKeyboard.LastKey
        Get
            Select Case MyLastKey
                Case VirtualKey.Number0 : Return "D0"
                Case VirtualKey.Number1 : Return "D1"
                Case VirtualKey.Number2 : Return "D2"
                Case VirtualKey.Number3 : Return "D3"
                Case VirtualKey.Number4 : Return "D4"
                Case VirtualKey.Number5 : Return "D5"
                Case VirtualKey.Number6 : Return "D6"
                Case VirtualKey.Number7 : Return "D7"
                Case VirtualKey.Number8 : Return "D8"
                Case VirtualKey.Number9 : Return "D9"
            End Select
            Return [Enum].GetName(GetType(VirtualKey), MyLastKey)
        End Get
    End Property

    Private Sub OnKeyDown(sender As CoreWindow, args As KeyEventArgs)
        MyLastKey = args.VirtualKey
        RaiseEvent KeyDown(Me, New EventArgs)
    End Sub

    Private Sub OnKeyUp(sender As CoreWindow, args As KeyEventArgs)
        MyLastKey = args.VirtualKey
        RaiseEvent KeyUp(Me, New EventArgs)
    End Sub

#End Region

End Class
