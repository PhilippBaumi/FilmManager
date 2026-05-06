using CommunityToolkit.Maui.Extensions;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Views;
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

    private void LoadWatchlist(object sender, EventArgs e)
    {
        this.watchlist = this.database.SelectAllEntries("Watchlist");
        this.watchlistWatchedViewModel = new(watchlist, watched);
        BindingContext = this.watchlistWatchedViewModel;
    }

    private void LoadWatched(object sender, EventArgs e)
    {
        this.watched = this.database.SelectAllEntries("Watched");
        this.watchlistWatchedViewModel = new(watchlist, watched);
        BindingContext = this.watchlistWatchedViewModel;
    }
}