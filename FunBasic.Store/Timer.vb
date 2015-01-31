Imports Windows.System.Threading

Public Class Timer
    Implements FunBasic.Library.ITimer

    Dim MyTimer As ThreadPoolTimer

    Dim MyInterval As Integer

    Public Property Interval As Integer _
        Implements Library.ITimer.Interval
        Get
            Return MyInterval
        End Get
        Set(value As Integer)
            MyInterval = value
            MyTimer = ThreadPoolTimer.CreatePeriodicTimer(AddressOf TimerElapsed, TimeSpan.FromMilliseconds(value))
        End Set
    End Property

    Public Event Tick(sender As Object, e As EventArgs) Implements Library.ITimer.Tick

    Private Sub TimerElapsed()
        RaiseEvent Tick(Me, New EventArgs())
    End Sub

End Class
