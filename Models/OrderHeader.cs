﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        [ValidateNever]
        [DisplayName("Utilisateur")]
        public AppUser AppUser { get; set; }

        [Required]
        [DisplayName("Date de l'ordre")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Date d'envoie")]
        public DateTime ShippingDate { get; set; }

        [DisplayName("Livraison Gratuite")]
        public bool FreeShipping { get; set; } = false;

        [DisplayName("Prix de la Livraison")]
        public double ShippingPrice { get; set; } = 6;

        public int MyProperty { get; set; }

        [DisplayName("Total de l'ordre")]
        public double OrderTotal { get; set; }

        [DisplayName("Statut de l'ordre")]
        public string? OrderStatus { get; set; }

        [DisplayName("Etat du paiement")]
        public string? PaymentStatus { get; set; }

        [DisplayName("Numero de suivi")]
        public string? TrackingNumber { get; set; }

        [DisplayName("Entreprise de livraison")]
        public string? Carrier { get; set; }

        [DisplayName("Date de paiement")]
        public DateTime PaymentDate { get; set; }

        [DisplayName("Date de paiement differe")]
        public DateTime PaymentDueDate { get; set; }

        [DisplayName("Session Stripe")]
        public string? SessionId { get; set; }

        [DisplayName("Paiement Id")]
        public string? PaymentIntentId { get; set; }

        [Required]
        [DisplayName("Numero de telephone")]
        public string PhoneNumber { get; set; }

        [Required]
        [DisplayName("Addresse")]
        public string StreetAddress { get; set; }

        [Required]
        [DisplayName("Ville")]
        public string City { get; set; }

        [DisplayName("Complement d'addresse")]
        public string AddressComplement { get; set; }

        [Required]
        [DisplayName("Code Postal")]
        public string PostalCode { get; set; }

        [Required]
        [DisplayName("Nom")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Prenom")]
        public string SurName { get; set; }
    }
}