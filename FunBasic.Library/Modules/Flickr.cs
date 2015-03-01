namespace FunBasic.Library
{
   public static class Flickr
   {
      private static IFlickr _flickr;

      internal static void Init(IFlickr flickr)
      {
         _flickr = flickr;
      }

      public static string GetPictureOfMoment()
      {
         return _flickr.GetInterestingPhoto();
      }

      public static string GetRandomPicture(string tag)
      {
         return _flickr.GetTaggedPhoto(tag);
      }
   }
}
