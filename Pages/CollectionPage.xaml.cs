using CommunityToolkit.Maui.Extensions;
using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using TMDbLib.Client;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager;

public partial class CollectionPage : ContentPage, IQueryAttributable
{

	private INavigationService navigationService;
	private IDatabase database;
	private CollectionPageViewModel collectionPageViewModel;
    private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
	private TMDBService tMDBService;
    public CollectionPage(INavigationService navigation, IDatabase database)
	{
		InitializeComponent();
		this.navigationService = navigation;
		this.collectionPageViewModel = new();
		this.database= database;
		this.tMDBService= new(new TMDbClient(apiKey));
		BindingContext = this.collectionPageViewModel;
	}

    private async void SearchCollection(object sender, EventArgs e)
    {
		string searchString=entrySearchCollection.Text;
		SearchContainer<SearchCollection>result=await this.tMDBService.SearchCollectionAsync(searchString);
		List<SearchCollection> collection = result.Results;
        this.collectionPageViewModel.SetList(collection);
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
		if (query.ContainsKey("content"))
		{
			object o= query["content"];
			if(o is TvShow tv)
			{
				await DisplayAlertAsync(AppResources.info, AppResources.noCollection, "OK");
			}
			if(o is Movie movie)
			{
				SearchCollection searchCollection=movie.BelongsToCollection;
				this.collectionPageViewModel.SetList(new List<SearchCollection> { searchCollection });
			}
		}
        query.Clear();
    }

}