namespace FunBasic.Library
{
   public static class Math
   {
      private static System.Random _random;

      public static double Pi
      {
         get { return 3.14159265358979; }
      }

      public static int GetRandomNumber(int maxNumber)
      {
         if (_random == null)
         {
            _random = new System.Random((int)System.DateTime.Now.Ticks);
         }
         return Math._random.Next(maxNumber) + 1;
      }

      public static int Remainder(int dividend, int divisor)
      {
         return dividend % divisor;
      }

      public static double Abs(double number)
      {
         return System.Math.Abs(number);
      }

      public static double Power(double baseNumber, double exponent)
      {
         return System.Math.Pow(baseNumber, exponent);
      }

      public static double SquareRoot(double number)
      {
         return System.Math.Sqrt(number);
      }

      public static double Floor(double number)
      {
         return System.Math.Floor(number);
      }

      public static double Ceiling(double number)
      {
         return System.Math.Ceiling(number);
      }

      public static double Round(double number)
      {
         return System.Math.Round(number);
      }

      public static double Max(double number1, double number2)
      {
         return System.Math.Max(number1, number2);
      }

      public static double Min(double number1, double number2)
      {
         return System.Math.Min(number1, number2);
      }

      public static double ArcTan(double number)
      {
         return System.Math.Atan(number);
      }

      public static double Cos(double number)
      {
         return System.Math.Cos(number);
      }

      public static double Sin(double number)
      {
         return System.Math.Sin(number);
      }

   }
}
