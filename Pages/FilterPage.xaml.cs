using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager;

public partial class FilterPage : ContentPage
{

	private List<string> options=new();
	public FilterPage()
	{
		InitializeComponent();
	}

    private async void HandleFilter(object sender, EventArgs e)
    {
		if(rbRegion.IsChecked)
		{
			options.Add("Region");
		}
		if(rbSprache.IsChecked)
		{
			options.Add("Sprache");
		}
    }
}