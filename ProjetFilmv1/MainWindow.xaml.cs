using System.Windows;
using System.Windows.Controls;

namespace ProjetFilmv1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new AccueilPage());   // Page par défaut
        }

        private void Accueil_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AccueilPage());
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ConnexionPage());
        }

        private void Inscription_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InscriptionPage());
        }

        private void Film_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FilmPage());
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            string query = SearchBox.Text;
            MessageBox.Show($"Recherche : {query}");
            // plus tard tu pourras naviguer vers une page de résultats
        }
    }
}