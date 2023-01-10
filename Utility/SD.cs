using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingFantasy.Utility
{
    public static class SD
    {
        //Role Utilisateur
        public const string Role_Admin = "Admin";
        public const string Role_Client = "Client";
        public const string Role_Pro = "Pro";


        //Statut des ordres
        public const string StatusAttente = "En Attente";
        public const string StatusApprouve = "Approuve";
        public const string StatusEnCours = "En Cours";
        public const string StatusEnvoye = "Envoye";
        public const string StatusAnnule = "Annule";
        public const string StatusRembourse = "Rembourse";


        //Statut des paiements
        public const string PaiementStatusAttente = "En Attente";
        public const string PaiementStatusApprouve = "Approuve";
        public const string PaiementStatusPaiementSousTrenteJour = "Approuve pour paiement sous 30 jours";
        public const string PaiementStatusRejet = "Paiement Rejete";


        //session stripe
        public const string SessionCart = "SessionShoppingCart";


        //livraison gratuite
        public const decimal ShippingFreeCost = 25;
        public const decimal ShippingCost = 5;
    }
}
