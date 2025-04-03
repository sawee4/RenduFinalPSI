using System;

namespace RenduFinalPSI
{
    public class Arc
    {
        public int StationDepart { get; set; }
        public int StationArrivee { get; set; }
        public int TempsTrajet { get; set; }
        public int TempsChangement { get; set; }

        public Arc(int depart, int arrivee, int tempsTrajet, int tempsChangement)
        {
            StationDepart = depart;
            StationArrivee = arrivee;
            TempsTrajet = tempsTrajet;
            TempsChangement = tempsChangement;
        }
    }
} 