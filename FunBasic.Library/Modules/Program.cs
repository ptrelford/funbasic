namespace FunBasic.Library
{
   public static class Program
   {
      public static void Delay(int time)
      {
         System.Threading.Tasks.Task.Delay(time).Wait();
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
