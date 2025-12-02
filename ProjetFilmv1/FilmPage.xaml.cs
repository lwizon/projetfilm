using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using ProjetFilmv1.Services;
using ProjetFilmv1.Models;

namespace ProjetFilmv1
{
    public partial class FilmPage : Page
    {
        private readonly TmdbService _tmdb = new TmdbService();

        public FilmPage()
        {
            InitializeComponent();
            Loaded += FilmPage_Loaded;
        }

        private async void FilmPage_Loaded(object sender, RoutedEventArgs e)
        {
            await DumpPopularMoviesAsync();
        }

        private async Task DumpPopularMoviesAsync()
        {
            try
            {
                var movies = await _tmdb.GetPopularMoviesAsync();
                var sb = new StringBuilder();
                sb.AppendLine($"Nombre de films récupérés : {movies.Count}");
                foreach (var m in movies.Take(10))
                {
                    sb.AppendLine($"- {m.Title} (note: {m.VoteAverage:N1})");
                }

                var dump = sb.ToString();
                Debug.WriteLine(dump);
                MessageBox.Show(dump, "Dump TMDB (test)", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erreur TMDB: {ex}");
                MessageBox.Show($"Erreur lors de la récupération : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
