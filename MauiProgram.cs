using CommunityToolkit.Maui;
using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using Mopups.Hosting;

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
                .ConfigureMopups()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<INavigationService, MauiNavigationService>();
            builder.Services.AddSingleton<AppShell>();
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
            return builder.Build();
        }
    }
}
