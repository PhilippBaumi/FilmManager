using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using FilmManager.Resources.Strings.Sprachen;
using SkiaSharp;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using SkiaSharp.Views.Maui;

namespace FilmManager;

public partial class SearchPage : ContentPage
{
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
        TMDbHelper tMDbHelper = new();
        try
        {
            string selectedItem = this.searchPageViewModel.SelectedItem;
            if (!string.IsNullOrEmpty(selectedItem))
            {
                selectedItem = tMDbHelper.ToImagePath(selectedItem);
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
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, $"  {AppResources.search}");
    }
}