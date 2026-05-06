using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;


namespace FilmManager.Models
{
    public partial class OverviewViewModel : ObservableObject
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";

        public ObservableCollection<string> Images { get; set; } = new();
        [ObservableProperty]
        private string? imageUrl;

        public OverviewViewModel(object? o)
        {
            if (o is List<SearchTv> series)
            {
                foreach (SearchTv serie in series)
                {
                    Images.Add($"{ImageBaseUrl}{serie.PosterPath}");
                }
            }
            else if (o is List<SearchMovie> movies)
            {
                foreach (SearchMovie movie in movies)
                {
                    Images.Add($"{ImageBaseUrl}{movie.PosterPath}");
                }
            }
        }

        public void UpdateData(object? o)
        {
            Images.Clear();
            if (o is IEnumerable<SearchTv> series)
            {
                foreach (SearchTv s in series)
                {
                    Images.Add($"{ImageBaseUrl}{s.PosterPath}");
                }

            }
            else if (o is IEnumerable<SearchMovie> movies)
            {
                foreach (SearchMovie m in movies)
                {
                    Images.Add($"{ImageBaseUrl}{m.PosterPath}");
                }
            }
        }
    }
}