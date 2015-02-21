Imports Windows.UI.Core

Public Class Images
    Implements Library.IImages

    Dim MyDispatcher As CoreDispatcher
    Dim MyImageList As New Dictionary(Of String, Image)()

    Public Sub New(dispatcher As CoreDispatcher)
        Me.MyDispatcher = dispatcher
    End Sub

    Public Function GetImageWidth(name As String) As Integer _
        Implements Library.IImages.GetImageWidth
        Dim image As Image = Nothing
        If MyImageList.TryGetValue(name, image) Then
            Return image.Width
        Else
            Return 0
        End If
    End Function

    Public Function GetImageHeight(name As String) As Integer _
        Implements Library.IImages.GetImageHeight
        Dim image As Image = Nothing
        If MyImageList.TryGetValue(name, image) Then
            Return image.Height
        Else
            Return 0
        End If
    End Function

    Public Function LoadImage(url As String) As String _
        Implements Library.IImages.LoadImage
        Dim name = "ImageList" + Guid.NewGuid.ToString()
        MyDispatcher.RunAsync( _
            CoreDispatcherPriority.Normal, _
            Sub()
                Dim image = CreateImage(url)
                image.Name = name
                MyImageList.Add(name, image)
            End Sub).AsTask().Wait()
        Return name
    End Function

    Public Function CreateImage(url As String) As Image
        Dim image As Image = Nothing
        If url.StartsWith("ImageList") Then
            If MyImageList.TryGetValue(url, image) Then
                Return New Image With {.Source = image.Source}
            Else
                System.Diagnostics.Debug.WriteLine("Failed to load image")
                Return New Image()
            End If
        Else
            Dim bitmap = New BitmapImage With {.UriSource = New Uri(url)}
            Return New Image With {.Source = bitmap}
        End If
    End Function

End Class
