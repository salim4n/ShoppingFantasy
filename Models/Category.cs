using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;

namespace ShoppingFantasy.Models
{
    public class Category
    {
        public int Id { get; set; }

        [DisplayName("Nom")]
        public string Name { get; set; }

        [DisplayName("Photo representant la categorie d'article")]
        public string ImageUrl { get; set; }

        [ValidateNever]
        public ICollection<Product> Products { get; set; }
    }
}
