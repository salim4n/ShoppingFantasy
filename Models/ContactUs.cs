using System.ComponentModel;

namespace ShoppingFantasy.Models
{
    public class ContactUs
    {
        public int Id { get; set; }

        [DisplayName("Contactez nous")]
        public string Message { get; set; } = "Contactez nous";
    }
}
