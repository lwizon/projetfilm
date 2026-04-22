using System;
using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1.Services;

namespace ProjetFilmv1
{
    public partial class AuthPage : Page
    {
        private readonly dbservice _dbservice;
        private bool _isRegisterMode;

        public AuthPage() : this(new dbservice(), false)
        {
        }

        public AuthPage(dbservice dbservice, bool startInRegisterMode = false)
        {
            InitializeComponent();
            _dbservice = dbservice;
            _isRegisterMode = startInRegisterMode;
            ApplyMode();
        }

        private void ApplyMode()
        {
            TitleText.Text = _isRegisterMode ? "Creer un compte" : "Connexion";
            SubtitleText.Text = _isRegisterMode
                ? "Inscris-toi pour enregistrer ton profil et interagir avec les films."
                : "Connecte-toi pour retrouver ton profil et tes actions.";
            SubmitButton.Content = _isRegisterMode ? "S'inscrire" : "Se connecter";
            TogglePromptText.Text = _isRegisterMode ? "Deja un compte ?" : "Pas de compte ?";
            ToggleModeButton.Content = _isRegisterMode ? "Connectez-vous" : "Inscrivez-vous";
            NameSection.Visibility = _isRegisterMode ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ToggleModeButton_Click(object sender, RoutedEventArgs e)
        {
            _isRegisterMode = !_isRegisterMode;
            ApplyMode();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRegisterMode)
            {
                Register();
                return;
            }

            Login();
        }

        private void Login()
        {
            var email = EmailTextBox.Text.Trim();
            var password = PasswordBox.Password;

            try
            {
                var userId = _dbservice.LoginUser(email, password);
                if (userId == -1)
                {
                    MessageBox.Show("Email ou mot de passe incorrect.", "Connexion impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    Session.IdUtilisateurConnecte = userId;
                    mainWindow.SetAuthenticatedUser(email);
                    mainWindow.NavigateToAccueil();
                }

                MessageBox.Show("Connexion reussie.", "Connexion", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Connexion impossible", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register()
        {
            var name = NomTextBox.Text.Trim();
            var email = EmailTextBox.Text.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Inscription", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _dbservice.RegisterUser(name, email, password);
                MessageBox.Show("Compte cree avec succes. Vous pouvez maintenant vous connecter.", "Inscription", MessageBoxButton.OK, MessageBoxImage.Information);
                _isRegisterMode = false;
                PasswordBox.Password = string.Empty;
                ApplyMode();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'inscription : {ex.Message}", "Inscription impossible", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
