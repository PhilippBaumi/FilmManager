using Polly;
using Polly.Retry;
using TMDbLib.Client;
using TMDbLib.Objects.Collections;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.People;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Backend
{
    public sealed class TMDBService
    {
        public TMDbClient client { get; }
        public List<string> MovieGenresName { get; } = new();
        public List<string> SerienGenresName { get; } = new();
        private readonly List<Genre> moviesGenres = new();
        private readonly List<Genre> serienGenres = new();
        private readonly SemaphoreSlim movieGenreLock = new(1, 1);
        private readonly SemaphoreSlim tvGenreLock = new(1, 1);

        public TMDBService(TMDbClient client)
        {
            this.client = client;
        }

        public async Task AddMoviesGenresToListAsync()
        {
            await this.movieGenreLock.WaitAsync();
            try
            {
                if (this.moviesGenres.Count > 0)
                {
                    return;
                }
                List<Genre>? taskResult = await RetryLoadingAsync(() => this.client.GetMovieGenresAsync());
                this.MovieGenresName.Clear();
                this.moviesGenres.Clear();
                if (taskResult is not null)
                {
                    foreach (Genre genre in taskResult)
                    {
                        if (!string.IsNullOrWhiteSpace(genre.Name))
                        {
                            this.MovieGenresName.Add(genre.Name);
                        }
                        this.moviesGenres.Add(genre);
                    }
                }
            }
            finally
            {
                this.movieGenreLock.Release();
            }
        }

        public async Task AddSerienGenresToListAsync()
        {
            await this.tvGenreLock.WaitAsync();
            try
            {
                if (this.serienGenres.Count > 0)
                {
                    return;
                }
                List<Genre>? taskResult = await RetryLoadingAsync(() => this.client.GetTvGenresAsync());
                this.SerienGenresName.Clear();
                this.serienGenres.Clear();
                if (taskResult is not null)
                {
                    foreach (Genre genre in taskResult)
                    {
                        if (!string.IsNullOrWhiteSpace(genre.Name))
                        {
                            this.SerienGenresName.Add(genre.Name);
                        }
                        this.serienGenres.Add(genre);
                    }
                }
            }
            finally
            {
                this.tvGenreLock.Release();
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

        public async Task<SearchContainer<SearchPerson>> SearchPersonAsync(string name)
        {
            return await RetryLoadingAsync(async () => await this.client.SearchPersonAsync(name));
        }

        public async Task<Person> GetPersonAsync(int id)
        {
            return await RetryLoadingAsync(async () => await this.client.GetPersonAsync(id));
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
                        if (string.Equals(genre.Name, selected))
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
                        if (string.Equals(genre.Name, selected))
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
