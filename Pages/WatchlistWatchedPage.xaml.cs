using CommunityToolkit.Maui.Extensions;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;

namespace FilmManager;

public partial class WatchlistWatchedPage : ContentPage
{
    private INavigationService navigationService;
    private WatchlistWatchedViewModel watchlistWatchedViewModel;
    private ObservableCollection<object> watched = new();
    private ObservableCollection<object> watchlist = new();
    private IDatabase database;

    public WatchlistWatchedPage(INavigationService navigation, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigation;
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
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
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
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
        this.watchlistWatchedViewModel = new(watchlist, watched);
        BindingContext = this.watchlistWatchedViewModel;
    }

    private async void HandlePickerSelection(object sender, EventArgs e)
    {
        string? selectedItem = watchlistWatchedViewModel.SelectedItem;
        try
        {
            if (selectedItem != null)
            {
                string[] st = selectedItem.Split("(");
                object? obj = this.watchlistWatchedViewModel.Get(st[0].Trim());
                bool inWatchedList = this.watchlistWatchedViewModel.IsInWatchedList(obj);
                OnClickPopup popup = new(obj, inWatchedList, database, navigationService);
                Application.Current.Windows[0].Page.ShowPopup(popup);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }
}