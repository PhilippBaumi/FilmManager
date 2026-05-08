using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Models
{
    public class DetailViewModel
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
        public string? OriginalName { get; set; }
        public string? Id { get; set; }
        public string? Language { get; set; }
        public string? Overview { get; set; }
        public string? ReleaseDate { get; set; }
        public string? Popularity { get; set; }
        public string? Backport { get; set; }
        public string? AverageVote { get; set; }
        public string? Poster { get; set; }
        public string? Homepage { get; set; }
        public string? Genres { get; set; }
        public string? CountVote { get; set; }
        public string? OriginCountry { get; set; }


        public DetailViewModel(object o)
        {
            if (o is Movie movie)
            {
                OriginalName = movie.OriginalTitle;
                Id = "ID: " + movie.Id;
                Language = $"{AppResources.language}: " + movie.OriginalLanguage;
                Poster = ImageBaseUrl + movie.PosterPath;
                Backport = ImageBaseUrl + movie.BackdropPath;
                Overview = $"{AppResources.description}: " + movie.Overview;
                ReleaseDate = $"{AppResources.releaseDate}: " + movie.ReleaseDate?.ToString("dd.MM.yyyy") ?? string.Empty;
                Popularity = $"{AppResources.popularity}: " + movie.Popularity;
                AverageVote = $"{AppResources.averageVote}: " + movie.VoteAverage;
                Homepage = movie.Homepage;
                Genres = "Genres: " + GenresToString(movie.Genres);
                CountVote = $"{AppResources.countVote}: " + movie.VoteCount;
            }
            if (o is TvShow serie)
            {
                OriginalName = serie.OriginalName;
                Id = "ID: " + serie.Id;
                Language = $"{AppResources.language}: " + serie.OriginalLanguage;
                Poster = ImageBaseUrl + serie.PosterPath;
                Backport = ImageBaseUrl + serie.BackdropPath;
                Overview = $"{AppResources.description}: " + serie.Overview;
                ReleaseDate = $"{AppResources.releaseDate}: " + serie.FirstAirDate?.ToString("dd.MM.yyyy") ?? string.Empty;
                Popularity = $"{AppResources.popularity}: " + serie.Popularity;
                AverageVote = $"{AppResources.averageVote}: " + serie.VoteAverage;
                Homepage = serie.Homepage;
                Genres = "Genres: " + GenresToString(serie.Genres);
                CountVote = $"{AppResources.countVote}: " + serie.VoteCount;
                OriginCountry = $"{AppResources.origionCountry}: " + string.Join(",", serie.OriginCountry);
            }
        }

        private string GenresToString(List<Genre>? genres)
        {
            List<string>? names = new();
            foreach (Genre? genre in genres)
            {
                if (!string.IsNullOrEmpty(genre?.Name))
                {
                    names.Add(genre.Name);
                }
            }
            return string.Join(", ", names);
        }
    }
}