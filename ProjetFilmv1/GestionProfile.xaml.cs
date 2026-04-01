using System.Windows;
using System.Windows.Controls;

namespace ProjetFilmV1 
{
    public partial class GestionProfile : Page 
    {
        public GestionProfile() 
        {
            InitializeComponent();
        }

        private void OpenProfileDetails(object sender, RoutedEventArgs e)
        {
            // Maintenant que MainContent est nommé dans le XAML, l'erreur disparaît
            if (MainContent != null) 
            {
                MainContent.Visibility = Visibility.Collapsed;
            }
            
            // On affiche la Frame et on navigue vers la page de détails
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new GestionInfoProfile());
        }
    }
}