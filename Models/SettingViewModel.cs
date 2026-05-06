using CommunityToolkit.Mvvm.ComponentModel;

namespace FilmManager.Models
{
    public partial class SettingViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? language;

        [ObservableProperty]
        private string? themeAsString;

        partial void OnThemeAsStringChanged(string? value)
        {
            if (themeAsString != null)
            {
                switch (themeAsString)
                {
                    case "Light":
                        Application.Current.UserAppTheme = AppTheme.Light;
                        break;
                    case "Dark":
                        Application.Current.UserAppTheme = AppTheme.Dark;
                        break;
                    default:
                        Application.Current.UserAppTheme = AppTheme.Unspecified;
                        break;
                }
            }
        }
    }
}
