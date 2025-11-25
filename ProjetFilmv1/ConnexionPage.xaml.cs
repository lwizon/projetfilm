using System.Windows;
using System.Windows.Controls;

namespace ProjetFilmv1
{
    public partial class ConnexionPage : Page
    {
        public ConnexionPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string mdp = PasswordBox.Password;

            MessageBox.Show($"Connexion : {email}\nMot de passe : {new string('*', mdp.Length)}");
            // TODO: authentification réelle
        }
    }
}
