namespace FunBasic.Library
{
   public interface IShapes
   {
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
      void Rotate(string name, double angle);
      void Zoom(string name, double scaleX, double scaleY);
      int GetOpacity(string name);
      void SetOpacity(string name, int opacity);
      void SetText(string name, string text);
   }
}
