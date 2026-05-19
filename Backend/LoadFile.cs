using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Resources.Strings.Sprachen;
using OfficeIMO.Word;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using TMDbLib.Objects.Search;

namespace FilmManager.Backend
{
    public class LoadFile : ILoadFile
    {
        private FileHelper fileHelper = new();
        private const string watchedPath = "FilmManager-Watched";
        private const string watchlistPath = "FilmManager-Watchlist";
        private IDatabase database;

        public LoadFile(IDatabase database)
        {
            this.database = database;
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
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            List<string> lines = new();
            ObservableCollection<object> list = new();
            using (WordDocument wordDocument = WordDocument.Load(path))
            {
                List<WordParagraph> paragraphs = wordDocument.Paragraphs;
                foreach (WordParagraph paragraph in paragraphs)
                {
                    string text = paragraph.Text?.Trim() ?? "";
                    if (string.IsNullOrEmpty(text))
                    {
                        continue;
                    }
                    lines.Add(text);
                    if (text.StartsWith("VoteCount"))
                    {
                        if (lines[1].StartsWith("MediaType"))
                        {
                            string[] st = lines[1].Split(";");
                            string s = st[1].Trim();
                            if (s.Equals("Tv"))
                            {
                                list.Add(fileHelper.GetSerieFromDOCX(lines));
                            }
                            if (s.Equals("Movie"))
                            {
                                list.Add(fileHelper.GetMovieFromDOCX(lines));
                            }
                            lines.Clear();
                        }
                    }
                }
            }
            return list;
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
            using (StreamReader sr = new StreamReader(path))
            {
                while (true)
                {
                    string? line = sr.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    Console.WriteLine(line);
                    string[] st = line.Split(";");
                    string type = st[0];
                    if (type.Equals("Movie"))
                    {
                        results.Add(fileHelper.GetMovie(st));
                    }
                    if (type.Equals("Tv"))
                    {
                        results.Add(fileHelper.GetTv(st));
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
