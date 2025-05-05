using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class MenuDuJour
    {
        public MenuDuJour()
        {
            // constructeur vide car on utilise la méthode statique
        }

        // obtenir les plats populaires de la saison
        public void AfficherPlatsPopulairesSaison()
        {
            string saison = GetSaisonActuelle();
            
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "SELECT p.nom, p.prix, COUNT(c.id_commande) as nb_commandes " +
                               "FROM PLAT p " +
                               "LEFT JOIN COMMANDE c ON p.id_plat = c.id_plat " +
                               "WHERE p.saison = @saison " +
                               "GROUP BY p.id_plat " +
                               "ORDER BY nb_commandes DESC " +
                               "LIMIT 5";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@saison", saison);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nPlats populaires de la saison (" + saison + ") :");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine("Plat : " + reader["nom"]);
                    Console.WriteLine("Prix : " + reader["prix"] + "€");
                    Console.WriteLine("Nombre de commandes : " + reader["nb_commandes"]);
                    Console.WriteLine("----------------------------------------");
                }

                reader.Close();
            }
        }

        // obtenir les ingrédients disponibles pour un cuisinier
        public void AfficherIngredientsDisponibles(int idCuisinier)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "SELECT i.nom, s.quantite " +
                               "FROM INGREDIENT i " +
                               "JOIN STOCK s ON i.id_ingredient = s.id_ingredient " +
                               "WHERE s.id_cuisinier = @idCuisinier AND s.quantite > 0";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nIngrédients disponibles :");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine(reader["nom"] + " : " + reader["quantite"]);
                }

                reader.Close();
            }
        }

        // obtenir la saison actuelle
        private string GetSaisonActuelle()
        {
            int mois = DateTime.Now.Month;
            
            if (mois >= 3 && mois <= 5)
                return "Printemps";
            else if (mois >= 6 && mois <= 8)
                return "Été";
            else if (mois >= 9 && mois <= 11)
                return "Automne";
            else
                return "Hiver";
        }
    }
} 