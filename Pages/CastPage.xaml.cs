using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Models;
using FilmManager.Popups;
using SkiaSharp;
using TMDbLib.Client;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using CommunityToolkit.Maui.Extensions;

namespace FilmManager;

public partial class CastPage : ContentPage, IQueryAttributable
{
    private CastViewModel castViewModel;
    private string apiKey;
    public CastPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if(query.ContainsKey("cast"))
        {
            this.castViewModel=new(query["cast"]);
            BindingContext = this.castViewModel;
        }
        if(query.ContainsKey("apiKey"))
        {
            this.apiKey = query["apiKey"] as string;
        }
        query.Clear();
    }

    private void HandlePaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, "Cast");
    }

    private async void HandleSelectedCast(object sender, SelectionChangedEventArgs e)
    {
        string? selectedCast = this.castViewModel.SelectedCast;
        TMDbHelper tMDbHelper = new();
        if(selectedCast is not null)
        {
            TMDBService tMDBService = new(new TMDbClient(apiKey));
            SearchPerson? person = this.castViewModel.GetPerson(tMDbHelper.ToImagePath(selectedCast));
            if (person is not null)
            {
                Person p = await tMDBService.GetPersonAsync(person.Id);
                CastPopup castPopup = new(p);
                await this.ShowPopupAsync(castPopup);
            }
        }
        ((CollectionView)sender).SelectedItem = null;
    }
}