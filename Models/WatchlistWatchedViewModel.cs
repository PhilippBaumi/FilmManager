
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

        [ObservableProperty]
        private string selectedItem;

        public WatchlistWatchedViewModel(ObservableCollection<object>? watchlist, ObservableCollection<object>? watched)
        {
            AddToWatched(watched);
            AddToWatchlist(watchlist);
        }

        private void AddToWatchlist(ObservableCollection<object>? watchlist)
        {
            if (watchlist is not null)
            {
                foreach (object o in watchlist)
                {
                    if (o is SearchMovie movie)
                    {
                        Watchlist.Add(movie.Title + $" ({AppResources.movie})");
                    }
                    if (o is SearchTv serie)
                    {
                        Watchlist.Add(serie.Name + " (Serie)");
                    }
                }
            }
        }

        private void AddToWatched(ObservableCollection<object>? watched)
        {
            if (watched is not null)
            {
                foreach (object o in watched)
                {
                    if (o is SearchMovie movie)
                    {
                        Watched.Add(movie.Title + $" ({AppResources.movie})");
                    }
                    if (o is SearchTv serie)
                    {
                        Watched.Add(serie.Name + " (Serie)");
                    }
                }
            }
        }
    }
}
