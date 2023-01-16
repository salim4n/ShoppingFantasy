using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.ViewModels;

namespace ShoppingFantasy.Pages
{
    public class OrderDetailsModel : PageModel
    {
        public readonly ApplicationDbContext _db;

		public OrderDetailsModel(ApplicationDbContext db
            )
		{
			_db = db;
		}

        [BindProperty]
        public OrderVM OrderVM { get; set; } = default!;

        public async Task OnGet(int orderId)
        {
            OrderVM order = new()
            {
                OrderHeader = await _db.OrderHeaders.Include(oh => oh.AppUser).FirstOrDefaultAsync(oh => oh.Id == orderId),
                OrderDetails = await _db.OrderDetails.Include(oh => oh.Product).Where(oh => oh.OrderId == orderId).ToListAsync()
            };

            OrderVM = order;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return Page();
        }
    }
}
