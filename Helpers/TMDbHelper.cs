using FilmManager.Backend;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

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
    }
}
