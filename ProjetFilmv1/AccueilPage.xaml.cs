using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using ProjetFilmv1.Services;
using ProjetFilmv1.Models;
using System.ComponentModel;

namespace ProjetFilmv1
{
    public partial class AccueilPage : Page, INotifyPropertyChanged
    {
        private readonly TmdbService _tmdb = new TmdbService();
        public ObservableCollection<Movie> Movies { get; } = new ObservableCollection<Movie>();
        public ObservableCollection<Genre> Genres { get; } = new ObservableCollection<Genre>();

        private int _currentPage = 1;
        private int _totalPages = 1;
        private int _currentGenreId = 0; // 0 = tous / non sélectionné

        private Movie? _selectedMovie;
        public Movie? SelectedMovie
        {
            get => _selectedMovie;
            set
            {
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged(nameof(SelectedMovie));
                    if (_selectedMovie != null)
                    {
                        ShowDetailsInPane(_selectedMovie);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private bool _suppressPageComboChanged = false;

        public AccueilPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += AccueilPage_Loaded;
            // Refresh button removed from UI; keep ReloadCurrentAsync available via PageComboBox or pagination controls.

            // populate page selector 1..20
            for (int i = 1; i <= 20; i++) PageComboBox.Items.Add(i);
            PageComboBox.SelectedIndex = 0; // page 1

            PageComboBox.SelectionChanged += async (s, e) =>
            {
                if (_suppressPageComboChanged) return;

                if (PageComboBox.SelectedItem is int p)
                {
                    _currentPage = p;
                    await LoadMoviesForCurrentSelectionAsync();
                }
                else if (PageComboBox.SelectedItem is object obj && int.TryParse(obj.ToString(), out var v))
                {
                    _currentPage = v;
                    await LoadMoviesForCurrentSelectionAsync();
                }
            };

            // after InitializeComponent, wire MoviesListBox selection changed just in case binding doesn't update immediately
            MoviesListBox.SelectionChanged += (s, e) =>
            {
                if (MoviesListBox.SelectedItem is Movie m)
                {
                    SelectedMovie = m;
                }
            };

            GenreComboBox.SelectionChanged += async (s, e) =>
            {
                if (GenreComboBox.SelectedItem is Genre g)
                {
                    _currentGenreId = g.Id;
                }
                else
                {
                    _currentGenreId = 0;
                }

                _currentPage = 1;
                _suppressPageComboChanged = true;
                PageComboBox.SelectedIndex = 0;
                _suppressPageComboChanged = false;
                await LoadMoviesForCurrentSelectionAsync();
            };

            PrevPageButton.Click += async (s, e) =>
            {
                if (_currentPage > 1)
                {
                    _currentPage--;
                    _suppressPageComboChanged = true;
                    PageComboBox.SelectedItem = _currentPage;
                    _suppressPageComboChanged = false;
                    await LoadMoviesForCurrentSelectionAsync();
                }
            };

            NextPageButton.Click += async (s, e) =>
            {
                if (_currentPage < _totalPages && _currentPage < 20)
                {
                    _currentPage++;
                    _suppressPageComboChanged = true;
                    PageComboBox.SelectedItem = _currentPage;
                    _suppressPageComboChanged = false;
                    await LoadMoviesForCurrentSelectionAsync();
                }
            };

            LoadAllButton.Click += async (s, e) => await LoadAllPagesAsync();

            // create page buttons 1..20
            for (int i = 1; i <= 20; i++)
            {
                var btn = new Button() { Content = i.ToString(), Width = 40, Margin = new Thickness(4), Tag = i };
                btn.Click += async (s, e) =>
                {
                    if (s is Button b && b.Tag is int page)
                    {
                        _currentPage = page;
                        _suppressPageComboChanged = true;
                        PageComboBox.SelectedItem = page;
                        _suppressPageComboChanged = false;
                        await LoadMoviesForCurrentSelectionAsync();
                        UpdatePageButtons();
                    }
                };
                PageButtonsPanel.Children.Add(btn);
            }

            UpdatePageButtons();
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
                Debug.WriteLine("Loading genres...");
                var list = await _tmdb.GetGenresAsync();
                Debug.WriteLine($"Genres loaded: {list.Count}");
                Genres.Clear();
                Genres.Add(new Genre { Id = 0, Name = "Tous" });
                foreach (var g in list) Genres.Add(g);
                GenreComboBox.ItemsSource = Genres;
                GenreComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur LoadGenresAsync: {ex}");
                MessageBox.Show($"Erreur lors de la récupération des genres : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadMoviesForCurrentSelectionAsync()
        {
            try
            {
                Debug.WriteLine($"Loading movies for genre {_currentGenreId} page {_currentPage}...");
                TmdbResponse? resp = null;
                if (_currentGenreId == 0)
                {
                    resp = await _tmdb.GetPopularMoviesResponseAsync(_currentPage);
                }
                else
                {
                    resp = await _tmdb.GetMoviesByGenreResponseAsync(_currentGenreId, _currentPage);
                }

                var movies = resp?.Results ?? new System.Collections.Generic.List<Movie>();
                Debug.WriteLine($"Movies loaded: {movies.Count}");
                Movies.Clear();
                foreach (var m in movies) Movies.Add(m);

                _totalPages = resp?.TotalPages ?? 1;
                PageText.Text = $"Page {_currentPage} / {_totalPages}";

                // Synchronize PageComboBox to reflect current page (silently)
                try
                {
                    _suppressPageComboChanged = true;
                    if (PageComboBox.Items.Count >= 1)
                    {
                        var index = Math.Max(0, Math.Min(PageComboBox.Items.Count - 1, _currentPage - 1));
                        PageComboBox.SelectedIndex = index;
                    }
                    PageComboBox.IsEnabled = _totalPages > 1;
                }
                finally
                {
                    _suppressPageComboChanged = false;
                }

                PrevPageButton.IsEnabled = _currentPage > 1;
                NextPageButton.IsEnabled = _currentPage < _totalPages;

                // Update page buttons UI
                try
                {
                    if (PageButtonsPanel != null)
                    {
                        PageButtonsPanel.Visibility = Visibility.Visible;
                        UpdatePageButtons();
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur LoadMoviesForCurrentSelectionAsync: {ex}");
                MessageBox.Show($"Erreur lors de la récupération des films : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ReloadCurrentAsync()
        {
            await LoadMoviesForCurrentSelectionAsync();
        }

        private void ShowDetailsInPane(Movie movie)
        {
            try
            {
                Debug.WriteLine($"ShowDetailsInPane movie id={movie?.Id} title={movie?.Title}");
                if (movie == null) return;

                DetailTitleText.Text = movie.Title ?? "Sans titre";
                DetailVoteText.Text = $"Note : {movie.VoteAverage:N1}";
                DetailOverviewText.Text = movie.Overview ?? string.Empty;

                if (!string.IsNullOrEmpty(movie.PosterFullPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(movie.PosterFullPath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    DetailPosterImage.Source = bitmap;
                }
                else
                {
                    DetailPosterImage.Source = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur ShowDetailsInPane: {ex}");
                MessageBox.Show($"Erreur en affichant le détail: {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnMovieButtonClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("OnMovieButtonClick triggered");
            if (sender is Button btn && btn.DataContext is Movie movie)
            {
                Debug.WriteLine($"Clicked movie id={movie.Id} title={movie.Title}");
                ShowDetailsInPane(movie);
            }
        }

        private async Task LoadAllPagesAsync()
        {
            try
            {
                _currentPage = 1;
                Movies.Clear();

                TmdbResponse? firstResp = null;
                if (_currentGenreId == 0)
                    firstResp = await _tmdb.GetPopularMoviesResponseAsync(1);
                else
                    firstResp = await _tmdb.GetMoviesByGenreResponseAsync(_currentGenreId, 1);

                if (firstResp == null) return;

                var total = firstResp.TotalPages;
                Debug.WriteLine($"LoadAll: total pages = {total}");

                // add first page
                foreach (var m in firstResp.Results) Movies.Add(m);

                // fetch remaining pages
                for (int p = 2; p <= total; p++)
                {
                    await Task.Delay(250); // small delay between requests
                    TmdbResponse? resp = null;
                    if (_currentGenreId == 0)
                        resp = await _tmdb.GetPopularMoviesResponseAsync(p);
                    else
                        resp = await _tmdb.GetMoviesByGenreResponseAsync(_currentGenreId, p);

                    if (resp == null) break;

                    foreach (var m in resp.Results) Movies.Add(m);
                }

                _currentPage = 1;
                _totalPages = total;
                _suppressPageComboChanged = true;
                PageComboBox.SelectedIndex = 0;
                _suppressPageComboChanged = false;
                PageText.Text = $"Page {_currentPage} / {_totalPages} (chargé)";
                PrevPageButton.IsEnabled = false;
                NextPageButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur LoadAllPagesAsync: {ex}");
                MessageBox.Show($"Erreur lors du chargement total : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePageButtons()
        {
            // Highlight the current page button and reset others
            foreach (var child in PageButtonsPanel.Children)
            {
                if (child is Button btn && btn.Tag is int page)
                {
                    btn.IsEnabled = true;
                    btn.Opacity = (page == _currentPage) ? 1.0 : 0.5;
                }
            }
        }
    }
}
