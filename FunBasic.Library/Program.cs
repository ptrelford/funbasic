using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunBasic.Library
{
   public static class Program
   {
      public static void Delay(object time)
      {
         System.Threading.Tasks.Task.Delay((int) time).Wait();
      }
   }
}
