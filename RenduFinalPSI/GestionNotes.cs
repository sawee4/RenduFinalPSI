using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class GestionNotes
    {
        public GestionNotes()
        {
            // constructeur vide car on utilise la méthode statique
        }

        // ajouter une note pour un cuisinier
        public void AjouterNote(int idCuisinier, int noteQualite, int notePonctualite, string commentaire)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "INSERT INTO NOTES (id_cuisinier, note_qualite, note_ponctualite, commentaire, date_note) " +
                               "VALUES (@idCuisinier, @noteQualite, @notePonctualite, @commentaire, @date)";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@noteQualite", noteQualite);
                commande.Parameters.AddWithValue("@notePonctualite", notePonctualite);
                commande.Parameters.AddWithValue("@commentaire", commentaire);
                commande.Parameters.AddWithValue("@date", DateTime.Now);

                commande.ExecuteNonQuery();
            }
        }

        // obtenir la moyenne des notes d'un cuisinier
        public double GetMoyenneNotes(int idCuisinier)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "SELECT AVG((note_qualite + note_ponctualite) / 2) as moyenne " +
                               "FROM NOTES WHERE id_cuisinier = @idCuisinier";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                object resultat = commande.ExecuteScalar();
                return resultat == DBNull.Value ? 0 : Convert.ToDouble(resultat);
            }
        }

        // afficher toutes les notes d'un cuisinier
        public void AfficherNotesCuisinier(int idCuisinier)
        {
            using (var conn = ConnexionBDD.GetConnection())
            {
                string requete = "SELECT note_qualite, note_ponctualite, commentaire, date_note " +
                               "FROM NOTES WHERE id_cuisinier = @idCuisinier " +
                               "ORDER BY date_note DESC";

                MySqlCommand commande = new MySqlCommand(requete, conn);
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);

                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nNotes du cuisinier " + idCuisinier + " :");
                Console.WriteLine("----------------------------------------");

                while (reader.Read())
                {
                    Console.WriteLine("Date : " + reader["date_note"]);
                    Console.WriteLine("Note qualité : " + reader["note_qualite"]);
                    Console.WriteLine("Note ponctualité : " + reader["note_ponctualite"]);
                    Console.WriteLine("Commentaire : " + reader["commentaire"]);
                    Console.WriteLine("----------------------------------------");
                }

                reader.Close();
            }
        }
    }
} 