namespace FunBasic.Library
{
   public static class GraphicsWindow
   {
      private static ISurface _surface;
      private static IDrawings _drawings;
      private static IKeyboard _keyboard;
      private static IMouse _mouse;
      private static System.Random _random = new System.Random();
      private static IStyle _style { get;set; }

      internal static void Init(
         IStyle style, 
         ISurface surface, 
         IDrawings graphics, 
         IKeyboard keyboard,      
         IMouse mouse)
      {
         _surface = surface;
         _drawings = graphics;
         _keyboard = keyboard;
         _style = style;
         _mouse = mouse;
         PenWidth = 2.0;
         BrushColor = "purple";
         PenColor = "black";
         FontSize = 12;
         FontName = "Tahoma";
      }

      public static string Title { get; set; }

      public static double Width
      {
         get { return (int) _surface.Width; }
         set { _surface.Width = value; }
      }

      public static double Height
      {
         get { return ((int ) _surface.Height / 2) * 2; }
         set { _surface.Height = value; }
      }

      public static double Top
      {
         get { return 0; }
         set { }
      }

      public static double Left
      {
         get { return 0; }
         set { }
      }

      public static bool CanResize
      {
         get { return true; }
         set { }
      }

      public static string BackgroundColor 
      {
         get { return _surface.BackgroundColor; }
         set { _surface.BackgroundColor = value; } 
      }
      
      public static string BrushColor 
      {
         get { return _style.BrushColor; }
         set { _style.BrushColor = value; }
      }

      public static string PenColor 
      {
         get { return _style.PenColor; }
         set { _style.PenColor = value; }
      }

      public static double PenWidth
      {
         get { return _style.PenWidth; }
         set { _style.PenWidth = value; }
      }

      public static double FontSize {
         get { return _style.FontSize; }
         set { _style.FontSize = value; }
      }

      public static string FontName 
      {
         get { return _style.FontName; }
         set { _style.FontName = value; }
      }

      public static bool FontItalic
      {
         get { return _style.FontItalic; }
         set { _style.FontItalic = value;  }
      }

      public static bool FontBold
      {
         get { return _style.FontBold; }
         set { _style.FontBold = value; }
      }

      public static string GetColorFromRGB(int r, int g, int b)
      {
         return string.Format("#{0:X2}{1:X2}{2:X2}",r,g,b);
      }

      public static string GetRandomColor()
      {
         return 
            string.Format("#{0:X2}{1:X2}{2:X2}", 
               _random.Next(256), 
               _random.Next(256), 
               _random.Next(256));
      }

      public static void Show()
      {
      }

      public static void Hide()
      {
      }

      public static void Clear()
      {
         _surface.Clear();
      }

      public static void ShowMessage(string content, string title)
      {
         _surface.ShowMessage(content, title);
      }

      #region Drawing
      public static void DrawText(double x, double y, string text)
      {
         _drawings.DrawText(x, y, text);
      }

      public static void DrawBoundText(double x, double y, double width, string text)
      {
         _drawings.DrawBoundText(x, y, width, text);
      }

      public static void DrawEllipse(double x, double y, double width, double height)
      {
         _drawings.DrawEllipse(x, y, width, height);
      }

      public static void DrawLine(double x1, double y1, double x2, double y2)
      {
         _drawings.DrawLine(x1, y1, x2, y2);
      }

      public static void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         _drawings.DrawTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void DrawRectangle(double x, double y, double width, double height)
      {
         _drawings.DrawRectangle(x, y, width, height);
      }

      public static void DrawImage(string imageName, double x, double y)
      {
         _drawings.DrawImage(imageName, x, y);
      }

      public static void DrawResizedImage(string imageName, double x, double y, double width, double height)
      {
         _drawings.DrawResizedImage(imageName, x, y, width, height);
      }

      public static string GetPixel(int x, int y)
      {
         return _drawings.GetPixel(x, y);
      }

      public static void SetPixel(int x, int y, string color)
      {
         _drawings.SetPixel(x, y, color);
      }
      #endregion

      #region Fill
      public static void FillEllipse(double x, double y, double width, double height)
      {
         _drawings.FillEllipse(x, y, width, height);
      }

      public static void FillTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         _drawings.FillTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static void FillRectangle(double x1, double y1, double width, double height)
      {
         _drawings.FillRectangle(x1, y1, width, height);
      }
      #endregion

      #region Keyboard
      public static string LastKey
      {
         get { return _keyboard.LastKey; }
      }

      public static event System.EventHandler KeyDown
      {
         add { _keyboard.KeyDown += value; }
         remove { _keyboard.KeyDown -= value; }
      }

      public static event System.EventHandler KeyUp
      {
         add { _keyboard.KeyUp += value; }
         remove { _keyboard.KeyUp -= value; }
      }
      #endregion

      #region Mouse
      public static double MouseX
      {
         get { return _mouse.MouseX; }
      }

      public static double MouseY
      {
         get { return _mouse.MouseY; }
      }

      public static event System.EventHandler MouseDown
      {
         add { _mouse.MouseDown += value; }
         remove { _mouse.MouseDown -= value; }
      }

      public static event System.EventHandler MouseUp
      {
         add { _mouse.MouseUp += value; }
         remove { _mouse.MouseUp -= value; }
      }

      public static event System.EventHandler MouseMove
      {
         add { _mouse.MouseMove += value; }
         remove { _mouse.MouseMove -= value; }
      }
      #endregion
   }
}
