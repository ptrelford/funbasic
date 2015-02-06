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

      public static double Width
      {
         get { return _graphics.Width; }
         set { _graphics.Width = value; }
      }

      public static double Height
      {
         get { return _graphics.Height; }
         set { }
      }

      public static bool CanResize
      {
         get { return true; }
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

      public static bool FontItalic
      {
         get { return Graphics.FontItalic; }
         set { Graphics.FontItalic = value;  }
      }

      public static bool FontBold
      {
         get { return Graphics.FontBold; }
         set { Graphics.FontBold = value; }
      }

      public static string GetColorFromRGB(int r, int g, int b)
      {
         return string.Format("#{0:X2}{1:X2}{2:X2}",r,g,b);
      }

      public static void Show()
      {
      }

      public static void Hide()
      {
      }

      public static void Clear()
      {
         Graphics.Clear();
      }

      public static void ShowMessage(string content, string title)
      {
         Graphics.ShowMessage(content, title);
      }

      #region Draw
      public static void DrawText(double x, double y, string text)
      {
         Graphics.DrawText(x, y, text);
      }

      public static void DrawBoundText(double x, double y, double width, string text)
      {
         Graphics.DrawBoundText(x, y, width, text);
      }

      public static void DrawLine(double x1, double y1, double x2, double y2)
      {
         Graphics.DrawLine(x1, y1, x2, y2);
      }

      public static void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         Graphics.DrawTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void DrawRectangle(double x, double y, double width, double height)
      {
         Graphics.DrawRectangle(x, y, width, height);
      }

      public static void DrawImage(string imageName, double x, double y)
      {
         Graphics.DrawImage(imageName, x, y);
      }

      public static void SetPixel(double x, double y, string color)
      {
         Graphics.DrawLine(x, y, x + 2, y);
      }
      #endregion

      #region Fill
      public static void FillEllipse(double x1, double y1, double width, double height)
      {
         Graphics.FillEllipse(x1, y1, width, height);
      }

      public static void FillTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         Graphics.FillTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void FillRectangle(double x1, double y1, double width, double height)
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
      public static double MouseX
      {
         get { return _graphics.MouseX; }
      }

      public static double MouseY
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
