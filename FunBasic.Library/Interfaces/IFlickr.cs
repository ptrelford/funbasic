namespace FunBasic.Library
{
   public interface IFlickr
   {
      string GetInterestingPhoto();
      string GetTaggedPhoto(string tags);
   }
}
