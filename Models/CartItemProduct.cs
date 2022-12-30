using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
    public class CartItemProduct
    {
        public int Id { get; set; }
        public int CartItemId { get; set; }

        [ForeignKey("CartItemId")]
        [ValidateNever]
        public CartItem CartItem { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
    }
}