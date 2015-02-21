namespace FunBasic.Library
{
   public interface IDrawings
   {
      void DrawEllipse(double x, double y, double width, double height);
      void DrawLine(double x1, double y1, double x2, double y2);
      void DrawTriangle(double x1, double y1, double x2, double y2, double x3, double y3);
      void DrawRectangle(double x, double y, double width, double height);
      void DrawText(double x, double y, string text);
      void DrawBoundText(double x, double y, double width, string text);
      void DrawImage(string url, double x, double y);
      void FillEllipse(double x, double y, double width, double height);
      void FillTriangle(double x1, double y1, double x2, double y2, double x3, double y3);
      void FillRectangle(double x, double y, double width, double height);
   }
}