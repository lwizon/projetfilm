using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1.Services;
using ProjetFilmv1.Models;
using System.Threading.Tasks;

namespace ProjetFilmv1
{
    public partial class SearchResultsPage : Page
    {
        private readonly string _query;
        private readonly TmdbService _tmdb = new TmdbService();

        public SearchResultsPage(string query)
        {
            InitializeComponent();
            _query = query;
            QueryText.Text = $"Résultats pour \"{query}\"";

            // Charger les résultats après que la page soit initialisée
            this.Loaded += async (s, e) => await LoadResultsAsync();
        }

        private async Task LoadResultsAsync()
        {
            try
            {
                NoResultsText.Visibility = Visibility.Collapsed;
                ResultsList.ItemsSource = null;

                var results = await _tmdb.SearchMoviesAsync(_query);

                if (results != null && results.Count > 0)
                {
                    ResultsList.ItemsSource = results;
                    NoResultsText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    NoResultsText.Text = "Aucun film trouvé.";
                    NoResultsText.Visibility = Visibility.Visible;
                }
            }
            catch (System.Exception)
            {
                NoResultsText.Text = "Erreur lors de la recherche. Vérifiez votre connexion ou la clé API.";
                NoResultsText.Visibility = Visibility.Visible;
            }
        }
    }
}
