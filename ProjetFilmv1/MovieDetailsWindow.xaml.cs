using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ProjetFilmv1.Models;

namespace ProjetFilmv1
{
    public partial class MovieDetailsWindow : Window
    {
        public MovieDetailsWindow(Movie movie)
        {
            InitializeComponent();

            if (movie == null)
            {
                return;
            }

            Title = movie.Title ?? "Détails du film";
            TitleText.Text = movie.Title ?? "Sans titre";
            SubtitleText.Text = movie.Id > 0 ? $"Film TMDB #{movie.Id}" : "Fiche film";
            VoteText.Text = $"{movie.VoteAverage:N1}/10";
            OverviewText.Text = string.IsNullOrWhiteSpace(movie.Overview)
                ? "Aucun synopsis disponible pour ce film."
                : movie.Overview;

            if (!string.IsNullOrWhiteSpace(movie.PosterFullPath))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(movie.PosterFullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                PosterImage.Source = bitmap;
            }

            AddInitialComment("Commentaires des utilisateurs", "Ajoute ton premier commentaire sur ce film.");
            AddCommentButton.Click += (_, _) => AddComment();
        }

        private void AddComment()
        {
            var text = CommentTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            AddInitialComment("Vous", text);
            CommentTextBox.Clear();
        }

        private void AddInitialComment(string author, string content)
        {
            var border = new Border
            {
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(229, 231, 235)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(10),
                Margin = new Thickness(0, 0, 0, 10)
            };

            var panel = new StackPanel();
            panel.Children.Add(new TextBlock
            {
                Text = author,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 4)
            });
            panel.Children.Add(new TextBlock
            {
                Text = content,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.FromRgb(75, 85, 99))
            });

            border.Child = panel;
            CommentsPanel.Children.Add(border);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
