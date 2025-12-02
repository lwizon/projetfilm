using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjetFilmv1.Models
{
    public class TmdbResponse
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("results")]
        public List<Movie> Results { get; set; } = new List<Movie>();

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_results")]
        public int TotalResults { get; set; }
    }

    public class Movie
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("overview")]
        public string? Overview { get; set; }

        [JsonPropertyName("poster_path")]
        public string? PosterPath { get; set; }

        [JsonPropertyName("vote_average")]
        public double VoteAverage { get; set; }

        // URL complet pour l'affiche (taille w200)
        public string? PosterFullPath => string.IsNullOrEmpty(PosterPath) ? null : $"https://image.tmdb.org/t/p/w200{PosterPath}";
    }

    public class Genre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class GenresResponse
    {
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new List<Genre>();
    }
}
