using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FilmManager;

public partial class SearchPage : ContentPage
{
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private SearchPageViewModel searchPageViewModel = new();
    private INavigationService navigationService;
    private TMDBService tMDBService;
    public SearchPage(INavigationService navigationService, TMDBService tmdbService)
    {
        InitializeComponent();
        this.navigationService = navigationService;
        this.tMDBService = tmdbService;
        BindingContext = searchPageViewModel;
    }
    private async void Search(object sender, EventArgs e)
    {
        string search = entrySearch.Text;
        if (rbMovie.IsChecked)
        {
            SearchContainer<SearchMovie> resultMovies = await this.tMDBService.SearchMovieAsync(search);
            List<SearchMovie> movies = resultMovies.Results;
            this.searchPageViewModel.SetList(movies);
        }
        if (rbTv.IsChecked)
        {
            SearchContainer<SearchTv> resultSerien = await this.tMDBService.SearchSerieAsync(search);
            List<SearchTv> tv = resultSerien.Results;
            this.searchPageViewModel.SetList(tv);
        }
    }

    private async void GetSearchToImage(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            string selectedItem = this.searchPageViewModel.SelectedItem;
            if (!string.IsNullOrEmpty(selectedItem))
            {
                selectedItem = selectedItem.Replace(ImageBaseUrl, string.Empty);
                List<object> list = this.searchPageViewModel.GetList(selectedItem);
                if (rbMovie.IsChecked)
                {
                    List<SearchMovie> movies = this.searchPageViewModel.GetSearchMovieList(list);
                    IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", movies },
                        { "apiKey", this.tMDBService.client.ApiKey }
                    };
                    await navigationService.NavigateToAsync("//Overview", parameters);
                }
                if (rbTv.IsChecked)
                {
                    List<SearchTv> tv = this.searchPageViewModel.GetSearchTvList(list);
                    IDictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "list", tv },
                        { "apiKey", this.tMDBService.client.ApiKey }
                    };
                    await navigationService.NavigateToAsync("//Overview", parameters);
                }
            }
            ((CollectionView)sender).SelectedItem = null;
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
        }
    }
}