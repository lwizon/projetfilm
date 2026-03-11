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

            MessageBox.Show($"Inscription : {nom} ({email})\nMot de passe : {new string('*', password.Length)}");
            // TODO : implémenter la logique d'inscription réelle (validation, enregistrement)
            _dbservice.getusers(nom, email, password);
        }
    }
}
