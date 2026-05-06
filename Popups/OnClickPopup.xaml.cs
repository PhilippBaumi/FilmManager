using CommunityToolkit.Maui.Views;
using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Views;

public partial class OnClickPopup : Popup
{
    private string? selectedItem;
    private INavigationService navigation;
    private OnClickPopupViewModel onClickPopupViewModel;
    private TMDBService tMDBService = new(new TMDbClient("c7108e21486edb11a641d92aa539f3e2"));
    private IDatabase database;
    public OnClickPopup(string? selectedItem, INavigationService navigation, IDatabase database)
    {
        InitializeComponent();
        this.database = database;
        this.selectedItem = selectedItem;
        this.navigation = navigation;
        this.onClickPopupViewModel = new(database);
        BindingContext = onClickPopupViewModel;
    }

    private async void RemoveFromList(object sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            object? obj = onClickPopupViewModel.Get(selectedItem);
            if (obj != null)
            {
                this.database.DeleteEntry(obj, "Watched");
                this.database.DeleteEntry(obj, "Watchlsit");
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.removed, $"{onClickPopupViewModel.GetName(obj)} {AppResources.removedMessage}", "OK");
            }
        }
    }

    private async void HandleClose(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void NavigateToDetails(object sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            object? obj = onClickPopupViewModel.Get(selectedItem);
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

    private async void NavigateToCollection(object sender, EventArgs e)
    {
        if (selectedItem != null)
        {
            object? obj = onClickPopupViewModel.Get(selectedItem);
            if (obj != null)
            {
                if (obj is SearchTv serie)
                {
                    TvShow tvShow = await tMDBService.GetTvShowAsync(serie.Id);
                    await navigation.NavigateToAsync("//Collection", new Dictionary<string, object> { { "content", tvShow } });
                }
                if (obj is SearchMovie movie)
                {
                    Movie tMovie = await tMDBService.GetMovieAsync(movie.Id);
                    await navigation.NavigateToAsync("//Collection", new Dictionary<string, object> { { "content", tMovie } });
                }
            }
            if (obj == null)
            {
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.cantNavigateToCollection, "OK");
            }
        }
    }
}
