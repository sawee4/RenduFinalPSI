using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RenduFinalPSI
{
    public class GestionStationUtilisateur
    {
        private GrapheMetro grapheMetro;
        private Random random = new Random();
        
        public GestionStationUtilisateur()
        {
            grapheMetro = new GrapheMetro();
        }

        /// trouver la station la plus proche d'une adresse
        public int TrouverStationPlusProche(string adresse)
        {
            // dans un vrai projet on utiliserait une API de géocodage
            // pour simplifier on va simuler avec des coordonnées aléatoires
            double longitude = 2.3 + random.NextDouble() * 0.2; // Paris est entre 2.3 et 2.5
            double latitude = 48.8 + random.NextDouble() * 0.2; // Paris est entre 48.8 et 49.0

            Station stationPlusProche = null;
            double distanceMin = double.MaxValue;

            foreach (var station in grapheMetro.Stations)
            {
                double distance = CalculerDistance(longitude, latitude, station.Longitude, station.Latitude);
                if (distance < distanceMin)
                {
                    distanceMin = distance;
                    stationPlusProche = station;
                }
            }

            return stationPlusProche?.Id ?? 0;
        }

        /// calculer la distance entre deux points
        private double CalculerDistance(double lon1, double lat1, double lon2, double lat2)
        {
            // formule de Haversine pour calculer la distance entre deux points
            double R = 6371; // rayon de la Terre en km
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                      Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                      Math.Sin(dLon/2) * Math.Sin(dLon/2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            return R * c;
        }

        /// associer une station à un utilisateur
        public void AssocierStationUtilisateur(int idUtilisateur, int idStation)
        {
            // Ne rien faire car la table n'existe pas
            Console.WriteLine("La table station_utilisateur n'existe pas, association simulée.");
        }

        /// obtenir la station d'un utilisateur
        public Station GetStationUtilisateur(int idUtilisateur)
        {
            try
            {
                // On récupère l'id_station de l'utilisateur
                MySqlConnection connection = ConnexionBDD.GetConnection();
                string requete = "SELECT id_station FROM UTILISATEUR WHERE id_utilisateur = @idUtilisateur";
                MySqlCommand commande = new MySqlCommand(requete, connection);
                commande.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);

                Console.WriteLine("\nRecherche de la station pour l'utilisateur " + idUtilisateur);
                
                object result = commande.ExecuteScalar();
                Console.WriteLine("Résultat de la requête : " + (result?.ToString() ?? "null"));

                if (result != null && result != DBNull.Value)
                {
                    int idStation = Convert.ToInt32(result);
                    Console.WriteLine("ID station trouvé : " + idStation);
                    
                    // On cherche la station correspondante
                    Station station = grapheMetro.Stations.FirstOrDefault(s => s.Id == idStation);
                    if (station != null)
                    {
                        Console.WriteLine("Station trouvée : " + station.Nom);
                        return station;
                    }
                    else
                    {
                        Console.WriteLine("Aucune station trouvée avec l'ID " + idStation);
                    }
                }
                else
                {
                    Console.WriteLine("Aucun id_station trouvé pour l'utilisateur");
                }

                // Si on ne trouve pas de station associée, on propose d'en ajouter une
                Console.WriteLine("\nAucune station associée à votre compte.");
                Console.WriteLine("Voulez-vous associer une station à votre compte ? (O/N)");
                string reponse = Console.ReadLine().ToUpper();

                if (reponse == "O")
                {
                    Console.WriteLine("\nListe des stations disponibles :");
                    for (int i = 0; i < grapheMetro.Stations.Count; i++)
                    {
                        Console.WriteLine((i + 1) + ". " + grapheMetro.Stations[i].Nom + " (ID: " + grapheMetro.Stations[i].Id + ")");
                    }

                    Console.Write("\nChoisissez une station (numéro) : ");
                    if (int.TryParse(Console.ReadLine(), out int choix) && choix > 0 && choix <= grapheMetro.Stations.Count)
                    {
                        Station stationChoisie = grapheMetro.Stations[choix - 1];
                        
                        // Mettre à jour la base de données
                        string updateRequete = "UPDATE UTILISATEUR SET id_station = @idStation WHERE id_utilisateur = @idUtilisateur";
                        MySqlCommand updateCommande = new MySqlCommand(updateRequete, connection);
                        updateCommande.Parameters.AddWithValue("@idStation", stationChoisie.Id);
                        updateCommande.Parameters.AddWithValue("@idUtilisateur", idUtilisateur);
                        int rowsAffected = updateCommande.ExecuteNonQuery();
                        
                        Console.WriteLine("Mise à jour effectuée : " + rowsAffected + " ligne(s) modifiée(s)");
                        Console.WriteLine("\nStation " + stationChoisie.Nom + " (ID: " + stationChoisie.Id + ") associée à votre compte avec succès !");
                        return stationChoisie;
                    }
                    else
                    {
                        Console.WriteLine("\nChoix invalide, une station aléatoire sera choisie.");
                    }
                }

                // Si l'utilisateur ne veut pas choisir ou fait un mauvais choix, on prend une station aléatoire
                Station stationAleatoire = grapheMetro.Stations[random.Next(grapheMetro.Stations.Count)];
                Console.WriteLine("\nStation aléatoire choisie : " + stationAleatoire.Nom + " (ID: " + stationAleatoire.Id + ")");
                return stationAleatoire;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la récupération de la station : " + ex.Message);
                Console.WriteLine("Stack trace : " + ex.StackTrace);
                // En cas d'erreur, on retourne une station aléatoire
                return grapheMetro.Stations[random.Next(grapheMetro.Stations.Count)];
            }
        }

        /// trouver le chemin le plus court entre un client et un cuisinier
        public List<Station> TrouverCheminLivraison(int idClient, int idCuisinier)
        {
            try
            {
                // Obtenir les stations des utilisateurs
                Station stationClient = GetStationUtilisateur(idClient);
                Station stationCuisinier = GetStationUtilisateur(idCuisinier);

                if (stationClient == null || stationCuisinier == null)
                {
                    Console.WriteLine("Impossible de trouver les stations des utilisateurs.");
                    return null;
                }

                Console.WriteLine("\nRecherche du chemin entre :");
                Console.WriteLine("- Station client : " + stationClient.Nom + " (ID: " + stationClient.Id + ")");
                Console.WriteLine("- Station cuisinier : " + stationCuisinier.Nom + " (ID: " + stationCuisinier.Id + ")");

                // Demander à l'utilisateur de choisir l'algorithme
                Console.WriteLine("\nChoisissez l'algorithme pour trouver le plus court chemin :");
                Console.WriteLine("1. Dijkstra (rapide, précis pour les graphes sans poids négatifs)");
                Console.WriteLine("2. Bellman-Ford (peut gérer les poids négatifs, plus lent)");
                Console.WriteLine("3. Floyd-Warshall (calcule tous les chemins entre toutes les paires de sommets)");
                Console.Write("\nVotre choix (1-3) : ");
                
                string choix = Console.ReadLine();
                List<Station> chemin = null;
                string nomAlgo = "";
                
                switch (choix)
                {
                    case "1":
                        chemin = grapheMetro.Dijkstra(stationClient.Id, stationCuisinier.Id);
                        nomAlgo = "Dijkstra";
                        break;
                    case "2":
                        chemin = grapheMetro.BellmanFord(stationClient.Id, stationCuisinier.Id);
                        nomAlgo = "Bellman-Ford";
                        break;
                    case "3":
                        chemin = grapheMetro.FloydWarshall(stationClient.Id, stationCuisinier.Id);
                        nomAlgo = "Floyd-Warshall";
                        break;
                    default:
                        Console.WriteLine("Choix invalide, utilisation de l'algorithme de Dijkstra par défaut.");
                        chemin = grapheMetro.Dijkstra(stationClient.Id, stationCuisinier.Id);
                        nomAlgo = "Dijkstra (par défaut)";
                        break;
                }
                
                Console.WriteLine("\nAlgorithme utilisé : " + nomAlgo);
                return chemin;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la recherche du chemin : " + ex.Message);
                return null;
            }
        }
    }
} 