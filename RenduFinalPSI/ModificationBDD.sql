-- Ajout des tables pour les nouvelles fonctionnalités

-- Table pour les notes des cuisiniers
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

-- Table pour les favoris des clients
CREATE TABLE FAVORIS (
    id_favori INT AUTO_INCREMENT,
    id_client INT NOT NULL,
    id_plat INT NOT NULL,
    date_ajout DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_favori),
    FOREIGN KEY (id_client) REFERENCES CLIENT(id_utilisateur),
    FOREIGN KEY (id_plat) REFERENCES PLAT(id_plat)
);

-- Table pour les ingrédients
CREATE TABLE INGREDIENT (
    id_ingredient INT AUTO_INCREMENT,
    nom VARCHAR(100) NOT NULL,
    unite_mesure VARCHAR(20),
    PRIMARY KEY (id_ingredient)
);

-- Table pour le stock des cuisiniers
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

-- Ajout de la colonne saison dans la table PLAT
ALTER TABLE PLAT
ADD COLUMN saison VARCHAR(20) DEFAULT 'Toutes saisons';

-- Ajout de quelques ingrédients de base
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