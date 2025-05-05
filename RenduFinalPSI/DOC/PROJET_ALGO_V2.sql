drop database if exists live_in_paris;
create database live_in_paris;
use live_in_paris;

-- Créer d'abord la table UTILISATEUR
CREATE TABLE UTILISATEUR(
    id_utilisateur INT AUTO_INCREMENT,
    prenom VARCHAR(50),
    adresse VARCHAR(50),
    telephone VARCHAR(50),
    mot_de_passe VARCHAR(50),
    type_utilisateur VARCHAR(50),
    email VARCHAR(50),
    nom VARCHAR(50),
    id_utilisateur_1 INT NULL,
    id_utilisateur_2 INT NULL,
    PRIMARY KEY(id_utilisateur)
    -- Les contraintes FOREIGN KEY seront ajoutées plus tard
);

-- Ensuite créer les tables dépendantes
CREATE TABLE cuisinier(
    id_utilisateur INT NOT NULL,
    note_moyenne DECIMAL(15,2),
    PRIMARY KEY(id_utilisateur),
    FOREIGN KEY(id_utilisateur) REFERENCES UTILISATEUR(id_utilisateur)
);

CREATE TABLE CLIENT(
    id_utilisateur INT NOT NULL,
    type_client VARCHAR(50) NOT NULL,
    nom_entreprise VARCHAR(100),
    referent VARCHAR(50),
    id_commande INT NULL DEFAULT NULL,
    id_client INT NULL DEFAULT NULL,
    PRIMARY KEY(id_utilisateur),
    FOREIGN KEY(id_utilisateur) REFERENCES UTILISATEUR(id_utilisateur)
    -- La contrainte vers COMMANDE sera ajoutée plus tard
);

CREATE TABLE PLAT(
    id_plat INT AUTO_INCREMENT,
    id_cuisinier INT NOT NULL DEFAULT 1,
    recette_id INT NOT NULL DEFAULT 1,
    nom VARCHAR(50),
    type VARCHAR(50),
    date_fabrication DATE,
    date_peremption DATE,
    prix_personne DECIMAL(15,2),
    nationalite VARCHAR(50),
    regime_alimentaire VARCHAR(50),
    ingredients TEXT,
    photo VARCHAR(50),
    nb_personnes INT,
    PRIMARY KEY(id_plat, id_cuisinier, recette_id),
    FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_utilisateur)
);

CREATE TABLE COMMANDE(
    id_commande INT NOT NULL DEFAULT 1,
    id_client INT NOT NULL DEFAULT 1,
    date_commande DATETIME,
    montant_total DECIMAL(15,2),
    PRIMARY KEY(id_commande, id_client),
    FOREIGN KEY(id_client) REFERENCES CLIENT(id_utilisateur)
);

CREATE TABLE RECETTE(
    id_recette INT NOT NULL DEFAULT 1,
    description TEXT,
    ingredients TEXT,
    nom VARCHAR(50),
    id_plat INT NOT NULL DEFAULT 1,
    id_cuisinier INT NOT NULL DEFAULT 1,
    recette_id INT NOT NULL DEFAULT 1,
    PRIMARY KEY(id_recette),
    FOREIGN KEY(id_plat, id_cuisinier, recette_id) REFERENCES PLAT(id_plat, id_cuisinier, recette_id)
);

-- Maintenant que toutes les tables sont créées, ajouter les contraintes restantes à UTILISATEUR
ALTER TABLE UTILISATEUR
ADD CONSTRAINT fk_utilisateur_cuisinier FOREIGN KEY(id_utilisateur_1) REFERENCES cuisinier(id_utilisateur),
ADD CONSTRAINT fk_utilisateur_client FOREIGN KEY(id_utilisateur_2) REFERENCES CLIENT(id_utilisateur);

-- Ajouter la contrainte manquante à CLIENT
ALTER TABLE CLIENT
ADD CONSTRAINT fk_client_commande FOREIGN KEY(id_commande, id_client) REFERENCES COMMANDE(id_commande, id_client);

CREATE TABLE cuisnier_livre_commande(
    id_utilisateur INT NOT NULL DEFAULT 1,
    id_commande INT NOT NULL DEFAULT 1,
    id_client INT NOT NULL DEFAULT 1,
    date_livraison DATETIME,
    adresse_livraison VARCHAR(300),
    PRIMARY KEY(id_utilisateur, id_commande, id_client),
    FOREIGN KEY(id_utilisateur) REFERENCES cuisinier(id_utilisateur),
    FOREIGN KEY(id_commande, id_client) REFERENCES COMMANDE(id_commande, id_client)
);

CREATE TABLE note_avis(
    id_utilisateur INT NOT NULL DEFAULT 1,
    id_utilisateur_1 INT NOT NULL DEFAULT 1,
    note VARCHAR(50),
    date_avis DATETIME,
    commentaire VARCHAR(400),
    PRIMARY KEY(id_utilisateur, id_utilisateur_1),
    FOREIGN KEY(id_utilisateur) REFERENCES cuisinier(id_utilisateur),
    FOREIGN KEY(id_utilisateur_1) REFERENCES CLIENT(id_utilisateur)
);

CREATE TABLE contient(
    id_plat INT NOT NULL DEFAULT 1,
    id_cuisinier INT NOT NULL DEFAULT 1,
    recette_id INT NOT NULL DEFAULT 1,
    id_commande INT NOT NULL DEFAULT 1,
    id_client INT NOT NULL DEFAULT 1,
    quantite INT,
    prix_unitaire DECIMAL(15,2),
    date_livraison DATETIME,
    adresse_livraison VARCHAR(300),
    PRIMARY KEY(id_plat, id_cuisinier, recette_id, id_commande, id_client),
    FOREIGN KEY(id_plat, id_cuisinier, recette_id) REFERENCES PLAT(id_plat, id_cuisinier, recette_id),
    FOREIGN KEY(id_commande, id_client) REFERENCES COMMANDE(id_commande, id_client)
);