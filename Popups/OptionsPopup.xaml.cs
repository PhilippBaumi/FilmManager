using CommunityToolkit.Maui.Views;
using FilmManager.Backend;
using FilmManager.Helpers;
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
        if (option.Equals("Detail"))
        {
            btnDetails.IsVisible = false;
        }
    }

    private void SetTitleToLabel()
    {
        if (this.obj is not null)
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
            if (this.selectedItem is not null)
            {
                if (this.obj == null)
                {
                    await AlertHelper.InfoAlert(AppResources.noChoosenMovieOrSerie);
                }
                this.database.CreateTable("Watched");
                if (this.obj is SearchTv serie)
                {
                    DateTime? rDate = serie.FirstAirDate;
                    if (rDate.HasValue)
                    {
                        if (rDate.Value > DateTime.Today)
                        {
                            await AlertHelper.InfoAlert(AppResources.dateIsFuture);
                        }
                        else
                        {
                            this.database.InsertEntry(serie, "Watched");
                            await AlertHelper.InfoAlert($"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}");
                        }
                    }
                }
                if (this.obj is SearchMovie movie)
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
                            this.database.InsertEntry(movie, "Watched");
                            await AlertHelper.InfoAlert($"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
        }
    }

    private async void AddToWatchlist(object sender, EventArgs e)
    {
        try
        {
            if (this.selectedItem is not null)
            {
                if (this.obj is null)
                {
                    await AlertHelper.InfoAlert(AppResources.noChoosenMovieOrSerie);
                }
                this.database.CreateTable("Watchlist");
                if (this.obj is SearchTv serie)
                {
                    this.database.InsertEntry(serie, "Watchlist");
                    await AlertHelper.InfoAlert($"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}");
                }
                if (this.obj is SearchMovie movie)
                {
                    this.database.InsertEntry(movie, "Watchlist");
                    await AlertHelper.InfoAlert($"{AppResources.successfullyCreatedTable} {AppResources.and} {AppResources.insertSuccess}");
                }
            }
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
        }
    }

    private async void NavigateToDetails(object sender, EventArgs e)
    {
        try
        {
            if (this.selectedItem is not null)
            {
                if (this.obj is not null)
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
                if (this.obj is null)
                {
                    await AlertHelper.ErrorAlert(AppResources.cantNavigateToDetails);
                }
            }
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
        }
    }

    private async void HandleClose(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}