using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using ProjetFilmv1.Models;
using System.Windows.Controls;

namespace ProjetFilmv1
{
    public partial class MovieDetailsWindow2 : Page
    {
        private readonly Movie _movie;
        public MovieDetailsWindow2(Movie movie)
        {
            try
            {
                InitializeComponent();
                _movie = movie;
                DataContext = movie;
            }
            catch (Exception initEx)
            {
                Debug.WriteLine($"InitializeComponent failed: {initEx}");
                try { MessageBox.Show($"Erreur lors de l'initialisation:\n{initEx}", "Erreur XAML", MessageBoxButton.OK, MessageBoxImage.Error); } catch { }
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
                    CloseButton.Click += (s, e) =>
                    {
                        if (NavigationService?.CanGoBack == true)
                            NavigationService.GoBack();
                    };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error wiring CloseButton: {ex}");
            }
            
            // Bouton commentaire fonctionnel
            try
            {
                if (AddCommentButton != null)
                    AddCommentButton.Click += (s, e) =>
                    {
                        var text = CommentTextBox?.Text?.Trim();

                        if (!string.IsNullOrEmpty(text) && CommentsPanel != null)
                        {
                            // Créer un nouveau TextBlock pour ce commentaire
                            var commentBlock = new TextBlock
                            {
                                Text = text,
                                TextWrapping = TextWrapping.Wrap,
                                Margin = new Thickness(0, 0, 0, 8)
                            };

                            CommentsPanel.Children.Add(commentBlock);
                            CommentTextBox.Text = string.Empty;
                        }
                    };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error wiring AddCommentButton: {ex}");
            }
        }
    }
}
