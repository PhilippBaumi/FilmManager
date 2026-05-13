using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using MediaType = TMDbLib.Objects.General.MediaType;

namespace FilmManager
{
    public partial class MainPage : ContentPage
    {
        private INavigationService navigationService;
        private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
        private TMDBService tMDBService = new(new TMDbClient(apiKey));
        private MainViewModel mainViewModel;
        private IDatabase database;
        public MainPage(INavigationService navigation, IDatabase database)
        {
            InitializeComponent();
            this.navigationService = navigation;
            this.database = database;
            this.tMDBService.AddMoviesGenresToList();
            List<string> movieGenres = tMDBService.MovieGenresName;
            this.tMDBService.AddSerienGenresToList();
            List<string> serieGenres = tMDBService.SerienGenresName;
            this.mainViewModel = new(movieGenres, serieGenres);
            BindingContext = mainViewModel;
        }
        private async void OnMoviesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedMovie = mainViewModel.SelectedMovie;
            if (selectedMovie != null)
            {
                await DisplayAlertAsync("Info", $"{selectedMovie} {AppResources.loading}", "OK");
                int id = tMDBService.GetIdToName(selectedMovie, MediaType.Movie);
                if (mainViewModel.Movies.Contains(selectedMovie))
                {
                    try
                    {
                        SearchContainer<SearchMovie> discoversMovies = await tMDBService.DiscoverMoviesByGenre(id, 1);
                        for (int page = discoversMovies.Page; page <= discoversMovies.TotalPages; page++)
                        {
                            discoversMovies.Page = page;
                            discoversMovies = await tMDBService.DiscoverMoviesByGenre(id, page);
                            if (discoversMovies.Results != null)
                            {
                                mainViewModel.GetMovies(discoversMovies.Results);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlertAsync("Info", $"{ex.Message}, {AppResources.tooMuchPages}" , "OK");
                    }
                }
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "list", mainViewModel.movies }
                };
                await navigationService.NavigateToAsync("//Overview", parameters);
            }
            ((CollectionView)sender).SelectedItem = null;
        }

        private async void OnSeriesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selectedSeries = mainViewModel.SelectedSerie;
            if (selectedSeries != null)
            {
                await DisplayAlertAsync("Info", $"{selectedSeries} {AppResources.loading}", "OK");
                int id = tMDBService.GetIdToName(selectedSeries, MediaType.Tv);
                if (mainViewModel.Serien.Contains(selectedSeries))
                {
                    try
                    {
                        SearchContainer<SearchTv> discoversSeries = await tMDBService.DiscoverSerienByGenre(id, 1);
                        if (discoversSeries.Results != null)
                        {
                            mainViewModel.GetSerien(discoversSeries.Results);
                        }
                        for (int page = discoversSeries.Page + 1; page <= discoversSeries.TotalPages; page++)
                        {
                            discoversSeries.Page = page;
                            discoversSeries = await tMDBService.DiscoverSerienByGenre(id, page);
                            if (discoversSeries.Results != null)
                            {
                                mainViewModel.GetSerien(discoversSeries.Results);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlertAsync("Info", $"{ex.Message}, {AppResources.tooMuchPages}", "OK");
                    }
                    IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", mainViewModel.series }
                    };
                    await navigationService.NavigateToAsync("//Overview", parameters);
                }

            }
            ((CollectionView)sender).SelectedItem = null;
        }

        private async void ResetDatabase(object sender, EventArgs e)
        {
            this.database.DeleteTable("Watched");
            this.database.DeleteTable("Watchlist");
            await DisplayAlertAsync("Info", AppResources.newGeneratedDatabase, "OK");
        }
    }
}
