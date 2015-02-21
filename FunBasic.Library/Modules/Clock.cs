namespace FunBasic.Library
{
   using System;

   public static class Clock
   {
      public static int Year
      {
         get
         {
            return DateTime.Now.Year;
         }
      }

      public static int Month
      {
         get
         {
            return DateTime.Now.Month;
         }
      }

      public static int Day
      {
         get
         {
            return DateTime.Now.Day;
         }
      }

      public static int Hour
      {
         get
         {
            return DateTime.Now.Hour;
         }
      }

      public static int Minute
      {
         get
         {
            return DateTime.Now.Second;
         }
      }

      public static int Millisecond
      {
         get
         {
            return DateTime.Now.Millisecond;
         }
      }

      public static double ElapsedMilliseconds
      {
         get
         {
            TimeSpan now = DateTime.Now - new DateTime(1900, 1, 1);
            return (double)now.TotalMilliseconds;
         }
      }

   }
}
