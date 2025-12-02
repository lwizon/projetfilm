using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ProjetFilmv1.Models;

namespace ProjetFilmv1.Services
{
    public class TmdbService
    {
        private readonly HttpClient _http = new HttpClient();
        private readonly string _apiKey;

        // Clé fournie par l'utilisateur utilisée uniquement en fallback pour les tests.
        // Supprimer ou déplacer en variable d'environnement avant de committer.
        private const string DefaultApiKey = "bf6dc748a3dd2b723ff2d416d0d9dffb";

        public TmdbService()
        {
            _apiKey = Environment.GetEnvironmentVariable("TMDB_API_KEY") ?? DefaultApiKey;
            _http.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        }

        public async Task<TmdbResponse?> GetPopularMoviesResponseAsync(int page = 1)
        {
            var url = $"movie/popular?api_key={_apiKey}&language=fr-FR&page={page}";
            using var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data;
        }

        public async Task<TmdbResponse?> GetMoviesByGenreResponseAsync(int genreId, int page = 1)
        {
            var url = $"discover/movie?api_key={_apiKey}&language=fr-FR&with_genres={genreId}&page={page}";
            using var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data;
        }

        // Backwards-compatible helpers
        public async Task<List<Movie>> GetPopularMoviesAsync(int page = 1)
        {
            var data = await GetPopularMoviesResponseAsync(page);
            return data?.Results ?? new List<Movie>();
        }

        public async Task<List<Movie>> GetMoviesByGenreAsync(int genreId, int page = 1)
        {
            var data = await GetMoviesByGenreResponseAsync(genreId, page);
            return data?.Results ?? new List<Movie>();
        }

        public async Task<List<Genre>> GetGenresAsync()
        {
            var url = $"genre/movie/list?api_key={_apiKey}&language=fr-FR";
            using var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<GenresResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Genres ?? new List<Genre>();
        }
    }
}
