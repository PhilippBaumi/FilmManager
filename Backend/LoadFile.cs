using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Resources.Strings.Sprachen;
using Sylvan.Data.Csv;
using System.Collections.ObjectModel;
using System.Text.Json;
using TMDbLib.Client;
using TMDbLib.Objects.Search;
using UglyToad.PdfPig;
using Page = UglyToad.PdfPig.Content.Page;

namespace FilmManager.Backend
{
    public class LoadFile : ILoadFile
    {
        private FileHelper fileHelper = new();
        private const string watchedPath = "FilmManager-Watched";
        private const string watchlistPath = "FilmManager-Watchlist";
        private const string apiKey = "c7108e21486edb11a641d92aa539f3e2";
        private TMDbClient client = new(apiKey);
        private TMDbHelper tMDbHelper = new();
        private IDatabase database;

        public LoadFile(IDatabase database)
        {
            this.database = database;
        }

        public void LoadFromPDF()
        {
            ObservableCollection<object> watched = LoadPDF(this.fileHelper.GetFilePath($"{watchedPath}.pdf"));
            this.database.DeleteTable("Watched");
            this.database.CreateTable("Watched");
            Save(watched, "Watched");
            ObservableCollection<object> watchlist = LoadPDF(this.fileHelper.GetFilePath($"{watchlistPath}.pdf"));
            this.database.DeleteTable("Watchlist");
            this.database.CreateTable("Watchlist");
            Save(watchlist, "Watchlist");
        }

        private ObservableCollection<object> LoadPDF(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            ObservableCollection<object> list = new();
            using (PdfDocument pdfDocument = PdfDocument.Open(path))
            {
                foreach(var page in pdfDocument.GetPages())
                {
                    string text=page.Text;
                    string mediaType = fileHelper.GetValue(text, "MediaType");
                    if(mediaType.Equals("Movie"))
                    {
                        SearchMovie movie = new();
                        movie.Id = Int32.Parse(fileHelper.GetValue(text, "ID"));
                        movie.Title = fileHelper.GetValue(text, "Title");
                        movie.OriginalTitle = fileHelper.GetValue(text, "OriginalTitle");
                        movie.OriginalLanguage = fileHelper.GetValue(text, "OriginalLanguage");
                        movie.Overview = fileHelper.GetValue(text, "Overview");
                        movie.GenreIds = fileHelper.GetIntegerList(text, "GenreIds");
                        movie.ReleaseDate = fileHelper.StringToDataTime(fileHelper.GetValue(text, "ReleaseDate"));
                        movie.PosterPath = fileHelper.GetValue(text, "PosterPath");
                        movie.BackdropPath = fileHelper.GetValue(text, "BackdropPath");
                        movie.Popularity = Double.Parse(fileHelper.GetValue(text, "Popularity"));
                        movie.VoteAverage = Double.Parse(fileHelper.GetValue(text, "VoteAverage"));
                        movie.VoteAverage = Int32.Parse(fileHelper.GetValue(text, "VoteCount"));
                        movie.Adult = Boolean.Parse(fileHelper.GetValue(text, "Adult"));
                        movie.Video = Boolean.Parse(fileHelper.GetValue(text, "Video"));
                        list.Add(movie);
                    }
                    if(mediaType.Equals("Tv"))
                    {
                        SearchTv tv = new();
                        tv.Id = Int32.Parse(fileHelper.GetValue(text, "ID"));
                        tv.Name = fileHelper.GetValue(text, "Title");
                        tv.OriginalName = fileHelper.GetValue(text, "OriginalTitle");
                        tv.OriginalLanguage = fileHelper.GetValue(text, "OriginalLanguage");
                        tv.OriginCountry = fileHelper.GetStringList(text, "OriginCountry");
                        tv.Overview = fileHelper.GetValue(text, "Overview");
                        tv.GenreIds = fileHelper.GetIntegerList(text, "GenreIds");
                        tv.FirstAirDate = fileHelper.StringToDataTime(fileHelper.GetValue(text, "ReleaseDate"));
                        tv.PosterPath = fileHelper.GetValue(text, "PosterPath");
                        tv.BackdropPath = fileHelper.GetValue(text, "BackdropPath");
                        tv.Popularity = Double.Parse(fileHelper.GetValue(text, "Popularity"));
                        tv.VoteAverage = Double.Parse(fileHelper.GetValue(text, "VoteAverage"));
                        tv.VoteAverage = Int32.Parse(fileHelper.GetValue(text, "VoteCount"));
                        list.Add(tv);
                    }
                }
            }
            return list;
        }

        public void LoadFromDOCX()
        {
            ObservableCollection<object> watched = LoadDOCX(this.fileHelper.GetFilePath($"{watchedPath}.docx"));
            this.database.DeleteTable("Watched");
            this.database.CreateTable("Watched");
            Save(watched, "Watched");
            ObservableCollection<object> watchlist = LoadDOCX(this.fileHelper.GetFilePath($"{watchlistPath}.docx"));
            this.database.DeleteTable("Watchlist");
            this.database.CreateTable("Watchlist");
            Save(watchlist, "Watchlist");
        }

        private ObservableCollection<object> LoadDOCX(string path)
        {
            throw new NotImplementedException();
        }

        public void LoadFromCSV()
        {
            ObservableCollection<object> watched = LoadCSV(this.fileHelper.GetFilePath($"{watchedPath}.docx"));
            this.database.DeleteTable("Watched");
            this.database.CreateTable("Watched");
            Save(watched, "Watched");
            ObservableCollection<object> watchlist = LoadCSV(this.fileHelper.GetFilePath($"{watchlistPath}.docx"));
            this.database.DeleteTable("Watchlist");
            this.database.CreateTable("Watchlist");
            Save(watchlist, "Watchlist");
        }

        private ObservableCollection<object> LoadCSV(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            ObservableCollection<object> results = new();
            using (CsvDataReader reader = CsvDataReader.Create(path, new CsvDataReaderOptions
            {
                Delimiter = ';'
            }))
            {
                while (reader.Read())
                {
                    string type = reader.GetString(0);
                    string[] parts = new string[reader.FieldCount];
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (reader.IsDBNull(i))
                        {
                            parts[i] = "";
                        }
                        else
                        {
                            parts[i] = reader.GetString(i);
                        }
                    }
                    if (type.Equals("Movie"))
                    {
                        results.Add(fileHelper.GetMovie(parts));
                    }
                    if (type.Equals("Tv"))
                    {
                        results.Add(fileHelper.GetTv(parts));
                    }
                }
            }
            return results;
        }

        public void LoadFromJSON()
        {
            ObservableCollection<object> watched = LoadJSON(this.fileHelper.GetFilePath($"{watchedPath}.json"));
            this.database.DeleteTable("Watched");
            this.database.CreateTable("Watched");
            Save(watched, "Watched");
            ObservableCollection<object> watchlist = LoadJSON(this.fileHelper.GetFilePath($"{watchlistPath}.json"));
            this.database.DeleteTable("Watchlist");
            this.database.CreateTable("Watchlist");
            Save(watchlist, "Watchlist");
        }

        private ObservableCollection<object> LoadJSON(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            ObservableCollection<object> list = new();
            List<string> jsonLines = fileHelper.ReadLines(path);
            foreach (string line in jsonLines)
            {
                using (JsonDocument jsonDocument = JsonDocument.Parse(line))
                {
                    JsonElement root = jsonDocument.RootElement;
                    string? type = root.GetProperty("Type").GetString();
                    if (type != null)
                    {
                        if (type == "Movie")
                        {
                            JsonEntry<SearchMovie>? entry = JsonSerializer.Deserialize<JsonEntry<SearchMovie>>(line);
                            if (entry != null && entry.Data != null)
                            {
                                list.Add(entry.Data);
                            }
                        }
                        if (type == "Tv")
                        {
                            JsonEntry<SearchTv>? entry = JsonSerializer.Deserialize<JsonEntry<SearchTv>>(line);
                            if (entry != null && entry.Data != null)
                            {
                                list.Add(entry.Data);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private void Save(ObservableCollection<object> watched, string tableName)
        {
            foreach (object item in watched)
            {
                this.database.InsertEntry(item, tableName);
            }
        }
    }
}
