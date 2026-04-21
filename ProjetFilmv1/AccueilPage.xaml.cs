using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1.Models;
using ProjetFilmv1.Services;

namespace ProjetFilmv1
{
    public partial class AccueilPage : Page, INotifyPropertyChanged
    {
        private readonly TmdbService _tmdb = new TmdbService();

        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        public ObservableCollection<Genre> Genres { get; } = new ObservableCollection<Genre>();

        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _currentGenreId;
        private bool _suppressPageComboChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public AccueilPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += AccueilPage_Loaded;

            for (int i = 1; i <= 20; i++)
            {
                PageComboBox.Items.Add(i);
            }

            PageComboBox.SelectedIndex = 0;

            PageComboBox.SelectionChanged += async (_, _) =>
            {
                if (_suppressPageComboChanged)
                {
                    return;
                }

                if (PageComboBox.SelectedItem is int page)
                {
                    _currentPage = page;
                    await LoadMoviesForCurrentSelectionAsync();
                }
                else if (PageComboBox.SelectedItem is object obj && int.TryParse(obj.ToString(), out var parsedPage))
                {
                    _currentPage = parsedPage;
                    await LoadMoviesForCurrentSelectionAsync();
                }
            };

            GenreComboBox.SelectionChanged += async (_, _) =>
            {
                _currentGenreId = GenreComboBox.SelectedItem is Genre genre ? genre.Id : 0;
                _currentPage = 1;

                _suppressPageComboChanged = true;
                PageComboBox.SelectedIndex = 0;
                _suppressPageComboChanged = false;

                await LoadMoviesForCurrentSelectionAsync();
            };

            PrevPageButton.Click += async (_, _) =>
            {
                if (_currentPage <= 1)
                {
                    return;
                }

                _currentPage--;
                _suppressPageComboChanged = true;
                PageComboBox.SelectedItem = _currentPage;
                _suppressPageComboChanged = false;
                await LoadMoviesForCurrentSelectionAsync();
            };

            NextPageButton.Click += async (_, _) =>
            {
                if (_currentPage >= _totalPages || _currentPage >= 20)
                {
                    return;
                }

                _currentPage++;
                _suppressPageComboChanged = true;
                PageComboBox.SelectedItem = _currentPage;
                _suppressPageComboChanged = false;
                await LoadMoviesForCurrentSelectionAsync();
            };
        }

        private async void AccueilPage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadGenresAsync();
            await LoadMoviesForCurrentSelectionAsync();
        }

        private async Task LoadGenresAsync()
        {
            try
            {
                var list = await _tmdb.GetGenresAsync();
                Genres.Clear();
                Genres.Add(new Genre { Id = 0, Name = "Tous" });

                foreach (var genre in list)
                {
                    Genres.Add(genre);
                }

                GenreComboBox.ItemsSource = Genres;
                GenreComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur LoadGenresAsync: {ex}");
                MessageBox.Show(
                    $"Erreur lors de la récupération des genres : {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private async Task LoadMoviesForCurrentSelectionAsync()
        {
            try
            {
                TmdbResponse? response = _currentGenreId == 0
                    ? await _tmdb.GetPopularMoviesResponseAsync(_currentPage)
                    : await _tmdb.GetMoviesByGenreResponseAsync(_currentGenreId, _currentPage);

                Movies.Clear();
                foreach (var movie in response?.Results ?? [])
                {
                    Movies.Add(movie);
                }

                _totalPages = response?.TotalPages ?? 1;
                PageText.Text = $"Page {_currentPage} / {_totalPages}";

                try
                {
                    _suppressPageComboChanged = true;
                    if (PageComboBox.Items.Count > 0)
                    {
                        PageComboBox.SelectedIndex = Math.Max(0, Math.Min(PageComboBox.Items.Count - 1, _currentPage - 1));
                    }

                    PageComboBox.IsEnabled = _totalPages > 1;
                }
                finally
                {
                    _suppressPageComboChanged = false;
                }

                PrevPageButton.IsEnabled = _currentPage > 1;
                NextPageButton.IsEnabled = _currentPage < _totalPages && _currentPage < 20;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur LoadMoviesForCurrentSelectionAsync: {ex}");
                MessageBox.Show(
                    $"Erreur lors de la récupération des films : {ex.Message}",
                    "Erreur",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void OnMovieButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.DataContext is not Movie movie)
            {
                return;
            }

            try
            {
                var owner = Window.GetWindow(this);
                var detailsWindow = new MovieDetailsWindow(movie)
                {
                    Owner = owner,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                detailsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur ouverture détail film: {ex}");
                MessageBox.Show($"Erreur en affichant le détail : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdatePaneLayout()
        {
            try
            {
                if (LeftColumn == null || RightColumn == null)
                    return;

                if (SelectedMovie != null)
                {
                    LeftColumn.Width = new GridLength(1, GridUnitType.Star);
                    RightColumn.Width = new GridLength(2, GridUnitType.Star);
                }
                else
                {
                    LeftColumn.Width = new GridLength(1, GridUnitType.Star);
                    RightColumn.Width = new GridLength(1, GridUnitType.Star);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur UpdatePaneLayout: {ex}");
            }
        }

        private void MoviesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (MoviesListBox.SelectedItem is Movie movie)
                    SelectedMovie = movie;
                else
                    SelectedMovie = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur MoviesListBox_SelectionChanged: {ex}");
            }
        }
    }
}