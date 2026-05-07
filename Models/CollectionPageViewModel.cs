using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public partial class CollectionPageViewModel
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
        public ObservableCollection<string> Images { get; set; } = new();
        public void SetList(List<SearchCollection> collection)
        {
            foreach (SearchCollection search in collection)
            {
                this.Images.Add($"{ImageBaseUrl}{search.PosterPath}");
            }
        }
    }
}
