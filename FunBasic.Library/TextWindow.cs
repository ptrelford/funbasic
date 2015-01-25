namespace FunBasic.Library
{
   public static class TextWindow
   {    
      public static IConsole Console;

      public static void WriteLine(object text)
      {
         Console.WriteLine(text);
      }
   }
}
