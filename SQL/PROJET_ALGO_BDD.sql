-- Suppression et création de la base de données
DROP DATABASE IF EXISTS live_in_paris;
CREATE DATABASE live_in_paris;
USE live_in_paris;

-- Création de la table UTILISATEUR
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
    id_station INT NULL,
    PRIMARY KEY(id_utilisateur)
);

-- Table CUISINIER
CREATE TABLE cuisinier(
    id_utilisateur INT NOT NULL,
    note_moyenne DECIMAL(15,2),
    PRIMARY KEY(id_utilisateur),
    FOREIGN KEY(id_utilisateur) REFERENCES UTILISATEUR(id_utilisateur)
);

-- Table CLIENT
CREATE TABLE CLIENT(
    id_utilisateur INT NOT NULL,
    type_client VARCHAR(50) NOT NULL,
    nom_entreprise VARCHAR(100),
    referent VARCHAR(50),
    id_commande INT NULL DEFAULT NULL,
    id_client INT NULL DEFAULT NULL,
    PRIMARY KEY(id_utilisateur),
    FOREIGN KEY(id_utilisateur) REFERENCES UTILISATEUR(id_utilisateur)
);

-- Table PLAT
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
    plat_du_jour BOOLEAN DEFAULT FALSE,
    saison VARCHAR(20) DEFAULT 'Toutes saisons',
    PRIMARY KEY(id_plat, id_cuisinier, recette_id),
    FOREIGN KEY(id_cuisinier) REFERENCES cuisinier(id_utilisateur)
);

-- Table COMMANDE
CREATE TABLE COMMANDE(
    id_commande INT NOT NULL DEFAULT 1,
    id_client INT NOT NULL DEFAULT 1,
    date_commande DATETIME,
    montant_total DECIMAL(15,2),
    PRIMARY KEY(id_commande, id_client),
    FOREIGN KEY(id_client) REFERENCES CLIENT(id_utilisateur)
);

-- Table RECETTE
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

-- Table CUISINIER_LIVRE_COMMANDE
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

-- Table NOTE_AVIS
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

-- Table CONTIENT
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

-- Nouvelles tables pour les fonctionnalités créatives

-- Table NOTES (système de notation)
CREATE TABLE NOTES (
    id_note INT AUTO_INCREMENT,
    id_cuisinier INT NOT NULL,
    note_qualite INT NOT NULL CHECK (note_qualite BETWEEN 1 AND 5),
    note_ponctualite INT NOT NULL CHECK (note_ponctualite BETWEEN 1 AND 5),
    commentaire TEXT,
    date_note DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_note),
    FOREIGN KEY (id_cuisinier) REFERENCES cuisinier(id_utilisateur)
);

-- Table FAVORIS (système de favoris)
CREATE TABLE FAVORIS (
    id_favori INT AUTO_INCREMENT,
    id_client INT NOT NULL,
    id_plat INT NOT NULL,
    date_ajout DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_favori),
    FOREIGN KEY (id_client) REFERENCES CLIENT(id_utilisateur),
    FOREIGN KEY (id_plat) REFERENCES PLAT(id_plat)
);

-- Table INGREDIENT (gestion des ingrédients)
CREATE TABLE INGREDIENT (
    id_ingredient INT AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    unite_mesure VARCHAR(20),
    PRIMARY KEY (id_ingredient)
);

-- Table STOCK (gestion du stock)
CREATE TABLE STOCK (
    id_stock INT AUTO_INCREMENT,
    id_cuisinier INT NOT NULL,
    id_ingredient INT NOT NULL,
    quantite DECIMAL(10,2) NOT NULL,
    date_maj DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_stock),
    FOREIGN KEY (id_cuisinier) REFERENCES cuisinier(id_utilisateur),
    FOREIGN KEY (id_ingredient) REFERENCES INGREDIENT(id_ingredient)
);

-- Ajout des contraintes manquantes
ALTER TABLE UTILISATEUR
ADD CONSTRAINT fk_utilisateur_cuisinier FOREIGN KEY(id_utilisateur_1) REFERENCES cuisinier(id_utilisateur),
ADD CONSTRAINT fk_utilisateur_client FOREIGN KEY(id_utilisateur_2) REFERENCES CLIENT(id_utilisateur);

ALTER TABLE CLIENT
ADD CONSTRAINT fk_client_commande FOREIGN KEY(id_commande, id_client) REFERENCES COMMANDE(id_commande, id_client);

-- Insertion des ingrédients de base
INSERT INTO INGREDIENT (nom, unite_mesure) VALUES
('Farine', 'kg'),
('Sucre', 'kg'),
('Oeufs', 'unité'),
('Lait', 'L'),
('Beurre', 'g'),
('Sel', 'g'),
('Poivre', 'g'),
('Huile d\'olive', 'L'),
('Tomates', 'kg'),
('Oignons', 'kg'),
('Ail', 'g'),
('Poulet', 'kg'),
('Boeuf', 'kg'),
('Poisson', 'kg'),
('Riz', 'kg'),
('Pâtes', 'kg'),
('Pommes de terre', 'kg'),
('Carottes', 'kg'),
('Courgettes', 'kg'),
('Aubergines', 'kg');