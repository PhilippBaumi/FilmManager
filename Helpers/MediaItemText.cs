using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Helpers
{
    public static class MediaItemText
    {
        public static string GetTitle(object? o)
        {
            switch (o)
            {
                case SearchMovie movie: return movie.OriginalTitle ?? movie.Title ?? string.Empty;
                case SearchTv tv: return tv.OriginalName ?? tv.Name ?? string.Empty;
                case Movie m: return m.OriginalTitle ?? m.Title ?? string.Empty;
                case TvShow show: return show.OriginalName ?? show.Name ?? string.Empty;
                default: return string.Empty;
            }
        }

        public static DateTime? GetReleaseDate(object? o)
        {
            switch (o)
            {
                case SearchMovie searchMovie: return searchMovie.ReleaseDate;
                case SearchTv tv: return tv.FirstAirDate;
                case Movie movie: return movie.ReleaseDate;
                case TvShow show: return show.FirstAirDate;
                default: return null;
            }
        }
    }
}

