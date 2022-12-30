using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [ValidateNever]
        public ICollection<CartItemProduct>? Products { get; set; }

        [DisplayName("Quantite")]
        public int Quantity { get; set; }

        public string AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        [ValidateNever]
        public AppUser AppUser { get; set; }
    }
}