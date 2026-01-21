using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1;
using ProjetFilmV1;


namespace ProfileApp 
{
    public partial class MainWindow : Window 
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void OpenProfileDetails(object sender, RoutedEventArgs e)
        {
            HeaderImg.Visibility = Visibility.Collapsed;
            
            MenuStackPanel.Visibility = Visibility.Collapsed;
            
            MainFrame.Visibility = Visibility.Visible;
            
            MainFrame.Navigate(new GestionInfoProfil_xaml());
        }
    }
}