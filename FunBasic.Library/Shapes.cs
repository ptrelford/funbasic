using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunBasic.Library
{
   public static class Shapes
   {
      private static IShapes _shapes { get; set; }

      internal static void Init(IShapes shapes)
      {
         _shapes = shapes;
      }

      public static string AddText(string text)
      {
         return _shapes.AddText(text);
      }

      public static string AddEllipse(double width, double height)
      {
         return _shapes.AddEllipse(width, height);
      }

      public static string AddLine(double x1, double y1, double x2, double y2)
      {
         return _shapes.AddLine(x1, y1, x2, y2);
      }

      public static string AddTriangle(double x1, double y1, double x2, double y2, double x3, double y3)
      {
         return _shapes.AddTriangle(x1, y1, x2, y2, x3, y3);
      }

      public static string AddRectangle(double width, double height)
      {
         return _shapes.AddRectangle(width, height);
      }

      public static string AddImage(string url)
      {
         return _shapes.AddImage(url);
      }

      public static void HideShape(string name)
      {
         _shapes.HideShape(name);
      }

      public static void ShowShape(string name)
      {
         _shapes.ShowShape(name);
      }

      public static void Remove(string name)
      {
         _shapes.Remove(name);
      }

      public static int GetOpacity(string name)
      {
         return _shapes.GetOpacity(name);
      }

      public static void SetOpacity(string name, int value)
      {
         _shapes.SetOpacity(name, value);
      }

      public static void SetText(string name, string text)
      {
         _shapes.SetText(name, text);
      }

      public static double GetLeft(string name)
      {
         return _shapes.GetLeft(name);
      }

      public static double GetTop(string name)
      {
         return _shapes.GetTop(name);
      }

      public static void Move(string name, double x, double y)
      {
         _shapes.Move(name, x, y);
      }

      public static void Animate(string name, double x, double y, int duration)
      {
         _shapes.Animate(name, x, y, duration);
      }

      public static void Rotate(string name, int angle)
      {
         _shapes.Rotate(name, angle);
      }

      public static void Zoom(string name, double scaleX, double scaleY)
      {
         _shapes.Zoom(name, scaleX, scaleY);
      }
   }
}
