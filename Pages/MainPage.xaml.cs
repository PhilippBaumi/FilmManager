using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using Mopups.PreBaked.Services;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FilmManager
{
    public partial class MainPage : ContentPage
    {
        private INavigationService navigationService;
        private TMDBService tMDBService;
        private MainViewModel mainViewModel;
        private IDatabase database;
        public MainPage(INavigationService navigation, IDatabase database)
        {
            InitializeComponent();
            this.navigationService = navigation;
            this.database = database;
            this.tMDBService = new TMDBService(new TMDbClient("c7108e21486edb11a641d92aa539f3e2"));
            this.tMDBService.AddMoviesGenresToList();
            List<string> movieGenres = tMDBService.MovieGenresName;
            this.tMDBService.AddSerienGenresToList();
            List<string> serieGenres = tMDBService.SerienGenresName;
            this.mainViewModel = new(movieGenres, serieGenres);
            BindingContext = mainViewModel;
        }

        private async void OnPickerSeriesSelectionChanged(object sender, EventArgs e)
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
                        SearchContainer<SearchTv> discoversSeries = await tMDBService.DiscoverSerien(id, 1);
                        await PreBakedMopupService.GetInstance().WrapTaskInLoader(tMDBService.DiscoverSerien(id, 1), Color.FromArgb("#0066CC"), Colors.Grey, new List<string> { $"{AppResources.page} 1 {AppResources.loading}"}, Colors.Black);
                        for (int page = discoversSeries.Page+1; page <= discoversSeries.TotalPages; page++)
                        {
                            discoversSeries.Page = page;
                            discoversSeries = await tMDBService.DiscoverSerien(id, page);
                            await PreBakedMopupService.GetInstance().WrapTaskInLoader(tMDBService.DiscoverSerien(id, page), Color.FromArgb("#0066CC"), Colors.Grey, new List<string> {$"{AppResources.page} {page} {AppResources.loading}"}, Colors.Black);
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
                        { "list", mainViewModel.series },
                        { "apiKey", "c7108e21486edb11a641d92aa539f3e2" }
                    };
                    await navigationService.NavigateToAsync("//Overview", parameters);
                }

            }
            ((Picker)sender).SelectedItem = null;
        }

        private async void OnPickerMoviesSelectionChanged(object sender, EventArgs e)
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
                        SearchContainer<SearchMovie> discoversMovies = await tMDBService.DiscoverMovies(id, 1);
                        await PreBakedMopupService.GetInstance().WrapTaskInLoader(tMDBService.DiscoverMovies(id, 1), Color.FromArgb("#0066CC"), Colors.Grey, new List<string> { $"{AppResources.page} 1 {AppResources.loading}" }, Colors.Black);
                        for (int page = discoversMovies.Page+1; page <= discoversMovies.TotalPages; page++)
                        {
                            discoversMovies.Page = page;
                            discoversMovies = await tMDBService.DiscoverMovies(id, page);
                            await PreBakedMopupService.GetInstance().WrapTaskInLoader(tMDBService.DiscoverMovies(id, page), Color.FromArgb("#0066CC"), Colors.Grey, new List<string> { $"{AppResources.page} {page} {AppResources.loading}" }, Colors.Black);
                            if (discoversMovies.Results != null)
                            {
                                mainViewModel.GetMovies(discoversMovies.Results);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlertAsync("Info", $"{ex.Message}, {AppResources.tooMuchPages}", "OK");
                    }
                }
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "list", mainViewModel.movies },
                    { "apiKey", "c7108e21486edb11a641d92aa539f3e2" }
                };
                await navigationService.NavigateToAsync("//Overview", parameters);
            }
            ((Picker)sender).SelectedItem = null;
        }

        private async void ResetDatabase(object sender, EventArgs e)
        {
            this.database.DeleteTable("Watched");
            this.database.DeleteTable("Watchlist");
            await DisplayAlertAsync("Info", AppResources.newGeneratedDatabase, "OK");
        }
    }
}
