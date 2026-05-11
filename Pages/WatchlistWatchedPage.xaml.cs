using CommunityToolkit.Maui.Extensions;
using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager;

public partial class WatchlistWatchedPage : ContentPage
{
    private INavigationService navigationService;
    private WatchlistWatchedViewModel watchlistWatchedViewModel;
    private ObservableCollection<object> watched = new();
    private ObservableCollection<object> watchlist = new();
    private IDatabase database;
    private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";

    public WatchlistWatchedPage(INavigationService navigation, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigation;
        this.database = database;
    }

    private async void OnSelectionItem(object sender, SelectionChangedEventArgs e)
    {
        string? selectedItem = watchlistWatchedViewModel.SelectedItem;
        if(selectedItem!=null)
        {
            TMDBService tMDBService = new TMDBService(new TMDbClient(apiKey));
            string[] st = selectedItem.Split(" ");
            object? obj=this.watchlistWatchedViewModel.Get(st[0]);
            bool navi = await DisplayAlertAsync(AppResources.info, AppResources.askForNavigation, AppResources.yes, AppResources.no);
            if(navi&&obj!=null)
            {
                IDictionary<string, object> dict;
                if(obj is SearchMovie movie)
                {
                    Movie m = await tMDBService.GetMovieAsync(movie.Id);
                    dict = new Dictionary<string, object>
                    {
                        {"content", m }
                    };
                    await navigationService.NavigateToAsync("//Detail", dict);
                }
                if(obj is SearchTv tv)
                {
                    TvShow show = await tMDBService.GetTvShowAsync(tv.Id);
                    dict = new Dictionary<string, object>
                    {
                        {"content", show }
                    };
                    await navigationService.NavigateToAsync("//Detail", dict);
                }
            }
        }
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