using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
using System.Security.Claims;

namespace ShoppingFantasy.Pages
{
	
	public class SommaireModel : PageModel
    {
        private readonly ApplicationDbContext _db;

		public SommaireModel(ApplicationDbContext db)
		{
			_db = db;
		}

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; } = default!;
		public int OrderTotal { get; set; }
		public List<ShippingService> Shipping { get; set; } = default!;

		public async Task OnGetAsync()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var shipping = await _db.ShippingServices.ToListAsync();
			Shipping = shipping;
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
			shoppingCart.OrderHeader.PhoneNumber = shoppingCart.OrderHeader.AppUser.PhoneNumber;

			foreach (var cart in shoppingCart.ListCart)
			{
				cart.Price = GetTotalPrice(cart);
				shoppingCart.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			if (shoppingCart.OrderHeader.OrderTotal > SD.ShippingFreeCost)
			{
				shoppingCart.OrderHeader.FreeShipping = true;
			}
			else
			{
				shoppingCart.OrderHeader.FreeShipping = false;
			}

			Shipping = await _db.ShippingServices.ToListAsync();
            ShoppingCartVM = shoppingCart;


			
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


			shoppingCart.OrderHeader.OrderDate = DateTime.Now;
			shoppingCart.OrderHeader.AppUserId = claim.Value;

			foreach (var cart in shoppingCart.ListCart)
			{
				cart.Price = GetTotalPrice(cart);
				shoppingCart.OrderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			if (shoppingCart.OrderHeader.OrderTotal > SD.ShippingFreeCost)
			{
				shoppingCart.OrderHeader.FreeShipping = true;
			}
			else
			{
				shoppingCart.OrderHeader.FreeShipping = false;
			}
			AppUser applicationUser = await _db.AppUsers.FirstOrDefaultAsync(u => u.Id == claim.Value);

			shoppingCart.OrderHeader.PaymentStatus = SD.PaiementStatusAttente;
			shoppingCart.OrderHeader.OrderStatus = SD.StatusAttente;
			shoppingCart.OrderHeader.AppUserId = applicationUser.Id;
			shoppingCart.OrderHeader.City = ShoppingCartVM.OrderHeader.City;
			shoppingCart.OrderHeader.Name = ShoppingCartVM.OrderHeader.Name;
			shoppingCart.OrderHeader.SurName = ShoppingCartVM.OrderHeader.SurName;
			shoppingCart.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.StreetAddress;
			shoppingCart.OrderHeader.AddressComplement = ShoppingCartVM.OrderHeader.AddressComplement;
			shoppingCart.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.PostalCode;
			shoppingCart.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber;
			shoppingCart.OrderHeader.Carrier = ShoppingCartVM.OrderHeader.Carrier;

			await _db.OrderHeaders.AddAsync(shoppingCart.OrderHeader);
			await _db.SaveChangesAsync();

			foreach (var cart in shoppingCart.ListCart)
			{
				OrderDetails orderDetails = new()
				{
					ProductId = cart.ProductId,
					OrderId = shoppingCart.OrderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};

				await _db.OrderDetails.AddAsync(orderDetails);
				await _db.SaveChangesAsync();
			}


			//stripe session
			var domain = "https://localhost:7138";
			var options = new SessionCreateOptions()
			{
				PaymentMethodTypes = new List<string>

				{
					"card",
				},
				LineItems = new List<SessionLineItemOptions>()
				,
				Mode = "payment",
				SuccessUrl = domain + $"/OrderConfirmation?id={shoppingCart.OrderHeader.Id}",
				CancelUrl = domain + $"/Panier",
				
			};

			foreach (var item in shoppingCart.ListCart)
			{

				var sessionLineItem = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Price * 100),
						Currency = "eur",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
						},
					},
					Quantity = item.Count,
				};

				options.LineItems.Add(sessionLineItem);
			}

			var service = new SessionService();
			Session session = service.Create(options);
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(o => o.Id == shoppingCart.OrderHeader.Id);
			orderFromDb.PaymentDate = DateTime.Now;
			orderFromDb.SessionId = session.Id;
			orderFromDb.PaymentIntentId = session.PaymentIntentId;
			_db.SaveChanges();
			Response.Headers.Add("Location", session.Url);

			ShoppingCartVM = shoppingCart;
			return new StatusCodeResult(303);
		}

		private decimal GetTotalPrice(ShoppingCart sp)
		{
			decimal productPrice;

			if (sp.Product.IsInPromo)
				productPrice = sp.Product.PromoPrice;
			else
				productPrice = sp.Product.Price;
			return productPrice;
		}

    }
}
