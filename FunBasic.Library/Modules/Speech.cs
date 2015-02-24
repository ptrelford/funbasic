namespace FunBasic.Library
{
   public static class Speech
   {
      private static ISpeech _speech;

      internal static void Init(ISpeech speech)
      {
         _speech = speech;
      }

      public static void Say(string text)
      {
         _speech.Say(text);
      }
   }
}
