using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace FunBasic.Library
{
   public static class Web
   {
      public static string Get(string url)
      {
         return GetAsync(url).Result;
      }

      private static async Task<string> GetAsync(string url)
      {
         var client = new HttpClient();
         var response = await client.GetAsync(new Uri(url));
         var result = await response.Content.ReadAsStringAsync();
         return result;
      }

      public static string Post(string url, string content)
      {
         return PostAsync(url, content).Result;
      }

      private static async Task<string> PostAsync(string url, string content)
      {
         var client = new HttpClient();
         var response = await client.PostAsync(new Uri(url), new HttpStringContent(content));
         var result = await response.Content.ReadAsStringAsync();
         return result;
      }

   }
}
