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

      public static string AddLine(object x1, object y1, object x2, object y2)
      {
         return GraphicsWindow.Graphics.AddLine((int)x1,(int)y1,(int)x2,(int)y2);
      }

      public static string AddImage(object url)
      {
         return GraphicsWindow.Graphics.AddImage((string)url);
      }

      public static void Remove(object name)
      {
         GraphicsWindow.Graphics.Remove((string)name);
      }

      public static void SetOpacity(object name, object value)
      {
         GraphicsWindow.Graphics.SetOpacity((string)name, (int)value);
      }

      public static void Move(object name, object x, object y)
      {
         GraphicsWindow.Graphics.Move((string)name, (int)x, (int)y);
      }

      public static void Rotate(object name, object angle)
      {
         throw new NotImplementedException();
      }
   }
}
