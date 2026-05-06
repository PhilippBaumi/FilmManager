using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager;

public partial class DetailPage : ContentPage, IQueryAttributable
{
    private INavigationService navigationService;
    private DetailViewModel detailViewModel;

    public DetailPage(INavigationService navigationSerive)
    {
        InitializeComponent();
        this.navigationService = navigationSerive;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("content"))
        {
            object o = query["content"];
            detailViewModel = new(o);
            BindingContext = detailViewModel;
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
}