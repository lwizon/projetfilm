using System.Windows;
using System.Windows.Controls;
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

            bool isConnected = _dbservice.LoginUser(email, mdp);

            if (isConnected)
            {
                MessageBox.Show("Connexion réussie !");
            }
            else
            {
                MessageBox.Show("Email ou mot de passe incorrect");
            }
        }
    }
}