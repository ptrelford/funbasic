namespace FunBasic.Library
{
   public static class ImageList
   {
      public static int GetWidthOfImage(string name)
      {
         return GraphicsWindow.Graphics.GetImageWidth(name);
      }

      public static int GetHeightOfImage(string name)
      {
         return GraphicsWindow.Graphics.GetImageHeight(name);
      }

      public static string LoadImage(string url)
      {
         return GraphicsWindow.Graphics.LoadImage(url);
      }
   }
}
