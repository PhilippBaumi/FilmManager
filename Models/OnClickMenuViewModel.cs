using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public class OnClickMenuViewModel
    {
        public bool MyContains(List<object> collection, object o)
        {
            foreach (object obj in collection)
            {
                if (o is SearchMovie movie && obj is SearchMovie m)
                {
                    if (movie.Id == m.Id)
                    {
                        return true;
                    }
                }
                if (o is SearchTv tv && obj is SearchTv t)
                {
                    if (tv.Id == t.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
