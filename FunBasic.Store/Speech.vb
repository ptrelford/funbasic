Imports Windows.UI.Core
Imports Windows.Media.SpeechSynthesis

Public Class Speech
    Implements FunBasic.Library.ISpeech

    Dim MyDispatcher As CoreDispatcher

    Public Sub New(dispatcher As CoreDispatcher)
        MyDispatcher = dispatcher
    End Sub

    Public Sub Say(text As String) Implements Library.ISpeech.Say
        Dim synth = New SpeechSynthesizer()
        Dim stream = synth.SynthesizeTextToStreamAsync(text).AsTask.Result
        Dim ignore = MyDispatcher.RunAsync( _
            Windows.UI.Core.CoreDispatcherPriority.Normal,
            Sub()
                Dim el = New MediaElement()
                el.SetSource(stream, stream.ContentType)
                el.Play()
            End Sub)
    End Sub

End Class
