namespace FunBasic.Library
{
   using System.Collections.Generic;

   public static class Stack
   {
      private static Dictionary<string, Stack<object>> _stackMap;

      internal static void Init()
      {
         _stackMap = new Dictionary<string, Stack<object>>();
      }

      public static object GetCount(string stackName)
      {
         Stack<object> values;
         if (!_stackMap.TryGetValue(stackName, out values))
         {
            values = new Stack<object>();
            _stackMap[stackName] = values;
         }
         return values.Count;
      }

      public static object PopValue(string stackName)
      {
         Stack<object> values;
         if (_stackMap.TryGetValue(stackName, out values))
         {
            return values.Pop();
         }
         return "";
      }

      public static void PushValue(string stackName, object value)
      {
         Stack<object> values;
         if (!_stackMap.TryGetValue(stackName, out values))
         {
            values = new Stack<object>();
            _stackMap[stackName] = values;
         }
         values.Push(value);
      }
   }
}