using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using ProjetFilmv1;

namespace ProjetFilmV1 
{
    public partial class GestionInfoProfile : Page 
    {
        private string _connectionString = "Server=172.20.11.6;Port=3306;Database=projetfilm;Uid=admin_bdd;Pwd=rootroot;";
        private int _currentUserId = Session.IdUtilisateurConnecte; 
        private User? _userProfile;

        public GestionInfoProfile()
        {
            InitializeComponent();
            ChargerDonnees();
        }

        private void ChargerDonnees()
        {
            try 
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT nom, email FROM users WHERE id_user = @id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", _currentUserId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _userProfile = new User
                            {
                                Username = reader["nom"].ToString() ?? "",
                                Email = reader["email"].ToString() ?? ""
                            };
                            this.DataContext = _userProfile;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Erreur lecture : " + ex.Message); }
        }

        private void BtnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();
                    string mdpSaisi = PasswordInput.Password; 
                    string sql;

                    if (string.IsNullOrWhiteSpace(mdpSaisi)) {
                        sql = "UPDATE users SET nom = @user, email = @email WHERE id_user = @id";
                    } else {
                        sql = "UPDATE users SET nom = @user, email = @email, password = @mdp WHERE id_user = @id";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@user", UsernameTextBox.Text);
                    cmd.Parameters.AddWithValue("@email", EmailTextBox.Text);
                    cmd.Parameters.AddWithValue("@id", _currentUserId);
                    
                    if (!string.IsNullOrWhiteSpace(mdpSaisi)) {
                        cmd.Parameters.AddWithValue("@mdp", mdpSaisi);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profil mis à jour !");
                    ChargerDonnees();
                }
            }
            catch (Exception ex) { MessageBox.Show("Erreur sauvegarde : " + ex.Message); }
        }
        
        private void BtnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            ChargerDonnees();
            PasswordInput.Clear();
        }
    }

    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}