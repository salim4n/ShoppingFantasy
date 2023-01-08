using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using System.Security.Claims;

namespace ShoppingFantasy.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public ProductDetailsModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public ShoppingCart ShoppingCart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ShoppingCart cartObj = new()
            {
                Count = 1,
                ProductId = id,
                Product = await _db.Products
                            .Include(p => p.Category)
                            .Include(p => p.Picture)
                            .FirstOrDefaultAsync(p => p.Id == id)
            }; 
               
            ShoppingCart = cartObj;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(ShoppingCart sp)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            sp.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = await _db.ShoppingCarts.FirstOrDefaultAsync(s => s.ApplicationUserId == claim.Value && s.ProductId == sp.ProductId);

            if (cartFromDb == null)
            {
                await _db.ShoppingCarts.AddAsync(sp);
                //await _db.SaveChangesAsync();
            }
            else
            {
                cartFromDb.Count += sp.Count;
            }
      
                await _db.SaveChangesAsync();
                return RedirectToPage("/Index");

        }
    }
}
