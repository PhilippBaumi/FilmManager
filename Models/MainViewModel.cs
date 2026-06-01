using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<string> Serien { get; set; } = new();
        public ObservableCollection<string> Movies { get; set; } = new();

        public List<SearchTv> series { get; } = new();
        public List<SearchMovie> movies { get; } = new();

        [ObservableProperty]
        private string? selectedSerie;
        [ObservableProperty]
        private string? selectedMovie;

        public MainViewModel(List<string>? movieGenres, List<string>? serieGenres)
        {
            if (movieGenres != null && serieGenres != null)
            {
                AddSerien(serieGenres);
                AddMovies(movieGenres);
            }
        }

        private void AddMovies(List<string> movieGenres)
        {
            foreach (string movie in movieGenres)
            {
                Movies.Add(movie);
            }
        }

        private void AddSerien(List<string> serieGenres)
        {
            foreach (string serie in serieGenres)
            {
                Serien.Add(serie);
            }
        }
        public void GetSerien(List<SearchTv> results)
        {
            foreach (SearchTv search in results)
            {
                if (!series.Contains(search))
                {
                    series.Add(search);
                }
            }
        }

        public void GetMovies(List<SearchMovie> results)
        {
            foreach (SearchMovie search in results)
            {
                if (!movies.Contains(search))
                {
                    movies.Add(search);
                }
            }
        }
    }
}
