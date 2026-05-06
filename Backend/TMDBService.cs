using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Backend
{
    public class TMDBService
    {
        private TMDbClient client;
        public List<string> MovieGenresName { get; } = new();
        public List<string> SerienGenresName { get; } = new();
        private List<Genre> moviesGenres = new();
        private List<Genre> serienGenres = new();

        public TMDBService(TMDbClient client)
        {
            this.client = client;
        }

        public void AddMoviesGenresToList()
        {
            List<Genre> taskResult = client.GetMovieGenresAsync().Result;
            foreach (Genre genre in taskResult)
            {
                MovieGenresName.Add(genre.Name);
                moviesGenres.Add(genre);
            }
        }

        public void AddSerienGenresToList()
        {
            List<Genre> taskResult = client.GetTvGenresAsync().Result;
            foreach (Genre genre in taskResult)
            {
                SerienGenresName.Add(genre.Name);
                serienGenres.Add(genre);
            }
        }

        public async Task<SearchContainer<SearchMovie>> DiscoverMoviesByGenre(int genreId, int page)
        {
            DiscoverMovie discover = client.DiscoverMoviesAsync();
            IEnumerable<int> enumerable = new int[] { genreId };
            discover = discover.IncludeWithAllOfGenre(enumerable);
            return await discover.Query(page);
        }

        public async Task<SearchContainer<SearchTv>> DiscoverSerienByGenre(int genreId, int page)
        {
            DiscoverTv discover = client.DiscoverTvShowsAsync();
            IEnumerable<int> enumerable = new int[] { genreId };
            discover = discover.WhereGenresInclude(enumerable);
            return await discover.Query(page);
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieAsync(string search)
        {
            return await client.SearchMovieAsync(search);
        }

        public async Task<SearchContainer<SearchTv>> SearchSerieAsync(string search)
        {
            return await client.SearchTvShowAsync(search);
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            return await client.GetMovieAsync(id);
        }

        public async Task<TvShow> GetTvShowAsync(int id)
        {
            return await client.GetTvShowAsync(id);
        }

        public int GetIdToName(string selected, MediaType type)
        {
            int id = 0;
            if (selected != null)
            {
                if (type == MediaType.Movie)
                {
                    foreach (Genre genre in moviesGenres)
                    {
                        if (genre.Name.Equals(selected))
                        {
                            id = genre.Id;
                            break;
                        }
                    }
                }
                if (type == MediaType.Tv)
                {
                    foreach (Genre genre in serienGenres)
                    {
                        if (genre.Name.Equals(selected))
                        {
                            id = genre.Id;
                            break;
                        }
                    }
                }
            }
            return id;
        }
    }
}
