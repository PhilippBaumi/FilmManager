using FilmManager.Backend;
using FilmManager.Helpers;
using FilmManager.Interfaces;
using FilmManager.Models;
using FilmManager.Resources.Strings.Sprachen;
using MarketAlly.Dialogs.Maui.Dialogs;
using MarketAlly.Dialogs.Maui.Models;
using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace FilmManager;

public partial class SaveLoadPage : ContentPage
{
    private SaveLoadViewModel saveLoadViewModel = new();
    private IDatabase database;
    private SemaphoreSlim isDialogShown = new(1, 1);

    public SaveLoadPage(IDatabase database)
    {
        InitializeComponent();
        this.database = database;
        BindingContext = saveLoadViewModel;
    }

    private void SaveOrLoad(object sender, CheckedChangedEventArgs e)
    {
        bool checkedArgs=e.Value;
        if (sender is RadioButton { Value: not null } bt && checkedArgs)
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
        try
        {
            bool result = await ShowDialog();
            if (result)
            {
                WriteFile writeFile = new(database);
                await LoadingDialog.ShowAsync(AppResources.saving, async () => writeFile.WriteToJSON());
                await Toast.ShowAsync(AppResources.saved, DialogType.Success);
            }
            else
            {
                LoadFile loadFile = new(database);
                await LoadingDialog.ShowAsync(AppResources.loading, async () => loadFile.LoadFromJSON());
                await Toast.ShowAsync(AppResources.loaded, DialogType.Success);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void SaveOrLoadDOCX()
    {
        try
        {
            bool result = await ShowDialog();
            if (result)
            {
                WriteFile writeFile = new(database);
                await LoadingDialog.ShowAsync(AppResources.saving, async () => writeFile.WriteToDOCX());
                await Toast.ShowAsync(AppResources.saved, DialogType.Success);
            }
            else
            {
                LoadFile loadFile = new(database);
                await LoadingDialog.ShowAsync(AppResources.loading, async () => loadFile.LoadFromDOCX());
                await Toast.ShowAsync(AppResources.loaded, DialogType.Success);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async void SaveOrLoadCSV()
    {
        try
        {
            bool result = await ShowDialog();
            if (result)
            {
                WriteFile writeFile = new(database);
                await LoadingDialog.ShowAsync(AppResources.saving, async () => writeFile.WriteToCSV());
                await Toast.ShowAsync(AppResources.saved, DialogType.Success);
            }
            else
            {
                LoadFile loadFile = new(database);
                await LoadingDialog.ShowAsync(AppResources.loading, async () => loadFile.LoadFromCSV());
                await Toast.ShowAsync(AppResources.loaded, DialogType.Success);
            }
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
        }
    }

    private async Task<bool> ShowDialog()
    {
        if (!await isDialogShown.WaitAsync(0))
        {
            return false;
        }
        try
        {
            ConfirmDialog dialog = new(AppResources.filesOptions, AppResources.messageFiles, AppResources.save, AppResources.load);
            return await dialog.ShowAsync();
        }
        catch (Exception ex)
        {
            await Toast.ShowAsync(ex.Message, DialogType.Error);
            return false;
        }
        finally
        {
            isDialogShown.Release();
        }
    }

    private void HandlePaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        SKCanvas canvas = e.Surface.Canvas;
        SKImageInfo info = e.Info;
        canvas.Clear(SKColors.Transparent);
        SKiaDrawHelper.DrawHeader(canvas, info, $"  {AppResources.saveLoad}");
    }
}