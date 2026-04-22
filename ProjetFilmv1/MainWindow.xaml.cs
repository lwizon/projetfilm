using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ProjetFilmV1;
using ProjetFilmv1.Models;
using ProjetFilmv1.Services;

namespace ProjetFilmv1
{
    public partial class MainWindow : Window
    {
        private readonly TmdbService _tmdb = new TmdbService();
        private CancellationTokenSource? _cts;
        private readonly Duration _animDuration = new Duration(TimeSpan.FromMilliseconds(180));
        private readonly dbservice _dbservice = new dbservice();
        private bool _isUserLoggedIn;
        private string? _connectedUserEmail;
        private string? _connectedUserName;

        private readonly SuggestionItem _dummySuggestion = new SuggestionItem();

        public MainWindow()
        {
            SuggestionItem.TouchProperties();
            _ = _dummySuggestion.Title;
            _ = _dummySuggestion.PosterImage;
            _ = _dummySuggestion.Rating;
            _ = _dummySuggestion.Description;

            InitializeComponent();
            UpdateThemeToggleButton();
            UpdateAuthenticationButtons();
            MainFrame.Navigate(new AccueilPage());
        }

        public void SetAuthenticatedUser(string email)
        {
            _isUserLoggedIn = true;
            _connectedUserEmail = email;
            _connectedUserName = _dbservice.GetUserName(email) ?? email;
            UpdateAuthenticationButtons();
        }

        public void NavigateToAccueil()
        {
            SetSearchAreaVisibility(true);
            MainFrame.Navigate(new AccueilPage());
        }

        public void ExecuteProfileLogout()
        {
            ExecuteLogout();
        }

        public bool DeleteCurrentUser()
        {
            if (!_isUserLoggedIn || string.IsNullOrWhiteSpace(_connectedUserEmail))
            {
                return false;
            }

            var deleted = _dbservice.DeleteUser(_connectedUserEmail);
            if (!deleted)
            {
                return false;
            }

            ClearAuthenticatedUser();
            NavigateToAccueil();
            MessageBox.Show(
                "Votre compte a ete supprime definitivement.",
                "Compte supprime",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return true;
        }

        private void ClearAuthenticatedUser()
        {
            _isUserLoggedIn = false;
            _connectedUserEmail = null;
            _connectedUserName = null;
            Session.IdUtilisateurConnecte = 0;
            SessionManager.CurrentUser = null;
            UpdateAuthenticationButtons();
        }

        private void UpdateAuthenticationButtons()
        {
            if (AuthButton == null || ProfileButton == null)
            {
                return;
            }

            AuthButton.Visibility = _isUserLoggedIn ? Visibility.Collapsed : Visibility.Visible;
            ProfileButton.Visibility = _isUserLoggedIn ? Visibility.Visible : Visibility.Collapsed;
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
            SetSearchAreaVisibility(true);
            MainFrame.Navigate(new AccueilPage());
        }

        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            SetSearchAreaVisibility(false);
            MainFrame.Navigate(new AuthPage(_dbservice));
        }

        private void Connexion_Click(object sender, RoutedEventArgs e)
        {
            Auth_Click(sender, e);
        }

        private void Inscription_Click(object sender, RoutedEventArgs e)
        {
            SetSearchAreaVisibility(false);
            MainFrame.Navigate(new AuthPage(_dbservice, true));
        }

        private void Film_Click(object sender, RoutedEventArgs e)
        {
            SetSearchAreaVisibility(true);
            MainFrame.Navigate(new FilmPage());
        }

        private void ThemeToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current is App app)
            {
                app.ToggleTheme();
                UpdateThemeToggleButton();
            }
        }

        private async void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = SearchBox.Text?.Trim() ?? string.Empty;

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

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
                        Description = string.IsNullOrWhiteSpace(m.Overview) ? "" : m.Overview.Length > 120 ? m.Overview.Substring(0, 117) + "..." : m.Overview,
                    })
                    .ToList();

                if (titles.Count > 0)
                {
                    SearchSuggestions.ItemsSource = titles;
                    SearchSuggestions.DisplayMemberPath = "Title";
                    ShowSuggestions();
                }
                else
                {
                    SearchSuggestions.ItemsSource = null;
                    HideSuggestions();
                }
            }
            catch
            {
                SearchSuggestions.ItemsSource = null;
                HideSuggestions();
            }
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                Search_Click(sender, new RoutedEventArgs());
            }
            else if (e.Key == Key.Down)
            {
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
            string query = (SearchBox.Text ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Veuillez entrer un titre de film a rechercher.", "Recherche vide", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MainFrame.Navigate(new SearchResultsPage(query));
            SuggestionsBorder.Visibility = Visibility.Collapsed;
            SetSearchAreaVisibility(true);
        }
        
        private void SearchSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //if (SearchSuggestions.SelectedItem is SuggestionItem sel &&  != null)
                //{
                //    SearchBox.Text = sel.Title;
                //    HideSuggestions();
                //    MainFrame.Navigate(new MovieDetailsWindow2());
                //    SearchSuggestions.SelectedItem = null;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erreur lors de l'ouverture du détail : {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
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
            MainFrame.Visibility = Visibility.Visible;
            SetSearchAreaVisibility(false);
            MainFrame.Navigate(new GestionProfile(_connectedUserName ?? _connectedUserEmail ?? "Utilisateur"));
        }

        private void ProfileMenu_Settings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ouvrir les parametres (placeholder)", "Parametres", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProfileMenu_Logout_Click(object sender, RoutedEventArgs e)
        {
            ExecuteLogout();
        }

        private void ExecuteLogout()
        {
            if (!_isUserLoggedIn)
            {
                return;
            }

            var email = _connectedUserEmail;
            ClearAuthenticatedUser();
            NavigateToAccueil();

            MessageBox.Show(
                string.IsNullOrWhiteSpace(email)
                    ? "Vous etes maintenant deconnecte."
                    : $"L'utilisateur {email} a ete deconnecte.",
                "Deconnexion",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void SetSearchAreaVisibility(bool isVisible)
        {
            if (SearchArea == null)
            {
                return;
            }

            SearchArea.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            if (!isVisible)
            {
                HideSuggestions();
            }
        }

        private void UpdateThemeToggleButton()
        {
            if (Application.Current is not App app || ThemeToggleIcon == null)
            {
                return;
            }

            ThemeToggleIcon.Text = app.IsDarkTheme ? "☀" : "☾";
        }
    }
}
