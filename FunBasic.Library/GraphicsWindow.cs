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
            BrushColor = "black";
            PenColor = "black";
            FontSize = 12;
            FontName = "Tahoma";
         }
      }

      public static string Title { get; set; }

      public static bool CanResize { get { return false; } }

      public static double Width
      {
         get { return _graphics.Width; }
         set { }
      }

      public static double Height
      {
         get { return _graphics.Height; }
         set { }
      }

      public static string BackgroundColor 
      {
         get { return Graphics.BackgroundColor; }
         set { Graphics.BackgroundColor = value; } 
      }
      
      public static string BrushColor 
      {
         get { return Graphics.BrushColor; }
         set { Graphics.BrushColor = value; }
      }

      public static string PenColor 
      {
         get { return Graphics.PenColor; }
         set { Graphics.PenColor = value; }
      }

      public static double PenWidth
      {
         get { return Graphics.PenWidth; }
         set { Graphics.PenWidth = value; }
      }

      public static double FontSize {
         get { return Graphics.FontSize; }
         set { Graphics.FontSize = value; }
      }

      public static string FontName 
      {
         get { return Graphics.FontName; }
         set { Graphics.FontName = value; }
      }

      public static void Clear()
      {
         Graphics.Clear();
      }
      
      public static void DrawText(object x, object y, object text)
      {
         Graphics.DrawText((int)x, (int)y, (string)text);
      }
      
      public static void DrawLine(object x1, object y1, object x2, object y2)
      {
         Graphics.DrawLine((int)x1,(int)y1,(int)x2,(int)y2);
      }

      public static void DrawTriangle(object x1, object y1, object x2, object y2, object x3, object y3)
      {
         Graphics.DrawTriangle((int)x1, (int)y1, (int)x2, (int)y2, (int)x3, (int)y3);
      }

      public static void DrawRectangle(object x,object y, object width, object height)
      {
         Graphics.DrawRectangle((int)x, (int)y, (int)width, (int)height);
      }

      public static void FillEllipse(object x1, object y1, object width, object height)
      {
         Graphics.FillEllipse((int)x1,(int)y1,(int)width,(int)height);
      }
      
      public static void DrawImage(object imageName, object x, object y)
      {
         Graphics.DrawImage((string)imageName, (int)x, (int)y);
      }

      public static void SetPixel(object x, object y, object color)
      {
         Graphics.DrawLine((int)x, (int)y, ((int)x) + 2, ((int)y));
      }

      public static string LastKey
      {
         get { return _graphics.LastKey; }
      }

      public static event System.EventHandler KeyDown
      {
         add { _graphics.KeyDown += value; }
         remove { _graphics.KeyDown -= value; }
      }

      public static int MouseX
      {
         get { return _graphics.MouseX; }
      }

      public static int MouseY
      {
         get { return _graphics.MouseY; }
      }

      public static event System.EventHandler MouseDown
      {
         add { _graphics.MouseDown += value; }
         remove { _graphics.MouseDown -= value; }
      }

      public static event System.EventHandler MouseUp
      {
         add { _graphics.MouseUp += value; }
         remove { _graphics.MouseUp -= value; }
      }

      public static event System.EventHandler MouseMove
      {
         add { _graphics.MouseMove += value; }
         remove { _graphics.MouseMove -= value; }
      }

   }
}
