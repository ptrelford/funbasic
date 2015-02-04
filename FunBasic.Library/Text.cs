
namespace FunBasic.Library
{
   public static class Text
   {
      public static string Append(string a, string b)
      {
         return a + b;
      }

      public static int GetLength(string text)
      {
         return text.Length;
      }

      public static string GetSubText(string text, int index, int length)
      {
         return text.Substring(index, length);
      }
   }
}
