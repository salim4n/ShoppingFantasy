using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public const string StatusApprouve = "Approuvé";
        public const string StatusEnCours = "En Cours";
        public const string StatusEnvoye = "Envoyé";
        public const string StatusAnnule = "Annulé";
        public const string StatusRembourse = "Remboursé";


        //Statut des paiements
        public const string PaiementStatusAttente = "En Attente";
        public const string PaiementStatusApprouve = "Approuvé";
        public const string PaiementStatusPaiementSousTrenteJour = "Approuve pour paiement sous 30 jours";
        public const string PaiementStatusRejet = "Paiement Rejeté";


        //session stripe
        public const string SessionCart = "SessionShoppingCart";


    }
}
