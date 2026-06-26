using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Resources.Strings.Sprachen;
using OfficeIMO.Word;
using System.Text.Json;
using TMDbLib.Objects.Search;

namespace FilmManager.Backend
{
    public class LoadFile : ILoadFile
    {
        private FileHelper fileHelper = new();
        private IDatabase database;
        private readonly string validateWatched = MediaTableNames.Validate("Watched");
        private readonly string validateWatchlist = MediaTableNames.Validate("Watchlist");
        private readonly string basePathWatchlist = "FilmManager-Watchlist";
        private readonly string basePathWatched = "FilmManager-Watched";

        public LoadFile(IDatabase database)
        {
            this.database = database;
        }

        public void LoadFromDOCX()
        {
            List<object> watched = LoadDOCX(this.fileHelper.GetFilePath($"{basePathWatched}.docx"));
            this.database.DeleteTable(this.validateWatched);
            this.database.CreateTable(this.validateWatched);
            Save(watched, this.validateWatched);
            List<object> watchlist = LoadDOCX(this.fileHelper.GetFilePath($"{basePathWatchlist}.docx"));
            this.database.DeleteTable(this.validateWatchlist);
            this.database.CreateTable(this.validateWatchlist);
            Save(watchlist, this.validateWatchlist);
        }

        private List<object> LoadDOCX(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            List<string> lines = new();
            List<object> list = new();
            using (WordDocument wordDocument = WordDocument.Load(path))
            {
                List<WordParagraph> paragraphs = wordDocument.Paragraphs;
                foreach (WordParagraph paragraph in paragraphs)
                {
                    string? text = paragraph.Text?.Trim() ?? "";
                    if (string.IsNullOrWhiteSpace(text))
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
                            if (string.Equals(s, "Tv"))
                            {
                                list.Add(fileHelper.GetSerieFromDOCX(lines));
                            }
                            if (string.Equals(s, "Movie"))
                            {
                                list.Add(fileHelper.GetMovieFromDOCX(lines));
                            }
                        }
                        lines.Clear();
                    }
                }
            }
            return list;
        }

        public void LoadFromCSV()
        {
            List<object> watched = LoadCSV(this.fileHelper.GetFilePath($"{basePathWatched}.csv"));
            this.database.DeleteTable(this.validateWatched);
            this.database.CreateTable(this.validateWatched);
            Save(watched, this.validateWatched);
            List<object> watchlist = LoadCSV(this.fileHelper.GetFilePath($"{basePathWatchlist}.csv"));
            this.database.DeleteTable(this.validateWatchlist);
            this.database.CreateTable(this.validateWatchlist);
            Save(watchlist, this.validateWatchlist);
        }

        private List<object> LoadCSV(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            List<object> results = new();
            using (StreamReader sr = new StreamReader(path))
            {
                while (true)
                {
                    string? line = sr.ReadLine();
                    if (line is null)
                    {
                        break;
                    }
                    string[] st = line.Split(";");
                    string type = st[0];
                    if (string.Equals(type, "Movie"))
                    {
                        results.Add(fileHelper.GetMovie(st));
                    }
                    if (string.Equals(type, "Tv"))
                    {
                        results.Add(fileHelper.GetTv(st));
                    }
                }
            }
            return results;
        }

        public void LoadFromJSON()
        {
            List<object> watched = LoadJSON(this.fileHelper.GetFilePath($"{basePathWatched}.json"));
            this.database.DeleteTable(this.validateWatched);
            this.database.CreateTable(this.validateWatched);
            Save(watched, this.validateWatched);
            List<object> watchlist = LoadJSON(this.fileHelper.GetFilePath($"{basePathWatchlist}.json"));
            this.database.DeleteTable(this.validateWatchlist);
            this.database.CreateTable(this.validateWatchlist);
            Save(watchlist, this.validateWatchlist);
        }

        private List<object> LoadJSON(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"{AppResources.file} {path} {AppResources.doesNotExists}!");
            }
            List<object> list = new();
            List<string> jsonLines = fileHelper.ReadLines(path);
            foreach (string line in jsonLines)
            {
                using (JsonDocument jsonDocument = JsonDocument.Parse(line))
                {
                    JsonElement root = jsonDocument.RootElement;
                    string? type = root.GetProperty("Type").GetString();
                    if (type is not null)
                    {
                        if (string.Equals(type, "Movie"))
                        {
                            JsonEntry<SearchMovie>? entry = JsonSerializer.Deserialize<JsonEntry<SearchMovie>>(line);
                            if (entry is not null && entry.Data is not null)
                            {
                                list.Add(entry.Data);
                            }
                        }
                        if (string.Equals(type, "Tv"))
                        {
                            JsonEntry<SearchTv>? entry = JsonSerializer.Deserialize<JsonEntry<SearchTv>>(line);
                            if (entry is not null && entry.Data is not null)
                            {
                                list.Add(entry.Data);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private void Save(List<object> watched, string tableName)
        {
            foreach (object item in watched)
            {
                this.database.InsertEntry(item, tableName);
            }
        }
    }
}
