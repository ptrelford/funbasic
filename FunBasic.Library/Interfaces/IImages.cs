namespace FunBasic.Library
{
   public interface IImages
   {
      #region ImageList
      string LoadImage(string url);
      int GetImageWidth(string name);
      int GetImageHeight(string name);
      #endregion
   }
}
