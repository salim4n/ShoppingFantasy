using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages
{
    public class PromotionModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public PromotionModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Product> Products { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var productList = await _db.Products.Include(p => p.Picture)
                .Where(p => p.IsInPromo)
                .ToListAsync();

            if (productList == null || productList.Count == 0)
            {
                TempData["Cle"] = "Aucune promotion actuellement";
                return RedirectToPage("/index");
            }

            Products = productList;
            return Page();
        }
    }
}
