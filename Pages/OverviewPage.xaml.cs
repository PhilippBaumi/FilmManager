using CommunityToolkit.Maui.Extensions;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TMDbLib.Objects.Search;

namespace FilmManager;

public partial class OverviewPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private object o;
    private OverviewViewModel overviewViewModel;
    private INavigationService navigationService;
    private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";

    public event PropertyChangedEventHandler PropertyChanged;

    private IDatabase database;

    public OverviewPage(INavigationService navigation, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigation;
        this.database = database;
    }
    public object O
    {
        get => o;
        set
        {
            if (o != value)
            {
                o = value;
                OnPropertyChanged();
            }
        }
    }

    public void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("list"))
        {
            O = query["list"];
            overviewViewModel = new(O);
            BindingContext = overviewViewModel;
        }
        if(query.ContainsKey("objectlist"))
        {
            object? obj = query["objectlist"];
            if(obj is List<object> list)
            {
                if (list.Count == 1 && list[0] is SearchTv tv)
                {
                    List<SearchTv> tvs = new();
                    tvs.Add(tv);
                    O = tvs;
                    overviewViewModel = new(O);
                    BindingContext = overviewViewModel;
                }
                if(list.Count==1 && list[0] is SearchMovie movie)
                {
                    List<SearchMovie> movies = new();
                    movies.Add(movie);
                    O = movies;
                    overviewViewModel = new(O);
                    BindingContext = overviewViewModel;
                }
            }
        }
        query.Clear();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (o != null)
            {
                overviewViewModel.UpdateData(o);
            }
        });
    }
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string? selectedItem = overviewViewModel.ImageUrl;
        if (!string.IsNullOrEmpty(selectedItem))
        {
            selectedItem = selectedItem.Replace(ImageBaseUrl, string.Empty);
            OptionsPopup popup = new(selectedItem, navigationService, o, database, "");
            Application.Current.Windows[0].Page.ShowPopup(popup);
        }
        ((CollectionView)sender).SelectedItem = null;
    }
}