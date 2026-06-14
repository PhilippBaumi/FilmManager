using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class WatchlistWatchedViewModel : ObservableObject
    {
        public ObservableCollection<string> Watched { get; } = new();
        public ObservableCollection<string> Watchlist { get; } = new();

        private List<object> watchlist = new();
        private List<object> watched = new();

        [ObservableProperty]
        private string selectedItem;

        public WatchlistWatchedViewModel(ObservableCollection<object>? watchlist, ObservableCollection<object>? watched)
        {
            AddToWatched(watched);
            AddToWatchlist(watchlist);
        }

        private void AddToWatchlist(ObservableCollection<object>? list)
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

        private void AddToWatched(ObservableCollection<object>? list)
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

        public object? Get(string s)
        {
            foreach (object obj in watched)
            {
                if(MatchesDisplayText(obj, s))
                {
                    return obj;
                }
            }
            foreach (object obj in watchlist)
            {
                if(MatchesDisplayText(obj, s))
                {
                    return obj;
                }
            }
            return null;
        }

        private bool MatchesDisplayText(object obj, string s)
        {
            if(obj is SearchMovie movie)
            {
                return string.Equals(s, movie.OriginalTitle+$" ({AppResources.movie})", StringComparison.Ordinal);
            }
            if(obj is SearchTv serie)
            {
                return string.Equals(s, serie.OriginalName + " (Serie)", StringComparison.Ordinal);
            }
            return false;
        }

        public bool IsInWatchedList(object? obj)
        {
            if (obj is not null && this.watched.Contains(obj))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
