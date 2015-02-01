namespace FunBasic.Library
{
   public static class Program
   {
      public static void Delay(int time)
      {
         System.Threading.Tasks.Task.Delay(time).Wait();
      }
   }
}
