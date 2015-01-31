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
      void DrawLine(int x1, int y1, int x2, int y2);
      void DrawText(int x, int y, string text);      
      void DrawImage(string url, int x, int y);
      void FillEllipse(int x, int y, int width, int height);   
      #endregion

      #region Shapes
      string AddText(string text);
      string AddLine(int x1, int y1, int x2, int y2);
      string AddRectangle(int width, int height);
      string AddImage(string url);
      void HideShape(string name);
      void ShowShape(string name);
      void Remove(string name);
      void Move(string name, int x, int y);
      void Animate(string name, int x, int y, int duration);
      void Rotate(string name, int angle);
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
