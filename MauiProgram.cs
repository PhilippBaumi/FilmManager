using CommunityToolkit.Maui;
using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using Mopups.Hosting;
using SkiaSharp.Views.Maui.Controls.Hosting;
using TMDbLib.Client;

namespace FilmManager
{

    public static class MauiProgram
    {
        private const string Tmd_API_Key = "c7108e21486edb11a641d92aa539f3e2";
        public static MauiApp CreateMauiApp()
        {
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<AppPaths>();
            builder.Services.AddSingleton<DatabaseHelper>();
            builder.Services.AddSingleton<FileHelper>();
            builder.Services.AddSingleton(new TMDbClient(Tmd_API_Key));
            builder.Services.AddSingleton<TMDBService>();
            builder.Services.AddSingleton<IDatabase>(serviceProvider =>
            {
                AppPaths paths = serviceProvider.GetRequiredService<AppPaths>();
                //FileHelper fileHelper = new();
                return new Database(paths.GetFilePath("FilmManager.db"));
            });
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<DetailPage>();
            builder.Services.AddTransient<OverviewPage>();
            builder.Services.AddTransient<SaveLoadPage>();
            builder.Services.AddTransient<SettingPage>();
            builder.Services.AddTransient<WatchlistWatchedPage>();
            builder.Services.AddTransient<CollectionPage>();
            builder.Services.AddTransient<CastPage>();
            builder.Services.AddTransient<SearchPage>();
            return builder.Build();
        }
    }
}
