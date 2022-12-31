using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingFantasy.Models
{
    public class Picture
    {
        public int Id { get; set; }

        [DisplayName("Nom de l'image")]
        public string FileName { get; set; }

        [DisplayName("lien de l'image")]
        public string PictureUrl { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [ValidateNever]
        public Product Product { get; set; }
    }
}