using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager.Helpers
{
    public static class AlertHelper
    {
        public static Task ErrorAlert(string message)
        {
            return Shell.Current.DisplayAlertAsync(AppResources.error, message, "OK");
        }

        public static Task InfoAlert(string message)
        {
            return Shell.Current.DisplayAlertAsync("Info", message, "OK");
        }

        public static Task BaseAlert(string title, string message)
        {
            return Shell.Current.DisplayAlertAsync(title, message, "OK");
        }
    }
}
