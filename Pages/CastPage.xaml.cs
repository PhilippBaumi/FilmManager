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
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
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
        if(selectedCast is not null)
        {
            TMDBService tMDBService = new(new TMDbClient(apiKey));
            SearchPerson? person = this.castViewModel.GetPerson(selectedCast.Replace(ImageBaseUrl, ""));
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