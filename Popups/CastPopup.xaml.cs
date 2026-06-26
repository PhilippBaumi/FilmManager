using CommunityToolkit.Maui.Views;
using FilmManager.Models;
using TMDbLib.Objects.People;

namespace FilmManager.Popups;

public partial class CastPopup : Popup
{
    private readonly CastPopupViewModel castPopupViewModel;

    public CastPopup(Person p)
    {
        InitializeComponent();
        this.castPopupViewModel = new(p);
        BindingContext = castPopupViewModel;
    }

    private async void ClosePopup(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}