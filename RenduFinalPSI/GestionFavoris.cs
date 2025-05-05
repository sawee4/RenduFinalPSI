using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class GestionFavoris
    {
        public GestionFavoris()
        {
            // constructeur vide car on utilise la méthode statique
        }

        // ajouter un plat aux favoris
        public void AjouterFavori(int idClient, int idPlat)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "INSERT INTO FAVORIS (id_client, id_plat, date_ajout) " +
                               "VALUES (@idClient, @idPlat, @date)";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.Parameters.AddWithValue("@idPlat", idPlat);
                commande.Parameters.AddWithValue("@date", DateTime.Now);

                commande.ExecuteNonQuery();
            }
        }

        // supprimer un plat des favoris
        public void SupprimerFavori(int idClient, int idPlat)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "DELETE FROM FAVORIS WHERE id_client = @idClient AND id_plat = @idPlat";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idClient", idClient);
                commande.Parameters.AddWithValue("@idPlat", idPlat);

                commande.ExecuteNonQuery();
            }
        }

        // afficher les favoris d'un client
        public void AfficherFavoris(int idClient)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "SELECT p.nom as nom_plat, p.prix, c.nom as nom_cuisinier " +
                               "FROM FAVORIS f " +
                               "JOIN PLAT p ON f.id_plat = p.id_plat " +
                               "JOIN UTILISATEUR c ON p.id_cuisinier = c.id_utilisateur " +
                               "WHERE f.id_client = @idClient " +
                               "ORDER BY f.date_ajout DESC";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idClient", idClient);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nPlats favoris du client " + idClient + " :");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine("Plat : " + reader["nom_plat"]);
                    Console.WriteLine("Prix : " + reader["prix"] + "€");
                    Console.WriteLine("Cuisinier : " + reader["nom_cuisinier"]);
                    Console.WriteLine("----------------------------------------");
                }

                reader.Close();
            }
        }
    }
} 