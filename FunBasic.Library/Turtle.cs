namespace FunBasic.Library
{
   using System;

   public static class Turtle
   {
      private static IDrawings _graphics;
      private static IShapes _shapes;
      private static bool _isPenDown;
      private static bool _isVisible = false;

      internal static void Init(ISurface surface, IDrawings drawings, IShapes shapes)
      {
         _graphics = drawings;
         _shapes = shapes;
         X = surface.Width / 2.0;
         Y = surface.Height / 2.0;
         Angle = 0;
         _isPenDown = true;
         Hide();
      }

      public static double Angle { get; set; }
      public static double X { get; set; }
      public static double Y { get; set; }
      public static int Speed { get; set; }

      public static void Move(double distance)
      {
         Show();
         var radians = (Angle-90.0) * System.Math.PI / 180;
         var x1 = X;
         var y1 = Y;
         var x2 = x1 + distance * System.Math.Cos(radians);
         var y2 = y1 + distance * System.Math.Sin(radians);
         X = x2;
         Y = y2;
         if (_isPenDown)
         {
            _graphics.DrawLine(x1, y1, x2, y2);
         }
         _shapes.Move("Turtle", x2 - 8.0, y2 - 8.0);
      }

      public static void MoveTo(double x, double y)
      {
         Show();
         X = x;
         Y = y;
         _shapes.Move("Turtle", x - 8.0, y - 8.0);
      }

      public static void Turn(double angle)
      {
         Show();
         Angle += angle%360.0;
         _shapes.Rotate("Turtle", Angle);
      }

      public static void TurnLeft()
      {
         Show();
         Turn(-90);
      }

      public static void TurnRight()
      {
         Show();
         Turn(90);
      }

      public static void PenUp()
      {
         Show();
         _isPenDown = false;
      }

      public static void PenDown()
      {
         Show();
         _isPenDown = true;
      }

      public static void Show()
      {
         if (!_isVisible)
         {
            _isVisible = true;
            _shapes.ShowShape("Turtle");
         }
      }

      public static void Hide()
      {
         _isVisible = false;
         _shapes.HideShape("Turtle");
      }
   }
}
