
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

namespace ShoppingFantasy.Pages.MonPanier
{

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
				return RedirectToPage("Identity/Account/Login");
            }
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var shoppingCart = new ShoppingCartVM()
			{
				ListCart = await _db.ShoppingCarts.Include(s => s.Product).Where(s => s.ApplicationUserId == claim.Value).ToListAsync(),
				OrderHeader = new()

			};

			shoppingCart.OrderHeader.AppUser = await _db.AppUsers.FirstOrDefaultAsync(o => o.Id == claim.Value);

			shoppingCart.OrderHeader.Name = shoppingCart.OrderHeader.AppUser.Name;
			shoppingCart.OrderHeader.SurName = shoppingCart.OrderHeader.AppUser.Surname;
			shoppingCart.OrderHeader.StreetAddress = shoppingCart.OrderHeader.AppUser.Address;
			shoppingCart.OrderHeader.City = shoppingCart.OrderHeader.AppUser.City;
			shoppingCart.OrderHeader.PostalCode = shoppingCart.OrderHeader.AppUser.PostalCode;
			shoppingCart.OrderHeader.AddressComplement = shoppingCart.OrderHeader.AppUser.AddressComplement;

			foreach (var cart in shoppingCart.ListCart)
			{
				cart.Price = GetTotalPrice(cart);
				shoppingCart.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			//await _db.AddAsync(shoppingCart.OrderHeader);
			//await _db.SaveChangesAsync();
			ShoppingCart = shoppingCart;

			return Redirect("MonPanier/Test"/*, new {ShoppingCart = shoppingCart}*/);
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
