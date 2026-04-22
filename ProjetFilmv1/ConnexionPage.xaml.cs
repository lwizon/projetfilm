using System;
using System.Windows;
using System.Windows.Controls;
using ProjetFilmV1;
using ProjetFilmv1.Services;

namespace ProjetFilmv1
{
    public partial class ConnexionPage : Page
    {
        private readonly dbservice _dbservice;

        public ConnexionPage()
        {
            InitializeComponent();
            _dbservice = new dbservice();
        }

        public ConnexionPage(dbservice dbservice) : this()
        {
            _dbservice = dbservice;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = EmailTextBox.Text;
                string mdp = PasswordBox.Password;

                int userId = _dbservice.LoginUser(email, mdp);

                if (userId != -1)
                {
                    Session.IdUtilisateurConnecte = userId;

                    MessageBox.Show("Connexion reussie !");
                    NavigationService?.Navigate(new GestionInfoProfile());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de la connexion : {ex.Message}",
                    "Connexion impossible",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
