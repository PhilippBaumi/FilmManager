using FilmManager.Backend;
using TMDbLib.Objects.Search;

namespace FilmManager.Helpers
{
    public class GenreHelper
    {
        private TMDBService tMDBService;
        public List<SearchTv> series { get; } = new();
        public List<SearchMovie> movies { get; } = new();

        public GenreHelper(TMDBService tMDBService)
        {
            this.tMDBService = tMDBService;
        }

        public async Task<List<string>> MovieGenresAsync()
        {
            await this.tMDBService.AddSerienGenresToListAsync();
            return tMDBService.MovieGenresName;
        }

        public async Task<List<string>> SeriesGenresAsync()
        {
            await this.tMDBService.AddSerienGenresToListAsync();
            return tMDBService.SerienGenresName;
        }

        public List<string> MovieGenres()
        {
            return MovieGenresAsync().GetAwaiter().GetResult();
        }

        public List<string> SeriesGenres()
        {
            return SeriesGenresAsync().GetAwaiter().GetResult();
        }

        public void GetSerien(IEnumerable<SearchTv>? results)
        {
            if (results is not null)
            {
                foreach (SearchTv search in results)
                {
                    if (this.series.All(existing => existing.Id != search.Id))
                    {
                        series.Add(search);
                    }
                }
            }
        }

        public void GetMovies(IEnumerable<SearchMovie>? results)
        {
            if (results is not null)
            {
                foreach (SearchMovie search in results)
                {
                    if (this.movies.All(existing => existing.Id != search.Id))
                    {
                        movies.Add(search);
                    }
                }
            }
        }

        public void ClearAll()
        {
            this.series.Clear();
            this.movies.Clear();
        }
    }
}
