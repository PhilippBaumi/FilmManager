using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;

namespace FilmManager.Models
{
    public partial class SettingViewModel : ObservableObject
    {

        public ObservableCollection<string> Languages { get; set; } = new();
        public ObservableCollection<string> Themes { get; set; } = new();
        [ObservableProperty]
        private string? language;

        [ObservableProperty]
        private string? themeAsString;

        public SettingViewModel()
        {
            this.Languages.Add(AppResources.german);
            this.Languages.Add(AppResources.english);

            this.Themes.Add("System");
            this.Themes.Add(AppResources.dark);
            this.Themes.Add(AppResources.light);
        }

        partial void OnThemeAsStringChanged(string? value)
        {
            if (value is not null)
            {
                switch (value)
                {
                    case "Light":
                    case "Hell":
                        Application.Current.UserAppTheme = AppTheme.Light;
                        break;
                    case "Dark":
                    case "Dunkel":
                        Application.Current.UserAppTheme = AppTheme.Dark;
                        break;
                    case "System":
                        Application.Current.UserAppTheme = AppTheme.Unspecified;
                        break;
                }
            }
        }
    }
}
