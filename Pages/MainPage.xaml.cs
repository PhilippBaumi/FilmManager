using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;

using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FilmManager
{
    public partial class MainPage : ContentPage
    {
        private INavigationService navigationService;
        private TMDBService tMDBService;
        private IDatabase database;
        private MainViewModel mainViewModel;
        public MainPage(INavigationService navigation, TMDBService tMDBService, IDatabase database)
        {
            InitializeComponent();
            this.navigationService = navigation;
            this.database = database;
            this.tMDBService = tMDBService;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                this.tMDBService.AddMoviesGenresToList();
                List<string> movieGenres = tMDBService.MovieGenresName;
                this.tMDBService.AddSerienGenresToList();
                List<string> serieGenres = tMDBService.SerienGenresName;
                this.mainViewModel = new(movieGenres, serieGenres);
            }
            catch (Exception ex)
            {
                await AlertHelper.ErrorAlert($"{ex.Message}, {AppResources.loadingError}");
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
            string? selectedSeries = mainViewModel.SelectedSerie;
            if (selectedSeries is not null)
            {
                int id = tMDBService.GetIdToName(selectedSeries, MediaType.Tv);
                LoadingPopup loadingPopup = new(AppResources.loading);
                CancellationTokenSource cts = new();
                Task popupTask = this.ShowPopupAsync(loadingPopup, new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                }, cts.Token);
                try
                {
                    SearchContainer<SearchTv> discoversSeries = await tMDBService.DiscoverSerien(id, 1);
                    if (discoversSeries.Results is not null)
                    {
                        mainViewModel.GetSerien(discoversSeries.Results);
                    }
                    for (int page = discoversSeries.Page + 1; page <= discoversSeries.TotalPages; page++)
                    {
                        discoversSeries.Page = page;
                        discoversSeries = await tMDBService.DiscoverSerien(id, page);
                        if (discoversSeries.Results is not null)
                        {
                            mainViewModel.GetSerien(discoversSeries.Results);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await AlertHelper.ErrorAlert($"{ex.Message}, {AppResources.tooMuchPages}");
                }
                finally
                {
                    cts.Cancel();
                }
                IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", mainViewModel.series },
                        { "apiKey", tMDBService.client.ApiKey }
                    };
                await navigationService.NavigateToAsync("//Overview", parameters);
            }
            ((Picker)sender).SelectedItem = null;
        }

        private async void OnPickerMoviesSelectionChanged(object sender, EventArgs e)
        {
            string? selectedMovie = mainViewModel.SelectedMovie;
            if (selectedMovie is not null)
            {
                int id = tMDBService.GetIdToName(selectedMovie, MediaType.Movie);
                LoadingPopup loadingPopup = new(AppResources.loading);
                CancellationTokenSource cts = new();
                Task popupTask = this.ShowPopupAsync(loadingPopup, new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = false
                }, cts.Token);
                try
                {
                    SearchContainer<SearchMovie> discoversMovies = await tMDBService.DiscoverMovies(id, 1);
                    if (discoversMovies.Results is not null)
                    {
                        mainViewModel.GetMovies(discoversMovies.Results);
                    }
                    for (int page = discoversMovies.Page + 1; page <= discoversMovies.TotalPages; page++)
                    {
                        discoversMovies.Page = page;
                        discoversMovies = await tMDBService.DiscoverMovies(id, page);
                        if (discoversMovies.Results is not null)
                        {
                            mainViewModel.GetMovies(discoversMovies.Results);
                        }
                    }
                }
                catch (Exception ex)
                {
                    await AlertHelper.ErrorAlert($"{ex.Message}, {AppResources.tooMuchPages}");
                }
                finally
                {
                    cts.Cancel();
                }
                IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", mainViewModel.movies },
                        { "apiKey", tMDBService.client.ApiKey }
                    };
                await navigationService.NavigateToAsync("//Overview", parameters);
            }
            ((Picker)sender).SelectedItem = null;
        }

        private async void ResetDatabase(object sender, EventArgs e)
        {
            this.database.DeleteTable("Watched");
            this.database.DeleteTable("Watchlist");
            await AlertHelper.InfoAlert(AppResources.newGeneratedDatabase);
        }
    }
}