using FilmManager.Backend;
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

public partial class WatchlistWatchedPage : ContentPage
{
    private INavigationService navigationService;
    private WatchlistWatchedViewModel watchlistWatchedViewModel;
    private List<object> watched = new();
    private List<object> watchlist = new();
    private IDatabase database;
    private TMDBService tMDBService;

    public WatchlistWatchedPage(INavigationService navigation, TMDBService tmdbService, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigation;
        this.tMDBService = tmdbService;
        this.database = database;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ReloadLists();
    }

    private async void ReloadLists()
    {
        try
        {
            this.watchlist = this.database.SelectAllEntries("Watchlist");
            this.watched = this.database.SelectAllEntries("Watched");
            this.watchlistWatchedViewModel = new(this.watchlist, this.watched);
            BindingContext = this.watchlistWatchedViewModel;
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, $"  {AppResources.watchedWatchlist}");
    }

    private async void HandleWatchedPickerSelection(object sender, EventArgs e)
    {
        if(sender is Picker picker)
        {
            await HandleSelectedObjectAsync(picker, this.watchlistWatchedViewModel.GetWatchedByIndex(picker.SelectedIndex));
        }
    }

    private async void HandleWatchlistPickerSelection(object sender, EventArgs e)
    {
        if (sender is Picker picker)
        {
            await HandleSelectedObjectAsync(picker, this.watchlistWatchedViewModel.GetWatchlistByIndex(picker.SelectedIndex));
        }
    }

    private async Task HandleSelectedObjectAsync(Picker picker, object? obj)
    {
        try
        {
            if(picker.SelectedIndex<0) 
            { 
                await Toast.ShowAsync(AppResources.indexLessThanZero, DialogType.Error);
                return;
            }
            if(obj is null)
            {
                await Toast.ShowAsync(AppResources.objectIsEmpty, DialogType.Error);
                return;
            }
            bool isInWatched = this.watchlistWatchedViewModel.IsInWatchedList(obj);
            OnClickMenu onClickMenu = new(obj, isInWatched, database, navigationService, tMDBService.client.ApiKey);
            await onClickMenu.ShowAsync();
            picker.SelectedItem = null;
            ReloadLists();
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }
}