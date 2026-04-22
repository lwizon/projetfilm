using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjetFilmv1.Models;
using ProjetFilmv1.Services;

namespace ProjetFilmv1
{
    public partial class MovieDetailsWindow2 : Page
    {
        private readonly Movie _movie;
        private readonly CommentService _commentService = new CommentService();

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
                try
                {
                    MessageBox.Show(
                        $"Erreur lors de l'initialisation :\n{initEx}",
                        "Erreur XAML",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
                catch { }
                return;
            }

            if (movie == null)
            {
                Debug.WriteLine("MovieDetailsWindow2: movie == null");
                return;
            }

            try
            {
                Title = movie.Title ?? "Détails du film";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting title: {ex}");
            }

            try
            {
                if (TitleText != null)
                    TitleText.Text = movie.Title ?? "Sans titre";

                if (VoteText != null)
                    VoteText.Text = $"Note : {movie.VoteAverage:N1}";

                if (OverviewText != null)
                    OverviewText.Text = movie.Overview ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error setting text fields: {ex}");
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(movie.PosterFullPath) && PosterImage != null)
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
                else if (PosterImage != null)
                {
                    PosterImage.Source = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading poster image: {ex}");
                try { PosterImage.Source = null; } catch { }
            }

            try
            {
                if (FavoriteButton != null)
                {
                    FavoriteButton.Checked += (s, e) =>
                    {
                        Debug.WriteLine($"Film ajouté aux favoris : {_movie?.Title}");
                    };

                    FavoriteButton.Unchecked += (s, e) =>
                    {
                        Debug.WriteLine($"Film retiré des favoris : {_movie?.Title}");
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error wiring FavoriteButton: {ex}");
            }

            try
            {
                if (CloseButton != null)
                {
                    CloseButton.Click += (s, e) =>
                    {
                        if (NavigationService?.CanGoBack == true)
                            NavigationService.GoBack();
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error wiring CloseButton: {ex}");
            }

            try
            {
                if (AddCommentButton != null)
                {
                    AddCommentButton.Click += (s, e) => AddComment();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error wiring AddCommentButton: {ex}");
            }

            LoadComments();
        }

        private void AddComment()
{
    var text = CommentTextBox?.Text?.Trim();

    if (string.IsNullOrWhiteSpace(text))
    {
        MessageBox.Show("Veuillez écrire un commentaire.");
        return;
    }

    if (!SessionManager.IsLoggedIn || SessionManager.CurrentUser == null)
    {
        MessageBox.Show("Vous devez être connecté pour commenter.");
        return;
    }

    try
    {
        _commentService.AddComment(_movie.Title, SessionManager.CurrentUser.IdUser, text);

        if (CommentTextBox != null)
            CommentTextBox.Text = string.Empty;

        LoadComments();
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erreur lors de l'ajout du commentaire : {ex.Message}");
    }
}

private void LoadComments()
{
    if (_movie == null || CommentsPanel == null)
        return;

    CommentsPanel.Children.Clear();

    try
    {
        var comments = _commentService.GetCommentsByFilmTitle(_movie.Title);

        foreach (var comment in comments)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(227, 231, 238)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 8)
            };

            var stack = new StackPanel();

            var authorText = new TextBlock
            {
                Text = comment.UserName,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(30, 136, 229)),
                Margin = new Thickness(0, 0, 0, 4)
            };

            var contentText = new TextBlock
            {
                Text = comment.Contenu,
                TextWrapping = TextWrapping.Wrap
            };

            stack.Children.Add(authorText);
            stack.Children.Add(contentText);

            border.Child = stack;
            CommentsPanel.Children.Add(border);
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Erreur lors du chargement des commentaires : {ex.Message}");
    }
}
    }
}

//LEFLRLF