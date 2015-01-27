namespace FunBasic.Library
{
   public static class Math
   {
      private static System.Random _random;

      public static int Remainder(object dividend, object divisor)
      {
         return (int)dividend % (int)divisor;
      }

      public static int GetRandomNumber(object maxNumber)
      {
         if (_random == null)
         {
            _random = new System.Random((int)System.DateTime.Now.Ticks);
         }
         return Math._random.Next((int)maxNumber) + 1;
      }
   }
}
