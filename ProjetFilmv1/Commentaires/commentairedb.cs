using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ProjetFilmv1.Models;

namespace ProjetFilmv1.Services
{
    public class CommentService
    {
        private readonly string connectionString =
            "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";

        public int GetFilmIdByTitle(string titre)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT id_film
                    FROM `films (temporaire)`
                    WHERE titre = @titre
                    LIMIT 1";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@titre", titre);

                    connection.Open();
                    var result = command.ExecuteScalar();

                    if (result != null)
                        return Convert.ToInt32(result);

                    return 0;
                }
            }
        }

        public int AddFilmIfNotExists(string titre)
        {
            int idFilm = GetFilmIdByTitle(titre);

            if (idFilm != 0)
                return idFilm;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO `films (temporaire)` (titre)
                    VALUES (@titre);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@titre", titre);

                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void AddComment(string titreFilm, int idUser, string contenu)
        {
            int idFilm = AddFilmIfNotExists(titreFilm);

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    INSERT INTO commentaire (id_film, id_user, contenu)
                    VALUES (@idFilm, @idUser, @contenu)";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idFilm", idFilm);
                    command.Parameters.AddWithValue("@idUser", idUser);
                    command.Parameters.AddWithValue("@contenu", contenu);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<MovieComment> GetCommentsByFilmTitle(string titreFilm)
        {
            var comments = new List<MovieComment>();
            int idFilm = GetFilmIdByTitle(titreFilm);

            if (idFilm == 0)
                return comments;

            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        c.id_commentaire,
                        c.id_film,
                        c.id_user,
                        c.contenu,
                        u.nom AS user_name
                    FROM commentaire c
                    INNER JOIN users u ON c.id_user = u.id_user
                    WHERE c.id_film = @idFilm
                    ORDER BY c.id_commentaire DESC";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@idFilm", idFilm);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new MovieComment
                            {
                                IdCommentaire = Convert.ToInt32(reader["id_commentaire"]),
                                IdFilm = Convert.ToInt32(reader["id_film"]),
                                IdUser = Convert.ToInt32(reader["id_user"]),
                                Contenu = reader["contenu"].ToString(),
                                UserName = reader["user_name"].ToString()
                            });
                        }
                    }
                }
            }

            return comments;
        }
    }
}