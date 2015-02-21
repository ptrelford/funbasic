namespace FunBasic.Library
{
   public static class Desktop
   {
      private static ISurface _surface;

      public static void Init(ISurface surface)
      {
         _surface = surface;
      }

      public static int Width {
         get { return (int)_surface.Width; }
      }

      public static int Height
      {
         get { return (int)_surface.Height; }
      }
   }
}
