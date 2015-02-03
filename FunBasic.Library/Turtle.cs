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
         var x1 = X;
         var y1 = Y;
         var x2 = x1 + n * System.Math.Cos(radians);
         var y2 = y1 + n * System.Math.Sin(radians);
         X = x2;
         Y = y2;
         if (_isPenDown)
         {
            Graphics.DrawLine(x1, y1, x2, y2);
         }
         Graphics.Move("Turtle", x2 - 8.0, y2 - 8.0);
      }

      public static void MoveTo(int x, int y)
      {
         X = x;
         Y = y;
         Graphics.Move("Turtle", x - 8.0, y - 8.0);
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
