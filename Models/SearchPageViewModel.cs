using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class SearchPageViewModel : ObservableObject
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
        public ObservableCollection<string> Images { get; set; } = new();

        private List<SearchMovie> m = new();
        private List<SearchTv> t = new();

        [ObservableProperty]
        private string selectedItem;
        public void SetList(object list)
        {
            this.Images.Clear();
            if (list is List<SearchMovie> movies)
            {
                this.m = movies;
                foreach (SearchMovie movie in movies)
                {
                    this.Images.Add($"{ImageBaseUrl}{movie.PosterPath}");
                }
            }
            if (list is List<SearchTv> tv)
            {
                this.t = tv;
                foreach (SearchTv searchTv in tv)
                {
                    this.Images.Add($"{ImageBaseUrl}{searchTv.PosterPath}");
                }
            }
        }

        public List<object> GetList(string selectedItem)
        {
            List<object> objs = new();
            foreach (SearchMovie movie in this.m)
            {
                if (!string.IsNullOrEmpty(movie.PosterPath))
                {
                    if (movie.PosterPath.Equals(selectedItem))
                    {
                        objs.Add(movie);
                    }
                }
            }
            foreach (SearchTv tv in this.t)
            {
                if (!string.IsNullOrEmpty(tv.PosterPath))
                {
                    if (tv.PosterPath.Equals(selectedItem))
                    {
                        objs.Add(tv);
                    }
                }
            }
            return objs;
        }

        public List<SearchMovie> GetSearchMovieList(List<object> list)
        {
            List<SearchMovie> movies = new();
            foreach (object o in list)
            {
                if (o is SearchMovie movie)
                {
                    movies.Add(movie);
                }
            }
            return movies;
        }

        public List<SearchTv> GetSearchTvList(List<object> list)
        {
            List<SearchTv> tv = new();
            foreach (object o in list)
            {
                if (o is SearchTv t)
                {
                    tv.Add(t);
                }
            }
            return tv;
        }
    }
}
