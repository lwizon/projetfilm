using ProjetFilmv1.Models;

namespace ProjetFilmv1.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string Nom { get; set; }
        public string Email { get; set; }
    }
    
    public class MovieComment
    {
        public int IdCommentaire { get; set; }
        public int IdFilm { get; set; }
        public int IdUser { get; set; }
        public string UserName { get; set; }
        public string Contenu { get; set; }
    }
}

namespace ProjetFilmv1.Services
{
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
        public static bool IsLoggedIn => CurrentUser != null;
    }
}

//deded