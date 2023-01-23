using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
	public class ShippingService
	{
		public int Id { get; set; }

		[DisplayName("Nom")]
		public string Name { get; set; }

		[DataType(DataType.Currency)]
		[DisplayName("Prix")]
		public double Price { get; set; }

		[DataType(DataType.Currency)]
		[DisplayName("Prix de livraison gratuite")]
		public double FreeShippingAt { get; set; }

		[NotMapped]
		public bool IsFree { get; set; } = false;

	}
}
