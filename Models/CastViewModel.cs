using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class CastViewModel : ObservableObject
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
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
                foreach (SearchPerson person in cast)
                {
                    if (!string.IsNullOrEmpty(person.ProfilePath))
                    {
                        this.Cast.Add($"{ImageBaseUrl}{person.ProfilePath}");
                    }
                }
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
