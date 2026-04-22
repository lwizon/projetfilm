using MySql.Data.MySqlClient;

const string connectionString = "Server=172.20.11.6;Port=3306;Database=projetfilm;Uid=admin_bdd;Pwd=rootroot;";
const string applicationConnectionString = "Server=172.20.11.6;Database=projetfilm;User ID=user_bdd;Password=rootroot;";
const string createTableSql = """
CREATE TABLE IF NOT EXISTS films_favoris (
    id_favori INT NOT NULL AUTO_INCREMENT,
    id_utilisateur INT NOT NULL,
    id_film_tmdb INT NOT NULL,
    titre_film VARCHAR(255) NOT NULL,
    date_ajout DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_favori),
    UNIQUE KEY uq_films_favoris_utilisateur_film (id_utilisateur, id_film_tmdb),
    KEY ix_films_favoris_utilisateur (id_utilisateur),
    CONSTRAINT fk_films_favoris_utilisateur
        FOREIGN KEY (id_utilisateur) REFERENCES users(id_user)
        ON DELETE CASCADE
);
""";

await using var connection = new MySqlConnection(connectionString);
await connection.OpenAsync();

await using var command = new MySqlCommand(createTableSql, connection);
await command.ExecuteNonQueryAsync();

await using var droitsCommande = new MySqlCommand("""
    GRANT SELECT, INSERT, DELETE ON projetfilm.films_favoris TO 'user_bdd'@'%';
    """, connection);
await droitsCommande.ExecuteNonQueryAsync();

Console.WriteLine("TABLE films_favoris CREEE OU DEJA PRESENTE");
Console.WriteLine("DROITS user_bdd AJOUTES");

await using var applicationConnection = new MySqlConnection(applicationConnectionString);
await applicationConnection.OpenAsync();

var lireCommande = new MySqlCommand("SELECT COUNT(*) FROM films_favoris", applicationConnection);
var totalFavoris = Convert.ToInt32(await lireCommande.ExecuteScalarAsync());
Console.WriteLine($"LECTURE OK - {totalFavoris} ligne(s) actuellement");

var utilisateurCommande = new MySqlCommand("SELECT id_user FROM users ORDER BY id_user LIMIT 1", applicationConnection);
var idUtilisateur = Convert.ToInt32(await utilisateurCommande.ExecuteScalarAsync());
const int idFilmTest = 9999991;
const string titreTest = "Test Favori Technique";

var insertionCommande = new MySqlCommand("""
    INSERT INTO films_favoris (id_utilisateur, id_film_tmdb, titre_film)
    VALUES (@idUtilisateur, @idFilmTmdb, @titreFilm)
    ON DUPLICATE KEY UPDATE titre_film = VALUES(titre_film)
    """, applicationConnection);
insertionCommande.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);
insertionCommande.Parameters.AddWithValue("@idFilmTmdb", idFilmTest);
insertionCommande.Parameters.AddWithValue("@titreFilm", titreTest);
await insertionCommande.ExecuteNonQueryAsync();
Console.WriteLine("INSERTION OK");

var suppressionCommande = new MySqlCommand("""
    DELETE FROM films_favoris
    WHERE id_utilisateur = @idUtilisateur AND id_film_tmdb = @idFilmTmdb
    """, applicationConnection);
suppressionCommande.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);
suppressionCommande.Parameters.AddWithValue("@idFilmTmdb", idFilmTest);
await suppressionCommande.ExecuteNonQueryAsync();
Console.WriteLine("SUPPRESSION OK");
