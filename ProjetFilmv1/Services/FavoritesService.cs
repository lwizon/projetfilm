using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProjetFilmv1.Models;

namespace ProjetFilmv1.Services
{
    public class FavoritesService
    {
        private readonly string _connectionString =
            "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";
        private bool _tableVerifiee;

        public List<FavoriteMovieItem> GetFavorites(int userId)
        {
            var favorites = new List<FavoriteMovieItem>();

            if (userId <= 0)
            {
                return favorites;
            }

            using var connection = new MySqlConnection(_connectionString);
            EnsureFavoritesTable(connection);
            const string query = @"
                SELECT id_film_tmdb, titre_film
                FROM films_favoris
                WHERE id_utilisateur = @userId
                ORDER BY date_ajout DESC, id_favori DESC";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                favorites.Add(new FavoriteMovieItem
                {
                    Id = System.Convert.ToInt32(reader["id_film_tmdb"]),
                    Title = reader["titre_film"]?.ToString() ?? "Sans titre"
                });
            }

            return favorites;
        }

        public bool IsFavorite(int userId, int movieId)
        {
            if (userId <= 0 || movieId <= 0)
            {
                return false;
            }

            using var connection = new MySqlConnection(_connectionString);
            EnsureFavoritesTable(connection);
            const string query = @"
                SELECT COUNT(*)
                FROM films_favoris
                WHERE id_utilisateur = @userId AND id_film_tmdb = @movieId";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@movieId", movieId);

            return System.Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        public bool ToggleFavorite(int userId, Movie movie)
        {
            if (userId <= 0 || movie.Id <= 0)
            {
                return false;
            }

            if (IsFavorite(userId, movie.Id))
            {
                RemoveFavorite(userId, movie.Id);
                return false;
            }

            AddFavorite(userId, movie);
            return true;
        }

        private void AddFavorite(int userId, Movie movie)
        {
            using var connection = new MySqlConnection(_connectionString);
            EnsureFavoritesTable(connection);
            const string query = @"
                INSERT INTO films_favoris (id_utilisateur, id_film_tmdb, titre_film)
                VALUES (@userId, @movieId, @title)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@movieId", movie.Id);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@title", movie.Title ?? "Sans titre");

            command.ExecuteNonQuery();
        }

        private void RemoveFavorite(int userId, int movieId)
        {
            using var connection = new MySqlConnection(_connectionString);
            EnsureFavoritesTable(connection);
            const string query = @"
                DELETE FROM films_favoris
                WHERE id_utilisateur = @userId AND id_film_tmdb = @movieId";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@movieId", movieId);

            command.ExecuteNonQuery();
        }

        private void EnsureFavoritesTable(MySqlConnection connection)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }

            if (_tableVerifiee)
            {
                return;
            }

            const string query = @"
                SELECT COUNT(*)
                FROM information_schema.tables
                WHERE table_schema = DATABASE()
                  AND table_name = 'films_favoris';";

            using var command = new MySqlCommand(query, connection);
            var exists = System.Convert.ToInt32(command.ExecuteScalar()) > 0;
            if (!exists)
            {
                throw new System.InvalidOperationException("La table films_favoris est absente de la base de donnees.");
            }

            _tableVerifiee = true;
        }
    }
}
