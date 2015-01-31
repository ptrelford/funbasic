namespace FunBasic.Library
{
   public interface ITimer
   {
      int Interval { get; set; }
      event System.EventHandler Tick;
   }
}
