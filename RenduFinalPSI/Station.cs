using System;

namespace RenduFinalPSI
{
    public class Station
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int Ligne { get; set; }

        public Station(int id, string nom, double longitude, double latitude, int ligne)
        {
            Id = id;
            Nom = nom;
            Longitude = longitude;
            Latitude = latitude;
            Ligne = ligne;
        }
    }
} 