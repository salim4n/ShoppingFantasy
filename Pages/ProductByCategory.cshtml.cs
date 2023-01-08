using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using System.Security.Claims;

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
        //public async Task<IActionResult> OnPostAsync(int id)
        //{
        //    var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


        //    if ( product == null)
        //    {
        //        TempData["error"] = "un probleme est survenue, actualisez la page et re-essayez s'il vous plait.";
        //        return Page();
        //    }
        //    else
        //    {
        //    }
        //}
    }
}
