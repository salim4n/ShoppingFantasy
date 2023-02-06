using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;
using ShoppingFantasy.ViewModels;
using Stripe.Checkout;
using System.Globalization;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace ShoppingFantasy.Pages
{
	[BindProperties]
	public class SommaireModel : PageModel
	{
		private readonly ApplicationDbContext _db;

		public SommaireModel(ApplicationDbContext db)
		{
			_db = db;
		}

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
			

			Shipping = await _db.ShippingServices.ToListAsync();
			ShoppingCartVM = shoppingCart;

		}




		public async Task<IActionResult> OnPostAsync()
		{
			string totalPrice;
			string message;

			int shippingServiceId = Convert.ToInt32(Request.Form["shipping-service-select"]);
			var relaisId = Request.Form["ParcelShopCode"];
			var relais = Request.Form["relais"];
			totalPrice = Request.Form["total-price"];

			if (totalPrice.IsNullOrEmpty())
			{
				TempData["Error"] = "Vous devez chosir un mode de livraison !";
				return RedirectToPage("Sommaire");
			}
			CultureInfo culture = new CultureInfo("en-US");
			decimal decimalPrice = Convert.ToDecimal(totalPrice, culture);

			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
			var shipping = await _db.FindAsync<ShippingService>(shippingServiceId);
			shipping.IsFree = IsShippingFree(decimalPrice, (decimal)shipping.FreeShippingAt);

			var shoppingCart = new ShoppingCartVM()
			{
				ListCart = await _db.ShoppingCarts.Include(s => s.Product).Where(s => s.ApplicationUserId == claim.Value).ToListAsync(),
				OrderHeader = new()
			};

			if (!relaisId.IsNullOrEmpty())
			{
				shoppingCart.OrderHeader.RelaisId = relais;
			}
			shoppingCart.OrderHeader.OrderDate = DateTime.Now;
			shoppingCart.OrderHeader.AppUserId = claim.Value;
			shoppingCart.OrderHeader.OrderTotal = decimalPrice;
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
			shoppingCart.OrderHeader.Carrier = shipping.Name;
			foreach (var cart in shoppingCart.ListCart)
			{
				cart.Price = GetTotalPrice(cart);

			}
			if (shipping.IsFree)
			{
				shoppingCart.OrderHeader.FreeShipping = true;
			}
			else
			{
				shoppingCart.OrderHeader.FreeShipping = false;
				shoppingCart.OrderHeader.ShippingPrice = (decimal?)shipping.Price;

			}

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
				SuccessUrl = $"https://shoppingfantasy20230205132818.azurewebsites.net/OrderConfirmation?id={shoppingCart.OrderHeader.Id}",
				CancelUrl = $"https://shoppingfantasy20230205132818.azurewebsites.net/Panier",
				Currency = "eur",
				CustomerEmail = applicationUser.Email,

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
							Images = (List<string>)item.Product.Picture,
						},
					},
					Quantity = item.Count,


				};

				options.LineItems.Add(sessionLineItem);
			}

			if (!shipping.IsFree)
			{
				options.LineItems.Add(new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(shipping.Price * 100),
						Currency = "eur",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = shipping.Name
						},
					},
					Quantity = 1
				});
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

		private bool IsShippingFree(decimal price, decimal freeAt)
		{
			if (price >= freeAt)
				return true;
			else
				return false;
		}

		//private decimal? ShippingFee(ShoppingCart sp )
		//{
		//	sp.Price = GetTotalPrice(sp);
		//	var check = IsShippingFree( sp.Price, freeAt );
		//}

	}
}
