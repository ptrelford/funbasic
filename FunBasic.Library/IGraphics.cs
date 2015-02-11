﻿namespace FunBasic.Library
{
   public interface IGraphics
   {
      #region Properties
      double Width { get; set; }
      double Height { get; }
      string BackgroundColor { get; set; }
      double PenWidth { get; set; }
      string PenColor { get; set; }
      string BrushColor { get; set; }
      double FontSize { get; set; }
      string FontName { get; set; }
      bool FontItalic { get; set; }
      bool FontBold { get; set; }
      #endregion

      #region Render
      void Clear();
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
      void ShowMessage(string content, string title);
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
      void Rotate(string name, double angle);
      void Zoom(string name, double scaleX, double scaleY);
      int GetOpacity(string name);
      void SetOpacity(string name, int opacity);
      void SetText(string name, string text);      
      #endregion

      #region ImageList
      string LoadImage(string url);
      int GetImageWidth(string name);
      int GetImageHeight(string name);
      #endregion

      #region Mouse
      double MouseX { get; }
      double MouseY { get; }
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
