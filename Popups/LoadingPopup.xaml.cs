namespace FilmManager.Popups;

public partial class LoadingPopup : ContentView
{
	public string LoadingText { get;}
	public LoadingPopup(string text)
	{
		InitializeComponent();
		LoadingText = text;
		BindingContext = this;
	}
}