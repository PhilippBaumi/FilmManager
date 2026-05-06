using CommunityToolkit.Maui.Views;
using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager;

public partial class OptionsPopup : Popup
{

    private string? selectedItem;
    private INavigationService navigation;
    private Object o;
    private OptionPopupViewModel optionPopupViewModel = new();
    private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
    private IDatabase database;

    public OptionsPopup(string? selectedItem, INavigationService navigation, object o, IDatabase database)
    {
        InitializeComponent();
        this.selectedItem = selectedItem;
        this.database = database;
        this.navigation = navigation;
        this.o = o;
    }

    private async void AddToWatched(object sender, EventArgs e)
    {
        try
        {
            if (selectedItem != null)
            {
                object? obj = optionPopupViewModel.Get(selectedItem, o);
                if (obj == null)
                {
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.noChoosenMovieAndSerie, "OK");
                }
                this.database.CreateTable("Watched");
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.info, AppResources.successfullyCreatedTable, "OK");
                if (obj is SearchTv serie)
                {
                    this.database.InsertEntry(serie, "Watched");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.info, AppResources.insertSuccess, "OK");
                }
                if (obj is SearchMovie movie)
                {
                    this.database.InsertEntry(movie, "Watched");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.info, AppResources.insertSuccess, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void AddToWatchlist(object sender, EventArgs e)
    {
        try
        {
            if (selectedItem != null)
            {
                object? obj = optionPopupViewModel.Get(selectedItem, o);
                if (obj == null)
                {
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.noChoosenMovieAndSerie, "OK");
                }
                this.database.CreateTable("Watchlist");
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.info, AppResources.successfullyCreatedTable, "OK");
                if (obj is SearchTv serie)
                {
                    this.database.InsertEntry(serie, "Watchlist");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.info, AppResources.insertSuccess, "OK");
                }
                if (obj is SearchMovie movie)
                {
                    this.database.InsertEntry(movie, "Watchlist");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.info, AppResources.insertSuccess, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void NavigateToDetails(object sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            object? obj = optionPopupViewModel.Get(selectedItem, o);
            TMDBService tMDBService = new(new TMDbClient(apiKey));
            if (obj != null)
            {
                if (obj is SearchTv serie)
                {
                    TvShow tvShow = await tMDBService.GetTvShowAsync(serie.Id);
                    await navigation.NavigateToAsync("//Detail", new Dictionary<string, object> { { "content", tvShow } });
                }
                if (obj is SearchMovie movie)
                {
                    Movie tMovie = await tMDBService.GetMovieAsync(movie.Id);
                    await navigation.NavigateToAsync("//Detail", new Dictionary<string, object> { { "content", tMovie } });
                }
            }
            if (obj == null)
            {
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.cantNavigateToDetails, "OK");
            }
        }
    }

    private async void HandleClose(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}