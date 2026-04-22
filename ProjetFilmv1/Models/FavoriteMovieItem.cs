namespace ProjetFilmv1.Models
{
    public class FavoriteMovieItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string? PosterPath { get; set; }
        public double VoteAverage { get; set; }

        public string? PosterFullPath =>
            string.IsNullOrWhiteSpace(PosterPath) ? null : $"https://image.tmdb.org/t/p/w200{PosterPath}";

        public Movie ToMovie()
        {
            return new Movie
            {
                Id = Id,
                Title = Title,
                Overview = Overview,
                PosterPath = PosterPath,
                VoteAverage = VoteAverage
            };
        }
    }
}
