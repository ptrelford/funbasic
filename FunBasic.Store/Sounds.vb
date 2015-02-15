Public Class Sounds
    Implements FunBasic.Library.ISounds

    Dim BeepBeep As MediaElement
    Dim BellRing As MediaElement
    Dim Chime As MediaElement
    Dim Click As MediaElement
    Dim Pause As MediaElement

    Public Sub New(beepBeep As MediaElement, _
                   bellRing As MediaElement, _
                   chime As MediaElement, _
                   click As MediaElement, _
                   pause As MediaElement)
        Me.BeepBeep = beepBeep
        Me.BellRing = bellRing
        Me.Chime = chime
        Me.Click = click
        Me.Pause = pause        
    End Sub

    Public Sub PlayStockSound(name As String, wait As Boolean) _
        Implements Library.ISounds.PlayStockSound
        Dim ignore =
            Me.BellRing.Dispatcher.RunAsync( _
                Windows.UI.Core.CoreDispatcherPriority.Normal, _
                Sub()
                    If name = "BellRing" Then
                        Me.BellRing.Play()
                    ElseIf name = "Chime" Then
                        Me.Chime.Play()
                    ElseIf name = "Click" Then
                        Me.Click.Play()
                    ElseIf name = "Chimes" Then
                        Me.Pause.Play()
                    End If
                End Sub)
    End Sub

End Class
