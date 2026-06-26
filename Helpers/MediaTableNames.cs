using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager.Helpers
{
    public static class MediaTableNames
    {
        private static readonly HashSet<string> AllowedNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Watched",
            "Watchlist"
        };

        public static string Validate(string tableName)
        {
            if (!AllowedNames.Contains(tableName))
            {
                throw new ArgumentException($"{AppResources.unknownTable}: {tableName}", nameof(tableName));
            }
            return tableName;
        }
    }
}
