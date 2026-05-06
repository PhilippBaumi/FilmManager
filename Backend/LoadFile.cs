using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Resources.Strings.Sprachen;
using Sylvan.Data.Csv;
using System.Collections.ObjectModel;
using System.Text.Json;
using TMDbLib.Client;
using TMDbLib.Objects.Search;

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
            throw new NotImplementedException();
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
            ObservableCollection<object> watched = LoadJSON(this.fileHelper.GetFilePath($"{watchedPath}.docx"));
            this.database.DeleteTable("Watched");
            this.database.CreateTable("Watched");
            Save(watched, "Watched");
            ObservableCollection<object> watchlist = LoadJSON(this.fileHelper.GetFilePath($"{watchlistPath}.docx"));
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
