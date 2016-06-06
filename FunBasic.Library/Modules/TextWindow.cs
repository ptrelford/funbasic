namespace FunBasic.Library
{
   public static class TextWindow
   {    
      private static IConsole _console;

      internal static void Init(IConsole console)
      {
         _console = console;
      }

      public static int Top { get; set; }
      public static int Left { get; set; }

      public static void WriteLine(string text)
      {
         _console.WriteLine(text);
      }
   }
}
