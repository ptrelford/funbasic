Imports System.Text, System.Reflection, System.Threading
Imports FunBasic.Interpreter, FunBasic.Library, FunBasic.Store
Imports ActiproSoftware.Text, ActiproSoftware.Text.Implementation
Imports Windows.UI

' The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

''' <summary>
''' A page that displays details for a single item within a group while allowing
''' gestures to flip through other items belonging to the same group.
''' </summary>
Public NotInheritable Class ItemDetailPage
    Inherits Page

    ''' <summary>
    ''' NavigationHelper is used on each page to aid in navigation and 
    ''' process lifetime management
    ''' </summary>
    Public ReadOnly Property NavigationHelper As Common.NavigationHelper
        Get
            Return Me._navigationHelper
        End Get
    End Property
    Private _navigationHelper As Common.NavigationHelper

    ''' <summary>
    ''' This can be changed to a strongly typed view model.
    ''' </summary>
    Public ReadOnly Property DefaultViewModel As Common.ObservableDictionary
        Get
            Return Me._defaultViewModel
        End Get
    End Property
    Private _defaultViewModel As New Common.ObservableDictionary()


    Public Sub New()
        InitializeComponent()
        Me._navigationHelper = New Common.NavigationHelper(Me)
        AddHandler Me._navigationHelper.LoadState,
            AddressOf NavigationHelper_LoadState

        Code.Document.Language = LoadLanguageDefinitionFromResourceStream("FunBasic.langdef")
    End Sub

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="sender">
    ''' The source of the event; typically <see cref="NavigationHelper"/>
    ''' </param>
    ''' <param name="e">Event data that provides both the navigation parameter passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested and
    ''' a dictionary of state preserved by this page during an earlier
    ''' session.  The state will be null the first time a page is visited.</param>
    Private Async Sub NavigationHelper_LoadState(sender As Object, e As Common.LoadStateEventArgs)
        ' TODO: Create an appropriate data model for your problem domain to replace the sample data
        Dim item As Data.SampleDataItem = Await Data.SampleDataSource.GetItemAsync(DirectCast(e.NavigationParameter, String))
        Me.DefaultViewModel("Item") = item

        Dim dataUri As New Uri(String.Format("ms-appx:///Programs/{0}", item.Program))

        Dim file As StorageFile = Await StorageFile.GetFileFromApplicationUriAsync(dataUri)
        Dim jsonText As String = Await FileIO.ReadTextAsync(file)
        Me.Code.Text = jsonText
    End Sub

#Region "NavigationHelper registration"

    ''' The methods provided in this section are simply used to allow
    ''' NavigationHelper to respond to the page's navigation methods.
    ''' 
    ''' Page specific logic should be placed in event handlers for the  
    ''' <see cref="Common.NavigationHelper.LoadState"/>
    ''' and <see cref="Common.NavigationHelper.SaveState"/>.
    ''' The navigation parameter is available in the LoadState method 
    ''' in addition to page state preserved during an earlier session.

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedTo(e)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        _navigationHelper.OnNavigatedFrom(e)
    End Sub
#End Region

    Dim ffi As New FFI()
    Dim timer As New FunBasic.Store.Timer()
    Dim cancelToken As New CancelToken()
    Dim done As ManualResetEvent = New ManualResetEvent(False)

    Private Async Sub StartButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles StartButton.Click
        StartButton.IsEnabled = False
        StopButton.IsEnabled = True
        backButton.IsTabStop = False

        Code.IsEnabled = False    

        cancelToken = New CancelToken()

        Dim program = Code.Text
        Await Task.Run(Sub() Start(program))
    End Sub

    Private Async Sub StopButton_Click(sender As Object, e As RoutedEventArgs) _
        Handles StopButton.Click
        StopButton.IsEnabled = False

        ffi.Unhook()
        timer.Pause()
        cancelToken.Cancel()

        Await Task.Run(Sub() [Stop]())
    End Sub

    Private Sub InitLibrary()
        Dim c = New Console(Me.MyConsole)
        TextWindow.Console = c
        Dim theTurtle = Me.MyTurtle
        Me.MyGraphics.Children.Clear()
        Me.MyGraphics.Background = New SolidColorBrush(Colors.White)
        Me.MyShapes.Children.Clear()
        Me.MyShapes.Children.Add(theTurtle)
        Dim graphics = New Graphics(Me.MyGraphics, Me.MyShapes, Me.MyTurtle)
        GraphicsWindow.Graphics = graphics
        Turtle.Graphics = graphics
        timer.Interval = -1
        FunBasic.Library.Timer.SetTimer(timer)
    End Sub

    Private Async Sub Start(program As String)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, _
                            Sub() InitLibrary())

        Try
            Runtime.Run(program, ffi, cancelToken)
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine(ex)
            TextWindow.Console.WriteLine(ex.Message)
        Finally
            done.Set()
        End Try
        If Not ffi.IsHooked Then
            timer.Pause()
            Await EnableStartButton()
        End If
    End Sub

    Private Async Sub [Stop]()
        If done IsNot Nothing Then
            Dim success = done.WaitOne()
            done.Reset()
        End If
        Await EnableStartButton()
    End Sub

    Private Async Function EnableStartButton() As Task
        Await Dispatcher.RunAsync(Core.CoreDispatcherPriority.Normal,
                    Sub()
                        StartButton.IsEnabled = True
                        StopButton.IsEnabled = False
                        backButton.IsTabStop = True
                        Me.Code.IsEnabled = True
                    End Sub)
    End Function

    ''' <summary>
    ''' Loads a language definition (.langdef file) from a resource stream.
    ''' </summary>
    ''' <param name="filename">The filename.</param>
    ''' <returns>The <see cref="ISyntaxLanguage"/> that was loaded.</returns>
    Public Shared Function LoadLanguageDefinitionFromResourceStream(ByVal filename As String) As ISyntaxLanguage
        Dim DefinitionPath = "FunBasic.App."
        Dim path As String = DefinitionPath & filename
        Using stream As Stream = GetType(App).GetTypeInfo().Assembly.GetManifestResourceStream(path)
            If stream IsNot Nothing Then
                Dim serializer As New SyntaxLanguageDefinitionSerializer()
                Return serializer.LoadFromStream(stream)
            Else
                Return SyntaxLanguage.PlainText
            End If
        End Using
    End Function

End Class
