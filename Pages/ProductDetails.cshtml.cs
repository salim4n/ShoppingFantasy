using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ProductDetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var product = await _db.Products.Include(p => p.Picture)
                .FirstOrDefaultAsync(p => p.Id == id);

            if(product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
            }
            return Page();
        }
    }
}
