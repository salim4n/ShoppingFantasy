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

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart cartFromDb = await _db.ShoppingCarts
                .FirstOrDefaultAsync(s => s.ApplicationUserId == claim.Value && s.ProductId == ShoppingCart.ProductId);

            if (cartFromDb == null)
            {
                var shoppingCart = new ShoppingCart
                {
                    ProductId = ShoppingCart.ProductId,
                    Count = ShoppingCart.Count,
                    ApplicationUserId = ShoppingCart.ApplicationUserId
                };

                await _db.ShoppingCarts.AddAsync(shoppingCart);

            }
            else
            {
                cartFromDb.Count += ShoppingCart.Count;
                //_db.ShoppingCarts.Update(cartFromDb);
                
            }


            await _db.SaveChangesAsync();
            TempData["success"] = $"{ShoppingCart.Count} article(s) ajoute(s) a votre panier .";
            return RedirectToPage("/Index");

        }
    }
}
