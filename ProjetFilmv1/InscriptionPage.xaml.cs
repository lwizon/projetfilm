using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1.Services;

namespace ProjetFilmv1
{
    public partial class InscriptionPage : Page
    {
        private readonly dbservice _dbservice;

        public InscriptionPage()
        {
            InitializeComponent();
            _dbservice = new dbservice();
        }

        public InscriptionPage(dbservice dbservice) : this()
        {
            _dbservice = dbservice;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string nom = NomTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(nom) || 
                string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs");
                return;
            }

            _dbservice.RegisterUser(nom, email, password);

            MessageBox.Show("Utilisateur créé avec succès !");
        }
    }
}