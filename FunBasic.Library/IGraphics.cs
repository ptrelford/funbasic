namespace FunBasic.Library
{
   public interface IGraphics
   {
      double Width { get; }
      double Height { get; }
      string BackgroundColor { get; set; }
      double PenWidth { get; set; }
      string PenColor { get; set; }
      string BrushColor { get; set; }
      double FontSize { get; set; }
      string FontName { get; set; }

      void Clear();
      void DrawLine(int x1, int y1, int x2, int y2);
      void DrawText(int x, int y, string text);
      void DrawImage(string url, int x, int y);
      void FillEllipse(int x, int y, int width, int height);

      string AddText(string text);
      string AddLine(int x1, int y1, int x2, int y2);
      string AddImage(string url);

      void Remove(string name);
      void SetOpacity(string name, int opacity);
      void Move(string name, int x, int y);      
   }
}
