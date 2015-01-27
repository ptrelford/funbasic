namespace FunBasic.Library
{
   using System.Collections.Generic;

   public static class Stack
   {
      private static Dictionary<string, Stack<object>> _stackMap;

      static Stack()
      {
         Stack._stackMap = new Dictionary<string, Stack<object>>();
      }

      public static object GetCount(object stackName)
      {
         Stack<object> primitives;
         if (!Stack._stackMap.TryGetValue((string)stackName, out primitives))
         {
            primitives = new Stack<object>();
            Stack._stackMap[(string)stackName] = primitives;
         }
         return primitives.Count;
      }

      public static object PopValue(object stackName)
      {
         Stack<object> primitives;
         if (Stack._stackMap.TryGetValue((string)stackName, out primitives))
         {
            return primitives.Pop();
         }
         return "";
      }

      public static void PushValue(object stackName, object value)
      {
         Stack<object> primitives;
         if (!Stack._stackMap.TryGetValue((string)stackName, out primitives))
         {
            primitives = new Stack<object>();
            Stack._stackMap[(string)stackName] = primitives;
         }
         primitives.Push(value);
      }
   }
}