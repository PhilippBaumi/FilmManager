using System.Text;
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

        public string DateTimeToString(DateTime? releaseDate)
        {
            StringBuilder sb = new();
            if (releaseDate.HasValue)
            {
                sb.Append(releaseDate.Value.Day);
                sb.Append(".");
                sb.Append(releaseDate.Value.Month);
                sb.Append(".");
                sb.Append(releaseDate.Value.Year);
            }
            return sb.ToString();
        }

        public List<string> ReadLines(string path)
        {
            List<string> list = new();
            using (StreamReader reader = new(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
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

        public string ConvertToCsv(string s)
        {
            if (s == null)
            {
                return "";
            }
            bool mustQuote = s.Contains(";") || s.Contains("\"") || s.Contains("\n") || s.Contains("\r");
            s = s.Replace("\"", "\"\"");
            if (mustQuote)
            {
                return $"\"{s}\"";
            }
            return s;
        }

        public DateTime? StringToDataTime(string? s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                DateTime dateTime = DateTime.Parse(s);
                return dateTime;
            }
            return null;
        }
    }
}
