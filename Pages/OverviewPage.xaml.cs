using CommunityToolkit.Maui.Extensions;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.ComponentModel;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using TMDbLib.Objects.Search;

namespace FilmManager;

public partial class OverviewPage : ContentPage, IQueryAttributable
{
    private IDatabase database;
    private INavigationService navigationService;
    private OverviewViewModel overviewViewModel=new(null);
    private object? content;
    private string? apiKey;

    public OverviewPage(INavigationService navigation, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigation;
        this.database = database;
        BindingContext = overviewViewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        this.content = GetContentForOverview(query);
        if(query.TryGetValue("apiKey", out object? key))
        {
            apiKey=key as string;
        }
        query.Clear();
        MainThread.BeginInvokeOnMainThread(() => overviewViewModel.UpdateData(content));
    }

    private object? GetContentForOverview(IDictionary<string, object> query)
    {
        if (query.TryGetValue("list", out object? list))
        {
            return list;
        } 

        if(query.TryGetValue("objectlist", out object? objectlist)&&objectlist is List<object> { Count: 1}values)
        {
            switch(values[0])
            {
                case SearchTv tv: return new List<SearchTv> { tv };
                case SearchMovie movie: return new List<SearchMovie> { movie };
            }
            return null;
        }
        return null;
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string? selectedItem = overviewViewModel.ImageUrl;
        TMDbHelper tMDbHelper = new TMDbHelper();
        if (!string.IsNullOrEmpty(selectedItem)&&content is not null&&!string.IsNullOrEmpty(apiKey))
        {
            string selectedPath = tMDbHelper.ToImagePath(selectedItem);
            OptionsMenu optionsMenu = new(selectedPath, navigationService, content, database, apiKey);
            await optionsMenu.ShowAsync("Overview");
        }
        ((CollectionView)sender).SelectedItem = null;
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, $"  {AppResources.overview}");
    }
}