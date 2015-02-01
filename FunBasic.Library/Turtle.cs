namespace FunBasic.Library
{
   using System;

   public static class Turtle
   {
      private static IGraphics _graphics;
      private static bool _isPenDown;

      public static IGraphics Graphics
      {
         get { return _graphics; }
         set {
            _graphics = value;
            X = _graphics.Width/2.0;
            Y = _graphics.Height/2.0;
            Angle = 0;
            _isPenDown = true;
         }
      }

      public static int Angle { get; set; }
      public static double X { get; set; }
      public static double Y { get; set; }
      public static int Speed { get; set; }

      public static void Move(int distance)
      {
         int n = distance;
         var radians = (Angle-90) * System.Math.PI / 180;
         var x2 = X + n * System.Math.Cos(radians);
         var y2 = Y + n * System.Math.Sin(radians);
         if (_isPenDown)
         {
            Graphics.DrawLine((int)X, (int)Y, (int)x2, (int)y2);
         }
         X = x2;
         Y = y2;
         Graphics.Move("Turtle", ((int)X)-7, ((int)Y)-7);
      }

      public static void MoveTo(int x, int y)
      {
         X = x;
         Y = y;
         Graphics.Move("Turtle", ((int)X) - 7, ((int)Y) - 7);
      }

      public static void Turn(int angle)
      {
         Angle += angle%360;
         Graphics.Rotate("Turtle", Angle);
      }

      public static void TurnLeft()
      {
         Turn(-90);
      }

      public static void TurnRight()
      {
         Turn(90);
      }

      public static void PenUp()
      {
         _isPenDown = false;
      }

      public static void PenDown()
      {
         _isPenDown = true;
      }

      public static void Show()
      {
         GraphicsWindow.Graphics.ShowShape("Turtle");
      }

      public static void Hide()
      {
         GraphicsWindow.Graphics.HideShape("Turtle");
      }
   }
}
