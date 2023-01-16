using ShoppingFantasy.Models;

namespace ShoppingFantasy.ViewModels
{
    public class OrderVM
    {
        public OrderHeader? OrderHeader { get; set; }
        public IEnumerable<OrderDetails>? OrderDetails { get; set; }
    }
}
