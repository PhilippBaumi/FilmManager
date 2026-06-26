using Dapper;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Resources.Strings.Sprachen;
using Microsoft.Data.Sqlite;
using TMDbLib.Objects.Search;

namespace FilmManager.Backend
{
    public sealed class Database : IDatabase, IDisposable
    {
        private readonly FileHelper fileHelper = new();
        private readonly DatabaseHelper databaseHelper = new();
        private readonly SqliteConnection connection;
        public Database(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(AppResources.databasePathError, nameof(path));
            }
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            this.connection = new($"Data Source={path}");
            this.connection.Open();
        }

        public void Dispose()
        {
            this.connection.Dispose();
        }
        public void CreateTable(string tableName)
        {
            this.connection.Execute($"""CREATE TABLE IF NOT EXISTS {tableName}(Id Integer PRIMARY KEY, MediaType Text NOT NULL, Title Text NOT NULL, OriginalTitle Text NOT NULL, Overview Text NOT NULL, GenreIds Text NOT NULL, OriginCountry Text NULL, OriginalLanguage Text NOT NULL, ReleaseDate Text NULL, BackdropPath Text NOT NULL, PosterPath Text NOT NULL, Popularity Real NOT NULL, VoteAverage Real NOT NULL, VoteCount Integer NOT NULL, Adult Integer NULL, Video Integer NULL)""");
        }

        public void DeleteTable(string tableName)
        {
            this.connection.Execute($"DROP TABLE IF EXISTS {tableName}");
        }

        public void DeleteEntry(object entry, string tableName)
        {
            int? getId = databaseHelper.GetId(entry);
            if (getId.HasValue)
            {
                this.connection.Execute($"DELETE FROM {tableName} WHERE Id=@Id", new { Id = getId });
            }
        }

        public void InsertEntry(object entry, string tableName)
        {
            string command = $"""INSERT OR REPLACE INTO {tableName} (Id, MediaType, Title, OriginalTitle, Overview, GenreIds, OriginCountry, OriginalLanguage, ReleaseDate, BackdropPath, PosterPath, Popularity, VoteAverage, VoteCount, Adult, Video) VALUES (@Id, @MediaType, @Title, @OriginalTitle, @Overview, @GenreIds, @OriginCountry, @OriginalLanguage, @ReleaseDate, @BackdropPath, @PosterPath, @Popularity, @VoteAverage, @VoteCount, @Adult, @Video)""";
            if (entry is SearchMovie movie)
            {
                this.connection.Execute(command, new
                {
                    Id = movie.Id,
                    MediaType = "Movie",
                    Title = movie.Title ?? string.Empty,
                    OriginalTitle = movie.OriginalTitle ?? string.Empty,
                    Overview = movie.Overview ?? string.Empty,
                    GenreIds = string.Join(",", movie.GenreIds ?? new List<int>()),
                    OriginCountry = (string?)null,
                    OriginalLanguage = movie.OriginalLanguage ?? string.Empty,
                    ReleaseDate = this.fileHelper.DateTimeToString(movie.ReleaseDate),
                    BackdropPath = movie.BackdropPath ?? string.Empty,
                    PosterPath = movie.PosterPath ?? string.Empty,
                    Popularity = movie.Popularity,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount,
                    Adult = movie.Adult,
                    Video = movie.Video
                });
            }
            if (entry is SearchTv tv)
            {
                this.connection.Execute(command, new
                {
                    Id = tv.Id,
                    MediaType = "Tv",
                    Title = tv.Name ?? string.Empty,
                    OriginalTitle = tv.OriginalName ?? string.Empty,
                    Overview = tv.Overview ?? string.Empty,
                    GenreIds = string.Join(",", tv.GenreIds ?? new List<int>()),
                    OriginCountry = string.Join(",", tv.OriginCountry ?? new List<string>()),
                    OriginalLanguage = tv.OriginalLanguage ?? string.Empty,
                    ReleaseDate = fileHelper.DateTimeToString(tv.FirstAirDate),
                    BackdropPath = tv.BackdropPath ?? string.Empty,
                    PosterPath = tv.PosterPath ?? string.Empty,
                    Popularity = tv.Popularity,
                    VoteAverage = tv.VoteAverage,
                    VoteCount = tv.VoteCount,
                    Adult = (string?)null,
                    Video = (string?)null
                });
            }
        }

        public List<object> SelectAllEntries(string tableName)
        {
            List<object> list = new();
            string command = $"SELECT * FROM {tableName}";
            using SqliteCommand sqliteCommand = this.connection.CreateCommand();
            sqliteCommand.CommandText = command;
            using SqliteDataReader reader = sqliteCommand.ExecuteReader();
            while (reader.Read())
            {
                string mediaType = reader["MediaType"].ToString() ?? "";
                if (mediaType is "Movie")
                {
                    SearchMovie movie = new();
                    movie.Id = Convert.ToInt32(reader["Id"]);
                    movie.Title = reader["Title"].ToString();
                    movie.OriginalTitle = reader["OriginalTitle"].ToString();
                    movie.Overview = reader["Overview"].ToString();
                    movie.GenreIds = databaseHelper.GetIntListFromString(reader["GenreIds"].ToString());
                    movie.OriginalLanguage = reader["OriginalLanguage"].ToString();
                    if (reader["ReleaseDate"] is not DBNull)
                    {
                        movie.ReleaseDate = fileHelper.StringToDataTime(reader["ReleaseDate"].ToString());
                    }
                    movie.BackdropPath = reader["BackdropPath"].ToString();
                    movie.PosterPath = reader["PosterPath"].ToString();
                    movie.Popularity = Convert.ToDouble(reader["Popularity"]);
                    movie.VoteAverage = Convert.ToDouble(reader["VoteAverage"]);
                    movie.VoteCount = Convert.ToInt32(reader["VoteCount"]);
                    if (reader["Adult"] is not DBNull)
                    {
                        movie.Adult = Convert.ToBoolean(reader["Adult"]);
                    }
                    if (reader["Video"] is not DBNull)
                    {
                        movie.Video = Convert.ToBoolean(reader["Video"]);
                    }
                    list.Add(movie);
                }
                if (mediaType is "Tv")
                {
                    SearchTv tv = new();
                    tv.Id = Convert.ToInt32(reader["Id"]);
                    tv.Name = reader["Title"].ToString();
                    tv.OriginalName = reader["OriginalTitle"].ToString();
                    tv.Overview = reader["Overview"].ToString();
                    tv.GenreIds = databaseHelper.GetIntListFromString(reader["GenreIds"].ToString());
                    if (reader["OriginCountry"] is not DBNull)
                    {
                        tv.OriginCountry = databaseHelper.GetStringListFromString(reader["OriginCountry"].ToString());
                    }
                    tv.OriginalLanguage = reader["OriginalLanguage"].ToString();
                    if (reader["ReleaseDate"] is not DBNull)
                    {
                        tv.FirstAirDate = fileHelper.StringToDataTime(reader["ReleaseDate"].ToString());
                    }
                    tv.BackdropPath = reader["BackdropPath"].ToString();
                    tv.PosterPath = reader["PosterPath"].ToString();
                    tv.Popularity = Convert.ToDouble(reader["Popularity"]);
                    tv.VoteAverage = Convert.ToDouble(reader["VoteAverage"]);
                    tv.VoteCount = Convert.ToInt32(reader["VoteCount"]);
                    list.Add(tv);
                }
            }
            return list;
        }
    }
}
