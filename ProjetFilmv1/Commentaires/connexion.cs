using System;
using MySql.Data.MySqlClient;
using ProjetFilmv1.Models;

namespace ProjetFilmv1.Services
{
    public class LoginService
    {
        private readonly string connectionString =
            "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";

        public User LoginUser(string email, string password)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string query = @"
                    SELECT id_user, nom, email
                    FROM users
                    WHERE email = @email AND password = @password
                    LIMIT 1";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new User
                            {
                                IdUser = Convert.ToInt32(reader["id_user"]),
                                Nom = reader["nom"].ToString(),
                                Email = reader["email"].ToString()
                            };

                            SessionManager.CurrentUser = user;
                            return user;
                        }
                    }
                }
            }

            return null;
        }
    }
}