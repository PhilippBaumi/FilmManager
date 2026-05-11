using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Popups;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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
            OnPropertyChanged(nameof(O));
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
            OptionsPopup popup = new(selectedItem, navigationService, o, database);
            Application.Current.MainPage.ShowPopup(popup);
        }
        ((CollectionView)sender).SelectedItem = null;
    }
}