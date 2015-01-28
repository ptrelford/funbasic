namespace FunBasic.Library
{
   using System;

   public static class Turtle
   {
      private static IGraphics _graphics;

      public static IGraphics Graphics
      {
         get { return _graphics; }
         set {
            _graphics = value;
            X = _graphics.Width/2.0;
            Y = _graphics.Height/2.0;
            Angle = 270;
         }
      }

      public static int Angle { get; set; }
      public static double X { get; set; }
      public static double Y { get; set; }
      public static int Speed { get; set; }

      public static void Move(object distance)
      {
         int n = (int)distance;
         var radians = Angle * System.Math.PI / 180;
         var x2 = X + n * System.Math.Cos(radians);
         var y2 = Y + n * System.Math.Sin(radians);
         Graphics.DrawLine(
            GraphicsWindow.PenWidth,
            GraphicsWindow.PenColor,
            (int)X, (int)Y, (int) x2, (int) y2);
         X = x2;
         Y = y2;
      }

      public static void MoveTo(object x, object y)
      {
         Graphics.DrawLine(
            GraphicsWindow.PenWidth,
            GraphicsWindow.PenColor,
            (int)X, (int)Y, (int) x, (int) y);
         X = (int)x;
         Y = (int)y;
      }

      public static void Turn(object angle)
      {
         Angle += (int)angle;
      }

      public static void TurnLeft(object angle)
      {
         Angle -= (int)angle;
      }

      public static void TurnRight(object angle)
      {
         Angle += (int)angle;
      }
   }
}
