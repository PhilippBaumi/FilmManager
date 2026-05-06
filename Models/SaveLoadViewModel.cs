using CommunityToolkit.Mvvm.ComponentModel;

namespace FilmManager.Models
{
    public partial class SaveLoadViewModel : ObservableObject
    {
        [ObservableProperty]
        private string? saveLoadOption;
    }
}
