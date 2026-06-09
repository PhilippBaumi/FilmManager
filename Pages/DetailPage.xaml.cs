using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace FilmManager;

public partial class DetailPage : ContentPage, IQueryAttributable
{
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private INavigationService navigationService;
    private DetailViewModel detailViewModel;
    private object o;
    private string apiKey;
    private IDatabase database;

    public DetailPage(INavigationService navigationSerive, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigationSerive;
        this.database = database;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("content"))
        {
            o = query["content"];
            detailViewModel = new(o);
            BindingContext = detailViewModel;
        }
        if (query.ContainsKey("apiKey"))
        {
            apiKey = query["apiKey"] as string;
        }
        query.Clear();
    }

    private async void HandleLinkClicked(object sender, TappedEventArgs e)
    {
        try
        {
            Uri uri = new(detailViewModel.Homepage);
            await Launcher.Default.TryOpenAsync(uri);
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandleLogos(object sender, EventArgs e)
    {
        try
        {
            string? selectedLogo = detailViewModel.SelectedLogo;
            if (selectedLogo is not null)
            {
                selectedLogo = selectedLogo.Substring(0, selectedLogo.Length - 4);
                Uri uri = new(selectedLogo);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandlePosters(object sender, EventArgs e)
    {
        try
        {
            string? selectedPoster = detailViewModel.SelectedPoster;
            if (selectedPoster is not null)
            {
                selectedPoster = selectedPoster.Substring(0, selectedPoster.Length - 4);
                Uri uri = new(selectedPoster);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandleBackports(object sender, EventArgs e)
    {
        try
        {
            string? selectedBackport = detailViewModel.SelectedBackport;
            if (selectedBackport is not null)
            {
                selectedBackport = selectedBackport.Substring(0, selectedBackport.Length - 4);
                Uri uri = new(selectedBackport);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandleRecommendation(object sender, EventArgs e)
    {
        string? selectedRecommentation = detailViewModel.SelectedRecommendation;
        if (!string.IsNullOrEmpty(selectedRecommentation))
        {
            List<object> list = detailViewModel.GetList(selectedRecommentation);
            IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "objectlist", list },
                { "apiKey", apiKey }
            };
            await navigationService.NavigateToAsync("//Overview", parameters);
        }
    }

    private async void HandlePopupShow(object sender, EventArgs e)
    {
        string? selectedPoster = detailViewModel.Poster;
        if (!string.IsNullOrEmpty(selectedPoster))
        {
            selectedPoster = selectedPoster.Replace(ImageBaseUrl, string.Empty);
            OptionsMenu optionsMenu = new(selectedPoster, navigationService, o, database, apiKey);
            await optionsMenu.ShowAsync("Detail");
        }
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, "Details");
    }
}