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
            if (routeParameters != null)
            {
                await Shell.Current.GoToAsync(route, routeParameters);
            }
            else
            {
                await Shell.Current.GoToAsync(route);
            }
        }
    }
}
