using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class CollectionPageViewModel : ObservableObject
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
        public ObservableCollection<string> Images { get; set; } = new();

        private List<SearchCollection?> collection;

        [ObservableProperty]
        private string selectedItem;
        public void SetList(List<SearchCollection?> collection)
        {
            this.collection = collection;
            this.Images.Clear();
            foreach (SearchCollection search in collection)
            {
                this.Images.Add($"{ImageBaseUrl}{search.PosterPath}");
            }
        }

        public SearchCollection GetSearchCollection(string selectedItem)
        {
            SearchCollection search = new();
            foreach (SearchCollection sear in this.collection)
            {
                string? posterPath = sear.PosterPath;
                if (!string.IsNullOrEmpty(posterPath))
                {
                    if (posterPath.Equals(selectedItem))
                    {
                        search = sear;
                        break;
                    }
                }
            }
            return search;
        }
    }
}
