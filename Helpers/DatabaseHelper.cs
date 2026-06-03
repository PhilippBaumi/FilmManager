using System.Reflection;

namespace FilmManager.Helpers
{
    public class DatabaseHelper
    {
        public List<string>? GetStringListFromString(string? s)
        {
            List<string> results = new();
            if (!string.IsNullOrEmpty(s))
            {
                string[] st = s.Split(",");
                foreach (string s2 in st)
                {
                    results.Add(s2);
                }
            }
            return results;
        }

        public List<int>? GetIntListFromString(string? s)
        {
            List<int> results = new();
            if (!string.IsNullOrEmpty(s))
            {
                string[] st = s.Split(",");
                foreach (string s2 in st)
                {
                    if (int.TryParse(s2, out int id))
                    {
                        results.Add(id);
                    }
                }
            }
            return results;
        }

        public int? GetId(object entry)
        {
            PropertyInfo? propertyInfo = entry.GetType().GetProperty("Id");
            if (propertyInfo is null)
            {
                return null;
            }
            else
            {
                return Convert.ToInt32(propertyInfo.GetValue(entry));
            }
        }
    }
}
