using FilmManager.Backend;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager;

public partial class SaveLoadPage : ContentPage
{
    private SaveLoadViewModel saveLoadViewModel = new();
    private INavigationService navigationService;
    private IDatabase database;

    public SaveLoadPage(INavigationService navigation, IDatabase database)
    {
        InitializeComponent();
        this.navigationService = navigation;
        this.database = database;
        BindingContext = saveLoadViewModel;
    }

    private async void SaveOrLoad(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton { Value: not null } bt)
        {
            string selectedButtonText = bt.Value.ToString();
            switch (selectedButtonText)
            {
                case "CSV": SaveOrLoadCSV(); break;
                case "PDF": SaveOrLoadPDF(); break;
                case "DOCX": SaveOrLoadDOCX(); break;
                case "JSON": SaveOrLoadJSON(); break;
            }
        }
    }

    private async void SaveOrLoadJSON()
    {
        bool result = await DisplayAlertAsync(AppResources.filesOptions, AppResources.messageFiles, AppResources.save, AppResources.load);
        try
        {
            if (result)
            {
                WriteFile writeFile = new(database);
                writeFile.WriteToJSON();
                await DisplayAlertAsync(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}", "OK");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromJSON();
                await DisplayAlertAsync(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void SaveOrLoadDOCX()
    {
        bool result = await DisplayAlertAsync(AppResources.filesOptions, AppResources.messageFiles, AppResources.save, AppResources.load);
        try
        {
            if (result)
            {
                WriteFile writeFile = new(database);
                writeFile.WriteToDOCX();
                await DisplayAlertAsync(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}", "OK");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromDOCX();
                await DisplayAlertAsync(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void SaveOrLoadPDF()
    {
        bool result = await DisplayAlertAsync(AppResources.filesOptions, AppResources.messageFiles, AppResources.save, AppResources.load);
        try
        {
            if (result)
            {
                WriteFile writeFile = new(database);
                writeFile.WriteToPDF();
                await DisplayAlertAsync(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}", "OK");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromPDF();
                await DisplayAlertAsync(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }

    private async void SaveOrLoadCSV()
    {
        bool result = await DisplayAlertAsync(AppResources.filesOptions, AppResources.messageFiles, AppResources.save, AppResources.load);
        try
        {
            if (result)
            {
                WriteFile writeFile = new(database);
                writeFile.WriteToCSV();
                await DisplayAlertAsync(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}", "OK");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromCSV();
                await DisplayAlertAsync(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(AppResources.error, ex.Message, "OK");
        }
    }
}