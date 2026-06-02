using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager;

public partial class CollectionPage : ContentPage, IQueryAttributable
{

    private CollectionPageViewModel collectionPageViewModel;
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private TMDBService tMDBService;
    private INavigationService navigation;
    public CollectionPage(INavigationService navigation, TMDBService tMDBService)
    {
        InitializeComponent();
        this.collectionPageViewModel = new();
        this.navigation = navigation;
        this.tMDBService = tMDBService;
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
            object o = query["content"];
            if (o is TvShow tv)
            {
                await DisplayAlertAsync("Info", AppResources.noCollection, "OK");
            }
            if (o is Movie movie)
            {
                SearchCollection? searchCollection = movie.BelongsToCollection;
                this.collectionPageViewModel.SetList(new List<SearchCollection> { searchCollection });
                BindingContext = this.collectionPageViewModel;
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
        try
        {
            string searchString = entrySearchCollection.Text;
            SearchContainer<SearchCollection> result = await this.tMDBService.SearchCollectionAsync(searchString);
            List<SearchCollection> collection = result.Results;
            this.collectionPageViewModel.SetList(collection);
            BindingContext = this.collectionPageViewModel;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void GetCollectionToImage(object sender, SelectionChangedEventArgs e)
    {
        string selectedItem = this.collectionPageViewModel.SelectedItem;
        try
        {
            if (!string.IsNullOrEmpty(selectedItem))
            {
                selectedItem = selectedItem.Replace(ImageBaseUrl, string.Empty);
                SearchCollection search = this.collectionPageViewModel.GetSearchCollection(selectedItem);
                Collection collection = await this.tMDBService.GetCollectionAsync(search.Id);
                List<SearchMovie> movies = collection.Parts;
                IDictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "list", movies },
                    { "apiKey", tMDBService.client.ApiKey }
                };

                await navigation.NavigateToAsync("//Overview", parameters);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
        ((CollectionView)sender).SelectedItem = null;
    }
}
