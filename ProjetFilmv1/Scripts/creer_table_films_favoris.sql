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

GRANT SELECT, INSERT, DELETE ON projetfilm.films_favoris TO 'user_bdd'@'%';
