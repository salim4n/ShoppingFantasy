using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.ViewModels;
using System.Security.Claims;

namespace ShoppingFantasy.Pages.MonPanier
{
	
	public class SummaryModel : PageModel
    {
        private readonly ApplicationDbContext _db;

		public SummaryModel(ApplicationDbContext db)
		{
			_db = db;
		}

		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; } = default!;
		public int OrderTotal { get; set; }

		public async Task<IActionResult> OnGetAsync()
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

			ShoppingCartVM = shoppingCart;

			return await Task.FromResult(Page());
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var shoppingCart = new ShoppingCartVM()
			{
				ListCart = await _db.ShoppingCarts.Include(s => s.Product).Where(s => s.ApplicationUserId == claim.Value).ToListAsync(),
			};

			ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.OrderHeader.AppUserId = claim.Value;
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
