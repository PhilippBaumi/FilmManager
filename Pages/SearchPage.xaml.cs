using FilmManager.Backend;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace FilmManager;

public partial class SearchPage : ContentPage
{
    private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
	private SearchPageViewModel searchPageViewModel = new();
    public SearchPage()
	{
		InitializeComponent();
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
}