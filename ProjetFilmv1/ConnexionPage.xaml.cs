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
            string email = EmailTextBox.Text;
            string mdp = PasswordBox.Password;

            // On récupère l'ID (int) au lieu du booléen
            int userId = _dbservice.LoginUser(email, mdp);

            if (userId != -1) // Si l'ID est différent de -1, c'est que l'utilisateur existe
            {
                // ON REMPLIT LA SESSION AUTOMATIQUEMENT
                Session.IdUtilisateurConnecte = userId;

                MessageBox.Show("Connexion réussie !");

                // Redirection vers la page de profil (ou ton accueil)
                // Elle saura maintenant toute seule quel profil afficher
                NavigationService.Navigate(new GestionInfoProfile());
            }
            else
            {
                MessageBox.Show("Email ou mot de passe incorrect");
            }
        }
    }
}