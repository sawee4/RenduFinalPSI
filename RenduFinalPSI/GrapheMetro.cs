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
                { 1, Color.FromArgb(255, 0, 0) },    
                { 2, Color.FromArgb(0, 0, 255) },     
                { 3, Color.FromArgb(0, 128, 0) },      
                { 4, Color.FromArgb(255, 165, 0) },    
                { 5, Color.FromArgb(128, 0, 128) },    
                { 6, Color.FromArgb(0, 128, 128) },    
                { 7, Color.FromArgb(255, 0, 255) },    
                { 8, Color.FromArgb(128, 128, 0) },    
                { 9, Color.FromArgb(0, 255, 0) },      
                { 10, Color.FromArgb(255, 255, 0) },  
                { 11, Color.FromArgb(0, 255, 255) },   
                { 12, Color.FromArgb(255, 128, 0) },   
                { 13, Color.FromArgb(128, 0, 0) },     
                { 14, Color.FromArgb(0, 128, 0) }    
            };
        }

        private int ConvertirNumeroLigne(string numeroLigne)
        {
            if (numeroLigne.EndsWith("bis"))
            {
                string numeroBase = numeroLigne.Replace("bis", "");
                return int.Parse(numeroBase) + 100;
            }
            return int.Parse(numeroLigne);
        }

        private void ChargerDonnees()
        {

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

            foreach (var station in Stations)
            {
                distances[station] = double.MaxValue;
                aVisiter.Add(station);
            }
            distances[stationDepart] = 0;

            while (aVisiter.Count > 0)
            {
                Station stationCourante = aVisiter.OrderBy(s => distances[s]).First();
                aVisiter.Remove(stationCourante);

                // Si on a atteint la station d'arrivée, on peut s'arrêter
                if (stationCourante.Id == arriveeId)
                {
                    break;
                }

                // On regarde tous les arcs qui partent de la station courante
                var arcsSortants = Arcs.Where(a => a.StationDepart == stationCourante.Id);
                foreach (var arc in arcsSortants)
                {
                    Station stationVoisine = Stations.FirstOrDefault(s => s.Id == arc.StationArrivee);
                    if (stationVoisine != null && aVisiter.Contains(stationVoisine))
                    {
                        double nouveauTemps = distances[stationCourante] + arc.TempsTrajet;
                        
                        // Si on change de ligne, on ajoute le temps de changement
                        if (stationVoisine.Ligne != stationCourante.Ligne)
                        {
                            nouveauTemps += arc.TempsChangement;
                        }

                        if (nouveauTemps < distances[stationVoisine])
                        {
                            distances[stationVoisine] = nouveauTemps;
                            predecesseurs[stationVoisine] = stationCourante;
                        }
                    }
                }

                // On regarde aussi les arcs qui arrivent à la station courante
                var arcsEntrants = Arcs.Where(a => a.StationArrivee == stationCourante.Id);
                foreach (var arc in arcsEntrants)
                {
                    Station stationVoisine = Stations.FirstOrDefault(s => s.Id == arc.StationDepart);
                    if (stationVoisine != null && aVisiter.Contains(stationVoisine))
                    {
                        double nouveauTemps = distances[stationCourante] + arc.TempsTrajet;
                        
                        // Si on change de ligne, on ajoute le temps de changement
                        if (stationVoisine.Ligne != stationCourante.Ligne)
                        {
                            nouveauTemps += arc.TempsChangement;
                        }

                        if (nouveauTemps < distances[stationVoisine])
                        {
                            distances[stationVoisine] = nouveauTemps;
                            predecesseurs[stationVoisine] = stationCourante;
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

            foreach (var station in Stations)
            {
                distances[station] = double.MaxValue;
                predecesseurs[station] = null;
            }
            distances[stationDepart] = 0;
            predecesseurs[stationDepart] = stationDepart;

            for (int i = 0; i < Stations.Count - 1; i++)
            {
                bool changement = false;

                // Parcours des arcs dans les deux sens
                foreach (var arc in Arcs)
                {
                    // Sens normal
                    Station stationDepartArc = Stations.FirstOrDefault(s => s.Id == arc.StationDepart);
                    Station stationArriveeArc = Stations.FirstOrDefault(s => s.Id == arc.StationArrivee);

                    if (stationDepartArc != null && stationArriveeArc != null)
                    {
                        double nouveauTemps = distances[stationDepartArc] + arc.TempsTrajet;
                        if (stationArriveeArc.Ligne != stationDepartArc.Ligne)
                        {
                            nouveauTemps += arc.TempsChangement;
                        }

                        if (distances[stationDepartArc] != double.MaxValue && nouveauTemps < distances[stationArriveeArc])
                        {
                            distances[stationArriveeArc] = nouveauTemps;
                            predecesseurs[stationArriveeArc] = stationDepartArc;
                            changement = true;
                        }
                    }

                    // Sens inverse
                    if (stationDepartArc != null && stationArriveeArc != null)
                    {
                        double nouveauTemps = distances[stationArriveeArc] + arc.TempsTrajet;
                        if (stationDepartArc.Ligne != stationArriveeArc.Ligne)
                        {
                            nouveauTemps += arc.TempsChangement;
                        }

                        if (distances[stationArriveeArc] != double.MaxValue && nouveauTemps < distances[stationDepartArc])
                        {
                            distances[stationDepartArc] = nouveauTemps;
                            predecesseurs[stationDepartArc] = stationArriveeArc;
                            changement = true;
                        }
                    }
                }

                foreach (var station in Stations)
                {
                    var stationsMemeEndroit = Stations.Where(s => 
                        s.Latitude == station.Latitude && 
                        s.Longitude == station.Longitude && 
                        s.Id != station.Id);

                    foreach (var stationChangement in stationsMemeEndroit)
                    {
                        double nouveauTemps = distances[station] + 2; // temps de changement de ligne
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
            Station[,] predecesseurs = new Station[n, n];

            // Initialisation
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        distances[i, j] = 0;
                        predecesseurs[i, j] = Stations[i];
                    }
                    else
                    {
                        distances[i, j] = double.MaxValue;
                        predecesseurs[i, j] = null;
                    }
                }
            }

            // Remplissage des distances initiales dans les deux sens
            foreach (var arc in Arcs)
            {
                int i = Stations.FindIndex(s => s.Id == arc.StationDepart);
                int j = Stations.FindIndex(s => s.Id == arc.StationArrivee);

                if (i >= 0 && j >= 0)
                {
                    // Sens normal
                    distances[i, j] = arc.TempsTrajet;
                    if (Stations[i].Ligne != Stations[j].Ligne)
                    {
                        distances[i, j] += arc.TempsChangement;
                    }
                    predecesseurs[i, j] = Stations[i];

                    // Sens inverse
                    distances[j, i] = arc.TempsTrajet;
                    if (Stations[j].Ligne != Stations[i].Ligne)
                    {
                        distances[j, i] += arc.TempsChangement;
                    }
                    predecesseurs[j, i] = Stations[j];
                }
            }

            // Ajout des temps de changement de ligne pour les stations au même endroit
            for (int i = 0; i < n; i++)
            {
                var stationsMemeEndroit = Stations.Where(s => 
                    s.Latitude == Stations[i].Latitude && 
                    s.Longitude == Stations[i].Longitude && 
                    s.Id != Stations[i].Id);

                foreach (var station in stationsMemeEndroit)
                {
                    int j = Stations.IndexOf(station);
                    distances[i, j] = 2; // temps de changement de ligne
                    distances[j, i] = 2; // temps de changement de ligne dans l'autre sens
                    predecesseurs[i, j] = Stations[i];
                    predecesseurs[j, i] = Stations[j];
                }
            }

            // Algorithme de Floyd-Warshall
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
                                predecesseurs[i, j] = predecesseurs[k, j];
                            }
                        }
                    }
                }
            }

            // Reconstruction du chemin
            List<Station> chemin = new List<Station>();
            int depart = Stations.IndexOf(stationDepart);
            int arrivee = Stations.IndexOf(stationArrivee);

            if (distances[depart, arrivee] == double.MaxValue)
            {
                return null;
            }

            Console.WriteLine("Temps total du trajet : " + distances[depart, arrivee] + " minutes");

            chemin.Add(stationArrivee);
            int current = arrivee;
            while (current != depart)
            {
                if (predecesseurs[depart, current] == null)
                {
                    return null;
                }
                current = Stations.IndexOf(predecesseurs[depart, current]);
                chemin.Insert(0, Stations[current]);
            }

            return chemin;
        }

        private List<Station> ReconstruireChemin(Dictionary<Station, Station> predecesseurs, Station depart, Station arrivee)
        {
            var chemin = new List<Station>();
            var stationCourante = arrivee;

            // On commence par la station d'arrivée
            chemin.Add(stationCourante);

            // On remonte jusqu'à la station de départ
            while (stationCourante != null && stationCourante.Id != depart.Id)
            {
                if (!predecesseurs.ContainsKey(stationCourante) || predecesseurs[stationCourante] == null)
                {
                    Console.WriteLine("Erreur : pas de prédécesseur pour la station " + stationCourante.Nom);
                    return null;
                }
                stationCourante = predecesseurs[stationCourante];
                chemin.Insert(0, stationCourante);
            }

            // Vérification que le chemin est complet
            if (chemin.Count == 0 || chemin[0].Id != depart.Id)
            {
                Console.WriteLine("Erreur : chemin incomplet ou incorrect");
                return null;
            }

            return chemin;
        }

        public void DessinerGraphe(string cheminFichier, List<Station> cheminPlusCourt = null)
        {
            double minLong = double.MaxValue, maxLong = double.MinValue;
            double minLat = double.MaxValue, maxLat = double.MinValue;

            foreach (var station in Stations)
            {
                minLong = Math.Min(minLong, station.Longitude);
                maxLong = Math.Max(maxLong, station.Longitude);
                minLat = Math.Min(minLat, station.Latitude);
                maxLat = Math.Max(maxLat, station.Latitude);
            }

            double margeLong = (maxLong - minLong) * 0.1;
            double margeLat = (maxLat - minLat) * 0.1;
            minLong -= margeLong;
            maxLong += margeLong;
            minLat -= margeLat;
            maxLat += margeLat;

            int largeur = 3000;
            int hauteur = 3000;
            using (Bitmap bitmap = new Bitmap(largeur, hauteur))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.Clear(Color.White);

                    // Dessiner les arcs normaux
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

                            bool estDansChemin = false;
                            if (cheminPlusCourt != null)
                            {
                                for (int i = 0; i < cheminPlusCourt.Count - 1; i++)
                                {
                                    if ((cheminPlusCourt[i].Id == arc.StationDepart && cheminPlusCourt[i + 1].Id == arc.StationArrivee) ||
                                        (cheminPlusCourt[i].Id == arc.StationArrivee && cheminPlusCourt[i + 1].Id == arc.StationDepart))
                                    {
                                        estDansChemin = true;
                                        break;
                                    }
                                }
                            }

                            Color couleurLigne = estDansChemin ? Color.Blue : 
                                (CouleursLignes.ContainsKey(stationDepart.Ligne) ? 
                                CouleursLignes[stationDepart.Ligne] : Color.Gray);

                            using (Pen pen = new Pen(couleurLigne, estDansChemin ? 6 : 2))
                            {
                                g.DrawLine(pen, x1, y1, x2, y2);
                            }

                            // Debug: afficher les arcs du chemin
                            if (estDansChemin)
                            {
                                Console.WriteLine("Arc du chemin trouvé : " + stationDepart.Nom + " -> " + stationArrivee.Nom);
                            }
                        }
                    }

                    // Dessiner les stations
                    foreach (var station in Stations)
                    {
                        int x = (int)((station.Longitude - minLong) / (maxLong - minLong) * largeur);
                        int y = (int)((station.Latitude - minLat) / (maxLat - minLat) * hauteur);

                        bool estDansChemin = cheminPlusCourt != null && cheminPlusCourt.Contains(station);
                        bool estDepart = cheminPlusCourt != null && cheminPlusCourt.Count > 0 && station.Id == cheminPlusCourt[0].Id;
                        bool estArrivee = cheminPlusCourt != null && cheminPlusCourt.Count > 0 && station.Id == cheminPlusCourt[cheminPlusCourt.Count - 1].Id;

                        Color couleurLigne = estDansChemin ? Color.Blue :
                            (CouleursLignes.ContainsKey(station.Ligne) ? 
                            CouleursLignes[station.Ligne] : Color.Gray);

                        // Dessiner le cercle de la station
                        using (SolidBrush brush = new SolidBrush(couleurLigne))
                        {
                            g.FillEllipse(brush, x - 8, y - 8, 16, 16);
                        }

                        using (SolidBrush brush = new SolidBrush(Color.White))
                        {
                            g.FillEllipse(brush, x - 6, y - 6, 12, 12);
                        }

                        // Dessiner les icônes pour départ et arrivée
                        if (estDepart)
                        {
                            using (Pen pen = new Pen(Color.Green, 3))
                            {
                                g.DrawRectangle(pen, x - 10, y - 10, 20, 20);
                            }
                            // Ajouter le texte "Départ"
                            using (Font font = new Font("Arial", 10, FontStyle.Bold))
                            using (SolidBrush brush = new SolidBrush(Color.Green))
                            {
                                g.DrawString("Départ", font, brush, x + 15, y - 20);
                            }
                        }
                        else if (estArrivee)
                        {
                            using (Pen pen = new Pen(Color.Red, 3))
                            {
                                g.DrawEllipse(pen, x - 10, y - 10, 20, 20);
                            }
                            // Ajouter le texte "Arrivée"
                            using (Font font = new Font("Arial", 10, FontStyle.Bold))
                            using (SolidBrush brush = new SolidBrush(Color.Red))
                            {
                                g.DrawString("Arrivée", font, brush, x + 15, y - 20);
                            }
                        }

                        // Dessiner le nom de la station
                        using (Font font = new Font("Arial", 8))
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            g.DrawString(station.Nom, font, brush, x + 5, y - 5);
                        }
                    }
                }

                bitmap.Save(cheminFichier);
            }
        }
    }
} 