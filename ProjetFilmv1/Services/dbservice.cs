using System;
using MySql.Data.MySqlClient;
using ProjetFilmv1.Models;

namespace ProjetFilmv1.Services
{
    public interface IUserService
    {
        // On change bool en int pour récupérer l'id_user
        int LoginUser(string email, string password);
        void RegisterUser(string nom, string email, string password);
        bool DeleteUser(string email);
        string? GetUserName(string email);
    }

    public class dbservice : IUserService
    {
        private string connectionString = "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";

        public int LoginUser(string email, string password)
        {
            // Le bloc 'using' ferme la connexion automatiquement à la fin
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    // On demande l'id_user au lieu de simplement compter
                    string query = "SELECT id_user FROM users WHERE email=@email AND password=@password";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();

                    // ExecuteScalar renvoie la première colonne de la première ligne (id_user)
                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        return Convert.ToInt32(result); // Succès : on renvoie l'ID (ex: 27)
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur BDD : " + e.Message);
                }
            }

            return -1; // Échec : on renvoie -1 si l'utilisateur n'est pas trouvé
        }

        public void RegisterUser(string nom, string email, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    string query = "INSERT INTO users (nom, email, password) VALUES (@nom, @email, @password)";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@password", password);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public bool DeleteUser(string email)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                string query = "DELETE FROM users WHERE email=@email";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);

                connection.Open();
                int rows = command.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        public string? GetUserName(string email)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                string query = "SELECT nom FROM users WHERE email=@email LIMIT 1";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);

                connection.Open();
                return command.ExecuteScalar()?.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                connection.Close();
            }
        }
    }
    
    //commentaires
    
}
