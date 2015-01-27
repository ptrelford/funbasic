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

      public static string BackgroundColor {
         get { return _graphics.BackgroundColor; }
         set { _graphics.BackgroundColor = value; } 
      }
      public static string BrushColor { get; set; }
      public static string PenColor { get; set; }
      public static double PenWidth { get; set; }
      
      public static void DrawLine(object x1, object y1, object x2, object y2)
      {
         Graphics.DrawLine(PenWidth,PenColor,(int)x1,(int)y1,(int)x2,(int)y2);
      }

      public static void FillEllipse(object x1, object y1, object width, object height)
      {
         Graphics.FillEllipse(BrushColor,(int)x1,(int)y1,(int)width,(int)height);
      }
      
      public static void SetPixel(object x, object y, object color)
      {
         Graphics.DrawLine(2.0, (string)color, (int)x, (int)y, ((int)x) + 2, ((int)y));
      }
   }
}
