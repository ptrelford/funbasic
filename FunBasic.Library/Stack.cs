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

      public static object GetCount(string stackName)
      {
         Stack<object> primitives;
         if (!Stack._stackMap.TryGetValue(stackName, out primitives))
         {
            primitives = new Stack<object>();
            Stack._stackMap[stackName] = primitives;
         }
         return primitives.Count;
      }

      public static object PopValue(string stackName)
      {
         Stack<object> primitives;
         if (Stack._stackMap.TryGetValue(stackName, out primitives))
         {
            return primitives.Pop();
         }
         return "";
      }

      public static void PushValue(string stackName, object value)
      {
         Stack<object> primitives;
         if (!Stack._stackMap.TryGetValue(stackName, out primitives))
         {
            primitives = new Stack<object>();
            Stack._stackMap[stackName] = primitives;
         }
         primitives.Push(value);
      }
   }
}