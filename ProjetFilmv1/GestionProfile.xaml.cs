using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1;

namespace ProjetFilmV1
{
    public partial class GestionProfile : Page
    {
        public GestionProfile() : this("Utilisateur")
        {
        }

        public GestionProfile(string userName)
        {
            InitializeComponent();
            UserNameText.Text = string.IsNullOrWhiteSpace(userName) ? "Utilisateur" : userName;
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
