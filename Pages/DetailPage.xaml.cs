using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager;

public partial class DetailPage : ContentPage, IQueryAttributable
{
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private INavigationService navigationService;
    private DetailViewModel detailViewModel;
    private object o;
    private string apiKey;
    private IDatabase database;
    private TMDBService tMDBService;
    private TMDbHelper tMDbHelper;
    private GenreHelper genreHelper;

    public DetailPage(INavigationService navigationSerive, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigationSerive;
        this.database = database;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("content"))
        {
            o = query["content"];
            detailViewModel = new(o);
            BindingContext = detailViewModel;
        }
        if (query.ContainsKey("apiKey"))
        {
            apiKey = query["apiKey"] as string;
            this.tMDBService = new(new TMDbClient(apiKey));
            this.genreHelper = new(tMDBService);
            this.tMDbHelper = new(tMDBService, genreHelper);
        }
        query.Clear();
    }

    private async void HandleLinkClicked(object sender, TappedEventArgs e)
    {
        try
        {
            Uri uri = new(detailViewModel.Homepage);
            await Launcher.Default.TryOpenAsync(uri);
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandleLogos(object sender, EventArgs e)
    {
        try
        {
            string? selectedLogo = detailViewModel.SelectedLogo;
            if (selectedLogo is not null)
            {
                selectedLogo = selectedLogo.Substring(0, selectedLogo.Length - 4);
                Uri uri = new(selectedLogo);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandlePosters(object sender, EventArgs e)
    {
        try
        {
            string? selectedPoster = detailViewModel.SelectedPoster;
            if (selectedPoster is not null)
            {
                selectedPoster = selectedPoster.Substring(0, selectedPoster.Length - 4);
                Uri uri = new(selectedPoster);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandleBackports(object sender, EventArgs e)
    {
        try
        {
            string? selectedBackport = detailViewModel.SelectedBackport;
            if (selectedBackport is not null)
            {
                selectedBackport = selectedBackport.Substring(0, selectedBackport.Length - 4);
                Uri uri = new(selectedBackport);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void HandleRecommendation(object sender, EventArgs e)
    {
        string? selectedRecommentation = detailViewModel.SelectedRecommendation;
        if (!string.IsNullOrEmpty(selectedRecommentation))
        {
            List<object> list = detailViewModel.GetList(selectedRecommentation);
            IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "objectlist", list },
                { "apiKey", apiKey }
            };
            await navigationService.NavigateToAsync("//Overview", parameters);
        }
    }

    private async void HandlePopupShow(object sender, EventArgs e)
    {
        string? selectedPoster = detailViewModel.Poster;
        if (!string.IsNullOrEmpty(selectedPoster))
        {
            selectedPoster = selectedPoster.Replace(ImageBaseUrl, string.Empty);
            OptionsMenu optionsMenu = new(selectedPoster, navigationService, o, database, apiKey);
            await optionsMenu.ShowAsync("Detail");
        }
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, "   Details");
    }

    private async void HandleCast(object sender, EventArgs e)
    {
        string? selectedCast = this.detailViewModel.SelectedCast;
        if(selectedCast is not null)
        {
            SearchContainer<SearchPerson>cast=await tMDBService.SearchPersonAsync(selectedCast);
            if(cast.Results is not null)
            {
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "cast", cast.Results },
                    { "apiKey", this.apiKey }
                };
                await this.navigationService.NavigateToAsync("//Cast", parameters);
            }
        }
    }

    private async void HandleGenreChanged(object sender, EventArgs e)
    {
        string? selectedGenre = this.detailViewModel.SelectedGenre;
        if (selectedGenre is not null && o is Movie movie)
        {
            try
            {
                this.tMDBService.AddMoviesGenresToList();
                int id = this.tMDBService.GetIdToName(selectedGenre, MediaType.Movie);
                await LoadingDialog.ShowAsync(AppResources.loading, async () =>
                {
                    SearchContainer<SearchMovie> discoversMovies = await this.tMDBService.DiscoverMovies(id, 1);
                    if (discoversMovies.Results is not null)
                    {
                        this.genreHelper.GetMovies(discoversMovies.Results);
                    }
                    for (int page = discoversMovies.Page + 1; page <= discoversMovies.TotalPages; page++)
                    {
                        discoversMovies.Page = page;
                        discoversMovies = await this.tMDBService.DiscoverMovies(id, page);
                        if (discoversMovies.Results is not null)
                        {
                            this.genreHelper.GetMovies(discoversMovies.Results);
                        }
                    }
                });
            }
            catch
            {
                await Toast.ShowAsync(AppResources.tooMuchPages, DialogType.Error);
            }
            finally
            {
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "list", genreHelper.movies },
                    { "apiKey", apiKey }
                };
                await navigationService.NavigateToAsync("//Overview", parameters);
                ((Picker)sender).SelectedItem = null;
            }
        }
        if(selectedGenre is not null && o is TvShow show)
        {
            try
            {
                this.tMDBService.AddSerienGenresToList();
                int id = this.tMDBService.GetIdToName(selectedGenre, MediaType.Tv);
                await LoadingDialog.ShowAsync(AppResources.loading, async () =>
                {
                    SearchContainer<SearchTv> discoversSeries = await this.tMDBService.DiscoverSerien(id, 1);
                    if (discoversSeries.Results is not null)
                    {
                        this.genreHelper.GetSerien(discoversSeries.Results);
                    }
                    for (int page = discoversSeries.Page + 1; page <= discoversSeries.TotalPages; page++)
                    {
                        discoversSeries.Page = page;
                        discoversSeries = await this.tMDBService.DiscoverSerien(id, page);
                        if (discoversSeries.Results is not null)
                        {
                            this.genreHelper.GetSerien(discoversSeries.Results);
                        }
                    }
                });
            }
            catch
            {
                await Toast.ShowAsync(AppResources.tooMuchPages, DialogType.Error);
            }
            IDictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "list", genreHelper.series },
            { "apiKey", apiKey }
        };
            await navigationService.NavigateToAsync("//Overview", parameters);
            ((Picker)sender).SelectedItem = null;
        }
    }

    private async void HandleSerie(string selectedGenre, TvShow show, GenreHelper genreHelper)
    {
        try
        {
            int id = tMDBService.GetIdToName(selectedGenre, MediaType.Tv);
            await LoadingDialog.ShowAsync(AppResources.loading, async () =>
            {
                SearchContainer<SearchTv> discoversSeries = await this.tMDBService.DiscoverSerien(id, 1);
                if (discoversSeries.Results is not null)
                {
                    this.genreHelper.GetSerien(discoversSeries.Results);
                }
                for (int page = discoversSeries.Page + 1; page <= discoversSeries.TotalPages; page++)
                {
                    discoversSeries.Page = page;
                    discoversSeries = await this.tMDBService.DiscoverSerien(id, page);
                    if (discoversSeries.Results is not null)
                    {
                        this.genreHelper.GetSerien(discoversSeries.Results);
                    }
                }
            });
        }
        catch
        {
            await Toast.ShowAsync(AppResources.tooMuchPages, DialogType.Error);
        }
        IDictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "list", genreHelper.series },
            { "apiKey", apiKey }
        };
        await navigationService.NavigateToAsync("//Overview", parameters);
    }

    private async void HandleMovie(string selectedGenre, Movie movie, GenreHelper genreHelper)
    {
        try
        {
            int id = tMDBService.GetIdToName(selectedGenre, MediaType.Movie);
            await LoadingDialog.ShowAsync(AppResources.loading, async () =>
            {
                SearchContainer<SearchMovie> discoversMovies = await this.tMDBService.DiscoverMovies(id, 1);
                if (discoversMovies.Results is not null)
                {
                    this.genreHelper.GetMovies(discoversMovies.Results);
                }
                for (int page = discoversMovies.Page + 1; page <= discoversMovies.TotalPages; page++)
                {
                    discoversMovies.Page = page;
                    discoversMovies = await this.tMDBService.DiscoverMovies(id, page);
                    if (discoversMovies.Results is not null)
                    {
                        this.genreHelper.GetMovies(discoversMovies.Results);
                    }
                }
            });
        }
        catch
        {
            await Toast.ShowAsync(AppResources.tooMuchPages, DialogType.Error);
        }
        IDictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "list", genreHelper.movies },
            { "apiKey", apiKey }
        };
        await navigationService.NavigateToAsync("//Overview", parameters);
    }
}