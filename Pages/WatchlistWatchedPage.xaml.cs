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
using System.Collections.ObjectModel;

namespace FilmManager;

public partial class WatchlistWatchedPage : ContentPage
{
    private INavigationService navigationService;
    private WatchlistWatchedViewModel watchlistWatchedViewModel;
    private ObservableCollection<object> watched = new();
    private ObservableCollection<object> watchlist = new();
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
        LoadWatched();
        LoadWatchlist();
    }

    private async void LoadWatchlist()
    {
        try
        {
            this.watchlist = this.database.SelectAllEntries("Watchlist");
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
        this.watchlistWatchedViewModel = new(watchlist, watched);
        BindingContext = this.watchlistWatchedViewModel;
    }

    private async void LoadWatched()
    {
        try
        {
            this.watched = this.database.SelectAllEntries("Watched");
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
        this.watchlistWatchedViewModel = new(watchlist, watched);
        if (this.watchlistWatchedViewModel is not null)
        {
            BindingContext = this.watchlistWatchedViewModel;
        }
        else
        {
            BindingContext = new WatchlistWatchedViewModel(null, null);
        }
    }

    private async void HandlePickerSelection(object sender, EventArgs e)
    {
        string? selectedItem = watchlistWatchedViewModel.SelectedItem;
        try
        {
            if (!string.IsNullOrWhiteSpace(selectedItem))
            {
                object? obj = this.watchlistWatchedViewModel.Get(selectedItem);
                bool inWatchedList = this.watchlistWatchedViewModel.IsInWatchedList(obj);
                OnClickMenu onClickMenu = new(obj, inWatchedList, database, navigationService, this.tMDBService.client.ApiKey);
                await onClickMenu.ShowAsync();
                ((Picker)sender).SelectedItem = null;
                LoadWatched();
                LoadWatchlist();
            }
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
}