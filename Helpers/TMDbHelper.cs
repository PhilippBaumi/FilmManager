using FilmManager.Backend;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Helpers
{
    public class TMDbHelper
    {
        public async Task<object> GetCurrentObjectAsync(TMDBService service, string type, string title, int? id)
        {
            object foundItem = null;
            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(title) && id.HasValue)
            {
                if (type == "Movie")
                {
                    SearchContainer<SearchMovie> searchResult = await service.SearchMovieAsync(title);
                    foundItem = FindMovieById(searchResult, id.Value);
                }
                if (type == "Tv")
                {
                    SearchContainer<SearchTv> searchResult = await service.SearchSerieAsync(title);
                    foundItem = FindSerieById(searchResult, id.Value);
                }
            }
            return foundItem;
        }

        private SearchMovie FindMovieById(SearchContainer<SearchMovie> searchResult, int id)
        {
            SearchMovie foundMovie = null;
            if (searchResult != null && searchResult.Results != null)
            {
                for (int i = 0; i < searchResult.Results.Count; i++)
                {
                    SearchMovie movie = searchResult.Results[i];
                    if (movie.Id == id)
                    {
                        foundMovie = movie;
                        break;
                    }
                }
            }
            return foundMovie;
        }

        private SearchTv FindSerieById(SearchContainer<SearchTv> searchResult, int id)
        {
            SearchTv foundTv = null;
            if (searchResult != null && searchResult.Results != null)
            {
                for (int i = 0; i < searchResult.Results.Count; i++)
                {
                    SearchTv movie = searchResult.Results[i];
                    if (movie.Id == id)
                    {
                        foundTv = movie;
                        break;
                    }
                }
            }
            return foundTv;
        }

        public SearchTv SearchTvFromTvShow(TvShow show)
        {
            SearchTv tv = new();
            tv.BackdropPath = show.BackdropPath;
            tv.FirstAirDate = show.FirstAirDate;
            tv.GenreIds = GetGenreIds(show.Genres);
            tv.Id = show.Id;
            tv.Name = show.Name;
            tv.OriginalLanguage = show.OriginalLanguage;
            tv.OriginalName = show.OriginalName;
            tv.OriginCountry = show.OriginCountry;
            tv.Overview = show.Overview;
            tv.Popularity = show.Popularity;
            tv.PosterPath = show.PosterPath;
            tv.VoteAverage = show.VoteAverage;
            tv.VoteCount = show.VoteCount;
            return tv;
        }
        public SearchMovie SearchMovieFromMovie(Movie movie)
        {
            SearchMovie sMovie = new();
            sMovie.Adult = movie.Adult;
            sMovie.BackdropPath = movie.BackdropPath;
            sMovie.GenreIds = GetGenreIds(movie.Genres);
            sMovie.Id = movie.Id;
            sMovie.OriginalLanguage = movie.OriginalLanguage;
            sMovie.OriginalTitle = movie.OriginalTitle;
            sMovie.Overview = movie.Overview;
            sMovie.Popularity = movie.Popularity.GetValueOrDefault();
            sMovie.PosterPath = movie.PosterPath;
            sMovie.ReleaseDate = movie.ReleaseDate;
            sMovie.Title = movie.Title;
            sMovie.Video = movie.Video;
            sMovie.VoteAverage = movie.VoteAverage;
            sMovie.VoteCount = movie.VoteCount;
            return sMovie;
        }

        private List<int>? GetGenreIds(List<Genre>? genres)
        {
            List<int> genreIds = new();
            foreach(Genre g in genres)
            {
                genreIds.Add(g.Id);
            }
            return genreIds;
        }
    }
}
