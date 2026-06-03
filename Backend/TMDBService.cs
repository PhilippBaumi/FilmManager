using Polly;
using Polly.Retry;
using TMDbLib.Client;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Backend
{
    public class TMDBService
    {
        public TMDbClient client { get; }
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
            List<Genre>? taskResult = client.GetMovieGenresAsync().Result;
            if (taskResult is not null)
            {
                foreach (Genre genre in taskResult)
                {
                    MovieGenresName.Add(genre.Name);
                    moviesGenres.Add(genre);
                }
            }
        }

        public void AddSerienGenresToList()
        {
            List<Genre>? taskResult = client.GetTvGenresAsync().Result;
            if (taskResult is not null)
            {
                foreach (Genre genre in taskResult)
                {
                    SerienGenresName.Add(genre.Name);
                    serienGenres.Add(genre);
                }
            }
        }

        public async Task<SearchContainer<SearchMovie>> DiscoverMovies(int genreId, int page)
        {
            DiscoverMovie discover = client.DiscoverMoviesAsync();
            IEnumerable<int> enumerable = new int[] { genreId };
            discover = discover.IncludeWithAllOfGenre(enumerable);
            discover = discover.OrderBy(DiscoverMovieSortBy.PopularityDesc);
            return await RetryLoadingAsync(async () => await discover.Query(page));
        }

        public async Task<SearchContainer<SearchTv>> DiscoverSerien(int genreId, int page)
        {
            DiscoverTv discover = client.DiscoverTvShowsAsync();
            IEnumerable<int> enumerable = new int[] { genreId };
            discover = discover.WhereGenresInclude(enumerable);
            discover = discover.OrderBy(DiscoverTvShowSortBy.PopularityDesc);
            return await RetryLoadingAsync(async () => await discover.Query(page));
        }

        public async Task<SearchContainer<SearchCollection>> SearchCollectionAsync(string search)
        {
            return await RetryLoadingAsync(async () => await client.SearchCollectionAsync(search));
        }

        public async Task<SearchContainer<SearchMovie>> SearchMovieAsync(string search)
        {
            return await RetryLoadingAsync(async () => await client.SearchMovieAsync(search));
        }

        public async Task<SearchContainer<SearchTv>> SearchSerieAsync(string search)
        {
            return await RetryLoadingAsync(async () => await client.SearchTvShowAsync(search));
        }

        public async Task<Movie> GetMovieAsync(int id)
        {
            return await RetryLoadingAsync(async () => await client.GetMovieAsync(id, MovieMethods.Credits | MovieMethods.Images | MovieMethods.Videos | MovieMethods.Lists | MovieMethods.Recommendations | MovieMethods.WatchProviders));
        }

        public async Task<TvShow> GetTvShowAsync(int id)
        {
            return await RetryLoadingAsync(async () => await client.GetTvShowAsync(id, TvShowMethods.Credits | TvShowMethods.Images | TvShowMethods.Videos | TvShowMethods.WatchProviders | TvShowMethods.Recommendations));
        }

        public async Task<Collection> GetCollectionAsync(int id)
        {
            return await RetryLoadingAsync(async () => await this.client.GetCollectionAsync(id));
        }

        public int GetIdToName(string selected, MediaType type)
        {
            int id = 0;
            if (selected is not null)
            {
                if (type is MediaType.Movie)
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
                if (type is MediaType.Tv)
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
        private async Task<T> RetryLoadingAsync<T>(Func<Task<T>> operation)
        {
            ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 5,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential
                })
                .AddTimeout(TimeSpan.FromSeconds(15))
                .Build();
            return await pipeline.ExecuteAsync(async _ => await operation());
        }
    }
}
