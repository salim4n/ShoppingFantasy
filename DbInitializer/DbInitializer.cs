using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ShoppingFantasy.Data;
using ShoppingFantasy.Models;
using ShoppingFantasy.Utility;

namespace ShoppingFantasy.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _db;

		public DbInitializer(UserManager<IdentityUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ApplicationDbContext db)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_db = db;
		}

		public void Initialize()
		{
			try
			{
				if (_db.Database.GetPendingMigrations().Count() > 0)
				{
					_db.Database.Migrate();
				}
			}
			catch (Exception ex)
			{
				
			}

			if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Client)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.Role_Pro)).GetAwaiter().GetResult();

			}
			if(_db.AppUsers.Count() < 1) {
                _userManager.CreateAsync(new AppUser
                {
					
                    UserName = "laimeche160@gmail.com",
                    Email = "laimeche160@gmail.com",
                    Name = "Salim Laimeche",
                    PhoneNumber = "0749628470",
                    Address = "Secteur Troyes",
                    PostalCode = "10350",
                    City = "Los Santos"
                }, "Admin123*").GetAwaiter().GetResult();

                AppUser user = _db.AppUsers.FirstOrDefault(u => u.Email == "laimeche160@gmail.com");

                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }

			if(_db.ContactUs.Count() < 1) 
			{
				ContactUs contactUs = new ContactUs();
				_db.ContactUs.Add(contactUs);
				_db.SaveChanges();
			}

			if(_db.ShippingServices.Count() < 1)
			{
				ShippingService shippingService = new ShippingService()
				{
					Name = "Mondial Relay",
					Price = 5,
					FreeShippingAt = 30

				};

				_db.Add(shippingService);
				_db.SaveChanges();
			}

			return; 
		}

		
	}
}
