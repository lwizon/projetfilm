using System.Diagnostics;

namespace ProjetFilmv1.Models
{
    // Classe utilisée pour binder les suggestions d'autocomplétion dans le XAML.
    // Les propriétés sont créées et consommées via binding XAML et code-behind.
    [DebuggerDisplay("{Title}")]
    public class SuggestionItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        // URL de l'image (poster) — binding WPF accepte une URL string pour Image.Source
        public string? PosterImage { get; set; }

        public string Rating { get; set; } = "-";

        public string Description { get; set; } = string.Empty;

        // Utilisé pour s'assurer que les analyseurs remarquent l'utilisation des propriétés
        public override string ToString()
        {
            return $"{Title} - {Rating} - {Description?.Substring(0, Math.Min(30, Description.Length))}";
        }

        // Appel explicite pour "utiliser" les propriétés afin d'éviter certains avertissements d'analyse statique
        public static void TouchProperties()
        {
            var s = new SuggestionItem();
            _ = s.Id;
            _ = s.Title;
            _ = s.PosterImage;
            _ = s.Rating;
            _ = s.Description;
        }
    }
}
