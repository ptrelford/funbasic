namespace FunBasic.Library
{
   public interface IMouse
   {
      #region Mouse
      double MouseX { get; }
      double MouseY { get; }
      event System.EventHandler MouseDown;
      event System.EventHandler MouseUp;
      event System.EventHandler MouseMove;
      #endregion
   }
}
