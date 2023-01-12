using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.ViewModels;
using System.Security.Claims;

namespace ShoppingFantasy.Pages.MonPanier
{
    public class TestModel : PageModel
    {
        public readonly ApplicationDbContext _db;

		public TestModel(ApplicationDbContext db)
		{
			_db = db;
		}

        [BindProperty]
        public ShoppingCartVM Shopping { get; set; } = default!;

        public async Task OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM cartVM = new()
            {
                ListCart = await _db.ShoppingCarts
                    .Include(sc => sc.Product)
                    .ThenInclude(p => p.Picture)
                    .Where(sc => sc.ApplicationUserId == claim.Value)
                    .ToListAsync(),

                OrderHeader = new()
            };
          
        }
    }
}
