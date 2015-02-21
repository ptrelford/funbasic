namespace FunBasic.Library
{
   using System;

   public static class Controls
   {
      private static IControls _controls;

      internal static void Init(IControls controls)
      {
         _controls = controls;
      }

      public static string AddButton(string caption, int x, int y)
      {
         return _controls.AddButton(caption, x, y);
      }

      public static string AddTextBox(int x, int y)
      {
         return _controls.AddTextBox(x, y);
      }

      public static string AddMultiLineTextBox(int x, int y)
      {
         return _controls.AddMultiLineTextBox(x,y);
      }

      public static string GetTextBoxText(string name)
      {
         return _controls.GetTextBoxText(name);
      }

      public static void SetTextBoxText(string name, string text)
      {
         _controls.SetTextBoxText(name, text);
      }

      public static void SetSize(string name, int width, int height)
      {
         _controls.SetSize(name, width, height);
      }

      public static string LastClickedButton
      {
         get { return _controls.LastClickedButton; }
      }

      public static string GetButtonCaption(string name)
      {
         return _controls.GetButtonCaption(name);
      }

      public static event EventHandler ButtonClicked
      {
         add { _controls.ButtonClicked += value; }
         remove { _controls.ButtonClicked -= value; }
      }
   }
}
