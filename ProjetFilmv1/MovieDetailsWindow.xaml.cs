using System;
using System.Windows;
using System.Windows.Media.Imaging;
using ProjetFilmv1.Models;

namespace ProjetFilmv1
{
    public partial class MovieDetailsWindow : Window
    {
        public MovieDetailsWindow(Movie movie)
        {
            InitializeComponent();
            if (movie == null) return;

            TitleText.Text = movie.Title ?? "Sans titre";
            VoteText.Text = $"Note : {movie.VoteAverage:N1}";
            OverviewText.Text = movie.Overview ?? string.Empty;

            if (!string.IsNullOrEmpty(movie.PosterFullPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(movie.PosterFullPath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PosterImage.Source = bitmap;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
