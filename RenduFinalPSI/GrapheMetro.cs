using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Linq;

namespace RenduFinalPSI
{
    public class GrapheMetro
    {
        public List<Station> Stations { get; set; }
        public List<Arc> Arcs { get; set; }
        private Dictionary<int, Color> CouleursLignes { get; set; }

        public GrapheMetro()
        {
            Stations = new List<Station>();
            Arcs = new List<Arc>();
            InitialiserCouleursLignes();
            ChargerDonnees();
        }

        private void InitialiserCouleursLignes()
        {
            CouleursLignes = new Dictionary<int, Color>
            {
                { 1, Color.FromArgb(255, 0, 0) },      // Rouge
                { 2, Color.FromArgb(0, 0, 255) },      // Bleu
                { 3, Color.FromArgb(0, 128, 0) },      // Vert
                { 4, Color.FromArgb(255, 165, 0) },    // Orange
                { 5, Color.FromArgb(128, 0, 128) },    // Violet
                { 6, Color.FromArgb(0, 128, 128) },    // Turquoise
                { 7, Color.FromArgb(255, 0, 255) },    // Magenta
                { 8, Color.FromArgb(128, 128, 0) },    // Olive
                { 9, Color.FromArgb(0, 255, 0) },      // Lime
                { 10, Color.FromArgb(255, 255, 0) },   // Jaune
                { 11, Color.FromArgb(0, 255, 255) },   // Cyan
                { 12, Color.FromArgb(255, 128, 0) },   // Orange foncé
                { 13, Color.FromArgb(128, 0, 0) },     // Marron
                { 14, Color.FromArgb(0, 128, 0) }      // Vert foncé
            };
        }

        private int ConvertirNumeroLigne(string numeroLigne)
        {
            if (numeroLigne.EndsWith("bis"))
            {
                string numeroBase = numeroLigne.Replace("bis", "");
                return int.Parse(numeroBase) + 100; // On ajoute 100 pour les lignes bis
            }
            return int.Parse(numeroLigne);
        }

        private void ChargerDonnees()
        {
            // Charger les stations
            string[] lignesStations = File.ReadAllLines("MetroParis(Noeuds).csv");
            for (int i = 1; i < lignesStations.Length; i++)
            {
                string[] colonnes = lignesStations[i].Split(';');
                if (colonnes.Length >= 7)
                {
                    try
                    {
                        int id = int.Parse(colonnes[0]);
                        int ligne = ConvertirNumeroLigne(colonnes[1]);
                        string nom = colonnes[2];
                        double longitude = double.Parse(colonnes[3], CultureInfo.InvariantCulture);
                        double latitude = double.Parse(colonnes[4], CultureInfo.InvariantCulture);
                        Stations.Add(new Station(id, nom, longitude, latitude, ligne));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur lors du chargement de la station : " + ex.Message);
                    }
                }
            }

            // Charger les arcs
            string[] lignesArcs = File.ReadAllLines("MetroParis(Arcs).csv");
            for (int i = 1; i < lignesArcs.Length; i++)
            {
                string[] colonnes = lignesArcs[i].Split(';');
                if (colonnes.Length >= 6)
                {
                    try
                    {
                        int depart = int.Parse(colonnes[0]);
                        int arrivee = int.Parse(colonnes[3]);
                        int tempsTrajet = int.Parse(colonnes[4]);
                        int tempsChangement = int.Parse(colonnes[5]);
                        Arcs.Add(new Arc(depart, arrivee, tempsTrajet, tempsChangement));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Erreur lors du chargement de l'arc : " + ex.Message);
                    }
                }
            }

            Console.WriteLine("Nombre de stations chargées : " + Stations.Count);
            Console.WriteLine("Nombre d'arcs chargés : " + Arcs.Count);
        }

        public List<Station> Dijkstra(int departId, int arriveeId)
        {
            /// cherche le plus court chemin entre 2 stations
            var stationDepart = Stations.FirstOrDefault(s => s.Id == departId);
            var stationArrivee = Stations.FirstOrDefault(s => s.Id == arriveeId);

            if (stationDepart == null || stationArrivee == null)
            {
                Console.WriteLine("station pas trouvee");
                return null;
            }

            Dictionary<Station, double> distances = new Dictionary<Station, double>();
            Dictionary<Station, Station> predecesseurs = new Dictionary<Station, Station>();
            List<Station> aVisiter = new List<Station>();

            // init des distances
            foreach (var station in Stations)
            {
                distances[station] = double.MaxValue;
                aVisiter.Add(station);
            }
            distances[stationDepart] = 0;

            while (aVisiter.Count > 0)
            {
                // prendre la station la plus proche
                Station stationCourante = aVisiter.OrderBy(s => distances[s]).First();
                aVisiter.Remove(stationCourante);

                // regarder les voisins
                var arcsVoisins = Arcs.Where(a => a.StationDepart == stationCourante.Id);
                foreach (var arc in arcsVoisins)
                {
                    Station stationVoisine = Stations.FirstOrDefault(s => s.Id == arc.StationArrivee);
                    if (stationVoisine != null && aVisiter.Contains(stationVoisine))
                    {
                        double nouveauTemps = distances[stationCourante] + 3; // 3 minutes entre stations
                        if (stationVoisine.Ligne != stationCourante.Ligne)
                        {
                            nouveauTemps += 2; // 2 minutes pour changement de ligne
                        }

                        if (nouveauTemps < distances[stationVoisine])
                        {
                            distances[stationVoisine] = nouveauTemps;
                            predecesseurs[stationVoisine] = stationCourante;
                        }
                    }
                }

                // regarder les changements de ligne possibles
                var stationsMemeEndroit = Stations.Where(s => 
                    s.Latitude == stationCourante.Latitude && 
                    s.Longitude == stationCourante.Longitude && 
                    s.Id != stationCourante.Id);

                foreach (var stationChangement in stationsMemeEndroit)
                {
                    if (aVisiter.Contains(stationChangement))
                    {
                        double nouveauTemps = distances[stationCourante] + 2; // 2 minutes pour changement de ligne
                        if (nouveauTemps < distances[stationChangement])
                        {
                            distances[stationChangement] = nouveauTemps;
                            predecesseurs[stationChangement] = stationCourante;
                        }
                    }
                }
            }

            // Afficher le temps total du trajet
            if (distances[stationArrivee] != double.MaxValue)
            {
                Console.WriteLine("Temps total du trajet : " + distances[stationArrivee] + " minutes");
            }

            return ReconstruireChemin(predecesseurs, stationDepart, stationArrivee);
        }

        public List<Station> BellmanFord(int departId, int arriveeId)
        {
            /// cherche le plus court chemin avec bellman ford
            var stationDepart = Stations.FirstOrDefault(s => s.Id == departId);
            var stationArrivee = Stations.FirstOrDefault(s => s.Id == arriveeId);

            if (stationDepart == null || stationArrivee == null)
            {
                Console.WriteLine("station pas trouvee");
                return null;
            }

            Dictionary<Station, double> distances = new Dictionary<Station, double>();
            Dictionary<Station, Station> predecesseurs = new Dictionary<Station, Station>();

            // init
            foreach (var station in Stations)
            {
                distances[station] = double.MaxValue;
            }
            distances[stationDepart] = 0;

            // relacher les arcs
            for (int i = 0; i < Stations.Count - 1; i++)
            {
                bool changement = false;

                // arcs normaux
                foreach (var arc in Arcs)
                {
                    Station stationDepartArc = Stations.FirstOrDefault(s => s.Id == arc.StationDepart);
                    Station stationArriveeArc = Stations.FirstOrDefault(s => s.Id == arc.StationArrivee);

                    if (stationDepartArc != null && stationArriveeArc != null)
                    {
                        double nouveauTemps = distances[stationDepartArc] + 3; // 3 minutes entre stations
                        if (stationArriveeArc.Ligne != stationDepartArc.Ligne)
                        {
                            nouveauTemps += 2; // 2 minutes pour changement de ligne
                        }

                        if (distances[stationDepartArc] != double.MaxValue && nouveauTemps < distances[stationArriveeArc])
                        {
                            distances[stationArriveeArc] = nouveauTemps;
                            predecesseurs[stationArriveeArc] = stationDepartArc;
                            changement = true;
                        }
                    }
                }

                // changements de ligne
                foreach (var station in Stations)
                {
                    var stationsMemeEndroit = Stations.Where(s => 
                        s.Latitude == station.Latitude && 
                        s.Longitude == station.Longitude && 
                        s.Id != station.Id);

                    foreach (var stationChangement in stationsMemeEndroit)
                    {
                        double nouveauTemps = distances[station] + 2; // 2 minutes pour changement de ligne
                        if (distances[station] != double.MaxValue && nouveauTemps < distances[stationChangement])
                        {
                            distances[stationChangement] = nouveauTemps;
                            predecesseurs[stationChangement] = station;
                            changement = true;
                        }
                    }
                }

                if (!changement) break;
            }

            // Afficher le temps total du trajet
            if (distances[stationArrivee] != double.MaxValue)
            {
                Console.WriteLine("Temps total du trajet : " + distances[stationArrivee] + " minutes");
            }

            return ReconstruireChemin(predecesseurs, stationDepart, stationArrivee);
        }

        public List<Station> FloydWarshall(int departId, int arriveeId)
        {
            /// cherche tous les plus courts chemins
            var stationDepart = Stations.FirstOrDefault(s => s.Id == departId);
            var stationArrivee = Stations.FirstOrDefault(s => s.Id == arriveeId);

            if (stationDepart == null || stationArrivee == null)
            {
                Console.WriteLine("station pas trouvee");
                return null;
            }

            int n = Stations.Count;
            double[,] distances = new double[n, n];
            Station[,] suivants = new Station[n, n];

            // init
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                        distances[i, j] = 0;
                    else
                        distances[i, j] = double.MaxValue;
                }
            }

            // arcs directs
            foreach (var arc in Arcs)
            {
                int i = Stations.FindIndex(s => s.Id == arc.StationDepart);
                int j = Stations.FindIndex(s => s.Id == arc.StationArrivee);

                if (i >= 0 && j >= 0)
                {
                    distances[i, j] = 3; // 3 minutes entre stations
                    if (Stations[i].Ligne != Stations[j].Ligne)
                    {
                        distances[i, j] += 2; // 2 minutes pour changement de ligne
                    }
                    suivants[i, j] = Stations[j];
                }
            }

            // changements de ligne
            for (int i = 0; i < n; i++)
            {
                var stationsMemeEndroit = Stations.Where(s => 
                    s.Latitude == Stations[i].Latitude && 
                    s.Longitude == Stations[i].Longitude && 
                    s.Id != Stations[i].Id);

                foreach (var station in stationsMemeEndroit)
                {
                    int j = Stations.IndexOf(station);
                    distances[i, j] = 2; // 2 minutes pour changement de ligne
                    suivants[i, j] = station;
                }
            }

            // floyd warshall
            for (int k = 0; k < n; k++)
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (distances[i, k] != double.MaxValue && distances[k, j] != double.MaxValue)
                        {
                            double nouveauTemps = distances[i, k] + distances[k, j];
                            if (nouveauTemps < distances[i, j])
                            {
                                distances[i, j] = nouveauTemps;
                                suivants[i, j] = suivants[i, k];
                            }
                        }
                    }
                }
            }

            // reconstruction du chemin
            List<Station> chemin = new List<Station>();
            int depart = Stations.IndexOf(stationDepart);
            int arrivee = Stations.IndexOf(stationArrivee);

            if (distances[depart, arrivee] == double.MaxValue)
            {
                return null;
            }

            // Afficher le temps total du trajet
            Console.WriteLine("Temps total du trajet : " + distances[depart, arrivee] + " minutes");

            chemin.Add(stationDepart);
            while (depart != arrivee)
            {
                if (suivants[depart, arrivee] == null)
                    break;
                chemin.Add(suivants[depart, arrivee]);
                depart = Stations.IndexOf(suivants[depart, arrivee]);
            }

            return chemin;
        }

        private List<Station> ReconstruireChemin(Dictionary<Station, Station> predecesseurs, Station depart, Station arrivee)
        {
            var chemin = new List<Station>();
            var stationCourante = arrivee;

            while (stationCourante != null)
            {
                chemin.Insert(0, stationCourante);
                if (stationCourante.Id == depart.Id)
                    break;
                stationCourante = predecesseurs.ContainsKey(stationCourante) ? predecesseurs[stationCourante] : null;
            }

            return chemin;
        }

        public void DessinerGraphe(string cheminFichier, List<Station> cheminPlusCourt = null)
        {
            // Trouver les limites des coordonnées
            double minLong = double.MaxValue, maxLong = double.MinValue;
            double minLat = double.MaxValue, maxLat = double.MinValue;

            foreach (var station in Stations)
            {
                minLong = Math.Min(minLong, station.Longitude);
                maxLong = Math.Max(maxLong, station.Longitude);
                minLat = Math.Min(minLat, station.Latitude);
                maxLat = Math.Max(maxLat, station.Latitude);
            }

            // Ajouter une marge de 10%
            double margeLong = (maxLong - minLong) * 0.1;
            double margeLat = (maxLat - minLat) * 0.1;
            minLong -= margeLong;
            maxLong += margeLong;
            minLat -= margeLat;
            maxLat += margeLat;

            // Créer l'image avec des dimensions plus grandes
            int largeur = 3000;
            int hauteur = 3000;
            using (Bitmap bitmap = new Bitmap(largeur, hauteur))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    // Fond blanc
                    g.Clear(Color.White);

                    // Dessiner les arcs
                    foreach (var arc in Arcs)
                    {
                        var stationDepart = Stations.Find(s => s.Id == arc.StationDepart);
                        var stationArrivee = Stations.Find(s => s.Id == arc.StationArrivee);

                        if (stationDepart != null && stationArrivee != null)
                        {
                            int x1 = (int)((stationDepart.Longitude - minLong) / (maxLong - minLong) * largeur);
                            int y1 = (int)((stationDepart.Latitude - minLat) / (maxLat - minLat) * hauteur);
                            int x2 = (int)((stationArrivee.Longitude - minLong) / (maxLong - minLong) * largeur);
                            int y2 = (int)((stationArrivee.Latitude - minLat) / (maxLat - minLat) * hauteur);

                            bool estDansChemin = cheminPlusCourt != null && 
                                cheminPlusCourt.Contains(stationDepart) && 
                                cheminPlusCourt.Contains(stationArrivee) &&
                                cheminPlusCourt.IndexOf(stationDepart) + 1 == cheminPlusCourt.IndexOf(stationArrivee);

                            Color couleurLigne = estDansChemin ? Color.Red : 
                                (CouleursLignes.ContainsKey(stationDepart.Ligne) ? 
                                CouleursLignes[stationDepart.Ligne] : Color.Gray);

                            using (Pen pen = new Pen(couleurLigne, estDansChemin ? 4 : 2))
                            {
                                g.DrawLine(pen, x1, y1, x2, y2);
                            }
                        }
                    }

                    // Dessiner les stations
                    foreach (var station in Stations)
                    {
                        int x = (int)((station.Longitude - minLong) / (maxLong - minLong) * largeur);
                        int y = (int)((station.Latitude - minLat) / (maxLat - minLat) * hauteur);

                        bool estDansChemin = cheminPlusCourt != null && cheminPlusCourt.Contains(station);
                        Color couleurLigne = estDansChemin ? Color.Red :
                            (CouleursLignes.ContainsKey(station.Ligne) ? 
                            CouleursLignes[station.Ligne] : Color.Gray);

                        // Dessiner le point coloré autour
                        using (SolidBrush brush = new SolidBrush(couleurLigne))
                        {
                            g.FillEllipse(brush, x - 6, y - 6, 12, 12);
                        }

                        // Dessiner le point noir central
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            g.FillEllipse(brush, x - 4, y - 4, 8, 8);
                        }

                        // Dessiner le nom de la station
                        using (Font font = new Font("Arial", 8))
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            g.DrawString(station.Nom, font, brush, x + 5, y - 5);
                        }
                    }
                }

                // Sauvegarder l'image
                bitmap.Save(cheminFichier);
            }
        }
    }
} 