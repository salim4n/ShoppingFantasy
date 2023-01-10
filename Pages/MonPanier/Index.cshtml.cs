using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using System.Security.Claims;

namespace ShoppingFantasy.Pages.MonPanier
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public ShoppingCartVM ShoppingCart { get; set; } = default!;

        public decimal ShippingPrice { get; } = SD.ShippingCost;

        public async Task<IActionResult> OnGet()
        {
            if (User.Identity.IsAuthenticated)
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

                foreach (var cart in cartVM.ListCart)
                {
                    cart.Price = GetTotalPrice(cart);
                    cartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
                }

                if (cartVM.OrderHeader.OrderTotal > (double)SD.ShippingFreeCost)
                {
                    cartVM.OrderHeader.FreeShipping = true;
                }
                else
                {
                    cartVM.OrderHeader.FreeShipping = false;
                    cartVM.OrderHeader.OrderTotal += (double)SD.ShippingCost;
                }

                ShoppingCart = cartVM;

                return Page();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

		public async Task<IActionResult> OnGetRemove(int cartId)
		{
			// Retrieve the item in the cart
			var item = _db.ShoppingCarts.FirstOrDefault(c => c.Id == cartId);

			// Remove the item from the cart and update the database
			_db.Remove(item);
			await _db.SaveChangesAsync();
			return RedirectToPage("Index");
		}


		private double GetTotalPrice(ShoppingCart sp)
        {
            decimal productPrice;

            if (sp.Product.IsInPromo)
                productPrice = sp.Product.PromoPrice;
            else
                productPrice = sp.Product.Price;
            return (double)productPrice;
        }



    }
}
