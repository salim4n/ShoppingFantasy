
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using Stripe.Checkout;
using System.Security.Claims;

namespace ShoppingFantasy.Pages
{

    public class PanierModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public PanierModel(ApplicationDbContext db)
        {
            _db = db;
        }

        
        public ShoppingCartVM ShoppingCart { get; set; }

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
                TempData["Error"] = "Vous devez etre connecté pour voir votre panier";
				return RedirectToPage("Index");
            }
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("Sommaire");
        }


        public async Task<IActionResult> OnGetRemove(int cartId)
		{
			// Retrieve the item in the cart
			var item = _db.ShoppingCarts.FirstOrDefault(c => c.Id == cartId);

			// Remove the item from the cart and update the database
			_db.Remove(item);
			await _db.SaveChangesAsync();
			return RedirectToPage("Panier");
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
