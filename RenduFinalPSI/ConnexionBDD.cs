using MySql.Data.MySqlClient;
using System;

namespace RenduFinalPSI
{
    public class ConnexionBDD
    {
        private static string connectionString = "Server=localhost;Database=live_in_paris;User ID=root;Password=Microbe44;";
        private static MySqlConnection connection = null;
        private static string typeUtilisateur = "";
        private static int idUtilisateur = 0;

        public static MySqlConnection GetConnection()
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
            }
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
            return connection;
        }

        public static bool SeConnecter(string email, string mdp)
        {
            bool connecte = false;
            int tentatives = 0;
            const int MAX_TENTATIVES = 3;

            while (!connecte && tentatives < MAX_TENTATIVES)
            {
                try
                {
                    connection = new MySqlConnection(connectionString);
                    connection.Open();

                    string requete = "SELECT id_utilisateur, type_utilisateur FROM UTILISATEUR WHERE email = @email AND mot_de_passe = @mdp";
                    MySqlCommand commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@email", email);
                    commande.Parameters.AddWithValue("@mdp", mdp);

                    MySqlDataReader reader = commande.ExecuteReader();
                    if (reader.Read())
                    {
                        idUtilisateur = reader.GetInt32("id_utilisateur");
                        typeUtilisateur = reader.GetString("type_utilisateur");
                        connecte = true;
                        Console.WriteLine("\nConnexion réussie !");
                    }
                    else
                    {
                        tentatives++;
                        if (tentatives < MAX_TENTATIVES)
                        {
                            Console.WriteLine("\nEmail ou mot de passe incorrect. Il vous reste " + (MAX_TENTATIVES - tentatives) + " tentative(s).");
                            Console.Write("Email : ");
                            email = Console.ReadLine();
                            Console.Write("Mot de passe : ");
                            mdp = Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("\nNombre maximum de tentatives atteint. Veuillez réessayer plus tard.");
                        }
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
                    tentatives++;
                    if (tentatives < MAX_TENTATIVES)
                    {
                        Console.WriteLine("\nVoulez-vous réessayer ? (O/N)");
                        string reponse = Console.ReadLine().ToUpper();
                        if (reponse != "O")
                        {
                            break;
                        }
                        Console.Write("Email : ");
                        email = Console.ReadLine();
                        Console.Write("Mot de passe : ");
                        mdp = Console.ReadLine();
                    }
                }
            }

            if (!connecte)
            {
                connection?.Close();
                connection = null;
            }

            return connecte;
        }

        public static void SeDeconnecter()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
            connection = null;
            typeUtilisateur = "";
            idUtilisateur = 0;
        }

        public static string GetTypeUtilisateur()
        {
            return typeUtilisateur;
        }

        public static int GetIdUtilisateur()
        {
            return idUtilisateur;
        }

        public static bool CreerCompte(string nom, string prenom, string email, string motDePasse, string type, string adresse = "", string telephone = "", string specialite = "", string description = "")
        {
            try
            {
                if (connection == null)
                {
                    connection = new MySqlConnection(connectionString);
                    connection.Open();
                }

                string requeteVerif = "SELECT COUNT(*) FROM UTILISATEUR WHERE email = @email";
                MySqlCommand commandeVerif = new MySqlCommand(requeteVerif, connection);
                commandeVerif.Parameters.AddWithValue("@email", email);
                int count = Convert.ToInt32(commandeVerif.ExecuteScalar());

                if (count > 0)
                {
                    Console.WriteLine("Cet email est déjà utilisé.");
                    return false;
                }

                string requete = "INSERT INTO UTILISATEUR (prenom, nom, email, mot_de_passe, type_utilisateur, adresse, telephone) VALUES (@prenom, @nom, @email, @motDePasse, @type, @adresse, @telephone)";
                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@prenom", prenom);
                commande.Parameters.AddWithValue("@nom", nom);
                commande.Parameters.AddWithValue("@email", email);
                commande.Parameters.AddWithValue("@motDePasse", motDePasse);
                commande.Parameters.AddWithValue("@type", type);
                commande.Parameters.AddWithValue("@adresse", adresse);
                commande.Parameters.AddWithValue("@telephone", telephone);
                commande.ExecuteNonQuery();
                commande.CommandText = "SELECT LAST_INSERT_ID()";
                int idUtilisateur = Convert.ToInt32(commande.ExecuteScalar());

                // Associer une station à l'utilisateur
                if (!string.IsNullOrEmpty(adresse))
                {
                    GestionStationUtilisateur gestionStation = new GestionStationUtilisateur();
                    int idStation = gestionStation.TrouverStationPlusProche(adresse);
                    if (idStation > 0)
                    {
                        gestionStation.AssocierStationUtilisateur(idUtilisateur, idStation);
                    }
                }

                if (type == "cuisinier")
                {
                    requete = "INSERT INTO CUISINIER (id_utilisateur, note_moyenne) VALUES (@idUtilisateur, 0)";
                    commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);
                    commande.ExecuteNonQuery();
                }
                else if (type == "client")
                {
                    requete = "INSERT INTO CLIENT (id_utilisateur, type_client) VALUES (@idUtilisateur, 'Particulier')";
                    commande = new MySqlCommand(requete, connection);
                    commande.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);
                    commande.ExecuteNonQuery();
                }

                typeUtilisateur = type;
                ConnexionBDD.idUtilisateur = idUtilisateur;
                Console.WriteLine("Compte créé avec le type : " + type);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la création du compte : " + ex.Message);
                return false;
            }
        }
    }
} 