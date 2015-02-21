namespace FunBasic.Library
{
   public interface IKeyboard
   {
      #region Keyboard
      string LastKey { get; }
      event System.EventHandler KeyDown;
      #endregion
   }
}
