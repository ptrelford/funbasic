using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunBasic.Library
{
   public static class Shapes
   {
      public static string AddText(object text)
      {
         return GraphicsWindow.Graphics.AddText((string)text);
      }

      public static string AddEllipse(object width, object height)
      {
         return GraphicsWindow.Graphics.AddEllipse((int)width, (int)height);
      }

      public static string AddLine(object x1, object y1, object x2, object y2)
      {
         return GraphicsWindow.Graphics.AddLine((int)x1,(int)y1,(int)x2,(int)y2);
      }

      public static string AddTriangle(object x1, object y1, object x2, object y2, object x3, object y3)
      {
         return GraphicsWindow.Graphics.AddTriangle((int)x1, (int)x2, (int)y1, (int)y2, (int)x3, (int)y3);
      }

      public static string AddRectangle(object width, object height)
      {
         return GraphicsWindow.Graphics.AddRectangle((int)width, (int)height);
      }

      public static string AddImage(object url)
      {
         return GraphicsWindow.Graphics.AddImage((string)url);
      }

      public static void HideShape(object name)
      {
         GraphicsWindow.Graphics.HideShape((string)name);
      }

      public static void ShowShape(object name)
      {
         GraphicsWindow.Graphics.ShowShape((string)name);
      }

      public static void Remove(object name)
      {
         GraphicsWindow.Graphics.Remove((string)name);
      }

      public static void SetOpacity(object name, object value)
      {
         GraphicsWindow.Graphics.SetOpacity((string)name, (int)value);
      }

      public static void SetText(object name, object text)
      {
         GraphicsWindow.Graphics.SetText((string)name, text.ToString());
      }

      public static void Move(object name, object x, object y)
      {
         GraphicsWindow.Graphics.Move((string)name, (int)x, (int)y);
      }

      public static void Animate(object name, object x, object y, object duration)
      {
         GraphicsWindow.Graphics.Animate((string)name, (int)x, (int)y, (int)duration);
      }

      public static void Rotate(object name, object angle)
      {
         GraphicsWindow.Graphics.Rotate((string)name, (int)angle);
      }

      public static void Zoom(object name, object scaleX, object scaleY)
      {
         GraphicsWindow.Graphics.Zoom((string)name,(double)scaleX, (double)scaleY);
      }
   }
}
