namespace FunBasic.Library
{
   public static class Sound
   {
      public static ISounds Sounds;

      public static void PlayBellRing()
      {
         Sounds.PlayStockSound("BellRing", false);
      }

      public static void PlayBellRingAndWait()
      {
         Sounds.PlayStockSound("BellRing", true);
      }

      public static void PlayChime()
      {
         Sounds.PlayStockSound("Chime", false);
      }

      public static void PlayChimeAndWait()
      {
         Sounds.PlayStockSound("Chime", true);
      }

      public static void PlayChimes()
      {
         Sounds.PlayStockSound("Chimes", false);
      }

      public static void PlayChimesAndWait()
      {
         Sounds.PlayStockSound("Chimes", true);
      }

      public static void PlayClick()
      {
         Sounds.PlayStockSound("Click", false);
      }

      public static void PlayClickAndWait()
      {
         Sounds.PlayStockSound("Click", true);
      }
   }
}
