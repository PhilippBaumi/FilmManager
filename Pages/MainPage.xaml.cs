using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FilmManager
{
    public partial class MainPage : ContentPage
    {
        private readonly INavigationService navigationService;
        private readonly TMDBService tMDBService;
        private readonly IDatabase database;
        private MainViewModel? mainViewModel;
        private readonly GenreHelper genreHelper;
        private readonly string validateWatched = MediaTableNames.Validate("Watched");
        private readonly string validateWatchlist = MediaTableNames.Validate("Watchlist");
        public MainPage(INavigationService navigation, TMDBService tMDBService, IDatabase database)
        {
            InitializeComponent();
            this.navigationService = navigation;
            this.database = database;
            this.tMDBService = tMDBService;
            this.genreHelper = new(tMDBService);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                this.mainViewModel = new(await this.genreHelper.MovieGenresAsync(), await this.genreHelper.SeriesGenresAsync());
            }
            catch (Exception ex)
            {
                Dispatcher.Dispatch(async () =>
                {
                    await Task.Delay(200);
                    await Toast.ShowAsync(ex.Message, DialogType.Error);
                });
            }
            finally
            {
                if (this.mainViewModel is not null)
                {
                    BindingContext = mainViewModel;
                }
                else
                {
                    BindingContext = new MainViewModel(null, null);
                }
            }
        }

        private async void OnPickerSeriesSelectionChanged(object sender, EventArgs e)
        {
            string? selectedSeries = mainViewModel?.SelectedSerie;
            if (selectedSeries is not null)
            {
                try
                {
                    int id = tMDBService.GetIdToName(selectedSeries, MediaType.Tv);
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
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    if (errorMessage.Equals("TMDb returned an unexpected HTTP error: 400"))
                    {
                        errorMessage = errorMessage.Remove(0, 28);
                    }
                    await Toast.ShowAsync(errorMessage, DialogType.Error);
                }
                finally
                {
                    IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", genreHelper.series },
                        { "apiKey", tMDBService.client.ApiKey }
                    };
                    await navigationService.NavigateToAsync("//Overview", parameters);
                    ((Picker)sender).SelectedItem = null;
                }
            }
        }

        private async void OnPickerMoviesSelectionChanged(object sender, EventArgs e)
        {
            string? selectedMovie = mainViewModel?.SelectedMovie;
            if (selectedMovie is not null)
            {
                try
                {
                    int id = tMDBService.GetIdToName(selectedMovie, MediaType.Movie);
                    await LoadingDialog.ShowAsync(AppResources.loading, async () =>
                    {
                        SearchContainer<SearchMovie> discoversMovies = await tMDBService.DiscoverMovies(id, 1);
                        if (discoversMovies.Results is not null)
                        {
                            genreHelper.GetMovies(discoversMovies.Results);
                        }
                        for (int page = discoversMovies.Page + 1; page <= discoversMovies.TotalPages; page++)
                        {
                            discoversMovies.Page = page;
                            discoversMovies = await tMDBService.DiscoverMovies(id, page);
                            if (discoversMovies.Results is not null)
                            {
                                genreHelper.GetMovies(discoversMovies.Results);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    if (errorMessage.Equals("TMDb returned an unexpected HTTP error: 400"))
                    {
                        errorMessage = errorMessage.Remove(0, 28);
                    }
                    await Toast.ShowAsync(errorMessage, DialogType.Error);
                }
                finally
                {
                    IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", genreHelper.movies },
                        { "apiKey", tMDBService.client.ApiKey }
                    };
                    await navigationService.NavigateToAsync("//Overview", parameters);
                    ((Picker)sender).SelectedItem = null;
                }
            }
        }

        private async void ResetDatabase(object sender, EventArgs e)
        {
            this.database.DeleteTable(this.validateWatched);
            this.database.DeleteTable(this.validateWatchlist);
            await Toast.ShowAsync(AppResources.newGeneratedDatabase, DialogType.Success);
        }

        private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            SKImageInfo info = e.Info;
            canvas.Clear(SKColors.Transparent);
            SKiaDrawHelper.DrawHeader(canvas, info, $"  {AppResources.home}");
        }
    }
}