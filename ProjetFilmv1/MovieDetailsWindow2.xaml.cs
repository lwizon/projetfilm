using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using ProjetFilmv1.Models;

namespace ProjetFilmv1
{
    public partial class MovieDetailsWindow2 : Window
    {
        public MovieDetailsWindow2(Movie movie)
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception initEx)
            {
                Debug.WriteLine($"InitializeComponent failed: {initEx}");
                try { MessageBox.Show($"Erreur lors de l'initialisation de la fenêtre de détails:\n{initEx}", "Erreur XAML", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
                return;
            }

            if (movie == null)
            {
                Debug.WriteLine("MovieDetailsWindow2: movie == null");
                return;
            }

            // Set window title
            try
            {
                this.Title = movie.Title ?? "Détails du film";
            }
            catch { }

            // Fill text fields (guard against missing controls)
            try
            {
                if (TitleText != null) TitleText.Text = movie.Title ?? "Sans titre";
                if (VoteText != null) VoteText.Text = $"Note : {movie.VoteAverage:N1}";
                if (OverviewText != null) OverviewText.Text = movie.Overview ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting text fields: {ex}");
            }

            // Load poster image safely
            try
            {
                if (!string.IsNullOrEmpty(movie.PosterFullPath) && PosterImage != null)
                {
                    if (Uri.TryCreate(movie.PosterFullPath, UriKind.Absolute, out var uri))
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = uri;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                        bitmap.EndInit();
                        PosterImage.Source = bitmap;
                    }
                    else
                    {
                        Debug.WriteLine($"Invalid poster URI: {movie.PosterFullPath}");
                        PosterImage.Source = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading poster image: {ex}");
                try { PosterImage.Source = null; } catch { }
            }

            try
            {
                if (CloseButton != null)
                    CloseButton.Click += (s, e) => this.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error wiring CloseButton: {ex}");
            }
        }
    }
}
