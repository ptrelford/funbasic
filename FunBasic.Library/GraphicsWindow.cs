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
            BrushColor = "purple";
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

      public static void ShowMessage(string text, string title)
      {

      }

      #region Draw
      public static void DrawText(int x, int y, string text)
      {
         Graphics.DrawText(x, y, text);
      }
      
      public static void DrawLine(int x1, int y1, int x2, int y2)
      {
         Graphics.DrawLine(x1, y1, x2, y2);
      }

      public static void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
      {
         Graphics.DrawTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void DrawRectangle(int x, int y, int width, int height)
      {
         Graphics.DrawRectangle(x, y, width, height);
      }

      public static void DrawImage(string imageName, int x, int y)
      {
         Graphics.DrawImage(imageName, x, y);
      }

      public static void SetPixel(int x, int y, string color)
      {
         Graphics.DrawLine(x, y, x + 2, y);
      }
      #endregion

      #region Fill
      public static void FillEllipse(int x1, int y1, int width, int height)
      {
         Graphics.FillEllipse(x1, y1, width, height);
      }

      public static void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
      {
         Graphics.FillTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void FillRectangle(int x1, int y1, int width, int height)
      {
         Graphics.FillRectangle(x1, y1, width, height);
      }
      #endregion

      #region Keyboard
      public static string LastKey
      {
         get { return _graphics.LastKey; }
      }

      public static event System.EventHandler KeyDown
      {
         add { _graphics.KeyDown += value; }
         remove { _graphics.KeyDown -= value; }
      }
      #endregion

      #region Mouse
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
      #endregion
   }
}
