using FilmManager.Backend;
using System.Collections.ObjectModel;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Helpers
{
    public class TMDbHelper
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
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
            foreach (Genre g in genres)
            {
                genreIds.Add(g.Id);
            }
            return genreIds;
        }

        public string ToImageUrl(string? path)
        {
            if(string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }
            else
            {
                return $"{ImageBaseUrl}{path}";
            }
        }
        public string ToImagePath(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return string.Empty;
            }
            else
            {
                return imageUrl.Replace(ImageBaseUrl, string.Empty);
            }
        }

        public void SetImages<T>(ObservableCollection<string>target, IEnumerable<T>? source, Func<T, string?>function)
        {
            target.Clear();
            if(source is not null)
            {
                foreach(T t in source)
                {
                    string imageUrl=ToImageUrl(function(t));
                    if(!string.IsNullOrEmpty(imageUrl))
                    {
                        target.Add(imageUrl);
                    }
                }
            }
        }
    }
}
