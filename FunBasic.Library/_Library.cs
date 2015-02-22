namespace FunBasic.Library
{
   using System.Collections.Generic;
   using System.Linq;
   using System.Threading;
   using System.Reflection;
   using System;

   public static class _Library
   {
      public static void Initialize(         
         IConsole console,
         ISurface surface,
         IStyle style,
         IDrawings drawing,
         IShapes shapes,
         IImages images,
         IControls controls,
         ISounds sounds,         
         IKeyboard keyboard,
         IMouse mouse,
         ITimer timer,
         CancellationToken token)
      {
         TextWindow.Init(console);
         Desktop.Init(surface);                  
         GraphicsWindow.Init(style, surface, drawing, keyboard, mouse);
         Shapes.Init(shapes);
         ImageList.Init(images);
         Turtle.Init(surface, drawing, shapes);
         Controls.Init(controls);
         Sound.Init(sounds);                      
         Timer.Init(timer);
         Stack.Init();
         Program.Init(token);
      }

      public static IDictionary<string, Tuple<string,string>[]> GetMemberLookup()
      {
         var ass = typeof(_Library).GetTypeInfo().Assembly;
         var types = ass.DefinedTypes;
         var lookup = new Dictionary<string, Tuple<string,string>[]>();
         foreach(var ti in types.Where(t => !t.IsInterface && !t.Name.StartsWith("_")))
         {
            var ty = ass.GetType("FunBasic.Library." + ti.Name);
            var ms =
               ty.GetRuntimeMethods()
                 .Where(m => m.IsStatic && m.IsPublic && !(m.Name.StartsWith("get_") || m.Name.StartsWith("set_") || m.Name.StartsWith("add_") || m.Name.StartsWith("remove_")))
                 .Select(m => Tuple.Create(m.Name, "Method "+m.Name + "(" + String.Join(", ",m.GetParameters().Select(pi => pi.Name)) +")"));
            
            var ps =
               ty.GetRuntimeProperties().Where(p => !p.Name.StartsWith("_")).Select(p => Tuple.Create(p.Name, "Property " + p.Name));
            var es =
               ty.GetRuntimeEvents().Select(e => Tuple.Create(e.Name, "Event "+e.Name));            

            lookup.Add(ty.Name, ms.Concat(ps).Concat(es).OrderBy(x=>x).ToArray());            
         }
         return lookup;
      }
   }
}
