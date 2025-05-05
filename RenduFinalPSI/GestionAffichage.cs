using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class GestionAffichage
    {
        public GestionAffichage()
        {
        }

        public void AfficherClients(int idCuisinier = 0, bool estAdmin = false)
        {
            Console.WriteLine("\n=== Affichage des clients ===");
            Console.WriteLine("1. Par ordre alphabétique");
            Console.WriteLine("2. Par rue");
            Console.WriteLine("3. Par montant des achats");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            string requete = "";
            if (estAdmin)
            {
                requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.adresse, " +
                         "COALESCE(SUM(c.montant_total), 0) as montant_total " +
                         "FROM UTILISATEUR u " +
                         "JOIN CLIENT cl ON u.id_utilisateur = cl.id_utilisateur " +
                         "LEFT JOIN COMMANDE c ON cl.id_utilisateur = c.id_client " +
                         "GROUP BY u.id_utilisateur, u.nom, u.prenom, u.adresse ";
            }
            else
            {
                requete = "SELECT DISTINCT u.id_utilisateur, u.nom, u.prenom, u.adresse, " +
                         "COALESCE(SUM(c.montant_total), 0) as montant_total " +
                         "FROM UTILISATEUR u " +
                         "JOIN CLIENT cl ON u.id_utilisateur = cl.id_utilisateur " +
                         "JOIN COMMANDE c ON cl.id_utilisateur = c.id_client " +
                         "JOIN contient ct ON c.id_commande = ct.id_commande AND c.id_client = ct.id_client " +
                         "JOIN PLAT p ON ct.id_plat = p.id_plat AND ct.id_cuisinier = p.id_cuisinier " +
                         "WHERE p.id_cuisinier = @idCuisinier " +
                         "GROUP BY u.id_utilisateur, u.nom, u.prenom, u.adresse ";
            }

            switch (choix)
            {
                case "1":
                    requete += "ORDER BY u.nom, u.prenom";
                    break;
                case "2":
                    requete += "ORDER BY u.adresse";
                    break;
                case "3":
                    requete += "ORDER BY montant_total DESC";
                    break;
                default:
                    Console.WriteLine("Choix invalide");
                    return;
            }

            try
            {
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                if (!estAdmin)
                {
                    commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                }

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nListe des clients :");
                Console.WriteLine("ID | Nom | Prénom | Adresse | Montant total des achats");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    int id = reader.GetInt32("id_utilisateur");
                    string nom = reader.GetString("nom");
                    string prenom = reader.GetString("prenom");
                    string adresse = reader.GetString("adresse");
                    decimal montant = reader.GetDecimal("montant_total");

                    Console.WriteLine(id + " | " + nom + " | " + prenom + " | " + adresse + " | " + montant + " euros");
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage des clients : " + ex.Message);
            }
        }

        public void AfficherCuisiniers()
        {
            Console.WriteLine("\n=== Affichage des cuisiniers ===");
            Console.WriteLine("1. Par ordre alphabétique");
            Console.WriteLine("2. Par rue");
            Console.WriteLine("3. Par nombre de commandes");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            string requete = "SELECT u.id_utilisateur, u.nom, u.prenom, u.adresse, " +
                           "COUNT(DISTINCT c.id_commande) as nombre_commandes " +
                           "FROM UTILISATEUR u " +
                           "JOIN cuisinier cu ON u.id_utilisateur = cu.id_utilisateur " +
                           "LEFT JOIN PLAT p ON cu.id_utilisateur = p.id_cuisinier " +
                           "LEFT JOIN contient ct ON p.id_plat = ct.id_plat AND p.id_cuisinier = ct.id_cuisinier " +
                           "LEFT JOIN COMMANDE c ON ct.id_commande = c.id_commande AND ct.id_client = c.id_client " +
                           "GROUP BY u.id_utilisateur, u.nom, u.prenom, u.adresse ";

            switch (choix)
            {
                case "1":
                    requete += "ORDER BY u.nom, u.prenom";
                    break;
                case "2":
                    requete += "ORDER BY u.adresse";
                    break;
                case "3":
                    requete += "ORDER BY nombre_commandes DESC";
                    break;
                default:
                    Console.WriteLine("Choix invalide");
                    return;
            }

            try
            {
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nListe des cuisiniers :");
                Console.WriteLine("ID | Nom | Prénom | Adresse | Nombre de commandes");
                Console.WriteLine("--------------------------------------------------");

                while (reader.Read())
                {
                    int id = reader.GetInt32("id_utilisateur");
                    string nom = reader.GetString("nom");
                    string prenom = reader.GetString("prenom");
                    string adresse = reader.GetString("adresse");
                    int nbCommandes = reader.GetInt32("nombre_commandes");

                    Console.WriteLine(id + " | " + nom + " | " + prenom + " | " + adresse + " | " + nbCommandes);
                }

                reader.Close();
                commande.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'affichage des cuisiniers : " + ex.Message);
            }
        }
    }
} 