using System;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class GestionUtilisateur
    {
        private int idUtilisateur;
        private bool estCuisinier;

        public GestionUtilisateur(int id, bool estCuisinier)
        {
            this.idUtilisateur = id;
            this.estCuisinier = estCuisinier;
        }

        public void MenuPrincipal()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.WriteLine("\nMenu Principal " + (estCuisinier ? "Cuisinier" : "Client"));
                Console.WriteLine("1. Voir mes informations");
                Console.WriteLine("2. Modifier mes informations");
                Console.WriteLine("3. " + (estCuisinier ? "Voir mes commandes à livrer" : "Passer une commande"));
                Console.WriteLine("4. Voir l'historique des commandes");
                Console.WriteLine("5. Calculer le plus court chemin");
                Console.WriteLine("0. Déconnexion");

                Console.Write("\nChoix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        AfficherInformations();
                        break;
                    case "2":
                        ModifierInformations();
                        break;
                    case "3":
                        if (estCuisinier)
                            VoirCommandesALivrer();
                        else
                            PasserCommande();
                        break;
                    case "4":
                        VoirHistoriqueCommandes();
                        break;
                    case "5":
                        CalculerPlusCourt();
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

        private void AfficherInformations()
        {
            try
            {
                string requete = "SELECT * FROM UTILISATEUR WHERE id_utilisateur = @id";
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@id", idUtilisateur);
                MySqlDataReader reader = commande.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine("\n=== Mes informations ===");
                    Console.WriteLine("Nom : " + reader["nom"]);
                    Console.WriteLine("Prénom : " + reader["prenom"]);
                    Console.WriteLine("Adresse : " + reader["adresse"]);
                    Console.WriteLine("Email : " + reader["email"]);
                    if (estCuisinier)
                    {
                        reader.Close();
                        commande = new MySqlCommand("SELECT specialite FROM CUISINIER WHERE id_utilisateur = @id", 
                                                  ConnexionBDD.GetConnection());
                        commande.Parameters.AddWithValue("@id", idUtilisateur);
                        reader = commande.ExecuteReader();
                        if (reader.Read())
                        {
                            Console.WriteLine("Spécialité : " + reader["specialite"]);
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void ModifierInformations()
        {
            try
            {
                AfficherInformations();
                Console.WriteLine("\nQuels champs souhaitez-vous modifier ? (séparés par des virgules, ex: 1,3)");
                Console.WriteLine("1. Nom");
                Console.WriteLine("2. Prénom");
                Console.WriteLine("3. Adresse");
                Console.WriteLine("4. Email");
                Console.WriteLine("5. Mot de passe");
                if (estCuisinier)
                    Console.WriteLine("6. Spécialité");
                Console.WriteLine("0. Tout modifier");

                Console.Write("\nVotre choix : ");
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
                if (estCuisinier && (toutModifier || choix.Contains("6")))
                {
                    Console.Write("Nouvelle spécialité : ");
                    specialite = Console.ReadLine();
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
                commande.Parameters.AddWithValue("@id", idUtilisateur);
                if (toutModifier || choix.Contains("1")) commande.Parameters.AddWithValue("@nom", nom);
                if (toutModifier || choix.Contains("2")) commande.Parameters.AddWithValue("@prenom", prenom);
                if (toutModifier || choix.Contains("3")) commande.Parameters.AddWithValue("@adresse", adresse);
                if (toutModifier || choix.Contains("4")) commande.Parameters.AddWithValue("@email", email);
                if (toutModifier || choix.Contains("5")) commande.Parameters.AddWithValue("@mdp", mdp);

                int resultat = commande.ExecuteNonQuery();
                if (resultat > 0)
                {
                    Console.WriteLine("Informations modifiées avec succès");
                }

                if (estCuisinier && (toutModifier || choix.Contains("6")))
                {
                    requete = "UPDATE CUISINIER SET specialite = @specialite WHERE id_utilisateur = @id";
                    commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                    commande.Parameters.AddWithValue("@id", idUtilisateur);
                    commande.Parameters.AddWithValue("@specialite", specialite);
                    commande.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void PasserCommande()
        {
            try
            {
                Console.WriteLine("\n=== Passer une commande ===");
                
                // Afficher les cuisiniers disponibles
                string requete = "SELECT u.id_utilisateur, u.nom, u.prenom, c.specialite " +
                               "FROM UTILISATEUR u JOIN CUISINIER c ON u.id_utilisateur = c.id_utilisateur";
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\nListe des cuisiniers :");
                while (reader.Read())
                {
                    Console.WriteLine("ID : " + reader["id_utilisateur"] + " - " + 
                                    reader["prenom"] + " " + reader["nom"] + 
                                    " (Spécialité : " + reader["specialite"] + ")");
                }
                reader.Close();

                Console.Write("\nID du cuisinier : ");
                string idCuisinier = Console.ReadLine();

                // Récupérer les stations de départ et d'arrivée
                Console.Write("ID de la station de départ : ");
                int stationDepart = Convert.ToInt32(Console.ReadLine());
                Console.Write("ID de la station d'arrivée : ");
                int stationArrivee = Convert.ToInt32(Console.ReadLine());

                // Choix de l'algorithme
                Console.WriteLine("\nChoisissez l'algorithme pour trouver le plus court chemin :");
                Console.WriteLine("1. Dijkstra");
                Console.WriteLine("2. Bellman-Ford");
                Console.WriteLine("3. Floyd-Warshall");
                Console.Write("\nVotre choix : ");
                string choixAlgo = Console.ReadLine();

                // Calculer le chemin avec l'algorithme choisi
                AlgorithmeChemin algo = new AlgorithmeChemin();
                List<Station> chemin = null;
                string nomAlgo = "";

                switch (choixAlgo)
                {
                    case "1":
                        chemin = algo.Dijkstra(stationDepart, stationArrivee);
                        nomAlgo = "Dijkstra";
                        break;
                    case "2":
                        chemin = algo.BellmanFord(stationDepart, stationArrivee);
                        nomAlgo = "Bellman-Ford";
                        break;
                    case "3":
                        chemin = algo.FloydWarshall(stationDepart, stationArrivee);
                        nomAlgo = "Floyd-Warshall";
                        break;
                    default:
                        Console.WriteLine("Choix invalide");
                        return;
                }

                if (chemin == null || chemin.Count == 0)
                {
                    Console.WriteLine("Aucun chemin trouvé avec l'algorithme " + nomAlgo);
                    return;
                }

                Console.WriteLine("\n=== Chemin de livraison (algorithme " + nomAlgo + ") ===");
                for (int i = 0; i < chemin.Count; i++)
                {
                    Console.WriteLine("Station " + (i + 1) + " : " + chemin[i].Nom + " (Ligne " + chemin[i].Ligne + ")");
                }
                Console.WriteLine("Nombre total de stations : " + chemin.Count);

                // Créer la commande
                requete = "INSERT INTO COMMANDE (id_client, date_commande, montant_total) " +
                         "VALUES (@idClient, NOW(), 0)";
                commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idClient", idUtilisateur);
                commande.ExecuteNonQuery();

                int idCommande = (int)commande.LastInsertedId;

                requete = "INSERT INTO cuisnier_livre_commande (id_utilisateur, id_commande) " +
                         "VALUES (@idCuisinier, @idCommande)";
                commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
                commande.Parameters.AddWithValue("@idCommande", idCommande);
                commande.ExecuteNonQuery();

                Console.WriteLine("Commande passée avec succès");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void VoirCommandesALivrer()
        {
            try
            {
                string requete = "SELECT c.*, u.nom as nom_client, u.prenom as prenom_client " +
                               "FROM COMMANDE c " +
                               "JOIN UTILISATEUR u ON c.id_client = u.id_utilisateur " +
                               "JOIN cuisnier_livre_commande clc ON c.id_commande = clc.id_commande " +
                               "WHERE clc.id_utilisateur = @id";
                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@id", idUtilisateur);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Commandes à livrer ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    Console.WriteLine("Client : " + reader["prenom_client"] + " " + reader["nom_client"]);
                    Console.WriteLine("Date : " + reader["date_commande"]);
                    Console.WriteLine("Montant : " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        private void VoirHistoriqueCommandes()
        {
            try
            {
                string requete;
                if (estCuisinier)
                {
                    requete = "SELECT c.*, u.nom as nom_client, u.prenom as prenom_client " +
                            "FROM COMMANDE c " +
                            "JOIN UTILISATEUR u ON c.id_client = u.id_utilisateur " +
                            "JOIN cuisnier_livre_commande clc ON c.id_commande = clc.id_commande " +
                            "WHERE clc.id_utilisateur = @id";
                }
                else
                {
                    requete = "SELECT c.*, u.nom as nom_cuisinier, u.prenom as prenom_cuisinier " +
                            "FROM COMMANDE c " +
                            "JOIN cuisnier_livre_commande clc ON c.id_commande = clc.id_commande " +
                            "JOIN UTILISATEUR u ON clc.id_utilisateur = u.id_utilisateur " +
                            "WHERE c.id_client = @id";
                }

                MySqlCommand commande = new MySqlCommand(requete, ConnexionBDD.GetConnection());
                commande.Parameters.AddWithValue("@id", idUtilisateur);
                MySqlDataReader reader = commande.ExecuteReader();

                Console.WriteLine("\n=== Historique des commandes ===");
                while (reader.Read())
                {
                    Console.WriteLine("Commande n°" + reader["id_commande"]);
                    if (estCuisinier)
                    {
                        Console.WriteLine("Client : " + reader["prenom_client"] + " " + reader["nom_client"]);
                    }
                    else
                    {
                        Console.WriteLine("Cuisinier : " + reader["prenom_cuisinier"] + " " + reader["nom_cuisinier"]);
                    }
                    Console.WriteLine("Date : " + reader["date_commande"]);
                    Console.WriteLine("Montant : " + reader["montant_total"] + " euros");
                    Console.WriteLine("-------------------");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }

        // Nouvelle méthode pour calculer et afficher le plus court chemin
        private void CalculerPlusCourt()
        {
            try
            {
                Console.WriteLine("\n=== Calculer le plus court chemin ===");
                
                // Afficher les stations disponibles
                Console.WriteLine("\nRécupération des stations...");
                MySqlConnection connection = ConnexionBDD.GetConnection();
                
                string requeteStations = "SELECT id_station, nom, ligne FROM STATION ORDER BY ligne, nom";
                MySqlCommand commandeStations = new MySqlCommand(requeteStations, connection);
                MySqlDataReader reader = commandeStations.ExecuteReader();

                Console.WriteLine("\nListe des stations :");
                while (reader.Read())
                {
                    Console.WriteLine("ID: " + reader["id_station"] + " - " + 
                                     reader["nom"] + " (Ligne " + reader["ligne"] + ")");
                }
                reader.Close();
                
                // Récupérer les stations de départ et d'arrivée
                Console.Write("\nID de la station de départ : ");
                int stationDepart = Convert.ToInt32(Console.ReadLine());
                Console.Write("ID de la station d'arrivée : ");
                int stationArrivee = Convert.ToInt32(Console.ReadLine());

                // Choix de l'algorithme
                Console.WriteLine("\nChoisissez l'algorithme pour trouver le plus court chemin :");
                Console.WriteLine("1. Dijkstra");
                Console.WriteLine("2. Bellman-Ford");
                Console.WriteLine("3. Floyd-Warshall");
                Console.Write("\nVotre choix : ");
                string choixAlgo = Console.ReadLine();

                // Calculer le chemin avec l'algorithme choisi
                AlgorithmeChemin algo = new AlgorithmeChemin();
                List<Station> chemin = null;
                string nomAlgo = "";

                switch (choixAlgo)
                {
                    case "1":
                        chemin = algo.Dijkstra(stationDepart, stationArrivee);
                        nomAlgo = "Dijkstra";
                        break;
                    case "2":
                        chemin = algo.BellmanFord(stationDepart, stationArrivee);
                        nomAlgo = "Bellman-Ford";
                        break;
                    case "3":
                        chemin = algo.FloydWarshall(stationDepart, stationArrivee);
                        nomAlgo = "Floyd-Warshall";
                        break;
                    default:
                        Console.WriteLine("Choix invalide");
                        return;
                }

                if (chemin == null || chemin.Count == 0)
                {
                    Console.WriteLine("Aucun chemin trouvé avec l'algorithme " + nomAlgo);
                    return;
                }

                Console.WriteLine("\n=== Chemin le plus court (algorithme " + nomAlgo + ") ===");
                for (int i = 0; i < chemin.Count; i++)
                {
                    Console.WriteLine("Station " + (i + 1) + " : " + chemin[i].Nom + " (Ligne " + chemin[i].Ligne + ")");
                }
                Console.WriteLine("Nombre total de stations : " + chemin.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }
    }
} 