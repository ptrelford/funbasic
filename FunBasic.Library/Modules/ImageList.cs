namespace FunBasic.Library
{
   public static class ImageList
   {
      private static IImages _images;

      internal static void Init(IImages images)
      {
         _images = images;
      }

      public static int GetWidthOfImage(string name)
      {
         return _images.GetImageWidth(name);
      }

      public static int GetHeightOfImage(string name)
      {
         return _images.GetImageHeight(name);
      }

      public static string LoadImage(string url)
      {
         return _images.LoadImage(url);
      }
   }
}
