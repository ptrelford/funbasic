namespace FunBasic.Library
{
   public interface IControls
   {
      #region Controls
      string AddButton(string text, int x, int y);
      string AddTextBox(int x, int y);
      string AddMultiLineTextBox(int x, int y);
      string GetTextBoxText(string name);
      void SetTextBoxText(string name, string text);
      void SetSize(string name, int width, int height);
      string LastClickedButton { get; }
      string GetButtonCaption(string name);
      event System.EventHandler ButtonClicked;
      #endregion
   }
}
