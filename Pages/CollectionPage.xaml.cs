using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;
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
                await Toast.ShowAsync(AppResources.noCollection, DialogType.Info);
            }
            if (o is Movie movie)
            {
                SearchCollection? searchCollection = movie.BelongsToCollection;
                this.collectionPageViewModel.SetList(new List<SearchCollection?> { searchCollection });
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
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void GetCollectionToImage(object sender, SelectionChangedEventArgs e)
    {
        TMDbHelper tMDbHelper = new();
        string selectedItem = this.collectionPageViewModel.SelectedItem;
        try
        {
            if (!string.IsNullOrEmpty(selectedItem))
            {
                selectedItem = tMDbHelper.ToImagePath(selectedItem);
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
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
        ((CollectionView)sender).SelectedItem = null;
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, $"  {AppResources.collection}");
    }
}
