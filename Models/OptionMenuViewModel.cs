using FilmManager.Backend;
using FilmManager.Helpers;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Models
{
    public class OptionMenuViewModel
    {
        public object? Get(string? selectedItem, object? o, TMDBService tMDBService)
        {
            TMDbHelper tmdbHelper = new(tMDBService, new GenreHelper(tMDBService));
            if (selectedItem is not null && o is not null)
            {
                if (o is List<SearchTv> series)
                {
                    foreach (SearchTv serie in series)
                    {
                        if (!string.IsNullOrEmpty(serie.PosterPath))
                        {
                            if (serie.PosterPath.Equals(selectedItem))
                            {
                                return serie;
                            }
                        }
                    }
                }
                else if (o is List<SearchMovie> movies)
                {
                    foreach (SearchMovie movie in movies)
                    {
                        if (!string.IsNullOrEmpty(movie.PosterPath))
                        {
                            if (movie.PosterPath.Equals(selectedItem))
                            {
                                return movie;
                            }
                        }
                    }
                }
                else if (o is TvShow show)
                {
                    return tmdbHelper.SearchTvFromTvShow(show);
                }
                else if (o is Movie movie)
                {
                    return tmdbHelper.SearchMovieFromMovie(movie);
                }
            }
            return null;
        }
    }
}
