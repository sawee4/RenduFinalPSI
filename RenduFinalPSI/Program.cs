using System;
using System.Collections.Generic;
using System.Linq;

namespace RenduFinalPSI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chargement du graphe du métro parisien...");
            GrapheMetro grapheMetro = new GrapheMetro();

            while (true)
            {
                Console.WriteLine("\n=== Menu Principal ===");
                Console.WriteLine("1. Afficher le graphe du métro");
                Console.WriteLine("2. Calculer le plus court chemin");
                Console.WriteLine("3. Quitter");
                Console.Write("Choix : ");

                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        grapheMetro.DessinerGraphe("metro_paris.png");
                        Console.WriteLine("Le graphe a été sauvegardé dans metro_paris.png");
                        break;

                    case "2":
                        Console.WriteLine("\n=== Calcul du plus court chemin ===");
                        Console.WriteLine("1. Dijkstra");
                        Console.WriteLine("2. Bellman-Ford");
                        Console.WriteLine("3. Floyd-Warshall");
                        Console.Write("Choix de l'algorithme : ");

                        string choixAlgo = Console.ReadLine();

                        // Afficher la liste des stations
                        Console.WriteLine("\nListe des stations disponibles :");
                        for (int i = 0; i < grapheMetro.Stations.Count; i++)
                        {
                            Console.WriteLine(i + 1 + ". " + grapheMetro.Stations[i].Nom);
                        }

                        Console.Write("\nNuméro de la station de départ : ");
                        if (int.TryParse(Console.ReadLine(), out int departIndex) && departIndex > 0 && departIndex <= grapheMetro.Stations.Count)
                        {
                            Console.Write("Numéro de la station d'arrivée : ");
                            if (int.TryParse(Console.ReadLine(), out int arriveeIndex) && arriveeIndex > 0 && arriveeIndex <= grapheMetro.Stations.Count)
                            {
                                var stationDepart = grapheMetro.Stations[departIndex - 1];
                                var stationArrivee = grapheMetro.Stations[arriveeIndex - 1];

                                List<Station> cheminPlusCourt = null;
                                string nomAlgo = "";

                                switch (choixAlgo)
                                {
                                    case "1":
                                        cheminPlusCourt = grapheMetro.Dijkstra(stationDepart.Id, stationArrivee.Id);
                                        nomAlgo = "Dijkstra";
                                        break;
                                    case "2":
                                        cheminPlusCourt = grapheMetro.BellmanFord(stationDepart.Id, stationArrivee.Id);
                                        nomAlgo = "Bellman-Ford";
                                        break;
                                    case "3":
                                        cheminPlusCourt = grapheMetro.FloydWarshall(stationDepart.Id, stationArrivee.Id);
                                        nomAlgo = "Floyd-Warshall";
                                        break;
                                    default:
                                        Console.WriteLine("Choix d'algorithme invalide");
                                        continue;
                                }

                                if (cheminPlusCourt != null)
                                {
                                    Console.WriteLine($"\nChemin trouvé avec l'algorithme de {nomAlgo} :");
                                    for (int i = 0; i < cheminPlusCourt.Count; i++)
                                    {
                                        Console.Write(cheminPlusCourt[i].Nom);
                                        if (i < cheminPlusCourt.Count - 1)
                                            Console.Write(" -> ");
                                    }
                                    Console.WriteLine();

                                    grapheMetro.DessinerGraphe("metro_paris_chemin.png", cheminPlusCourt);
                                    Console.WriteLine("Le graphe avec le chemin a été sauvegardé dans metro_paris_chemin.png");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Numéro de station d'arrivée invalide");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Numéro de station de départ invalide");
                        }
                        break;

                    case "3":
                        return;

                    default:
                        Console.WriteLine("Choix invalide");
                        break;
                }
            }
        }
    }
}
