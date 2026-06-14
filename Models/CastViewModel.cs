using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Helpers;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class CastViewModel : ObservableObject
    {
        public ObservableCollection<string> Cast { get; set; } = new();
        [ObservableProperty]
        private string? selectedCast;
        [ObservableProperty]
        private List<SearchPerson> persons;

        public CastViewModel(object o)
        {
            if(o is List<SearchPerson> cast)
            {
                this.persons = cast;
                TMDbHelper tMDbHelper = new TMDbHelper();
                tMDbHelper.SetImages(Cast, cast, person => person.ProfilePath);
            }
        }

        public SearchPerson? GetPerson(string selectedCast)
        {
            if(!string.IsNullOrEmpty(selectedCast))
            {
                foreach (SearchPerson p in this.persons)
                {
                    if(p.ProfilePath.Equals(selectedCast))
                    {
                        return p;
                    }
                }
            }
            return null;
        }
    }
}
