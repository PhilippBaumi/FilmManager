using CommunityToolkit.Maui.Views;
using FilmManager.Models;
using TMDbLib.Objects.People;

namespace FilmManager.Popups;

public partial class CastPopup : Popup
{
    private Person p;
    private CastPopupViewModel castPopupViewModel;

    public CastPopup(Person p)
	{
		InitializeComponent();
        this.p=p;
        this.castPopupViewModel = new(p);
        BindingContext=castPopupViewModel;
	}

    private async void ClosePopup(object sender, EventArgs e)
    {
		await CloseAsync();
    }
}