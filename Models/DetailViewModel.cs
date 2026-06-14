using CommunityToolkit.Mvvm.ComponentModel;
using FilmManager.Helpers;
using FilmManager.Resources.Strings.Sprachen;
using System.Collections.ObjectModel;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace FilmManager.Models
{
    public partial class DetailViewModel : ObservableObject
    {
        public string? Id { get; set; }
        public string? Language { get; set; }
        public string? Overview { get; set; }
        public string? ReleaseDate { get; set; }
        public string? Popularity { get; set; }
        public string? Backport { get; set; }
        public string? AverageVote { get; set; }
        public string? Poster { get; set; }
        public string? Homepage { get; set; }
        public string? CountVote { get; set; }
        public string? OriginCountry { get; set; }
        public string? Networks { get; set; }
        public string? CreatedBy { get; set; }
        public string? Logo { get; set; }
        public string? EpisodsCount { get; set; }
        public string? SeasonsCount { get; set; }

        public string? ProductionCompanies { get; set; }

        public ObservableCollection<string> Logos { get; set; } = new();
        public ObservableCollection<string> Posters { get; set; } = new();
        public ObservableCollection<string> Backports { get; set; } = new();
        public ObservableCollection<string> Cast { get; set; } = new();
        public ObservableCollection<string> Recommendations { get; set; } = new();
        public ObservableCollection<string> Genres { get; set; } = new();

        private List<object> recommentationList = new();


        [ObservableProperty]
        private string? selectedLogo;
        [ObservableProperty]
        private string? selectedPoster;
        [ObservableProperty]
        private string? selectedBackport;
        [ObservableProperty]
        private string? selectedRecommendation;
        [ObservableProperty]
        private string? selectedCast;
        [ObservableProperty]
        private string? selectedGenre;
        public DetailViewModel(object o)
        {
            TMDbHelper tMDbHelper = new TMDbHelper();
            FileHelper fileHelper = new();
            if (o is Movie movie)
            {
                string? logo = GetRandomImage(movie.Images.Logos?.Where(image => string.Equals(image.Iso_3166_1, "US", StringComparison.OrdinalIgnoreCase)).ToList() ?? new());
                if (logo is not null)
                {
                    Logo = tMDbHelper.ToImageUrl(logo);
                }
                Id = $"ID: {movie.Id}";
                Language = $"{AppResources.language}: {movie.OriginalLanguage}";
                string? poster = GetRandomImage(movie.Images.Posters?.Where(image => string.Equals(image.Iso_3166_1, "US", StringComparison.OrdinalIgnoreCase)).ToList() ?? new());
                if (poster is not null)
                {
                    Poster= tMDbHelper.ToImageUrl(poster);
                }
                else
                {
                    Poster = tMDbHelper.ToImageUrl(movie.PosterPath);
                }
                string? backport = GetRandomImage(movie.Images.Backdrops?.Where(image => string.Equals(image.Iso_3166_1, "US", StringComparison.OrdinalIgnoreCase)).ToList() ?? new());
                if (backport is not null)
                {
                    Backport = tMDbHelper.ToImageUrl(backport);
                }
                else
                {
                    Backport = tMDbHelper.ToImageUrl(movie.BackdropPath);
                }
                Overview = $"{AppResources.description}: {movie.Overview}";
                ReleaseDate = $"{AppResources.releaseDate}: {(fileHelper.DateTimeToString(movie.ReleaseDate) ?? string.Empty)}";
                Popularity = $"{AppResources.popularity}: {movie.Popularity}";
                AverageVote = $"{AppResources.averageVote}: {movie.VoteAverage}";
                Homepage = movie.Homepage;
                CountVote = $"{AppResources.countVote}: {movie.VoteCount}";
                AddImages(Logos, movie.Images.Logos);
                AddImages(Posters, movie.Images.Posters);
                AddImages(Backports,  movie.Images.Backdrops);
                AddRecommendations(movie.Recommendations?.Results, m => m.Title);
                AddNames(Genres, movie.Genres, genre => genre.Name);
                SetCast(movie.Credits.Cast);
            }
            if (o is TvShow serie)
            {
                string? logo = GetRandomImage(serie.Images.Logos?.Where(image => string.Equals(image.Iso_3166_1, "US", StringComparison.OrdinalIgnoreCase)).ToList() ?? new());
                if (logo is not null)
                {
                    Logo = tMDbHelper.ToImageUrl(logo);
                }
                Id = "ID: " + serie.Id;
                Language = $"{AppResources.language}: {serie.OriginalLanguage}";
                string? poster = GetRandomImage(serie.Images.Posters?.Where(image => string.Equals(image.Iso_3166_1, "US", StringComparison.OrdinalIgnoreCase)).ToList() ?? new());
                if (poster is not null)
                {
                    Poster = tMDbHelper.ToImageUrl(poster);
                }
                else
                {
                    Poster = tMDbHelper.ToImageUrl(serie.PosterPath);
                }
                string? backport = GetRandomImage(serie.Images.Backdrops?.Where(image => string.Equals(image.Iso_3166_1, "US", StringComparison.OrdinalIgnoreCase)).ToList() ?? new());
                if (backport is not null)
                {
                    Backport = tMDbHelper.ToImageUrl(backport);
                }
                else
                {
                    Backport = tMDbHelper.ToImageUrl(serie.BackdropPath);
                }
                Overview = $"{AppResources.description}: {serie.Overview}";
                ReleaseDate = $"{AppResources.releaseDate}: {(fileHelper.DateTimeToString(serie.FirstAirDate) ?? string.Empty)}";
                Popularity = $"{AppResources.popularity}: {serie.Popularity}";
                AverageVote = $"{AppResources.averageVote}: {serie.VoteAverage}";
                Homepage = serie.Homepage;
                CountVote = $"{AppResources.countVote}: {serie.VoteCount}";
                OriginCountry = $"{AppResources.origionCountry}: {string.Join(",", serie.OriginCountry)}";
                Networks = $"Networks: {JoinNames(serie.Networks, network => network.Name)}";
                CreatedBy = $"{AppResources.createdBy}: {JoinNames(serie.CreatedBy, created => created.Name)}";
                EpisodsCount = $"{AppResources.numberOfEpisodes}: {serie.NumberOfEpisodes}";
                SeasonsCount = $"{AppResources.numberOfSeasons}: {serie.NumberOfSeasons}";
                ProductionCompanies = $"{AppResources.productionCompanies}: {JoinNames(serie.ProductionCompanies, company => company.Name)}";
                AddImages(Logos, serie.Images.Logos);
                AddImages(Posters, serie.Images.Posters);
                AddImages(Backports, serie.Images.Backdrops);
                AddRecommendations(serie.Recommendations?.Results, tv => tv.Name);
                AddNames(Genres, serie.Genres, tv => tv.Name);
                SetCast(serie.Credits.Cast);
            }
        }

        private string JoinNames<T>(IEnumerable<T>? source, Func<T, string?> getName)
        {
            if(source is null)
            {
                return string.Empty;
            }
            else
            {
                return string.Join(", ", source.Select(getName).Where(name=>!string.IsNullOrWhiteSpace(name)));
            }
        }

        private void AddNames<T>(ObservableCollection<string> target, IEnumerable<T>? source, Func<T, string?> getGenre)
        {
            if (source is not null)
            {
                foreach (string? name in source.Select(getGenre).Where(name=>!string.IsNullOrWhiteSpace(name)))
                {
                    target.Add(name!);
                }
            }
        }

        private void AddRecommendations<T>(IEnumerable<T>? recommentations, Func<T, string?> getName)
        {
            if (recommentations != null)
            {
                foreach (T recomment in recommentations)
                {
                    string? name=getName(recomment);
                    if(!string.IsNullOrWhiteSpace(name))
                    {
                        this.Recommendations.Add(name);
                        this.recommentationList.Add(recomment);
                    }
                }
            }
        }

        private void AddImages(ObservableCollection<string> target, IEnumerable<ImageData>? images)
        {
            TMDbHelper tMDbHelper= new TMDbHelper();
            if(images is not null)
            {
                foreach(ImageData image in images)
                {
                    if(!string.IsNullOrEmpty(image.FilePath)&&!string.IsNullOrEmpty(image.Iso_3166_1))
                    {
                        target.Add($"{tMDbHelper.ToImageUrl(image.FilePath)}[{image.Iso_3166_1}]");
                    }
                }
            }
        }

        private void SetCast(object? obj)
        {
            if (obj is not null)
            {
                if (obj is List<TMDbLib.Objects.Movies.Cast> mCastList)
                {
                    foreach (TMDbLib.Objects.Movies.Cast mCast in mCastList)
                    {
                        Cast.Add(mCast.Name);
                    }
                }
                if (obj is List<TMDbLib.Objects.TvShows.Cast> tCastList)
                {
                    foreach (TMDbLib.Objects.TvShows.Cast tCast in tCastList)
                    {
                        Cast.Add(tCast.Name);
                    }
                }
            }
        }

        private string? GetRandomImage(List<ImageData>? images)
        {
            if (images is not null && images.Count is not 0)
            {
                Random random = new();
                int r = random.Next(0, images.Count);
                ImageData image = images[r];
                return image.FilePath;
            }
            return null;
        }

        public List<object> GetList(string selectedRecommentation)
        {
            List<object> list = new();
            foreach (object o in this.recommentationList)
            {
                if (o is SearchMovie movie)
                {
                    if (string.Equals(movie.Title, selectedRecommentation))
                    {
                        list.Add(movie);
                    }
                }
                if (o is SearchTv tv)
                {
                    if (string.Equals(tv.Name, selectedRecommendation))
                    {
                        list.Add(tv);
                    }
                }
            }
            return list;
        }
    }
}