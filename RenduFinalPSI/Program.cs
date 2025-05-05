using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenduFinalPSI
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("           BIENVENUE DANS LIV'IN PARIS            ");
            Console.WriteLine("==================================================");
            Console.WriteLine("Application de livraisons de repas");
            Console.WriteLine("==================================================");
            
            bool connecte = false;
            

            string choixConnexion = "";
            while (choixConnexion != "1" && choixConnexion != "2")
            {
                Console.WriteLine("\nSouhaitez-vous vous connecter à la base de données ?");
                Console.WriteLine("1. Oui");
                Console.WriteLine("2. Non (utiliser l'application sans base de données)");
                Console.Write("\nVotre choix : ");
                choixConnexion = Console.ReadLine();
                
                if (choixConnexion != "1" && choixConnexion != "2")
                {
                    Console.WriteLine("Choix invalide. Veuillez choisir 1 ou 2.");
                }
            }
            
            if (choixConnexion == "1")
            {
                connecte = ConnexionBaseDeDonnees();
            }
            
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                Console.WriteLine("==================================================");
                Console.WriteLine("                   MENU PRINCIPAL                  ");
                Console.WriteLine("==================================================");
                
                if (connecte)
                {
                    string typeUtilisateur = ConnexionBDD.GetTypeUtilisateur();
                    Console.WriteLine("Type d'utilisateur actuel : " + typeUtilisateur);
                    Console.WriteLine("==================================================");
                    
                    if (typeUtilisateur == "admin")
                    {
                        AfficherMenuAdmin();
                    }
                    else if (typeUtilisateur == "client")
                    {
                        Console.WriteLine("1. Mes Commandes");
                        Console.WriteLine("2. Mes Paiements");
                        Console.WriteLine("3. Mes Cuisiniers");
                        Console.WriteLine("4. Passer une commande");
                        Console.WriteLine("5. Noter mes commandes");
                        Console.WriteLine("6. Voir le plus court chemin vers un cuisinier");
                        Console.WriteLine("7. Retour");
                        Console.WriteLine("8. Quitter l'application");
                    }
                    else if (typeUtilisateur == "cuisinier")
                    {
                        Console.WriteLine("1. Mes Commandes");
                        Console.WriteLine("2. Mes Plats");
                        Console.WriteLine("3. Mes Clients");
                        Console.WriteLine("4. Ajouter un plat");
                        Console.WriteLine("5. Voir le plus court chemin vers un client");
                        Console.WriteLine("6. Statistiques");
                        Console.WriteLine("7. Retour");
                        Console.WriteLine("8. Quitter l'application");
                    }
                }
                else
                {
                    Console.WriteLine("1. Quitter l'application");
                }
                
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();
                
                if (connecte)
                {
                    string typeUtilisateur = ConnexionBDD.GetTypeUtilisateur();
                    
                    if (typeUtilisateur == "admin")
                    {
                        GererMenuAdmin();
                    }
                    else if (typeUtilisateur == "client")
                    {
                        switch (choix)
                        {
                            case "1":
                                MesCommandes();
                                break;
                            case "2":
                                MesPaiements();
                                break;
                            case "3":
                                MesCuisiniers();
                                break;
                            case "4":
                                PasserCommande();
                                break;
                            case "5":
                                NoterCommandes();
                                break;
                            case "6":
                                GestionClient gestionClient = new GestionClient();
                                gestionClient.AfficherPlusCourtChemin(ConnexionBDD.GetIdUtilisateur());
                                break;
                            case "7":
                                // Retour au menu précédent
                                break;
                            case "8":
                                continuer = false;
                                ConnexionBDD.SeDeconnecter();
                                Console.WriteLine("Au revoir !");
                                break;
                            default:
                                Console.WriteLine("Choix invalide. Veuillez réessayer.");
                                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                                Console.ReadKey();
                                break;
                        }
                    }
                    else if (typeUtilisateur == "cuisinier")
                    {
                        switch (choix)
                        {
                            case "1":
                                MesCommandesCuisinier();
                                break;
                            case "2":
                                MesPlats();
                                break;
                            case "3":
                                MesClients();
                                break;
                            case "4":
                                AjouterPlat();
                                break;
                            case "5":
                                GestionCuisinier gestionCuisinier = new GestionCuisinier();
                                gestionCuisinier.AfficherPlusCourtChemin(ConnexionBDD.GetIdUtilisateur());
                                break;
                            case "6":
                                GestionStatistiquesCuisinier stats = new GestionStatistiquesCuisinier();
                                stats.AfficherMenuStatistiques(ConnexionBDD.GetIdUtilisateur());
                                break;
                            case "7":
                                // Retour au menu précédent
                                break;
                            case "8":
                                continuer = false;
                                ConnexionBDD.SeDeconnecter();
                                Console.WriteLine("Au revoir !");
                                break;
                            default:
                                Console.WriteLine("Choix invalide. Veuillez réessayer.");
                                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                                Console.ReadKey();
                                break;
                        }
                    }
                }
                else
                {
                    switch (choix)
                    {
                        case "1":
                            continuer = false;
                            Console.WriteLine("Au revoir !");
                            break;
                        default:
                            Console.WriteLine("Choix invalide. Veuillez réessayer.");
                            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                            Console.ReadKey();
                            break;
                    }
                }
            }
        }
        
        static bool ConnexionBaseDeDonnees()
        {
            Console.Clear();
            Console.WriteLine("\n=== Connexion à la base de données ===");
            
            Console.WriteLine("\nQue souhaitez-vous faire ?");
            Console.WriteLine("1. Créer un compte");
            Console.WriteLine("2. Se connecter");
            Console.Write("\nVotre choix : ");
            
            string action = Console.ReadLine();
            
            if (action == "1")
            {
                Console.Clear();
                Console.WriteLine("\n=== Création d'un compte ===");

                Console.WriteLine("\nQuel type de compte souhaitez-vous créer ?");
                Console.WriteLine("1. Client");
                Console.WriteLine("2. Cuisinier");
                Console.Write("\nVotre choix : ");
                
                string typeUtilisateur = Console.ReadLine();
                string role = "";
                
                switch (typeUtilisateur)
                {
                    case "1":
                        role = "client";
                        break;
                    case "2":
                        role = "cuisinier";
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return false;
                }
                

                Console.Write("Nom : ");
                string nom = Console.ReadLine();
                Console.Write("Prénom : ");
                string prenom = Console.ReadLine();
                Console.Write("Email : ");
                string email = Console.ReadLine();
                Console.Write("Mot de passe : ");
                string mdp = Console.ReadLine();
                

                string adresse = "";
                string telephone = "";
                string specialite = "";
                string description = "";
                
                if (role == "client")
                {
                    Console.Write("Adresse : ");
                    adresse = Console.ReadLine();
                    Console.Write("Téléphone : ");
                    telephone = Console.ReadLine();
                }
                else if (role == "cuisinier")
                {
                    Console.Write("Spécialité culinaire : ");
                    specialite = Console.ReadLine();
                    Console.Write("Description : ");
                    description = Console.ReadLine();
                }
                
                if (ConnexionBDD.CreerCompte(nom, prenom, email, mdp, role, adresse, telephone, specialite, description))
                {
                    Console.WriteLine("\nCompte créé avec succès !");
                    Console.WriteLine("Type d'utilisateur : " + role);
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return true;
                }
                else
                {
                    Console.WriteLine("\nErreur lors de la création du compte.");
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                    return false;
                }
            }
            else if (action == "2")
            {
                Console.Clear();
                Console.WriteLine("\n=== Connexion ===");
                
                Console.WriteLine("\nQuel type d'utilisateur êtes-vous ?");
                Console.WriteLine("1. Client");
                Console.WriteLine("2. Cuisinier");
                Console.WriteLine("3. Administrateur");
                Console.Write("\nVotre choix : ");
                
                string typeUtilisateur = Console.ReadLine();
                string role = "";
                
                switch (typeUtilisateur)
                {
                    case "1":
                        role = "client";
                        break;
                    case "2":
                        role = "cuisinier";
                        break;
                    case "3":
                        role = "admin";
                        break;
                    default:
                        Console.WriteLine("Choix invalide.");
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return false;
                }
                
                Console.Write("Email : ");
                string email = Console.ReadLine();
                Console.Write("Mot de passe : ");
                string mdp = Console.ReadLine();
                
                if (ConnexionBDD.SeConnecter(email, mdp))
                {
                    if (ConnexionBDD.GetTypeUtilisateur() == role)
                    {
                        Console.WriteLine("\nConnexion réussie !");
                        Console.WriteLine("Type d'utilisateur : " + role);
                        
                        if (role == "admin")
                        {
                            Console.WriteLine("\nSouhaitez-vous créer un nouveau compte administrateur ?");
                            Console.WriteLine("1. Oui");
                            Console.WriteLine("2. Non");
                            Console.Write("\nVotre choix : ");
                            
                            string choixAdmin = Console.ReadLine();
                            if (choixAdmin == "1")
                            {
                                Console.Clear();
                                Console.WriteLine("\n=== Création d'un compte administrateur ===");
                                
                                Console.Write("Nom : ");
                                string nom = Console.ReadLine();
                                Console.Write("Prénom : ");
                                string prenom = Console.ReadLine();
                                Console.Write("Email : ");
                                string emailAdmin = Console.ReadLine();
                                Console.Write("Mot de passe : ");
                                string mdpAdmin = Console.ReadLine();
                                
                                if (ConnexionBDD.CreerCompte(nom, prenom, emailAdmin, mdpAdmin, "admin"))
                                {
                                    Console.WriteLine("\nCompte administrateur créé avec succès !");
                                }
                                else
                                {
                                    Console.WriteLine("\nErreur lors de la création du compte administrateur.");
                                }
                            }
                        }
                        
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("\nType d'utilisateur incorrect.");
                        ConnexionBDD.SeDeconnecter();
                    }
                }
                else
                {
                    Console.WriteLine("\nEmail ou mot de passe incorrect.");
                }
                
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
                return false;
            }
            else
            {
                Console.WriteLine("Choix invalide.");
                Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                Console.ReadKey();
                return false;
            }
        }
        
        static void AfficherMenuAdmin()
        {
            GestionAdmin gestionAdmin = new GestionAdmin();
            gestionAdmin.MenuPrincipal();
        }

        static void GererMenuAdmin()
        {
            bool continuer = true;
            while (continuer)
            {
                Console.Clear();
                AfficherMenuAdmin();
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        GestionClients();
                        break;
                    case "2":
                        GestionCuisiniers();
                        break;
                    case "3":
                        GestionCommandes();
                        break;
                    case "4":
                        Statistiques();
                        break;
                    case "5":
                        continuer = false;
                        ConnexionBDD.SeDeconnecter();
                        Console.WriteLine("Au revoir !");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Veuillez réessayer.");
                        Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                        Console.ReadKey();
                        break;
                }
            }
        }
        
        static void GestionClients()
        {
            Console.Clear();
            GestionAdmin gestionAdmin = new GestionAdmin();
            gestionAdmin.AfficherClients();
        }
        
        static void GestionCuisiniers()
        {
            Console.Clear();
            GestionAdmin gestionAdmin = new GestionAdmin();
            gestionAdmin.AfficherCuisiniers();
        }
        
        static void GestionCommandes()
        {
            Console.Clear();
            GestionAdmin gestionAdmin = new GestionAdmin();
            gestionAdmin.AfficherCommandes();
        }
        
        static void Statistiques()
        {
            Console.Clear();
            GestionAdmin gestionAdmin = new GestionAdmin();
            gestionAdmin.AfficherStatistiques();
        }
        
        static void ItinerairesMetro()
        {
            Console.Clear();
            string typeUtilisateur = ConnexionBDD.GetTypeUtilisateur();
            int idUtilisateur = ConnexionBDD.GetIdUtilisateur();

            if (typeUtilisateur == "client")
            {
                GestionClient gestionClient = new GestionClient();
                gestionClient.AfficherPlusCourtChemin(idUtilisateur);
            }
            else if (typeUtilisateur == "cuisinier")
            {
                GestionCuisinier gestionCuisinier = new GestionCuisinier();
                gestionCuisinier.AfficherPlusCourtChemin(idUtilisateur);
            }
        }

        static void MesCommandes()
        {
            Console.Clear();
            GestionClient gestionClient = new GestionClient();
            gestionClient.AfficherMesCommandes(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void MesPaiements()
        {
            Console.Clear();
            GestionClient gestionClient = new GestionClient();
            gestionClient.AfficherMesPaiements(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void MesCuisiniers()
        {
            Console.Clear();
            GestionClient gestionClient = new GestionClient();
            gestionClient.AfficherMesCuisiniers(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void PasserCommande()
        {
            Console.Clear();
            GestionClient gestionClient = new GestionClient();
            gestionClient.PasserCommande(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void MesCommandesCuisinier()
        {
            Console.Clear();
            GestionCuisinier gestionCuisinier = new GestionCuisinier();
            gestionCuisinier.AfficherMesCommandes(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void MesPlats()
        {
            Console.Clear();
            GestionCuisinier gestionCuisinier = new GestionCuisinier();
            gestionCuisinier.AfficherMesPlats(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void MesClients()
        {
            Console.Clear();
            GestionCuisinier gestionCuisinier = new GestionCuisinier();
            gestionCuisinier.AfficherMesClients(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void AjouterPlat()
        {
            Console.Clear();
            GestionCuisinier gestionCuisinier = new GestionCuisinier();
            gestionCuisinier.AjouterPlat(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        static void NoterCommandes()
        {
            Console.Clear();
            GestionClient gestionClient = new GestionClient();
            gestionClient.NoterCommandes(ConnexionBDD.GetIdUtilisateur());
            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
            Console.ReadKey();
        }

        public static void GererMenuCuisinier(int idUtilisateur)
        {
            GestionCuisinier gestionCuisinier = new GestionCuisinier();
            bool continuer = true;

            while (continuer)
            {
                Console.WriteLine("\n=== Menu Cuisinier ===");
                Console.WriteLine("1. Afficher mes commandes");
                Console.WriteLine("2. Afficher mes plats");
                Console.WriteLine("3. Ajouter un plat");
                Console.WriteLine("4. Passer une commande");
                Console.WriteLine("5. Quitter");
                Console.Write("\nVotre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        gestionCuisinier.AfficherMesCommandes(idUtilisateur);
                        break;
                    case "2":
                        gestionCuisinier.AfficherMesPlats(idUtilisateur);
                        break;
                    case "3":
                        gestionCuisinier.AjouterPlat(idUtilisateur);
                        break;
                    case "4":
                        gestionCuisinier.PasserCommande(idUtilisateur);
                        break;
                    case "5":
                        continuer = false;
                        ConnexionBDD.SeDeconnecter();
                        Console.WriteLine("Au revoir !");
                        break;
                    default:
                        Console.WriteLine("Choix invalide. Veuillez réessayer.");
                        break;
                }

                if (choix != "5")
                {
                    Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                    Console.ReadKey();
                }
            }
        }
    }
}


