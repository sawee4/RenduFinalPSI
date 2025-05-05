using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace RenduFinalPSI
{
    public class GestionCuisinier
    {
        public GestionCuisinier()
        {
        }

        /// afficher les commandes du cuisinier
        public void AfficherMesCommandes(int idCuisinier)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT c.id_commande, c.date_commande, c.montant_total, " +
                               "GROUP_CONCAT(p.nom) as plats, " +
                               "u.nom as nom_client, u.prenom as prenom_client " +
                               "FROM COMMANDE c " +
                               "JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +
                               "JOIN PLAT p ON ct.id_plat = p.id_plat " +
                               "JOIN UTILISATEUR u ON c.id_client = u.id_utilisateur " +
                               "WHERE p.id_cuisinier = @idCuisinier " +
                               "GROUP BY c.id_commande, c.date_commande, c.montant_total, u.nom, u.prenom";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Mes Commandes ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Date: " + reader["date_commande"]);
                    Console.WriteLine("Client: " + reader["prenom_client"] + " " + reader["nom_client"]);
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

        /// afficher les plats du cuisinier
        public void AfficherMesPlats(int idCuisinier)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT p.id_plat, p.nom, p.type, p.prix_personne, p.nationalite, " +
                               "p.regime_alimentaire, p.ingredients, p.nb_personnes " +
                               "FROM PLAT p " +
                               "WHERE p.id_cuisinier = @idCuisinier";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Mes Plats ===");
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["id_plat"]);
                    Console.WriteLine("Nom: " + reader["nom"]);
                    Console.WriteLine("Type: " + reader["type"]);
                    Console.WriteLine("Prix: " + reader["prix_personne"] + " euros");
                    Console.WriteLine("Nationalité: " + reader["nationalite"]);
                    Console.WriteLine("Régime: " + reader["regime_alimentaire"]);
                    Console.WriteLine("Ingrédients: " + reader["ingredients"]);
                    Console.WriteLine("Nombre de personnes: " + reader["nb_personnes"]);
                    Console.WriteLine("-------------------");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        /// afficher les clients du cuisinier
        public void AfficherMesClients(int idCuisinier)
        {
            GestionAffichage gestionAffichage = new GestionAffichage();
            gestionAffichage.AfficherClients(idCuisinier, false);
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// ajouter un nouveau plat
        public void AjouterPlat(int idCuisinier)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                
                Console.WriteLine("\n=== Ajout d'un nouveau plat ===");
                
                Console.Write("Nom du plat: ");
                string nom = Console.ReadLine();
                
                Console.Write("Type de plat: ");
                string type = Console.ReadLine();
                
                Console.Write("Prix par personne: ");
                decimal prix = Convert.ToDecimal(Console.ReadLine());
                
                Console.Write("Nationalité: ");
                string nationalite = Console.ReadLine();
                
                Console.Write("Régime alimentaire: ");
                string regime = Console.ReadLine();
                
                Console.Write("Ingrédients: ");
                string ingredients = Console.ReadLine();
                
                Console.Write("Nombre de personnes: ");
                int nbPersonnes = Convert.ToInt32(Console.ReadLine());

                // Insérer le plat
                string requetePlat = "INSERT INTO PLAT (id_cuisinier, nom, type, prix_personne, nationalite, " +
                                   "regime_alimentaire, ingredients, nb_personnes, date_fabrication) " +
                                   "VALUES (@idCuisinier, @nom, @type, @prix, @nationalite, @regime, " +
                                   "@ingredients, @nbPersonnes, NOW())";

                MySqlCommand commande = new MySqlCommand(requetePlat, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@nom", nom);
                commande.Parameters.AddWithValue("@type", type);
                commande.Parameters.AddWithValue("@prix", prix);
                commande.Parameters.AddWithValue("@nationalite", nationalite);
                commande.Parameters.AddWithValue("@regime", regime);
                commande.Parameters.AddWithValue("@ingredients", ingredients);
                commande.Parameters.AddWithValue("@nbPersonnes", nbPersonnes);
                commande.ExecuteNonQuery();

                // Récupérer l'ID du plat créé
                long idPlat = commande.LastInsertedId;

                // Ajouter la recette
                Console.WriteLine("\n=== Ajout de la recette ===");
                Console.Write("Description de la recette: ");
                string description = Console.ReadLine();

                string requeteRecette = "INSERT INTO RECETTE (id_recette, description, ingredients, nom, id_plat, id_cuisinier, recette_id) " +
                                      "VALUES (@idRecette, @description, @ingredients, @nom, @idPlat, @idCuisinier, 1)";

                commande = new MySqlCommand(requeteRecette, connection);
                commande.Parameters.AddWithValue("@idRecette", idPlat);
                commande.Parameters.AddWithValue("@description", description);
                commande.Parameters.AddWithValue("@ingredients", ingredients);
                commande.Parameters.AddWithValue("@nom", nom);
                commande.Parameters.AddWithValue("@idPlat", idPlat);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.ExecuteNonQuery();

                Console.WriteLine("Plat ajouté avec succès!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'ajout du plat: " + ex.Message);
            }
        }

        public void PasserCommande(int idCuisinier)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                
                string requetePlats = "SELECT p.id_plat, p.nom, p.prix_personne, p.nationalite, " +"u.nom as nom_cuisinier, u.prenom as prenom_cuisinier " +"FROM PLAT p " +"JOIN cuisinier c ON p.id_cuisinier = c.id_utilisateur " +"JOIN UTILISATEUR u ON c.id_utilisateur = u.id_utilisateur " +"WHERE p.id_cuisinier != @idCuisinier";

                MySqlCommand commande = new MySqlCommand(requetePlats, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
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

                Console.WriteLine("\nEntrez l'ID du plat que vous souhaitez commander:");
                int idPlat = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Entrez la quantité:");
                int quantite = Convert.ToInt32(Console.ReadLine());

                string requeteMaxId = "SELECT COALESCE(MAX(id_commande), 0) + 1 FROM COMMANDE";
                commande = new MySqlCommand(requeteMaxId, connection);
                int idCommande = Convert.ToInt32(commande.ExecuteScalar());

                string requeteCommande = "INSERT INTO COMMANDE (id_commande, id_client, date_commande, montant_total) " +
                                       "VALUES (@idCommande, @idClient, NOW(), 0)";
                
                commande = new MySqlCommand(requeteCommande, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idCuisinier);
                commande.ExecuteNonQuery();

                string requeteContient = "INSERT INTO contient (id_plat, id_cuisinier, recette_id, id_commande, id_client, quantite, prix_unitaire) " +"SELECT id_plat, id_cuisinier, recette_id, @idCommande, @idClient, @quantite, prix_personne " +"FROM PLAT WHERE id_plat = @idPlat";

                commande = new MySqlCommand(requeteContient, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idCuisinier);
                commande.Parameters.AddWithValue("@quantite", quantite);
                commande.Parameters.AddWithValue("@idPlat", idPlat);
                commande.ExecuteNonQuery();

                string requeteCuisinier = "SELECT id_cuisinier FROM PLAT WHERE id_plat = @idPlat";
                commande = new MySqlCommand(requeteCuisinier, connection);
                commande.Parameters.AddWithValue("@idPlat", idPlat);
                int idCuisinierCommande = Convert.ToInt32(commande.ExecuteScalar());

                string requeteLivraison = "INSERT INTO cuisnier_livre_commande (id_utilisateur, id_commande, id_client, date_livraison) " +"VALUES (@idCuisinierCommande, @idCommande, @idClient, NOW())";
                
                commande = new MySqlCommand(requeteLivraison, connection);
                commande.Parameters.AddWithValue("@idCuisinierCommande", idCuisinierCommande);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idCuisinier);
                commande.ExecuteNonQuery();

                string requetePrix = "UPDATE COMMANDE c " +"SET montant_total = (SELECT SUM(quantite * prix_unitaire) " +"FROM contient WHERE id_commande = c.id_commande AND id_client = c.id_client) " +"WHERE c.id_commande = @idCommande AND c.id_client = @idClient";

                commande = new MySqlCommand(requetePrix, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@idClient", idCuisinier);
                commande.ExecuteNonQuery();

                Console.WriteLine("Commande passée avec succès!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la commande: " + ex.Message);
            }
        }

        public void GererMenuCuisinier(int idCuisinier)
        {
            bool continuer = true;
            while (continuer)
            {
                Console.WriteLine("\n=== Menu Cuisinier ===");
                Console.WriteLine("1. Mes Commandes");
                Console.WriteLine("2. Mes Plats");
                Console.WriteLine("3. Mes Clients");
                Console.WriteLine("4. Ajouter un plat");
                Console.WriteLine("5. Voir le plus court chemin vers un client");
                Console.WriteLine("6. Statistiques");
                Console.WriteLine("7. Retour");
                Console.WriteLine("8. Quitter l'application");
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AfficherMesCommandes(idCuisinier);
                        break;
                    case "2":
                        AfficherMesPlats(idCuisinier);
                        break;
                    case "3":
                        AfficherMesClients(idCuisinier);
                        break;
                    case "4":
                        AjouterPlat(idCuisinier);
                        break;
                    case "5":
                        AfficherPlusCourtChemin(idCuisinier);
                        break;
                    case "6":
                        GestionStatistiquesCuisinier stats = new GestionStatistiquesCuisinier();
                        stats.AfficherMenuStatistiques(idCuisinier);
                        break;
                    case "7":
                        continuer = false;
                        break;
                    case "8":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
        }

        public void AfficherPlusCourtChemin(int idCuisinier)
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                GestionStationUtilisateur gestionStation = new GestionStationUtilisateur();

                // Afficher la liste des clients
                string requete = "SELECT DISTINCT u.id_utilisateur, u.nom, u.prenom, c.type_client " +
                               "FROM UTILISATEUR u " +
                               "JOIN CLIENT c ON u.id_utilisateur = c.id_utilisateur " +
                               "JOIN COMMANDE cmd ON c.id_utilisateur = cmd.id_client " +
                               "JOIN contient cont ON cmd.id_commande = cont.id_commande " +
                               "JOIN PLAT p ON cont.id_plat = p.id_plat " +
                               "WHERE p.id_cuisinier = @idCuisinier " +
                               "ORDER BY u.nom, u.prenom";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();
                Console.WriteLine("\n=== Liste de vos clients ===");
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["id_utilisateur"] + " - " + 
                                    reader["nom"] + " " + reader["prenom"] + 
                                    " (" + reader["type_client"] + ")");
                }
                reader.Close();

                bool continuer = true;
                while (continuer)
                {
                    Console.WriteLine("\nEntrez l'ID du client pour voir le plus court chemin (ou 0 pour retourner) :");
                    if (int.TryParse(Console.ReadLine(), out int idClient))
                    {
                        if (idClient == 0)
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