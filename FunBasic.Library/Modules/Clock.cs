namespace FunBasic.Library
{
   using System;
   using System.Globalization;

   public static class Clock
   {

      public static string Date
      {
         get
         {
            var format = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentCulture);
            return DateTime.Now.ToString(format.ShortDatePattern, CultureInfo.CurrentUICulture);
         }
      }

      public static string Time
      {
         get
         {
            var format = DateTimeFormatInfo.GetInstance(CultureInfo.CurrentCulture);
            return DateTime.Now.ToString(format.LongTimePattern, CultureInfo.CurrentUICulture);
         }
      }

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
            return DateTime.Now.Minute;
         }
      }

      public static int Second
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
