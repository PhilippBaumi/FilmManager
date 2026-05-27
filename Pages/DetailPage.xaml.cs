using CommunityToolkit.Maui.Extensions;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager;

public partial class DetailPage : ContentPage, IQueryAttributable
{
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
    private INavigationService navigationService;
    private DetailViewModel detailViewModel;
    private object o;
    private string apiKey;
    private IDatabase database;

    public DetailPage(INavigationService navigationSerive, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigationSerive;
        this.database = database;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("content"))
        {
            o = query["content"];
            detailViewModel = new(o);
            BindingContext = detailViewModel;
        }
        if(query.ContainsKey("apiKey"))
        {
            apiKey = query["apiKey"] as string;
        }
        query.Clear();
    }

    private async void HandleLinkClicked(object sender, TappedEventArgs e)
    {
        try
        {
            Uri uri = new(detailViewModel.Homepage);
            await Launcher.Default.TryOpenAsync(uri);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void HandleLogos(object sender, EventArgs e)
    {
        try
        {
            string? selectedLogo = detailViewModel.SelectedLogo;
            if (selectedLogo != null)
            {
                selectedLogo = selectedLogo.Substring(0, selectedLogo.Length - 4);
                Uri uri = new(selectedLogo);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void HandlePosters(object sender, EventArgs e)
    {
        try
        {
            string? selectedPoster = detailViewModel.SelectedPoster;
            if (selectedPoster != null)
            {
                selectedPoster = selectedPoster.Substring(0, selectedPoster.Length - 4);
                Uri uri = new(selectedPoster);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void HandleBackports(object sender, EventArgs e)
    {
        try
        {
            string? selectedBackport = detailViewModel.SelectedBackport;
            if (selectedBackport != null)
            {
                selectedBackport = selectedBackport.Substring(0, selectedBackport.Length - 4);
                Uri uri = new(selectedBackport);
                await Launcher.Default.TryOpenAsync(uri);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void HandleRecommendation(object sender, EventArgs e)
    {
        string? selectedRecommentation = detailViewModel.SelectedRecommendation;
        if (!string.IsNullOrEmpty(selectedRecommentation))
        {
            List<object> list = detailViewModel.GetList(selectedRecommentation);
            IDictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "objectlist", list },
                { "apiKey", apiKey }
            };
            await navigationService.NavigateToAsync("//Overview", parameters);
        }
    }

    private void HandlePopupShow(object sender, EventArgs e)
    {
        string? selectedPoster = detailViewModel.Poster;
        selectedPoster = selectedPoster.Replace(ImageBaseUrl, string.Empty);
        OptionsPopup popup = new(selectedPoster, navigationService,o, database, "Detail", apiKey);
        Application.Current.Windows[0].Page.ShowPopup(popup);
    }
}