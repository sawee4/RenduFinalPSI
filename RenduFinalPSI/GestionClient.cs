using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace RenduFinalPSI
{
    public class GestionClient
    {
        public GestionClient()
        {
        }

        /// afficher les commandes du client
        public void AfficherMesCommandes(int idClient)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT c.id_commande, c.date_commande, c.montant_total, " +"GROUP_CONCAT(p.nom) as plats " +"FROM COMMANDE c " +"JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +"JOIN PLAT p ON ct.id_plat = p.id_plat " +"WHERE c.id_client = @idClient " +"GROUP BY c.id_commande";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idClient", idClient);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Mes Commandes ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Date: " + reader["date_commande"]);
                    Console.WriteLine("Montant: " + reader["montant_total"] + " euros");
                    Console.WriteLine("Plats: " + reader["plats"]);
                    Console.WriteLine("-------------------");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        /// afficher les paiements du client
        public void AfficherMesPaiements(int idClient)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT c.id_commande, c.date_commande, c.montant_total " +"FROM COMMANDE c " +"WHERE c.id_client = @idClient " +"ORDER BY c.date_commande DESC";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idClient", idClient);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Mes Paiements ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Date: " + reader["date_commande"]);
                    Console.WriteLine("Montant: " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        /// afficher les cuisiniers du client
        public void AfficherMesCuisiniers(int idClient)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT DISTINCT u.id_utilisateur, u.nom, u.prenom, c.note_moyenne, " +
                               "COUNT(DISTINCT p.id_plat) as nb_plats " +
                               "FROM UTILISATEUR u " +
                               "JOIN cuisinier c ON u.id_utilisateur = c.id_utilisateur " +
                               "JOIN PLAT p ON c.id_utilisateur = p.id_cuisinier " +
                               "GROUP BY u.id_utilisateur, u.nom, u.prenom, c.note_moyenne " +
                               "ORDER BY u.nom, u.prenom";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Mes Cuisiniers ===");
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["id_utilisateur"]);
                    Console.WriteLine("Nom: " + reader["nom"] + " " + reader["prenom"]);
                    Console.WriteLine("Note moyenne: " + reader["note_moyenne"]);
                    Console.WriteLine("Nombre de plats disponibles: " + reader["nb_plats"]);
                    Console.WriteLine("-------------------");
                }
                reader.Close();

                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        /// passer une commande
        public void PasserCommande(int idClient)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                
                // Afficher les plats disponibles
                AfficherPlatsDisponibles();
                
                Console.Write("\nEntrez l'ID du plat que vous souhaitez commander : ");
                int idPlat = Convert.ToInt32(Console.ReadLine());
                
                Console.Write("Quantité : ");
                int quantite = Convert.ToInt32(Console.ReadLine());

                // Créer la commande
                string requeteMaxId = "SELECT COALESCE(MAX(id_commande), 0) + 1 FROM COMMANDE";
                MySqlCommand commande = new MySqlCommand(requeteMaxId, connection);
                int idCommande = Convert.ToInt32(commande.ExecuteScalar());

                string requeteCommande = "INSERT INTO COMMANDE (id_commande, id_client, date_commande, montant_total) VALUES (@idCommande, @idClient, NOW(), 0)";
                commande = new MySqlCommand(requeteCommande, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.ExecuteNonQuery();

                // Récupérer les informations du plat
                string requetePlat = "SELECT id_cuisinier, recette_id, prix_personne FROM PLAT WHERE id_plat = @idPlat";
                commande = new MySqlCommand(requetePlat, connection);
                commande.Parameters.AddWithValue("@idPlat", idPlat);
                MySqlDataReader reader = commande.ExecuteReader();
                reader.Read();
                int idCuisinier = Convert.ToInt32(reader["id_cuisinier"]);
                int recetteId = Convert.ToInt32(reader["recette_id"]);
                decimal prixUnitaire = Convert.ToDecimal(reader["prix_personne"]);
                reader.Close();

                // Ajouter le plat à la commande
                string requeteContient = "INSERT INTO contient (id_commande, id_client, id_plat, id_cuisinier, recette_id, quantite, prix_unitaire) " +
                                       "VALUES (@idCommande, @idClient, @idPlat, @idCuisinier, @recetteId, @quantite, @prixUnitaire)";
                
                commande = new MySqlCommand(requeteContient, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.Parameters.AddWithValue("@idPlat", idPlat);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@recetteId", recetteId);
                commande.Parameters.AddWithValue("@quantite", quantite);
                commande.Parameters.AddWithValue("@prixUnitaire", prixUnitaire);
                commande.ExecuteNonQuery();

                // Mettre à jour le montant total
                string requeteUpdate = "UPDATE COMMANDE SET montant_total = @montantTotal WHERE id_commande = @idCommande AND id_client = @idClient";
                commande = new MySqlCommand(requeteUpdate, connection);
                commande.Parameters.AddWithValue("@montantTotal", quantite * prixUnitaire);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.ExecuteNonQuery();

                Console.WriteLine("Commande passée avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la commande: " + ex.Message);
            }
        }

        /// afficher les plats disponibles
        private void AfficherPlatsDisponibles()
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT p.id_plat, p.nom, p.prix_personne, p.nationalite, " +"u.nom as nom_cuisinier, u.prenom as prenom_cuisinier " +"FROM PLAT p " +"JOIN cuisinier c ON p.id_cuisinier = c.id_utilisateur " +"JOIN UTILISATEUR u ON c.id_utilisateur = u.id_utilisateur";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Plats Disponibles ===");
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["id_plat"]);
                    Console.WriteLine("Nom: " + reader["nom"]);
                    Console.WriteLine("Prix: " + reader["prix_personne"] + " euros");
                    Console.WriteLine("Nationalité: " + reader["nationalite"]);
                    Console.WriteLine("Cuisinier: " + reader["nom_cuisinier"] + " " + reader["prenom_cuisinier"]);
                    Console.WriteLine("-------------------");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        /// noter une commande
        public void NoterCommande(int idClient)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                
                string requete = "SELECT c.id_commande, c.date_commande, p.nom as plat_nom, " +"cu.id_utilisateur as id_cuisinier, u.nom as nom_cuisinier, u.prenom as prenom_cuisinier " + "FROM COMMANDE c " +"JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +"JOIN PLAT p ON ct.id_plat = p.id_plat " +"JOIN cuisinier cu ON p.id_cuisinier = cu.id_utilisateur " +"JOIN UTILISATEUR u ON cu.id_utilisateur = u.id_utilisateur " + "LEFT JOIN note_avis n ON c.id_commande = n.id_commande " +"WHERE c.id_client = @idClient AND n.id_commande IS NULL " +"GROUP BY c.id_commande";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idClient", idClient);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Commandes à noter ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Date: " + reader["date_commande"]);
                    Console.WriteLine("Plat: " + reader["plat_nom"]);
                    Console.WriteLine("Cuisinier: " + reader["prenom_cuisinier"] + " " + reader["nom_cuisinier"]);
                    Console.WriteLine("-------------------");
                }
                reader.Close();

                Console.Write("\nEntrez l'ID de la commande à noter (0 pour annuler) : ");
                int idCommande = Convert.ToInt32(Console.ReadLine());

                if (idCommande == 0) return;

                Console.Write("Note (1-5) : ");
                int note = Convert.ToInt32(Console.ReadLine());
                while (note < 1 || note > 5)
                {
                    Console.Write("Note invalide. Veuillez entrer une note entre 1 et 5 : ");
                    note = Convert.ToInt32(Console.ReadLine());
                }

                Console.Write("Commentaire (optionnel) : ");
                string commentaire = Console.ReadLine();

                requete = "SELECT p.id_cuisinier FROM COMMANDE c " +"JOIN contient ct ON c.id_commande = ct.id_commande " +"JOIN PLAT p ON ct.id_plat = p.id_plat " +"WHERE c.id_commande = @idCommande AND c.id_client = @idClient " +"LIMIT 1";

                commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idClient);
                int idCuisinier = Convert.ToInt32(commande.ExecuteScalar());

                requete = "INSERT INTO note_avis (id_utilisateur, id_utilisateur_1, note, date_avis, commentaire) " +"VALUES (@idCuisinier, @idClient, @note, NOW(), @commentaire)";

                commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.Parameters.AddWithValue("@note", note);
                commande.Parameters.AddWithValue("@commentaire", commentaire);
                commande.ExecuteNonQuery();

                requete = "UPDATE cuisinier c " +"SET note_moyenne = (SELECT AVG(CAST(note AS DECIMAL)) FROM note_avis WHERE id_utilisateur = c.id_utilisateur) " +"WHERE c.id_utilisateur = @idCuisinier";

                commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.ExecuteNonQuery();

                Console.WriteLine("Note enregistrée avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la notation : " + ex.Message);
            }
        }

        public void GererMenuClient(int idClient)
        {
            bool continuer = true;
            while (continuer)
            {
                Console.WriteLine("\n=== Menu Client ===");
                Console.WriteLine("1. Afficher mes commandes");
                Console.WriteLine("2. Passer une commande");
                Console.WriteLine("3. Noter mes commandes");
                Console.WriteLine("4. Retour");
                Console.WriteLine("5. Quitter");
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AfficherMesCommandes(idClient);
                        break;
                    case "2":
                        PasserCommande(idClient);
                        break;
                    case "3":
                        NoterCommandes(idClient);
                        break;
                    case "4":
                        continuer = false;
                        break;
                    case "5":
                        continuer = false;
                        ConnexionBDD.SeDeconnecter();
                        Console.WriteLine("Au revoir !");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
        }

        public void NoterCommandes(int idClient)
        {
            try
            {
                string requete = "SELECT c.id_commande, c.date_commande, c.montant_total, " +"u.nom as nom_cuisinier, u.prenom as prenom_cuisinier, " +"clc.id_utilisateur as id_cuisinier " +"FROM COMMANDE c " +"JOIN cuisnier_livre_commande clc ON c.id_commande = clc.id_commande " +"JOIN UTILISATEUR u ON clc.id_utilisateur = u.id_utilisateur " +"WHERE c.id_client = @idClient " +"AND NOT EXISTS (SELECT 1 FROM note_avis na " +"WHERE na.id_utilisateur = clc.id_utilisateur " +"AND na.id_utilisateur_1 = @idClient)";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idClient", idClient);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Commandes à noter ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Date : " + reader["date_commande"]);
                    Console.WriteLine("Cuisinier : " + reader["prenom_cuisinier"] + " " + reader["nom_cuisinier"]);
                    Console.WriteLine("Montant : " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();

                Console.Write("\nEntrez le numéro de la commande à noter (0 pour annuler) : ");
                int idCommande = Convert.ToInt32(Console.ReadLine());

                if (idCommande == 0)
                {
                    return;
                }

                string requeteCuisinier = "SELECT clc.id_utilisateur as id_cuisinier " +
                                        "FROM cuisnier_livre_commande clc " +
                                        "WHERE clc.id_commande = @idCommande";
                
                commande = new MySqlCommand(requeteCuisinier, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                reader = commande.ExecuteReader();
                
                if (!reader.Read())
                {
                    Console.WriteLine("Commande non trouvée.");
                    reader.Close();
                    return;
                }
                
                int idCuisinier = Convert.ToInt32(reader["id_cuisinier"]);
                reader.Close();

                Console.Write("Note (1-5) : ");
                int note = Convert.ToInt32(Console.ReadLine());
                while (note < 1 || note > 5)
                {
                    Console.Write("Note invalide. Entrez une note entre 1 et 5 : ");
                    note = Convert.ToInt32(Console.ReadLine());
                }

                Console.Write("Commentaire (optionnel) : ");
                string commentaire = Console.ReadLine();

                string requeteInsert = "INSERT INTO note_avis (id_utilisateur, id_utilisateur_1, note, commentaire) " +
                                     "VALUES (@idCuisinier, @idClient, @note, @commentaire)";
                
                commande = new MySqlCommand(requeteInsert, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.Parameters.AddWithValue("@note", note);
                commande.Parameters.AddWithValue("@commentaire", commentaire);
                
                commande.ExecuteNonQuery();
                
                string requeteUpdate = "UPDATE cuisinier c " +"SET c.note_moyenne = (SELECT AVG(note) FROM note_avis WHERE id_utilisateur = c.id_utilisateur) " +"WHERE c.id_utilisateur = @idCuisinier";
                
                commande = new MySqlCommand(requeteUpdate, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.ExecuteNonQuery();

                Console.WriteLine("Note enregistrée avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la notation : " + ex.Message);
            }
        }

        public void AfficherPlusCourtChemin(int idClient)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                GestionStationUtilisateur gestionStation = new GestionStationUtilisateur();

                // Afficher la liste des cuisiniers
                string requete = "SELECT DISTINCT u.id_utilisateur, u.nom, u.prenom, c.note_moyenne " +
                               "FROM UTILISATEUR u " +
                               "JOIN CUISINIER c ON u.id_utilisateur = c.id_utilisateur " +
                               "JOIN PLAT p ON c.id_utilisateur = p.id_cuisinier " +
                               "ORDER BY u.nom, u.prenom";

                MySqlCommand commande = new MySqlCommand(requete, connection);

                MySqlDataReader reader = commande.ExecuteReader();
                Console.WriteLine("\n=== Liste des cuisiniers ===");
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["id_utilisateur"] + " - " + 
                                    reader["nom"] + " " + reader["prenom"] + 
                                    " (Note: " + reader["note_moyenne"] + ")");
                }
                reader.Close();

                bool continuer = true;
                while (continuer)
                {
                    Console.WriteLine("\nEntrez l'ID du cuisinier pour voir le plus court chemin (ou 0 pour retourner) :");
                    if (int.TryParse(Console.ReadLine(), out int idCuisinier))
                    {
                        if (idCuisinier == 0)
                        {
                            continuer = false;
                            continue;
                        }

                        // Utilisez la méthode mise à jour qui permet de choisir l'algorithme
                        List<Station> chemin = gestionStation.TrouverCheminLivraison(idClient, idCuisinier);
                        
                        if (chemin != null && chemin.Count > 0)
                        {
                            Console.WriteLine("\n=== Chemin de livraison détaillé ===");
                            Console.WriteLine("Départ : " + chemin[0].Nom + " (Ligne " + chemin[0].Ligne + ")");
                            
                            for (int i = 1; i < chemin.Count - 1; i++)
                            {
                                Console.WriteLine("Correspondance : " + chemin[i].Nom + " (Ligne " + chemin[i].Ligne + ")");
                            }
                            
                            Console.WriteLine("Arrivée : " + chemin[chemin.Count - 1].Nom + " (Ligne " + chemin[chemin.Count - 1].Ligne + ")");
                            Console.WriteLine("Nombre total de stations : " + chemin.Count);

                            // Affichez les arcs du chemin pour vérification
                            GrapheMetro graphe = new GrapheMetro();
                            for (int i = 0; i < chemin.Count - 1; i++)
                            {
                                Console.WriteLine("Arc du chemin trouvé : " + chemin[i].Nom + " -> " + chemin[i + 1].Nom);
                            }

                            // Afficher le graphe
                            graphe.DessinerGraphe("chemin_metro.png", chemin);
                            Console.WriteLine("\nLe graphe a été sauvegardé dans le fichier 'chemin_metro.png'");
                            
                            // Ouvrir le graphe
                            try
                            {
                                string cheminComplet = Path.Combine(Directory.GetCurrentDirectory(), "chemin_metro.png");
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = cheminComplet,
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Impossible d'ouvrir le graphe automatiquement : " + ex.Message);
                                Console.WriteLine("Le graphe a été sauvegardé dans : " + Path.Combine(Directory.GetCurrentDirectory(), "chemin_metro.png"));
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nAucun chemin trouvé.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("ID invalide.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }
    }
} 