namespace FunBasic.Library
{
   public static class Math
   {
      private static System.Random _random;

      public static int GetRandomNumber(object maxNumber)
      {
         if (_random == null)
         {
            _random = new System.Random((int)System.DateTime.Now.Ticks);
         }
         return Math._random.Next((int)maxNumber) + 1;
      }

      public static int Remainder(object dividend, object divisor)
      {
         return (int)dividend % (int)divisor;
      }

      public static double Power(object baseNumber, object exponent)
      {
         return System.Math.Pow((double)baseNumber, (double)exponent);
      }

      public static double SquareRoot(object number)
      {
         return System.Math.Sqrt((double)number);
      }

      public static double Floor(object number)
      {
         return System.Math.Floor((double)number);
      }

      public static double Ceiling(object number)
      {
         return System.Math.Ceiling((double)number);
      }

   }
}
