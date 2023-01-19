﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
	}
}
