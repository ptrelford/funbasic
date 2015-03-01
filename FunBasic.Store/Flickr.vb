Imports System.Xml.Linq

Public Class Flickr
    Implements FunBasic.Library.IFlickr

    Dim apiKey = "d1ffb915d0e0661972fe6537184d8338"

    Public Function GetInterestingPhoto() As String _
        Implements Library.IFlickr.GetInterestingPhoto
        Dim url = "https://api.flickr.com/services/rest/?method=flickr.interestingness.getList&api_key=" + apiKey
        Dim doc As XDocument = XDocument.Load(url)
        Return GetRandomPhotoUrl(doc)
    End Function

    Public Function GetTaggedPhoto(tags As String) As String _
        Implements Library.IFlickr.GetTaggedPhoto
        Dim url = "https://api.flickr.com/services/rest/?method=flickr.photos.search&tags=" + tags + "&api_key=" + apiKey
        Dim doc = XDocument.Load(url)
        Return GetRandomPhotoUrl(doc)
    End Function

    Private Function GetRandomPhotoUrl(doc As XDocument)
        Dim photos = doc.Root.Element("photos").Elements("photo")
        Dim count = photos.Count
        If count > 0 Then
            Dim rand = New Random()
            Dim photo = photos(rand.Next(photos.Count()))
            Dim farm = photo.Attribute("farm").Value
            Dim server = photo.Attribute("server").Value
            Dim id = photo.Attribute("id").Value
            Dim secret = photo.Attribute("secret").Value
            Return String.Format("https://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg", farm, server, id, secret)
        Else
            Return ""
        End If
    End Function

End Class
