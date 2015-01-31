namespace FunBasic.Library
{
   using System;

   public static class Timer
   {
      private static ITimer _timer;

      public static void SetTimer(ITimer timer)
      {
         _timer = timer;
      }

      public static int Interval
      {
         get { return _timer.Interval; }
         set { _timer.Interval = value; }
      }

      public static event EventHandler Tick
      {
         add { _timer.Tick += value; }
         remove { _timer.Tick -= value; }         
      }

      public static void Pause()
      {
         _timer.Pause();
      }

      public static void Resume()
      {
         _timer.Resume();
      }
   }
}
