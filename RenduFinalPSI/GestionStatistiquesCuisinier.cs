using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class GestionStatistiquesCuisinier
    {
        public void AfficherMenuStatistiques(int idCuisinier)
        {
            bool continuer = true;
            while (continuer)
            {
                Console.WriteLine("\n=== Menu Statistiques ===");
                Console.WriteLine("1. Afficher les clients servis");
                Console.WriteLine("2. Afficher les plats par fréquence");
                Console.WriteLine("3. Afficher/modifier le plat du jour");
                Console.WriteLine("4. Modifier un plat");
                Console.WriteLine("5. Retour au menu principal");
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AfficherClientsServis(idCuisinier);
                        break;
                    case "2":
                        AfficherPlatsParFrequence(idCuisinier);
                        break;
                    case "3":
                        GererPlatDuJour(idCuisinier);
                        break;
                    case "4":
                        ModifierPlat(idCuisinier);
                        break;
                    case "5":
                        continuer = false;
                        break;
                    default:
                        Console.WriteLine("Choix invalide");
                        break;
                }
            }
        }

        private void AfficherClientsServis(int idCuisinier)
        {
            Console.WriteLine("\n=== Affichage des clients servis ===");
            Console.WriteLine("1. Tous les clients");
            Console.WriteLine("2. Sur une période spécifique");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            string requete = "";
            string dateDebut = "";
            string dateFin = "";

            if (choix == "1")
            {
                requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.adresse, " +
                         "COUNT(DISTINCT c.id_commande) as nb_commandes, " +
                         "SUM(c.montant_total) as montant_total " +
                         "FROM UTILISATEUR u " +
                         "JOIN CLIENT cl ON u.id_utilisateur = cl.id_utilisateur " +
                         "JOIN COMMANDE c ON cl.id_utilisateur = c.id_client " +
                         "JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +
                         "JOIN PLAT p ON ct.id_plat = p.id_plat AND ct.id_cuisinier = p.id_cuisinier " +
                         "WHERE p.id_cuisinier = @idCuisinier " +
                         "GROUP BY u.id_utilisateur, u.nom, u.prenom, u.adresse " +
                         "ORDER BY nb_commandes DESC";
            }
            else if (choix == "2")
            {
                Console.Write("Date de début (YYYY-MM-DD) : ");
                dateDebut = Console.ReadLine();
                Console.Write("Date de fin (YYYY-MM-DD) : ");
                dateFin = Console.ReadLine();

                requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.adresse, " +
                         "COUNT(DISTINCT c.id_commande) as nb_commandes, " +
                         "SUM(c.montant_total) as montant_total " +
                         "FROM UTILISATEUR u " +
                         "JOIN CLIENT cl ON u.id_utilisateur = cl.id_utilisateur " +
                         "JOIN COMMANDE c ON cl.id_utilisateur = c.id_client " +
                         "JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +
                         "JOIN PLAT p ON ct.id_plat = p.id_plat AND ct.id_cuisinier = p.id_cuisinier " +
                         "WHERE p.id_cuisinier = @idCuisinier " +
                         "AND c.date_commande BETWEEN @dateDebut AND @dateFin " +
                         "GROUP BY u.id_utilisateur, u.nom, u.prenom, u.adresse " +
                         "ORDER BY nb_commandes DESC";
            }
            else
            {
                Console.WriteLine("Choix invalide");
                return;
            }

            try
            {
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                if (choix == "2")
                {
                    commande.Parameters.AddWithValue("@dateDebut", dateDebut);
                    commande.Parameters.AddWithValue("@dateFin", dateFin);
                }

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nListe des clients servis :");
                Console.WriteLine("ID | Nom | Prénom | Adresse | Nombre de commandes | Montant total");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    int id = reader.GetInt32("id_utilisateur");
                    string nom = reader.GetString("nom");
                    string prenom = reader.GetString("prenom");
                    string adresse = reader.GetString("adresse");
                    int nbCommandes = reader.GetInt32("nb_commandes");
                    decimal montant = reader.GetDecimal("montant_total");

                    Console.WriteLine(id + " | " + nom + " | " + prenom + " | " + adresse + " | " + nbCommandes + " | " + montant + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage des clients : " + ex.Message);
            }
        }

        private void AfficherPlatsParFrequence(int idCuisinier)
        {
            string requete = "SELECT p.id_plat, p.nom, p.type, p.prix_personne, " +
                           "COUNT(DISTINCT c.id_commande) as nb_commandes, " +
                           "SUM(ct.quantite * ct.prix_unitaire) as montant_total " +
                           "FROM PLAT p " +
                           "LEFT JOIN contient ct ON p.id_plat = ct.id_plat AND p.id_cuisinier = ct.id_cuisinier " +
                           "LEFT JOIN COMMANDE c ON ct.id_commande = c.id_commande AND ct.id_client = c.id_client " +
                           "WHERE p.id_cuisinier = @idCuisinier " +
                           "GROUP BY p.id_plat, p.nom, p.type, p.prix_personne " +
                           "ORDER BY nb_commandes DESC";

            try
            {
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nListe des plats par fréquence :");
                Console.WriteLine("ID | Nom | Type | Prix unitaire | Nombre de commandes | Montant total");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    int id = reader.GetInt32("id_plat");
                    string nom = reader.GetString("nom");
                    string type = reader.GetString("type");
                    decimal prix = reader.GetDecimal("prix_personne");
                    int nbCommandes = reader.GetInt32("nb_commandes");
                    decimal montant = reader.GetDecimal("montant_total");

                    Console.WriteLine(id + " | " + nom + " | " + type + " | " + prix + " euros | " + nbCommandes + " | " + montant + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage des plats : " + ex.Message);
            }
        }

        private void GererPlatDuJour(int idCuisinier)
        {
            Console.WriteLine("\n=== Gestion du plat du jour ===");
            Console.WriteLine("1. Afficher le plat du jour actuel");
            Console.WriteLine("2. Changer le plat du jour");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            if (choix == "1")
            {
                AfficherPlatDuJour(idCuisinier);
            }
            else if (choix == "2")
            {
                ChangerPlatDuJour(idCuisinier);
            }
            else
            {
                Console.WriteLine("Choix invalide");
            }
        }

        private void AfficherPlatDuJour(int idCuisinier)
        {
            string requete = "SELECT p.id_plat, p.nom, p.type, p.prix_personne, p.ingredients " +
                           "FROM PLAT p " +
                           "WHERE p.id_cuisinier = @idCuisinier AND p.plat_du_jour = TRUE " +
                           "LIMIT 1";

            try
            {
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\nPlat du jour actuel :");
                    Console.WriteLine("Nom : " + reader["nom"]);
                    Console.WriteLine("Type : " + reader["type"]);
                    Console.WriteLine("Prix : " + reader["prix_personne"] + " euros");
                    Console.WriteLine("Ingrédients : " + reader["ingredients"]);
                }
                else
                {
                    Console.WriteLine("\nAucun plat du jour n'est défini.");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage du plat du jour : " + ex.Message);
            }
        }

        private void ChangerPlatDuJour(int idCuisinier)
        {
            try
            {
                // Afficher la liste des plats du cuisinier
                string requeteListe = "SELECT p.id_plat, p.nom, p.type, p.prix_personne, p.ingredients, p.nationalite, p.regime_alimentaire, p.nb_personnes " +
                                    "FROM PLAT p " +
                                    "WHERE p.id_cuisinier = @idCuisinier " +
                                    "ORDER BY p.nom";
                
                MySqlCommand commandeListe = new MySqlCommand(requeteListe, ConnexionBDD.GetConnection());
                commandeListe.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                
                MySqlDataReader reader = commandeListe.ExecuteReader();
                
                Console.WriteLine("\nListe de vos plats :");
                Console.WriteLine("ID | Nom | Type | Prix | Personnes");
                Console.WriteLine("----------------------------------");
                
                List<int> idsPlats = new List<int>();
                while (reader.Read())
                {
                    int id = reader.GetInt32("id_plat");
                    idsPlats.Add(id);
                    Console.WriteLine(id + " | " + reader["nom"] + " | " + reader["type"] + " | " + reader["prix_personne"] + " euros | " + reader["nb_personnes"]);
                }
                
                reader.Close();
                commandeListe.Dispose();
                
                if (idsPlats.Count == 0)
                {
                    Console.WriteLine("\nVous n'avez pas encore de plats dans votre menu.");
                    return;
                }
                
                // Demander le choix du plat
                Console.Write("\nEntrez l'ID du plat que vous voulez mettre en plat du jour : ");
                string choix = Console.ReadLine();
                
                if (int.TryParse(choix, out int idPlatChoisi) && idsPlats.Contains(idPlatChoisi))
                {
                    // D'abord, réinitialiser tous les plats du jour du cuisinier
                    string requeteReset = "UPDATE PLAT SET plat_du_jour = FALSE WHERE id_cuisinier = @idCuisinier";
                    MySqlCommand commandeReset = new MySqlCommand(requeteReset, ConnexionBDD.GetConnection());
                    commandeReset.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                    commandeReset.ExecuteNonQuery();
                    commandeReset.Dispose();
                    
                    // Mettre à jour le nouveau plat du jour
                    string requeteUpdate = "UPDATE PLAT SET plat_du_jour = TRUE WHERE id_plat = @idPlat AND id_cuisinier = @idCuisinier";
                    MySqlCommand commandeUpdate = new MySqlCommand(requeteUpdate, ConnexionBDD.GetConnection());
                    commandeUpdate.Parameters.AddWithValue("@idPlat", idPlatChoisi);
                    commandeUpdate.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                    commandeUpdate.ExecuteNonQuery();
                    commandeUpdate.Dispose();
                    
                    Console.WriteLine("\nLe plat du jour a été mis à jour avec succès !");
                    AfficherPlatDuJour(idCuisinier);
                }
                else
                {
                    Console.WriteLine("\nID de plat invalide. Veuillez choisir un ID dans la liste.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors du changement du plat du jour : " + ex.Message);
            }
        }

        private void ModifierPlat(int idCuisinier)
        {
            try
            {
                // Afficher la liste des plats du cuisinier
                string requeteListe = "SELECT p.id_plat, p.nom, p.type, p.prix_personne, p.ingredients, p.nationalite, p.regime_alimentaire, p.nb_personnes " +
                                    "FROM PLAT p " +
                                    "WHERE p.id_cuisinier = @idCuisinier " +
                                    "ORDER BY p.nom";
                
                MySqlCommand commandeListe = new MySqlCommand(requeteListe, ConnexionBDD.GetConnection());
                commandeListe.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                
                MySqlDataReader reader = commandeListe.ExecuteReader();
                
                Console.WriteLine("\nListe de vos plats :");
                Console.WriteLine("ID | Nom | Type | Prix | Personnes");
                Console.WriteLine("----------------------------------");
                
                List<int> idsPlats = new List<int>();
                while (reader.Read())
                {
                    int id = reader.GetInt32("id_plat");
                    idsPlats.Add(id);
                    Console.WriteLine(id + " | " + reader["nom"] + " | " + reader["type"] + " | " + reader["prix_personne"] + " euros | " + reader["nb_personnes"]);
                }
                
                reader.Close();
                commandeListe.Dispose();
                
                if (idsPlats.Count == 0)
                {
                    Console.WriteLine("\nVous n'avez pas encore de plats dans votre menu.");
                    return;
                }
                
                // Demander le choix du plat
                Console.Write("\nEntrez l'ID du plat que vous voulez modifier : ");
                string choix = Console.ReadLine();
                
                if (int.TryParse(choix, out int idPlatChoisi) && idsPlats.Contains(idPlatChoisi))
                {
                    // Afficher les informations actuelles du plat
                    string requetePlat = "SELECT * FROM PLAT WHERE id_plat = @idPlat AND id_cuisinier = @idCuisinier";
                    MySqlCommand commandePlat = new MySqlCommand(requetePlat, ConnexionBDD.GetConnection());
                    commandePlat.Parameters.AddWithValue("@idPlat", idPlatChoisi);
                    commandePlat.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                    
                    reader = commandePlat.ExecuteReader();
                    reader.Read();
                    
                    Console.WriteLine("\nInformations actuelles du plat :");
                    Console.WriteLine("1. Nom : " + reader["nom"]);
                    Console.WriteLine("2. Type : " + reader["type"]);
                    Console.WriteLine("3. Prix : " + reader["prix_personne"] + " euros");
                    Console.WriteLine("4. Ingrédients : " + reader["ingredients"]);
                    Console.WriteLine("5. Nationalité : " + reader["nationalite"]);
                    Console.WriteLine("6. Régime alimentaire : " + reader["regime_alimentaire"]);
                    Console.WriteLine("7. Nombre de personnes : " + reader["nb_personnes"]);
                    
                    reader.Close();
                    commandePlat.Dispose();
                    
                    // Demander ce qu'il veut modifier
                    Console.WriteLine("\nQue voulez-vous modifier ? (entrez le numéro)");
                    Console.Write("Votre choix : ");
                    string choixModif = Console.ReadLine();
                    
                    string champ = "";
                    string valeur = "";
                    
                    switch (choixModif)
                    {
                        case "1":
                            champ = "nom";
                            Console.Write("Nouveau nom : ");
                            valeur = Console.ReadLine();
                            break;
                        case "2":
                            champ = "type";
                            Console.Write("Nouveau type : ");
                            valeur = Console.ReadLine();
                            break;
                        case "3":
                            champ = "prix_personne";
                            Console.Write("Nouveau prix : ");
                            valeur = Console.ReadLine();
                            break;
                        case "4":
                            champ = "ingredients";
                            Console.Write("Nouveaux ingrédients : ");
                            valeur = Console.ReadLine();
                            break;
                        case "5":
                            champ = "nationalite";
                            Console.Write("Nouvelle nationalité : ");
                            valeur = Console.ReadLine();
                            break;
                        case "6":
                            champ = "regime_alimentaire";
                            Console.Write("Nouveau régime alimentaire : ");
                            valeur = Console.ReadLine();
                            break;
                        case "7":
                            champ = "nb_personnes";
                            Console.Write("Nouveau nombre de personnes : ");
                            valeur = Console.ReadLine();
                            break;
                        default:
                            Console.WriteLine("Choix invalide");
                            return;
                    }
                    
                    // Mettre à jour le plat
                    string requeteUpdate = "UPDATE PLAT SET " + champ + " = @valeur WHERE id_plat = @idPlat AND id_cuisinier = @idCuisinier";
                    MySqlCommand commandeUpdate = new MySqlCommand(requeteUpdate, ConnexionBDD.GetConnection());
                    commandeUpdate.Parameters.AddWithValue("@valeur", valeur);
                    commandeUpdate.Parameters.AddWithValue("@idPlat", idPlatChoisi);
                    commandeUpdate.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                    
                    int result = commandeUpdate.ExecuteNonQuery();
                    commandeUpdate.Dispose();
                    
                    if (result > 0)
                    {
                        Console.WriteLine("\nLe plat a été modifié avec succès !");
                    }
                    else
                    {
                        Console.WriteLine("\nErreur lors de la modification du plat.");
                    }
                }
                else
                {
                    Console.WriteLine("\nID de plat invalide. Veuillez choisir un ID dans la liste.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la modification du plat : " + ex.Message);
            }
        }
    }
} 