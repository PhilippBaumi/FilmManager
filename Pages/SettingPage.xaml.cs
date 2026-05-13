using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager;

public partial class SettingPage : ContentPage
{
    private INavigationService navigationService;
    private SettingViewModel settingViewModel = new();
    public SettingPage(INavigationService navigationService)
    {
        InitializeComponent();
        this.navigationService = navigationService;
        BindingContext = settingViewModel;
    }

    private async void HandleChecked(object sender, CheckedChangedEventArgs e)
    {
        Localization localization = new();
        string? language = settingViewModel.Language;
        if (language != null)
        {
            switch (language)
            {
                case "Deutsch": localization.SetLanguage("de"); break;
                case "German": localization.SetLanguage("de"); break;
                case "Englisch": localization.SetLanguage("en"); break;
                case "English": localization.SetLanguage("en"); break;
            }
        }
        await DisplayAlertAsync("Info", AppResources.languageChangedAndNavigateToHome, "OK");
        Application.Current.MainPage = new AppShell(navigationService);
    }
}