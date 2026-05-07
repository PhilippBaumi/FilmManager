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

	private CollectionPageViewModel collectionPageViewModel;
    private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
	private TMDBService tMDBService;
    public CollectionPage()
	{
		InitializeComponent();
		this.collectionPageViewModel = new();
		this.tMDBService= new(new TMDbClient(apiKey));
		BindingContext = this.collectionPageViewModel;
	}

    private async void SearchCollection(object sender, EventArgs e)
    {
		ReadEntryAndShow();
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
				BindingContext= this.collectionPageViewModel;
			}
		}
        query.Clear();
    }

    private async void OnCompleted(object sender, EventArgs e)
    {
		ReadEntryAndShow();
    }

    private async void ReadEntryAndShow()
    {
        string searchString = entrySearchCollection.Text;
        SearchContainer<SearchCollection> result = await this.tMDBService.SearchCollectionAsync(searchString);
        List<SearchCollection> collection = result.Results;
        this.collectionPageViewModel.SetList(collection);
		BindingContext= this.collectionPageViewModel;
    }
}