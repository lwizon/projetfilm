using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using ProjetFilmv1; // Ne pas oublier pour MySQL

namespace ProjetFilmV1 
{
    public partial class GestionProfile : Page 
    {
        // Ta chaîne de connexion (la même que d'habitude)
        private string _connectionString = "Server=172.20.11.6;Port=3306;Database=projetfilm;Uid=admin_bdd;Pwd=rootroot;";

        public GestionProfile() 
        {
            InitializeComponent();
            ChargerNomUtilisateur();
        }

        private void ChargerNomUtilisateur()
        {
            try 
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    // On cherche le 'nom' pour l'ID stocké en session
                    string sql = "SELECT nom FROM users WHERE id_user = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", Session.IdUtilisateurConnecte);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        // On envoie le nom au XAML via le DataContext
                        this.DataContext = new { NomAffiche = result.ToString() };
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show("Erreur chargement nom : " + ex.Message);
            }
        }

        private void OpenProfileDetails(object sender, RoutedEventArgs e)
        {
            if (MainContent != null) 
            {
                MainContent.Visibility = Visibility.Collapsed;
            }
            
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new GestionInfoProfile());
        }
    }
}