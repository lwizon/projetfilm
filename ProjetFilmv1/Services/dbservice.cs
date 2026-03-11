using System;
using MySql.Data.MySqlClient;

namespace ProjetFilmv1.Services
{
    public interface IUserService
    {
        bool LoginUser(string email, string password);
        void RegisterUser(string nom, string email, string password);
    }

    public class dbservice : IUserService
    {
        private string connectionString = "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";

        // Vérifie si l'utilisateur existe (connexion)
        public bool LoginUser(string email, string password)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                string query = "SELECT COUNT(*) FROM users WHERE email=@email AND password=@password";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                connection.Open();

                int count = Convert.ToInt32(command.ExecuteScalar());

                return count > 0;
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

        // Créer un utilisateur
        public void RegisterUser(string nom, string email, string password)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

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
            finally
            {
                connection.Close();
            }
        }
    }
}