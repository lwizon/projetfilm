using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySql.Data.MySqlClient;
using ProjetFilmv1;
using ProjetFilmv1.Models;
using ProjetFilmv1.Services;

namespace ProjetFilmV1
{
    public partial class GestionProfile : Page
    {
        private readonly string _connectionString = "Server=172.20.11.6;Port=3306;Database=projetfilm;Uid=admin_bdd;Pwd=rootroot;";
        private readonly string? _displayName;
        private readonly FavoritesService _favoritesService = new FavoritesService();
        private readonly TmdbService _tmdbService = new TmdbService();

        public ObservableCollection<FavoriteMovieItem> FavoriteMovies { get; } = new ObservableCollection<FavoriteMovieItem>();

        public GestionProfile()
        {
            InitializeComponent();
            DataContext = this;
            ChargerNomUtilisateur();
            ShowOverviewSection();
            Loaded += async (_, _) => await LoadFavoritesAsync();
        }

        public GestionProfile(string displayName) : this()
        {
            _displayName = displayName;

            if (!string.IsNullOrWhiteSpace(displayName) && UserNameText != null)
            {
                UserNameText.Text = displayName;
            }
        }

        private void ChargerNomUtilisateur()
        {
            try
            {
                using var conn = new MySqlConnection(_connectionString);
                conn.Open();

                const string sql = "SELECT nom FROM users WHERE id_user = @id";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", Session.IdUtilisateurConnecte);

                var result = cmd.ExecuteScalar();

                if (result != null && UserNameText != null)
                {
                    UserNameText.Text = result.ToString();
                }
                else if (!string.IsNullOrWhiteSpace(_displayName) && UserNameText != null)
                {
                    UserNameText.Text = _displayName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement nom : " + ex.Message);
            }
        }

        private async Task LoadFavoritesAsync()
        {
            FavoriteMovies.Clear();

            try
            {
                foreach (var favorite in _favoritesService.GetFavorites(Session.IdUtilisateurConnecte))
                {
                    var enrichedFavorite = favorite;

                    if (favorite.Id > 0)
                    {
                        var movie = await _tmdbService.GetMovieByIdAsync(favorite.Id);
                        if (movie != null)
                        {
                            enrichedFavorite = new FavoriteMovieItem
                            {
                                Id = movie.Id,
                                Title = movie.Title ?? favorite.Title,
                                Overview = movie.Overview ?? string.Empty,
                                PosterPath = movie.PosterPath,
                                VoteAverage = movie.VoteAverage
                            };
                        }
                    }

                    FavoriteMovies.Add(enrichedFavorite);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement favoris : " + ex.Message);
            }

            UpdateFavoritesUiState();
        }

        private void UpdateFavoritesUiState()
        {
            if (FavoritesCountText != null)
            {
                FavoritesCountText.Text = FavoriteMovies.Count.ToString();
            }

            if (FavoritesBadgeText != null)
            {
                FavoritesBadgeText.Text = FavoriteMovies.Count == 1
                    ? "1 film sauvegarde"
                    : $"{FavoriteMovies.Count} films sauvegardes";
            }

            if (FavoritesEmptyState != null)
            {
                FavoritesEmptyState.Visibility = FavoriteMovies.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void ShowOverviewSection()
        {
            MainFrame.Visibility = Visibility.Collapsed;
            OverviewSection.Visibility = Visibility.Visible;
            FavoritesSection.Visibility = Visibility.Collapsed;
            ContentTitleText.Text = "Vue d'ensemble";
            ContentSubtitleText.Text = "Retrouvez vos raccourcis de compte et vos films enregistres.";
            ApplyMenuState(isOverviewActive: true);
            UpdateFavoritesUiState();
        }

        private async Task ShowFavoritesSectionAsync()
        {
            await LoadFavoritesAsync();
            MainFrame.Visibility = Visibility.Collapsed;
            OverviewSection.Visibility = Visibility.Collapsed;
            FavoritesSection.Visibility = Visibility.Visible;
            ContentTitleText.Text = "Mes favoris";
            ContentSubtitleText.Text = "Une selection visuelle de vos films enregistres.";
            ApplyMenuState(isOverviewActive: false);
        }

        private void ApplyMenuState(bool isOverviewActive)
        {
            if (ShowOverviewButton != null)
            {
                ShowOverviewButton.Background = isOverviewActive
                    ? (Brush)FindResource("AccentSoftBrush")
                    : Brushes.Transparent;
                ShowOverviewButton.Foreground = isOverviewActive
                    ? (Brush)FindResource("AccentBrush")
                    : (Brush)FindResource("SecondaryTextBrush");
            }

            if (ShowFavoritesButton != null)
            {
                ShowFavoritesButton.Background = !isOverviewActive
                    ? (Brush)FindResource("AccentSoftBrush")
                    : Brushes.Transparent;
                ShowFavoritesButton.Foreground = !isOverviewActive
                    ? (Brush)FindResource("AccentBrush")
                    : (Brush)FindResource("SecondaryTextBrush");
            }
        }

        private void OpenProfileDetails(object sender, RoutedEventArgs e)
        {
            OverviewSection.Visibility = Visibility.Collapsed;
            FavoritesSection.Visibility = Visibility.Collapsed;
            MainFrame.Visibility = Visibility.Visible;
            MainFrame.Navigate(new GestionInfoProfile());
        }

        private void ShowOverviewButton_Click(object sender, RoutedEventArgs e)
        {
            ShowOverviewSection();
        }

        private async void ShowFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowFavoritesSectionAsync();
        }

        private async void OpenFavoriteMovie_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not FavoriteMovieItem favorite)
            {
                return;
            }

            var movie = favorite.ToMovie();
            if (favorite.Id > 0)
            {
                movie = await _tmdbService.GetMovieByIdAsync(favorite.Id) ?? movie;
            }

            var owner = Window.GetWindow(this);
            var detailsWindow = new MovieDetailsWindow(movie)
            {
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            detailsWindow.ShowDialog();
            await LoadFavoritesAsync();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                var confirm = MessageBox.Show(
                    "Voulez-vous vraiment supprimer votre compte ? Toutes les donnees seront perdues.",
                    "Confirmation de suppression",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes && !mainWindow.DeleteCurrentUser())
                {
                    MessageBox.Show(
                        "La suppression du compte a echoue.",
                        "Suppression impossible",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
