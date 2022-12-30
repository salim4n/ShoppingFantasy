using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("Nom")]
        public string Name { get; set; }


        [DisplayName("Prix")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [DisplayName("Prix choc !")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PromoPrice { get; set; }

        [DisplayName("Ajouter une promotion ?")]
        public bool IsInPromo { get; set; } = false;
        public string Description { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public ICollection<Picture> Picture { get; set; }

        [ValidateNever]
        public ICollection<CartItemProduct> CartItems { get; set; }
    }
}
