using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using System.Text;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;
using FilmManager.Helpers;

namespace FilmManager.Models
{
    public partial class DetailViewModel : ObservableObject
    {
        private const string ImageBaseUrl = "https://image.tmdb.org/t/p/w500";
        public string? Id { get; set; }
        public string? Language { get; set; }
        public string? Overview { get; set; }
        public string? ReleaseDate { get; set; }
        public string? Popularity { get; set; }
        public string? Backport { get; set; }
        public string? AverageVote { get; set; }
        public string? Poster { get; set; }
        public string? Homepage { get; set; }
        public string? Genres { get; set; }
        public string? CountVote { get; set; }
        public string? OriginCountry { get; set; }
        public string? Networks { get; set; }
        public string? Cast { get; set; } 
        public string? CreatedBy { get; set; }
        public string? Logo { get; set; }
        public string? EpisodsCount { get; set; }
        public string? SeasonsCount { get; set; }

        public string? ProductionCompanies { get; set;  }

        public ObservableCollection<string> Logos { get; set; } = new();
        public ObservableCollection<string> Posters { get; set; } = new();
        public ObservableCollection<string> Backports { get; set; } = new();

        public ObservableCollection<string> Recommendations { get; set; } = new();

        private List<object> recommentationList = new();

        [ObservableProperty]
        private string? selectedLogo;
        [ObservableProperty]
        private string? selectedPoster;
        [ObservableProperty]
        private string? selectedBackport;
        [ObservableProperty]
        private string? selectedRecommendation;
        public DetailViewModel(object o)
        {
            FileHelper fileHelper = new();
            if (o is Movie movie)
            {
                string? logo = GetRandomImage(GetUSList(movie.Images.Logos));
                if (logo != null)
                {
                    Logo = ImageBaseUrl + logo;
                }
                Id = $"ID: {movie.Id}";
                Language = $"{AppResources.language}: {movie.OriginalLanguage}";
                string? poster= GetRandomImage(GetUSList(movie.Images.Posters));
                if (poster != null)
                {
                    Poster = ImageBaseUrl+poster;
                }
                else
                {
                    Poster = ImageBaseUrl + movie.PosterPath;
                }
                string? backport= GetRandomImage(GetUSList(movie.Images.Backdrops));
                if(backport != null)
                {
                    Backport = ImageBaseUrl+backport;
                }
                else
                {
                    Backport = ImageBaseUrl + movie.BackdropPath;
                }
                Overview = $"{AppResources.description}: {movie.Overview}";
                ReleaseDate = $"{AppResources.releaseDate}: {(fileHelper.DateTimeToString(movie.ReleaseDate) ?? string.Empty)}";
                Popularity = $"{AppResources.popularity}: {movie.Popularity}";
                AverageVote = $"{AppResources.averageVote}: {movie.VoteAverage}";
                Homepage = movie.Homepage;
                Genres = $"Genres: {GenresToString(movie.Genres)}";
                CountVote = $"{AppResources.countVote}: {movie.VoteCount}";
                Cast = $"Cast: {GetCast(movie.Credits.Cast)}";
                SetLogos(movie.Images.Logos);
                SetPosters(movie.Images.Posters);
                SetBackdrops(movie.Images.Backdrops);
                SetRecommendations(movie.Recommendations);
            }
            if (o is TvShow serie)
            {
                string? logo = GetRandomImage(GetUSList(serie.Images.Logos));
                if (logo != null)
                {
                    Logo = ImageBaseUrl + logo;
                }
                Id = "ID: " + serie.Id;
                Language = $"{AppResources.language}: {serie.OriginalLanguage}";
                string? poster = GetRandomImage(GetUSList(serie.Images.Posters));
                if (poster != null)
                {
                    Poster = ImageBaseUrl + poster;
                }
                else
                {
                    Poster = ImageBaseUrl + serie.PosterPath;
                }
                string? backport = GetRandomImage(GetUSList(serie.Images.Backdrops));
                if (backport != null)
                {
                    Backport = ImageBaseUrl + backport;
                }
                else
                {
                    Backport = ImageBaseUrl + serie.BackdropPath;
                }
                Overview = $"{AppResources.description}: {serie.Overview}";
                ReleaseDate = $"{AppResources.releaseDate}: {(fileHelper.DateTimeToString(serie.FirstAirDate) ?? string.Empty)}";
                Popularity = $"{AppResources.popularity}: {serie.Popularity}";
                AverageVote = $"{AppResources.averageVote}: {serie.VoteAverage}";
                Homepage = serie.Homepage;
                Genres = $"Genres: {GenresToString(serie.Genres)}";
                CountVote = $"{AppResources.countVote}: {serie.VoteCount}";
                OriginCountry = $"{AppResources.origionCountry}: {string.Join(",", serie.OriginCountry)}";
                Networks = $"Networks: {GetNetworks(serie.Networks)}";
                Cast = $"Cast: {GetCast(serie.Credits.Cast)}";
                CreatedBy = $"{AppResources.createdBy}: {CreatedByToString(serie.CreatedBy)}";
                EpisodsCount = $"{AppResources.numberOfEpisodes}: {serie.NumberOfEpisodes}";
                SeasonsCount = $"{AppResources.numberOfSeasons}: {serie.NumberOfSeasons}";
                ProductionCompanies = $"{AppResources.productionCompanies}: {ProductionCompaniesToString(serie.ProductionCompanies)}";
                SetLogos(serie.Images.Logos);
                SetPosters(serie.Images.Posters);
                SetBackdrops(serie.Images.Backdrops);
                SetRecommendations(serie.Recommendations);
            }
        }

        private void SetRecommendations(SearchContainer<SearchTv>? recommendations)
        {
            List<SearchTv> searchTv = recommendations.Results;
            foreach(SearchTv tv in searchTv)
            {
                Recommendations.Add(tv.Name);
                this.recommentationList.Add(tv);
            }
        }

        private void SetRecommendations(SearchContainer<SearchMovie>? recommendations)
        {
            List<SearchMovie> searchMovie = recommendations.Results;
            foreach (SearchMovie movie in searchMovie)
            {
                Recommendations.Add(movie.Title);
                recommentationList.Add(movie);
            }
        }

        private string ProductionCompaniesToString(List<ProductionCompany>? productionCompanies)
        {
            StringBuilder sb = new();
            foreach (ProductionCompany company in productionCompanies)
            {
                sb.Append(company.Name);
                sb.Append(", ");
            }
            string s = sb.ToString();
            if (s.Length >= 2)
            {
                return s.Substring(0, s.Length - 2);
            }
            return s;
        }

        private string CreatedByToString(List<CreatedBy>? createdBy)
        {
            StringBuilder sb = new();
            foreach(CreatedBy created in createdBy)
            {
                sb.Append(created.Name);
                sb.Append(", ");
            }
            string s = sb.ToString();
            if (s.Length >= 2)
            {
                return s.Substring(0, s.Length - 2);
            }
            return s;
        }

        private void SetBackdrops(List<ImageData>? backdrops)
        {
            foreach (ImageData image in backdrops)
            {
                if (image.FilePath != null && image.Iso_3166_1 != null)
                {
                    Backports.Add($"{ImageBaseUrl}{image.FilePath} [{image.Iso_3166_1}]");
                }
            }
        }

        private void SetPosters(List<ImageData>? posters)
        {
            foreach (ImageData image in posters)
            {
                if(image.FilePath!=null&&image.Iso_3166_1!=null)
                {
                    Posters.Add($"{ImageBaseUrl}{image.FilePath} [{image.Iso_3166_1}]");
                } 
            }
        }

        private void SetLogos(List<ImageData>? logos)
        {
            foreach (ImageData image in logos)
            {
                if (image.FilePath != null && image.Iso_3166_1 != null)
                {
                    Logos.Add($"{ImageBaseUrl}{image.FilePath} [{image.Iso_3166_1}]");
                }
            }
        }

        private List<ImageData>? GetUSList(List<ImageData>? list)
        {
            List<ImageData>? images = new();
            foreach(ImageData image in list)
            {
                if(image!=null)
                {
                    if(image.Iso_3166_1!=null &&image.Iso_3166_1.Equals("US"))
                    {
                        images.Add(image);
                    }
                }
            }
            return images;
        }

        private string? GetRandomImage(List<ImageData>? images)
        {
            if (images != null && images.Count != 0)
            {
                Random random = new();
                int r = random.Next(0, images.Count);
                ImageData image = images[r];
                return image.FilePath;
            }
            return null;
        }

        private string GetCast(object? obj)
        {
            StringBuilder sb = new();
            if(obj!=null)
            {
                if(obj is List<TMDbLib.Objects.Movies.Cast> mCastList)
                {
                    foreach(TMDbLib.Objects.Movies.Cast mCast in mCastList)
                    {
                        sb.Append(mCast.Name);
                        sb.Append(", ");
                    }
                }
                if(obj is List<TMDbLib.Objects.TvShows.Cast> tCastList)
                {
                    foreach(TMDbLib.Objects.TvShows.Cast tCast in tCastList)
                    {
                        sb.Append(tCast.Name);
                        sb.Append(", ");
                    }
                }
            }
            string s = sb.ToString();
            if (s.Length >= 2)
            {
                return s.Substring(0, s.Length - 2);
            }
            return s;
        }

        private string? GetNetworks(List<NetworkWithLogo>? networks)
        {
            StringBuilder sb = new();
            if (networks.Count >= 1)
            {
                foreach (NetworkWithLogo network in networks)
                {
                    sb.Append(network.Name);
                    sb.Append(", ");
                }
            }
            string s = sb.ToString();
            return s.Substring(0, s.Length - 2);
        }

        private string GenresToString(List<Genre>? genres)
        {
            List<string>? names = new();
            foreach (Genre? genre in genres)
            {
                if (!string.IsNullOrEmpty(genre?.Name))
                {
                    names.Add(genre.Name);
                }
            }
            return string.Join(", ", names);
        }

        public List<object> GetList(string selectedRecommentation)
        {
            List<object> list = new();
            foreach(object o in this.recommentationList)
            {
                if(o is SearchMovie movie)
                {
                    if(movie.Title.Equals(selectedRecommentation))
                    {
                        list.Add(movie);
                    }
                }
                if(o is SearchTv tv)
                {
                    if(tv.Name.Equals(selectedRecommendation))
                    {
                        list.Add(tv);
                    }
                }
            }
            return list;
        }
    }
}