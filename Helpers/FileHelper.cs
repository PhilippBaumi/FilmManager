using FilmManager.Resources.Strings.Sprachen;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using TMDbLib.Objects.Search;

namespace FilmManager.Helpers
{
    public class FileHelper
    {
        public string GetFilePath(string s)
        {
            string folderPath;
#if WINDOWS
        folderPath = Path.Combine(AppContext.BaseDirectory, "FilmManager");
#elif ANDROID
            folderPath = Path.Combine(Android.App.Application.Context.FilesDir.AbsolutePath, "FilmManager");
#else
        throw new PlatformNotSupportedException();
#endif

            Directory.CreateDirectory(folderPath);
            return Path.Combine(folderPath, s);
        }

        public void DeleteIfExits(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public string? DateTimeToString(DateTime? releaseDate)
        {
            if (releaseDate != null)
            {
                DateTime date= releaseDate.Value;
                return date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            }
            return null;
        }

        public List<string> ReadLines(string path)
        {
            List<string> list = new();
            using (StreamReader reader = new(path))
            {
                while (true)
                {
                    string? line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    list.Add(line);
                }
            }
            return list;
        }

        public SearchMovie GetMovie(string[] parts)
        {
            SearchMovie m = new();
            m.Id = Int32.Parse(parts[1]);
            m.Title = parts[2];
            m.OriginalTitle = parts[3];
            m.ReleaseDate = DateTime.Parse(parts[4]);
            m.BackdropPath = parts[6];
            m.PosterPath = parts[7];
            m.GenreIds = GetGenreIds(parts[8]);
            m.OriginalLanguage = parts[9];
            m.Overview = parts[10];
            m.VoteCount = Int32.Parse(parts[11]);
            m.VoteAverage = Double.Parse(parts[12]);
            m.Popularity = Double.Parse(parts[13]);
            m.Adult = Boolean.Parse(parts[14]);
            m.Video = Boolean.Parse(parts[15]);
            return m;
        }

        public SearchTv GetTv(string[] parts)
        {
            SearchTv tv = new();
            tv.Id = Int32.Parse(parts[1]);
            tv.Name = parts[2];
            tv.OriginalName = parts[3];
            tv.FirstAirDate = DateTime.Parse(parts[4]);
            tv.OriginCountry = GetCountries(parts[5]);
            tv.BackdropPath = parts[6];
            tv.PosterPath = parts[7];
            tv.GenreIds = GetGenreIds(parts[8]);
            tv.OriginalLanguage = parts[9];
            tv.Overview = parts[10];
            tv.VoteCount = Int32.Parse(parts[11]);
            tv.VoteAverage = Double.Parse(parts[12]);
            tv.Popularity = Double.Parse(parts[13]);
            return tv;
        }

        private List<int>? GetGenreIds(string s)
        {
            List<int>? ids = new();
            string[] spt = s.Split(",");
            foreach (string st in spt)
            {
                ids.Add(Int32.Parse(st));
            }
            return ids;
        }

        private List<string>? GetCountries(string s)
        {
            List<string>? strings = new();
            string[] sp = s.Split(",");
            foreach (string st in sp)
            {
                strings.Add(st);
            }
            return strings;
        }

        public string ConvertToCsv(string? s)
        {
            if (s == null)
            {
                return "";
            }
            bool mustQuote = s.Contains(";") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r");
            s = s.Replace("\"", "\"\"");
            if (mustQuote)
            {
                string st = $"\"{s}\"";
                return st;
            }
            return s;
        }

        public DateTime? StringToDataTime(string? s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                DateTime dateTime = DateTime.ParseExact(s, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                return dateTime;
            }
            return null;
        }

        public string GetValue(string value, string key)
        {
            string? val = TryGetValue(value, key);
            if (string.IsNullOrEmpty(val))
            {
                throw new Exception(AppResources.cantGetValue);
            }
            return val;
        }

        private string? TryGetValue(string value, string key)
        {
            string pattern = $@"{key}:\s*(.*?)(?=\s*(ID|MediaType|Title|OriginalTitle|OriginalLanguage|OriginCountry|Overview|GenreIds|ReleaseDate|PosterPath|BackdropPath|Popularity|VoteAverage|VoteCount|Adult|Video):|$)";
            string s = Regex.Match(value, pattern, RegexOptions.Singleline).Groups[1].Value.Trim();
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            return s;
        }

        public List<int> GetIntegerList(string text, string v)
        {
            List<int> list = new();
            string s = GetValue(text, v);
            string[] st = s.Split(",");
            foreach (string sT in st)
            {
                list.Add(Int32.Parse(sT));
            }
            return list;
        }

        public List<string>? GetStringList(string text, string v)
        {
            List<string> list = new();
            string s = GetValue(text, v);
            string[] st = s.Split(",");
            foreach (string sT in st)
            {
                list.Add(sT);
            }
            return list;
        }

        public SearchTv GetSerieFromDOCX(List<string> lines)
        {
            SearchTv tv = new();
            tv.Id = Int32.Parse(GetValueFromString(lines[0]));
            tv.Name = GetValueFromString(lines[2]);
            tv.OriginalName = GetValueFromString(lines[3]);
            tv.Overview = GetValueFromString(lines[4]);
            tv.OriginalLanguage = GetValueFromString(lines[5]);
            tv.OriginCountry = GetStringListFromString(GetValueFromString(lines[6]));
            tv.GenreIds = GetIntegerListFromString(GetValueFromString(lines[7]));
            tv.FirstAirDate = StringToDataTime(GetValueFromString(lines[8]));
            tv.PosterPath = GetValueFromString(lines[9]);
            tv.BackdropPath = GetValueFromString(lines[10]);
            tv.Popularity = Double.Parse(GetValueFromString(lines[11]));
            tv.VoteAverage = Double.Parse(GetValueFromString(lines[12]));
            tv.VoteCount = Int32.Parse(GetValueFromString(lines[13]));
            return tv;
        }

        private List<string>? GetStringListFromString(string s)
        {
            string[] st = s.Split(",");
            List<string> list = new();
            foreach (string s2 in st)
            {
                list.Add(s2);
            }
            return list;
        }

        public object GetMovieFromDOCX(List<string> lines)
        {
            SearchMovie movie = new();
            movie.Id = Int32.Parse(GetValueFromString(lines[0]));
            movie.Title = GetValueFromString(lines[2]);
            movie.OriginalTitle = GetValueFromString(lines[3]);
            movie.Overview = GetValueFromString(lines[4]);
            movie.OriginalLanguage = GetValueFromString(lines[5]);
            movie.GenreIds = GetIntegerListFromString(GetValueFromString(lines[6]));
            movie.ReleaseDate = StringToDataTime(GetValueFromString(lines[7]));
            movie.PosterPath = GetValueFromString(lines[8]);
            movie.BackdropPath = GetValueFromString(lines[9]);
            movie.Adult = Boolean.Parse(GetValueFromString(lines[10]));
            movie.Video = Boolean.Parse(GetValueFromString(lines[11]));
            movie.Popularity = Double.Parse(GetValueFromString(lines[12]));
            movie.VoteAverage = Double.Parse(GetValueFromString(lines[13]));
            movie.VoteCount = Int32.Parse(GetValueFromString(lines[14]));
            return movie;
        }

        private List<int>? GetIntegerListFromString(string s)
        {
            List<int> list = new();
            string[] st = s.Split(",");
            foreach (string str in st)
            {
                list.Add(Int32.Parse(str));
            }
            return list;
        }

        private string GetValueFromString(string s)
        {
            string[] st = s.Split("; ");
            return st[1].Trim();
        }
    }
}
