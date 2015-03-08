Imports ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.IntelliPrompt.Implementation
Imports ActiproSoftware.UI.Xaml.Controls.SyntaxEditor
Imports ActiproSoftware.Text
Imports ActiproSoftware.Text.Lexing
Imports ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Margins

''' <summary>
''' Implements a quick info provider that demonstrates automated quick info session management when
''' hovering over text in the editor and also when hovering over the line number margin.
''' </summary>
Public Class CustomQuickInfoProvider
    Inherits QuickInfoProviderBase

    '///////////////////////////////////////////////////////////////////////////////////////////////////
    ' OBJECT
    '///////////////////////////////////////////////////////////////////////////////////////////////////

    ''' <summary>
    ''' Initializes a new instance of the <c>CustomQuickInfoProvider</c> class.
    ''' </summary>
    Public Sub New()
        MyBase.New("Custom")
    End Sub

    '///////////////////////////////////////////////////////////////////////////////////////////////////
    ' NESTED TYPES
    '///////////////////////////////////////////////////////////////////////////////////////////////////

    ''' <summary>
    ''' Contains context information for the text area.
    ''' </summary>
    Private Class TextRangeContext

        Public Range As TextRange

        ''' <summary>
        ''' Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        ''' </summary>
        ''' <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        ''' <returns>
        ''' true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        ''' </returns>
        ''' <exception cref="NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim context As TextRangeContext = TryCast(obj, TextRangeContext)
            If context IsNot Nothing Then
                Return context.Range.Equals(Me.Range)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Serves as a hash function for a particular type.
        ''' </summary>
        ''' <returns>
        ''' A hash code for the current <see cref="Object"/>.
        ''' </returns>
        Public Overrides Function GetHashCode() As Integer
            Return Me.Range.GetHashCode()
        End Function

    End Class

    ''' <summary>
    ''' Contains context information for the line number margin.
    ''' </summary>
    Private Class LineNumberMarginContext

        Public LineIndex As Integer

        ''' <summary>
        ''' Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Object"/>.
        ''' </summary>
        ''' <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
        ''' <returns>
        ''' true if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, false.
        ''' </returns>
        ''' <exception cref="NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            Dim context As LineNumberMarginContext = TryCast(obj, LineNumberMarginContext)
            If context IsNot Nothing Then
                Return context.LineIndex.Equals(Me.LineIndex)
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Serves as a hash function for a particular type.
        ''' </summary>
        ''' <returns>
        ''' A hash code for the current <see cref="Object"/>.
        ''' </returns>
        Public Overrides Function GetHashCode() As Integer
            Return Me.LineIndex.GetHashCode()
        End Function

    End Class

    '///////////////////////////////////////////////////////////////////////////////////////////////////
    ' PUBLIC PROCEDURES
    '///////////////////////////////////////////////////////////////////////////////////////////////////

    ''' <summary>
    ''' Gets the context <c>Type</c> objects that are supported by this provider, which are the list of custom types
    ''' that are possibly returned by the <see cref="GetContext"/> methods.
    ''' </summary>
    ''' <value>The context <c>Type</c> objects that are supported by this provider.</value>
    Protected Overrides ReadOnly Property ContextTypes() As IEnumerable(Of Type)
        Get
            Return New Type() {GetType(TextRangeContext), GetType(LineNumberMarginContext)}
        End Get
    End Property

    ''' <summary>
    ''' Returns an object describing the quick info context for the specified <see cref="IHitTestResult"/>, if any.
    ''' </summary>
    ''' <param name="hitTestResult">The <see cref="IHitTestResult"/> to examine.</param>
    ''' <returns>
    ''' An object describing the quick info context for the specified <see cref="IHitTestResult"/>, if any.
    ''' A <see langword="null"/> value indicates that no context is available.
    ''' </returns>
    ''' <remarks>
    ''' This method is called in response to mouse events.
    ''' </remarks>
    Public Overrides Function GetContext(ByVal hitTestResult As IHitTestResult) As Object
        Select Case hitTestResult.Type
            Case HitTestResultType.ViewTextAreaOverCharacter
                ' Over a character... this is what the default base method implementation does:                
                Return Me.GetContext(hitTestResult.View, hitTestResult.Offset)
            Case HitTestResultType.ViewMargin
                ' Over a margin
                If hitTestResult.ViewMargin.Key = EditorViewMarginKeys.LineNumber Then
                    ' Over the line number margin
                    Dim context As New LineNumberMarginContext()
                    context.LineIndex = hitTestResult.Position.Line
                    Return context
                End If
        End Select

        ' No context
        Return Nothing
    End Function

    ''' <summary>
    ''' Returns an object describing the quick info context for the specified text offset, if any.
    ''' </summary>
    ''' <param name="view">The <see cref="IEditorView"/> in which the offset is located.</param>
    ''' <param name="offset">The text offset to examine.</param>
    ''' <returns>
    ''' An object describing the quick info context for the specified text offset, if any.
    ''' A <see langword="null"/> value indicates that no context is available.
    ''' </returns>
    ''' <remarks>
    ''' This method is called in response to keyboard events.
    ''' </remarks>
    Public Overrides Function GetContext(ByVal view As IEditorView, ByVal offset As Integer) As Object
        ' Get the range of the current word
        Dim context As New TextRangeContext()
        context.Range = view.CurrentSnapshot.GetWordTextRange(offset)
        Return context
    End Function

    Dim memberLookup As IDictionary(Of String, Tuple(Of String, String)()) = FunBasic.Library._Library.GetMemberLookup()

    ''' <summary>
    ''' Requests that an <see cref="IQuickInfoSession"/> be opened for the specified <see cref="IEditorView"/>.
    ''' </summary>
    ''' <param name="view">The <see cref="IEditorView"/> that will host the session.</param>
    ''' <param name="context">A context object returned by <see cref="GetContext"/>.</param>
    ''' <returns>
    ''' <c>true</c> if a session was opened; otherwise, <c>false</c>.
    ''' </returns>
    Protected Overrides Function RequestSession(ByVal view As IEditorView, ByVal context As Object) As Boolean
        ' Create a session and assign a context that can be used to identify it
        Dim session As New QuickInfoSession()
        session.Context = context
        Dim textRangeContext As TextRangeContext = TryCast(context, TextRangeContext)
        If textRangeContext IsNot Nothing Then
            Dim position = view.OffsetToPosition(textRangeContext.Range.StartOffset)
            Dim line = view.CurrentSnapshot.Lines(position.Line).Text
            Dim info = Runtime.GetInfo(line, position.Character + 1, memberLookup)
            If info IsNot Nothing Then
                ' Create some marked-up content indicating the token at the offset and the line it's on                
                session.Content = info
                ' Open the session
                session.Open(view, textRangeContext.Range)
                Return True
            End If
        Else
            Dim marginContext As LineNumberMarginContext = TryCast(context, LineNumberMarginContext)
            If marginContext IsNot Nothing Then
                ' Create some marked-up content indicating the line number
                session.Content = New HtmlContentProvider(String.Format("Line number: <b>{0}</b>", marginContext.LineIndex + 1)).GetContent()

                ' Get the margin
                Dim margin As IEditorViewMargin = view.Margins.Item(EditorViewMarginKeys.LineNumber)

                ' Get the view line that contains the line
                Dim viewLine As IEditorViewLine = view.GetViewLine(New TextPosition(marginContext.LineIndex, 0))
                If (margin IsNot Nothing) AndAlso (viewLine IsNot Nothing) Then
                    ' Get line bounds relative to the margin
                    Dim bounds As Rect = view.TransformFromTextArea(viewLine.Bounds)
                    bounds.X = 0
                    bounds.Width = margin.VisualElement.RenderSize.Width

                    ' Open the session
                    session.Open(view, PlacementMode.Bottom, view.VisualElement, bounds)
                    Return True
                End If
            End If
        End If
        Return False
    End Function
End Class