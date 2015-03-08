namespace FunBasic.Library
{
   public interface IKeyboard
   {
      #region Keyboard
      string LastKey { get; }
      event System.EventHandler KeyDown;
      event System.EventHandler KeyUp;
      #endregion
   }
}
