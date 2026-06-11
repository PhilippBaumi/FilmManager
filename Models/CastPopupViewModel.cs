using FilmManager.Resources.Strings.Sprachen;
using TMDbLib.Objects.People;

namespace FilmManager.Models
{
    public class CastPopupViewModel
    {
        public string Name { get; set; }
        public string AlsoKnownAs { get; set; }
        public string Birthday { get; set; }
        public string Gender { get; set; }
        public string PlaceOfBirth { get; set; }
        public string KnownForDepartment { get; set; }
        public string Popularity { get; set; }
        public string Biography { get; set; }
        public CastPopupViewModel(Person p)
        {
            this.Name = $"Name: {p.Name}";
            this.AlsoKnownAs=$"Alias: {string.Join(", ", p.AlsoKnownAs)}";
            this.Birthday=$"{AppResources.birthday}: {p.Birthday}";
            this.PlaceOfBirth=$"{AppResources.placeOfBirth}: {p.PlaceOfBirth}";
            this.Gender=$"{AppResources.gender}: {p.Gender}";
            this.Popularity=$"{AppResources.popularity}: {p.Popularity}";
            this.KnownForDepartment=$"{AppResources.knownForDepartment}: {p.KnownForDepartment}";
            this.Biography=$"{AppResources.biography}: {p.Biography}";
        }
    }
}
