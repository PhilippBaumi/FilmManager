using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Helpers;
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

        public OverviewViewModel(object? source)
        {
            UpdateData(source);
        }

        public void UpdateData(object? source)
        {
            TMDbHelper tMDbHelper = new();
            switch(source)
            {
                case IEnumerable<SearchTv> series: tMDbHelper.SetImages(Images, series, serie=>serie.PosterPath); break;
                case IEnumerable<SearchMovie> movies: tMDbHelper.SetImages(Images, movies, movie => movie.PosterPath); break;
                default: Images.Clear(); break;
            }
        }
    }
}