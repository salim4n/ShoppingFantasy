using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        [DisplayName("Article")]
        public Product Product { get; set; }

        [DisplayName("Nombre")]
        public int Count { get; set; }

        [DisplayName("Prix")]
		[Column(TypeName = "decimal(18,2)")]
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }

    }
}
