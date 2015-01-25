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

   }
}
