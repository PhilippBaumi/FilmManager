using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using System.Collections.ObjectModel;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Popups
{
    public class OnClickMenu
    {
        private object? o;
        private bool inWatchedList;
        private IDatabase database;
        private INavigationService navigationService;
        private string apiKey;
        private OnClickMenuViewModel onClickPopupViewModel = new();

        public OnClickMenu(object? obj, bool inWatchedList, IDatabase database, INavigationService navigationService, string apiKey)
        {
            this.o = obj;
            this.inWatchedList = inWatchedList;
            this.database = database;
            this.navigationService = navigationService;
            this.apiKey = apiKey;
        }

        public async Task ShowAsync()
        {
            string title = GetTitle();
            List<ActionItem> items = AddActions();
            await ActionListDialog.ShowWithActionsAsync(title, items, AppResources.close);
        }

        private List<ActionItem> AddActions()
        {
            List<ActionItem> items = new();
            if (!inWatchedList)
            {
                items.Add(new ActionItem(AppResources.markAsWatched, async () => await MarkAsWatchedAsync()));
            }
            items.Add(new ActionItem(AppResources.remove, async () => await RemoveAsync()));
            items.Add(new ActionItem(AppResources.toDetails, async () => await NavigateToDetailAsync()));
            return items;
        }

        private async Task NavigateToDetailAsync()
        {
            TMDBService tMDBService = new(new TMDbClient(apiKey));
            IDictionary<string, object> dict;
            if (this.o is SearchMovie movie)
            {
                Movie m = await tMDBService.GetMovieAsync(movie.Id);
                dict = new Dictionary<string, object>
            {
                {"content", m },
                {"apiKey", apiKey }
            };
                await navigationService.NavigateToAsync("//Detail", dict);
            }
            if (this.o is SearchTv tv)
            {
                TvShow show = await tMDBService.GetTvShowAsync(tv.Id);
                dict = new Dictionary<string, object>
            {
                {"content", show },
                {"apiKey", apiKey  }
            };
                await navigationService.NavigateToAsync("//Detail", dict);
            }
        }

        private async Task RemoveAsync()
        {
            List<object> watched = this.database.SelectAllEntries("Watched");
            List<object> watchlist = this.database.SelectAllEntries("Watchlist");
            if (o is SearchMovie movie)
            {
                bool result = await ShowDialog(movie.OriginalTitle);
                if(result)
                {
                    if (!this.onClickPopupViewModel.MyContains(watched, movie) && !this.onClickPopupViewModel.MyContains(watchlist, movie))
                    {
                        await Toast.ShowAsync(AppResources.notRemoved, DialogType.Info);
                        return;
                    }
                    if (this.onClickPopupViewModel.MyContains(watched, movie))
                    {
                        this.database.DeleteEntry(movie, "Watched");
                        await Snackbar.ShowAsync(AppResources.removed, AppResources.undo, async () =>
                        {
                            this.database.InsertEntry(movie, "Watched");
                            await Toast.ShowAsync(AppResources.undoSuccess, DialogType.Success);
                        });
                        return;
                    }
                    if (this.onClickPopupViewModel.MyContains(watchlist, movie))
                    {
                        this.database.DeleteEntry(movie, "Watchlist");
                        await Snackbar.ShowAsync(AppResources.removed, AppResources.undo, async () =>
                        {
                            this.database.InsertEntry(movie, "Watchlist");
                            await Toast.ShowAsync(AppResources.undoSuccess, DialogType.Success);
                        });
                        return;
                    }
                }
                else
                {
                    await Toast.ShowAsync(AppResources.notRemoved, DialogType.Info);
                    return;
                }
            }
            else if (o is SearchTv tv)
            {
                bool result = await ShowDialog(tv.OriginalName);
                if (result)
                {
                    if (!this.onClickPopupViewModel.MyContains(watched, tv) && !this.onClickPopupViewModel.MyContains(watchlist, tv))
                    {
                        await Toast.ShowAsync(AppResources.notRemoved, DialogType.Info);
                        return;
                    }
                    if (this.onClickPopupViewModel.MyContains(watched, tv))
                    {
                        this.database.DeleteEntry(tv, "Watched");
                        await Snackbar.ShowAsync(AppResources.removed, AppResources.undo, async () =>
                        {
                            this.database.InsertEntry(tv, "Watched");
                            await Toast.ShowAsync(AppResources.undoSuccess, DialogType.Success);
                        });
                        return;
                    }
                    if (this.onClickPopupViewModel.MyContains(watchlist, tv))
                    {
                        this.database.DeleteEntry(tv, "Watchlist");
                        await Snackbar.ShowAsync(AppResources.removed, AppResources.undo, async () =>
                        {
                            this.database.InsertEntry(tv, "Watchlist");
                            await Toast.ShowAsync(AppResources.undoSuccess, DialogType.Success);
                        });
                        return;
                    }
                }
                else
                {
                    await Toast.ShowAsync(AppResources.notRemoved, DialogType.Info);
                    return;
                }
            }
        }

        private async Task MarkAsWatchedAsync()
        {
            if (o is SearchMovie movie)
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
                        this.database.DeleteEntry(movie, "Watchlist");
                        this.database.InsertEntry(movie, "Watched");
                        await Toast.ShowAsync(AppResources.markedAsWatched, DialogType.Success);
                    }
                }
            }
            if (o is SearchTv tv)
            {
                DateTime? rDate = tv.FirstAirDate;
                if (rDate.HasValue)
                {
                    if (rDate.Value > DateTime.Today)
                    {
                        await Toast.ShowAsync(AppResources.dateIsFuture, DialogType.Info);
                    }
                    else
                    {
                        this.database.DeleteEntry(tv, "Watchlist");
                        this.database.InsertEntry(tv, "Watched");
                        await Toast.ShowAsync(AppResources.markedAsWatched, DialogType.Success);
                    }
                }
            }
        }

        private string GetTitle()
        {
            if (this.o is not null)
            {
                if (this.o is SearchMovie movie)
                {
                    return movie.OriginalTitle;
                }
                if (this.o is SearchTv tv)
                {
                    return tv.OriginalName;
                }
            }
            return string.Empty;
        }

        private async Task<bool> ShowDialog(string name)
        {
            SemaphoreSlim isDialogShown = new(1, 1);
            if (!await isDialogShown.WaitAsync(0))
            {
                return false;
            }
            try
            {
                ConfirmDialog dialog = new(AppResources.delete, $"{name} {AppResources.confirmDelete}", AppResources.yes, AppResources.no);
                return await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                await Toast.ShowAsync(ex.Message, DialogType.Error);
                return false;
            }
            finally
            {
                isDialogShown.Dispose();
            }
        }
    }
}
