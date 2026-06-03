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

    private async void HandlePickerLanguage(object sender, EventArgs e)
    {
        Localization localization = new();
        string? language = settingViewModel.Language;

        if (language is not null)
        {
            switch (language)
            {
                case "Deutsch": localization.SetLanguage("de"); break;
                case "German": localization.SetLanguage("de"); break;
                case "Englisch": localization.SetLanguage("en"); break;
                case "English": localization.SetLanguage("en"); break;
            }
        }
        await AlertHelper.InfoAlert(AppResources.languageChangedAndNavigateToHome);
        Application.Current.MainPage = new AppShell(navigationService);
    }
}