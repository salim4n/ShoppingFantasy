using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System.ComponentModel;

namespace ShoppingFantasy.Models
{
    public class AppUser : IdentityUser
    {
        [DisplayName("Nom")]
        [Required]
        public string? Name { get; set; }

        [DisplayName("Prenom")]
        [Required]
        public string? Surname { get; set; }


        [DisplayName("Addresse")]
        public string? Address { get; set; }

        [DisplayName("Complement d'addresse")]
        public string? AddressComplement { get; set; }

        [DisplayName("Ville")]
        public string? City { get; set; }

        [DisplayName("Code Postal")]
        public string? PostalCode { get; set; }

        [ValidateNever]
        public ICollection<CartItem>? CartItems { get; set; }

    }
}
