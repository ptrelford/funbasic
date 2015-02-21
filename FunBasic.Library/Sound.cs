namespace FunBasic.Library
{
   public static class Sound
   {
      private static ISounds _sounds;

      internal static void Init(ISounds sounds)
      {
         _sounds = sounds;
      }

      public static void PlayBellRing()
      {
         _sounds.PlayStockSound("BellRing", false);
      }

      public static void PlayBellRingAndWait()
      {
         _sounds.PlayStockSound("BellRing", true);
      }

      public static void PlayChime()
      {
         _sounds.PlayStockSound("Chime", false);
      }

      public static void PlayChimeAndWait()
      {
         _sounds.PlayStockSound("Chime", true);
      }

      public static void PlayChimes()
      {
         _sounds.PlayStockSound("Chimes", false);
      }

      public static void PlayChimesAndWait()
      {
         _sounds.PlayStockSound("Chimes", true);
      }

      public static void PlayClick()
      {
         _sounds.PlayStockSound("Click", false);
      }

      public static void PlayClickAndWait()
      {
         _sounds.PlayStockSound("Click", true);
      }
   }
}
