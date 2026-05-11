using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FilmManager;

public partial class SearchPage : ContentPage
{
    private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private SearchPageViewModel searchPageViewModel = new();
    private INavigationService navigationService;
    public SearchPage(INavigationService navigationService)
	{
		InitializeComponent();
        this.navigationService = navigationService;
		BindingContext = searchPageViewModel;
	}

    private async void Search(object sender, EventArgs e)
    {
		TMDBService tMDBService=new TMDBService(new TMDbClient(apiKey));
		string search=entrySearch.Text;
		if(rbMovie.IsChecked)
		{
			SearchContainer<SearchMovie>resultMovies=await tMDBService.SearchMovieAsync(search);
			List<SearchMovie>movies=resultMovies.Results;
			this.searchPageViewModel.SetList(movies);
		}
		if (rbTv.IsChecked)
		{
			SearchContainer<SearchTv> resultSerien = await tMDBService.SearchSerieAsync(search);
			List<SearchTv> tv = resultSerien.Results;
			this.searchPageViewModel.SetList(tv);
		}
    }

    private async void GetSearchToImage(object sender, SelectionChangedEventArgs e)
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
                     { "list", movies }
                };
                await navigationService.NavigateToAsync("//Overview", parameters);
            }
            if (rbTv.IsChecked)
            {
                List<SearchTv> tv = this.searchPageViewModel.GetSearchTvList(list);
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                     { "list", tv }
                };
                await navigationService.NavigateToAsync("//Overview", parameters);
            }
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}