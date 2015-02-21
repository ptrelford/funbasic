namespace FunBasic.Library
{
   public interface IStyle
   {
      double PenWidth { get; set; }
      string PenColor { get; set; }
      string BrushColor { get; set; }
      double FontSize { get; set; }
      string FontName { get; set; }
      bool FontItalic { get; set; }
      bool FontBold { get; set; }
   }
}
