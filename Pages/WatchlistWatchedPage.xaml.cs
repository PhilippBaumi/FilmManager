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

    private void OnSelectionItem(object sender, SelectionChangedEventArgs e)
    {
        string? selectedItem = watchlistWatchedViewModel.SelectedItem;
        if (selectedItem != null)
        {
            OnClickPopup popup = new(selectedItem, navigationService, database);
            Application.Current.MainPage.ShowPopup(popup);
        }
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void LoadWatchlist(object sender, EventArgs e)
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

    private async void LoadWatched(object sender, EventArgs e)
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
}