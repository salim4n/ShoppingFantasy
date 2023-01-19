using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ShoppingFantasy.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        [DisplayName("Article")]
        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage = "Choisissez une valeur comprise entre 1 et 1000")]
        [DisplayName("Nombre")]
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        [DisplayName("Utilisateur")]
        public AppUser AppUser { get; set; }

        [NotMapped]
        [DisplayName("Prix")]
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }
    }

    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> builder)
        {
            builder.Property(s => s.Id)
                   .ValueGeneratedOnAdd();

            builder.HasOne(s => s.Product)
                           .WithMany(p => p.ShoppingCarts)
                           .HasForeignKey(s => s.ProductId);
        }
    }


}
