using FilmManager.Interfaces;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public class OnClickPopupViewModel
    {
        private IDatabase database;

        public OnClickPopupViewModel(IDatabase database)
        {
            this.database = database;
        }
        public object? Get(string selectedItem)
        {
            string[] st = selectedItem.Split("(");
            string name = st[0].Trim();
            object? obj = Search(selectedItem, name);
            return obj;
        }

        private object? Search(string selectedItem, string name)
        {
            ObservableCollection<object> watched = this.database.SelectAllEntries("Watched");
            foreach (object o in watched)
            {
                if (o is SearchTv serie)
                {
                    if (!string.IsNullOrEmpty(serie.Name))
                    {
                        if (serie.Name.Equals(name))
                        {
                            return o;
                        }
                    }
                }
                if (o is SearchMovie movie)
                {
                    if (!string.IsNullOrEmpty(movie.Title))
                    {
                        if (movie.Title.Equals(name))
                        {
                            return o;
                        }
                    }
                }
            }
            ObservableCollection<object> watchlist = this.database.SelectAllEntries("Watchlist");
            foreach (object o in watchlist)
            {
                if (o is SearchTv serie)
                {
                    if (!string.IsNullOrEmpty(serie.Name))
                    {
                        if (serie.Name.Equals(name))
                        {
                            return o;
                        }
                    }
                }
                if (o is SearchMovie movie)
                {
                    if (!string.IsNullOrEmpty(movie.Title))
                    {
                        if (movie.Title.Equals(name))
                        {
                            return o;
                        }
                    }
                }
            }
            return null;
        }

        public string? GetName(object obj)
        {
            if (obj is SearchTv serie)
            {
                return serie.Name;
            }
            if (obj is SearchMovie movie)
            {
                return movie.Title;
            }
            return null;
        }
    }
}
