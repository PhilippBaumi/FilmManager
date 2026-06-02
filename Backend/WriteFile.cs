using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Resources.Strings.Sprachen;
using OfficeIMO.Word;
using System.Collections.ObjectModel;
using System.Text.Json;
using TMDbLib.Objects.Search;

namespace FilmManager.Backend
{
    public class WriteFile : IWriteFile
    {
        private FileHelper fileHelper = new();
        private const string watchedPath = "FilmManager-Watched";
        private const string watchlistPath = "FilmManager-Watchlist";
        private IDatabase database;

        public WriteFile(IDatabase database)
        {
            this.database = database;
        }

        public void WriteToCSV()
        {
            WriteCSV(this.fileHelper.GetFilePath($"{watchedPath}.csv"), this.database.SelectAllEntries("Watched"));
            WriteCSV(this.fileHelper.GetFilePath($"{watchlistPath}.csv"), this.database.SelectAllEntries("Watchlist"));
        }
        private void WriteCSV(string path, ObservableCollection<object> collection)
        {
            fileHelper.DeleteIfExits(path);
            using (StreamWriter streamWriter = new(path))
            {
                streamWriter.WriteLine($"{AppResources.type};ID;{AppResources.title};{AppResources.originalTitle};{AppResources.releaseDate};{AppResources.origionCountry};BackdropPath;PosterPath;GenreIDs;{AppResources.originalLanguage};{AppResources.overview};{AppResources.countVote};{AppResources.averageVote};{AppResources.popularity};{AppResources.forAdults};Video");
                foreach (object o in collection)
                {
                    if (o is SearchTv tv)
                    {
                        streamWriter.WriteLine(string.Join(";", fileHelper.ConvertToCsv(tv.MediaType.ToString()), fileHelper.ConvertToCsv(tv.Id.ToString()), fileHelper.ConvertToCsv(tv.Name), fileHelper.ConvertToCsv(tv.OriginalName), fileHelper.ConvertToCsv(fileHelper.DateTimeToString(tv.FirstAirDate)), fileHelper.ConvertToCsv(string.Join(",", tv.OriginCountry)), fileHelper.ConvertToCsv(tv.BackdropPath), fileHelper.ConvertToCsv(tv.PosterPath), fileHelper.ConvertToCsv(string.Join(",", tv.GenreIds)), fileHelper.ConvertToCsv(tv.OriginalLanguage), fileHelper.ConvertToCsv(tv.Overview), fileHelper.ConvertToCsv(tv.VoteCount.ToString()), fileHelper.ConvertToCsv(tv.VoteAverage.ToString()), fileHelper.ConvertToCsv(tv.Popularity.ToString()), fileHelper.ConvertToCsv(""), fileHelper.ConvertToCsv("")));
                    }
                    if (o is SearchMovie m)
                    {
                        streamWriter.WriteLine(string.Join(";", fileHelper.ConvertToCsv(m.MediaType.ToString()), fileHelper.ConvertToCsv(m.Id.ToString()), fileHelper.ConvertToCsv(m.Title), fileHelper.ConvertToCsv(m.OriginalTitle), fileHelper.ConvertToCsv(fileHelper.DateTimeToString(m.ReleaseDate)), fileHelper.ConvertToCsv(""), fileHelper.ConvertToCsv(m.BackdropPath), fileHelper.ConvertToCsv(m.PosterPath), fileHelper.ConvertToCsv(string.Join(",", m.GenreIds)), fileHelper.ConvertToCsv(m.OriginalLanguage), fileHelper.ConvertToCsv(m.Overview), fileHelper.ConvertToCsv(m.VoteCount.ToString()), fileHelper.ConvertToCsv(m.VoteAverage.ToString()), fileHelper.ConvertToCsv(m.Popularity.ToString()), fileHelper.ConvertToCsv(m.Adult.ToString()), fileHelper.ConvertToCsv(m.Video.ToString())));
                    }
                }
            }
        }

        public void WriteToJSON()
        {
            WriteJSON(this.fileHelper.GetFilePath($"{watchedPath}.json"), this.database.SelectAllEntries("Watched"));
            WriteJSON(this.fileHelper.GetFilePath($"{watchlistPath}.json"), this.database.SelectAllEntries("Watchlist"));
        }

        private void WriteJSON(string path, ObservableCollection<object> collection)
        {
            fileHelper.DeleteIfExits(path);
            using (StreamWriter streamWriter = new(path))
            {
                foreach (object o in collection)
                {
                    if (o is SearchMovie m)
                    {
                        JsonEntry<SearchMovie> entry = new JsonEntry<SearchMovie>
                        {
                            Type = "Movie",
                            Data = m
                        };
                        streamWriter.WriteLine(JsonSerializer.Serialize(entry));
                    }
                    if (o is SearchTv s)
                    {
                        JsonEntry<SearchTv> entry = new JsonEntry<SearchTv>
                        {
                            Type = "Tv",
                            Data = s
                        };
                        streamWriter.WriteLine(JsonSerializer.Serialize(entry));
                    }
                }
            }
        }

        public void WriteToDOCX()
        {
            WriteDOCX(this.fileHelper.GetFilePath($"{watchedPath}.docx"), this.database.SelectAllEntries("Watched"), watchedPath);
            WriteDOCX(this.fileHelper.GetFilePath($"{watchlistPath}.docx"), this.database.SelectAllEntries("Watchlist"), watchlistPath);
        }

        private void WriteDOCX(string path, ObservableCollection<object> collection, string basePath)
        {
            fileHelper.DeleteIfExits(path);
            using (WordDocument wordDocument = WordDocument.Create(path))
            {
                foreach (object obj in collection)
                {
                    if (obj is SearchTv tv)
                    {
                        wordDocument.AddParagraph($"ID; {tv.Id}");
                        wordDocument.AddParagraph($"MediaType; {tv.MediaType}");
                        wordDocument.AddParagraph($"Title; {tv.Name}");
                        wordDocument.AddParagraph($"OriginalTitle; {tv.OriginalName}");
                        wordDocument.AddParagraph($"Overview; {tv.Overview}");
                        wordDocument.AddParagraph($"OriginalLanguage; {tv.OriginalLanguage}");
                        wordDocument.AddParagraph($"OriginCountry; {string.Join(",", tv.OriginCountry)}");
                        wordDocument.AddParagraph($"GenreIds; {string.Join(",", tv.GenreIds)}");
                        wordDocument.AddParagraph($"ReleaseDate; {fileHelper.DateTimeToString(tv.FirstAirDate)}");
                        wordDocument.AddParagraph($"PosterPath; {tv.PosterPath}");
                        wordDocument.AddParagraph($"BackdropPath; {tv.BackdropPath}");
                        wordDocument.AddParagraph($"Popularity; {tv.Popularity}");
                        wordDocument.AddParagraph($"VoteAverage; {tv.VoteAverage}");
                        wordDocument.AddParagraph($"VoteCount; {tv.VoteCount}");

                    }
                    if (obj is SearchMovie movie)
                    {
                        wordDocument.AddParagraph($"ID; {movie.Id}");
                        wordDocument.AddParagraph($"MediaType; {movie.MediaType}");
                        wordDocument.AddParagraph($"Title; {movie.Title}");
                        wordDocument.AddParagraph($"OriginalTitle; {movie.OriginalTitle}");
                        wordDocument.AddParagraph($"Overview; {movie.Overview}");
                        wordDocument.AddParagraph($"OriginalLanguage; {movie.OriginalLanguage}");
                        wordDocument.AddParagraph($"GenreIds; {string.Join(",", movie.GenreIds)}");
                        wordDocument.AddParagraph($"ReleaseDate; {fileHelper.DateTimeToString(movie.ReleaseDate)}");
                        wordDocument.AddParagraph($"PosterPath; {movie.PosterPath}");
                        wordDocument.AddParagraph($"BackdropPath; {movie.BackdropPath}");
                        wordDocument.AddParagraph($"Adult; {movie.Adult}");
                        wordDocument.AddParagraph($"Video; {movie.Video}");
                        wordDocument.AddParagraph($"Popularity; {movie.Popularity}");
                        wordDocument.AddParagraph($"VoteAverage; {movie.VoteAverage}");
                        wordDocument.AddParagraph($"VoteCount; {movie.VoteCount}");
                    }
                    wordDocument.AddPageBreak();
                }
                wordDocument.Save();
            }
        }
    }
}
