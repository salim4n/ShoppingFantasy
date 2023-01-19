using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }

        [DisplayName("Prix Promo")]
        [Column(TypeName = "decimal(18,2)")]
		[DataType(DataType.Currency)]
		public decimal PromoPrice { get; set; }

        [DisplayName(" Promotion ?")]
        public bool IsInPromo { get; set; } = false;
        public string Description { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        [DisplayName("Categorie")]
        public Category Category { get; set; }

        [ValidateNever]
        [DisplayName("Images")]
        public ICollection<Picture> Picture { get; set; }

        [ValidateNever]
        [DisplayName("Paniers")]
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }


        //[ValidateNever]
        //[DisplayName("Panier")]
        //public ICollection<CartItemProduct> CartItems { get; set; }
    }
}
