using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager.Helpers
{
    public sealed class AppPaths
    {
        public string GetFilePath(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException(AppResources.filePathEmpty, nameof(s));
            }
            string folderPath;
#if WINDOWS
            folderPath = Path.Combine(AppContext.BaseDirectory, "FilmManager");
#elif ANDROID
            folderPath = Path.Combine(Android.App.Application.Context.FilesDir.AbsolutePath, "FilmManager");
#else
            folderPath=Path.Combine(FileSystem.AppDataDirectory, "FilmManager");
#endif
            Directory.CreateDirectory(folderPath);
            return Path.Combine(folderPath, s);
        }
    }
}
