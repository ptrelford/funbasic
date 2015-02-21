namespace FunBasic.Library
{
   using System.Threading;

   public static class Program
   {
      private static CancellationToken _token;

      internal static void Init(CancellationToken token)
      {
         _token = token;
      }

      [System.Diagnostics.DebuggerStepThrough]
      public static void Delay(int time)
      {
         try
         {
            System.Threading.Tasks.Task.Delay(time).Wait(_token);
         }
         catch (System.OperationCanceledException) { }
      }

      public static string Directory
      {
         get { return ""; }
      }

      public static void End()
      {
      }
   }
}
