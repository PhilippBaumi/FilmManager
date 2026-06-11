using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FilmManager.Backend;
using System;
using System.Collections.Generic;
using System.Text;
using TMDbLib.Objects.Search;

namespace FilmManager.Helpers
{
    public class GenreHelper
    {
        private TMDBService tMDBService;
        public List<SearchTv> series { get; } = new();
        public List<SearchMovie> movies { get; } = new();

        public GenreHelper(TMDBService tMDBService)
        {
            this.tMDBService = tMDBService;
        }

        public List<string> MovieGenres()
        {
            this.tMDBService.AddMoviesGenresToList();
            return tMDBService.MovieGenresName;
        }

        public List<string> SeriesGenres()
        {
            this.tMDBService.AddSerienGenresToList();
            return tMDBService.SerienGenresName;
        }

        public void GetSerien(List<SearchTv> results)
        {
            foreach (SearchTv search in results)
            {
                if (!series.Contains(search))
                {
                    series.Add(search);
                }
            }
        }

        public void GetMovies(List<SearchMovie> results)
        {
            foreach (SearchMovie search in results)
            {
                if (!movies.Contains(search))
                {
                    movies.Add(search);
                }
            }
        }
    }
}
