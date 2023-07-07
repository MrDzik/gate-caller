using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GateCaller.Helpers
{
    internal static class LangHelper
    {
        public static async Task<string?> GetLang()
        {
            return await SecureStorage.Default.GetAsync("Lang");
        }

        public static async Task ChangeLang(string lang)
        {
            SecureStorage.Default.Remove("Lang");
            await SecureStorage.Default.SetAsync("Lang", lang);
        }

        public static void ChangeCulture(string lang)
        {
            if (lang == "pl")
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("pl-PL");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("pl-PL");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
        }
    }
}
