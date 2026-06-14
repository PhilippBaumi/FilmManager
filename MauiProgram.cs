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
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
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
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton(new TMDbClient("c7108e21486edb11a641d92aa539f3e2"));
            builder.Services.AddSingleton<TMDBService>();
            builder.Services.AddSingleton<IDatabase>(serviceProvider =>
            {
                FileHelper fileHelper = new();
                return new Database(fileHelper.GetFilePath("FilmManager.db"));
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
