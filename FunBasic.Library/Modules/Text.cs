
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
            var len = Math.Max(0, Math.Min(length, text.Length - startIndex + 1));
            return text.Substring(startIndex - 1, (int)len);
         }
         else
            return "";
      }

      public static string GetSubTextToEnd(string text, int index)
      {
         if (index > 0)
         {
            var startIndex = (int)Math.Min(text.Length + 1, index);            
            return text.Substring(startIndex - 1);
         }
         else
            return "";
      }

      public static int GetIndexOf(string text, string value)
      {
         return text.IndexOf(value) + 1;
      }

      public static int GetCharacterCode(string character)
      {
         return character.Length > 0
            ? (int)character[0]
            : 0;
      }

      public static string GetCharacter(int code)
      {
         return ((char)code).ToString();
      }

      public static string ConvertToLowerCase(string text)
      {
         return text.ToLowerInvariant();
      }

      public static string ConvertToUpperCase(string text)
      {
         return text.ToUpperInvariant();
      }
   }
}
