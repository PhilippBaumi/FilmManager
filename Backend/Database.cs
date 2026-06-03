using Dapper;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Backend
{
    public class Database : IDatabase
    {
        private FileHelper fileHelper = new();
        private DatabaseHelper databaseHelper = new();
        private readonly SqliteConnection connection;
        public Database(string path)
        {
            this.connection = new($"Data Source={path}");
            this.connection.Open();
        }
        public void CreateTable(string tableName)
        {
            this.connection.Execute($"""CREATE TABLE IF NOT EXISTS {tableName}(Id Integer PRIMARY KEY, MediaType Text NOT NULL, Title Text NOT NULL, OriginalTitle Text NOT NULL, Overview Text NOT NULL, GenreIds Text NOT NULL, OriginCountry Text NULL, OriginalLanguage Text NOT NULL, ReleaseDate Text NULL, BackdropPath Text NOT NULL, PosterPath Text NOT NULL, Popularity Real NOT NULL, VoteAverage Real NOT NULL, VoteCount Integer NOT NULL, Adult Integer NULL, Video Integer NULL)""");
        }

        public void DeleteEntry(object entry, string tableName)
        {
            int? getId = databaseHelper.GetId(entry);
            if (getId.HasValue)
            {
                this.connection.Execute($"DELETE FROM {tableName} WHERE Id=@Id", new { Id = getId });
            }
        }

        public void DeleteTable(string tableName)
        {
            this.connection.Execute($"DROP TABLE IF EXISTS {tableName}");
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
                    Title = movie.Title,
                    OriginalTitle = movie.OriginalTitle,
                    Overview = movie.Overview,
                    GenreIds = string.Join(",", movie.GenreIds),
                    OriginCountry = (string?)null,
                    OriginalLanguage = movie.OriginalLanguage,
                    ReleaseDate = this.fileHelper.DateTimeToString(movie.ReleaseDate),
                    BackdropPath = movie.BackdropPath,
                    PosterPath = movie.PosterPath,
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
                    Title = tv.Name,
                    OriginalTitle = tv.OriginalName,
                    Overview = tv.Overview,
                    GenreIds = string.Join(",", tv.GenreIds),
                    OriginCountry = string.Join(",", tv.OriginCountry),
                    OriginalLanguage = tv.OriginalLanguage,
                    ReleaseDate = fileHelper.DateTimeToString(tv.FirstAirDate),
                    BackdropPath = tv.BackdropPath,
                    PosterPath = tv.PosterPath,
                    Popularity = tv.Popularity,
                    VoteAverage = tv.VoteAverage,
                    VoteCount = tv.VoteCount,
                    Adult = (string?)null,
                    Video = (string?)null
                });
            }
        }

        public ObservableCollection<object> SelectAllEntries(string tableName)
        {
            ObservableCollection<object> collection = new();
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
                    if (reader["ReleaseDate"] != DBNull.Value)
                    {
                        movie.ReleaseDate = fileHelper.StringToDataTime(reader["ReleaseDate"].ToString());
                    }
                    movie.BackdropPath = reader["BackdropPath"].ToString();
                    movie.PosterPath = reader["PosterPath"].ToString();
                    movie.Popularity = Convert.ToDouble(reader["Popularity"]);
                    movie.VoteAverage = Convert.ToDouble(reader["VoteAverage"]);
                    movie.VoteCount = Convert.ToInt32(reader["VoteCount"]);
                    if (reader["Adult"] != DBNull.Value)
                    {
                        movie.Adult = Convert.ToBoolean(reader["Adult"]);
                    }
                    if (reader["Video"] != DBNull.Value)
                    {
                        movie.Video = Convert.ToBoolean(reader["Video"]);
                    }
                    collection.Add(movie);
                }
                if (mediaType is "Tv")
                {
                    SearchTv tv = new();
                    tv.Id = Convert.ToInt32(reader["Id"]);
                    tv.Name = reader["Title"].ToString();
                    tv.OriginalName = reader["OriginalTitle"].ToString();
                    tv.Overview = reader["Overview"].ToString();
                    tv.GenreIds = databaseHelper.GetIntListFromString(reader["GenreIds"].ToString());
                    if (reader["OriginCountry"] != DBNull.Value)
                    {
                        tv.OriginCountry = databaseHelper.GetStringListFromString(reader["OriginCountry"].ToString());
                    }
                    tv.OriginalLanguage = reader["OriginalLanguage"].ToString();
                    if (reader["ReleaseDate"] != DBNull.Value)
                    {
                        tv.FirstAirDate = fileHelper.StringToDataTime(reader["ReleaseDate"].ToString());
                    }
                    tv.BackdropPath = reader["BackdropPath"].ToString();
                    tv.PosterPath = reader["PosterPath"].ToString();
                    tv.Popularity = Convert.ToDouble(reader["Popularity"]);
                    tv.VoteAverage = Convert.ToDouble(reader["VoteAverage"]);
                    tv.VoteCount = Convert.ToInt32(reader["VoteCount"]);
                    collection.Add(tv);
                }
            }
            return collection;
        }
    }
}
