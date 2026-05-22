using CommunityToolkit.Maui.Views;
using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Popups;

public partial class OptionsPopup : Popup
{

    private string? selectedItem;
    private INavigationService navigation;
    private OptionPopupViewModel optionPopupViewModel = new();
    private IDatabase database;
    private TMDBService tMDBService;
    private object? obj;
    private string apiKey;

    public OptionsPopup(string? selectedItem, INavigationService navigation, object? o, IDatabase database, string option, string apiKey)
    {
        InitializeComponent();
        this.selectedItem = selectedItem;
        this.database = database;
        this.navigation = navigation;
        this.apiKey = apiKey;
        this.tMDBService = new(new TMDbClient(apiKey));
        this.obj = optionPopupViewModel.Get(selectedItem, o);
        SetTitleToLabel();
        if(option.Equals("Detail"))
        {
            btnDetails.IsVisible = false;
        }
    }

    private void SetTitleToLabel()
    {
        if (this.obj != null)
        {
            if (this.obj is SearchMovie movie)
            {
                lbTitle.Text = movie.OriginalTitle;
            }
            if (this.obj is SearchTv tv)
            {
                lbTitle.Text = tv.OriginalName;
            }
        }
    }

    private async void AddToWatched(object sender, EventArgs e)
    {
        try
        {
            if (this.selectedItem != null)
            {
                if (this.obj == null)
                {
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.noChoosenMovieOrSerie, "OK");
                }
                this.database.CreateTable("Watched");
                if (this.obj is SearchTv serie)
                {
                    DateTime? rDate = serie.FirstAirDate;
                    if(rDate.HasValue)
                    {
                        if(rDate.Value>DateTime.Today)
                        {
                            await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.dateIsFuture, "OK");
                        }
                        else
                        {
                            this.database.InsertEntry(serie, "Watched");
                            await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", $"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}", "OK");
                        }
                    }
                }
                if (this.obj is SearchMovie movie)
                {
                    DateTime? rDate = movie.ReleaseDate;
                    if(rDate.HasValue)
                    {
                        if(rDate.Value>DateTime.Today)
                        {
                            await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", AppResources.dateIsFuture, "OK");
                        }
                        else
                        {
                            this.database.InsertEntry(movie, "Watched");
                            await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", $"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}", "OK");
                        }
                    }
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
            if (this.selectedItem != null)
            {
                if (this.obj == null)
                {
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.noChoosenMovieOrSerie, "OK");
                }
                this.database.CreateTable("Watchlist");
                if (this.obj is SearchTv serie)
                {
                    this.database.InsertEntry(serie, "Watchlist");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", $"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}", "OK");                }
                if (this.obj is SearchMovie movie)
                {
                    this.database.InsertEntry(movie, "Watchlist");
                    await Application.Current.Windows[0].Page.DisplayAlertAsync("Info", $"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}", "OK");
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
        try
        { 
            if (this.selectedItem != null)
            {
                if (this.obj != null)
                {
                    if (this.obj is SearchTv serie)
                    {
                        TvShow tvShow = await this.tMDBService.GetTvShowAsync(serie.Id);
                        IDictionary<string, object> dict = new Dictionary<string, object>
                        {
                            { "content", tvShow },
                            { "apiKey", apiKey }
                        };
                        await navigation.NavigateToAsync("//Detail", dict);
                    }
                    if (this.obj is SearchMovie movie)
                    {
                        Movie tMovie = await this.tMDBService.GetMovieAsync(movie.Id);
                        IDictionary<string, object> dict = new Dictionary<string, object>
                        {
                            { "content", tMovie },
                            { "apiKey", apiKey }
                        };
                        await navigation.NavigateToAsync("//Detail", dict);
                    }
                }
                if (this.obj == null)
                {
                    await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, AppResources.cantNavigateToDetails, "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Windows[0].Page.DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void HandleClose(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}