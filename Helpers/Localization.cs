using FilmManager.Resources.Strings.Sprachen;
using System.Globalization;

namespace FilmManager.Helpers
{
    public class Localization : ILocalization
    {
        public void SetLanguage(string cultureCode)
        {
            CultureInfo culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            AppResources.Culture = culture;
        }
    }
}
