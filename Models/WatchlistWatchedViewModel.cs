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
                if (obj is SearchMovie movie)
                {
                    if (movie.OriginalTitle.Equals(s))
                    {
                        return movie;
                    }
                }
                if (obj is SearchTv tv)
                {
                    if (tv.OriginalName.Equals(s))
                    {
                        return tv;
                    }
                }
            }
            foreach (object obj in watchlist)
            {
                if (obj is SearchTv serie)
                {
                    if (serie.OriginalName.Equals(s))
                    {
                        return serie;
                    }
                }
                if (obj is SearchMovie movie)
                {
                    if (movie.OriginalTitle.Equals(s))
                    {
                        return movie;
                    }
                }
            }
            return null;
        }

        public bool IsInWatchedList(object? obj)
        {
            if (obj!=null&&this.watched.Contains(obj))
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
