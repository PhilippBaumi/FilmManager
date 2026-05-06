using FilmManager.Interfaces;

namespace FilmManager
{
    public partial class AppShell : Shell
    {
        private INavigationService navigationService;
        public AppShell(INavigationService navigationService)
        {
            InitializeComponent();
            this.navigationService = navigationService;

            Loaded += async (s, e) => await this.navigationService.InitializeAsync();
        }
    }
}
