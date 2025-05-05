USE live_in_paris;

-- Insertion des utilisateurs de base
INSERT INTO UTILISATEUR (prenom, nom, adresse, telephone, mot_de_passe, type_utilisateur, email, id_station) VALUES
-- Cuisiniers
('Thomas', 'Dubois', '15 rue de la Paix, Paris', '0612345678', 'chef123', 'cuisinier', 'thomas.dubois@gmail.com', 1),
('Sophie', 'Martin', '24 boulevard Saint-Michel, Paris', '0623456789', 'chef456', 'cuisinier', 'sophie.martin@gmail.com', 2),
('Pierre', 'Lefebvre', '8 rue du Faubourg Saint-Honoré, Paris', '0634567890', 'chef789', 'cuisinier', 'pierre.lefebvre@gmail.com', 3),
('Marie', 'Dupont', '42 rue de Rivoli, Paris', '0645678901', 'chef101', 'cuisinier', 'marie.dupont@gmail.com', 1),
('Jean', 'Moreau', '13 avenue des Champs-Élysées, Paris', '0656789012', 'chef202', 'cuisinier', 'jean.moreau@gmail.com', 2),
-- Clients particuliers
('Lucie', 'Bernard', '7 rue Mouffetard, Paris', '0767890123', 'client123', 'client', 'lucie.bernard@gmail.com', 4),
('Antoine', 'Petit', '36 rue des Martyrs, Paris', '0778901234', 'client456', 'client', 'antoine.petit@gmail.com', 3),
('Camille', 'Leroy', '19 rue Oberkampf, Paris', '0789012345', 'client789', 'client', 'camille.leroy@gmail.com', 5),
('Nicolas', 'Garcia', '28 rue de la Roquette, Paris', '0790123456', 'client101', 'client', 'nicolas.garcia@gmail.com', 1),
('Émilie', 'Roux', '51 rue Saint-Dominique, Paris', '0701234567', 'client202', 'client', 'emilie.roux@gmail.com', 2),
-- Clients professionnels
('Alexandre', 'Simon', '14 rue de Charonne, Paris', '0712345678', 'pro123', 'client', 'alexandre.simon@gmail.com', 4),
('Julie', 'Laurent', '22 rue de Belleville, Paris', '0723456789', 'pro456', 'client', 'julie.laurent@gmail.com', 3),
('Mathieu', 'Michel', '9 boulevard de Clichy, Paris', '0734567890', 'pro789', 'client', 'mathieu.michel@gmail.com', 5),
('Claire', 'Girard', '31 rue des Abbesses, Paris', '0745678901', 'pro101', 'client', 'claire.girard@gmail.com', 1),
('Sébastien', 'Mercier', '45 avenue Parmentier, Paris', '0756789012', 'pro202', 'client', 'sebastien.mercier@gmail.com', 2);

-- Insertion des cuisiniers
INSERT IGNORE INTO cuisinier (id_utilisateur, note_moyenne) VALUES
(1, 4.8),
(2, 4.5),
(3, 4.7),
(4, 4.9),
(5, 4.6);

-- Insertion des clients
INSERT IGNORE INTO CLIENT (id_utilisateur, type_client, nom_entreprise, referent) VALUES
-- Clients particuliers
(6, 'particulier', NULL, NULL),
(7, 'particulier', NULL, NULL),
(8, 'particulier', NULL, NULL),
(9, 'particulier', NULL, NULL),
(10, 'particulier', NULL, NULL),
-- Clients professionnels
(11, 'entreprise', 'StartUp Nation', 'Alexandre Simon'),
(12, 'entreprise', 'Tech Solutions', 'Julie Laurent'),
(13, 'entreprise', 'Design Studio', 'Mathieu Michel'),
(14, 'entreprise', 'Marketing Agency', 'Claire Girard'),
(15, 'entreprise', 'Finance Consulting', 'Sébastien Mercier');

-- Mise à jour des références dans la table UTILISATEUR
UPDATE UTILISATEUR SET id_utilisateur_1 = id_utilisateur WHERE id_utilisateur <= 5;
UPDATE UTILISATEUR SET id_utilisateur_2 = id_utilisateur WHERE id_utilisateur > 5;

-- Insertion des plats
INSERT IGNORE INTO PLAT (id_cuisinier, recette_id, nom, type, date_fabrication, date_peremption, prix_personne, nationalite, regime_alimentaire, ingredients, photo, nb_personnes) VALUES
(1, 1, 'Coq au Vin', 'plat principal', '2025-05-01', '2025-05-04', 15.50, 'française', 'omnivore', 'Poulet fermier, vin rouge, lardons, champignons, oignons, carottes, thym, laurier', 'coq_au_vin.jpg', 4),
(1, 2, 'Tarte Tatin', 'dessert', '2025-05-02', '2025-05-05', 8.75, 'française', 'végétarien', 'Pommes, pâte feuilletée, sucre, beurre, cannelle', 'tarte_tatin.jpg', 6),
(2, 1, 'Risotto aux Champignons', 'plat principal', '2025-05-01', '2025-05-04', 14.25, 'italienne', 'végétarien', 'Riz arborio, champignons, oignon, vin blanc, bouillon de légumes, parmesan', 'risotto.jpg', 3),
(2, 2, 'Tiramisu', 'dessert', '2025-05-02', '2025-05-05', 9.50, 'italienne', 'végétarien', 'Mascarpone, café, biscuits cuillère, œufs, sucre, cacao', 'tiramisu.jpg', 6),
(3, 1, 'Paella', 'plat principal', '2025-05-02', '2025-05-05', 18.00, 'espagnole', 'omnivore', 'Riz, fruits de mer, poulet, poivrons, safran, petits pois', 'paella.jpg', 5),
(3, 2, 'Crème Catalane', 'dessert', '2025-05-02', '2025-05-05', 7.50, 'espagnole', 'végétarien', 'Lait, œufs, sucre, cannelle, zeste de citron', 'creme_catalane.jpg', 4),
(4, 1, 'Tajine de Poulet', 'plat principal', '2025-05-01', '2025-05-04', 16.75, 'marocaine', 'omnivore', 'Poulet, citrons confits, olives, oignons, épices, coriandre', 'tajine.jpg', 4),
(4, 2, 'Baklava', 'dessert', '2025-05-02', '2025-05-05', 10.25, 'moyen-orientale', 'végétarien', 'Pâte filo, noix, miel, cannelle, cardamome', 'baklava.jpg', 8),
(5, 1, 'Pad Thai', 'plat principal', '2025-05-02', '2025-05-05', 13.50, 'thaïlandaise', 'omnivore', 'Nouilles de riz, crevettes, œuf, tofu, cacahuètes, sauce tamarin', 'pad_thai.jpg', 2),
(5, 2, 'Mango Sticky Rice', 'dessert', '2025-05-02', '2025-05-05', 8.00, 'thaïlandaise', 'végétarien', 'Riz gluant, mangue, lait de coco, sucre', 'mango_sticky_rice.jpg', 2);

-- Insertion des recettes avec échappement des apostrophes
INSERT IGNORE INTO RECETTE (id_recette, description, ingredients, nom, id_plat, id_cuisinier, recette_id) VALUES
(1, 'Faire mariner le poulet dans le vin rouge pendant 24h. Faire revenir les lardons et les légumes, ajouter le poulet et le vin, mijoter à feu doux pendant 1h30.', 'Poulet fermier, vin rouge, lardons, champignons, oignons, carottes, thym, laurier', 'Coq au Vin Traditionnel', 1, 1, 1),
(2, 'Caraméliser les pommes dans une poêle avec du beurre et du sucre. Disposer la pâte feuilletée par-dessus et cuire au four.', 'Pommes, pâte feuilletée, sucre, beurre, cannelle', 'Tarte Tatin Classique', 2, 1, 2),
(3, 'Faire revenir les champignons et l\'oignon. Ajouter le riz et le vin blanc. Incorporer progressivement le bouillon chaud. Finir avec du parmesan.', 'Riz arborio, champignons, oignon, vin blanc, bouillon de légumes, parmesan', 'Risotto aux Champignons des Bois', 3, 2, 1),
(4, 'Mélanger le mascarpone avec les jaunes d\'œufs et le sucre. Tremper les biscuits dans le café. Alterner couches de crème et de biscuits. Saupoudrer de cacao.', 'Mascarpone, café, biscuits cuillère, œufs, sucre, cacao', 'Tiramisu Classique', 4, 2, 2),
(5, 'Faire revenir le riz avec de l\'huile d\'olive et du safran. Ajouter le bouillon. Incorporer les fruits de mer et le poulet en cours de cuisson.', 'Riz, fruits de mer, poulet, poivrons, safran, petits pois', 'Paella Valenciana', 5, 3, 1),
(6, 'Chauffer le lait avec la cannelle et le zeste de citron. Mélanger avec les jaunes d\'œufs et le sucre. Cuire jusqu\'à épaississement. Caraméliser le dessus.', 'Lait, œufs, sucre, cannelle, zeste de citron', 'Crème Catalane Traditionnelle', 6, 3, 2),
(7, 'Mariner le poulet dans les épices. Faire revenir les oignons. Ajouter le poulet, les citrons confits et les olives. Mijoter à feu doux.', 'Poulet, citrons confits, olives, oignons, épices, coriandre', 'Tajine de Poulet aux Citrons Confits', 7, 4, 1),
(8, 'Superposer des feuilles de pâte filo beurrées. Ajouter la farce aux noix entre les couches. Cuire au four et arroser de sirop de miel.', 'Pâte filo, noix, miel, cannelle, cardamome', 'Baklava aux Noix et Miel', 8, 4, 2),
(9, 'Faire tremper les nouilles. Faire sauter les crevettes, le tofu et les légumes. Ajouter les nouilles et la sauce. Garnir de cacahuètes et de coriandre.', 'Nouilles de riz, crevettes, œuf, tofu, cacahuètes, sauce tamarin', 'Pad Thai Authentique', 9, 5, 1),
(10, 'Cuire le riz gluant à la vapeur avec du lait de coco. Servir avec des tranches de mangue mûre et arroser de sirop de lait de coco.', 'Riz gluant, mangue, lait de coco, sucre', 'Mango Sticky Rice Traditionnel', 10, 5, 2);

-- Insertion des commandes
INSERT IGNORE INTO COMMANDE (id_commande, id_client, date_commande, montant_total) VALUES
(1, 6, '2025-05-01 10:15:00', 93.00),
(2, 7, '2025-05-01 11:30:00', 42.75),
(3, 8, '2025-05-01 14:45:00', 74.00),
(4, 9, '2025-05-02 09:20:00', 67.00),
(5, 10, '2025-05-02 13:10:00', 85.75),
(6, 11, '2025-05-01 15:30:00', 232.50),
(7, 12, '2025-05-02 10:45:00', 171.00),
(8, 13, '2025-05-02 12:20:00', 190.00),
(9, 14, '2025-05-02 16:05:00', 201.25),
(10, 15, '2025-05-02 17:40:00', 216.00);

-- Mise à jour des références dans la table CLIENT
UPDATE CLIENT SET id_commande = 1, id_client = 6 WHERE id_utilisateur = 6;
UPDATE CLIENT SET id_commande = 2, id_client = 7 WHERE id_utilisateur = 7;
UPDATE CLIENT SET id_commande = 3, id_client = 8 WHERE id_utilisateur = 8;
UPDATE CLIENT SET id_commande = 4, id_client = 9 WHERE id_utilisateur = 9;
UPDATE CLIENT SET id_commande = 5, id_client = 10 WHERE id_utilisateur = 10;
UPDATE CLIENT SET id_commande = 6, id_client = 11 WHERE id_utilisateur = 11;
UPDATE CLIENT SET id_commande = 7, id_client = 12 WHERE id_utilisateur = 12;
UPDATE CLIENT SET id_commande = 8, id_client = 13 WHERE id_utilisateur = 13;
UPDATE CLIENT SET id_commande = 9, id_client = 14 WHERE id_utilisateur = 14;
UPDATE CLIENT SET id_commande = 10, id_client = 15 WHERE id_utilisateur = 15;

-- Insertion des détails des commandes (contient)
INSERT IGNORE INTO contient (id_plat, id_cuisinier, recette_id, id_commande, id_client, quantite, prix_unitaire, date_livraison, adresse_livraison) VALUES
-- Commande 1
(1, 1, 1, 1, 6, 2, 15.50, '2025-05-03 18:30:00', '7 rue Mouffetard, Paris'),
(2, 1, 2, 1, 6, 1, 8.75, '2025-05-03 18:30:00', '7 rue Mouffetard, Paris'),
(3, 2, 1, 1, 6, 3, 14.25, '2025-05-03 18:30:00', '7 rue Mouffetard, Paris'),
-- Commande 2
(3, 2, 1, 2, 7, 3, 14.25, '2025-05-03 19:00:00', '36 rue des Martyrs, Paris'),
-- Commande 3
(5, 3, 1, 3, 8, 2, 18.00, '2025-05-03 19:15:00', '19 rue Oberkampf, Paris'),
(6, 3, 2, 3, 8, 2, 7.50, '2025-05-03 19:15:00', '19 rue Oberkampf, Paris'),
(4, 2, 2, 3, 8, 1, 9.50, '2025-05-03 19:15:00', '19 rue Oberkampf, Paris'),
-- Commande 4
(7, 4, 1, 4, 9, 4, 16.75, '2025-05-04 12:30:00', '28 rue de la Roquette, Paris'),
-- Commande 5
(9, 5, 1, 5, 10, 5, 13.50, '2025-05-04 13:00:00', '51 rue Saint-Dominique, Paris'),
(10, 5, 2, 5, 10, 3, 8.00, '2025-05-04 13:00:00', '51 rue Saint-Dominique, Paris'),
-- Commande 6 (entreprise)
(1, 1, 1, 6, 11, 10, 15.50, '2025-05-05 11:30:00', 'StartUp Nation, 14 rue de Charonne, Paris'),
(2, 1, 2, 6, 11, 5, 8.75, '2025-05-05 11:30:00', 'StartUp Nation, 14 rue de Charonne, Paris'),
-- Commande 7 (entreprise)
(3, 2, 1, 7, 12, 12, 14.25, '2025-05-05 12:00:00', 'Tech Solutions, 22 rue de Belleville, Paris'),
-- Commande 8 (entreprise)
(5, 3, 1, 8, 13, 8, 18.00, '2025-05-05 12:30:00', 'Design Studio, 9 boulevard de Clichy, Paris'),
(6, 3, 2, 8, 13, 6, 7.50, '2025-05-05 12:30:00', 'Design Studio, 9 boulevard de Clichy, Paris'),
-- Commande 9 (entreprise)
(7, 4, 1, 9, 14, 12, 16.75, '2025-05-05 13:00:00', 'Marketing Agency, 31 rue des Abbesses, Paris'),
-- Commande 10 (entreprise)
(9, 5, 1, 10, 15, 16, 13.50, '2025-05-05 13:30:00', 'Finance Consulting, 45 avenue Parmentier, Paris');

-- Insertion des livraisons par les cuisiniers
INSERT IGNORE INTO cuisnier_livre_commande (id_utilisateur, id_commande, id_client, date_livraison, adresse_livraison) VALUES
(1, 1, 6, '2025-05-03 18:30:00', '7 rue Mouffetard, Paris'),
(2, 2, 7, '2025-05-03 19:00:00', '36 rue des Martyrs, Paris'),
(3, 3, 8, '2025-05-03 19:15:00', '19 rue Oberkampf, Paris'),
(4, 4, 9, '2025-05-04 12:30:00', '28 rue de la Roquette, Paris'),
(5, 5, 10, '2025-05-04 13:00:00', '51 rue Saint-Dominique, Paris'),
(1, 6, 11, '2025-05-05 11:30:00', 'StartUp Nation, 14 rue de Charonne, Paris'),
(2, 7, 12, '2025-05-05 12:00:00', 'Tech Solutions, 22 rue de Belleville, Paris'),
(3, 8, 13, '2025-05-05 12:30:00', 'Design Studio, 9 boulevard de Clichy, Paris'),
(4, 9, 14, '2025-05-05 13:00:00', 'Marketing Agency, 31 rue des Abbesses, Paris'),
(5, 10, 15, '2025-05-05 13:30:00', 'Finance Consulting, 45 avenue Parmentier, Paris');

-- Insertion des notes et avis
INSERT IGNORE INTO note_avis (id_utilisateur, id_utilisateur_1, note, date_avis, commentaire) VALUES
(1, 6, '5', '2025-05-03 21:30:00', 'Excellent coq au vin ! Les saveurs étaient parfaitement équilibrées et la viande très tendre.'),
(2, 7, '4', '2025-05-03 22:00:00', 'Très bon risotto, mais un peu trop salé à mon goût. Sinon, la texture était parfaite.'),
(3, 8, '5', '2025-05-03 22:15:00', 'La paella était absolument délicieuse ! Les fruits de mer étaient très frais.'),
(4, 9, '5', '2025-05-04 15:30:00', 'Le tajine était succulent, les saveurs étaient authentiques et la viande fondante.'),
(5, 10, '4', '2025-05-04 16:00:00', 'Le pad thai était très bon, mais j\'aurais aimé un peu plus de piquant. Le mango sticky rice était parfait !'),
(1, 11, '5', '2025-05-05 14:30:00', 'Toute l\'équipe a adoré le repas ! Service impeccable et plats délicieux.'),
(2, 12, '4', '2025-05-05 15:00:00', 'Très bonne prestation, tout le monde a apprécié le risotto. Nous ferons à nouveau appel à vos services.'),
(3, 13, '5', '2025-05-05 15:30:00', 'La paella a remporté un grand succès lors de notre événement. Merci pour la qualité du service !'),
(4, 14, '5', '2025-05-05 16:00:00', 'Le tajine était parfait pour notre réunion d\'équipe. Tout le monde en redemande !'),
(5, 15, '5', '2025-05-05 16:30:00', 'Excellent pad thai qui a fait l\'unanimité parmi nos collaborateurs. Nous vous recontacterons pour nos prochains événements.');