namespace FunBasic.Library
{
   public interface IGraphics
   {
      double Width { get; }
      double Height { get; }
      void DrawLine(double penWidth, string penColor, int x1, int y1, int x2, int y2);
   }
}
