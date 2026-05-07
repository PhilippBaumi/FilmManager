using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TMDbLib.Objects.Search;

namespace FilmManager.Models
{
    public class SearchPageViewModel
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
        public ObservableCollection<string> Images { get; set; } = new();
        public void SetList(object list)
        {
            if(list is List<SearchMovie>movies)
            {
                foreach (SearchMovie movie in movies)
                {
                    this.Images.Add($"{ImageBaseUrl}{movie.PosterPath}");
                }
            }
            if (list is List<SearchTv> tv)
            {
                foreach(SearchTv searchTv in tv)
                {
                    this.Images.Add($"{ImageBaseUrl}{searchTv.PosterPath}");
                }
            }
        }
    }
}
