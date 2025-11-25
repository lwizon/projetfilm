using System.Windows;
using System.Windows.Controls;

namespace ProjetFilmv1
{
    public partial class InscriptionPage : Page
    {
        public InscriptionPage()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string nom = NomTextBox.Text;
            string email = EmailTextBox.Text;
            string mdp = PasswordBox.Password;

            MessageBox.Show($"Inscription : {nom} ({email})\nMot de passe : {new string('*', mdp.Length)}");
            // TODO : implémenter la logique d'inscription réelle (validation, enregistrement)
        }
    }
}
