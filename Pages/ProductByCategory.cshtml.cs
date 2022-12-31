using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;

namespace ShoppingFantasy.Pages
{
    public class ProductByCategoryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ProductByCategoryModel(ApplicationDbContext db)
        {
            _db = db;
        }


        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _db.Categories == null) return NotFound();

            var category = await _db.Categories.Include(c => c.Products)
                                               .ThenInclude(p => p.Picture)
                                               .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null) return NotFound();
            else Category = category;

            return Page();
        }
    }
}
