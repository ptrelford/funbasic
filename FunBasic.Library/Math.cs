namespace FunBasic.Library
{
   public static class Math
   {
      private static System.Random _random;

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

   }
}
