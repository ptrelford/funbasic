Imports Windows.System.Threading

Public Class Timer
    Implements FunBasic.Library.ITimer

    Dim MyTimer As ThreadPoolTimer

    Dim MyInterval As Integer

    Private Sub Start()
        [Stop]()
        MyTimer = ThreadPoolTimer.CreatePeriodicTimer(AddressOf TimerElapsed, TimeSpan.FromMilliseconds(Interval))
    End Sub

    Private Sub [Stop]()
        If MyTimer IsNot Nothing Then
            MyTimer.Cancel()
            MyTimer = Nothing
        End If
    End Sub

    Private Sub TimerElapsed()
        RaiseEvent Tick(Me, New EventArgs())
    End Sub

    Public Property Interval As Integer _
        Implements Library.ITimer.Interval
        Get
            Return MyInterval
        End Get
        Set(value As Integer)
            MyInterval = value
            Start()
        End Set
    End Property

    Public Event Tick(sender As Object, e As EventArgs) Implements Library.ITimer.Tick

    Public Sub Pause() Implements Library.ITimer.Pause
        [Stop]()
    End Sub

    Public Sub [Resume]() Implements Library.ITimer.Resume
        Start()
    End Sub
End Class
