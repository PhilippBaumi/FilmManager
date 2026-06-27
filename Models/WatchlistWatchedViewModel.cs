using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class WatchlistWatchedViewModel
    {
        public List<string> Watched { get; } = new();
        public List<string> Watchlist { get; } = new();

        private readonly List<object> watchlist = new();
        private readonly List<object> watched = new();

        public WatchlistWatchedViewModel(List<object>? watchlist, List<object>? watched)
        {
            AddToWatched(watched);
            AddToWatchlist(watchlist);
        }

        private void AddToWatchlist(List<object>? list)
        {
            if (list is not null)
            {
                foreach (object o in list)
                {
                    if (o is SearchMovie movie)
                    {
                        this.watchlist.Add(movie);
                        Watchlist.Add(movie.OriginalTitle + $" ({AppResources.movie})");
                    }
                    if (o is SearchTv serie)
                    {
                        this.watchlist.Add(serie);
                        Watchlist.Add(serie.OriginalName + " (Serie)");
                    }
                }
            }
        }

        private void AddToWatched(List<object>? list)
        {
            if (list is not null)
            {
                foreach (object o in list)
                {
                    if (o is SearchMovie movie)
                    {
                        this.watched.Add(movie);
                        Watched.Add(movie.OriginalTitle + $" ({AppResources.movie})");
                    }
                    if (o is SearchTv serie)
                    {
                        this.watched.Add(serie);
                        Watched.Add(serie.OriginalName + " (Serie)");
                    }
                }
            }
        }

        public object? GetWatchedByIndex(int index)
        {
            if (index < 0 || index >= this.watched.Count)
            {
                return null;
            }
            return this.watched[index];
        }

        public object? GetWatchlistByIndex(int index)
        {
            if (index < 0 || index >= this.watched.Count)
            {
                return null;
            }
            return this.watchlist[index];
        }

        public bool IsInWatchedList(object? obj)
        {
            if (obj is not null && obj is SearchMovie movie)
            {
                return this.watched.Any(x => x is SearchMovie watchedMovie && watchedMovie.Id == movie.Id);
            }
            if (obj is not null && obj is SearchTv tv)
            {
                return this.watched.Any(x => x is SearchTv watchedTv && watchedTv.Id == tv.Id);
            }
            return false;
        }
    }
}
