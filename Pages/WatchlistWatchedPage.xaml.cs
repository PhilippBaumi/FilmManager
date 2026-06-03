using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
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
            if (selectedItem is not null)
            {
                string[] st = selectedItem.Split("(");
                object? obj = this.watchlistWatchedViewModel.Get(st[0].Trim());
                bool inWatchedList = this.watchlistWatchedViewModel.IsInWatchedList(obj);
                OnClickMenu onClickMenu = new(obj, inWatchedList, database, navigationService, this.tMDBService.client.ApiKey);
                await onClickMenu.ShowAsync();
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }
}