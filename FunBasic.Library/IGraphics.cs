namespace FunBasic.Library
{
   public interface IGraphics
   {
      double Width { get; }
      double Height { get; }
      string BackgroundColor { get; set; }

      void DrawLine(double penWidth, string penColor, int x1, int y1, int x2, int y2);
      void FillEllipse(string brushColor, int x1, int y1, int x2, int y2);
   }
}
