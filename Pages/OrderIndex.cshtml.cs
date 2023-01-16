using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using System.Security.Claims;

namespace ShoppingFantasy.Pages
{
    public class OrderIndexModel : PageModel
    {

        public readonly ApplicationDbContext _db;

        public OrderIndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public OrderVM OrderVM { get; set; } = default!;
        public IEnumerable<OrderHeader> OrderHeaders { get; set; } = default!;

        public async Task OnGet()
        {
            IEnumerable<OrderHeader> orderHeaders;

            if(User.IsInRole(SD.Role_Admin))
            {
                orderHeaders = await _db.OrderHeaders.Include(oh => oh.AppUser)
                    .ToListAsync();
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = await _db.OrderHeaders.Include(oh => oh.AppUser)
                    .Where(oh => oh.AppUserId == claim.Value)
                    .ToListAsync();
            }

            OrderHeaders = orderHeaders;
        }
    }
}
