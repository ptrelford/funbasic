
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
         if (index > 0)
         {
            var startIndex = (int)Math.Min(text.Length + 1, index);
            var len = Math.Min(length, text.Length - startIndex + 1);
            return text.Substring(startIndex - 1, (int)len);
         }
         else
            return "";
      }
   }
}
