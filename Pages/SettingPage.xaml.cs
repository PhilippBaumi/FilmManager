using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;

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
                case "Deutsch": case "German": localization.SetLanguage("de"); break;
                case "Englisch": case "English": localization.SetLanguage("en"); break;
                default: localization.SetLanguage("en"); break;
            }
        }
        await Toast.ShowAsync(AppResources.languageChanged, DialogType.Info);
        Application.Current.MainPage = new AppShell(navigationService);
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, AppResources.settings);
    }
}