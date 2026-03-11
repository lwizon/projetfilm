using System.Windows.Controls;

namespace ProjetFilmv1.Services;

using System;
using MySql.Data.MySqlClient;

public interface IUserService
{
   
}
public class dbservice :  IUserService
{
   private string connectionString = "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";

   public void getusers(string email, string nom, string password)
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
         Console.WriteLine("Connected");
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
         throw;
      }
      finally{
         connection.Close();
      }
   }
}