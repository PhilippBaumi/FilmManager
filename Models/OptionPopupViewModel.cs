using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public class OptionPopupViewModel
    {
        public object? Get(string selectedItem, object? o)
        {
            if (selectedItem != null&&o!=null)
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
            }
            return null;
        }
    }
}
