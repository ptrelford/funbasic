Imports System.Xml.Linq

Public Class Flickr
    Implements FunBasic.Library.IFlickr

    Public Function GetInterestingPhoto() As String Implements Library.IFlickr.GetInterestingPhoto
        Dim apiKey = "d1ffb915d0e0661972fe6537184d8338"
        Dim url = "https://api.flickr.com/services/rest/?method=flickr.interestingness.getList&api_key=" + apiKey
        Dim doc = XDocument.Load(url)
        Dim photos = doc.Root.Element("photos").Elements("photo")
        Dim rand = New Random()
        Dim photo = photos(rand.Next(photos.Count()))
        Dim farm = photo.Attribute("farm").Value
        Dim server = photo.Attribute("server").Value
        Dim id = photo.Attribute("id").Value
        Dim secret = photo.Attribute("secret").Value
        Return String.Format("https://farm{0}.staticflickr.com/{1}/{2}_{3}.jpg", farm, server, id, secret)
    End Function

End Class
