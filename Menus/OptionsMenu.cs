using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Popups
{
    public class OptionsMenu
    {
        private string selectedItem;
        private INavigationService navigationService;
        private object obj;
        private TMDBService tmdbService;
        private IDatabase database;
        private string apiKey;
        private OptionMenuViewModel optionPopupViewModel = new();
        private readonly string validateWatched = MediaTableNames.Validate("Watched");
        private readonly string validateWatchlist = MediaTableNames.Validate("Watchlist");

        public OptionsMenu(string? selectedItem, INavigationService navigationService, object? o, IDatabase database, string apiKey)
        {
            this.selectedItem = selectedItem;
            this.navigationService = navigationService;
            this.database = database;
            this.apiKey = apiKey;
            this.tmdbService = new(new TMDbClient(apiKey));
            this.obj = optionPopupViewModel.Get(selectedItem, o);
        }

        public async Task ShowAsync(string action)
        {
            string title = GetTitle();
            List<ActionItem> items = AddActions(action);
            await ActionListDialog.ShowWithActionsAsync(title, items, AppResources.close);
        }

        private List<ActionItem> AddActions(string action)
        {
            List<ActionItem> items = new();
            items.Add(new ActionItem(AppResources.addToWatched, async () => await AddToWatchedAsync()));
            items.Add(new ActionItem(AppResources.addToWatchlist, async () => await AddToWatchlistAsync()));
            if (!string.Equals(action, "Detail"))
            {
                items.Add(new ActionItem(AppResources.toDetails, async () => await NavigateToDetailAsync()));
            }
            return items;
        }

        private string GetTitle()
        {
            if (this.obj is not null)
            {
                if (this.obj is SearchMovie movie)
                {
                    return movie.OriginalTitle;
                }
                if (this.obj is SearchTv tv)
                {
                    return tv.OriginalName;
                }
            }
            return string.Empty;
        }

        private async Task AddToWatchedAsync()
        {
            try
            {
                if (this.selectedItem is not null)
                {
                    if (this.obj is null)
                    {
                        await Toast.ShowAsync(AppResources.noChoosenMovieOrSerie, DialogType.Info);
                    }
                    this.database.CreateTable(this.validateWatched);
                    if (this.obj is SearchTv serie)
                    {
                        DateTime? rDate = serie.FirstAirDate;
                        if (rDate.HasValue)
                        {
                            if (rDate.Value > DateTime.Today)
                            {
                                await Toast.ShowAsync(AppResources.dateIsFuture, DialogType.Info);
                            }
                            else
                            {
                                this.database.InsertEntry(serie, this.validateWatched);
                                await Toast.ShowAsync(AppResources.insertSuccess, DialogType.Info);
                            }
                        }
                    }
                    if (this.obj is SearchMovie movie)
                    {
                        DateTime? rDate = movie.ReleaseDate;
                        if (rDate.HasValue)
                        {
                            if (rDate.Value > DateTime.Today)
                            {
                                await Toast.ShowAsync(AppResources.dateIsFuture, DialogType.Info);
                            }
                            else
                            {
                                this.database.InsertEntry(movie, this.validateWatched);
                                await Toast.ShowAsync(AppResources.insertSuccess, DialogType.Info);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.ShowAsync(ex.Message, DialogType.Error);
            }
        }

        private async Task AddToWatchlistAsync()
        {
            try
            {
                if (this.selectedItem is not null)
                {
                    if (this.obj is null)
                    {
                        await Toast.ShowAsync(AppResources.noChoosenMovieOrSerie, DialogType.Info);
                    }
                    this.database.CreateTable(this.validateWatchlist);
                    if (this.obj is SearchTv serie)
                    {
                        this.database.InsertEntry(serie, this.validateWatchlist);
                        await Toast.ShowAsync(AppResources.insertSuccess, DialogType.Info);
                    }
                    if (this.obj is SearchMovie movie)
                    {
                        this.database.InsertEntry(movie, this.validateWatchlist);
                        await Toast.ShowAsync(AppResources.insertSuccess, DialogType.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.ShowAsync(ex.Message, DialogType.Error);
            }
        }

        private async Task NavigateToDetailAsync()
        {
            try
            {
                if (this.selectedItem is not null)
                {
                    if (this.obj is not null)
                    {
                        if (this.obj is SearchTv serie)
                        {
                            TvShow tvShow = await this.tmdbService.GetTvShowAsync(serie.Id);
                            IDictionary<string, object> dict = new Dictionary<string, object>
                        {
                            { "content", tvShow },
                            { "apiKey", apiKey }
                        };
                            await navigationService.NavigateToAsync("//Detail", dict);
                        }
                        if (this.obj is SearchMovie movie)
                        {
                            Movie tMovie = await this.tmdbService.GetMovieAsync(movie.Id);
                            IDictionary<string, object> dict = new Dictionary<string, object>
                        {
                            { "content", tMovie },
                            { "apiKey", apiKey }
                        };
                            await navigationService.NavigateToAsync("//Detail", dict);
                        }
                    }
                    if (this.obj is null)
                    {
                        await Toast.ShowAsync(AppResources.cantNavigateToDetails, DialogType.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                await Toast.ShowAsync(ex.Message, DialogType.Error);
            }
        }
    }
}
