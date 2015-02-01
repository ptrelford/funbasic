namespace FunBasic.Library
{
   public interface IGraphics
   {
      #region Properties
      double Width { get; }
      double Height { get; }
      string BackgroundColor { get; set; }
      double PenWidth { get; set; }
      string PenColor { get; set; }
      string BrushColor { get; set; }
      double FontSize { get; set; }
      string FontName { get; set; }
      #endregion

      #region Render
      void Clear();
      void DrawEllipse(int x, int y, int width, int height);   
      void DrawLine(int x1, int y1, int x2, int y2);
      void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3);
      void DrawRectangle(int x, int y, int width, int height);
      void DrawText(int x, int y, string text);      
      void DrawImage(string url, int x, int y);
      void FillEllipse(int x, int y, int width, int height);
      void FillTriangle(int x1, int y1, int x2, int y2, int x3, int y3);
      void FillRectangle(int x, int y, int width, int height);
      #endregion

      #region Shapes
      string AddText(string text);
      string AddEllipse(double width, double height);
      string AddLine(double x1, double y1, double x2, double y2);
      string AddTriangle(double x1, double y1, double x2, double y2, double x3, double y3);
      string AddRectangle(double width, double height);
      string AddImage(string url);
      void HideShape(string name);
      void ShowShape(string name);
      void Remove(string name);
      double GetLeft(string name);
      double GetTop(string name);
      void Move(string name, double x, double y);
      void Animate(string name, double x, double y, int duration);
      void Rotate(string name, int angle);
      void Zoom(string name, double scaleX, double scaleY);
      void SetOpacity(string name, int opacity);
      void SetText(string name, string text);      
      #endregion

      #region Mouse
      int MouseX { get; }
      int MouseY { get; }
      event System.EventHandler MouseDown;
      event System.EventHandler MouseUp;
      event System.EventHandler MouseMove;
      #endregion

      #region Keyboard
      string LastKey { get; }
      event System.EventHandler KeyDown;
      #endregion
   }
}
