using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using ProjetFilmv1.Services;
using ProjetFilmv1.Models;

namespace ProjetFilmv1
{
    public partial class MainWindow : Window
    {
        private readonly TmdbService _tmdb = new TmdbService();
        private CancellationTokenSource? _cts;
        private readonly Duration _animDuration = new Duration(TimeSpan.FromMilliseconds(180));

        // Champ dummy utilisé uniquement pour référencer les propriétés de SuggestionItem (analyseurs)
        private readonly SuggestionItem _dummySuggestion = new SuggestionItem();

        public MainWindow()
        {
            // Appel utilitaire pour s'assurer que l'analyseur sait que SuggestionItem est utilisé
            SuggestionItem.TouchProperties();

            // Références explicites pour éviter les avertissements de l'analyseur
            _ = _dummySuggestion.Title;
            _ = _dummySuggestion.PosterImage;
            _ = _dummySuggestion.Rating;
            _ = _dummySuggestion.Description;

            InitializeComponent();
            MainFrame.Navigate(new AccueilPage());   // Page par défaut
        }

        private void ShowSuggestions()
        {
            SuggestionsBorder.Visibility = Visibility.Visible;

            var fade = new DoubleAnimation(0, 1, _animDuration) { EasingFunction = new QuadraticEase() };
            var slide = new DoubleAnimation(-6, 0, _animDuration) { EasingFunction = new QuadraticEase() };

            SuggestionsBorder.BeginAnimation(UIElement.OpacityProperty, fade);
            var tt = (TranslateTransform)SuggestionsBorder.RenderTransform;
            tt.BeginAnimation(TranslateTransform.YProperty, slide);
        }

        private void HideSuggestions()
        {
            var fade = new DoubleAnimation(1, 0, _animDuration) { EasingFunction = new QuadraticEase() };
            var slide = new DoubleAnimation(0, -6, _animDuration) { EasingFunction = new QuadraticEase() };

            fade.Completed += (s, e) => SuggestionsBorder.Visibility = Visibility.Collapsed;

            SuggestionsBorder.BeginAnimation(UIElement.OpacityProperty, fade);
            var tt = (TranslateTransform)SuggestionsBorder.RenderTransform;
            tt.BeginAnimation(TranslateTransform.YProperty, slide);
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

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = SearchBox.Text?.Trim() ?? string.Empty;

            // Cancel previous pending requests
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            // Debounce: attendre 300ms avant d'effectuer la requête
            try
            {
                await Task.Delay(300, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                SearchSuggestions.ItemsSource = null;
                HideSuggestions();
                return;
            }

            try
            {
                var results = await _tmdb.SearchMoviesAsync(text, 1);
                var titles = results
                    .Take(5)
                    .Select(m => new SuggestionItem
                    {
                        Id = m.Id,
                        Title = m.Title ?? string.Empty,
                        PosterImage = m.PosterFullPath,
                        Rating = m.VoteAverage > 0 ? m.VoteAverage.ToString("0.0") : "-",
                        Description = string.IsNullOrWhiteSpace(m.Overview) ? "" : m.Overview.Length > 120 ? m.Overview.Substring(0, 117) + "..." : m.Overview
                    })
                    .ToList();
                if (titles.Count > 0)
                {
                    SearchSuggestions.ItemsSource = titles;
                    SearchSuggestions.DisplayMemberPath = "Title"; // pas nécessaire avec ItemTemplate mais sans mal
                    ShowSuggestions();
                }
                else
                {
                    SearchSuggestions.ItemsSource = null;
                    HideSuggestions();
                }
            }
            catch (Exception)
            {
                // Silence les erreurs de suggestion pour ne pas déranger l'utilisateur
                SearchSuggestions.ItemsSource = null;
                HideSuggestions();
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Empêche le 'ding' du système
                e.Handled = true;
                Search_Click(sender, new RoutedEventArgs());
            }
            else if (e.Key == Key.Down)
            {
                // Navigue dans la liste de suggestions
                if (SuggestionsBorder.Visibility == Visibility.Visible && SearchSuggestions.Items.Count > 0)
                {
                    SearchSuggestions.Focus();
                    SearchSuggestions.SelectedIndex = 0;
                    var item = (ListBoxItem)SearchSuggestions.ItemContainerGenerator.ContainerFromIndex(0);
                    item?.Focus();
                }
            }
        }

        private void SearchSuggestions_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SearchSuggestions.SelectedItem != null)
            {
                dynamic sel = SearchSuggestions.SelectedItem;
                SearchBox.Text = sel.Title;
                SuggestionsBorder.Visibility = Visibility.Collapsed;
                Search_Click(sender, new RoutedEventArgs());
            }
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            // Récupère la requête et supprime les espaces superflus
            string query = (SearchBox.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Veuillez entrer un titre de film à rechercher.", "Recherche vide", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Navigue vers la page de résultats de recherche
            MainFrame.Navigate(new SearchResultsPage(query));
            // Masque les suggestions
            SuggestionsBorder.Visibility = Visibility.Collapsed;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileButton.ContextMenu != null)
            {
                ProfileButton.ContextMenu.PlacementTarget = ProfileButton;
                ProfileButton.ContextMenu.IsOpen = true;
            }
        }

        private void ProfileMenu_Profil_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ouvrir la page Profil (placeholder)", "Profil", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProfileMenu_Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ouvrir les paramètres (placeholder)", "Paramètres", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProfileMenu_Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Déconnexion (placeholder)", "Déconnexion", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}