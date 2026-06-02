using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;

namespace FilmManager;

public partial class SaveLoadPage : ContentPage
{
    private SaveLoadViewModel saveLoadViewModel = new();
    private IDatabase database;

    public SaveLoadPage(IDatabase database)
    {
        InitializeComponent();
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
                case "DOCX": SaveOrLoadDOCX(); break;
                case "JSON": SaveOrLoadJSON(); break;
            }
            bt.IsChecked = false;
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
                await AlertHelper.BaseAlert(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromJSON();
                await AlertHelper.BaseAlert(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}");
            }
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
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
                await AlertHelper.BaseAlert(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromDOCX();
                await AlertHelper.BaseAlert(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}");
            }
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
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
                await AlertHelper.BaseAlert(AppResources.saved, $"{AppResources.successfully} {AppResources.saved.ToLower()}");
            }
            else
            {
                LoadFile loadFile = new(database);
                loadFile.LoadFromCSV();
                await AlertHelper.BaseAlert(AppResources.loaded, $"{AppResources.successfully} {AppResources.loaded.ToLower()}");
            }
        }
        catch (Exception ex)
        {
            await AlertHelper.ErrorAlert(ex.Message);
        }
    }
}