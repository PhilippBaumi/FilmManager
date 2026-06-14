using FilmManager.Interfaces;

namespace FilmManager.Helpers
{
    internal class MauiNavigationService : INavigationService
    {
        public async Task InitializeAsync()
        {
            await NavigateToAsync("//Home");
        }

        public async Task NavigateToAsync(string route, IDictionary<string, object>? routeParameters = null)
        {
            if (Shell.Current is null)
            {
                throw new InvalidOperationException("Shell is not initialized.");
            }
            if(route=="//Overview"&&routeParameters is not null)
            {
                await NavigateToOverviewAsync(routeParameters);
                return;
            }
            if (routeParameters is not null)
            {
                await Shell.Current.GoToAsync(route, routeParameters);
            }
            else
            {
                await Shell.Current.GoToAsync(route);
            }
        }

        private async Task NavigateToOverviewAsync(IDictionary<string, object> routeParameters)
        {
            await Shell.Current.GoToAsync("//Overview");
            if(Shell.Current.CurrentPage is IQueryAttributable overviewPage)
            {
                overviewPage.ApplyQueryAttributes(new Dictionary<string, object>(routeParameters));
            }
        }
    }
}
