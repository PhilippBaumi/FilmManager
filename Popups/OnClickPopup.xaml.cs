using CommunityToolkit.Maui.Views;
using FilmManager.Backend;
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
        this.onClickPopupViewModel = new(database);
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
        return null;
    }

    private async void MarkAsWatched(object sender, EventArgs e)
    {
        if(o is SearchMovie movie)
        {
            DateTime? rDate = movie.ReleaseDate;
            if (rDate.HasValue)
            {
                if (rDate.Value > DateTime.Today)
                {
                    await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.dateIsFuture, "OK");
                }
                else
                {
                    this.database.DeleteEntry(movie, "Watchlist");
                    this.database.InsertEntry(movie, "Watched");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.markedAsWatched, "OK");
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
                    await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.dateIsFuture, "OK");
                }
                else
                {
                    this.database.DeleteEntry(tv, "Watchlist");
                    this.database.InsertEntry(tv, "Watched");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.markedAsWatched, "OK");
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
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.notRemoved, "OK");
            }
            if (this.onClickPopupViewModel.MyContains(watched, movie))
            {
                this.database.DeleteEntry(movie, "Watched");
                await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.removed, "OK");
            }
            if (this.onClickPopupViewModel.MyContains(watchlist, movie))
            {
                this.database.DeleteEntry(movie, "Watchlist");
                await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.removed, "OK");
            }
        }
        if (o is SearchTv tv)
        {
            if(!this.onClickPopupViewModel.MyContains(watched, tv)&&!this.onClickPopupViewModel.MyContains(watchlist, tv))
            {
                await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.notRemoved, "OK");
            }
            if (this.onClickPopupViewModel.MyContains(watched, tv))
            {
                this.database.DeleteEntry(tv, "Watched");
                await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.removed, "OK");
            }
            if (this.onClickPopupViewModel.MyContains(watchlist, tv))
            {
                this.database.DeleteEntry(tv, "Watchlist");
                await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.removed, "OK");
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
                {"content", m }
            };
            await navigationService.NavigateToAsync("//Detail", dict);
        }
        if (this.o is SearchTv tv)
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