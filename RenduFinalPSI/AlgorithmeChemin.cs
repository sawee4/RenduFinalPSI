using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace RenduFinalPSI
{
    public class AlgorithmeChemin
    {
        private List<Station> stations;
        private Dictionary<int, Dictionary<int, int>> graphe;

        public AlgorithmeChemin()
        {
            stations = new List<Station>();
            graphe = new Dictionary<int, Dictionary<int, int>>();
            ChargerGraphe();
        }

        private void ChargerGraphe()
        {
            try
            {
                MySqlConnection connection = ConnexionBDD.GetConnection();
                
                // Charger les stations
                string requeteStations = "SELECT * FROM STATION";
                MySqlCommand commandeStations = new MySqlCommand(requeteStations, connection);
                MySqlDataReader reader = commandeStations.ExecuteReader();

                while (reader.Read())
                {
                    Station station = new Station(
                        Convert.ToInt32(reader["id_station"]),
                        reader["nom"].ToString(),
                        Convert.ToDouble(reader["latitude"]),
                        Convert.ToDouble(reader["longitude"]),
                        Convert.ToInt32(reader["ligne"])
                    );
                    stations.Add(station);
                }
                reader.Close();

                // Charger les liens et construire le graphe
                string requeteLiens = "SELECT * FROM LIEN";
                MySqlCommand commandeLiens = new MySqlCommand(requeteLiens, connection);
                reader = commandeLiens.ExecuteReader();

                while (reader.Read())
                {
                    int depart = Convert.ToInt32(reader["station_depart"]);
                    int arrivee = Convert.ToInt32(reader["station_arrivee"]);
                    int temps = Convert.ToInt32(reader["temps"]);

                    if (!graphe.ContainsKey(depart))
                        graphe[depart] = new Dictionary<int, int>();
                    graphe[depart][arrivee] = temps;
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors du chargement du graphe : " + ex.Message);
            }
        }

        public List<Station> Dijkstra(int depart, int arrivee)
        {
            Dictionary<int, int> distance = new Dictionary<int, int>();
            Dictionary<int, int> precedent = new Dictionary<int, int>();
            List<int> nonVisites = new List<int>();

            foreach (var station in stations)
            {
                distance[station.Id] = int.MaxValue;
                precedent[station.Id] = -1;
                nonVisites.Add(station.Id);
            }

            distance[depart] = 0;

            while (nonVisites.Count > 0)
            {
                int u = nonVisites[0];
                foreach (var v in nonVisites)
                {
                    if (distance[v] < distance[u])
                        u = v;
                }
                nonVisites.Remove(u);

                if (u == arrivee)
                    break;

                if (graphe.ContainsKey(u))
                {
                    foreach (var voisin in graphe[u])
                    {
                        int v = voisin.Key;
                        int alt = distance[u] + voisin.Value;
                        if (alt < distance[v])
                        {
                            distance[v] = alt;
                            precedent[v] = u;
                        }
                    }
                }
            }

            return ReconstruireChemin(precedent, depart, arrivee);
        }

        public List<Station> BellmanFord(int depart, int arrivee)
        {
            Dictionary<int, int> distance = new Dictionary<int, int>();
            Dictionary<int, int> precedent = new Dictionary<int, int>();

            foreach (var station in stations)
            {
                distance[station.Id] = int.MaxValue;
                precedent[station.Id] = -1;
            }

            distance[depart] = 0;

            for (int i = 0; i < stations.Count - 1; i++)
            {
                foreach (var u in graphe.Keys)
                {
                    foreach (var v in graphe[u].Keys)
                    {
                        if (distance[u] != int.MaxValue && 
                            distance[u] + graphe[u][v] < distance[v])
                        {
                            distance[v] = distance[u] + graphe[u][v];
                            precedent[v] = u;
                        }
                    }
                }
            }

            return ReconstruireChemin(precedent, depart, arrivee);
        }

        public List<Station> FloydWarshall(int depart, int arrivee)
        {
            int n = stations.Count;
            int[,] distance = new int[n + 1, n + 1];
            int[,] precedent = new int[n + 1, n + 1];

            // Initialisation
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    distance[i, j] = int.MaxValue;
                    precedent[i, j] = -1;
                }
                distance[i, i] = 0;
            }

            // Remplissage des distances initiales
            foreach (var u in graphe.Keys)
            {
                foreach (var v in graphe[u].Keys)
                {
                    distance[u, v] = graphe[u][v];
                    precedent[u, v] = u;
                }
            }

            // Algorithme de Floyd-Warshall
            for (int k = 1; k <= n; k++)
            {
                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= n; j++)
                    {
                        if (distance[i, k] != int.MaxValue && distance[k, j] != int.MaxValue &&
                            distance[i, k] + distance[k, j] < distance[i, j])
                        {
                            distance[i, j] = distance[i, k] + distance[k, j];
                            precedent[i, j] = precedent[k, j];
                        }
                    }
                }
            }

            return ReconstruireCheminFloydWarshall(precedent, depart, arrivee);
        }

        private List<Station> ReconstruireChemin(Dictionary<int, int> precedent, int depart, int arrivee)
        {
            List<Station> chemin = new List<Station>();
            int current = arrivee;

            while (current != -1)
            {
                chemin.Insert(0, stations.Find(s => s.Id == current));
                current = precedent[current];
            }

            return chemin;
        }

        private List<Station> ReconstruireCheminFloydWarshall(int[,] precedent, int depart, int arrivee)
        {
            List<Station> chemin = new List<Station>();
            int current = arrivee;

            while (current != -1)
            {
                chemin.Insert(0, stations.Find(s => s.Id == current));
                current = precedent[depart, current];
            }

            return chemin;
        }

        public void AfficherChemin(int depart, int arrivee)
        {
            Console.WriteLine("\nChoisissez l'algorithme pour trouver le plus court chemin :");
            Console.WriteLine("1. Dijkstra");
            Console.WriteLine("2. Bellman-Ford");
            Console.WriteLine("3. Floyd-Warshall");
            Console.Write("\nVotre choix : ");
            string choix = Console.ReadLine();

            List<Station> chemin = null;
            string nomAlgo = "";

            switch (choix)
            {
                case "1":
                    chemin = Dijkstra(depart, arrivee);
                    nomAlgo = "Dijkstra";
                    break;
                case "2":
                    chemin = BellmanFord(depart, arrivee);
                    nomAlgo = "Bellman-Ford";
                    break;
                case "3":
                    chemin = FloydWarshall(depart, arrivee);
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
        }
    }
} 