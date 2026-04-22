using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using ProjetFilmv1; // Ne pas oublier pour MySQL

namespace ProjetFilmV1
{
    public partial class GestionProfile : Page
    {
        private readonly string _connectionString = "Server=172.20.11.6;Port=3306;Database=projetfilm;Uid=admin_bdd;Pwd=rootroot;";
        private readonly string? _displayName;

        public GestionProfile() 
        {
            InitializeComponent();
            ChargerNomUtilisateur();
        }

        public GestionProfile(string displayName) : this()
        {
            _displayName = displayName;

            if (!string.IsNullOrWhiteSpace(displayName) && UserNameText != null)
            {
                UserNameText.Text = displayName;
            }
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

                    if (result != null && UserNameText != null)
                    {
                        UserNameText.Text = result.ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(_displayName) && UserNameText != null)
                    {
                        UserNameText.Text = _displayName;
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

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                var confirm = MessageBox.Show(
                    "Voulez-vous vraiment supprimer votre compte ? Toutes les données seront perdues.",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    if (!mainWindow.DeleteCurrentUser())
                    {
                        MessageBox.Show(
                            "La suppression du compte a échoué.",
                            "Suppression impossible",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
