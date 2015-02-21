namespace FunBasic.Library
{
   using System.Threading;

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
   }
}
