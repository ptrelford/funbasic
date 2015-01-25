namespace FunBasic.Library
{
   public static class GraphicsWindow
   {
      private static IGraphics _graphics;

      public static IGraphics Graphics
      {
         get { return _graphics; }
         set
         {
            _graphics = value;
            PenWidth = 2.0;
            PenColor = "black";
         }

      }

      public static double PenWidth { get; set; }
      public static string PenColor { get; set; }

      public static void DrawLine(object x1, object y1, object x2, object y2)
      {
         Graphics.DrawLine(PenWidth,PenColor,(int)x1,(int)y1,(int)x2,(int)y2);
      }
   }
}
