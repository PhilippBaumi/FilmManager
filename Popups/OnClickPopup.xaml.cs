using CommunityToolkit.Maui.Views;
using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Popups;

public partial class OnClickPopup : Popup
{
    private object? o;
    private IDatabase database;
    private INavigationService navigationService;
    private OnClickPopupViewModel onClickPopupViewModel;
    private string apiKey;
    public OnClickPopup(object? o, bool isInWatched, IDatabase database, INavigationService navigationService, string apiKey)
    {
        InitializeComponent();
        this.o = o;
        lbTitle.Text = SetText();
        this.database = database;
        this.apiKey = apiKey;
        this.onClickPopupViewModel = new();
        this.navigationService = navigationService;
        if (isInWatched)
        {
            btnMarkedAsWatched.IsVisible = false;
        }
        BindingContext = onClickPopupViewModel;
    }

    private string SetText()
    {
        if (this.o != null)
        {
            if (this.o is SearchMovie movie)
            {
                return movie.OriginalTitle;
            }
            if (this.o is SearchTv tv)
            {
                return tv.OriginalName;
            }
        }
        return "";
    }

    private async void MarkAsWatched(object sender, EventArgs e)
    {
        if (o is SearchMovie movie)
        {
            DateTime? rDate = movie.ReleaseDate;
            if (rDate.HasValue)
            {
                if (rDate.Value > DateTime.Today)
                {
                    await AlertHelper.InfoAlert(AppResources.dateIsFuture);
                }
                else
                {
                    this.database.DeleteEntry(movie, "Watchlist");
                    this.database.InsertEntry(movie, "Watched");
                    await AlertHelper.InfoAlert(AppResources.markedAsWatched);
                }
            }
        }
        if (o is SearchTv tv)
        {
            DateTime? rDate = tv.FirstAirDate;
            if (rDate.HasValue)
            {
                if (rDate.Value > DateTime.Today)
                {
                    await AlertHelper.InfoAlert(AppResources.dateIsFuture);
                }
                else
                {
                    this.database.DeleteEntry(tv, "Watchlist");
                    this.database.InsertEntry(tv, "Watched");
                    await AlertHelper.InfoAlert(AppResources.markedAsWatched);
                }
            }
        }
    }

    private async void HandleRemove(object sender, EventArgs e)
    {
        ObservableCollection<object> watched = this.database.SelectAllEntries("Watched");
        ObservableCollection<object> watchlist = this.database.SelectAllEntries("Watchlist");
        if (o is SearchMovie movie)
        {
            if (!this.onClickPopupViewModel.MyContains(watched, movie) && !this.onClickPopupViewModel.MyContains(watchlist, movie))
            {
                await AlertHelper.InfoAlert(AppResources.notRemoved);
                return;
            }
            if (this.onClickPopupViewModel.MyContains(watched, movie))
            {
                this.database.DeleteEntry(movie, "Watched");
                await AlertHelper.InfoAlert(AppResources.removed);
                return;
            }
            if (this.onClickPopupViewModel.MyContains(watchlist, movie))
            {
                this.database.DeleteEntry(movie, "Watchlist");
                await AlertHelper.InfoAlert(AppResources.removed);
                return;
            }
        }
        else if (o is SearchTv tv)
        {
            if (!this.onClickPopupViewModel.MyContains(watched, tv) && !this.onClickPopupViewModel.MyContains(watchlist, tv))
            {
                await AlertHelper.InfoAlert(AppResources.notRemoved);
                return;
            }
            if (this.onClickPopupViewModel.MyContains(watched, tv))
            {
                this.database.DeleteEntry(tv, "Watched");
                await AlertHelper.InfoAlert(AppResources.removed);
                return;
            }
            if (this.onClickPopupViewModel.MyContains(watchlist, tv))
            {
                this.database.DeleteEntry(tv, "Watchlist");
                await AlertHelper.InfoAlert(AppResources.removed);
                return;
            }
        }
    }

    private async void HandleClose(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void NavigateToDetail(object sender, EventArgs e)
    {
        TMDBService tMDBService = new(new TMDbClient(apiKey));
        IDictionary<string, object> dict;
        if (this.o is SearchMovie movie)
        {
            Movie m = await tMDBService.GetMovieAsync(movie.Id);
            dict = new Dictionary<string, object>
            {
                {"content", m },
                {"apiKey", apiKey }
            };
            await navigationService.NavigateToAsync("//Detail", dict);
        }
        if (this.o is SearchTv tv)
        {
            TvShow show = await tMDBService.GetTvShowAsync(tv.Id);
            dict = new Dictionary<string, object>
            {
                {"content", show },
                {"apiKey", apiKey  }
            };
            await navigationService.NavigateToAsync("//Detail", dict);
        }
    }
}