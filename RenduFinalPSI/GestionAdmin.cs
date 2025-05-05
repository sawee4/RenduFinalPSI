using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace RenduFinalPSI
{
    public class GestionAdmin
    {
        public GestionAdmin()
        {
        }

        /// afficher tous les clients
        public void AfficherClients()
        {
            GestionAffichage gestionAffichage = new GestionAffichage();
            gestionAffichage.AfficherClients(0, true);
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// afficher tous les cuisiniers
        public void AfficherCuisiniers()
        {
            GestionAffichage gestionAffichage = new GestionAffichage();
            gestionAffichage.AfficherCuisiniers();
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        /// <summary>
        /// affichage alphabétique des clients
        /// </summary>
        private void AfficherClientsAlphabetique()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("\n=== Clients par ordre alphabétique ===");
                
                string requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.adresse, " + "SUM(c.montant_total) as montant_total " + "FROM UTILISATEUR u " + "LEFT JOIN COMMANDE c ON u.id_utilisateur = c.id_client " + "GROUP BY u.id_utilisateur " + "ORDER BY u.nom, u.prenom";
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("ID : " + reader["id_utilisateur"]);
                    Console.WriteLine("Nom : " + reader["nom"] + " " + reader["prenom"]);
                    Console.WriteLine("Email : " + reader["email"]);
                    Console.WriteLine("Téléphone : " + reader["telephone"]);
                    Console.WriteLine("Adresse : " + reader["adresse"]);
                    Console.WriteLine("Montant total des achats : " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }
        /// <summary>
        /// affichage par adresse des clients
        /// </summary>
        private void AfficherClientsParAdresse()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("\n=== Clients par adresse ===");
                
                string requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.adresse, " + "SUM(c.montant_total) as montant_total " + "FROM UTILISATEUR u " + "LEFT JOIN COMMANDE c ON u.id_utilisateur = c.id_client " + "GROUP BY u.id_utilisateur " + "ORDER BY u.adresse";
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("ID : " + reader["id_utilisateur"]);
                    Console.WriteLine("Nom : " + reader["nom"] + " " + reader["prenom"]);
                    Console.WriteLine("Email : " + reader["email"]);
                    Console.WriteLine("Téléphone : " + reader["telephone"]);
                    Console.WriteLine("Adresse : " + reader["adresse"]);
                    Console.WriteLine("Montant total des achats : " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                Console.WriteLine("\nAppuyez sur une touche pour continuer..");
                Console.ReadKey();
            }
        }
        /// <summary>
        /// affichage par montant des achats des clients
        /// </summary>
        private void AfficherClientsParMontant()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("\n=== Clients par montant des achat ===");
                
                string requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.adresse, " + "SUM(c.montant_total) as montant_total " + "FROM UTILISATEUR u " + "LEFT JOIN COMMANDE c ON u.id_utilisateur = c.id_client " + "GROUP BY u.id_utilisateur " + "ORDER BY montant_total DESC";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("ID : " + reader["id_utilisateur"]);
                    Console.WriteLine("Nom : " + reader["nom"] + " " + reader["prenom"]);
                    Console.WriteLine("Email : " + reader["email"]);
                    Console.WriteLine("Téléphone : " + reader["telephone"]);
                    Console.WriteLine("Adresse : " + reader["adresse"]);
                    Console.WriteLine("Montant total des achats : " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }

        /// modifier un client
        private void ModifierClient()
        {
            try
            {
                AfficherClients();
                Console.Write("\nID du client à modifier : ");
                string id = Console.ReadLine();

                // Afficher les informations actuelles du client
                string requeteInfo = "SELECT * FROM UTILISATEUR WHERE id_utilisateur = @id";
                MySqlCommand commandeInfo = new MySqlCommand(requeteInfo, ConnexionBDD.GetConnection());
                commandeInfo.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = commandeInfo.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\n=== Informations actuelles ===");
                    Console.WriteLine("1. Nom : " + reader["nom"]);
                    Console.WriteLine("2. Prénom : " + reader["prenom"]);
                    Console.WriteLine("3. Adresse : " + reader["adresse"]);
                    Console.WriteLine("4. Email : " + reader["email"]);
                    Console.WriteLine("5. Mot de passe : " + reader["mot_de_passe"]);
                }
                reader.Close();

                Console.WriteLine("\nQuels champs souhaitez-vous modifier ? (séparés par des virgules, ex: 1,3,5)");
                Console.WriteLine("0. Tout modifier");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();

                string nom = "", prenom = "", adresse = "", email = "", mdp = "";
                bool toutModifier = choix == "0";

                if (toutModifier || choix.Contains("1"))
                {
                    Console.Write("Nouveau nom : ");
                    nom = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("2"))
                {
                    Console.Write("Nouveau prénom : ");
                    prenom = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("3"))
                {
                    Console.Write("Nouvelle adresse : ");
                    adresse = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("4"))
                {
                    Console.Write("Nouvel email : ");
                    email = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("5"))
                {
                    Console.Write("Nouveau mot de passe : ");
                    mdp = Console.ReadLine();
                }

                string requete = "UPDATE UTILISATEUR SET ";
                List<string> champs = new List<string>();
                if (toutModifier || choix.Contains("1")) champs.Add("nom = @nom");
                if (toutModifier || choix.Contains("2")) champs.Add("prenom = @prenom");
                if (toutModifier || choix.Contains("3")) champs.Add("adresse = @adresse");
                if (toutModifier || choix.Contains("4")) champs.Add("email = @email");
                if (toutModifier || choix.Contains("5")) champs.Add("mot_de_passe = @mdp");

                requete += string.Join(", ", champs) + " WHERE id_utilisateur = @id";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@id", id);
                if (toutModifier || choix.Contains("1")) commande.Parameters.AddWithValue("@nom", nom);
                if (toutModifier || choix.Contains("2")) commande.Parameters.AddWithValue("@prenom", prenom);
                if (toutModifier || choix.Contains("3")) commande.Parameters.AddWithValue("@adresse", adresse);
                if (toutModifier || choix.Contains("4")) commande.Parameters.AddWithValue("@email", email);
                if (toutModifier || choix.Contains("5")) commande.Parameters.AddWithValue("@mdp", mdp);

                int resultat = commande.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("Client modifié avec succès");
                }
                else
                {
                    Console.WriteLine("Erreur lors de la modification du client");
                }

                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// supprimer un client
        private void SupprimerClient()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("\n=== Suppression d'un client ===");
                
                string requeteListe = "SELECT u.id_utilisateur, u.nom, u.prenom, u.email, u.telephone, u.adresse " + "FROM UTILISATEUR u " + "JOIN CLIENT c ON u.id_utilisateur = c.id_utilisateur " + "ORDER BY u.nom, u.prenom";
                
                MySqlCommand commandeListe = new MySqlCommand(requeteListe, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commandeListe.ExecuteReader();
                
                Console.WriteLine("\nListe des clients :");
                while (reader.Read())
                {
                    Console.WriteLine("ID : " + reader["id_utilisateur"]);
                    Console.WriteLine("Nom : " + reader["nom"] + " " + reader["prenom"]);
                    Console.WriteLine("Email : " + reader["email"]);
                    Console.WriteLine("-------------------");
                }
                reader.Close();
                
                Console.Write("\nEntrez l'ID du client à supprimer : ");
                int idClient = Convert.ToInt32(Console.ReadLine());
                
                string requeteVerif = "SELECT COUNT(*) as nb_commandes FROM COMMANDE WHERE id_client = @idClient";
                MySqlCommand commandeVerif = new MySqlCommand(requeteVerif, ConnexionBDD.GetConnection());
                commandeVerif.Parameters.AddWithValue("@idClient", idClient);
                int nbCommandes = Convert.ToInt32(commandeVerif.ExecuteScalar());
                
                if (nbCommandes > 0)
                {
                    Console.WriteLine("\nCe client a des commandes en cours. Impossible de le supprimer.");
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return;
                }
                
                string requeteSuppression = "DELETE FROM CLIENT WHERE id_utilisateur = @idClient";
                MySqlCommand commandeSuppression = new MySqlCommand(requeteSuppression, ConnexionBDD.GetConnection());
                commandeSuppression.Parameters.AddWithValue("@idClient", idClient);
                
                int resultat = commandeSuppression.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("\nClient supprimé avec succès !");
                }
                else
                {
                    Console.WriteLine("\nAucun client trouvé avec cet ID.");
                }
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }

        /// afficher toutes les commandes
        public void AfficherCommandes()
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT c.id_commande, c.date_commande, c.montant_total, " +"u1.nom as nom_client, u1.prenom as prenom_client, " +"u2.nom as nom_cuisinier, u2.prenom as prenom_cuisinier, " +"GROUP_CONCAT(p.nom) as plats " +"FROM COMMANDE c " +"JOIN UTILISATEUR u1 ON c.id_client = u1.id_utilisateur " +"JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +"JOIN PLAT p ON ct.id_plat = p.id_plat " +"JOIN cuisinier cu ON p.id_cuisinier = cu.id_utilisateur " +"JOIN UTILISATEUR u2 ON cu.id_utilisateur = u2.id_utilisateur " +"GROUP BY c.id_commande, c.date_commande, c.montant_total, " +"u1.nom, u1.prenom, u2.nom, u2.prenom";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Liste des Commandes ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Date: " + reader["date_commande"]);
                    Console.WriteLine("Client: " + reader["prenom_client"] + " " + reader["nom_client"]);
                    Console.WriteLine("Cuisinier: " + reader["prenom_cuisinier"] + " " + reader["nom_cuisinier"]);
                    Console.WriteLine("Plats: " + reader["plats"]);
                    Console.WriteLine("Montant: " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();

                Console.WriteLine("\nQue souhaitez-vous faire ?");
                Console.WriteLine("1. Modifier une commande");
                Console.WriteLine("2. Supprimer une commande");
                Console.WriteLine("3. Retour au menu principal");
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        ModifierCommande();
                        break;
                    case "2":
                        SupprimerCommande();
                        break;
                    case "3":
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        private void ModifierCommande()
        {
            try
            {
                Console.Write("\nEntrez l'ID de la commande à modifier : ");
                int idCommande = Convert.ToInt32(Console.ReadLine());

                Console.Write("Nouveau montant total : ");
                decimal montantTotal = Convert.ToDecimal(Console.ReadLine());

                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "UPDATE COMMANDE SET montant_total = @montant WHERE id_commande = @idCommande";

                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.Parameters.AddWithValue("@montant", montantTotal);

                int result = commande.ExecuteNonQuery();
                if (result > 0)
                {
                    Console.WriteLine("Commande modifiée avec succès !");
                }
                else
                {
                    Console.WriteLine("Aucune commande trouvée avec cet ID.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur: " + ex.Message);
            }
        }

        private void SupprimerCommande()
        {
            try
            {
                Console.Write("\nEntrez l'ID de la commande à supprimer : ");
                int idCommande = Convert.ToInt32(Console.ReadLine());

                MySqlConnection connection = ConnexionBDD.GetConnection();

                Console.WriteLine("Suppression en cours...");

                string requeteVerif = "SELECT * FROM COMMANDE WHERE id_commande = @idCommande";
                MySqlCommand commandeVerif = new MySqlCommand(requeteVerif, connection);
                commandeVerif.Parameters.AddWithValue("@idCommande", idCommande);
                MySqlDataReader reader = commandeVerif.ExecuteReader();
                
                if (reader.Read())
                {
                    int idClient = Convert.ToInt32(reader["id_client"]);
                    reader.Close();
                    
                    try
                    {

                        MySqlTransaction transaction = connection.BeginTransaction();
                        
                        try
                        {

                            string requeteLivraisons = "DELETE FROM cuisnier_livre_commande WHERE id_commande = @idCommande";
                            MySqlCommand commandeLivraisons = new MySqlCommand(requeteLivraisons, connection);
                            commandeLivraisons.Transaction = transaction;
                            commandeLivraisons.Parameters.AddWithValue("@idCommande", idCommande);
                            commandeLivraisons.ExecuteNonQuery();
                            Console.WriteLine("Suppression des livraisons...");

                            string requeteContient = "DELETE FROM contient WHERE id_commande = @idCommande";
                            MySqlCommand commandeContient = new MySqlCommand(requeteContient, connection);
                            commandeContient.Transaction = transaction;
                            commandeContient.Parameters.AddWithValue("@idCommande", idCommande);
                            commandeContient.ExecuteNonQuery();
                            Console.WriteLine("Suppression des contenus de commande...");

                            string requeteCommande = "DELETE FROM COMMANDE WHERE id_commande = @idCommande";
                            MySqlCommand commandeCommande = new MySqlCommand(requeteCommande, connection);
                            commandeCommande.Transaction = transaction;
                            commandeCommande.Parameters.AddWithValue("@idCommande", idCommande);
                            commandeCommande.ExecuteNonQuery();

                            transaction.Commit();
                            Console.WriteLine("\nCommande n°" + idCommande + " et toutes ses données associées supprimées avec succès !");
                        }
                        catch (Exception ex)
                        {

                            transaction.Rollback();
                            throw ex;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur durant la transaction: " + ex.Message);
                    }
                }
                else
                {
                    reader.Close();
                    Console.WriteLine("\nAucune commande trouvée avec cet ID.");
                }
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la suppression: " + ex.Message);
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
        }

        /// afficher les statistiques
        public void AfficherStatistiques()
        {
            try
            {
                bool continuer = true;
                while (continuer)
                {
                    Console.Clear();
                    Console.WriteLine("\n=== Menu Administrateur ===");
                    Console.WriteLine("1. Gérer les clients");
                    Console.WriteLine("2. Gérer les cuisiniers");
                    Console.WriteLine("3. Gérer les commandes");
                    Console.WriteLine("4. Bilan des livraisons par cuisinier");
                    Console.WriteLine("5. Commandes par période");
                    Console.WriteLine("6. Statistiques des prix");
                    Console.WriteLine("7. Statistiques des comptes clients");
                    Console.WriteLine("8. Commandes par nationalité");
                    Console.WriteLine("9. Quitter");
                    Console.Write("\nVotre choix : ");
                    string choix = Console.ReadLine();

                    switch (choix)
                    {
                        case "1":
                            GererClients();
                            break;
                        case "2":
                            GererCuisiniers();
                            break;
                        case "3":
                            GererCommandes();
                            break;
                        case "4":
                            AfficherBilanLivraisons();
                            break;
                        case "5":
                            AfficherCommandesParPeriode();
                            break;
                        case "6":
                            AfficherStatistiquesPrix();
                            break;
                        case "7":
                            AfficherStatistiquesComptes();
                            break;
                        case "8":
                            AfficherCommandesParNationalite();
                            break;
                        case "9":
                            continuer = false;
                            break;
                        default:
                            Console.WriteLine("Choix invalide");
                            break;
                    }

                    if (continuer)
                    {
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void AfficherLivraisonsParCuisinier()
        {
            try
            {
                string requete = "SELECT u.nom, u.prenom, COUNT(clc.id_commande) as nb_livraisons " +"FROM UTILISATEUR u " +"JOIN cuisinier c ON u.id_utilisateur = c.id_utilisateur " +"LEFT JOIN cuisnier_livre_commande clc ON c.id_utilisateur = clc.id_utilisateur " +"GROUP BY u.id_utilisateur " +"ORDER BY nb_livraisons DESC";
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Nombre de livraisons par cuisinier ===");
                while (reader.Read())
                {
                    Console.WriteLine(reader["prenom"] + " " + reader["nom"] + " : " + reader["nb_livraisons"] + " livraisons");
                }
                reader.Close();
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void AfficherCommandesParPeriode()
        {
            try
            {
                Console.WriteLine("\n=== Choix de la période ===");
                Console.WriteLine("1. Par mois");
                Console.WriteLine("2. Par trimestre");
                Console.WriteLine("3. Par année");
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();

                string formatDate = "";
                switch (choix)
                {
                    case "1":
                        formatDate = "%Y-%m";
                        break;
                    case "2":
                        formatDate = "%Y-Q%q";
                        break;
                    case "3":
                        formatDate = "%Y";
                        break;
                    default:
                        Console.WriteLine("Choix invalide");
                        return;
                }

                string requete = "SELECT DATE_FORMAT(date_commande, @format) as periode, " +
                               "COUNT(*) as nb_commandes, " +
                               "SUM(montant_total) as montant_total, " +
                               "AVG(montant_total) as moyenne " +
                               "FROM COMMANDE " +
                               "GROUP BY periode " +
                               "ORDER BY periode DESC";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@format", formatDate);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Commandes par période ===");
                Console.WriteLine("Période | Nombre de commandes | Montant total | Moyenne");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine(reader["periode"] + " | " + reader["nb_commandes"] + " | " + 
                                    reader["montant_total"] + " euros | " + reader["moyenne"] + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void AfficherMoyennes()
        {
            try
            {
                string requete = "SELECT AVG(montant_total) as moyenne_prix, " +"COUNT(DISTINCT id_client) as nb_clients " +"FROM COMMANDE";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Moyennes ===");
                if (reader.Read())
                {
                    Console.WriteLine("Prix moyen des commandes : " + reader["moyenne_prix"] + " euros");
                    Console.WriteLine("Nombre total de clients : " + reader["nb_clients"]);
                }
                reader.Close();
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void AfficherCommandesParTypePlat()
        {
            try
            {
                string requete = "SELECT p.type, COUNT(*) as nb_commandes " +"FROM PLAT p " +"JOIN contient c ON p.id_plat = c.id_plat " +"GROUP BY p.type " +"ORDER BY nb_commandes DESC";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Commandes par type de plat ===");
                while (reader.Read())
                {
                    Console.WriteLine(reader["type"] + " : " + reader["nb_commandes"] + " commandes");
                }
                reader.Close();
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// afficher le bilan des livraisons par cuisinier
        public void AfficherBilanLivraisons()
        {
            try
            {
                string requete = "SELECT u.nom, u.prenom, COUNT(clc.id_commande) as nb_livraisons, " +
                               "SUM(c.montant_total) as montant_total " +
                               "FROM UTILISATEUR u " +
                               "JOIN cuisinier cu ON u.id_utilisateur = cu.id_utilisateur " +
                               "JOIN cuisnier_livre_commande clc ON cu.id_utilisateur = clc.id_utilisateur " +
                               "JOIN COMMANDE c ON clc.id_commande = c.id_commande " +
                               "GROUP BY u.id_utilisateur, u.nom, u.prenom " +
                               "ORDER BY nb_livraisons DESC";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Bilan des livraisons par cuisinier ===");
                Console.WriteLine("Nom | Prénom | Nombre de livraisons | Montant total");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine(reader["nom"] + " | " + reader["prenom"] + " | " + 
                                    reader["nb_livraisons"] + " | " + reader["montant_total"] + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// afficher les statistiques des prix
        public void AfficherStatistiquesPrix()
        {
            try
            {
                string requete = "SELECT AVG(montant_total) as moyenne, " +
                               "MIN(montant_total) as minimum, " +
                               "MAX(montant_total) as maximum " +
                               "FROM COMMANDE";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\n=== Statistiques des prix ===");
                    Console.WriteLine("Prix moyen des commandes : " + reader["moyenne"] + " euros");
                    Console.WriteLine("Prix minimum : " + reader["minimum"] + " euros");
                    Console.WriteLine("Prix maximum : " + reader["maximum"] + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// afficher les statistiques des comptes clients
        public void AfficherStatistiquesComptes()
        {
            try
            {
                string requete = "SELECT AVG(montant_total) as moyenne, " +
                               "COUNT(DISTINCT id_client) as nb_clients " +
                               "FROM COMMANDE";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\n=== Statistiques des comptes clients ===");
                    Console.WriteLine("Montant moyen dépensé par client : " + reader["moyenne"] + " euros");
                    Console.WriteLine("Nombre total de clients : " + reader["nb_clients"]);
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// afficher les commandes par nationalité et période
        public void AfficherCommandesParNationalite()
        {
            try
            {
                Console.Write("\nDate de début (YYYY-MM-DD) : ");
                string dateDebut = Console.ReadLine();
                Console.Write("Date de fin (YYYY-MM-DD) : ");
                string dateFin = Console.ReadLine();

                string requete = "SELECT p.nationalite, COUNT(DISTINCT c.id_commande) as nb_commandes, " +
                               "SUM(c.montant_total) as montant_total, " +
                               "AVG(c.montant_total) as moyenne " +
                               "FROM PLAT p " +
                               "JOIN contient ct ON p.id_plat = ct.id_plat " +
                               "JOIN COMMANDE c ON ct.id_commande = c.id_commande " +
                               "WHERE c.date_commande BETWEEN @dateDebut AND @dateFin " +
                               "GROUP BY p.nationalite " +
                               "ORDER BY nb_commandes DESC";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@dateDebut", dateDebut);
                commande.Parameters.AddWithValue("@dateFin", dateFin);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Commandes par nationalité ===");
                Console.WriteLine("Nationalité | Nombre de commandes | Montant total | Moyenne");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine(reader["nationalite"] + " | " + reader["nb_commandes"] + " | " + 
                                    reader["montant_total"] + " euros | " + reader["moyenne"] + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        public void MenuPrincipal()
        {
            bool continuer = true;

            while (continuer)
            {
                Console.WriteLine("\nMenu Principal Admin");
                Console.WriteLine("1. Gérer les clients");
                Console.WriteLine("2. Gérer les cuisiniers");
                Console.WriteLine("3. Gérer les commandes");
                Console.WriteLine("4. Bilan des livraisons par cuisinier");
                Console.WriteLine("5. Commandes par période");
                Console.WriteLine("6. Statistiques des prix");
                Console.WriteLine("7. Statistiques des comptes clients");
                Console.WriteLine("8. Commandes par nationalité");
                Console.WriteLine("9. Import/Export de données");
                Console.WriteLine("0. Quitter");

                Console.Write("\nChoix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        GererClients();
                        break;
                    case "2":
                        GererCuisiniers();
                        break;
                    case "3":
                        GererCommandes();
                        break;
                    case "4":
                        AfficherBilanLivraisons();
                        break;
                    case "5":
                        AfficherCommandesParPeriode();
                        break;
                    case "6":
                        AfficherStatistiquesPrix();
                        break;
                    case "7":
                        AfficherStatistiquesComptes();
                        break;
                    case "8":
                        AfficherCommandesParNationalite();
                        break;
                    case "0":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide");
                        break;
                }

                if (continuer)
                {
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }

        /// gerer les clients
        public void GererClients()
        {
            Console.WriteLine("\n=== Gestion des clients ===");
            Console.WriteLine("1. Afficher tous les clients");
            Console.WriteLine("2. Ajouter un client");
            Console.WriteLine("3. Modifier un client");
            Console.WriteLine("4. Supprimer un client");
            Console.WriteLine("5. Retour");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AfficherClients();
                    break;
                case "2":
                    AjouterClient();
                    break;
                case "3":
                    ModifierClient();
                    break;
                case "4":
                    SupprimerClient();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Choix invalide");
                    break;
            }
        }

        /// gerer les cuisiniers
        public void GererCuisiniers()
        {
            Console.WriteLine("\n=== Gestion des cuisiniers ===");
            Console.WriteLine("1. Afficher tous les cuisiniers");
            Console.WriteLine("2. Ajouter un cuisinier");
            Console.WriteLine("3. Modifier un cuisinier");
            Console.WriteLine("4. Supprimer un cuisinier");
            Console.WriteLine("5. Retour");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AfficherCuisiniers();
                    break;
                case "2":
                    AjouterCuisinier();
                    break;
                case "3":
                    ModifierCuisinier();
                    break;
                case "4":
                    SupprimerCuisinier();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Choix invalide");
                    break;
            }
        }

        /// gerer les commandes
        public void GererCommandes()
        {
            Console.WriteLine("\n=== Gestion des commandes ===");
            Console.WriteLine("1. Afficher toutes les commandes");
            Console.WriteLine("2. Ajouter une commande");
            Console.WriteLine("3. Modifier une commande");
            Console.WriteLine("4. Supprimer une commande");
            Console.WriteLine("5. Retour");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    AfficherCommandes();
                    break;
                case "2":
                    AjouterCommande();
                    break;
                case "3":
                    ModifierCommande();
                    break;
                case "4":
                    SupprimerCommande();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Choix invalide");
                    break;
            }
        }

        /// ajouter un client
        public void AjouterClient()
        {
            try
            {
                Console.Write("\nNom : ");
                string nom = Console.ReadLine();
                Console.Write("Prénom : ");
                string prenom = Console.ReadLine();
                Console.Write("Adresse : ");
                string adresse = Console.ReadLine();
                Console.Write("Email : ");
                string email = Console.ReadLine();
                Console.Write("Mot de passe : ");
                string mdp = Console.ReadLine();

                string requete = "INSERT INTO UTILISATEUR (nom, prenom, adresse, email, mot_de_passe) " +
                               "VALUES (@nom, @prenom, @adresse, @email, @mdp)";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@nom", nom);
                commande.Parameters.AddWithValue("@prenom", prenom);
                commande.Parameters.AddWithValue("@adresse", adresse);
                commande.Parameters.AddWithValue("@email", email);
                commande.Parameters.AddWithValue("@mdp", mdp);

                int resultat = commande.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("Client ajouté avec succès");
                }
                else
                {
                    Console.WriteLine("Erreur lors de l'ajout du client");
                }

                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// ajouter un cuisinier
        public void AjouterCuisinier()
        {
            try
            {
                Console.Write("\nNom : ");
                string nom = Console.ReadLine();
                Console.Write("Prénom : ");
                string prenom = Console.ReadLine();
                Console.Write("Adresse : ");
                string adresse = Console.ReadLine();
                Console.Write("Email : ");
                string email = Console.ReadLine();
                Console.Write("Mot de passe : ");
                string mdp = Console.ReadLine();
                Console.Write("Spécialité : ");
                string specialite = Console.ReadLine();

                string requete = "INSERT INTO UTILISATEUR (nom, prenom, adresse, email, mot_de_passe) " +
                               "VALUES (@nom, @prenom, @adresse, @email, @mdp); " +
                               "INSERT INTO CUISINIER (id_utilisateur, specialite) " +
                               "VALUES (LAST_INSERT_ID(), @specialite)";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@nom", nom);
                commande.Parameters.AddWithValue("@prenom", prenom);
                commande.Parameters.AddWithValue("@adresse", adresse);
                commande.Parameters.AddWithValue("@email", email);
                commande.Parameters.AddWithValue("@mdp", mdp);
                commande.Parameters.AddWithValue("@specialite", specialite);

                int resultat = commande.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("Cuisinier ajouté avec succès");
                }
                else
                {
                    Console.WriteLine("Erreur lors de l'ajout du cuisinier");
                }

                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// modifier un cuisinier
        public void ModifierCuisinier()
        {
            try
            {
                AfficherCuisiniers();
                Console.Write("\nID du cuisinier à modifier : ");
                string id = Console.ReadLine();

                // Afficher les informations actuelles du cuisinier
                string requeteInfo = "SELECT u.*, c.specialite FROM UTILISATEUR u " +
                                   "JOIN CUISINIER c ON u.id_utilisateur = c.id_utilisateur " +
                                   "WHERE u.id_utilisateur = @id";
                MySqlCommand commandeInfo = new MySqlCommand(requeteInfo, ConnexionBDD.GetConnection());
                commandeInfo.Parameters.AddWithValue("@id", id);
                MySqlDataReader reader = commandeInfo.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\n=== Informations actuelles ===");
                    Console.WriteLine("1. Nom : " + reader["nom"]);
                    Console.WriteLine("2. Prénom : " + reader["prenom"]);
                    Console.WriteLine("3. Adresse : " + reader["adresse"]);
                    Console.WriteLine("4. Email : " + reader["email"]);
                    Console.WriteLine("5. Mot de passe : " + reader["mot_de_passe"]);
                    Console.WriteLine("6. Spécialité : " + reader["specialite"]);
                }
                reader.Close();

                Console.WriteLine("\nQuels champs souhaitez-vous modifier ? (séparés par des virgules, ex: 1,3,5)");
                Console.WriteLine("0. Tout modifier");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();

                string nom = "", prenom = "", adresse = "", email = "", mdp = "", specialite = "";
                bool toutModifier = choix == "0";

                if (toutModifier || choix.Contains("1"))
                {
                    Console.Write("Nouveau nom : ");
                    nom = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("2"))
                {
                    Console.Write("Nouveau prénom : ");
                    prenom = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("3"))
                {
                    Console.Write("Nouvelle adresse : ");
                    adresse = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("4"))
                {
                    Console.Write("Nouvel email : ");
                    email = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("5"))
                {
                    Console.Write("Nouveau mot de passe : ");
                    mdp = Console.ReadLine();
                }
                if (toutModifier || choix.Contains("6"))
                {
                    Console.Write("Nouvelle spécialité : ");
                    specialite = Console.ReadLine();
                }

                string requete = "UPDATE UTILISATEUR u " +
                               "JOIN CUISINIER c ON u.id_utilisateur = c.id_utilisateur " +
                               "SET ";
                List<string> champs = new List<string>();
                if (toutModifier || choix.Contains("1")) champs.Add("u.nom = @nom");
                if (toutModifier || choix.Contains("2")) champs.Add("u.prenom = @prenom");
                if (toutModifier || choix.Contains("3")) champs.Add("u.adresse = @adresse");
                if (toutModifier || choix.Contains("4")) champs.Add("u.email = @email");
                if (toutModifier || choix.Contains("5")) champs.Add("u.mot_de_passe = @mdp");
                if (toutModifier || choix.Contains("6")) champs.Add("c.specialite = @specialite");

                requete += string.Join(", ", champs) + " WHERE u.id_utilisateur = @id";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@id", id);
                if (toutModifier || choix.Contains("1")) commande.Parameters.AddWithValue("@nom", nom);
                if (toutModifier || choix.Contains("2")) commande.Parameters.AddWithValue("@prenom", prenom);
                if (toutModifier || choix.Contains("3")) commande.Parameters.AddWithValue("@adresse", adresse);
                if (toutModifier || choix.Contains("4")) commande.Parameters.AddWithValue("@email", email);
                if (toutModifier || choix.Contains("5")) commande.Parameters.AddWithValue("@mdp", mdp);
                if (toutModifier || choix.Contains("6")) commande.Parameters.AddWithValue("@specialite", specialite);

                int resultat = commande.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("Cuisinier modifié avec succès");
                }
                else
                {
                    Console.WriteLine("Erreur lors de la modification du cuisinier");
                }

                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// supprimer un cuisinier
        public void SupprimerCuisinier()
        {
            try
            {
                AfficherCuisiniers();
                Console.Write("\nID du cuisinier à supprimer : ");
                string id = Console.ReadLine();

                string requete = "DELETE FROM UTILISATEUR WHERE id_utilisateur = @id";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@id", id);

                int resultat = commande.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("Cuisinier supprimé avec succès");
                }
                else
                {
                    Console.WriteLine("Erreur lors de la suppression du cuisinier");
                }

                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        /// ajouter une commande
        public void AjouterCommande()
        {
            try
            {
                AfficherClients();
                Console.Write("\nID du client : ");
                string idClient = Console.ReadLine();

                AfficherCuisiniers();
                Console.Write("ID du cuisinier : ");
                string idCuisinier = Console.ReadLine();

                // Récupérer les stations de départ et d'arrivée
                Console.Write("ID de la station de départ : ");
                int stationDepart = Convert.ToInt32(Console.ReadLine());
                Console.Write("ID de la station d'arrivée : ");
                int stationArrivee = Convert.ToInt32(Console.ReadLine());

                // Calculer le chemin avec l'algorithme choisi
                AlgorithmeChemin algo = new AlgorithmeChemin();
                algo.AfficherChemin(stationDepart, stationArrivee);

                string requete = "INSERT INTO COMMANDE (id_client, date_commande, montant_total) " +
                               "VALUES (@idClient, NOW(), 0)";

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.ExecuteNonQuery();

                int idCommande = (int)commande.LastInsertedId;

                requete = "INSERT INTO cuisnier_livre_commande (id_utilisateur, id_commande) " +
                         "VALUES (@idCuisinier, @idCommande)";

                commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.ExecuteNonQuery();

                Console.WriteLine("Commande ajoutée avec succès");
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }
    }
} 