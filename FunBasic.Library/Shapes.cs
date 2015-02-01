using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunBasic.Library
{
   public static class Shapes
   {
      public static string AddText(string text)
      {
         return GraphicsWindow.Graphics.AddText(text);
      }

      public static string AddEllipse(int width, int height)
      {
         return GraphicsWindow.Graphics.AddEllipse(width, height);
      }

      public static string AddLine(int x1, int y1, int x2, int y2)
      {
         return GraphicsWindow.Graphics.AddLine(x1, y1, x2, y2);
      }

      public static string AddTriangle(int x1, int y1, int x2, int y2, int x3, int y3)
      {
         return GraphicsWindow.Graphics.AddTriangle(x1, x2, y1, y2, x3, y3);
      }

      public static string AddRectangle(int width, int height)
      {         
         return GraphicsWindow.Graphics.AddRectangle(width, height);
      }

      public static string AddImage(string url)
      {
         return GraphicsWindow.Graphics.AddImage(url);
      }

      public static void HideShape(string name)
      {
         GraphicsWindow.Graphics.HideShape(name);
      }

      public static void ShowShape(string name)
      {
         GraphicsWindow.Graphics.ShowShape(name);
      }

      public static void Remove(string name)
      {
         GraphicsWindow.Graphics.Remove(name);
      }

      public static void SetOpacity(string name, int value)
      {
         GraphicsWindow.Graphics.SetOpacity(name, value);
      }

      public static void SetText(string name, string text)
      {
         GraphicsWindow.Graphics.SetText(name, text);
      }

      public static int GetLeft(string name)
      {
         return GraphicsWindow.Graphics.GetLeft(name);
      }

      public static int GetTop(string name)
      {
         return GraphicsWindow.Graphics.GetTop(name);
      }

      public static void Move(string name, int x, int y)
      {
         GraphicsWindow.Graphics.Move(name, x, y);
      }

      public static void Animate(string name, int x, int y, int duration)
      {
         GraphicsWindow.Graphics.Animate(name, x, y, duration);
      }

      public static void Rotate(string name, int angle)
      {
         GraphicsWindow.Graphics.Rotate(name, angle);
      }

      public static void Zoom(string name, double scaleX, double scaleY)
      {
         GraphicsWindow.Graphics.Zoom(name, scaleX, scaleY);
      }
   }
}
