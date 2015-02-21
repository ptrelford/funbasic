namespace FunBasic.Library
{
   public interface ISurface
   {
      double Width { get; set; }
      double Height { get; set; }
      string BackgroundColor { get; set; }
      void Clear();
      void ShowMessage(string content, string title);
   }
}
